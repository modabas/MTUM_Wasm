using MTUM_Wasm.Server.Core.Common.Extension;
using MTUM_Wasm.Server.Core.Database.Dto;
using MTUM_Wasm.Server.Core.Database.Entity;
using MTUM_Wasm.Server.Core.Database.Service;
using MTUM_Wasm.Server.Core.Identity.Service;
using MTUM_Wasm.Server.Infrastructure.Database.Postgres.Dapper.QueryParameter;
using MTUM_Wasm.Shared.Core.Common.Result;
using MTUM_Wasm.Shared.Core.Common.Utility;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Infrastructure.Database.Postgres.Service;

internal class AuditRepo : IAuditRepo
{
    private readonly IUserAccessor _userAccessor;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDbContext _dbContext;
    private readonly ILogger<AuditRepo> _logger;

    public AuditRepo(IUserAccessor userAccessor, IHttpContextAccessor httpContextAccessor, IDbContext dbContext, ILogger<AuditRepo> logger)
    {
        _userAccessor = userAccessor;
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
        _logger = logger;
    }

    private static readonly AsyncLocal<DbTransactionHolder> _dbTransactionCurrent = new AsyncLocal<DbTransactionHolder>();

    public DbTransaction? DbTransaction
    {
        get
        {
            return _dbTransactionCurrent.Value?.WrappedObject;
        }
        set
        {
            var holder = _dbTransactionCurrent.Value;
            if (holder != null)
            {
                // Clear current DbTransaction trapped in the AsyncLocals, as its done.
                holder.WrappedObject = null;
            }

            if (value != null)
            {
                // Use an object indirection to hold the DbTransaction in the AsyncLocal,
                // so it can be cleared in all ExecutionContexts when its cleared.
                _dbTransactionCurrent.Value = new DbTransactionHolder { WrappedObject = value };
            }
        }
    }

    private class DbTransactionHolder
    {
        public DbTransaction? WrappedObject;
    }

    private bool UseTransaction()
    {
        if (DbTransaction is null)
            return false;
        return true;
    }

    public async Task<AuditLogPrimaryKey> CreateEntry(AuditLogEntry entry, CancellationToken cancellationToken)
    {
        var currentUser = await _userAccessor.GetCurrentUser(cancellationToken);
        var controllerName = _httpContextAccessor.HttpContext?.Request.RouteValues["controller"]?.ToString() ?? string.Empty;
        var actionName = _httpContextAccessor.HttpContext?.Request.RouteValues["action"]?.ToString() ?? string.Empty;
        var commandName = $"{controllerName}/{actionName}";
        var remoteIpResult = _httpContextAccessor.GetRequestIP();
        var remoteIp = remoteIpResult.Succeeded ? remoteIpResult.Data ?? "unknown" : "unknown";
        var currentUserJsonString = JsonHelper.SerializeJson(currentUser);
        var entryJsonString = entry.ToJsonString();
        var query = "INSERT INTO tbl_audit_log (command_name, remote_ip, logged_in_user, entry) VALUES (@command_name, @remote_ip, @logged_in_user, @entry) RETURNING created_date, id;";

        if (UseTransaction())
        {
            //UseTransaction method does null check on dbTransaction
            var conn = DbTransaction!.Connection;
            if (conn is not null)
            {
                var ret = (await conn.QueryAsync<DateTime, long, AuditLogPrimaryKey>(new CommandDefinition(query,
                    new
                    {
                        command_name = commandName,
                        remote_ip = remoteIp,
                        logged_in_user = new JsonbParameter(currentUserJsonString),
                        entry = new JsonbParameter(entryJsonString)
                    }, DbTransaction, cancellationToken: cancellationToken),
                    (createdDate, id) =>
                    {
                        return new() { CreatedDate = createdDate, Id = id };
                    },
                    splitOn: "id")).Single();
                return ret;
            }
            else
                throw new ApplicationException("Cannot insert transactional audit log. Connection is null");
        }
        else
        {
            using (var conn = _dbContext.GetConnection())
            {
                var ret = (await conn.QueryAsync<DateTime, long, AuditLogPrimaryKey>(new CommandDefinition(query,
                    new
                    {
                        command_name = commandName,
                        remote_ip = remoteIp,
                        logged_in_user = new JsonbParameter(currentUserJsonString),
                        entry = new JsonbParameter(entryJsonString)
                    }, cancellationToken: cancellationToken),
                    (createdDate, id) => 
                    { 
                        return new() { CreatedDate = createdDate, Id = id };
                    },
                    splitOn: "id")).Single();
                return ret;
            }
        }
    }

    public async Task<IServiceResult<SearchAuditLogsOutput>> Search(SearchAuditLogsInput requestDto, CancellationToken cancellationToken)
    {
        var sql = "SELECT *, COUNT(1) OVER() AS total FROM tbl_audit_log WHERE created_date>=@start_date AND created_date<=@end_date";
        if (requestDto.TenantId is not null)
            sql += " AND tenant_id=@tenant_id";
        if (!string.IsNullOrWhiteSpace(requestDto.UserEmail))
            sql += " AND email=@email";
        if (!string.IsNullOrWhiteSpace(requestDto.CommandName))
            sql += " AND command_name ilike @command_name";

        var offset = (requestDto.Page - 1) * requestDto.PageSize;
        sql += $" ORDER BY created_at DESC LIMIT {requestDto.PageSize} OFFSET {offset};";

        var sqlParameters = new { 
            email = string.IsNullOrWhiteSpace(requestDto.UserEmail) ? null : NormalizeEmail(requestDto.UserEmail.Trim()), 
            tenant_id = requestDto.TenantId, 
            command_name = string.IsNullOrWhiteSpace(requestDto.CommandName) ? null : $"%{requestDto.CommandName.Trim()}%", 
            start_date = requestDto.StartDate, 
            end_date = requestDto.EndDate };

        HashSet<long> totalCount = new();
        using (var conn = _dbContext.GetConnection())
        {
            var logs = (await conn.QueryAsync<AuditLogEntity, long, AuditLogEntity>(
                new CommandDefinition(sql, sqlParameters, cancellationToken: cancellationToken),
                (result, count) =>
                {
                    totalCount.Add(count);
                    return result;
                },
                "total")).ToList().AsReadOnly();
            return Result<SearchAuditLogsOutput>.Success(new SearchAuditLogsOutput() { Logs = logs, TotalCount = totalCount.SingleOrDefault() });
        }
    }

    private string NormalizeEmail(string email)
    {
        return email.ToLower(CultureInfo.InvariantCulture);
    }

    public async Task<(bool, AuditLogPrimaryKey)> TryCreateEntry(AuditLogEntry entry, CancellationToken cancellationToken)
    {
        try
        {
            var entryPrimaryKey = await CreateEntry(entry, cancellationToken);
            return (true, entryPrimaryKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "TryCreateEntry encountered an error.");
            return (false, new());
        }
    }
}

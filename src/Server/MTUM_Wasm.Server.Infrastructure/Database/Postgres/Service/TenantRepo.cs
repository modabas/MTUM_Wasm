using MTUM_Wasm.Server.Core.Common.Entity;
using MTUM_Wasm.Server.Core.Common.Service;
using MTUM_Wasm.Server.Core.Database.Entity;
using MTUM_Wasm.Server.Core.Database.Extensions;
using MTUM_Wasm.Server.Core.Database.Service;
using MTUM_Wasm.Server.Core.SystemAdmin.Dto;
using MTUM_Wasm.Shared.Core.Common.Result;
using MTUM_Wasm.Shared.Core.Identity.Entity;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Infrastructure.Database.Postgres.Service;

internal class TenantRepo : ITenantRepo
{
    private readonly IDbContext _dbContext;
    private readonly IAuditRepo _auditRepo;

    public TenantRepo(IDbContext dbContext, IAuditRepo auditRepo)
    {
        _dbContext = dbContext;
        _auditRepo = auditRepo;
    }

    public async Task<IServiceResult<TenantEntity>> CreateTenant(string tenantName, CancellationToken cancellationToken)
    {
        using (var conn = _dbContext.GetConnection())
        {
            await conn.OpenAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            using (var tran = conn.BeginTransaction(_auditRepo))
            {
                var sql = "INSERT INTO tbl_tenant (name) VALUES (@name) RETURNING *;";
                var tenant = await conn.QuerySingleOrDefaultAsync<TenantEntity>(new CommandDefinition(sql, new { name = tenantName }, tran, cancellationToken: cancellationToken));
                cancellationToken.ThrowIfCancellationRequested();

                if (tenant is null)
                    return Result<TenantEntity>.Fail($"Cannot create tenant with name {tenantName}");

                await _auditRepo.CreateEntry(new AuditLogEntry(new AuditLogItem("new_tenant", tenant)), cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                await tran.CommitAsync(cancellationToken);
                return Result<TenantEntity>.Success(tenant);
            }
        }
    }

    public async Task<IServiceResult<IEnumerable<TenantEntity>>> GetTenants(CancellationToken cancellationToken)
    {
        using (var conn = _dbContext.GetConnection())
        {
            var sql = "SELECT * FROM tbl_tenant ORDER BY name_lower;";
            var tenants = (await conn.QueryAsync<TenantEntity>(new CommandDefinition(sql, cancellationToken: cancellationToken))).ToList().AsReadOnly();
            return Result<IEnumerable<TenantEntity>>.Success(tenants);
        }
    }

    public async Task<IServiceResult<TenantEntity>> GetTenant(Guid tenantId, CancellationToken cancellationToken)
    {
        using (var conn = _dbContext.GetConnection())
        {
            var sql = "SELECT * FROM tbl_tenant WHERE id = @id;";
            var tenant = await conn.QueryFirstOrDefaultAsync<TenantEntity>(new CommandDefinition(sql, new { id = tenantId }, cancellationToken: cancellationToken));
            if (tenant is null)
                return Result<TenantEntity>.Fail($"Cannot find tenant with id: {tenantId}");
            return Result<TenantEntity>.Success(tenant);
        }
    }

    public async Task<IServiceResult<TenantEntity>> UpdateTenant(TenantEntity tenant, CancellationToken cancellationToken)
    {
        using (var conn = _dbContext.GetConnection())
        {
            await conn.OpenAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            using (var tran = conn.BeginTransaction(_auditRepo))
            {
                //updates row and returns old value of row and new value of row in this order
                var sql = "WITH updated AS (UPDATE tbl_tenant SET name=@name, is_enabled=@is_enabled, last_modified_at=now() WHERE id=@id RETURNING *) " +
                            "SELECT * FROM tbl_tenant WHERE id=@id UNION ALL SELECT * FROM updated;";
                var tenants = (await conn.QueryAsync<TenantEntity>(new CommandDefinition(sql, new { id = tenant.Id, name = tenant.Name, is_enabled = tenant.IsEnabled }, tran, cancellationToken: cancellationToken))).ToArray();
                cancellationToken.ThrowIfCancellationRequested();

                if (tenants.Length == 2)
                {
                    await _auditRepo.CreateEntry(new AuditLogEntry(new AuditLogItem("old_tenant", tenants[0]), new AuditLogItem("new_tenant", tenants[1])), cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();

                    await tran.CommitAsync(cancellationToken);
                    //return new value
                    return Result<TenantEntity>.Success(tenants[1]);
                }
                return Result<TenantEntity>.Fail($"An error occured during updating tenant with id {tenant.Id}");
            }
        }
    }

    public async Task<IServiceResult<TenantEntity>> UpdateTenantNacPolicy(Guid tenantId, NacPolicy? nacPolicy, CancellationToken cancellationToken)
    {
        using (var conn = _dbContext.GetConnection())
        {
            await conn.OpenAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            using (var tran = conn.BeginTransaction(_auditRepo))
            {
                //updates row and returns old value of row and new value of row in this order
                var sql = "WITH updated AS (UPDATE tbl_tenant SET nac_policy=@nac_policy, last_modified_at=now() WHERE id=@id RETURNING *) " +
                            "SELECT * FROM tbl_tenant WHERE id=@id UNION ALL SELECT * FROM updated;";
                var tenants = (await conn.QueryAsync<TenantEntity>(new CommandDefinition(sql, new { id = tenantId, nac_policy = nacPolicy }, tran, cancellationToken: cancellationToken))).ToArray();
                cancellationToken.ThrowIfCancellationRequested();

                if (tenants.Length == 2)
                {
                    await _auditRepo.CreateEntry(new AuditLogEntry(new AuditLogItem("old_tenant", tenants[0]), new AuditLogItem("new_tenant", tenants[1])), cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();

                    await tran.CommitAsync(cancellationToken);
                    //return new value
                    return Result<TenantEntity>.Success(tenants[1]);
                }
                return Result<TenantEntity>.Fail($"An error occured during updating tenant with id {tenantId}");
            }
        }
    }
}

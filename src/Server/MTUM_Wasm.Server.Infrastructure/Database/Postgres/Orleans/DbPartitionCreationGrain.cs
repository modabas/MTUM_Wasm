using MTUM_Wasm.Server.Core.Database.Orleans;
using MTUM_Wasm.Server.Core.Database.Service;
using Dapper;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using System;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Infrastructure.Database.Postgres.Orleans;

internal class DbPartitionCreationGrain : Grain, IDbPartitionCreationGrain, IRemindable
{
    private bool _isBusy = false;
    private const string ReminderName = "DbPartitionCreationGrain_Postgres";

    private readonly ILogger<DbPartitionCreationGrain> _logger;
    private readonly IDbContext _dbContext;

    public DbPartitionCreationGrain(ILogger<DbPartitionCreationGrain> logger, IDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public Task<bool> IsBusy()
    {
        return Task.FromResult(_isBusy);
    }

    public Task Poke()
    {
        return Task.CompletedTask;
    }

    public override async Task OnActivateAsync()
    {
        try
        {
            //on activation, register self as reminder
            //activation is done by a seperate startup task
            await RegisterOrUpdateReminder(ReminderName, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(5));
            await base.OnActivateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OnActivateAsync failed");
            throw;
        }
    }

    public async Task ReceiveReminder(string reminderName, TickStatus status)
    {
        try
        {
            if (reminderName != ReminderName)
                return;
            if (_isBusy)
                return;
            _isBusy = true;

            await CreateAuditLogPartitions();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing {reminderName} reminder.", reminderName);
        }
        finally
        {
            _isBusy = false;
        }
    }

    private string GetCreatePartitionCommandForAuditLogTable(DateTime date)
    {
        //if not exists, create month long partitions on date from first day of current month to first day of next month
        var partitionTableName = $"tbl_audit_log_y{date.Year.ToString("0000")}_m{date.Month.ToString("00")}";
        var commandText =
                    "DO $$ " +
                    "BEGIN " +
                    $"IF NOT EXISTS(SELECT 1 FROM pg_tables WHERE schemaname = N'public' AND tablename = N'{partitionTableName}') THEN " +
                        $"CREATE TABLE public.{partitionTableName} PARTITION OF public.tbl_audit_log " +
                        $"FOR VALUES FROM ('{date.ToString("yyyy'-'MM")}-01') TO ('{date.AddMonths(1).ToString("yyyy'-'MM")}-01') " +
                        "TABLESPACE pg_default; " +
                    "END IF; " +
                    "END $$ ";

        return commandText;
    }

    private async Task<int> CreatePartitionForAuditLogTable(DateTime date)
    {
        using (var conn = _dbContext.GetConnection())
        {
            var sql = GetCreatePartitionCommandForAuditLogTable(date);
            return await conn.ExecuteAsync(new CommandDefinition(sql));
        }
    }

    private async Task CreateAuditLogPartitions()
    {
        try
        {
            var todayUtc = DateTime.UtcNow.Date;
            //create partition for this month and next month (to be safe if app server time and db server time are skewed)
            await Task.WhenAll(CreatePartitionForAuditLogTable(todayUtc), CreatePartitionForAuditLogTable(todayUtc.AddMonths(1)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating partitions for audit log table.");
        }
    }
}


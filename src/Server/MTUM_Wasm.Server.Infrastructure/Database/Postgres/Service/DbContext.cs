using Ardalis.SmartEnum.Dapper;
using MTUM_Wasm.Server.Core.Database.Entity;
using MTUM_Wasm.Server.Core.Database.Service;
using MTUM_Wasm.Server.Infrastructure.Database.Postgres.Dapper.Mapping;
using MTUM_Wasm.Server.Infrastructure.Database.Postgres.Dapper.TypeHandler;
using Dapper;
using Dapper.FluentMap;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Data.Common;

namespace MTUM_Wasm.Server.Infrastructure.Database.Postgres.Service;

internal class DbContext : IDbContext
{
    private readonly ServiceDbOptions _dbOptions;

    public DbContext(IOptions<ServiceDbOptions> dbOptions)
    {
        _dbOptions = dbOptions.Value;
        SqlMapper.AddTypeHandler(NacPolicyHandler.Instance);
        FluentMapper.Initialize(config =>
        {
            config.AddMap(new TenantMap());
            config.AddMap(new AuditLogMap());
        });
    }

    public DbConnection GetConnection()
    {
        return new NpgsqlConnection(_dbOptions.ConnectionString);
    }
}

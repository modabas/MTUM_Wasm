using MTUM_Wasm.Shared.Core.Common.Utility;
using MTUM_Wasm.Shared.Core.Identity.Entity;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using static Dapper.SqlMapper;

namespace MTUM_Wasm.Server.Infrastructure.Database.Postgres.Dapper.TypeHandler;

class NacPolicyHandler : TypeHandler<NacPolicy>
{
    private NacPolicyHandler() { }
    public static NacPolicyHandler Instance { get; } = new NacPolicyHandler();
#pragma warning disable CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).
    public override NacPolicy? Parse(object value)
#pragma warning restore CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).
    {
        return value is string jsonString ? JsonHelper.DeserializeJson<NacPolicy>(jsonString) : null;
    }
    public override void SetValue(IDbDataParameter parameter, NacPolicy? value)
    {
        parameter.Value = value is null ? null : JsonHelper.SerializeJson(value);
        ((NpgsqlParameter)parameter).NpgsqlDbType = NpgsqlDbType.Jsonb;
    }
}

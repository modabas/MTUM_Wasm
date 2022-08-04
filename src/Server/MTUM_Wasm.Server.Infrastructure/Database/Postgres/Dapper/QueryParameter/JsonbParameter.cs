using Npgsql;
using NpgsqlTypes;
using System.Data;
using static Dapper.SqlMapper;

namespace MTUM_Wasm.Server.Infrastructure.Database.Postgres.Dapper.QueryParameter;

internal class JsonbParameter : ICustomQueryParameter
{
    private readonly string _value;

    public JsonbParameter(string value)
    {
        _value = value;
    }

    public void AddParameter(IDbCommand command, string name)
    {
        var parameter = new NpgsqlParameter(name, NpgsqlDbType.Jsonb)
        {
            Value = _value
        };
        command.Parameters.Add(parameter);
    }
}

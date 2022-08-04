using System.Data.Common;

namespace MTUM_Wasm.Server.Core.Database.Service;

internal interface IDbContext
{
    public DbConnection GetConnection();
}

using MTUM_Wasm.Server.Core.Database.Service;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.Database.Extensions;

internal static class DbConnectionExtensions
{
    //!!!IMPORTANT
    //if we override BeginTransactionAsync and implement with async keyword,
    //AsyncLocal parameter DbTransaction of audit repo will be lost when async method will return to caller
    //we have to stay in current async context or in its children
    //so we only use sync BeginTransaction overloads to set DbTransaction
    public static DbTransaction BeginTransaction(this DbConnection conn, IAuditRepo? auditRepo = null)
    { 
        var transaction = conn.BeginTransaction();
        if (auditRepo is not null)
            auditRepo.DbTransaction = transaction;
        return transaction;
    }

    public static DbTransaction BeginTransaction(this DbConnection conn, IsolationLevel isolationLevel, IAuditRepo? auditRepo = null)
    {
        var transaction = conn.BeginTransaction(isolationLevel);
        if (auditRepo is not null)
            auditRepo.DbTransaction = transaction;
        return transaction;
    }
}

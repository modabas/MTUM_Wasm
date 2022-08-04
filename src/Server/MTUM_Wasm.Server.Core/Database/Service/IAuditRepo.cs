using MTUM_Wasm.Server.Core.Database.Dto;
using MTUM_Wasm.Server.Core.Database.Entity;
using MTUM_Wasm.Shared.Core.Common.Result;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.Database.Service;

internal interface IAuditRepo
{
    public DbTransaction? DbTransaction { get; set; }

    Task<AuditLogPrimaryKey> CreateEntry(AuditLogEntry entry, CancellationToken cancellationToken);
    Task<(bool, AuditLogPrimaryKey)> TryCreateEntry(AuditLogEntry entry, CancellationToken cancellationToken);
    Task<IServiceResult<SearchAuditLogsOutput>> Search(SearchAuditLogsInput requestDto, CancellationToken cancellationToken);
}

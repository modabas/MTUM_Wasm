using MTUM_Wasm.Server.Core.Database.Entity;
using System.Collections.Generic;
using System.Linq;

namespace MTUM_Wasm.Server.Core.Database.Dto;

internal class SearchAuditLogsOutput
{
    public IEnumerable<AuditLogEntity> Logs { get; set; } = Enumerable.Empty<AuditLogEntity>();
    public long TotalCount { get; set; }
}

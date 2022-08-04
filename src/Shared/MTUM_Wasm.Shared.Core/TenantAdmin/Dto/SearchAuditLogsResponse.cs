using MTUM_Wasm.Shared.Core.Common.Dto;
using System.Collections.Generic;
using System.Linq;

namespace MTUM_Wasm.Shared.Core.TenantAdmin.Dto;

public class SearchAuditLogsResponse
{
    public IEnumerable<AuditLogDto> Logs { get; set; } = Enumerable.Empty<AuditLogDto>();
    public long TotalCount { get; set; }
}

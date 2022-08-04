using System;

namespace MTUM_Wasm.Shared.Core.TenantAdmin.Dto;

public class SearchAuditLogsRequest
{
    public string? CommandName { get; set; }
    public string? UserEmail { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

using System;

namespace MTUM_Wasm.Shared.Core.Common.Dto;

public class AuditLogDto
{
    public long Id { get; set; }
    public string CommandName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid? TenantId { get; set; }
    public string RemoteIp { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime CreatedDate { get; set; }
    public string User { get; set; } = string.Empty;
    public string Entry { get; set; } = string.Empty;
}

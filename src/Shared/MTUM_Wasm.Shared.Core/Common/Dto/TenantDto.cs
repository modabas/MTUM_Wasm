using MTUM_Wasm.Shared.Core.Identity.Entity;
using System;

namespace MTUM_Wasm.Shared.Core.Common.Dto;

public class TenantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameLowercase { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModifiedAt { get; set; }
    public NacPolicy? NacPolicy { get; set; }
}

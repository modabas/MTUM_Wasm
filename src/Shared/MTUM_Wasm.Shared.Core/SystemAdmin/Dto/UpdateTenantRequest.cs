using System;

namespace MTUM_Wasm.Shared.Core.SystemAdmin.Dto;

public class UpdateTenantRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
}

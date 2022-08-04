using System;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Dto;

internal class UpdateTenantInput
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
}

namespace MTUM_Wasm.Shared.Core.TenantAdmin.Dto;

public class ChangeUserStateRequest
{
    public string Email { get; set; } = string.Empty;
    public bool SetEnabled { get; set; }
}

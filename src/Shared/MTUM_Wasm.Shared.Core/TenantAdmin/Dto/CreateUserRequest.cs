namespace MTUM_Wasm.Shared.Core.TenantAdmin.Dto;

public class CreateUserRequest
{
    public string Email { get; set; } = string.Empty;
    public string GivenName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
    public string TemporaryPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

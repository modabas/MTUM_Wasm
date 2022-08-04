namespace MTUM_Wasm.Shared.Core.SystemAdmin.Dto;

public class UpdateUserAttributesRequest
{
    public string Email { get; set; } = string.Empty;
    public string GivenName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; }
}

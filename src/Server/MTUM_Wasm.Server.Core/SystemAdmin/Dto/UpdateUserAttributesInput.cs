namespace MTUM_Wasm.Server.Core.SystemAdmin.Dto;

internal class UpdateUserAttributesInput
{
    public string Email { get; set; } = string.Empty;
    public string GivenName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; }
}

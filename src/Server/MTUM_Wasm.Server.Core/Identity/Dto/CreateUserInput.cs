namespace MTUM_Wasm.Server.Core.Identity.Dto;

internal class CreateUserInput
{
    public string Email { get; set; } = string.Empty;
    public string GivenName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
    public string TemporaryPassword { get; set; } = string.Empty;
}


namespace MTUM_Wasm.Server.Core.Identity.Dto;

internal class LoginInput
{
    public string EmailAddress { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

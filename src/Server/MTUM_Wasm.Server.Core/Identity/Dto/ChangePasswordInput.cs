namespace MTUM_Wasm.Server.Core.Identity.Dto;

internal class ChangePasswordInput
{
    public string EmailAddress { get; set; } = string.Empty;
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmNewPassword { get; set; } = string.Empty;
}

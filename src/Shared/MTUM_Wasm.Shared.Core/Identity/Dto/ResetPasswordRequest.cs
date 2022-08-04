namespace MTUM_Wasm.Shared.Core.Identity.Dto;

public class ResetPasswordRequest
{
    public string EmailAddress { get; set; } = string.Empty;
    public string ResetToken { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmNewPassword { get; set; } = string.Empty;
}

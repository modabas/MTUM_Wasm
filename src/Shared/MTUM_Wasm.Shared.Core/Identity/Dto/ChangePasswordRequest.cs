namespace MTUM_Wasm.Shared.Core.Identity.Dto;

public class ChangePasswordRequest
{
    public string EmailAddress { get; set; } = string.Empty;
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmNewPassword { get; set; } = string.Empty;
}

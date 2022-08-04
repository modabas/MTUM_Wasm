using MTUM_Wasm.Server.Core.Identity.Dto;
using MTUM_Wasm.Shared.Core.Identity.Dto;

namespace MTUM_Wasm.Server.Core.Identity.Mapping;

internal static class ResetPasswordExtensions
{
    public static ResetPasswordInput ToInput(this ResetPasswordRequest request)
    {
        return new ResetPasswordInput
        {
            EmailAddress = request.EmailAddress,
            ResetToken = request.ResetToken,
            NewPassword = request.NewPassword,
            ConfirmNewPassword = request.ConfirmNewPassword
        };
    }
}

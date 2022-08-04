using MTUM_Wasm.Server.Core.Identity.Dto;
using MTUM_Wasm.Shared.Core.Identity.Dto;

namespace MTUM_Wasm.Server.Core.Identity.Mapping;

internal static class ChangePasswordExtensions
{
    public static ChangePasswordInput ToInput(this ChangePasswordRequest request)
    {
        return new ChangePasswordInput
        {
            EmailAddress = request.EmailAddress,
            CurrentPassword = request.CurrentPassword,
            NewPassword = request.NewPassword,
            ConfirmNewPassword = request.ConfirmNewPassword
        };
    }
}

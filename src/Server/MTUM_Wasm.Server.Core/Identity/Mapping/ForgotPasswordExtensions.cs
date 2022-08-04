using MTUM_Wasm.Server.Core.Identity.Dto;
using MTUM_Wasm.Shared.Core.Identity.Dto;

namespace MTUM_Wasm.Server.Core.Identity.Mapping;

internal static class ForgotPasswordExtensions
{
    public static ForgotPasswordInput ToInput(this ForgotPasswordRequest request)
    {
        return new ForgotPasswordInput
        {
            EmailAddress = request.EmailAddress
        };
    }
}

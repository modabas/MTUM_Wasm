using MTUM_Wasm.Server.Core.SystemAdmin.Dto;
using MTUM_Wasm.Shared.Core.SystemAdmin.Dto;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Mapping;

internal static class ChangeTenantUserStateExtensions
{
    public static ChangeUserStateInput ToInput(this ChangeUserStateRequest request)
    {
        return new ChangeUserStateInput
        {
            Email = request.Email,
            SetEnabled = request.SetEnabled
        };
    }

    public static Identity.Dto.ChangeUserStateInput ToIdentityInput(this ChangeUserStateInput input)
    {
        return new Identity.Dto.ChangeUserStateInput
        {
            Email = input.Email,
            SetEnabled = input.SetEnabled
        };
    }
}

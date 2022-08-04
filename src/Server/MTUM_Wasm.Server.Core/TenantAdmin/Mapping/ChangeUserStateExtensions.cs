using MTUM_Wasm.Server.Core.TenantAdmin.Dto;
using MTUM_Wasm.Shared.Core.TenantAdmin.Dto;

namespace MTUM_Wasm.Server.Core.TenantAdmin.Mapping;

internal static class ChangeUserStateExtensions
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

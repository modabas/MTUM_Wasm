using MTUM_Wasm.Server.Core.TenantAdmin.Dto;
using MTUM_Wasm.Shared.Core.TenantAdmin.Dto;

namespace MTUM_Wasm.Server.Core.TenantAdmin.Mapping;

internal static class UpdateUserNacPolicyExtensions
{
    public static UpdateUserNacPolicyInput ToInput(this UpdateUserNacPolicyRequest request)
    {
        return new UpdateUserNacPolicyInput
        {
            Email = request.Email,
            NacPolicy = request.NacPolicy
        };
    }

    public static Identity.Dto.UpdateUserNacPolicyInput ToIdentityInput(this UpdateUserNacPolicyInput input)
    {
        return new Identity.Dto.UpdateUserNacPolicyInput
        {
            Email = input.Email,
            NacPolicy = input.NacPolicy
        };
    }
}

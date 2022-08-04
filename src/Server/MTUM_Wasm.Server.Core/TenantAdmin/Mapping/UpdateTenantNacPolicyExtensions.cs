using MTUM_Wasm.Server.Core.TenantAdmin.Dto;
using MTUM_Wasm.Shared.Core.TenantAdmin.Dto;

namespace MTUM_Wasm.Server.Core.TenantAdmin.Mapping;

internal static class UpdateTenantNacPolicyExtensions
{
    public static UpdateTenantNacPolicyInput ToInput(this UpdateTenantNacPolicyRequest request)
    {
        return new UpdateTenantNacPolicyInput
        {
            NacPolicy = request.NacPolicy
        };
    }
}

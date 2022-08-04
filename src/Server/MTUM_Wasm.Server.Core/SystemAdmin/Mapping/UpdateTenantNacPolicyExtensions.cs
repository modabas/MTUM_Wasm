using MTUM_Wasm.Server.Core.SystemAdmin.Dto;
using MTUM_Wasm.Shared.Core.SystemAdmin.Dto;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Mapping;

internal static class UpdateTenantNacPolicyExtensions
{
    public static UpdateTenantNacPolicyInput ToInput(this UpdateTenantNacPolicyRequest request)
    {
        return new UpdateTenantNacPolicyInput
        {
            Id = request.Id,
            NacPolicy = request.NacPolicy
        };
    }
}

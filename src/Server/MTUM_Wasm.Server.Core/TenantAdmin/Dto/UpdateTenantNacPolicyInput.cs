using MTUM_Wasm.Shared.Core.Identity.Entity;

namespace MTUM_Wasm.Server.Core.TenantAdmin.Dto;

internal class UpdateTenantNacPolicyInput
{
    public NacPolicy? NacPolicy { get; set; }
}

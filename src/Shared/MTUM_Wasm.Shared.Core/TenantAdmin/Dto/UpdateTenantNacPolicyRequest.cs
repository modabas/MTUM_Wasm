using MTUM_Wasm.Shared.Core.Identity.Entity;

namespace MTUM_Wasm.Shared.Core.TenantAdmin.Dto;

public class UpdateTenantNacPolicyRequest
{
    public NacPolicy? NacPolicy { get; set; }
}
using MTUM_Wasm.Shared.Core.Identity.Entity;

namespace MTUM_Wasm.Shared.Core.TenantAdmin.Dto;

public class UpdateUserNacPolicyRequest
{
    public string Email { get; set; } = string.Empty;
    public NacPolicy? NacPolicy { get; set; }
}

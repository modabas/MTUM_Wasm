using MTUM_Wasm.Shared.Core.Identity.Entity;

namespace MTUM_Wasm.Server.Core.Identity.Dto;

internal class UpdateUserNacPolicyInput
{
    public string Email { get; set; } = string.Empty;
    public NacPolicy? NacPolicy { get; set; }
}

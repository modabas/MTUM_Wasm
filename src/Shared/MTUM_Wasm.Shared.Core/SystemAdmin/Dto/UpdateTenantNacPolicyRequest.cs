using MTUM_Wasm.Shared.Core.Identity.Entity;
using System;

namespace MTUM_Wasm.Shared.Core.SystemAdmin.Dto;

public class UpdateTenantNacPolicyRequest
{
    public Guid Id { get; set; }
    public NacPolicy? NacPolicy { get; set; }
}

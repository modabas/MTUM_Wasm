using MTUM_Wasm.Shared.Core.Identity.Entity;
using System;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Dto;

internal class UpdateTenantNacPolicyInput
{
    public Guid Id { get; set; }
    public NacPolicy? NacPolicy { get; set; }
}

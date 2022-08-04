using MTUM_Wasm.Server.Core.Common.Entity;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Dto;

internal class GetTenantOutput
{
    public TenantEntity? Tenant { get; set; }
}

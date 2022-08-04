using MTUM_Wasm.Server.Core.Common.Entity;

namespace MTUM_Wasm.Server.Core.TenantViewerOrUp.Dto;

internal class GetTenantOutput
{
    public TenantEntity? Tenant { get; set; }
}

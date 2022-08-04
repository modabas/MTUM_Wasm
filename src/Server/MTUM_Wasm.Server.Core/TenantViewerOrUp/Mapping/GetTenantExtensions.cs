using MTUM_Wasm.Server.Core.Common.Mapping;
using MTUM_Wasm.Server.Core.Database.Mapping;
using MTUM_Wasm.Server.Core.TenantViewerOrUp.Dto;
using MTUM_Wasm.Shared.Core.TenantViewerOrUp.Dto;

namespace MTUM_Wasm.Server.Core.TenantViewerOrUp.Mapping;

internal static class GetTenantExtensions
{
    public static GetTenantResponse ToResponse(this GetTenantOutput output)
    {
        return new GetTenantResponse
        {
            Tenant = output.Tenant?.ToSharedDto()
        };
    }
}

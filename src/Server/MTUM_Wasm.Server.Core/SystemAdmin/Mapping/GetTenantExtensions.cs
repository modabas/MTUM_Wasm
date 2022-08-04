using MTUM_Wasm.Server.Core.Common.Mapping;
using MTUM_Wasm.Server.Core.Database.Mapping;
using MTUM_Wasm.Server.Core.SystemAdmin.Dto;
using MTUM_Wasm.Shared.Core.SystemAdmin.Dto;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Mapping;

internal static class GetTenantExtensions
{
    public static GetTenantInput ToInput(this GetTenantRequest request)
    {
        return new GetTenantInput
        {
            Id = request.Id
        };
    }

    public static GetTenantResponse ToResponse(this GetTenantOutput output)
    {
        return new GetTenantResponse
        {
            Tenant = output.Tenant?.ToSharedDto()
        };
    }
}

using MTUM_Wasm.Server.Core.Common.Mapping;
using MTUM_Wasm.Server.Core.Database.Mapping;
using MTUM_Wasm.Server.Core.SystemAdmin.Dto;
using MTUM_Wasm.Shared.Core.SystemAdmin.Dto;
using System.Linq;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Mapping;

internal static class GetTenantsExtensions
{
    public static GetTenantsResponse ToResponse(this GetTenantsOutput output)
    {
        return new GetTenantsResponse
        {
            Tenants = output.Tenants.Select(dtoTenant => dtoTenant.ToSharedDto())
        };
    }
}

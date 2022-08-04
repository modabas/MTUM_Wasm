using MTUM_Wasm.Server.Core.SystemAdmin.Dto;
using MTUM_Wasm.Shared.Core.SystemAdmin.Dto;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Mapping;

internal static class CreateTenantExtensions
{
    public static CreateTenantInput ToInput(this CreateTenantRequest request)
    {
        return new CreateTenantInput
        {
            Name = request.Name
        };
    }
}

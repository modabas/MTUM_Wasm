using MTUM_Wasm.Server.Core.Common.Mapping;
using MTUM_Wasm.Server.Core.SystemAdmin.Dto;
using MTUM_Wasm.Server.Core.TenantAdmin.Mapping;
using MTUM_Wasm.Shared.Core.SystemAdmin.Dto;
using System.Linq;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Mapping;

internal static class GetTenantUsersExtensions
{
    public static GetTenantUsersInput ToInput(this GetTenantUsersRequest request)
    {
        return new GetTenantUsersInput
        {
            TenantId = request.TenantId
        };
    }

    public static GetTenantUsersResponse ToResponse(this GetTenantUsersOutput output)
    {
        return new GetTenantUsersResponse
        {
            Users = output.Users.Select(p => p.ToSharedDto())
        };
    }

    public static GetTenantUsersOutput ToSystemAdminOutput(this Identity.Dto.GetUsersOutput output)
    {
        return new GetTenantUsersOutput
        {
            Users = output.Users
        };
    }
}

using MTUM_Wasm.Server.Core.Common.Mapping;
using MTUM_Wasm.Server.Core.TenantAdmin.Dto;
using MTUM_Wasm.Shared.Core.TenantAdmin.Dto;
using System.Linq;

namespace MTUM_Wasm.Server.Core.TenantAdmin.Mapping;

internal static class GetUsersExtensions
{
    public static GetUsersResponse ToResponse(this GetUsersOutput output)
    {
        return new GetUsersResponse
        {
            Users = output.Users.Select(p => p.ToSharedDto())
        };
    }

    public static GetUsersOutput ToTenantAdminOutput(this Identity.Dto.GetUsersOutput output)
    {
        return new GetUsersOutput
        {
            Users = output.Users
        };
    }
}

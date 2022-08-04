using MTUM_Wasm.Server.Core.Common.Mapping;
using MTUM_Wasm.Server.Core.SystemAdmin.Dto;
using MTUM_Wasm.Shared.Core.SystemAdmin.Dto;
using System.Linq;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Mapping;

internal static class GetUsersInGroupExtensions
{
    public static GetUsersInGroupInput ToInput(this GetUsersInGroupRequest request)
    {
        return new GetUsersInGroupInput
        {
            GroupName = request.GroupName
        };
    }

    public static Identity.Dto.GetUsersInGroupInput ToIdentityInput(this GetUsersInGroupInput input)
    {
        return new Identity.Dto.GetUsersInGroupInput
        {
            GroupName = input.GroupName
        };
    }

    public static GetUsersInGroupResponse ToResponse(this GetUsersInGroupOutput output)
    {
        return new GetUsersInGroupResponse
        {
            Users = output.Users.Select(u => u.ToSharedDto())
        };
    }

    public static GetUsersInGroupOutput ToSystemAdminOutput(this Identity.Dto.GetUsersInGroupOutput output)
    {
        return new GetUsersInGroupOutput
        {
            Users = output.Users
        };
    }
}

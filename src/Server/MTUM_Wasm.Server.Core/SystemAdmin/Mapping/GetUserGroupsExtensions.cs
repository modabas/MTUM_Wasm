using MTUM_Wasm.Server.Core.SystemAdmin.Dto;
using MTUM_Wasm.Shared.Core.SystemAdmin.Dto;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Mapping;

internal static class GetUserGroupsExtensions
{
    public static GetUserGroupsInput ToInput(this GetUserGroupsRequest request)
    {
        return new GetUserGroupsInput
        {
            Email = request.Email
        };
    }

    public static GetUserGroupsResponse ToResponse(this GetUserGroupsOutput output)
    {
        return new GetUserGroupsResponse
        {
            Groups = output.Groups
        };
    }

    public static Identity.Dto.GetUserGroupsInput ToIdentityInput(this GetUserGroupsInput input)
    {
        return new Identity.Dto.GetUserGroupsInput
        {
            Email = input.Email
        };
    }

    public static GetUserGroupsOutput ToSystemAdminOutput(this Identity.Dto.GetUserGroupsOutput output)
    {
        return new GetUserGroupsOutput
        {
            Groups = output.Groups
        };
    }
}

using MTUM_Wasm.Server.Core.TenantAdmin.Dto;
using MTUM_Wasm.Shared.Core.TenantAdmin.Dto;

namespace MTUM_Wasm.Server.Core.TenantAdmin.Mapping;

internal static class UpdateUserGroupsExtensions
{
    public static UpdateUserGroupsInput ToInput(this UpdateUserGroupsRequest request)
    {
        return new UpdateUserGroupsInput
        {
            Email = request.Email,
            NewGroups = request.NewGroups
        };
    }

    public static Identity.Dto.UpdateUserGroupsInput ToIdentityInput(this UpdateUserGroupsInput input)
    {
        return new Identity.Dto.UpdateUserGroupsInput
        {
            Email = input.Email,
            NewGroups = input.NewGroups,
            GroupsToAdd = input.GroupsToAdd,
            GroupsToRemove = input.GroupsToRemove
        };
    }
}

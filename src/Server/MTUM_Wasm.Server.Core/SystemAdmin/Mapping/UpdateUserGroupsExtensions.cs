using MTUM_Wasm.Server.Core.SystemAdmin.Dto;
using MTUM_Wasm.Shared.Core.SystemAdmin.Dto;
using System;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Mapping;

internal static class UpdateUserGroupsExtensions
{
    public static UpdateUserGroupsInput ToInput(this UpdateUserGroupsRequest request)
    {
        return new UpdateUserGroupsInput
        {
            Email = request.Email,
            OpMode = request.OpMode.ToDto(),
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

    public static UpdateUserGroupsOpModeDto ToDto(this UpdateUserGroupsOpMode operationMode)
    {
        return operationMode switch
        {
            UpdateUserGroupsOpMode.PossibleSystemRoles => UpdateUserGroupsOpModeDto.PossibleSystemRoles,
            UpdateUserGroupsOpMode.PossibleTenantRoles => UpdateUserGroupsOpModeDto.PossibleTenantRoles,
            _ => throw new ArgumentException("Invalid op mode value.", nameof(operationMode)),
        };
    }
}

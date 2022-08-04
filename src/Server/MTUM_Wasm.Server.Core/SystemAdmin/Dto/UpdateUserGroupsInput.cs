using System.Collections.Generic;
using System.Linq;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Dto;

internal class UpdateUserGroupsInput
{
    public string Email { get; set; } = string.Empty;
    public UpdateUserGroupsOpModeDto OpMode { get; set; }
    public IEnumerable<string> NewGroups { get; set; } = Enumerable.Empty<string>();
    public IEnumerable<string> GroupsToAdd { get; set; } = Enumerable.Empty<string>();
    public IEnumerable<string> GroupsToRemove { get; set; } = Enumerable.Empty<string>();
}

internal enum UpdateUserGroupsOpModeDto
{
    PossibleTenantRoles = 1, PossibleSystemRoles = 2
}
using System.Collections.Generic;
using System.Linq;

namespace MTUM_Wasm.Shared.Core.SystemAdmin.Dto;

public class UpdateUserGroupsRequest
{
    public string Email { get; set; } = string.Empty;
    public UpdateUserGroupsOpMode OpMode { get; set; }
    public IEnumerable<string> NewGroups { get; set; } = Enumerable.Empty<string>();
}

public enum UpdateUserGroupsOpMode
{
    PossibleTenantRoles = 1, PossibleSystemRoles = 2
}
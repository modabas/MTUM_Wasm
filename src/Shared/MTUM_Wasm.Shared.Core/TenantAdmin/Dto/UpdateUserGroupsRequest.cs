using System.Collections.Generic;
using System.Linq;

namespace MTUM_Wasm.Shared.Core.TenantAdmin.Dto;

public class UpdateUserGroupsRequest
{
    public string Email { get; set; } = string.Empty;
    public IEnumerable<string> NewGroups { get; set; } = Enumerable.Empty<string>();
}

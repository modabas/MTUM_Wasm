using System.Collections.Generic;
using System.Linq;

namespace MTUM_Wasm.Shared.Core.SystemAdmin.Dto;

public class GetUserGroupsResponse
{
    public IEnumerable<string> Groups { get; set; } = Enumerable.Empty<string>();
}

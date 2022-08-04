using System.Collections.Generic;
using System.Linq;

namespace MTUM_Wasm.Server.Core.Identity.Dto;

internal class GetUserGroupsOutput
{
    public IEnumerable<string> Groups { get; set; } = Enumerable.Empty<string>();
}

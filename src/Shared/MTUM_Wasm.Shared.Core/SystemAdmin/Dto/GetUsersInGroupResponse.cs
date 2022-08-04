using MTUM_Wasm.Shared.Core.Common.Dto;
using System.Collections.Generic;
using System.Linq;

namespace MTUM_Wasm.Shared.Core.SystemAdmin.Dto;

public class GetUsersInGroupResponse
{
    public IEnumerable<SystemUserDto> Users { get; set; } = Enumerable.Empty<SystemUserDto>();
}

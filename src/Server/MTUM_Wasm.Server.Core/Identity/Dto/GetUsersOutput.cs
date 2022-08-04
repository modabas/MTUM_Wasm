using MTUM_Wasm.Server.Core.Common.Entity;
using System.Collections.Generic;
using System.Linq;

namespace MTUM_Wasm.Server.Core.Identity.Dto;

internal class GetUsersOutput
{
    public IEnumerable<SystemUserEntity> Users { get; set; } = Enumerable.Empty<SystemUserEntity>();
}

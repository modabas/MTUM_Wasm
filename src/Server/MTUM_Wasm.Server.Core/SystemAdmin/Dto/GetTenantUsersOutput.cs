using MTUM_Wasm.Server.Core.Common.Entity;
using System.Collections.Generic;
using System.Linq;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Dto;

internal class GetTenantUsersOutput
{
    public IEnumerable<SystemUserEntity> Users { get; set; } = Enumerable.Empty<SystemUserEntity>();
}

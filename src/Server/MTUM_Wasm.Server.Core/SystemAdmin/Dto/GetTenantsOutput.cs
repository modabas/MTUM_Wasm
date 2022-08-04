using MTUM_Wasm.Server.Core.Common.Entity;
using System.Collections.Generic;
using System.Linq;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Dto;

internal class GetTenantsOutput
{
    public IEnumerable<TenantEntity> Tenants { get; set; } = Enumerable.Empty<TenantEntity>();
}

using MTUM_Wasm.Shared.Core.Common.Dto;
using System.Collections.Generic;
using System.Linq;

namespace MTUM_Wasm.Shared.Core.SystemAdmin.Dto;

public class GetTenantsResponse
{
    public IEnumerable<TenantDto> Tenants { get; set; } = Enumerable.Empty<TenantDto>();
}

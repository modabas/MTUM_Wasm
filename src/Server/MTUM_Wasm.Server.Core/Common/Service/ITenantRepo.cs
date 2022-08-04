using MTUM_Wasm.Server.Core.Common.Entity;
using MTUM_Wasm.Server.Core.SystemAdmin.Dto;
using MTUM_Wasm.Shared.Core.Common.Result;
using MTUM_Wasm.Shared.Core.Identity.Entity;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.Common.Service;

internal interface ITenantRepo
{
    Task<IServiceResult<TenantEntity>> CreateTenant(string tenantName, CancellationToken cancellationToken);
    Task<IServiceResult<TenantEntity>> GetTenant(Guid tenantId, CancellationToken cancellationToken);
    Task<IServiceResult<IEnumerable<TenantEntity>>> GetTenants(CancellationToken cancellationToken);
    Task<IServiceResult<TenantEntity>> UpdateTenant(TenantEntity tenant, CancellationToken cancellationToken);
    Task<IServiceResult<TenantEntity>> UpdateTenantNacPolicy(Guid tenantId, NacPolicy? nacPolicy, CancellationToken cancellationToken);
}

using MTUM_Wasm.Server.Core.Common.Entity;
using MTUM_Wasm.Shared.Core.Common.Result;
using MTUM_Wasm.Shared.Core.Identity.Entity;
using Orleans;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.Common.Orleans;

internal interface ITenantGrain : IGrainWithGuidKey
{
    Task<IServiceResult<TenantEntity>> GetTenant(GrainCancellationToken grainCancellationToken);
    Task<IServiceResult> UpdateTenant(TenantEntity tenant, GrainCancellationToken grainCancellationToken);
    Task<IServiceResult> TenantExists(GrainCancellationToken grainCancellationToken);
    Task<IServiceResult> UpdateTenantNacPolicy(NacPolicy? nacPolicy, GrainCancellationToken grainCancellationToken);
}

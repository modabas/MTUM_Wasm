using MTUM_Wasm.Shared.Core.Common.Result;
using MTUM_Wasm.Shared.Core.TenantViewerOrUp.Dto;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Client.Core.TenantViewerOrUp.Service;

internal interface ITenantViewerOrUpService
{
    Task<IServiceResult<GetTenantResponse>> GetTenant(CancellationToken cancellationToken);

}

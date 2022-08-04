using MTUM_Wasm.Client.Core.Utility.QueryHelpers;
using MTUM_Wasm.Shared.Core.Common.Result;
using MTUM_Wasm.Shared.Core.TenantViewerOrUp.Dto;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Client.Core.TenantViewerOrUp.Service;

internal class TenantViewerOrUpService : ITenantViewerOrUpService
{
    private readonly HttpClient _httpClient;

    private const string GetTenantUri = "/api/TenantViewerOrUp/getTenant";

    public TenantViewerOrUpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IServiceResult<GetTenantResponse>> GetTenant(CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(GetTenantUri, cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.DeserializeResult<GetTenantResponse>(cancellationToken);

        return result;
    }
}

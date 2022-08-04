using MTUM_Wasm.Client.Core.Identity.Service;
using MTUM_Wasm.Client.Core.Utility;
using MTUM_Wasm.Shared.Core.Common.Utility;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Client.Core.Identity.Http;

internal class AuthenticationHeaderHandler : DelegatingHandler
{
    private readonly ITokenStore _tokenStore;

    public AuthenticationHeaderHandler(ITokenStore tokenStore)
    {
        _tokenStore = tokenStore;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.Headers.Authorization?.Scheme != Definitions.BearerSchemeName)
        {
            var accessToken = await _tokenStore.GetTokenProperty<string>(TokenPropertyNameEnum.AccessToken, cancellationToken);
            var idToken = await _tokenStore.GetTokenProperty<string>(TokenPropertyNameEnum.IdToken, cancellationToken);

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(Definitions.BearerSchemeName, accessToken);
            }
            if (!string.IsNullOrWhiteSpace(idToken))
            {
                request.Headers.Set(HttpHeader.Name.IdToken, idToken);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}

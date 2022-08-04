using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Infrastructure.Identity.AwsCognito.Authentication;

internal interface IAwsCognitoConfigManager
{
    Task<OpenIdConnectConfiguration> GetMetadata(CancellationToken cancellationToken);
}
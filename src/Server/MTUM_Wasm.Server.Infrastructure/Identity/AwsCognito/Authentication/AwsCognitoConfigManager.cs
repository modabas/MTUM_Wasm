using MTUM_Wasm.Server.Infrastructure.Identity.AwsCognito.Utility;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Infrastructure.Identity.AwsCognito.Authentication;

internal class AwsCognitoConfigManager : IAwsCognitoConfigManager
{
    private readonly IConfigurationManager<OpenIdConnectConfiguration> _configManager;

    public AwsCognitoConfigManager(IOptions<AwsCognitoOptions> options)
    {
        var awsCognitoOptions = options.Value;
        var metadataAddress = $"https://cognito-idp.{awsCognitoOptions.Region}.amazonaws.com/{awsCognitoOptions.UserPoolId}/.well-known/openid-configuration";
        _configManager = new ConfigurationManager<OpenIdConnectConfiguration>(metadataAddress, new OpenIdConnectConfigurationRetriever(), new HttpDocumentRetriever());
    }

    public async Task<OpenIdConnectConfiguration> GetMetadata(CancellationToken cancellationToken)
    {
        return await _configManager.GetConfigurationAsync(cancellationToken);
    }
}

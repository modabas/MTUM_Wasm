using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using MTUM_Wasm.Server.Core.Identity.Service;
using MTUM_Wasm.Server.Infrastructure.Identity.AwsCognito.Authentication;
using MTUM_Wasm.Server.Infrastructure.Identity.AwsCognito.Mapping;
using MTUM_Wasm.Server.Infrastructure.Identity.AwsCognito.Service;
using MTUM_Wasm.Shared.Core.Identity.Service;
using MTUM_Wasm.Shared.Infrastructure.Identity.AwsCognito;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MTUM_Wasm.Server.Infrastructure.Identity.AwsCognito.Utility;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAwsCognitoAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var awsCognitoOptionsSection = configuration.GetSection("AwsCognito");
        var awsCognitoOptions = awsCognitoOptionsSection.Get<AwsCognitoOptions>();
        services.Configure<AwsCognitoOptions>(awsCognitoOptionsSection);
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = awsCognitoOptions.GetTokenValidationParameters();
            options.MetadataAddress = $"https://cognito-idp.{awsCognitoOptions.Region}.amazonaws.com/{awsCognitoOptions.UserPoolId}/.well-known/openid-configuration";
        });

        var provider = new AmazonCognitoIdentityProviderClient(
                        awsCognitoOptions.AccessKeyId,
                        awsCognitoOptions.SecretAccessKey,
                        RegionEndpoint.GetBySystemName(awsCognitoOptions.Region));

        var userPool = new CognitoUserPoolWrapper(new CognitoUserPool(
                awsCognitoOptions.UserPoolId,
                awsCognitoOptions.UserPoolClientId,
                provider,
                awsCognitoOptions.UserPoolClientSecret));

        services.AddSingleton<IAmazonCognitoIdentityProvider>(provider);
        services.AddSingleton<ICognitoUserPoolWrapper>(userPool);

        services.AddSingleton<IIdentityService, AwsCognitoIdentityService>();
        services.AddSingleton<IIdentityManagementService, AwsCognitoIdentityManagementService>();
        services.AddSingleton<ITokenClaimResolver, AwsCognitoTokenClaimResolver>();
        services.AddSingleton<IAwsCognitoConfigManager, AwsCognitoConfigManager>();
        services.AddSingleton<IClaimsTransformation, AwsClaimsTransformation>();

        return services;
    }
}

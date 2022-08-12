using MTUM_Wasm.Server.Infrastructure.Identity.AwsCognito.Utility;
using Microsoft.IdentityModel.Tokens;
using System;

namespace MTUM_Wasm.Server.Infrastructure.Identity.AwsCognito.Mapping;

internal static class AwsCognitoOptionsExtensions
{
    public static TokenValidationParameters GetTokenValidationParameters(this AwsCognitoOptions options, TokenValidationParametersTypeEnum parameterType)
    {
        return parameterType switch
        {
            TokenValidationParametersTypeEnum.ForAccessToken => new TokenValidationParameters()
            {
                ClockSkew = TimeSpan.FromSeconds(60), //default 5 minutes
                ValidIssuer = $"https://cognito-idp.{options.Region}.amazonaws.com/{options.UserPoolId}",
                //ValidAudience = options.UserPoolClientId,
                ValidateAudience = false,
            },
            TokenValidationParametersTypeEnum.ForIdToken => new TokenValidationParameters()
            {
                ClockSkew = TimeSpan.FromSeconds(60), //default 5 minutes
                ValidIssuer = $"https://cognito-idp.{options.Region}.amazonaws.com/{options.UserPoolId}",
                ValidAudience = options.UserPoolClientId,
                ValidateAudience = true,
            },
            _ => throw new ArgumentOutOfRangeException(nameof(parameterType), parameterType, "Unexpected token validation parameter type."),
        };
    }
}

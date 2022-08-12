using MTUM_Wasm.Server.Core.Identity.Utility;
using MTUM_Wasm.Server.Infrastructure.Identity.AwsCognito.Mapping;
using MTUM_Wasm.Server.Infrastructure.Identity.AwsCognito.Utility;
using MTUM_Wasm.Shared.Core.Common.Utility;
using MTUM_Wasm.Shared.Core.Identity.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace MTUM_Wasm.Server.Infrastructure.Identity.AwsCognito.Authentication;

internal class AwsClaimsTransformation : IClaimsTransformation
{
    private readonly ITokenClaimResolver _tokenClaimResolver;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAwsCognitoConfigManager _authConfigManager;
    private readonly AwsCognitoOptions _options;

    public AwsClaimsTransformation(ITokenClaimResolver tokenClaimResolver,
        IHttpContextAccessor httpContextAccessor,
        IOptions<AwsCognitoOptions> options,
        IAwsCognitoConfigManager authConfigManager)
    {
        _tokenClaimResolver = tokenClaimResolver;
        _httpContextAccessor = httpContextAccessor;
        _authConfigManager = authConfigManager;
        _options = options.Value;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        //Aws Cognito access token does not include audience (aud) claim to be used at validation
        //but a "client_id" claim exists which can be checked for audience
        var accessTokenAudience = principal.Claims.FirstOrDefault(t => t.Type == "client_id")?.Value;
        if (accessTokenAudience is null || !accessTokenAudience.Equals(_options.UserPoolClientId))
            throw new UserNotAuthenticated("Invalid Access token.");

        //Check Id token exists in request headers
        var idToken = _httpContextAccessor.HttpContext?.Request.Headers[HttpHeader.Name.IdToken].ToString();
        if (string.IsNullOrWhiteSpace(idToken))
            throw new UserNotAuthenticated("Id token not found.");

        var cancellationToken = _httpContextAccessor.HttpContext?.RequestAborted ?? default;

        //Validate Id token
        var tokenValidationParametersForIdToken = _options.GetTokenValidationParameters(TokenValidationParametersTypeEnum.ForIdToken);
        var metadata = await _authConfigManager.GetMetadata(cancellationToken);
        tokenValidationParametersForIdToken.IssuerSigningKeys = metadata.SigningKeys;
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationResult = await tokenHandler.ValidateTokenAsync(idToken, tokenValidationParametersForIdToken);
        if (!validationResult.IsValid)
            throw new UserNotAuthenticated("Invalid Id token.");


        var claims = new List<Claim>();
        //Access token claims
        claims.AddRange(await _tokenClaimResolver.TransformAccessTokenClaims(principal.Claims, cancellationToken));
        //Id token claims
        claims.AddRange(await _tokenClaimResolver.TransformIdTokenClaims(idToken, cancellationToken));

        var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
        return new ClaimsPrincipal(claimsIdentity);
    }
}

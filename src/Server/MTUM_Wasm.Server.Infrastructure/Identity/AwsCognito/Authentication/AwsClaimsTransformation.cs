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
        var claims = new List<Claim>();

        var cancellationToken = _httpContextAccessor.HttpContext?.RequestAborted ?? default;
        claims.AddRange(await _tokenClaimResolver.TransformAccessTokenClaims(principal.Claims, cancellationToken));

        var idToken = _httpContextAccessor.HttpContext?.Request.Headers[HttpHeader.Name.IdToken].ToString();
        if (string.IsNullOrWhiteSpace(idToken))
            throw new UserNotAuthenticated("Id token not found.");

        var tokenValidationParameters = _options.GetTokenValidationParameters();
        var metadata = await _authConfigManager.GetMetadata(cancellationToken);
        tokenValidationParameters.IssuerSigningKeys = metadata.SigningKeys;
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationResult = await tokenHandler.ValidateTokenAsync(idToken, tokenValidationParameters);
        if (!validationResult.IsValid)
            throw new UserNotAuthenticated("Invalid Id token.");
        claims.AddRange(await _tokenClaimResolver.TransformIdTokenClaims(idToken, cancellationToken));

        var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);

        return new ClaimsPrincipal(claimsIdentity);
    }
}

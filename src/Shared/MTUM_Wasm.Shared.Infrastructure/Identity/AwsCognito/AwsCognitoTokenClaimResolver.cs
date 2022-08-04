using MTUM_Wasm.Shared.Core.Common.Extension;
using MTUM_Wasm.Shared.Core.Identity.Entity;
using MTUM_Wasm.Shared.Core.Identity.Service;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Shared.Infrastructure.Identity.AwsCognito;

public class AwsCognitoTokenClaimResolver : ITokenClaimResolver
{
    public Task<ICurrentUser> GetCurrentUser(ClaimsIdentity? identity, CancellationToken cancellationToken)
    {
        ICurrentUser currentUser = new AwsCognitoCurrentUser(identity);
        return Task.FromResult(currentUser);
    }

    public Task<IEnumerable<Claim>> TransformAccessTokenClaims(IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        var ret = new List<Claim>();

        ret.AddRange(claims.Where(t => t.Type != "cognito:groups"));
        var roleClaims = claims.Where(t => t.Type == "cognito:groups");
        ret.AddRange(roleClaims.Select(c => new Claim(ClaimTypes.Role, c.Value)));

        return Task.FromResult<IEnumerable<Claim>>(ret.AsReadOnly());
    }

    public Task<IEnumerable<Claim>> TransformIdTokenClaims(IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        var ret = new List<Claim>();

        //set name of principal to email address
        ret.Add(new Claim(ClaimTypes.Name, claims.FirstOrDefault(c => c.Type == "email")?.Value ?? string.Empty));
        ret.Add(new Claim(ClaimTypes.Email, claims.FirstOrDefault(c => c.Type == "email")?.Value ?? string.Empty));
        ret.Add(new Claim(ClaimTypes.GivenName, claims.FirstOrDefault(c => c.Type == "given_name")?.Value ?? string.Empty));
        ret.Add(new Claim("middle_name", claims.FirstOrDefault(c => c.Type == "middle_name")?.Value ?? string.Empty));
        ret.Add(new Claim(ClaimTypes.Surname, claims.FirstOrDefault(c => c.Type == "family_name")?.Value ?? string.Empty));
        ret.Add(new Claim("cognito_user_name", claims.FirstOrDefault(c => c.Type == "cognito:username")?.Value ?? string.Empty));
        ret.Add(new Claim("tenant_id", claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value ?? string.Empty));
        ret.Add(new Claim("nac", claims.FirstOrDefault(c => c.Type == "custom:nac")?.Value ?? string.Empty));
        ret.Add(new Claim("authenticated_at", claims.FirstOrDefault(c => c.Type == "iat")?.Value ?? string.Empty));

        return Task.FromResult<IEnumerable<Claim>>(ret.AsReadOnly());
    }
}

using MTUM_Wasm.Shared.Core.Identity.Entity;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Shared.Core.Identity.Service;

public interface ITokenClaimResolver
{
    public Task<IEnumerable<Claim>> TransformIdTokenClaims(string idToken, CancellationToken cancellationToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler().ReadJwtToken(idToken);
        return TransformIdTokenClaims(tokenHandler.Claims, cancellationToken);
    }
    public Task<IEnumerable<Claim>> TransformAccessTokenClaims(string accessToken, CancellationToken cancellationToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
        return TransformAccessTokenClaims(tokenHandler.Claims, cancellationToken);
    }
    public Task<IEnumerable<Claim>> TransformIdTokenClaims(IEnumerable<Claim> claims, CancellationToken cancellationToken);
    public Task<IEnumerable<Claim>> TransformAccessTokenClaims(IEnumerable<Claim> claims, CancellationToken cancellationToken);
    public Task<ICurrentUser> GetCurrentUser(ClaimsIdentity? identity, CancellationToken cancellationToken);
}

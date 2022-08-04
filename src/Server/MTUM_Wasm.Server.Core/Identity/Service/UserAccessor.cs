using MTUM_Wasm.Shared.Core.Identity.Entity;
using MTUM_Wasm.Shared.Core.Identity.Service;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.Identity.Service;

internal class UserAccessor : IUserAccessor
{
    private readonly IHttpContextAccessor _accessor;
    private readonly ITokenClaimResolver _tokenClaimResolver;

    public UserAccessor(IHttpContextAccessor accessor, ITokenClaimResolver tokenClaimResolver)
    {
        _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        _tokenClaimResolver = tokenClaimResolver;
    }

    public ClaimsPrincipal? User => _accessor.HttpContext?.User;

    public async Task<ICurrentUser> GetCurrentUser(CancellationToken cancellationToken)
    {
        var identity = (ClaimsIdentity?)_accessor.HttpContext?.User.Identity;
        var currentUser = await _tokenClaimResolver.GetCurrentUser(identity, cancellationToken);
        return currentUser;
    }

}

using MTUM_Wasm.Server.Core.Common.Extension;
using MTUM_Wasm.Server.Core.Identity.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.Common.Policy.Nac;

internal class UserNacRequirementHandler : AuthorizationHandler<UserNacRequirement>
{
    private readonly IUserAccessor _userAccessor;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public UserNacRequirementHandler(IUserAccessor userAccessor, IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _userAccessor = userAccessor;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserNacRequirement requirement)
    {
        var currentUser = await _userAccessor.GetCurrentUser(_httpContextAccessor.HttpContext?.RequestAborted ?? default);
        if (currentUser.IsAuthenticated == false)
        {
            context.Fail(new(this, "User is not authenticated."));
            return;
        }
        var nacPolicy = currentUser.NacPolicy;
        var remoteIpAddress = _httpContextAccessor.GetRequestIP(true);

        NacPolicyHelper.ValidateNacRequirement(context, this, requirement, nacPolicy, remoteIpAddress);
        return;
    }
}

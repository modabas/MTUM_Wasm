using MTUM_Wasm.Shared.Core.Common.Extension;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MTUM_Wasm.Shared.Core.Identity.Policy;

public class TenantClaimHandler : AuthorizationHandler<TenantClaimRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TenantClaimRequirement requirement)
    {
        var identity = (ClaimsIdentity?)context.User.Identity;
        var tenant = identity?.GetTenantId();
        if (tenant is not null)
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}

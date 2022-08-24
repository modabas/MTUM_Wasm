using MTUM_Wasm.Shared.Core.Common.Result;
using MTUM_Wasm.Server.Core.Common.Extension;
using MTUM_Wasm.Shared.Core.Identity.Entity;
using Microsoft.AspNetCore.Authorization;

namespace MTUM_Wasm.Server.Core.Common.Policy.Nac;

internal static class NacPolicyHelper
{
    public static void ValidateNacRequirement<TRequirement>(AuthorizationHandlerContext context,
        AuthorizationHandler<TRequirement> handler,
        TRequirement requirement,
        NacPolicy? nacPolicy,
        IServiceResult<string> remoteIpAddressResult) where TRequirement : IAuthorizationRequirement
    {
        //No nac policy is defined
        if (nacPolicy is null)
        {
            context.Succeed(requirement);
            return;
        }

        var validationResult = nacPolicy.Validate(remoteIpAddressResult);
        if (validationResult.Succeeded)
            context.Succeed(requirement);
        else
            context.Fail(new(handler, string.Join(" ", validationResult.Messages)));
        return;
    }
}

using MTUM_Wasm.Shared.Core.Common.Result;
using MTUM_Wasm.Shared.Core.Identity.Entity;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;

namespace MTUM_Wasm.Server.Core.Common.Policy.Nac;

internal static class NacPolicyHelper
{
    public static void ValidateNacRequirement<TRequirement>(AuthorizationHandlerContext context,
        AuthorizationHandler<TRequirement> handler,
        TRequirement requirement,
        NacPolicy? nacPolicy,
        IServiceResult<string> remoteIpAddress) where TRequirement : IAuthorizationRequirement
    {
        //No nac policy is defined
        if (nacPolicy is null)
        {
            context.Succeed(requirement);
            return;
        }

        //check for blacklisted
        if (nacPolicy.Blacklist is not null)
        {
            if (!remoteIpAddress.Succeeded)
            {
                context.Fail(new(handler, "Cannot determine client's IP address."));
                return;
            }
            if (nacPolicy.Blacklist.Any(p => p.Equals(remoteIpAddress.Data, StringComparison.OrdinalIgnoreCase)))
            {
                context.Fail(new(handler, "Client IP is blacklisted."));
                return;
            }
        }
        //check safelist if only IPs in safelist are allowed
        if (nacPolicy.UseSafelist)
        {
            if (!remoteIpAddress.Succeeded)
            {
                context.Fail(new(handler, "Cannot determine client's IP address."));
                return;
            }
            //if safelist is not provided or user IP is not in safelist
            if (nacPolicy.Safelist is null ||
                (nacPolicy.Safelist is not null && !nacPolicy.Safelist.Any(p => p.Equals(remoteIpAddress.Data, StringComparison.OrdinalIgnoreCase))))
            {
                context.Fail(new(handler, "Client IP is not in safelist."));
                return;
            }

        }
        //else nac policy requirements are met
        context.Succeed(requirement);
        return;
    }
}

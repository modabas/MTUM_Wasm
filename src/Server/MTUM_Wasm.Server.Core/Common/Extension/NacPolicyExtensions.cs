using MTUM_Wasm.Shared.Core.Common.Result;
using MTUM_Wasm.Shared.Core.Identity.Entity;
using System;
using System.Linq;

namespace MTUM_Wasm.Server.Core.Common.Extension;

internal static class NacPolicyExtensions
{
    public static IServiceResult Validate(this NacPolicy nacPolicy, IServiceResult<string> remoteIpAddressResult)
    {
        //check for blacklisted
        if (nacPolicy.Blacklist is not null)
        {
            if (!remoteIpAddressResult.Succeeded)
            {
                return Result.Fail("Cannot determine client's IP address.");
            }
            if (nacPolicy.Blacklist.Any(p => p.Equals(remoteIpAddressResult.Data, StringComparison.OrdinalIgnoreCase)))
            {
                return Result.Fail("Client IP is blacklisted.");
            }
        }
        //check safelist if only IPs in safelist are allowed
        if (nacPolicy.UseSafelist)
        {
            if (!remoteIpAddressResult.Succeeded)
            {
                return Result.Fail("Cannot determine client's IP address.");
            }
            //if safelist is not provided or user IP is not in safelist
            if (nacPolicy.Safelist is null ||
                (nacPolicy.Safelist is not null && !nacPolicy.Safelist.Any(p => p.Equals(remoteIpAddressResult.Data, StringComparison.OrdinalIgnoreCase))))
            {
                return Result.Fail("Client IP is not in safelist.");
            }

        }
        //else nac policy requirements are met
        return Result.Success();
    }
}

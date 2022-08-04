using Microsoft.AspNetCore.Authorization;

namespace MTUM_Wasm.Server.Core.Common.Policy.TenantEnabled;

internal class HasEnabledTenantRequirement : IAuthorizationRequirement
{
    public HasEnabledTenantRequirement()
    {
    }
}
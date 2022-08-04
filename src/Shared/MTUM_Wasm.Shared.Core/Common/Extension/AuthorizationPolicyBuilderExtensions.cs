using MTUM_Wasm.Shared.Core.Common.Authorization;
using MTUM_Wasm.Shared.Core.Identity.Policy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace MTUM_Wasm.Shared.Core.Common.Extension;

public static class AuthorizationPolicyBuilderExtensions
{
    public static AuthorizationPolicyBuilder AddIsTenantAdminPolicy(this AuthorizationPolicyBuilder builder)
    {
        return builder.AddRequirements(new TenantClaimRequirement(), new RolesAuthorizationRequirement(new string[] { Role.Name.TenantAdmin }));
    }

    public static AuthorizationPolicyBuilder AddIsTenantUserOrUpPolicy(this AuthorizationPolicyBuilder builder)
    {
        return builder.AddRequirements(new TenantClaimRequirement(), new RolesAuthorizationRequirement(new string[] { Role.Name.TenantUser, Role.Name.TenantAdmin }));
    }

    public static AuthorizationPolicyBuilder AddIsTenantViewerOrUpPolicy(this AuthorizationPolicyBuilder builder)
    {
        return builder.AddRequirements(new TenantClaimRequirement(), new RolesAuthorizationRequirement(new string[] { Role.Name.TenantViewer, Role.Name.TenantUser, Role.Name.TenantAdmin }));
    }

    public static AuthorizationPolicyBuilder AddIsSystemAdminPolicy(this AuthorizationPolicyBuilder builder)
    {
        return builder.AddRequirements(new RolesAuthorizationRequirement(new string[] { Role.Name.SystemAdmin }));
    }

}

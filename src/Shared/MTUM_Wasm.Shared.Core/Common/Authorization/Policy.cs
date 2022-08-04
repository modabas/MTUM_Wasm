namespace MTUM_Wasm.Shared.Core.Common.Authorization;

public class Policy
{
    public class Name
    {
        public const string IsTenantAdmin = "IsTenantAdminPolicy";
        public const string IsTenantUserOrUp = "IsTenantUserOrUpPolicy";
        public const string IsTenantViewerOrUp = "IsTenantViewerOrUpPolicy";
        public const string IsSystemAdmin = "IsSystemAdminPolicy";
        public const string UserNac = "UserNacPolicy";
        public const string HasEnabledTenant = "HasEnabledTenantPolicy";
        public const string TenantNac = "TenantNacPolicy";
    }
}

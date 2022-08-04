namespace MTUM_Wasm.Client.Core.Utility
{
    internal class PageUri
    {
        internal class Authentication
        {
            public const string Login = "/authentication/login";
            public const string ChangePassword = "/authentication/changePassword";
            public const string ForgotPassword = "/authentication/forgotPassword";
            public const string ResetPassword = "/authentication/resetPassword";
        }

        internal class TenantAdmin
        {
            public const string Users = "/tenantAdmin/users";
            public const string TenantNacPolicy = "/tenantAdmin/tenantNacPolicy";
            public const string AuditLogs = "/tenantAdmin/auditLogs";
        }

        internal class SystemAdmin
        {
            public const string Tenants = "/systemAdmin/tenants";
            public const string TenantUsers = "/systemAdmin/tenantUsers";
            public const string CreateUser = "/systemAdmin/createUser";
            public const string AuditLogs = "/systemAdmin/auditLogs";
            public const string SystemAdmins = "/systemAdmin/systemAdmins";
        }
    }
}

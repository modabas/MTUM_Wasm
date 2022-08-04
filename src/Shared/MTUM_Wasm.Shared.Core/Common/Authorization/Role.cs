using System.Collections.Generic;
using System.Linq;

namespace MTUM_Wasm.Shared.Core.Common.Authorization;

public class Role
{
    public class Name
    {
        public const string SystemAdmin = "systemAdmin";
        public const string TenantAdmin = "tenantAdmin";
        public const string TenantUser = "tenantUser";
        public const string TenantViewer = "tenantViewer";

        public static IEnumerable<string> PossibleTenantRoles
        {
            get
            {
                return new List<string>(new string[] { Role.Name.TenantUser, Role.Name.TenantViewer, Role.Name.TenantAdmin }).AsReadOnly();
            }
        }

        public static IEnumerable<string> PossibleSystemRoles
        {
            get
            {
                return new List<string>(new string[] { Role.Name.SystemAdmin }).AsReadOnly();
            }
        }

        public static IEnumerable<string> AllPossibleGroups
        {
            get
            {
                return PossibleTenantRoles.Union(PossibleSystemRoles).ToList().AsReadOnly();
            }
        }
    }
}

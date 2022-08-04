using Microsoft.AspNetCore.Authorization;

namespace MTUM_Wasm.Server.Core.Common.Policy.Nac;

internal class TenantNacRequirement : IAuthorizationRequirement
{
    public TenantNacRequirement()
    {
    }
}

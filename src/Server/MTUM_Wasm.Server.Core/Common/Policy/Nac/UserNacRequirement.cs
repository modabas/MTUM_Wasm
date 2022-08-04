using Microsoft.AspNetCore.Authorization;

namespace MTUM_Wasm.Server.Core.Common.Policy.Nac;

internal class UserNacRequirement : IAuthorizationRequirement
{
    public UserNacRequirement()
    {
    }
}

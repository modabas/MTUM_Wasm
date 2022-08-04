using MTUM_Wasm.Shared.Core.Identity.Entity;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.Identity.Service
{
    internal interface IUserAccessor
    {
        ClaimsPrincipal? User { get; }

        Task<ICurrentUser> GetCurrentUser(CancellationToken cancellationToken);
    }
}

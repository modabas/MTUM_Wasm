using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Client.Core.Identity.Service;

internal interface ITokenStore
{
    Task<T> GetTokenProperty<T>(TokenPropertyNameEnum tokenPropertyName, CancellationToken cancellationToken);
    Task SetTokenProperty<T>(TokenPropertyNameEnum tokenPropertyName, T value, CancellationToken cancellationToken);
    Task RemoveTokenProperty(TokenPropertyNameEnum tokenPropertyName, CancellationToken cancellationToken);
}

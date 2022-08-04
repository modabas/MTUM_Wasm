using System.Threading.Tasks;
using Toolbelt.Blazor;

namespace MTUM_Wasm.Client.Core.Identity.Http;

internal interface IHttpInterceptorService
{
    void DisposeEvent();
    Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs e);
    void RegisterEvent();
}

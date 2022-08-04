using MTUM_Wasm.Client.Core.Identity.Http;
using MTUM_Wasm.Client.Core.Identity.Service;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace MTUM_Wasm.Client.Web;

public partial class App : IDisposable
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Inject] IHttpInterceptorService HttpInterceptorService { get; set; }
    [Inject] RefreshingAuthenticationStateProvider ApplicationStateProvider { get; set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    protected override async Task OnInitializedAsync()
    {
        HttpInterceptorService.RegisterEvent();

        //to fire off authentication revalidation loop at application startup if necessary (in case user sign in state and tokens are preserved in between application startups)
        await ApplicationStateProvider.StateChangedAsync();
    }

    public void Dispose()
    {
        HttpInterceptorService.DisposeEvent();
    }
}

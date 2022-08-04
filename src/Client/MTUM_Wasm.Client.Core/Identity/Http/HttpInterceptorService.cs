using MTUM_Wasm.Client.Core.Identity.Service;
using MTUM_Wasm.Client.Core.Utility;
using MTUM_Wasm.Client.Core.Utility.MessageDisplay;
using MTUM_Wasm.Client.Core.Utility.Spinner;
using MTUM_Wasm.Shared.Core.Common.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Toolbelt.Blazor;

namespace MTUM_Wasm.Client.Core.Identity.Http;

internal class HttpInterceptorService : IHttpInterceptorService
{
    private readonly HttpClientInterceptor _interceptor;
    private readonly IIdentityService _identityService;
    private readonly NavigationManager _navigationManager;
    private readonly IMessageDisplayService _messageDisplay;
    private readonly ISpinnerService _spinnerService;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public HttpInterceptorService(
        HttpClientInterceptor interceptor,
        IIdentityService identityService,
        NavigationManager navigationManager,
        IMessageDisplayService messageDisplay, 
        ISpinnerService spinnerService,
        AuthenticationStateProvider authenticationStateProvider)
    {
        _interceptor = interceptor;
        _identityService = identityService;
        _navigationManager = navigationManager;
        _messageDisplay = messageDisplay;
        _spinnerService = spinnerService;
        _authenticationStateProvider = authenticationStateProvider;
    }

    public void RegisterEvent()
    {
        _interceptor.BeforeSendAsync += InterceptBeforeHttpAsync;
        _interceptor.AfterSendAsync += InterceptAfterHttpAsync;
    }

    public async Task InterceptAfterHttpAsync(object sender, HttpClientInterceptorEventArgs e)
    {
        HideSpinner();
        try
        {
            var absPath = e.Request.RequestUri?.AbsolutePath ?? string.Empty;
            if (!absPath.Contains("api/Identity/", StringComparison.OrdinalIgnoreCase) && 
                e.Response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var result = await _identityService.Logout(LogoutTypeEnum.Local, default);
                if (result.Succeeded)
                {
                    ((RefreshingAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
                    _navigationManager.NavigateTo(_navigationManager.Uri);
                }
                return;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            _messageDisplay.ShowError(ex.Message);
        }
    }

    public Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs e)
    {
        ShowSpinner();
        return Task.CompletedTask;
    }

    public void DisposeEvent()
    {
        _interceptor.BeforeSendAsync -= InterceptBeforeHttpAsync;
        _interceptor.AfterSendAsync -= InterceptAfterHttpAsync;
    }

    private void ShowSpinner()
    {
        try
        {
            _spinnerService.Show();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            _messageDisplay.ShowError(ex.Message);
        }
    }

    private void HideSpinner()
    {
        try
        {
            _spinnerService.Hide();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            _messageDisplay.ShowError(ex.Message);
        }
    }
}

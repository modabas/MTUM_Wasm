using MTUM_Wasm.Client.Core.Identity.Service;
using MTUM_Wasm.Client.Core.Utility;
using MTUM_Wasm.Client.Core.Utility.MessageDisplay;
using MTUM_Wasm.Client.Core.Utility.QueryHelpers;
using MTUM_Wasm.Shared.Core.Identity.Dto;
using MTUM_Wasm.Shared.Core.Identity.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MTUM_Wasm.Client.Web.Pages.Authentication;

[Route(PageUri.Authentication.Login)]
[AllowAnonymous]
public partial class Login
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Inject] internal NavigationManager NavigationManager { get; set; }
    [Inject] internal IIdentityService IdentityService { get; set; }
    [Inject] internal IMessageDisplayService MessageDisplayService { get; set; }
    [Inject] internal RefreshingAuthenticationStateProvider ApplicationStateProvider { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


    private readonly LoginRequest _loginRequest = new();
    private readonly LoginRequestValidator _loginRequestValidator = new();
    private MudForm? _form;
    public IMask emailMask = RegexMask.Email();

    private bool _passwordVisibility;
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

    private void TogglePasswordVisibility()
    {
        if (_passwordVisibility)
        {
            _passwordVisibility = false;
            _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
            _passwordInput = InputType.Password;
        }
        else
        {
            _passwordVisibility = true;
            _passwordInputIcon = Icons.Material.Filled.Visibility;
            _passwordInput = InputType.Text;
        }
    }

    private async Task SignIn()
    {
        try
        {
            if (_form is null)
            {
                MessageDisplayService.ShowError("Critical error. Try restarting application.");
                return;
            }
            await _form.Validate();
            if (_form.IsValid)
            {
                var ret = await IdentityService.Login(_loginRequest, default);
                if (ret.Succeeded)
                {
                    await ApplicationStateProvider.StateChangedAsync();
                    var currentUri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);

                    if (QueryHelpers.ParseQuery(currentUri.Query).TryGetValue("returnUrl", out var param))
                    {
                        var returnUrlParam = param.First();
                        NavigationManager.NavigateTo(returnUrlParam);
                    }
                    else
                    {
                        NavigationManager.NavigateTo("");
                    }
                }
                else
                {
                    MessageDisplayService.ShowError(ret.Messages);
                }
            }
        }
        catch (Exception ex)
        {
            MessageDisplayService.ShowError(ex.Message);
        }
    }
}

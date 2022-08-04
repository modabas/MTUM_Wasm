using MTUM_Wasm.Client.Core.Identity.Service;
using MTUM_Wasm.Client.Core.Utility;
using MTUM_Wasm.Client.Core.Utility.MessageDisplay;
using MTUM_Wasm.Shared.Core.Identity.Dto;
using MTUM_Wasm.Shared.Core.Identity.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Threading.Tasks;

namespace MTUM_Wasm.Client.Web.Pages.Authentication;

[Route(PageUri.Authentication.ResetPassword)]
[AllowAnonymous]
public partial class ResetPassword
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Inject] internal NavigationManager NavigationManager { get; set; }
    [Inject] internal IIdentityService IdentityService { get; set; }
    [Inject] internal IMessageDisplayService MessageDisplayService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


    private readonly ResetPasswordRequest _resetPasswordRequest = new();
    private readonly ResetPasswordRequestValidator _resetPasswordRequestValidator = new();
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

    private async Task ResetUserPassword()
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
                var ret = await IdentityService.ResetPassword(_resetPasswordRequest, default);
                if (ret.Succeeded)
                {
                    MessageDisplayService.ShowSuccess("Password has been changed successfully.");
                    NavigationManager.NavigateTo(PageUri.Authentication.Login);
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

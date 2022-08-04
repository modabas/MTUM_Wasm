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

[Route(PageUri.Authentication.ForgotPassword)]
[AllowAnonymous]
public partial class ForgotPassword
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Inject] internal NavigationManager NavigationManager { get; set; }
    [Inject] internal IIdentityService IdentityService { get; set; }
    [Inject] internal IMessageDisplayService MessageDisplayService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


    private readonly ForgotPasswordRequest _forgotPasswordRequest = new();
    private readonly ForgotPasswordRequestValidator _forgotPasswordRequestValidator = new();
    private MudForm? _form;
    public IMask emailMask = RegexMask.Email();

    private async Task Submit()
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
                var ret = await IdentityService.ForgotPassword(_forgotPasswordRequest, default);
                if (ret.Succeeded)
                {
                    NavigationManager.NavigateTo(PageUri.Authentication.ResetPassword);
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

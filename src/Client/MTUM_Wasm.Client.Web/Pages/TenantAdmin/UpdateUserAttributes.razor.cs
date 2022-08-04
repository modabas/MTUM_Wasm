using MTUM_Wasm.Client.Core.TenantAdmin.Service;
using MTUM_Wasm.Client.Core.Utility.MessageDisplay;
using MTUM_Wasm.Shared.Core.Common.Extension;
using MTUM_Wasm.Shared.Core.TenantAdmin.Dto;
using MTUM_Wasm.Shared.Core.TenantAdmin.Validation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MTUM_Wasm.Client.Web.Pages.TenantAdmin;

public partial class UpdateUserAttributes
{
    [CascadingParameter] private MudDialogInstance? MudDialog { get; set; }
    [CascadingParameter] Task<AuthenticationState>? AuthenticationState { get; set; }
    [Parameter] public string Email { get; set; } = string.Empty;

    private string _loggedInEmail = string.Empty;
    private UpdateUserAttributesRequest _updateUserAttributesRequest = new();
    private readonly UpdateUserAttributesRequestValidator _updateUserAttributesRequestValidator = new();
    private ChangeUserStateRequest _changeUserStateRequest = new();
    private MudForm? _form;
    private string _enabledButtonText = "Enable User";
    private bool _enabledButtonIsDisabled = false;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Inject] internal ITenantAdminService TenantAdminService { get; set; }
    [Inject] internal IMessageDisplayService MessageDisplayService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    protected override async Task OnInitializedAsync()
    {
        if (AuthenticationState is not null)
        {
            var identity = (await AuthenticationState).User.Identity;
            var claimsIdentity = (ClaimsIdentity?)identity;
            _loggedInEmail = claimsIdentity?.GetEmail() ?? string.Empty;
        }
        await GetUser();
    }

    private async Task GetUser()
    {
        try
        {
            var request = new GetUserRequest() { Email = Email };
            var result = await TenantAdminService.GetUser(request, default);
            if (result.Succeeded && result.Data is not null)
            {
                var user = result.Data.User;
                if (user is not null)
                {
                    _updateUserAttributesRequest = new()
                    {
                        FamilyName = user.FamilyName,
                        GivenName = user.GivenName,
                        MiddleName = user.MiddleName,
                        Email = user.EmailAddress,
                        IsEmailVerified = user.IsEmailVerified
                    };
                    _changeUserStateRequest = new()
                    {
                        Email = user.EmailAddress,
                        SetEnabled = !user.Enabled
                    };
                    SetToggleEnabledButtonProperties(user.EmailAddress);
                    return;
                }
            }
            else
            {
                MessageDisplayService.ShowError(result.Messages);
                MudDialog?.Cancel();
            }
            MessageDisplayService.ShowError("Cannot get user data.");
            MudDialog?.Cancel();
        }
        catch (Exception ex)
        {
            MessageDisplayService.ShowError(ex.Message);
        }
    }

    private void SetToggleEnabledButtonProperties(string selectedEmail)
    {
        if (_changeUserStateRequest.SetEnabled)
        {
            _enabledButtonText = "Enable User";
        }
        else
        {
            _enabledButtonText = "Disable User";
        }
        //don't let logged in user to perform enabled/disabled change on himself
        if (_loggedInEmail.Equals(selectedEmail, StringComparison.OrdinalIgnoreCase))
        {
            _enabledButtonIsDisabled = true;
        }
        else
        {
            _enabledButtonIsDisabled = false;
        }
    }

    private async Task Update()
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
                var ret = await TenantAdminService.UpdateUserAttributes(_updateUserAttributesRequest, default);
                if (ret.Succeeded)
                {
                    MessageDisplayService.ShowSuccess("User attributes updated.");
                    MudDialog?.Close();
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

    private void Cancel()
    {
        try
        {
            MudDialog?.Cancel();
        }
        catch (Exception ex)
        {
            MessageDisplayService.ShowError(ex.Message);
        }
    }

    private async Task ToggleEnabled()
    {
        try
        {
            var ret = await TenantAdminService.ChangeUserState(_changeUserStateRequest, default);
            if (ret.Succeeded)
            {
                MessageDisplayService.ShowSuccess("User state updated.");
                MudDialog?.Close();
            }
            else
            {
                MessageDisplayService.ShowError(ret.Messages);
            }
        }
        catch (Exception ex)
        {
            MessageDisplayService.ShowError(ex.Message);
        }
    }
}

using MTUM_Wasm.Client.Core.Identity.Service;
using MTUM_Wasm.Client.Core.Utility;
using MTUM_Wasm.Client.Core.Utility.MessageDisplay;
using MTUM_Wasm.Shared.Core.Identity.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MTUM_Wasm.Client.Web.Shared;

public partial class LoginDisplay
{
    [CascadingParameter]
    Task<AuthenticationState>? AuthenticationState { get; set; }

    public string WelcomeMessage { get; set; } = string.Empty;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Inject] private ITokenClaimResolver TokenClaimResolver { get; set; }
    [Inject] private IIdentityService IdentityService { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private IMessageDisplayService MessageDisplayService { get; set; }
    [Inject] private RefreshingAuthenticationStateProvider ApplicationStateProvider { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    protected override async Task OnInitializedAsync()
    {
        if (AuthenticationState is not null)
        {
            var identity = (await AuthenticationState).User.Identity;
            var claimsIdentity = (ClaimsIdentity?)identity;
            var currentUser = await TokenClaimResolver.GetCurrentUser(claimsIdentity, default);
            if (currentUser.IsAuthenticated && !string.IsNullOrWhiteSpace(currentUser.FullName))
            {
                WelcomeMessage = $"Hello, {currentUser.FullName}!";
            }
            else
            {
                WelcomeMessage = string.Empty;
            }
        }
    }

    private async Task SignOut(LogoutTypeEnum logoutType)
    {
        var ret = await IdentityService.Logout(logoutType, default);
        if (ret.Succeeded)
        {
            ApplicationStateProvider.MarkUserAsLoggedOut();
            NavigationManager.NavigateTo(PageUri.Authentication.Login);
        }
        else
            MessageDisplayService.ShowError(ret.Messages);
    }
}

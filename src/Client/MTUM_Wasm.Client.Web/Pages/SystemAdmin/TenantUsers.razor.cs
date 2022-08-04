using MTUM_Wasm.Client.Core.SystemAdmin.Service;
using MTUM_Wasm.Client.Core.Utility;
using MTUM_Wasm.Client.Core.Utility.MessageDisplay;
using MTUM_Wasm.Shared.Core.Common.Authorization;
using MTUM_Wasm.Shared.Core.Common.Dto;
using MTUM_Wasm.Shared.Core.SystemAdmin.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Threading.Tasks;

namespace MTUM_Wasm.Client.Web.Pages.SystemAdmin;

[Route($"{PageUri.SystemAdmin.TenantUsers}/{{TenantId:guid}}")]
[Authorize(Policy = Policy.Name.IsSystemAdmin)]
public partial class TenantUsers
{
    [Parameter]
    public Guid TenantId { get; set; }

    private GetTenantUsersResponse _getTenantUsersResponse = new();
    private string _searchString = string.Empty;
    private string _tenantNameLabel = "Users";

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Inject] internal ISystemAdminService SystemAdminService { get; set; }
    [Inject] internal IMessageDisplayService MessageDisplayService { get; set; }
    [Inject] internal IDialogService DialogService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


    private Func<SystemUserDto, bool> _quickFilter => x =>
    {
        if (string.IsNullOrWhiteSpace(_searchString))
            return true;

        if (x.FullName.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
            return true;

        if (x.EmailAddress.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
            return true;

        if (x.UserStatus.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    };

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await GetUserList();
            await SetTenantLabel();
        }
        catch (Exception ex)
        {
            MessageDisplayService.ShowError(ex.Message);
        }
    }

    public async Task ShowUpdateUserDialog(string email)
    {
        try
        {
            var parameters = new DialogParameters();
            parameters.Add(nameof(UpdateUserAttributes.TenantId), TenantId);
            parameters.Add(nameof(UpdateUserAttributes.Email), email);
            var options = new DialogOptions { CloseButton = false, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };
            var dialog = DialogService.Show<UpdateUserAttributes>("Update user attributes", parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await GetUserList();
            }
        }
        catch (Exception ex)
        {
            MessageDisplayService.ShowError(ex.Message);
        }
    }

    public async Task ShowUpdateUserGroupsDialog(string email)
    {
        try
        {
            var parameters = new DialogParameters();
            parameters.Add(nameof(UpdateUserGroups.TenantId), TenantId);
            parameters.Add(nameof(UpdateUserGroups.Email), email);
            var options = new DialogOptions { CloseButton = false, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };
            var dialog = DialogService.Show<UpdateUserGroups>("Update user groups", parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await GetUserList();
            }
        }
        catch (Exception ex)
        {
            MessageDisplayService.ShowError(ex.Message);
        }
    }

    public async Task ShowUpdateUserNacPolicyDialog(string email)
    {
        try
        {
            var parameters = new DialogParameters();
            parameters.Add(nameof(UpdateUserNacPolicy.Email), email);
            var options = new DialogOptions { CloseButton = false, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };
            var dialog = DialogService.Show<UpdateUserNacPolicy>("Update user nac policy", parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await GetUserList();
            }
        }
        catch (Exception ex)
        {
            MessageDisplayService.ShowError(ex.Message);
        }
    }

    private async Task ShowCreateUserDialog()
    {
        try
        {
            var parameters = new DialogParameters();
            parameters.Add(nameof(CreateUser.TenantId), TenantId);
            var options = new DialogOptions { CloseButton = false, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };
            var dialog = DialogService.Show<CreateUser>("Create user", parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await GetUserList();
            }
        }
        catch (Exception ex)
        {
            MessageDisplayService.ShowError(ex.Message);
        }
    }

    private async Task GetUserList()
    {
        var result = await SystemAdminService.GetTenantUsers(new GetTenantUsersRequest() { TenantId = TenantId }, default);
        if (result.Succeeded)
        {
            _getTenantUsersResponse = result.Data ?? new();
        }
        else
        {
            _getTenantUsersResponse = new();
            MessageDisplayService.ShowError(result.Messages);
        }
    }

    private async Task SetTenantLabel()
    {
        var result = await SystemAdminService.GetTenant(new GetTenantRequest() { Id = TenantId }, default);
        _tenantNameLabel = result.Succeeded && result.Data is not null && result.Data.Tenant is not null
            ? $"Users of {result.Data.Tenant.Name}"
            : "Users";
    }

}

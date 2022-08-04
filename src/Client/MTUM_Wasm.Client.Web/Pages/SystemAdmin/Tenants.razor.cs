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

[Route(PageUri.SystemAdmin.Tenants)]
[Authorize(Policy = Policy.Name.IsSystemAdmin)]
public partial class Tenants
{
    private GetTenantsResponse _getTenantsResponse = new();
    private string _searchString = string.Empty;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Inject] internal ISystemAdminService SystemAdminService { get; set; }
    [Inject] internal IMessageDisplayService MessageDisplayService { get; set; }
    [Inject] internal NavigationManager NavigationManager { get; set; }
    [Inject] internal IDialogService DialogService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


    private Func<TenantDto, bool> _quickFilter => x =>
    {
        if (string.IsNullOrWhiteSpace(_searchString))
            return true;

        if (x.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
            return true;

        if (x.Id.ToString().Contains(_searchString, StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    };

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await GetTenantList();
        }
        catch (Exception ex)
        {
            MessageDisplayService.ShowError(ex.Message);
        }
    }

    public async Task ShowUpdateTenantDialog(Guid tenantId)
    {
        try
        {
            var parameters = new DialogParameters();
            parameters.Add(nameof(UpdateTenant.Id), tenantId);
            var options = new DialogOptions { CloseButton = false, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };
            var dialog = DialogService.Show<UpdateTenant>("Update tenant", parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await GetTenantList();
            }
        }
        catch (Exception ex)
        {
            MessageDisplayService.ShowError(ex.Message);
        }
    }

    public void ListTenantUsers(Guid tenantId)
    {
        try
        {
            NavigationManager.NavigateTo($"{PageUri.SystemAdmin.TenantUsers}/{tenantId}");
        }
        catch (Exception ex)
        {
            MessageDisplayService.ShowError(ex.Message);
        }
    }

    private async Task ShowCreateTenantDialog()
    {
        try
        {
            var options = new DialogOptions { CloseButton = false, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };
            var dialog = DialogService.Show<CreateTenant>("Create tenant", options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await GetTenantList();
            }
        }
        catch (Exception ex)
        {
            MessageDisplayService.ShowError(ex.Message);
        }
    }

    private async Task GetTenantList()
    {
        var result = await SystemAdminService.GetTenants(default);
        if (result.Succeeded)
        {
            _getTenantsResponse = result.Data ?? new();
        }
        else
        {
            _getTenantsResponse = new();
            MessageDisplayService.ShowError(result.Messages);
        }
    }

    public async Task ShowUpdateTenantNacPolicyDialog(Guid tenantId)
    {
        try
        {
            var parameters = new DialogParameters();
            parameters.Add(nameof(UpdateTenantNacPolicy.Id), tenantId);
            var options = new DialogOptions { CloseButton = false, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };
            var dialog = DialogService.Show<UpdateTenantNacPolicy>("Update tenant nac policy", parameters, options);
            _ = await dialog.Result;
         }
        catch (Exception ex)
        {
            MessageDisplayService.ShowError(ex.Message);
        }
    }
}


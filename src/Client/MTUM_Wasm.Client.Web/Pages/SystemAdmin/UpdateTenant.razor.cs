using MTUM_Wasm.Client.Core.SystemAdmin.Service;
using MTUM_Wasm.Client.Core.Utility.MessageDisplay;
using MTUM_Wasm.Shared.Core.SystemAdmin.Dto;
using MTUM_Wasm.Shared.Core.SystemAdmin.Validation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Threading.Tasks;

namespace MTUM_Wasm.Client.Web.Pages.SystemAdmin;

public partial class UpdateTenant
{
    [CascadingParameter] private MudDialogInstance? MudDialog { get; set; }

    [Parameter] public Guid Id { get; set; }

    private UpdateTenantRequest _updateTenantRequest = new();
    private readonly UpdateTenantRequestValidator _updateTenantRequestValidator = new();
    private MudForm? _form;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Inject] internal ISystemAdminService SystemAdminService { get; set; }
    [Inject] internal IMessageDisplayService MessageDisplayService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await GetTenant();
        }
        catch (Exception ex)
        {
            MessageDisplayService.ShowError(ex.Message);
        }
    }

    private async Task GetTenant()
    {
        var request = new GetTenantRequest() { Id = Id };
        var result = await SystemAdminService.GetTenant(request, default);
        if (result.Succeeded && result.Data is not null)
        {
            var tenant = result.Data.Tenant;
            if (tenant is not null)
            {
                _updateTenantRequest = new()
                {
                    Id = tenant.Id,
                    IsEnabled = tenant.IsEnabled,
                    Name = tenant.Name
                };
                return;
            }
        }
        else
        {
            MessageDisplayService.ShowError(result.Messages);
            MudDialog?.Cancel();
        }
        MessageDisplayService.ShowError("Cannot get tenant data.");
        MudDialog?.Cancel();
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
                var ret = await SystemAdminService.UpdateTenant(_updateTenantRequest, default);
                if (ret.Succeeded)
                {
                    MessageDisplayService.ShowSuccess("Tenant updated.");
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


}

using MTUM_Wasm.Client.Core.TenantAdmin.Service;
using MTUM_Wasm.Client.Core.TenantViewerOrUp.Service;
using MTUM_Wasm.Client.Core.Utility;
using MTUM_Wasm.Client.Core.Utility.MessageDisplay;
using MTUM_Wasm.Client.Core.Utility.Validation;
using MTUM_Wasm.Shared.Core.Common.Authorization;
using MTUM_Wasm.Shared.Core.Common.Extension;
using MTUM_Wasm.Shared.Core.Identity.Entity;
using MTUM_Wasm.Shared.Core.TenantAdmin.Dto;
using MTUM_Wasm.Shared.Core.TenantAdmin.Validation;
using MTUM_Wasm.Shared.Core.TenantViewerOrUp.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MTUM_Wasm.Client.Web.Pages.TenantAdmin;

[Route(PageUri.TenantAdmin.TenantNacPolicy)]
[Authorize(Policy = Policy.Name.IsTenantAdmin)]
public partial class TenantNacPolicy
{
    private readonly IPAddressValidator _ipAddressValidator = new();
    private readonly UpdateTenantNacPolicyRequestValidator _updateTenantNacPolicyRequestValidator = new();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Inject] internal ITenantAdminService TenantAdminService { get; set; }
    [Inject] internal IMessageDisplayService MessageDisplayService { get; set; }
    [Inject] internal ITenantViewerOrUpService TenantViewerOrUpService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private bool _useSafelist;
    private string _safeIPAddress = string.Empty;
    private List<string> _safeIPAddressList = new();
    private string _blackIPAddress = string.Empty;
    private List<string> _blackIPAddressList = new();
    private bool _updateButtonEnabled;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await GetOwnTenant();
        }
        catch (Exception ex)
        {
            MessageDisplayService.ShowError(ex.Message);
        }
    }

    private async Task GetOwnTenant()
    {
        var result = await TenantViewerOrUpService.GetTenant(default);
        if (result.Succeeded && result.Data is not null)
        {
            var tenant = result.Data.Tenant;
            if (tenant is not null)
            {
                if (tenant.NacPolicy is not null)
                {
                    var nacPolicy = tenant.NacPolicy;
                    _safeIPAddressList = nacPolicy.Safelist.ToList();
                    _blackIPAddressList = nacPolicy.Blacklist.ToList();
                    _useSafelist = nacPolicy.UseSafelist;
                }
                _updateButtonEnabled = true;
                return;
            }
        }
        else
        {
            MessageDisplayService.ShowError(result.Messages);
        }
        MessageDisplayService.ShowError("Cannot get tenant data.");
    }

    private async Task Update()
    {
        try
        {
            var updateUserNacPolicyRequest = new UpdateTenantNacPolicyRequest()
            {
                NacPolicy = new NacPolicy()
                {
                    UseSafelist = _useSafelist,
                    Safelist = _safeIPAddressList.ToArray(),
                    Blacklist = _blackIPAddressList.ToArray()
                }
            };
            var results = await _updateTenantNacPolicyRequestValidator.ValidateAsync(updateUserNacPolicyRequest);
            if (results.IsValid)
            {
                var ret = await TenantAdminService.UpdateTenantNacPolicy(updateUserNacPolicyRequest, default);
                if (ret.Succeeded)
                {
                    MessageDisplayService.ShowSuccess("Tenant nac policy updated.");
                }
                else
                {
                    MessageDisplayService.ShowError(ret.Messages);
                }
            }
            else
            {
                var errorMessages = results.Errors.Select(r => r.ErrorMessage).ToList();
                MessageDisplayService.ShowWarning(errorMessages);
            }
        }
        catch (Exception ex)
        {
            MessageDisplayService.ShowError(ex.Message);
        }
    }

    private async Task AddSafelistIPAddress()
    {
        try
        {
            var ipAddress = _safeIPAddress;
            var results = await _ipAddressValidator.ValidateAsync(ipAddress);
            //Add to safelist if it doesn't exist
            if (results.IsValid)
            {
                if (_safeIPAddressList.Any(x => x.Equals(ipAddress, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageDisplayService.ShowWarning("IP already exists in list.");
                }
                else
                {
                    _safeIPAddressList.Add(ipAddress);
                }
            }
            else
            {
                var errorMessages = results.Errors.Select(r => r.ErrorMessage).ToList();
                MessageDisplayService.ShowWarning(errorMessages);
            }
        }
        catch (Exception ex)
        {
            MessageDisplayService.ShowError(ex.Message);
        }
    }

    private void RemoveSafelistIPAddress(MudChip chip)
    {
        try
        {
            _safeIPAddressList.Remove(chip.Text);
        }
        catch (Exception ex)
        {
            MessageDisplayService.ShowError(ex.Message);
        }
    }

    private async Task AddBlacklistIPAddress()
    {
        try
        {
            var ipAddress = _blackIPAddress;
            var results = await _ipAddressValidator.ValidateAsync(ipAddress);
            //Add to blacklist if it doesn't exist
            if (results.IsValid)
            {
                if (_blackIPAddressList.Any(x => x.Equals(ipAddress, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageDisplayService.ShowWarning("IP already exists in list.");
                }
                else
                {
                    _blackIPAddressList.Add(ipAddress);
                }
            }
            else
            {
                var errorMessages = results.Errors.Select(r => r.ErrorMessage).ToList();
                MessageDisplayService.ShowWarning(errorMessages);
            }
        }
        catch (Exception ex)
        {
            MessageDisplayService.ShowError(ex.Message);
        }
    }

    private void RemoveBlacklistIPAddress(MudChip chip)
    {
        try
        {
            _blackIPAddressList.Remove(chip.Text);
        }
        catch (Exception ex)
        {
            MessageDisplayService.ShowError(ex.Message);
        }
    }
}

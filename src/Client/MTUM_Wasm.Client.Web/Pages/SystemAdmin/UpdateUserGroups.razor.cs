using MTUM_Wasm.Client.Core.SystemAdmin.Service;
using MTUM_Wasm.Client.Core.Utility.MessageDisplay;
using MTUM_Wasm.Shared.Core.Common.Authorization;
using MTUM_Wasm.Shared.Core.SystemAdmin.Dto;
using MTUM_Wasm.Shared.Core.SystemAdmin.Validation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTUM_Wasm.Client.Web.Pages.SystemAdmin;

public partial class UpdateUserGroups
{
    [CascadingParameter] private MudDialogInstance? MudDialog { get; set; }
    [Parameter] public string Email { get; set; } = string.Empty;
    [Parameter] public Guid TenantId { get; set; }

    private UpdateUserGroupsRequest _updateUserGroupsRequest = new();
    private readonly UpdateUserGroupsRequestValidator _updateUserGroupsRequestValidator = new();
    private MudForm? _form;
    private IEnumerable<string> _possibleGroups = Role.Name.PossibleTenantRoles;
    private MudChip[] _selectedGroupChips = Array.Empty<MudChip>();
    private List<MudChip> _possibleGroupChips = new List<MudChip>();
    private MudChip PossibleGroupChipRefCollector
    {
        set
        {
            _possibleGroupChips.Add(value);
        }
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Inject] internal ISystemAdminService SystemAdminService { get; set; }
    [Inject] internal IMessageDisplayService MessageDisplayService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    protected override async Task OnInitializedAsync()
    {
        await GetUserGroups();
    }

    private async Task GetUserGroups()
    {
        try
        {
            var request = new GetUserGroupsRequest() { Email = Email };
            var result = await SystemAdminService.GetUserGroups(request, default);
            if (result.Succeeded && result.Data is not null)
            {
                var currentGroups = result.Data.Groups;
                _selectedGroupChips = _possibleGroupChips.Where(c => currentGroups.Contains(c.Text, StringComparer.OrdinalIgnoreCase)).ToArray();
                _updateUserGroupsRequest = new()
                {
                    Email = Email,
                    OpMode = UpdateUserGroupsOpMode.PossibleTenantRoles
                };
                return;
            }
            else
            {
                MessageDisplayService.ShowError(result.Messages);
                MudDialog?.Cancel();
            }
            MessageDisplayService.ShowError("Cannot get user groups.");
            MudDialog?.Cancel();
        }
        catch (Exception ex)
        {
            MessageDisplayService.ShowError(ex.Message);
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
                _updateUserGroupsRequest.NewGroups = _selectedGroupChips.Select(c => c.Text).ToList().AsReadOnly();

                var ret = await SystemAdminService.UpdateUserGroups(_updateUserGroupsRequest, default);
                if (ret.Succeeded)
                {
                    MessageDisplayService.ShowSuccess("User groups updated.");
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
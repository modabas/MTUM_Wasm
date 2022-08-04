using MTUM_Wasm.Client.Core.SystemAdmin.Service;
using MTUM_Wasm.Client.Core.Utility;
using MTUM_Wasm.Client.Core.Utility.MessageDisplay;
using MTUM_Wasm.Shared.Core.Common.Authorization;
using MTUM_Wasm.Shared.Core.Common.Dto;
using MTUM_Wasm.Shared.Core.SystemAdmin.Dto;
using MTUM_Wasm.Shared.Core.SystemAdmin.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MTUM_Wasm.Client.Web.Pages.SystemAdmin
{
    [Route(PageUri.SystemAdmin.SystemAdmins)]
    [Authorize(Policy = Policy.Name.IsSystemAdmin)]
    public partial class SystemAdmins
    {
        private GetUsersInGroupResponse _getUsersInGroupResponse = new();
        private string _searchString = string.Empty;
        private UpdateUserGroupsRequest _addUserToSystemAdminsRequest = new() { NewGroups = new string[] { Role.Name.SystemAdmin }, OpMode = UpdateUserGroupsOpMode.PossibleSystemRoles };
        private readonly UpdateUserGroupsRequestValidator _addUserToSystemAdminsRequestValidator = new();
        private MudForm? _form;
        public IMask emailMask = RegexMask.Email();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [Inject] internal ISystemAdminService SystemAdminService { get; set; }
        [Inject] internal IMessageDisplayService MessageDisplayService { get; set; }
        [Inject] internal NavigationManager NavigationManager { get; set; }
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
            }
            catch (Exception ex)
            {
                MessageDisplayService.ShowError(ex.Message);
            }
        }

        private async Task AddUserToSystemAdmins()
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
                    bool? result = await DialogService.ShowMessageBox(
                        "Confirmation",
                        $"Do you want to add '{_addUserToSystemAdminsRequest.Email}' to system administrators group?",
                        yesText: "Add", cancelText: "Cancel");
                    bool cancelled = result == null;

                    if (cancelled)
                        return;

                    //No system roles
                    var newGroups = Enumerable.Empty<string>();
                    var updateGroupsResult = await SystemAdminService.UpdateUserGroups(_addUserToSystemAdminsRequest, default);
                    if (updateGroupsResult.Succeeded)
                    {
                        MessageDisplayService.ShowSuccess("User added to system admins.");
                        await GetUserList();
                    }
                    else
                    {
                        MessageDisplayService.ShowError(updateGroupsResult.Messages);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageDisplayService.ShowError(ex.Message);
            }
        }

        private async Task ShowUpdateUserDialog(string email)
        {
            try
            {
                var parameters = new DialogParameters();
                parameters.Add(nameof(UpdateUserAttributes.TenantId), Guid.Empty);
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

        private async Task RemoveUserFromSystemAdmins(string email)
        {
            try
            {
                bool? result = await DialogService.ShowMessageBox(
                    "Confirmation",
                    $"Do you want to remove '{email}' from system administrators group?",
                    yesText: "Remove", cancelText: "Cancel");
                bool cancelled = result == null;

                if (cancelled)
                    return;

                //No system roles
                var newGroups = Enumerable.Empty<string>();
                var updateGroupsResult = await SystemAdminService.UpdateUserGroups(new UpdateUserGroupsRequest() { Email = email, NewGroups = newGroups, OpMode = UpdateUserGroupsOpMode.PossibleSystemRoles }, default);
                if (updateGroupsResult.Succeeded)
                {
                    MessageDisplayService.ShowSuccess("User groups updated.");
                    await GetUserList();
                }
                else
                {
                    MessageDisplayService.ShowError(updateGroupsResult.Messages);
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
                parameters.Add(nameof(CreateUser.TenantId), Guid.Empty);
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
            var result = await SystemAdminService.GetUsersInGroup(new GetUsersInGroupRequest() { GroupName = Role.Name.SystemAdmin }, default);
            if (result.Succeeded)
            {
                _getUsersInGroupResponse = result.Data ?? new();
            }
            else
            {
                _getUsersInGroupResponse = new();
                MessageDisplayService.ShowError(result.Messages);
            }
        }


    }
}

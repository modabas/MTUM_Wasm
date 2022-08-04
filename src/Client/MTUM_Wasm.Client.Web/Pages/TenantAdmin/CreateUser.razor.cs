using MTUM_Wasm.Client.Core.TenantAdmin.Service;
using MTUM_Wasm.Client.Core.Utility.MessageDisplay;
using MTUM_Wasm.Client.Core.Utility.PasswordGenerator;
using MTUM_Wasm.Shared.Core.TenantAdmin.Dto;
using MTUM_Wasm.Shared.Core.TenantAdmin.Validation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Threading.Tasks;

namespace MTUM_Wasm.Client.Web.Pages.TenantAdmin;

public partial class CreateUser
{
    [CascadingParameter] private MudDialogInstance? MudDialog { get; set; }

    private CreateUserRequest _createUserRequest = new();
    private readonly CreateUserRequestValidator _createUserRequestValidator = new();
    private MudForm? _form;
    public IMask emailMask = RegexMask.Email();

    private bool _passwordVisibility;
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Inject] internal ITenantAdminService TenantAdminService { get; set; }
    [Inject] internal IMessageDisplayService MessageDisplayService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private async Task Create()
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
                var ret = await TenantAdminService.CreateUser(_createUserRequest, default);
                if (ret.Succeeded)
                {
                    MessageDisplayService.ShowSuccess("User created.");
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

    private void GenerateRandomPassword()
    {
        try
        {
            var generator = new PasswordGenerator();
            string password = generator.Generate();
            _createUserRequest.TemporaryPassword = password;
            _createUserRequest.ConfirmPassword = password;
            _form?.Validate();
        }
        catch (Exception ex)
        {
            MessageDisplayService.ShowError(ex.Message);
        }
    }
}

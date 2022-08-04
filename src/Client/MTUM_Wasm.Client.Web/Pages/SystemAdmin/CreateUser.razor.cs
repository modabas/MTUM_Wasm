using MTUM_Wasm.Client.Core.SystemAdmin.Service;
using MTUM_Wasm.Client.Core.Utility.MessageDisplay;
using MTUM_Wasm.Client.Core.Utility.PasswordGenerator;
using MTUM_Wasm.Shared.Core.SystemAdmin.Dto;
using MTUM_Wasm.Shared.Core.SystemAdmin.Validation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Threading.Tasks;

namespace MTUM_Wasm.Client.Web.Pages.SystemAdmin;

public partial class CreateUser
{
    [CascadingParameter] private MudDialogInstance? MudDialog { get; set; }

    [Parameter]
    public Guid TenantId { get; set; }

    private CreateUserRequest _createUserRequest = new();
    private readonly CreateUserRequestValidator _createUserRequestValidator = new();
    private MudForm? _form;
    public IMask emailMask = RegexMask.Email();

    private bool _passwordVisibility;
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Inject] internal ISystemAdminService SystemAdminService { get; set; }
    [Inject] internal IMessageDisplayService MessageDisplayService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    protected override Task OnInitializedAsync()
    {
        try
        {
            _createUserRequest.TenantId = TenantId;
        }
        catch (Exception ex)
        {
            MessageDisplayService.ShowError(ex.Message);
        }
        return Task.CompletedTask;
    }

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
                var ret = await SystemAdminService.CreateUser(_createUserRequest, default);
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

using System;

namespace MTUM_Wasm.Shared.Core.SystemAdmin.Dto;

public class CreateUserRequest
{
    public Guid TenantId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string GivenName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
    public string TemporaryPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

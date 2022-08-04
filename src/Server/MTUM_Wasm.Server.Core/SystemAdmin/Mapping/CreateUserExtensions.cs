using MTUM_Wasm.Server.Core.SystemAdmin.Dto;
using MTUM_Wasm.Shared.Core.SystemAdmin.Dto;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Mapping;

internal static class CreateUserExtensions
{
    public static CreateUserInput ToInput(this CreateUserRequest request)
    {
        return new CreateUserInput
        {
            TenantId = request.TenantId,
            Email = request.Email,
            GivenName = request.GivenName,
            MiddleName = request.MiddleName,
            FamilyName = request.FamilyName,
            TemporaryPassword = request.TemporaryPassword,
            ConfirmPassword = request.ConfirmPassword
        };
    }

    public static Identity.Dto.CreateUserInput ToIdentityInput(this CreateUserInput input)
    {
        return new Identity.Dto.CreateUserInput
        {
            Email = input.Email,
            GivenName = input.GivenName,
            MiddleName = input.MiddleName,
            FamilyName = input.FamilyName,
            TemporaryPassword = input.TemporaryPassword
        };
    }

    public static CreateUserInput MaskForAuditLog(this CreateUserInput dto)
    {
        return new CreateUserInput
        {
            TenantId = dto.TenantId,
            Email = dto.Email,
            GivenName = dto.GivenName,
            MiddleName = dto.MiddleName,
            FamilyName = dto.FamilyName,
            TemporaryPassword = "*",
            ConfirmPassword = "*"
        };
    }
}

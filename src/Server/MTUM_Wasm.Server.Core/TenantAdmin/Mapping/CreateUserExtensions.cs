using MTUM_Wasm.Server.Core.TenantAdmin.Dto;
using MTUM_Wasm.Shared.Core.TenantAdmin.Dto;

namespace MTUM_Wasm.Server.Core.TenantAdmin.Mapping;

internal static class CreateUserExtensions
{
    public static CreateUserInput ToInput(this CreateUserRequest request)
    {
        return new CreateUserInput
        {
            Email = request.Email,
            GivenName = request.GivenName,
            MiddleName = request.MiddleName,
            FamilyName = request.FamilyName,
            TemporaryPassword = request.TemporaryPassword
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

    public static CreateUserInput MaskForAuditLog(this CreateUserInput input)
    {
        return new CreateUserInput
        {
            Email = input.Email,
            GivenName = input.GivenName,
            MiddleName = input.MiddleName,
            FamilyName = input.FamilyName,
            TemporaryPassword = "*"
        };
    }
}

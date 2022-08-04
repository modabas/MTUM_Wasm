using MTUM_Wasm.Server.Core.Common.Mapping;
using MTUM_Wasm.Server.Core.TenantAdmin.Dto;
using MTUM_Wasm.Shared.Core.TenantAdmin.Dto;

namespace MTUM_Wasm.Server.Core.TenantAdmin.Mapping;

internal static class GetUserExtension
{
    public static GetUserInput ToInput(this GetUserRequest request)
    {
        return new GetUserInput
        {
            Email = request.Email
        };
    }

    public static GetUserResponse ToResponse(this GetUserOutput output) => new GetUserResponse
    {
        User = output.User?.ToSharedDto()
    };

    public static Identity.Dto.GetUserInput ToIdentityInput(this GetUserInput input)
    {
        return new Identity.Dto.GetUserInput
        {
            Email = input.Email
        };
    }

    public static GetUserOutput ToTenantAdminOutput(this Identity.Dto.GetUserOutput output)
    {
        return new GetUserOutput
        {
            User = output.User
        };
    }
}

using MTUM_Wasm.Server.Core.Common.Mapping;
using MTUM_Wasm.Server.Core.SystemAdmin.Dto;
using MTUM_Wasm.Shared.Core.SystemAdmin.Dto;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Mapping;

internal static class GetUserExtension
{
    public static GetUserInput ToInput(this GetUserRequest request)
    {
        return new GetUserInput
        {
            Email = request.Email
        };
    }

    public static GetUserResponse ToResponse(this GetUserOutput output)
    {
        return new GetUserResponse
        {
            User = output.User?.ToSharedDto()
        };
    }

    public static Identity.Dto.GetUserInput ToIdentityInput(this GetUserInput input)
    {
        return new Identity.Dto.GetUserInput
        {
            Email = input.Email
        };
    }

    public static GetUserOutput ToSystemAdminDto(this Identity.Dto.GetUserOutput dto)
    {
        return new GetUserOutput
        {
            User = dto.User
        };
    }
}

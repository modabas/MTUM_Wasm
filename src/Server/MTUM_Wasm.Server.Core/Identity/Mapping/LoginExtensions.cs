using MTUM_Wasm.Server.Core.Identity.Dto;
using MTUM_Wasm.Shared.Core.Identity.Dto;

namespace MTUM_Wasm.Server.Core.Identity.Mapping;

internal static class LoginExtensions
{
    public static LoginInput ToInput(this LoginRequest request)
    {
        return new LoginInput
        {
            EmailAddress = request.EmailAddress ?? throw new System.ArgumentNullException(nameof(request), "The value of 'loginRequest.EmailAddress' should not be null"),
            Password = request.Password ?? throw new System.ArgumentNullException(nameof(request), "The value of 'loginRequest.Password' should not be null")
        };
    }

    public static LoginResponse ToResponse(this LoginOutput output)
    {
        return new LoginResponse
        {
            AccessToken = output.AccessToken,
            IdToken = output.IdToken,
            RefreshToken = output.RefreshToken,
            RefreshTokenExpiresAt = output.RefreshTokenExpiresAt
        };
    }
}

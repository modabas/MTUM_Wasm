using MTUM_Wasm.Server.Core.Identity.Dto;
using MTUM_Wasm.Shared.Core.Identity.Dto;

namespace MTUM_Wasm.Server.Core.Identity.Mapping;

internal static class RefreshExtensions
{
    public static RefreshInput ToInput(this RefreshRequest request)
    {
        return new RefreshInput
        {
            RefreshToken = request.RefreshToken,
            RefreshTokenExpiresAt = request.RefreshTokenExpiresAt
        };
    }

    public static RefreshResponse ToResponse(this RefreshOutput output)
    {
        return new RefreshResponse
        {
            AccessToken = output.AccessToken,
            IdToken = output.IdToken,
            RefreshToken = output.RefreshToken,
            RefreshTokenExpiresAt = output.RefreshTokenExpiresAt
        };
    }
}

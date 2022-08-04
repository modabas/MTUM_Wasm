using System;

namespace MTUM_Wasm.Server.Core.Identity.Dto;

internal class RefreshInput
{
    public string Email { get; set; } = string.Empty;
    public DateTime? AuthenticatedAt { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiresAt { get; set; }
}

using System;

namespace MTUM_Wasm.Shared.Core.Identity.Dto;

public class RefreshRequest
{
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiresAt { get; set; }
}

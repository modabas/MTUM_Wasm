using System;

namespace MTUM_Wasm.Server.Core.Identity.Dto;

[Serializable]
internal class RefreshOutput
{
    public Guid Id { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiresAt { get; set; }
    public string IdToken { get; set; } = string.Empty;
}

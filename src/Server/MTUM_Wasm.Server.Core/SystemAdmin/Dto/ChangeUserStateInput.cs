using System;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Dto;

internal class ChangeUserStateInput
{
    public string Email { get; set; } = string.Empty;
    public bool SetEnabled { get; set; }
}

using System;

namespace MTUM_Wasm.Shared.Core.SystemAdmin.Dto;

public class ChangeUserStateRequest
{
    public string Email { get; set; } = string.Empty;
    public bool SetEnabled { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace MTUM_Wasm.Shared.Core.Identity.Dto;

public class LoginRequest
{
    public string? EmailAddress { get; set; }
    public string? Password { get; set; }
}

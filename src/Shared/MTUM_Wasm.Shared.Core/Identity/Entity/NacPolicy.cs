using System;

namespace MTUM_Wasm.Shared.Core.Identity.Entity;

public class NacPolicy
{
    public bool UseSafelist { get; set; } = false;
    public string[] Safelist { get; set; } = Array.Empty<string>();
    public string[] Blacklist { get; set; } = Array.Empty<string>();
}

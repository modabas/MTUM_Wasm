using MTUM_Wasm.Shared.Core.Identity.Entity;
using System;

namespace MTUM_Wasm.Server.Core.Common.Entity;

internal class SystemUserEntity
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string GivenName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
    public Guid? TenantId { get; set; }
    public NacPolicy? NacPolicy { get; set; } = null;
    public string Name { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string UserStatus { get; set; } = string.Empty;
    public bool Enabled { get; set; }
    public bool IsEmailVerified { get; set; } = false;
}

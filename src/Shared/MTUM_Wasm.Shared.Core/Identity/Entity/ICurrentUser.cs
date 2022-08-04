using System;

namespace MTUM_Wasm.Shared.Core.Identity.Entity;

public interface ICurrentUser
{
    public bool IsAuthenticated { get; }
    public Guid Id { get; }
    public string UserName { get; }
    public string EmailAddress { get; } 
    public string GivenName { get; } 
    public string MiddleName { get; } 
    public string FamilyName { get; } 
    public string[] Roles { get; }
    public Guid? TenantId { get; }
    public NacPolicy? NacPolicy { get; } 
    public string Name { get; } 
    public string FullName { get; } 
    public DateTime? AuthenticatedAt { get; }
}

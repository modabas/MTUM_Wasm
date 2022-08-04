using MTUM_Wasm.Shared.Core.Common.Extension;
using MTUM_Wasm.Shared.Core.Identity.Entity;
using System;
using System.Security.Claims;

namespace MTUM_Wasm.Shared.Infrastructure.Identity.AwsCognito;

public class AwsCognitoCurrentUser : ICurrentUser
{
    private readonly ClaimsIdentity? _identity;
    public AwsCognitoCurrentUser(ClaimsIdentity? identity)
    {
        _identity = identity;
    }

    public bool IsAuthenticated
    {
        get
        {
            return _identity is not null && _identity.IsAuthenticated;
        }
    }

    public Guid Id
    {
        get
        {
            return _identity?.GetId() ?? Guid.Empty;
        }
    }

    public string Name
    {
        get
        {
            return _identity?.GetName() ?? string.Empty;
        }
    }

    public string EmailAddress
    {
        get
        {
            return _identity?.GetEmail() ?? string.Empty;
        }
    }

    public string GivenName
    {
        get
        {
            return _identity?.GetGivenName() ?? string.Empty;
        }
    }

    public string MiddleName
    {
        get
        {
            return _identity?.GetMiddleName() ?? string.Empty;
        }
    }

    public string FamilyName
    {
        get
        {
            return _identity?.GetFamilyName() ?? string.Empty;
        }
    }

    public string FullName
    {
        get
        {
            return _identity?.GetFullName() ?? string.Empty;
        }
    }

    public Guid? TenantId
    {
        get
        {
            return _identity?.GetTenantId();
        }
    }

    public NacPolicy? NacPolicy
    {
        get
        {
            return _identity?.GetNacPolicy();
        }
    }

    public string UserName
    {
        get
        {
            return _identity?.GetUserName() ?? string.Empty;
        }
    }

    public string[] Roles
    {
        get
        {
            return _identity?.GetRoles() ?? Array.Empty<string>();
        }
    }

    public DateTime? AuthenticatedAt
    {
        get
        {
            return _identity?.GetAuthenticatedAt();
        }
    }
}

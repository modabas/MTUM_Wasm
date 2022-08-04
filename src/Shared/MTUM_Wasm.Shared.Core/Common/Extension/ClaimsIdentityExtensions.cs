using MTUM_Wasm.Shared.Core.Common.Utility;
using MTUM_Wasm.Shared.Core.Identity.Entity;
using System;
using System.Linq;
using System.Security.Claims;

namespace MTUM_Wasm.Shared.Core.Common.Extension;

public static class ClaimsIdentityExtensions
{
    public static Guid GetId(this ClaimsIdentity identity)
    {
        //Id claim type differs on server side and client side
        var idClaimType = identity.Claims.Any(c => c.Type == ClaimTypes.NameIdentifier) ? ClaimTypes.NameIdentifier : "sub";
        return new Guid(identity.Claims.Single(q => q.Type == idClaimType).Value);
    }

    public static string GetName(this ClaimsIdentity identity)
    {
        return identity.Claims.SingleOrDefault(q => q.Type == ClaimTypes.Email)?.Value ?? string.Empty;
    }

    public static string GetEmail(this ClaimsIdentity identity)
    {
        return identity.Claims.SingleOrDefault(q => q.Type == ClaimTypes.Email)?.Value ?? string.Empty;
    }

    public static string GetGivenName(this ClaimsIdentity identity)
    {
        return identity.Claims.SingleOrDefault(q => q.Type == ClaimTypes.GivenName)?.Value ?? string.Empty;
    }

    public static string GetMiddleName(this ClaimsIdentity identity)
    {
        return identity.Claims.SingleOrDefault(q => q.Type == "middle_name")?.Value ?? string.Empty;
    }

    public static string GetFamilyName(this ClaimsIdentity identity)
    {
        return identity.Claims.SingleOrDefault(q => q.Type == ClaimTypes.Surname)?.Value ?? string.Empty;
    }

    public static Guid? GetTenantId(this ClaimsIdentity identity)
    {
        var tenantIdString = identity.Claims.SingleOrDefault(q => q.Type == "tenant_id")?.Value ?? string.Empty;
        if (Guid.TryParse(tenantIdString, out var tenantId))
            return tenantId;
        return null;
    }

    public static NacPolicy? GetNacPolicy(this ClaimsIdentity identity)
    {
        var nacPolicyResult = JsonHelper.TryDeserializeJson<NacPolicy>(identity.Claims.FirstOrDefault(c => c.Type == "nac")?.Value ?? string.Empty);
        return nacPolicyResult.Succeeded ? nacPolicyResult.Data : null;
    }

    public static string GetUserName(this ClaimsIdentity identity)
    {
        return identity.Claims.SingleOrDefault(q => q.Type == "cognito_user_name")?.Value ?? string.Empty;
    }

    public static string[] GetRoles(this ClaimsIdentity identity)
    {
        return identity.Claims.Where(q => q.Type == ClaimTypes.Role).Select(c => c.Value).ToArray() ?? Array.Empty<string>();
    }

    public static string GetFullName(this ClaimsIdentity identity)
    {
        return $"{identity.GetGivenName()} {identity.GetMiddleName()} {identity.GetFamilyName()}".NormalizeWhitespaces();
    }

    public static DateTime? GetAuthenticatedAt(this ClaimsIdentity identity)
    {
        var authenticatedAtString = identity.Claims.FirstOrDefault(c => c.Type == "authenticated_at")?.Value ?? string.Empty;
        if (TryGetDateTime(authenticatedAtString, out var authenticatedAt))
            return authenticatedAt;
        return null;
    }

    private static bool TryGetDateTime(string timestampString, out DateTime result)
    {
        if (!string.IsNullOrWhiteSpace(timestampString) && long.TryParse(timestampString, out var timestamp))
        {
            result = DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
            return true;
        }
        result = DateTime.MinValue.ToUniversalTime();
        return false;
    }
}

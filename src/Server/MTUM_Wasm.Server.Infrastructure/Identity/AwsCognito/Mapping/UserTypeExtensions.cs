using Amazon.CognitoIdentityProvider.Model;
using MTUM_Wasm.Server.Core.Common.Entity;
using MTUM_Wasm.Shared.Core.Common.Extension;
using MTUM_Wasm.Shared.Core.Common.Utility;
using MTUM_Wasm.Shared.Core.Identity.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTUM_Wasm.Server.Infrastructure.Identity.AwsCognito.Mapping;

internal static class UserTypeExtensions
{
    public static SystemUserEntity ToSystemUserEntity(this UserType userType)
    {
        var attributes = userType.Attributes.ToDictionary(att => att.Name, att => att.Value);
        return new SystemUserEntity
        {
            Id = GetId(attributes),
            UserName = userType.Username,
            EmailAddress = GetEmail(attributes),
            GivenName = GetGivenName(attributes),
            MiddleName = GetMiddleName(attributes),
            FamilyName = GetFamilyName(attributes),
            TenantId = GetTenantId(attributes),
            NacPolicy = GetNacPolicy(attributes),
            Name = GetName(attributes),
            FullName = GetFullName(attributes),
            UserStatus = userType.UserStatus.ToString(),
            Enabled = userType.Enabled,
            IsEmailVerified = GetEmailVerified(attributes)
        };
    }

    private static Guid GetId(Dictionary<string, string> attributes)
    {
        //Id claim type differs on server side and client side
        return new Guid(attributes.Single(q => q.Key == "sub").Value);
    }

    private static string GetName(Dictionary<string, string> attributes)
    {
        return attributes.SingleOrDefault(q => q.Key == "email").Value ?? string.Empty;
    }

    private static string GetEmail(Dictionary<string, string> attributes)
    {
        return attributes.SingleOrDefault(q => q.Key == "email").Value ?? string.Empty;
    }

    private static string GetGivenName(Dictionary<string, string> attributes)
    {
        return attributes.SingleOrDefault(q => q.Key == "given_name").Value ?? string.Empty;
    }

    private static string GetMiddleName(Dictionary<string, string> attributes)
    {
        return attributes.SingleOrDefault(q => q.Key == "middle_name").Value ?? string.Empty;
    }

    private static string GetFamilyName(Dictionary<string, string> attributes)
    {
        return attributes.SingleOrDefault(q => q.Key == "family_name").Value ?? string.Empty;
    }

    private static NacPolicy? GetNacPolicy(Dictionary<string, string> attributes)
    {
        var nacPolicyResult = JsonHelper.TryDeserializeJson<NacPolicy>(attributes.FirstOrDefault(c => c.Key == "custom:nac").Value ?? string.Empty);
        return nacPolicyResult.Succeeded ? nacPolicyResult.Data : null;
    }

    private static string GetFullName(Dictionary<string, string> attributes)
    {
        return $"{GetGivenName(attributes)} {GetMiddleName(attributes)} {GetFamilyName(attributes)}".NormalizeWhitespaces();
    }

    private static Guid? GetTenantId(Dictionary<string, string> attributes)
    {
        var tenantIdString = attributes.SingleOrDefault(q => q.Key == "preferred_username").Value ?? string.Empty;
        if (Guid.TryParse(tenantIdString, out var tenantId))
            return tenantId;
        return null;
    }

    private static bool GetEmailVerified(Dictionary<string, string> attributes)
    {
        var emailVerifiedString = attributes.SingleOrDefault(q => q.Key == "email_verified").Value ?? "false";
        if (bool.TryParse(emailVerifiedString, out var emailVerified))
            return emailVerified;
        return false;
    }
}

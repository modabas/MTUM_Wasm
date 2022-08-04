using Amazon.CognitoIdentityProvider.Model;
using MTUM_Wasm.Server.Core.Identity.Dto;
using MTUM_Wasm.Shared.Core.Common.Utility;
using MTUM_Wasm.Shared.Core.Identity.Entity;
using System;
using System.Collections.Generic;

namespace MTUM_Wasm.Server.Infrastructure.Identity.AwsCognito.Mapping;

internal static class IdentityExtensions
{
    public static List<AttributeType> ToAttributeTypeList(this UpdateUserAttributesInput input)
    {
        var ret = new List<AttributeType>();
        ret.Add(new() { Name = "given_name", Value = input.GivenName });
        ret.Add(new() { Name = "middle_name", Value = input.MiddleName });
        ret.Add(new() { Name = "family_name", Value = input.FamilyName });
        ret.Add(new() { Name = "email_verified", Value = input.IsEmailVerified.ToString() });
        return ret;
    }

    public static List<AttributeType> ToAttributeTypeList(this CreateUserInput input, Guid? tenantId)
    {
        var ret = new List<AttributeType>();
        ret.Add(new() { Name = "email", Value = input.Email });
        ret.Add(new() { Name = "email_verified", Value = "true" });
        ret.Add(new() { Name = "given_name", Value = input.GivenName });
        ret.Add(new() { Name = "middle_name", Value = input.MiddleName });
        ret.Add(new() { Name = "family_name", Value = input.FamilyName });
        if (tenantId is not null)
            ret.Add(new() { Name = "preferred_username", Value = tenantId.Value.ToString() });
        ret.Add(new() { Name = "custom:nac", Value = JsonHelper.SerializeJson(new NacPolicy()) });
        return ret;
    }

    public static List<AttributeType> ToAttributeTypeList(this UpdateUserNacPolicyInput input)
    {
        var ret = new List<AttributeType>();
        ret.Add(new() { Name = "custom:nac", Value = JsonHelper.SerializeJson(input.NacPolicy ?? new NacPolicy()) });
        return ret;
    }
}

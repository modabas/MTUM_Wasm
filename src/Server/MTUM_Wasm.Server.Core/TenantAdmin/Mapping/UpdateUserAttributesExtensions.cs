﻿using MTUM_Wasm.Server.Core.TenantAdmin.Dto;
using MTUM_Wasm.Shared.Core.TenantAdmin.Dto;

namespace MTUM_Wasm.Server.Core.TenantAdmin.Mapping;

internal static class UpdateUserAttributesExtensions
{
    public static UpdateUserAttributesInput ToInput(this UpdateUserAttributesRequest request)
    {
        return new UpdateUserAttributesInput
        {
            Email = request.Email,
            GivenName = request.GivenName.Trim(),
            MiddleName = request.MiddleName.Trim(),
            FamilyName = request.FamilyName.Trim(),
            IsEmailVerified = request.IsEmailVerified
        };
    }

    public static Identity.Dto.UpdateUserAttributesInput ToIdentityInput(this UpdateUserAttributesInput input)
    {
        return new Identity.Dto.UpdateUserAttributesInput
        {
            Email = input.Email,
            GivenName = input.GivenName,
            MiddleName = input.MiddleName,
            FamilyName = input.FamilyName,
            IsEmailVerified = input.IsEmailVerified
        };
    }
}

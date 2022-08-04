using MTUM_Wasm.Server.Core.Common.Entity;
using MTUM_Wasm.Shared.Core.Common.Dto;

namespace MTUM_Wasm.Server.Core.Common.Mapping;

internal static class SystemUserExtensions
{
    public static SystemUserDto ToSharedDto(this SystemUserEntity entity)
    {
        return new SystemUserDto
        {
            Id = entity.Id,
            UserName = entity.UserName,
            EmailAddress = entity.EmailAddress,
            GivenName = entity.GivenName,
            MiddleName = entity.MiddleName,
            FamilyName = entity.FamilyName,
            TenantId = entity.TenantId,
            NacPolicy = entity.NacPolicy,
            Name = entity.Name,
            FullName = entity.FullName,
            UserStatus = entity.UserStatus,
            Enabled = entity.Enabled,
            IsEmailVerified = entity.IsEmailVerified
        };
    }
}

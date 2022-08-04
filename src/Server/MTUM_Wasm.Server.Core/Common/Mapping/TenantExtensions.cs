using MTUM_Wasm.Server.Core.Common.Entity;
using MTUM_Wasm.Shared.Core.Common.Dto;

namespace MTUM_Wasm.Server.Core.Common.Mapping;

internal static class TenantExtensions
{
    public static TenantDto ToSharedDto(this TenantEntity entity)
    {
        return new TenantDto
        {
            Id = entity.Id,
            Name = entity.Name,
            NameLowercase = entity.NameLowercase,
            IsEnabled = entity.IsEnabled,
            CreatedAt = entity.CreatedAt,
            LastModifiedAt = entity.LastModifiedAt,
            NacPolicy = entity.NacPolicy
        };
    }
}

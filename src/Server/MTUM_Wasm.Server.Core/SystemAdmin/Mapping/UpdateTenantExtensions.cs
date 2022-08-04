using MTUM_Wasm.Server.Core.Common.Entity;
using MTUM_Wasm.Server.Core.SystemAdmin.Dto;
using MTUM_Wasm.Shared.Core.SystemAdmin.Dto;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Mapping;

internal static class UpdateTenantExtensions
{
    public static UpdateTenantInput ToInput(this UpdateTenantRequest request)
    {
        return new UpdateTenantInput
        {
            Id = request.Id,
            Name = request.Name,
            IsEnabled = request.IsEnabled
        };
    }

    public static TenantEntity ToTenantEntity(this UpdateTenantInput input)
    {
        return new TenantEntity
        {
            Id = input.Id,
            Name = input.Name,
            IsEnabled = input.IsEnabled
        };
    }
}

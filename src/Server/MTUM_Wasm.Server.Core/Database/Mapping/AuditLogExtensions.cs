using MTUM_Wasm.Server.Core.Database.Entity;
using MTUM_Wasm.Shared.Core.Common.Dto;
using MTUM_Wasm.Shared.Core.Common.Utility;

namespace MTUM_Wasm.Server.Core.Database.Mapping;

internal static class AuditLogExtensions
{
    public static AuditLogDto ToSharedDto(this AuditLogEntity entity)
    {
        return new AuditLogDto
        {
            Id = entity.Id,
            CommandName = entity.CommandName,
            Email = entity.Email,
            TenantId = entity.TenantId,
            RemoteIp = entity.RemoteIp,
            CreatedAt = entity.CreatedAt,
            CreatedDate = entity.CreatedDate,
            User = entity.User,
            Entry = entity.Entry
        };
    }

    public static AuditLogDto ToPrettifiedSharedDto(this AuditLogEntity entity)
    {
        return new AuditLogDto
        {
            Id = entity.Id,
            CommandName = entity.CommandName,
            Email = entity.Email,
            TenantId = entity.TenantId,
            RemoteIp = entity.RemoteIp,
            CreatedAt = entity.CreatedAt,
            CreatedDate = entity.CreatedDate,
            User = JsonHelper.PrettifyJsonString(entity.User),
            Entry = JsonHelper.PrettifyJsonString(entity.Entry)
        };
    }
}

using MTUM_Wasm.Server.Core.Database.Entity;
using Dapper.FluentMap.Mapping;

namespace MTUM_Wasm.Server.Infrastructure.Database.Postgres.Dapper.Mapping;

internal class AuditLogMap : EntityMap<AuditLogEntity>
{
    public AuditLogMap()
    {
        Map(p => p.Id).ToColumn("id");
        Map(p => p.CommandName).ToColumn("command_name");
        Map(p => p.Email).ToColumn("email");
        Map(p => p.TenantId).ToColumn("tenant_id");
        Map(p => p.RemoteIp).ToColumn("remote_ip");
        Map(p => p.CreatedAt).ToColumn("created_at");
        Map(p => p.CreatedDate).ToColumn("created_date");
        Map(p => p.User).ToColumn("logged_in_user");
        Map(p => p.Entry).ToColumn("entry");
    }
}

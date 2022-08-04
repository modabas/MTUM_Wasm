using MTUM_Wasm.Server.Core.Common.Entity;
using Dapper.FluentMap.Mapping;

namespace MTUM_Wasm.Server.Infrastructure.Database.Postgres.Dapper.Mapping;

internal class TenantMap : EntityMap<TenantEntity>
{
    public TenantMap()
    {
        Map(p => p.Id).ToColumn("id");
        Map(p => p.Name).ToColumn("name");
        Map(p => p.NameLowercase).ToColumn("name_lower");
        Map(p => p.CreatedAt).ToColumn("created_at");
        Map(p => p.IsEnabled).ToColumn("is_enabled");
        Map(p => p.LastModifiedAt).ToColumn("last_modified_at");
        Map(p => p.NacPolicy).ToColumn("nac_policy");
    }
}

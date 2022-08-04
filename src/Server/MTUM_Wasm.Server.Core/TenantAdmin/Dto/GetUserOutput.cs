using MTUM_Wasm.Server.Core.Common.Entity;

namespace MTUM_Wasm.Server.Core.TenantAdmin.Dto;

internal class GetUserOutput
{
    public SystemUserEntity? User { get; set; }
}

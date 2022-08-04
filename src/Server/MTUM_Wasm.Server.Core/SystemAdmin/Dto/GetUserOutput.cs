using MTUM_Wasm.Server.Core.Common.Entity;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Dto;

internal class GetUserOutput
{
    public SystemUserEntity? User { get; set; }
}

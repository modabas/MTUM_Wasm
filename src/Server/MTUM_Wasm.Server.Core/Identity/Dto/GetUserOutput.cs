using MTUM_Wasm.Server.Core.Common.Entity;

namespace MTUM_Wasm.Server.Core.Identity.Dto;

internal class GetUserOutput
{
    public SystemUserEntity? User { get; set; }
}

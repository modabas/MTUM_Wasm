using System;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Dto;

internal class GetTenantUsersInput
{
    public Guid TenantId { get; set; }
}

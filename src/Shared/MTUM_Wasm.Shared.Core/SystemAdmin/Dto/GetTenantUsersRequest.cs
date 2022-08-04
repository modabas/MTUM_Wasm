using System;

namespace MTUM_Wasm.Shared.Core.SystemAdmin.Dto;

public class GetTenantUsersRequest
{
    public Guid TenantId { get; set; }
}

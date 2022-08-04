using MTUM_Wasm.Shared.Core.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTUM_Wasm.Shared.Core.TenantAdmin.Dto
{
    public class GetUsersResponse
    {
        public IEnumerable<SystemUserDto> Users { get; set; } = Enumerable.Empty<SystemUserDto>();
    }
}

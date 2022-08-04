using MTUM_Wasm.Server.Core.Identity.Service;
using MTUM_Wasm.Server.Core.TenantAdmin.Dto;
using MTUM_Wasm.Server.Core.TenantAdmin.Service;
using MTUM_Wasm.Shared.Core.Common.Extension;
using MTUM_Wasm.Shared.Core.Common.Result;
using MediatR;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.TenantAdmin.Mediatr
{
    internal class GetUsersQuery : IRequest<IServiceResult<GetUsersOutput>>
    {
    }

    internal class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IServiceResult<GetUsersOutput>>
    {
        private readonly IUserAccessor _userAccessor;
        private readonly ITenantAdminService _tenantAdminService;

        public GetUsersQueryHandler(IUserAccessor userAccessor, ITenantAdminService tenantAdminService)
        {
            _userAccessor = userAccessor;
            _tenantAdminService = tenantAdminService;
        }

        public async Task<IServiceResult<GetUsersOutput>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
        {
            var claimsIdentity = (ClaimsIdentity?)_userAccessor.User?.Identity;
            var tenantId = claimsIdentity?.GetTenantId();

            if (tenantId is null)
            {
                return Result<GetUsersOutput>.Fail("Cannot determine logged in user's tenant.");
            }
            return await _tenantAdminService.GetUsersByTenant(tenantId.Value, cancellationToken);
        }
    }
}

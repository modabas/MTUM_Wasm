using MTUM_Wasm.Server.Core.Identity.Service;
using MTUM_Wasm.Server.Core.TenantAdmin.Dto;
using MTUM_Wasm.Server.Core.TenantAdmin.Service;
using MTUM_Wasm.Shared.Core.Common.Extension;
using MTUM_Wasm.Shared.Core.Common.Result;
using MediatR;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.TenantAdmin.Mediatr;

internal class GetUserQuery : IRequest<IServiceResult<GetUserOutput>>
{
    public GetUserInput? Request { get; set; }
}

internal class GetUserQueryHandler : IRequestHandler<GetUserQuery, IServiceResult<GetUserOutput>>
{
    private readonly IUserAccessor _userAccessor;
    private readonly ITenantAdminService _tenantAdminService;

    public GetUserQueryHandler(IUserAccessor userAccessor, ITenantAdminService tenantAdminService)
    {
        _userAccessor = userAccessor;
        _tenantAdminService = tenantAdminService;
    }

    public async Task<IServiceResult<GetUserOutput>> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        if (query.Request is null)
            throw new ArgumentNullException(nameof(query), "The value of 'query.Request' should not be null");

        var claimsIdentity = (ClaimsIdentity?)_userAccessor.User?.Identity;
        var loggedInUserTenantId = claimsIdentity?.GetTenantId();

        if (loggedInUserTenantId is null)
        {
            return Result<GetUserOutput>.Fail("Cannot determine logged in user's tenant.");
        }

        return await _tenantAdminService.GetUser(query.Request, loggedInUserTenantId.Value, cancellationToken);
    }
}

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

internal class ChangeUserStateCommand : IRequest<IServiceResult>
{
    public ChangeUserStateInput? Request { get; set; }
}

internal class ChangeUserStateCommandHandler : IRequestHandler<ChangeUserStateCommand, IServiceResult>
{
    private readonly IUserAccessor _userAccessor;
    private readonly ITenantAdminService _tenantAdminService;

    public ChangeUserStateCommandHandler(IUserAccessor userAccessor, ITenantAdminService tenantAdminService)
    {
        _userAccessor = userAccessor;
        _tenantAdminService = tenantAdminService;
    }

    public async Task<IServiceResult> Handle(ChangeUserStateCommand query, CancellationToken cancellationToken)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        if (query.Request is null)
            throw new ArgumentNullException(nameof(query), "The value of 'query.Request' should not be null");

        var claimsIdentity = (ClaimsIdentity?)_userAccessor.User?.Identity;

        var loggedInUserEmail = claimsIdentity?.GetEmail();
        if (string.IsNullOrWhiteSpace(loggedInUserEmail))
        {
            return Result.Fail("Cannot determine logged in user's email address.");
        }
        if (loggedInUserEmail.Equals(query.Request.Email, StringComparison.OrdinalIgnoreCase))
        {
            return Result.Fail("Logged in users cannot change their own state.");
        }

        var loggedInUserTenantId = claimsIdentity?.GetTenantId();
        if (loggedInUserTenantId is null)
        {
            return Result.Fail("Cannot determine logged in user's tenant.");
        }
        return await _tenantAdminService.ChangeUserState(query.Request, loggedInUserTenantId.Value, cancellationToken);
    }
}


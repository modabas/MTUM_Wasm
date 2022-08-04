using MTUM_Wasm.Server.Core.Identity.Service;
using MTUM_Wasm.Server.Core.SystemAdmin.Dto;
using MTUM_Wasm.Server.Core.SystemAdmin.Service;
using MTUM_Wasm.Shared.Core.Common.Extension;
using MTUM_Wasm.Shared.Core.Common.Result;
using MediatR;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Mediatr;

internal class ChangeUserStateCommand : IRequest<IServiceResult>
{
    public ChangeUserStateInput? Request { get; set; }
}

internal class ChangeUserStateCommandHandler : IRequestHandler<ChangeUserStateCommand, IServiceResult>
{
    private readonly ISystemAdminService _systemAdminService;
    private readonly IUserAccessor _userAccessor;

    public ChangeUserStateCommandHandler(ISystemAdminService systemAdminService, IUserAccessor userAccessor)
    {
        _systemAdminService = systemAdminService;
        _userAccessor = userAccessor;
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

        return await _systemAdminService.ChangeUserState(query.Request, cancellationToken);
    }
}

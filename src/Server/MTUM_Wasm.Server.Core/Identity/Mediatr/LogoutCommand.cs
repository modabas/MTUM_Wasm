using MTUM_Wasm.Server.Core.Identity.Dto;
using MTUM_Wasm.Server.Core.Identity.Service;
using MTUM_Wasm.Shared.Core.Common.Result;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.Identity.Mediatr;

internal class LogoutCommand : IRequest<IServiceResult>
{
}

internal class LogoutCommandHandler : IRequestHandler<LogoutCommand, IServiceResult>
{
    
    private readonly IIdentityService _identityService;
    private readonly IUserAccessor _userAccessor;

    public LogoutCommandHandler(IIdentityService identityService, IUserAccessor userAccessor)
    {
        _identityService = identityService;
        _userAccessor = userAccessor;
    }

    public async Task<IServiceResult> Handle(LogoutCommand query, CancellationToken cancellationToken)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        var user = await _userAccessor.GetCurrentUser(cancellationToken);
        return await _identityService.Logout(new LogoutInput() { EmailAddress = user.EmailAddress }, cancellationToken);
    }
}

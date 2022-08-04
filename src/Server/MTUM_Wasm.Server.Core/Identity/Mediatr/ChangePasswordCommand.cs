using MTUM_Wasm.Server.Core.Identity.Dto;
using MTUM_Wasm.Server.Core.Identity.Service;
using MTUM_Wasm.Shared.Core.Common.Result;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.Identity.Mediatr;

internal class ChangePasswordCommand : IRequest<IServiceResult>
{
    public ChangePasswordInput? Request { get; set; }
}

internal class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, IServiceResult>
{
    private readonly IIdentityService _identityService;

    public ChangePasswordCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<IServiceResult> Handle(ChangePasswordCommand query, CancellationToken cancellationToken)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        if (query.Request is null)
            throw new ArgumentNullException(nameof(query), "The value of 'query.ChangePasswordRequest' should not be null");

        return await _identityService.ChangePassword(query.Request, cancellationToken);
    }
}

using MTUM_Wasm.Server.Core.Identity.Dto;
using MTUM_Wasm.Server.Core.Identity.Service;
using MTUM_Wasm.Shared.Core.Common.Result;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.Identity.Mediatr;

internal class ResetPasswordCommand : IRequest<IServiceResult>
{
    public ResetPasswordInput? Request { get; set; }
}

internal class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, IServiceResult>
{
    private readonly IIdentityService _identityService;

    public ResetPasswordCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<IServiceResult> Handle(ResetPasswordCommand query, CancellationToken cancellationToken)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        if (query.Request is null)
            throw new ArgumentNullException(nameof(query), "The value of 'query.ResetPasswordRequest' should not be null");

        return await _identityService.ResetPassword(query.Request, cancellationToken);
    }
}
using MTUM_Wasm.Server.Core.Identity.Dto;
using MTUM_Wasm.Server.Core.Identity.Service;
using MTUM_Wasm.Shared.Core.Common.Result;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.Identity.Mediatr;

internal class ForgotPasswordCommand : IRequest<IServiceResult>
{
    public ForgotPasswordInput? Request { get; set; }
}

internal class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, IServiceResult>
{
    private readonly IIdentityService _identityService;

    public ForgotPasswordCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<IServiceResult> Handle(ForgotPasswordCommand query, CancellationToken cancellationToken)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        if (query.Request is null)
            throw new ArgumentNullException(nameof(query), "The value of 'query.ForgotPasswordRequest' should not be null");

        return await _identityService.ForgotPassword(query.Request, cancellationToken);
    }
}

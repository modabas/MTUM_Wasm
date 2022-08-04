using MTUM_Wasm.Server.Core.Identity.Dto;
using MTUM_Wasm.Server.Core.Identity.Service;
using MTUM_Wasm.Shared.Core.Common.Result;
using MediatR;
using Orleans;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.Identity.Mediatr;

internal class LoginCommand : IRequest<IServiceResult<LoginOutput>>
{
    public LoginInput? Request { get; set; }
}

internal class LoginCommandHandler : IRequestHandler<LoginCommand, IServiceResult<LoginOutput>>
{
    private readonly IGrainFactory _grainFactory;
    private readonly IIdentityService _identityService;

    public LoginCommandHandler(IGrainFactory grainFactory, IIdentityService identityService)
    {
        _grainFactory = grainFactory;
        _identityService = identityService;
    }

    public async Task<IServiceResult<LoginOutput>> Handle(LoginCommand query, CancellationToken cancellationToken)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        if (query.Request is null)
            throw new ArgumentNullException(nameof(query), "The value of 'query.Request' should not be null");

        return await _identityService.Login(query.Request, cancellationToken);
    }
}

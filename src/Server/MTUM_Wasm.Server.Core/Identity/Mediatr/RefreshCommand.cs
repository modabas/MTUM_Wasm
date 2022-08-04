using MTUM_Wasm.Server.Core.Identity.Dto;
using MTUM_Wasm.Server.Core.Identity.Service;
using MTUM_Wasm.Shared.Core.Common.Result;
using MediatR;
using Orleans;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.Identity.Mediatr;

internal class RefreshCommand : IRequest<IServiceResult<RefreshOutput>>
{
    public RefreshInput? Request { get; set; }
}

internal class RefreshCommandHandler : IRequestHandler<RefreshCommand, IServiceResult<RefreshOutput>>
{
    private readonly IGrainFactory _grainFactory;
    private readonly IUserAccessor _userAccessor;
    private readonly IIdentityService _identityService;

    public RefreshCommandHandler(IGrainFactory grainFactory, IUserAccessor userAccessor, IIdentityService identityService)
    {
        _grainFactory = grainFactory;
        _userAccessor = userAccessor;
        _identityService = identityService;
    }

    public async Task<IServiceResult<RefreshOutput>> Handle(RefreshCommand query, CancellationToken cancellationToken)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        if (query.Request is null)
            throw new ArgumentNullException(nameof(query), "The value of 'query.Request' should not be null");

        var user = await _userAccessor.GetCurrentUser(cancellationToken);
        var inputParams = query.Request;
        inputParams.AuthenticatedAt = user.AuthenticatedAt;
        inputParams.Email = user.EmailAddress;
        return await _identityService.Refresh(inputParams, cancellationToken);

    }
}


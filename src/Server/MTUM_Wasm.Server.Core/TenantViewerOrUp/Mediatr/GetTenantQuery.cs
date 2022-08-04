using MTUM_Wasm.Server.Core.Common.Orleans;
using MTUM_Wasm.Server.Core.Identity.Service;
using MTUM_Wasm.Server.Core.TenantViewerOrUp.Dto;
using MTUM_Wasm.Shared.Core.Common.Extension;
using MTUM_Wasm.Shared.Core.Common.Result;
using MediatR;
using Orleans;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.TenantViewerOrUp.Mediatr;

internal class GetTenantQuery : IRequest<IServiceResult<GetTenantOutput>>
{
}

internal class GetTenantQueryHandler : IRequestHandler<GetTenantQuery, IServiceResult<GetTenantOutput>>
{
    private readonly IGrainFactory _grainFactory;
    private readonly IUserAccessor _userAccessor;

    public GetTenantQueryHandler(IGrainFactory grainFactory, IUserAccessor userAccessor)
    {
        _grainFactory = grainFactory;
        _userAccessor = userAccessor;
    }

    public async Task<IServiceResult<GetTenantOutput>> Handle(GetTenantQuery query, CancellationToken cancellationToken)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        var claimsIdentity = (ClaimsIdentity?)_userAccessor.User?.Identity;
        var loggedInUserTenantId = claimsIdentity?.GetTenantId();

        if (loggedInUserTenantId is null)
        {
            return Result<GetTenantOutput>.Fail("Cannot determine logged in user's tenant.");
        }


        using (var grainCancellationTokenSource = new GrainCancellationTokenSource())
        {
            //create a grain cancellation token
            var grainCancellationToken = grainCancellationTokenSource.Token;
            //link cancellation token to grain cancellation token source for this scope, so grain token will be cancelled if token is cancelled
            using (cancellationToken.Register(async () => await grainCancellationTokenSource.Cancel()))
            {
                var tenantResult = await _grainFactory.GetGrain<ITenantGrain>(loggedInUserTenantId.Value).GetTenant(grainCancellationToken);
                if (tenantResult.Succeeded)
                {
                    return Result<GetTenantOutput>.Success(new GetTenantOutput() { Tenant = tenantResult.Data });
                }
                return Result<GetTenantOutput>.Fail(tenantResult.Messages);
            }
        }
    }
}
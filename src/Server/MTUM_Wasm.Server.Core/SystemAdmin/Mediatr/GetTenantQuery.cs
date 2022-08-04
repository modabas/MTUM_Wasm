using MTUM_Wasm.Server.Core.Common.Orleans;
using MTUM_Wasm.Server.Core.SystemAdmin.Dto;
using MTUM_Wasm.Shared.Core.Common.Result;
using MediatR;
using Orleans;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Mediatr;

internal class GetTenantQuery : IRequest<IServiceResult<GetTenantOutput>>
{
    public GetTenantInput? Request { get; set; }
}

internal class GetTenantQueryHandler : IRequestHandler<GetTenantQuery, IServiceResult<GetTenantOutput>>
{
    private readonly IGrainFactory _grainFactory;

    public GetTenantQueryHandler(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory;
    }

    public async Task<IServiceResult<GetTenantOutput>> Handle(GetTenantQuery query, CancellationToken cancellationToken)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        if (query.Request is null)
            throw new ArgumentNullException(nameof(query), "The value of 'query.Request' should not be null");

        using (var grainCancellationTokenSource = new GrainCancellationTokenSource())
        {
            //create a grain cancellation token
            var grainCancellationToken = grainCancellationTokenSource.Token;
            //link cancellation token to grain cancellation token source for this scope, so grain token will be cancelled if token is cancelled
            using (cancellationToken.Register(async () => await grainCancellationTokenSource.Cancel()))
            {
                var tenantResult = await _grainFactory.GetGrain<ITenantGrain>(query.Request.Id).GetTenant(grainCancellationToken);
                if (tenantResult.Succeeded)
                {
                    return Result<GetTenantOutput>.Success(new GetTenantOutput() { Tenant = tenantResult.Data });
                }
                return Result<GetTenantOutput>.Fail(tenantResult.Messages);
            }
        }
    }
}

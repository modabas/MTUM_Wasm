using MTUM_Wasm.Server.Core.Common.Orleans;
using MTUM_Wasm.Shared.Core.Common.Result;
using MediatR;
using Orleans;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Mediatr;

internal class TenantExistsQuery : IRequest<IServiceResult>
{
    public Guid Id { get; set; }
}

internal class TenantExistsQueryHandler : IRequestHandler<TenantExistsQuery, IServiceResult>
{
    private readonly IGrainFactory _grainFactory;

    public TenantExistsQueryHandler(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory;
    }

    public async Task<IServiceResult> Handle(TenantExistsQuery query, CancellationToken cancellationToken)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        using (var grainCancellationTokenSource = new GrainCancellationTokenSource())
        {
            //create a grain cancellation token
            var grainCancellationToken = grainCancellationTokenSource.Token;
            //link cancellation token to grain cancellation token source for this scope, so grain token will be cancelled if token is cancelled
            using (cancellationToken.Register(async () => await grainCancellationTokenSource.Cancel()))
            {
                return await _grainFactory.GetGrain<ITenantGrain>(query.Id).TenantExists(grainCancellationToken);
            }
        }
    }
}

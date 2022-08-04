using MTUM_Wasm.Server.Core.Common.Orleans;
using MTUM_Wasm.Server.Core.Identity.Service;
using MTUM_Wasm.Server.Core.TenantAdmin.Dto;
using MTUM_Wasm.Shared.Core.Common.Extension;
using MTUM_Wasm.Shared.Core.Common.Result;
using MediatR;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.TenantAdmin.Mediatr;

internal class UpdateTenantNacPolicyCommand : IRequest<IServiceResult>
{
    public UpdateTenantNacPolicyInput? Request { get; set; }
}

internal class UpdateTenantNacPolicyCommandHandler : IRequestHandler<UpdateTenantNacPolicyCommand, IServiceResult>
{
    private readonly IGrainFactory _grainFactory;
    private readonly IUserAccessor _userAccessor;

    public UpdateTenantNacPolicyCommandHandler(IGrainFactory grainFactory, IUserAccessor userAccessor)
    {
        _grainFactory = grainFactory;
        _userAccessor = userAccessor;
    }

    public async Task<IServiceResult> Handle(UpdateTenantNacPolicyCommand query, CancellationToken cancellationToken)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        if (query.Request is null)
            throw new ArgumentNullException(nameof(query), "The value of 'query.Request' should not be null");

        var claimsIdentity = (ClaimsIdentity?)_userAccessor.User?.Identity;
        var loggedInUserTenantId = claimsIdentity?.GetTenantId();

        if (loggedInUserTenantId is null)
        {
            return Result.Fail("Cannot determine logged in user's tenant.");
        }

        using (var grainCancellationTokenSource = new GrainCancellationTokenSource())
        {
            //create a grain cancellation token
            var grainCancellationToken = grainCancellationTokenSource.Token;
            //link cancellation token to grain cancellation token source for this scope, so grain token will be cancelled if token is cancelled
            using (cancellationToken.Register(async () => await grainCancellationTokenSource.Cancel()))
            {
                return await _grainFactory.GetGrain<ITenantGrain>(loggedInUserTenantId.Value).UpdateTenantNacPolicy(query.Request.NacPolicy, grainCancellationToken);
            }
        }
    }
}

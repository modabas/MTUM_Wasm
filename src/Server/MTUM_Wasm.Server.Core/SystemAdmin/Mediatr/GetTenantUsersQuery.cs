using MTUM_Wasm.Server.Core.SystemAdmin.Dto;
using MTUM_Wasm.Server.Core.SystemAdmin.Service;
using MTUM_Wasm.Shared.Core.Common.Result;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Mediatr;

internal class GetTenantUsersQuery : IRequest<IServiceResult<GetTenantUsersOutput>>
{
    public GetTenantUsersInput? Request { get; set; }
}

internal class GetTenantUsersQueryHandler : IRequestHandler<GetTenantUsersQuery, IServiceResult<GetTenantUsersOutput>>
{
    private readonly ISystemAdminService _systemAdminService;
    private readonly IMediator _mediator;

    public GetTenantUsersQueryHandler(ISystemAdminService systemAdminService, IMediator mediator)
    {
        _systemAdminService = systemAdminService;
        _mediator = mediator;
    }

    public async Task<IServiceResult<GetTenantUsersOutput>> Handle(GetTenantUsersQuery query, CancellationToken cancellationToken)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        if (query.Request is null)
            throw new ArgumentNullException(nameof(query), "The value of 'query.Request' should not be null");

        //check if tenant exists
        var checkResult = await _mediator.Send(new TenantExistsQuery() { Id = query.Request.TenantId });
        if (!checkResult.Succeeded)
            return Result<GetTenantUsersOutput>.Fail(checkResult.Messages);

        return await _systemAdminService.GetTenantUsers(query.Request, cancellationToken);
    }
}

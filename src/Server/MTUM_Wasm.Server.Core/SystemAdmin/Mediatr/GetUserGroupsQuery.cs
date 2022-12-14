using MTUM_Wasm.Server.Core.SystemAdmin.Dto;
using MTUM_Wasm.Server.Core.SystemAdmin.Service;
using MTUM_Wasm.Shared.Core.Common.Result;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Mediatr;

internal class GetUserGroupsQuery : IRequest<IServiceResult<GetUserGroupsOutput>>
{
    public GetUserGroupsInput? Request { get; set; }
}

internal class GetUserGroupsQueryHandler : IRequestHandler<GetUserGroupsQuery, IServiceResult<GetUserGroupsOutput>>
{
    private readonly ISystemAdminService _systemAdminService;

    public GetUserGroupsQueryHandler(ISystemAdminService systemAdminService)
    {
        _systemAdminService = systemAdminService;
    }

    public async Task<IServiceResult<GetUserGroupsOutput>> Handle(GetUserGroupsQuery query, CancellationToken cancellationToken)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        if (query.Request is null)
            throw new ArgumentNullException(nameof(query), "The value of 'query.Request' should not be null");

        return await _systemAdminService.GetUserGroups(query.Request, cancellationToken);
    }
}

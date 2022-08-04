using MTUM_Wasm.Server.Core.SystemAdmin.Dto;
using MTUM_Wasm.Server.Core.SystemAdmin.Service;
using MTUM_Wasm.Shared.Core.Common.Result;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Mediatr;

internal class GetUsersInGroupQuery : IRequest<IServiceResult<GetUsersInGroupOutput>>
{
    public GetUsersInGroupInput? Request { get; set; }
}

internal class GetUsersInGroupQueryHandler : IRequestHandler<GetUsersInGroupQuery, IServiceResult<GetUsersInGroupOutput>>
{
    private readonly ISystemAdminService _systemAdminService;

    public GetUsersInGroupQueryHandler(ISystemAdminService systemAdminService)
    {
        _systemAdminService = systemAdminService;
    }

    public async Task<IServiceResult<GetUsersInGroupOutput>> Handle(GetUsersInGroupQuery query, CancellationToken cancellationToken)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        if (query.Request is null)
            throw new ArgumentNullException(nameof(query), "The value of 'query.Request' should not be null");

        return await _systemAdminService.GetUsersInGroup(query.Request, cancellationToken);
    }
}

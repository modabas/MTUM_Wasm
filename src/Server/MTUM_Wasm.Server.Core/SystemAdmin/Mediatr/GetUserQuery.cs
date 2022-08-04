using MTUM_Wasm.Server.Core.SystemAdmin.Dto;
using MTUM_Wasm.Server.Core.SystemAdmin.Service;
using MTUM_Wasm.Shared.Core.Common.Result;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Mediatr;

internal class GetUserQuery : IRequest<IServiceResult<GetUserOutput>>
{
    public GetUserInput? Request { get; set; }
}

internal class GetUserQueryHandler : IRequestHandler<GetUserQuery, IServiceResult<GetUserOutput>>
{
    private readonly ISystemAdminService _systemAdminService;

    public GetUserQueryHandler(ISystemAdminService systemAdminService)
    {
        _systemAdminService = systemAdminService;
    }

    public async Task<IServiceResult<GetUserOutput>> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        if (query.Request is null)
            throw new ArgumentNullException(nameof(query), "The value of 'query.Request' should not be null");

        return await _systemAdminService.GetUser(query.Request, cancellationToken);
    }
}

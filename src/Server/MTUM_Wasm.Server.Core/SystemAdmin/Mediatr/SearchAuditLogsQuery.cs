using MTUM_Wasm.Server.Core.Identity.Service;
using MTUM_Wasm.Server.Core.SystemAdmin.Dto;
using MTUM_Wasm.Server.Core.SystemAdmin.Service;
using MTUM_Wasm.Shared.Core.Common.Extension;
using MTUM_Wasm.Shared.Core.Common.Result;
using MediatR;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Mediatr;

internal class SearchAuditLogsQuery : IRequest<IServiceResult<SearchAuditLogsOutput>>
{
    public SearchAuditLogsInput? Request { get; set; }
}

internal class SearchAuditLogsQueryHandler : IRequestHandler<SearchAuditLogsQuery, IServiceResult<SearchAuditLogsOutput>>
{
    private readonly ISystemAdminService _SystemAdminService;

    public SearchAuditLogsQueryHandler(ISystemAdminService SystemAdminService)
    {
        _SystemAdminService = SystemAdminService;
    }

    public async Task<IServiceResult<SearchAuditLogsOutput>> Handle(SearchAuditLogsQuery query, CancellationToken cancellationToken)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        if (query.Request is null)
            throw new ArgumentNullException(nameof(query), "The value of 'query.Request' should not be null");

        return await _SystemAdminService.SearchAuditLogs(query.Request, cancellationToken);
    }
}
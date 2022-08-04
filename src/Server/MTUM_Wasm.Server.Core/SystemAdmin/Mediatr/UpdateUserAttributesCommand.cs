using MTUM_Wasm.Server.Core.SystemAdmin.Dto;
using MTUM_Wasm.Server.Core.SystemAdmin.Service;
using MTUM_Wasm.Shared.Core.Common.Result;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Mediatr;

internal class UpdateUserAttributesCommand : IRequest<IServiceResult>
{
    public UpdateUserAttributesInput? Request { get; set; }
}

internal class UpdateUserAttributesCommandHandler : IRequestHandler<UpdateUserAttributesCommand, IServiceResult>
{
    private readonly ISystemAdminService _systemAdminService;

    public UpdateUserAttributesCommandHandler(ISystemAdminService systemAdminService)
    {
        _systemAdminService = systemAdminService;
    }

    public async Task<IServiceResult> Handle(UpdateUserAttributesCommand query, CancellationToken cancellationToken)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        if (query.Request is null)
            throw new ArgumentNullException(nameof(query), "The value of 'query.Request' should not be null");

        return await _systemAdminService.UpdateUserAttributes(query.Request, cancellationToken);
    }
}

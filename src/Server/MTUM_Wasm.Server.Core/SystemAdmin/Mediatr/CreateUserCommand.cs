using MTUM_Wasm.Server.Core.SystemAdmin.Dto;
using MTUM_Wasm.Server.Core.SystemAdmin.Service;
using MTUM_Wasm.Shared.Core.Common.Result;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Mediatr;

internal class CreateUserCommand : IRequest<IServiceResult>
{
    public CreateUserInput? Request { get; set; }
}

internal class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, IServiceResult>
{
    private readonly ISystemAdminService _systemAdminService;
    private readonly IMediator _mediator;

    public CreateUserCommandHandler(ISystemAdminService systemAdminService, IMediator mediator)
    {
        _systemAdminService = systemAdminService;
        _mediator = mediator;
    }

    public async Task<IServiceResult> Handle(CreateUserCommand query, CancellationToken cancellationToken)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        if (query.Request is null)
            throw new ArgumentNullException(nameof(query), "The value of 'query.Request' should not be null");

        //check if tenant exists
        if (query.Request.TenantId != Guid.Empty)
        {
            var checkResult = await _mediator.Send(new TenantExistsQuery() { Id = query.Request.TenantId });
            if (!checkResult.Succeeded)
                return Result.Fail(checkResult.Messages);
        }

        return await _systemAdminService.CreateUser(query.Request, cancellationToken);
    }
}
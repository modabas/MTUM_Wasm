﻿using MTUM_Wasm.Server.Core.Identity.Service;
using MTUM_Wasm.Server.Core.TenantAdmin.Dto;
using MTUM_Wasm.Server.Core.TenantAdmin.Service;
using MTUM_Wasm.Shared.Core.Common.Extension;
using MTUM_Wasm.Shared.Core.Common.Result;
using MediatR;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.TenantAdmin.Mediatr;

internal class UpdateUserAttributesCommand : IRequest<IServiceResult>
{
    public UpdateUserAttributesInput? Request { get; set; }
}

internal class UpdateUserAttributesCommandHandler : IRequestHandler<UpdateUserAttributesCommand, IServiceResult>
{
    private readonly IUserAccessor _userAccessor;
    private readonly ITenantAdminService _tenantAdminService;

    public UpdateUserAttributesCommandHandler(IUserAccessor userAccessor, ITenantAdminService tenantAdminService)
    {
        _userAccessor = userAccessor;
        _tenantAdminService = tenantAdminService;
    }

    public async Task<IServiceResult> Handle(UpdateUserAttributesCommand query, CancellationToken cancellationToken)
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

        return await _tenantAdminService.UpdateUserAttributes(query.Request, loggedInUserTenantId.Value, cancellationToken);
    }
}

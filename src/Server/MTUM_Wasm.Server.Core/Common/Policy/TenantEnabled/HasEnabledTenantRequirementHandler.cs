using MTUM_Wasm.Server.Core.Identity.Service;
using MTUM_Wasm.Server.Core.SystemAdmin.Mediatr;
using MTUM_Wasm.Shared.Core.Common.Extension;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.Common.Policy.TenantEnabled;

internal class HasEnabledTenantRequirementHandler : AuthorizationHandler<HasEnabledTenantRequirement>
{
    private readonly IUserAccessor _userAccessor;
    private readonly IMediator _mediator;

    public HasEnabledTenantRequirementHandler(IUserAccessor userAccessor, IMediator mediator)
    {
        _userAccessor = userAccessor;
        _mediator = mediator;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, HasEnabledTenantRequirement requirement)
    {
        var tenantId = ((ClaimsIdentity?)_userAccessor.User?.Identity)?.GetTenantId();
        //user doesn't have tenant claim
        if (tenantId is null)
        {
            context.Fail(new(this, "Cannot determine logged in user's tenant."));
            return;
        }

        //get tenant info
        var tenantResult = await _mediator.Send(new GetTenantQuery() { Request = new SystemAdmin.Dto.GetTenantInput() { Id = tenantId.Value } });
        if (!tenantResult.Succeeded)
        {
            context.Fail(new(this, string.Join(" ", tenantResult.Messages)));
            return;
        }

        var tenant = tenantResult.Data?.Tenant;
        if (tenant is null)
        {
            context.Fail(new(this, "Tenant is null."));
            return;
        }

        if (tenant.IsEnabled)
        {
            context.Succeed(requirement);
            return;
        }
        else
        {
            context.Fail(new(this, "Tenant is disabled."));
            return;
        }
    }
}


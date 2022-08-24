using MTUM_Wasm.Server.Core.Common.Extension;
using MTUM_Wasm.Server.Core.Identity.Service;
using MTUM_Wasm.Server.Core.SystemAdmin.Mediatr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.Common.Policy.Nac;

internal class TenantNacRequirementHandler : AuthorizationHandler<TenantNacRequirement>
{
    private readonly IUserAccessor _userAccessor;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMediator _mediator;

    public TenantNacRequirementHandler(IUserAccessor userAccessor, IHttpContextAccessor httpContextAccessor, IMediator mediator)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _userAccessor = userAccessor;
        _mediator = mediator;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TenantNacRequirement requirement)
    {
        var currentUser = await _userAccessor.GetCurrentUser(_httpContextAccessor.HttpContext?.RequestAborted ?? default);
        if (currentUser.IsAuthenticated == false)
        {
            context.Fail(new(this, "User is not authenticated."));
            return;
        }
        var tenantId = currentUser.TenantId;
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

        var nacPolicy = tenant.NacPolicy;
        var remoteIpAddressResult = _httpContextAccessor.GetRequestIP(true);

        NacPolicyHelper.ValidateNacRequirement(context, this, requirement, nacPolicy, remoteIpAddressResult);
        return;
    }
}

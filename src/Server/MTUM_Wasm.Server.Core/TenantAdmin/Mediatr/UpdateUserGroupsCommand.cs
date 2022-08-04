using MTUM_Wasm.Server.Core.Identity.Service;
using MTUM_Wasm.Server.Core.TenantAdmin.Dto;
using MTUM_Wasm.Server.Core.TenantAdmin.Service;
using MTUM_Wasm.Shared.Core.Common.Authorization;
using MTUM_Wasm.Shared.Core.Common.Extension;
using MTUM_Wasm.Shared.Core.Common.Result;
using MediatR;
using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.TenantAdmin.Mediatr;

internal class UpdateUserGroupsCommand : IRequest<IServiceResult>
{
    public UpdateUserGroupsInput? Request { get; set; }
}

internal class UpdateUserGroupsCommandHandler : IRequestHandler<UpdateUserGroupsCommand, IServiceResult>
{
    private readonly IUserAccessor _userAccessor;
    private readonly ITenantAdminService _tenantAdminService;

    public UpdateUserGroupsCommandHandler(ITenantAdminService tenantAdminService, IUserAccessor userAccessor)
    {
        _tenantAdminService = tenantAdminService;
        _userAccessor = userAccessor;
    }

    public async Task<IServiceResult> Handle(UpdateUserGroupsCommand query, CancellationToken cancellationToken)
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

        var currentGroupsResult = await _tenantAdminService.GetUserGroups(new GetUserGroupsInput() { Email = query.Request.Email }, loggedInUserTenantId.Value, cancellationToken);
        if (!currentGroupsResult.Succeeded)
        {
            return Result.Fail("Cannot get user's current groups.");
        }
        var possibleGroups = Role.Name.PossibleTenantRoles;

        var currentGroups = currentGroupsResult.Data?.Groups ?? Enumerable.Empty<string>();
        //join possible group list with user's current groups to get a list of groups to work on
        //directly using current groups may result in removing groups not in list of possible groups.
        var currentGroupsWorkSet = (from cg in currentGroups
                                    join pg in possibleGroups
                                    on cg.ToLower(CultureInfo.InvariantCulture) equals pg.ToLower(CultureInfo.InvariantCulture)
                                    select cg).ToArray();

        var newGroups = query.Request.NewGroups;
        //join possible group list with user's new groups from request to get a list of groups to work on
        //directly using new groups may result in adding groups not in list of possible groups.
        var newGroupsWorkSet = (from ng in newGroups
                                join pg in possibleGroups
                                    on ng.ToLower(CultureInfo.InvariantCulture) equals pg.ToLower(CultureInfo.InvariantCulture)
                                select ng).ToArray();

        var groupsToRemove = currentGroupsWorkSet.Where(g => !newGroupsWorkSet.Any(ng => ng.Equals(g, StringComparison.OrdinalIgnoreCase))).ToArray();
        var groupsToAdd = newGroupsWorkSet.Where(ng => !currentGroupsWorkSet.Any(g => g.Equals(ng, StringComparison.OrdinalIgnoreCase))).ToArray();

        //check if logged in tenant admin is trying to remove own tenant admin group and forbid it
        if (groupsToRemove.Contains(Role.Name.TenantAdmin, StringComparer.OrdinalIgnoreCase))
        {
            var loggedInUserEmail = claimsIdentity?.GetEmail();
            if (string.IsNullOrWhiteSpace(loggedInUserEmail))
            {
                return Result.Fail("Cannot determine logged in user's email address.");
            }
            if (loggedInUserEmail.Equals(query.Request.Email, StringComparison.OrdinalIgnoreCase))
            {
                return Result.Fail("Logged in users cannot remove their own tenant admin group.");
            }
        }

        query.Request.GroupsToAdd = groupsToAdd;
        query.Request.GroupsToRemove = groupsToRemove;
        return await _tenantAdminService.UpdateUserGroups(query.Request, loggedInUserTenantId.Value, cancellationToken);
    }
}
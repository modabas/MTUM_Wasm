using MTUM_Wasm.Server.Core.Identity.Service;
using MTUM_Wasm.Server.Core.SystemAdmin.Dto;
using MTUM_Wasm.Server.Core.SystemAdmin.Service;
using MTUM_Wasm.Shared.Core.Common.Authorization;
using MTUM_Wasm.Shared.Core.Common.Extension;
using MTUM_Wasm.Shared.Core.Common.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Mediatr;

internal class UpdateUserGroupsCommand : IRequest<IServiceResult>
{
    public UpdateUserGroupsInput? Request { get; set; }
}

internal class UpdateUserGroupsCommandHandler : IRequestHandler<UpdateUserGroupsCommand, IServiceResult>
{
    private readonly ISystemAdminService _systemAdminService;
    private readonly IUserAccessor _userAccessor;

    public UpdateUserGroupsCommandHandler(ISystemAdminService systemAdminService, IUserAccessor userAccessor)
    {
        _systemAdminService = systemAdminService;
        _userAccessor = userAccessor;
    }

    public async Task<IServiceResult> Handle(UpdateUserGroupsCommand query, CancellationToken cancellationToken)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        if (query.Request is null)
            throw new ArgumentNullException(nameof(query), "The value of 'query.Request' should not be null");


        var currentGroupsResult = await _systemAdminService.GetUserGroups(new GetUserGroupsInput() { Email = query.Request.Email }, cancellationToken);
        if (!currentGroupsResult.Succeeded)
        {
            return Result.Fail(currentGroupsResult.Messages);
        }

        IEnumerable<string> possibleGroups = Enumerable.Empty<string>();
        if (query.Request.OpMode == UpdateUserGroupsOpModeDto.PossibleSystemRoles)
            possibleGroups = Role.Name.PossibleSystemRoles;
        else if (query.Request.OpMode == UpdateUserGroupsOpModeDto.PossibleTenantRoles)
            possibleGroups = Role.Name.PossibleTenantRoles;

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

        //check if logged in system admin is trying to remove own system admin group and forbid it
        if (groupsToRemove.Contains(Role.Name.SystemAdmin, StringComparer.OrdinalIgnoreCase))
        {
            var claimsIdentity = (ClaimsIdentity?)_userAccessor.User?.Identity;
            var loggedInUserEmail = claimsIdentity?.GetEmail();
            if (string.IsNullOrWhiteSpace(loggedInUserEmail))
            {
                return Result.Fail("Cannot determine logged in user's email address.");
            }
            if (loggedInUserEmail.Equals(query.Request.Email, StringComparison.OrdinalIgnoreCase))
            {
                return Result.Fail("Logged in users cannot remove thamselves from system admin group.");
            }
        }

        //check if user belongs to a tenant. If so don't let it be added as system admin
        if (groupsToAdd.Contains(Role.Name.SystemAdmin, StringComparer.OrdinalIgnoreCase))
        {
            var getUserResult = await _systemAdminService.GetUser(new GetUserInput() { Email = query.Request.Email }, cancellationToken);
            if (!getUserResult.Succeeded)
            {
                return Result.Fail(getUserResult.Messages);
            }
            if (getUserResult.Data is null || getUserResult.Data.User is null)
            {
                return Result.Fail($"Cannot check tenant information for user: '{query.Request.Email}'.");
            }

            var tenantId = getUserResult.Data.User.TenantId;
            if (tenantId is not null)
            {
                return Result.Fail($"User already belongs to tenant id: '{tenantId}'.");
            }
        }

        query.Request.GroupsToAdd = groupsToAdd;
        query.Request.GroupsToRemove = groupsToRemove;
        return await _systemAdminService.UpdateUserGroups(query.Request, cancellationToken);
    }
}
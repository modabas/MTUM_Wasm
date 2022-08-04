using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using MTUM_Wasm.Server.Core.Common.Entity;
using MTUM_Wasm.Server.Core.Identity.Dto;
using MTUM_Wasm.Server.Core.Identity.Service;
using MTUM_Wasm.Server.Infrastructure.Identity.AwsCognito.Mapping;
using MTUM_Wasm.Server.Infrastructure.Identity.AwsCognito.Utility;
using MTUM_Wasm.Shared.Core.Common.Result;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Infrastructure.Identity.AwsCognito.Service;

internal class AwsCognitoIdentityManagementService : IIdentityManagementService
{
    private readonly IAmazonCognitoIdentityProvider _provider;
    private readonly ICognitoUserPoolWrapper _userPool;
    private readonly ILogger<AwsCognitoIdentityManagementService> _logger;
    private readonly AwsCognitoOptions _options;

    private const string AttributeNameForTenant = "preferred_username";

    private const int MaxUsersPerTenant = 40;

    public AwsCognitoIdentityManagementService(IAmazonCognitoIdentityProvider provider,
        ICognitoUserPoolWrapper userPool,
        ILogger<AwsCognitoIdentityManagementService> logger,
        IOptions<AwsCognitoOptions> awsCognitoOptions)
    {
        _provider = provider;
        _userPool = userPool;
        _logger = logger;
        _options = awsCognitoOptions.Value;
    }

    public async Task<IServiceResult<GetUsersOutput>> GetUsersByTenant(Guid tenantId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var users = await FindByTenantIdAsync(tenantId, cancellationToken);
        if (users is null || users.Data is null)
        {
            return Result<GetUsersOutput>.Fail($"Unable to retrieve users for tenant '{tenantId}'.");
        }
        return Result<GetUsersOutput>.Success(new GetUsersOutput() { Users = users.Data });
    }

    public async Task<IServiceResult> UpdateUserAttributes(UpdateUserAttributesInput requestDto, Guid? loggedInUserTenantIdToCompare, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var findUserResult = await ServiceHelper.FindByEmailAsync(_provider, _userPool, ServiceHelper.NormalizeEmail(requestDto.Email), cancellationToken);
        if (!findUserResult.Succeeded)
        {
            return Result.Fail(findUserResult.Messages);
        }
        if (findUserResult.Data is null)
        {
            return Result.Fail($"Unable to retrieve user '{requestDto.Email}'.");
        }

        //if an operator tenant is supplied to compare with target user tanant, do so.
        var compareResult = CompareTenants(findUserResult.Data, loggedInUserTenantIdToCompare);
        if (!compareResult.Succeeded)
            return compareResult;

        var username = findUserResult.Data.UserName;
        var ret = await UpdateUserAttributes(username, requestDto.ToAttributeTypeList(), cancellationToken);
        return ret;
    }

    public async Task<IServiceResult> ChangeUserState(ChangeUserStateInput requestDto, Guid? loggedInUserTenantIdToCompare, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var findUserResult = await ServiceHelper.FindByEmailAsync(_provider, _userPool, ServiceHelper.NormalizeEmail(requestDto.Email), cancellationToken);
        if (!findUserResult.Succeeded)
        {
            return Result.Fail(findUserResult.Messages);
        }
        if (findUserResult.Data is null)
        {
            return Result.Fail($"Unable to retrieve user '{requestDto.Email}'.");
        }

        //if an operator tenant is supplied to compare with target user tanant, do so.
        var compareResult = CompareTenants(findUserResult.Data, loggedInUserTenantIdToCompare);
        if (!compareResult.Succeeded)
            return compareResult;

        var username = findUserResult.Data.UserName;
        if (requestDto.SetEnabled)
            return await EnableUser(username, cancellationToken);
        else
            return await DisableUser(username, cancellationToken);
    }

    public async Task<IServiceResult<GetUserGroupsOutput>> GetUserGroups(GetUserGroupsInput requestDto, Guid? loggedInUserTenantIdToCompare, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var findUserResult = await ServiceHelper.FindByEmailAsync(_provider, _userPool, ServiceHelper.NormalizeEmail(requestDto.Email), cancellationToken);
        if (!findUserResult.Succeeded)
        {
            return Result<GetUserGroupsOutput>.Fail(findUserResult.Messages);
        }
        if (findUserResult.Data is null)
        {
            return Result<GetUserGroupsOutput>.Fail($"Unable to retrieve user '{requestDto.Email}'.");
        }

        //if an operator tenant is supplied to compare with target user tanant, do so.
        var compareResult = CompareTenants(findUserResult.Data, loggedInUserTenantIdToCompare);
        if (!compareResult.Succeeded)
            return Result<GetUserGroupsOutput>.Fail(compareResult.Messages);

        var username = findUserResult.Data.UserName;
        var ret = await GetUserGroups(username, cancellationToken);
        if (ret.Succeeded)
            return Result<GetUserGroupsOutput>.Success(new GetUserGroupsOutput()
            {
                Groups = ret.Data ?? Enumerable.Empty<string>()
            });
        else
            return Result<GetUserGroupsOutput>.Fail(ret.Messages);
    }

    public async Task<IServiceResult> UpdateUserGroups(UpdateUserGroupsInput requestDto, Guid? loggedInUserTenantIdToCompare, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (requestDto.GroupsToAdd.Count() == 0 && requestDto.GroupsToRemove.Count() == 0)
            return Result.Success();


        var findUserResult = await ServiceHelper.FindByEmailAsync(_provider, _userPool, ServiceHelper.NormalizeEmail(requestDto.Email), cancellationToken);
        if (!findUserResult.Succeeded)
        {
            return Result.Fail(findUserResult.Messages);
        }
        if (findUserResult.Data is null)
        {
            return Result.Fail($"Unable to retrieve user '{requestDto.Email}'.");
        }

        //if an operator tenant is supplied to compare with target user tanant, do so.
        var compareResult = CompareTenants(findUserResult.Data, loggedInUserTenantIdToCompare);
        if (!compareResult.Succeeded)
            return Result.Fail(compareResult.Messages);

        var username = findUserResult.Data.UserName;
        var addRet = await AddUserToGroups(username, requestDto.GroupsToAdd, cancellationToken);
        var removeRet = await RemoveUserFromGroups(username, requestDto.GroupsToRemove, cancellationToken);

        if (addRet.Succeeded && removeRet.Succeeded)
        {
            return Result.Success();
        }
        else
        {
            var failMessages = new List<string>(addRet.Messages);
            failMessages.AddRange(removeRet.Messages);
            return Result.Fail(failMessages);
        }
    }

    public async Task<IServiceResult> CreateUser(CreateUserInput requestDto, Guid? tenantId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        requestDto.Email = ServiceHelper.NormalizeEmail(requestDto.Email);
        var findUserResult = await ServiceHelper.FindByEmailAsync(_provider, _userPool, requestDto.Email, cancellationToken);
        if (findUserResult.Succeeded)
        {
            return Result<GetUserOutput>.Fail($"User already exists: '{requestDto.Email}'.");
        }

        //we are trying to create a tenant user
        if (tenantId is not null)
        {
            var users = await FindByTenantIdAsync(tenantId.Value, cancellationToken);
            if (users.Succeeded)
            {
                if (users.Data is not null && users.Data.Count() >= MaxUsersPerTenant)
                {
                    return Result<GetUserOutput>.Fail($"Cannot create more than {MaxUsersPerTenant} users per tenant.");
                }
                //ok, continue user creation
            }
            else
            {
                return Result<GetUserOutput>.Fail($"Cannot check maximum user count for tenant: '{tenantId}'.");
            }
        }
        var ret = await CreateUser(requestDto.Email, requestDto.TemporaryPassword, requestDto.ToAttributeTypeList(tenantId), cancellationToken);
        return ret;
    }

    public async Task<IServiceResult<GetUsersInGroupOutput>> GetUsersInGroup(GetUsersInGroupInput requestDto, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var users = await FindByGroupNameAsync(requestDto.GroupName, cancellationToken);
        if (users is null || users.Data is null)
        {
            return Result<GetUsersInGroupOutput>.Fail($"Unable to retrieve users in group '{requestDto.GroupName}'.");
        }
        return Result<GetUsersInGroupOutput>.Success(new GetUsersInGroupOutput() { Users = users.Data });
    }

    public async Task<IServiceResult<GetUserOutput>> GetUser(GetUserInput requestDto, Guid? loggedInUserTenantIdToCompare, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var findUserResult = await ServiceHelper.FindByEmailAsync(_provider, _userPool, ServiceHelper.NormalizeEmail(requestDto.Email), cancellationToken);
        if (!findUserResult.Succeeded)
        {
            return Result<GetUserOutput>.Fail(findUserResult.Messages);
        }
        if (findUserResult.Data is null)
        {
            return Result<GetUserOutput>.Fail($"Unable to retrieve user '{requestDto.Email}'.");
        }

        //if an operator tenant is supplied to compare with target user tanant, do so.
        var compareResult = CompareTenants(findUserResult.Data, loggedInUserTenantIdToCompare);
        if (!compareResult.Succeeded)
            return Result<GetUserOutput>.Fail(compareResult.Messages);

        return Result<GetUserOutput>.Success(new GetUserOutput() { User = findUserResult.Data });
    }

    public async Task<IServiceResult> UpdateUserNacPolicy(UpdateUserNacPolicyInput requestDto, Guid? loggedInUserTenantIdToCompare, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var findUserResult = await ServiceHelper.FindByEmailAsync(_provider, _userPool, ServiceHelper.NormalizeEmail(requestDto.Email), cancellationToken);
        if (!findUserResult.Succeeded)
        {
            return Result.Fail(findUserResult.Messages);
        }
        if (findUserResult.Data is null)
        {
            return Result.Fail($"Unable to retrieve user '{requestDto.Email}'.");
        }

        //if an operator tenant is supplied to compare with target user tanant, do so.
        var compareResult = CompareTenants(findUserResult.Data, loggedInUserTenantIdToCompare);
        if (!compareResult.Succeeded)
            return compareResult;

        var username = findUserResult.Data.UserName;
        var ret = await UpdateUserAttributes(username, requestDto.ToAttributeTypeList(), cancellationToken);

        return ret;
    }

    private IServiceResult CompareTenants(SystemUserEntity? userResponse, Guid? loggedInUserTenantIdToCompare)
    {
        //if an operator tenant is supplied to compare with target user tanant, do so.
        if (loggedInUserTenantIdToCompare is not null)
        {
            if (userResponse is null || userResponse.TenantId is null)
            {
                return Result.Fail($"Cannot determine tenant for target user.");
            }
            else
            {
                if (!loggedInUserTenantIdToCompare.Value.Equals(userResponse.TenantId.Value))
                    return Result.Fail("Logged in user tenant and target user tenant do not match.");
            }
        }
        return Result.Success();
    }

    private async Task<IServiceResult<IEnumerable<string>>> GetUserGroups(string userName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await _provider.AdminListGroupsForUserAsync(new AdminListGroupsForUserRequest()
        {
            UserPoolId = _userPool.PoolID,
            Username = userName
        }, cancellationToken);

        if (ServiceHelper.IsSuccessStatusCode(result.HttpStatusCode))
        {
            if (result.Groups is not null)
                return Result<IEnumerable<string>>.Success(result.Groups.Select(g => g.GroupName).ToList().AsReadOnly());
            return Result<IEnumerable<string>>.Success(Enumerable.Empty<string>());
        }
        else
        {
            return Result<IEnumerable<string>>.Fail($"Get user groups failed with code:{result.HttpStatusCode}");
        }
    }

    private async Task<IServiceResult> CreateUser(string emailAddress, string temporaryPassword, List<AttributeType> attributes, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await _provider.AdminCreateUserAsync(new AdminCreateUserRequest()
        {
            DesiredDeliveryMediums = new List<string>(new string[] { "EMAIL" }),
            TemporaryPassword = temporaryPassword,
            UserAttributes = attributes,
            UserPoolId = _userPool.PoolID,
            Username = emailAddress
        }, cancellationToken);

        if (ServiceHelper.IsSuccessStatusCode(result.HttpStatusCode))
        {
            return Result.Success();
        }
        else
        {
            return Result.Fail($"User creation failed with code:{result.HttpStatusCode}");
        }
    }

    private async Task<IServiceResult> RemoveUserFromGroups(string userName, IEnumerable<string> groupsToRemove, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var failedGroups = new List<string>();
        foreach (var groupToRemove in groupsToRemove)
        {
            var result = await _provider.AdminRemoveUserFromGroupAsync(new AdminRemoveUserFromGroupRequest()
            {
                GroupName = groupToRemove,
                UserPoolId = _userPool.PoolID,
                Username = userName
            }, cancellationToken);
            if (!ServiceHelper.IsSuccessStatusCode(result.HttpStatusCode))
            {
                failedGroups.Add(groupToRemove);
            }
        }

        if (failedGroups.Count > 0)
        {
            return Result.Fail($"Failed to remove user from groups {string.Join(", ", failedGroups)}");
        }
        else
        {
            return Result.Success();
        }
    }

    private async Task<IServiceResult> AddUserToGroups(string userName, IEnumerable<string> groupsToAdd, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var failedGroups = new List<string>();
        foreach (var groupToAdd in groupsToAdd)
        {
            var result = await _provider.AdminAddUserToGroupAsync(new AdminAddUserToGroupRequest()
            {
                GroupName = groupToAdd,
                UserPoolId = _userPool.PoolID,
                Username = userName
            }, cancellationToken);
            if (!ServiceHelper.IsSuccessStatusCode(result.HttpStatusCode))
            {
                failedGroups.Add(groupToAdd);
            }
        }

        if (failedGroups.Count > 0)
        {
            return Result.Fail($"Failed to add user to groups {string.Join(", ", failedGroups)}");
        }
        else
        {
            return Result.Success();
        }
    }

    private async Task<IServiceResult> UpdateUserAttributes(string userName, List<AttributeType> attributes, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await _provider.AdminUpdateUserAttributesAsync(new AdminUpdateUserAttributesRequest()
        {
            UserAttributes = attributes,
            UserPoolId = _userPool.PoolID,
            Username = userName
        }, cancellationToken);

        if (ServiceHelper.IsSuccessStatusCode(result.HttpStatusCode))
        {
            return Result.Success();
        }
        else
        {
            return Result.Fail($"User attribute update failed with code:{result.HttpStatusCode}");
        }
    }

    private async Task<IServiceResult> EnableUser(string userName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await _provider.AdminEnableUserAsync(new AdminEnableUserRequest()
        {
            UserPoolId = _userPool.PoolID,
            Username = userName
        }, cancellationToken);

        if (ServiceHelper.IsSuccessStatusCode(result.HttpStatusCode))
        {
            return Result.Success();
        }
        else
        {
            return Result.Fail($"Enable user failed with code:{result.HttpStatusCode}");
        }
    }

    private async Task<IServiceResult> DisableUser(string userName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await _provider.AdminDisableUserAsync(new AdminDisableUserRequest()
        {
            UserPoolId = _userPool.PoolID,
            Username = userName
        }, cancellationToken);

        if (ServiceHelper.IsSuccessStatusCode(result.HttpStatusCode))
        {
            return Result.Success();
        }
        else
        {
            return Result.Fail($"Disable user failed with code:{result.HttpStatusCode}");
        }
    }

    private async Task<IServiceResult<IEnumerable<SystemUserEntity>>> FindByGroupNameAsync(string groupName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await _provider.ListUsersInGroupAsync(new ListUsersInGroupRequest()
        {
            GroupName = groupName,
            UserPoolId = _userPool.PoolID
        }, cancellationToken);

        if (ServiceHelper.IsSuccessStatusCode(result.HttpStatusCode) && result.Users is not null)
        {
            return Result<IEnumerable<SystemUserEntity>>.Success(result.Users.Select(p => p.ToSystemUserEntity()).ToList().AsReadOnly());
        }
        return Result<IEnumerable<SystemUserEntity>>.Fail($"List users failed with code:{result.HttpStatusCode}");
    }

    private async Task<IServiceResult<IEnumerable<SystemUserEntity>>> FindByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await _provider.ListUsersAsync(new ListUsersRequest
        {
            Filter = $"{AttributeNameForTenant} = \"{tenantId}\"",
            UserPoolId = _userPool.PoolID
        }, cancellationToken);

        if (ServiceHelper.IsSuccessStatusCode(result.HttpStatusCode) && result.Users is not null)
        {
            return Result<IEnumerable<SystemUserEntity>>.Success(result.Users.Select(p => p.ToSystemUserEntity()).ToList().AsReadOnly());
        }
        return Result<IEnumerable<SystemUserEntity>>.Fail($"List users failed with code:{result.HttpStatusCode}");
    }
}

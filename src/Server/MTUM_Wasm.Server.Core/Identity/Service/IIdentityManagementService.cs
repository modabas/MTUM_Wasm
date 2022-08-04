using MTUM_Wasm.Server.Core.Identity.Dto;
using MTUM_Wasm.Shared.Core.Common.Result;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.Identity.Service;

internal interface IIdentityManagementService
{
    Task<IServiceResult<GetUsersOutput>> GetUsersByTenant(Guid tenantId, CancellationToken cancellationToken);
    Task<IServiceResult> UpdateUserAttributes(UpdateUserAttributesInput requestDto, Guid? loggedInUserTenantIdToCompare, CancellationToken cancellationToken);
    Task<IServiceResult> CreateUser(CreateUserInput requestDto, Guid? tenantId, CancellationToken cancellationToken);
    Task<IServiceResult> ChangeUserState(ChangeUserStateInput requestDto, Guid? loggedInUserTenantIdToCompare, CancellationToken cancellationToken);
    Task<IServiceResult<GetUserGroupsOutput>> GetUserGroups(GetUserGroupsInput requestDto, Guid? loggedInUserTenantIdToCompare, CancellationToken cancellationToken);
    Task<IServiceResult<GetUserOutput>> GetUser(GetUserInput requestDto, Guid? loggedInUserTenantIdToCompare, CancellationToken cancellationToken);
    Task<IServiceResult> UpdateUserGroups(UpdateUserGroupsInput requestDto, Guid? loggedInUserTenantIdToCompare, CancellationToken cancellationToken);
    Task<IServiceResult<GetUsersInGroupOutput>> GetUsersInGroup(GetUsersInGroupInput requestDto, CancellationToken cancellationToken);
    Task<IServiceResult> UpdateUserNacPolicy(UpdateUserNacPolicyInput requestDto, Guid? loggedInUserTenantIdToCompare, CancellationToken cancellationToken);
}

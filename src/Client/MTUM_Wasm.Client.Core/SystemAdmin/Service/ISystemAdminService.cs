using MTUM_Wasm.Shared.Core.Common.Result;
using MTUM_Wasm.Shared.Core.SystemAdmin.Dto;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Client.Core.SystemAdmin.Service;

internal interface ISystemAdminService
{
    Task<IServiceResult<GetTenantsResponse>> GetTenants(CancellationToken cancellationToken);
    Task<IServiceResult> CreateTenant(CreateTenantRequest request, CancellationToken cancellationToken);
    Task<IServiceResult> UpdateTenant(UpdateTenantRequest request, CancellationToken cancellationToken);
    Task<IServiceResult<GetTenantResponse>> GetTenant(GetTenantRequest request, CancellationToken cancellationToken);
    Task<IServiceResult<GetTenantUsersResponse>> GetTenantUsers(GetTenantUsersRequest request, CancellationToken cancellationToken);
    Task<IServiceResult> CreateUser(CreateUserRequest request, CancellationToken cancellationToken);
    Task<IServiceResult> ChangeUserState(ChangeUserStateRequest request, CancellationToken cancellationToken);
    Task<IServiceResult> UpdateUserAttributes(UpdateUserAttributesRequest request, CancellationToken cancellationToken);
    Task<IServiceResult<GetUserResponse>> GetUser(GetUserRequest request, CancellationToken cancellationToken);
    Task<IServiceResult<GetUserGroupsResponse>> GetUserGroups(GetUserGroupsRequest request, CancellationToken cancellationToken);
    Task<IServiceResult> UpdateUserGroups(UpdateUserGroupsRequest request, CancellationToken cancellationToken);
    Task<IServiceResult<SearchAuditLogsResponse>> SearchAuditLogs(SearchAuditLogsRequest request, CancellationToken cancellationToken);
    Task<IServiceResult<GetUsersInGroupResponse>> GetUsersInGroup(GetUsersInGroupRequest request, CancellationToken cancellationToken);
    Task<IServiceResult> UpdateUserNacPolicy(UpdateUserNacPolicyRequest request, CancellationToken cancellationToken);
    Task<IServiceResult> UpdateTenantNacPolicy(UpdateTenantNacPolicyRequest request, CancellationToken cancellationToken);
}

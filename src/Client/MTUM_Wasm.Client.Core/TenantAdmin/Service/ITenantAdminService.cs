using MTUM_Wasm.Shared.Core.Common.Result;
using MTUM_Wasm.Shared.Core.TenantAdmin.Dto;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Client.Core.TenantAdmin.Service;

internal interface ITenantAdminService
{
    Task<IServiceResult<GetUsersResponse>> GetUsers(CancellationToken cancellationToken);
    Task<IServiceResult> UpdateUserAttributes(UpdateUserAttributesRequest request, CancellationToken cancellationToken);
    Task<IServiceResult> CreateUser(CreateUserRequest request, CancellationToken cancellationToken);
    Task<IServiceResult> ChangeUserState(ChangeUserStateRequest request, CancellationToken cancellationToken);
    Task<IServiceResult<GetUserGroupsResponse>> GetUserGroups(GetUserGroupsRequest request, CancellationToken cancellationToken);
    Task<IServiceResult<GetUserResponse>> GetUser(GetUserRequest request, CancellationToken cancellationToken);
    Task<IServiceResult> UpdateUserGroups(UpdateUserGroupsRequest request, CancellationToken cancellationToken);
    Task<IServiceResult<SearchAuditLogsResponse>> SearchAuditLogs(SearchAuditLogsRequest request, CancellationToken cancellationToken);
    Task<IServiceResult> UpdateUserNacPolicy(UpdateUserNacPolicyRequest request, CancellationToken cancellationToken);
    Task<IServiceResult> UpdateTenantNacPolicy(UpdateTenantNacPolicyRequest request, CancellationToken cancellationToken);
}

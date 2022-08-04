using MTUM_Wasm.Server.Core.Common.Entity;
using MTUM_Wasm.Server.Core.SystemAdmin.Dto;
using MTUM_Wasm.Shared.Core.Common.Result;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Service;

internal interface ISystemAdminService
{
    Task<IServiceResult<TenantEntity>> CreateTenant(CreateTenantInput dto, CancellationToken cancellationToken);
    Task<IServiceResult<IEnumerable<TenantEntity>>> GetTenants(CancellationToken cancellationToken);
    Task<IServiceResult<GetTenantUsersOutput>> GetTenantUsers(GetTenantUsersInput dto, CancellationToken cancellationToken);
    Task<IServiceResult> CreateUser(CreateUserInput requestDto, CancellationToken cancellationToken);
    Task<IServiceResult> ChangeUserState(ChangeUserStateInput requestDto, CancellationToken cancellationToken);
    Task<IServiceResult> UpdateUserAttributes(UpdateUserAttributesInput requestDto, CancellationToken cancellationToken);
    Task<IServiceResult<GetUserOutput>> GetUser(GetUserInput requestDto, CancellationToken cancellationToken);
    Task<IServiceResult> UpdateUserGroups(UpdateUserGroupsInput requestDto, CancellationToken cancellationToken);
    Task<IServiceResult<GetUserGroupsOutput>> GetUserGroups(GetUserGroupsInput requestDto, CancellationToken cancellationToken);
    Task<IServiceResult<SearchAuditLogsOutput>> SearchAuditLogs(SearchAuditLogsInput requestDto, CancellationToken cancellationToken);
    Task<IServiceResult<GetUsersInGroupOutput>> GetUsersInGroup(GetUsersInGroupInput requestDto, CancellationToken cancellationToken);
    Task<IServiceResult> UpdateUserNacPolicy(UpdateUserNacPolicyInput requestDto, CancellationToken cancellationToken);
}

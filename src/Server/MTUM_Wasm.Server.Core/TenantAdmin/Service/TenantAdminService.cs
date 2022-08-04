using MTUM_Wasm.Server.Core.Database.Entity;
using MTUM_Wasm.Server.Core.Database.Service;
using MTUM_Wasm.Server.Core.Identity.Service;
using MTUM_Wasm.Server.Core.TenantAdmin.Dto;
using MTUM_Wasm.Server.Core.TenantAdmin.Mapping;
using MTUM_Wasm.Shared.Core.Common.Result;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.TenantAdmin.Service;

internal class TenantAdminService : ITenantAdminService
{
    private readonly IIdentityManagementService _identityManagementService;
    private readonly IAuditRepo _auditRepo;

    public TenantAdminService(IIdentityManagementService identityManagementService, IAuditRepo auditRepo)
    {
        _identityManagementService = identityManagementService;
        _auditRepo = auditRepo;
    }

    public async Task<IServiceResult> ChangeUserState(ChangeUserStateInput requestDto, Guid loggedInUserTenantIdToCompare, CancellationToken cancellationToken)
    {
        var ret = await _identityManagementService.ChangeUserState(requestDto.ToIdentityInput(), loggedInUserTenantIdToCompare, cancellationToken);
        if (ret.Succeeded)
            await _auditRepo.TryCreateEntry(new AuditLogEntry(new AuditLogItem("requestDto", requestDto), new AuditLogItem("loggedInUserTenantToCompare", loggedInUserTenantIdToCompare)), cancellationToken);
        return ret;
    }

    public async Task<IServiceResult> CreateUserForTenant(CreateUserInput requestDto, Guid tenantId, CancellationToken cancellationToken)
    {
        var ret = await _identityManagementService.CreateUser(requestDto.ToIdentityInput(), tenantId, cancellationToken);
        if (ret.Succeeded)
            await _auditRepo.TryCreateEntry(new AuditLogEntry(new AuditLogItem("requestDto", requestDto.MaskForAuditLog()), new AuditLogItem("tenant_id", tenantId)), cancellationToken);
        return ret;
    }

    public async Task<IServiceResult<GetUserOutput>> GetUser(GetUserInput requestDto, Guid loggedInUserTenantIdToCompare, CancellationToken cancellationToken)
    {
        var ret = await _identityManagementService.GetUser(requestDto.ToIdentityInput(), loggedInUserTenantIdToCompare, cancellationToken);
        if (ret.Succeeded)
            return Result<GetUserOutput>.Success(ret.Data?.ToTenantAdminOutput());
        return Result<GetUserOutput>.Fail(ret.Messages);
    }

    public async Task<IServiceResult<GetUserGroupsOutput>> GetUserGroups(GetUserGroupsInput requestDto, Guid loggedInUserTenantIdToCompare, CancellationToken cancellationToken)
    {
        var ret = await _identityManagementService.GetUserGroups(requestDto.ToIdentityInput(), loggedInUserTenantIdToCompare, cancellationToken);
        if (ret.Succeeded)
            return Result<GetUserGroupsOutput>.Success(ret.Data?.ToTenantAdminOutput());
        return Result<GetUserGroupsOutput>.Fail(ret.Messages);
    }

    public async Task<IServiceResult<GetUsersOutput>> GetUsersByTenant(Guid tenantId, CancellationToken cancellationToken)
    {
        var ret = await _identityManagementService.GetUsersByTenant(tenantId, cancellationToken);
        if (ret.Succeeded)
            return Result<GetUsersOutput>.Success(ret.Data?.ToTenantAdminOutput());
        return Result<GetUsersOutput>.Fail(ret.Messages);
    }

    public async Task<IServiceResult> UpdateUserGroups(UpdateUserGroupsInput requestDto, Guid loggedInUserTenantIdToCompare, CancellationToken cancellationToken)
    {
        var ret = await _identityManagementService.UpdateUserGroups(requestDto.ToIdentityInput(), loggedInUserTenantIdToCompare, cancellationToken);
        if (ret.Succeeded)
            await _auditRepo.TryCreateEntry(new AuditLogEntry(new AuditLogItem("requestDto", requestDto), new AuditLogItem("loggedInUserTenantToCompare", loggedInUserTenantIdToCompare)), cancellationToken);
        return ret;
    }

    public async Task<IServiceResult> UpdateUserAttributes(UpdateUserAttributesInput requestDto, Guid loggedInUserTenantIdToCompare, CancellationToken cancellationToken)
    {
        var ret = await _identityManagementService.UpdateUserAttributes(requestDto.ToIdentityInput(), loggedInUserTenantIdToCompare, cancellationToken);
        if (ret.Succeeded)
            await _auditRepo.TryCreateEntry(new AuditLogEntry(new AuditLogItem("requestDto", requestDto), new AuditLogItem("loggedInUserTenantToCompare", loggedInUserTenantIdToCompare)), cancellationToken);
        return ret;
    }

    public async Task<IServiceResult<SearchAuditLogsOutput>> SearchAuditLogs(SearchAuditLogsInput requestDto, Guid tenantId, CancellationToken cancellationToken)
    {
        var ret = await _auditRepo.Search(requestDto.ToDatabaseInput(tenantId), cancellationToken);
        if (ret.Succeeded)
            return Result<SearchAuditLogsOutput>.Success(ret.Data?.ToTenantAdminOutput());
        return Result<SearchAuditLogsOutput>.Fail(ret.Messages);
    }

    public async Task<IServiceResult> UpdateUserNacPolicy(UpdateUserNacPolicyInput requestDto, Guid loggedInUserTenantIdToCompare, CancellationToken cancellationToken)
    {
        var ret = await _identityManagementService.UpdateUserNacPolicy(requestDto.ToIdentityInput(), loggedInUserTenantIdToCompare, cancellationToken);
        if (ret.Succeeded)
            await _auditRepo.TryCreateEntry(new AuditLogEntry(new AuditLogItem("requestDto", requestDto), new AuditLogItem("loggedInUserTenantToCompare", loggedInUserTenantIdToCompare)), cancellationToken);
        return ret;
    }
}

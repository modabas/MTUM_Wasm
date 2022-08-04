using MTUM_Wasm.Server.Core.Common.Entity;
using MTUM_Wasm.Server.Core.Common.Service;
using MTUM_Wasm.Server.Core.Database.Entity;
using MTUM_Wasm.Server.Core.Database.Service;
using MTUM_Wasm.Server.Core.Identity.Service;
using MTUM_Wasm.Server.Core.SystemAdmin.Dto;
using MTUM_Wasm.Server.Core.SystemAdmin.Mapping;
using MTUM_Wasm.Server.Core.TenantAdmin.Mapping;
using MTUM_Wasm.Shared.Core.Common.Result;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Service
{
    internal class SystemAdminService : ISystemAdminService
    {
        private readonly ITenantRepo _tenantRepo;
        private readonly IAuditRepo _auditRepo;
        private readonly IIdentityManagementService _identityManagementService;

        public SystemAdminService(ITenantRepo tenantRepo, IAuditRepo auditRepo, IIdentityManagementService identityManagementService)
        {
            _tenantRepo = tenantRepo;
            _auditRepo = auditRepo;
            _identityManagementService = identityManagementService;
        }

        public async Task<IServiceResult> ChangeUserState(ChangeUserStateInput requestDto, CancellationToken cancellationToken)
        {
            var ret = await _identityManagementService.ChangeUserState(requestDto.ToIdentityInput(), null, cancellationToken);
            if (ret.Succeeded)
                await _auditRepo.TryCreateEntry(new AuditLogEntry(new AuditLogItem("requestDto", requestDto)), cancellationToken);
            return ret;
        }

        public Task<IServiceResult<TenantEntity>> CreateTenant(CreateTenantInput dto, CancellationToken cancellationToken)
        {
            return _tenantRepo.CreateTenant(dto.Name, cancellationToken);
        }

        public async Task<IServiceResult> CreateUser(CreateUserInput requestDto, CancellationToken cancellationToken)
        {
            var ret = await _identityManagementService.CreateUser(requestDto.ToIdentityInput(), requestDto.TenantId == Guid.Empty ? null : requestDto.TenantId, cancellationToken);
            if (ret.Succeeded)
                await _auditRepo.TryCreateEntry(new AuditLogEntry(new AuditLogItem("requestDto", requestDto.MaskForAuditLog())), cancellationToken);
            return ret;
        }

        public Task<IServiceResult<IEnumerable<TenantEntity>>> GetTenants(CancellationToken cancellationToken)
        {
            return _tenantRepo.GetTenants(cancellationToken);
        }

        public async Task<IServiceResult<GetTenantUsersOutput>> GetTenantUsers(GetTenantUsersInput requestDto, CancellationToken cancellationToken)
        {
            var usersResult = await _identityManagementService.GetUsersByTenant(requestDto.TenantId, cancellationToken);
            if (usersResult.Succeeded)
            {
                return Result<GetTenantUsersOutput>.Success(usersResult.Data?.ToSystemAdminOutput());
            }
            return Result<GetTenantUsersOutput>.Fail(usersResult.Messages);
        }

        public async Task<IServiceResult<GetUserOutput>> GetUser(GetUserInput requestDto, CancellationToken cancellationToken)
        {
            var userResult = await _identityManagementService.GetUser(requestDto.ToIdentityInput(), null, cancellationToken);
            if (userResult.Succeeded)
            {
                return Result<GetUserOutput>.Success(userResult.Data?.ToSystemAdminDto());
            }
            return Result<GetUserOutput>.Fail(userResult.Messages);
        }

        public async Task<IServiceResult<GetUserGroupsOutput>> GetUserGroups(GetUserGroupsInput requestDto, CancellationToken cancellationToken)
        {
            var userResult = await _identityManagementService.GetUserGroups(requestDto.ToIdentityInput(), null, cancellationToken);
            if (userResult.Succeeded)
            {
                return Result<GetUserGroupsOutput>.Success(userResult.Data?.ToSystemAdminOutput());
            }
            return Result<GetUserGroupsOutput>.Fail(userResult.Messages);
        }

        public async Task<IServiceResult> UpdateUserAttributes(UpdateUserAttributesInput requestDto, CancellationToken cancellationToken)
        {
            var ret = await _identityManagementService.UpdateUserAttributes(requestDto.ToIdentityInput(), null, cancellationToken);
            if (ret.Succeeded)
                await _auditRepo.TryCreateEntry(new AuditLogEntry(new AuditLogItem("requestDto", requestDto)), cancellationToken);
            return ret;
        }

        public async Task<IServiceResult> UpdateUserGroups(UpdateUserGroupsInput requestDto, CancellationToken cancellationToken)
        {
            var ret = await _identityManagementService.UpdateUserGroups(requestDto.ToIdentityInput(), null, cancellationToken);
            if (ret.Succeeded)
                await _auditRepo.TryCreateEntry(new AuditLogEntry(new AuditLogItem("requestDto", requestDto)), cancellationToken);
            return ret;
        }

        public async Task<IServiceResult<SearchAuditLogsOutput>> SearchAuditLogs(SearchAuditLogsInput requestDto, CancellationToken cancellationToken)
        {
            var ret = await _auditRepo.Search(requestDto.ToDatabaseInput(), cancellationToken);
            if (ret.Succeeded)
                return Result<SearchAuditLogsOutput>.Success(ret.Data?.ToSystemAdminOutput());
            return Result<SearchAuditLogsOutput>.Fail(ret.Messages);
        }

        public async Task<IServiceResult<GetUsersInGroupOutput>> GetUsersInGroup(GetUsersInGroupInput requestDto, CancellationToken cancellationToken)
        {
            var userResult = await _identityManagementService.GetUsersInGroup(requestDto.ToIdentityInput(), cancellationToken);
            if (userResult.Succeeded)
            {
                return Result<GetUsersInGroupOutput>.Success(userResult.Data?.ToSystemAdminOutput());
            }
            return Result<GetUsersInGroupOutput>.Fail(userResult.Messages);
        }

        public async Task<IServiceResult> UpdateUserNacPolicy(UpdateUserNacPolicyInput requestDto, CancellationToken cancellationToken)
        {
            var ret = await _identityManagementService.UpdateUserNacPolicy(requestDto.ToIdentityInput(), null, cancellationToken);
            if (ret.Succeeded)
                await _auditRepo.TryCreateEntry(new AuditLogEntry(new AuditLogItem("requestDto", requestDto)), cancellationToken);
            return ret;
        }
    }
}

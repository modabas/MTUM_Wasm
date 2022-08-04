using MTUM_Wasm.Server.Core.Database.Mapping;
using MTUM_Wasm.Server.Core.TenantAdmin.Dto;
using MTUM_Wasm.Shared.Core.TenantAdmin.Dto;
using System;
using System.Linq;

namespace MTUM_Wasm.Server.Core.TenantAdmin.Mapping
{
    internal static class SearchAuditLogsExtensions
    {
        public static SearchAuditLogsInput ToInput(this SearchAuditLogsRequest request)
        {
            return new SearchAuditLogsInput
            {
                CommandName = request.CommandName,
                UserEmail = request.UserEmail,
                StartDate = request.StartDate ?? DateTime.Today,
                EndDate = request.EndDate ?? DateTime.Today,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

        public static Database.Dto.SearchAuditLogsInput ToDatabaseInput(this SearchAuditLogsInput input, Guid tenantId)
        {
            return new Database.Dto.SearchAuditLogsInput
            {
                CommandName = input.CommandName,
                UserEmail = input.UserEmail,
                StartDate = input.StartDate,
                EndDate = input.EndDate,
                Page = input.Page,
                PageSize = input.PageSize,
                TenantId = tenantId
            };
        }

        public static SearchAuditLogsResponse ToResponse(this SearchAuditLogsOutput output)
        {
            return new SearchAuditLogsResponse
            {
                Logs = output.Logs.Select(dtoLog => dtoLog.ToPrettifiedSharedDto()),
                TotalCount = output.TotalCount
            };
        }

        public static SearchAuditLogsOutput ToTenantAdminOutput(this Database.Dto.SearchAuditLogsOutput dto)
        {
            return new SearchAuditLogsOutput
            {
                Logs = dto.Logs,
                TotalCount = dto.TotalCount
            };
        }
    }
}

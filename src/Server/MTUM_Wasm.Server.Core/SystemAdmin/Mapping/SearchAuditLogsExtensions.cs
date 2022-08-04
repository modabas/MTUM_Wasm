using MTUM_Wasm.Server.Core.Database.Mapping;
using MTUM_Wasm.Server.Core.SystemAdmin.Dto;
using MTUM_Wasm.Shared.Core.SystemAdmin.Dto;
using System;
using System.Linq;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Mapping
{
    internal static class SearchAuditLogsExtensions
    {
        public static SearchAuditLogsInput ToInput(this SearchAuditLogsRequest request)
        {
            return new SearchAuditLogsInput
            {
                CommandName = request.CommandName,
                UserEmail = request.UserEmail,
                TenantId = request.TenantId,
                StartDate = request.StartDate ?? throw new ArgumentNullException(nameof(request), "The value of 'model.StartDate' should not be null"),
                EndDate = request.EndDate ?? throw new ArgumentNullException(nameof(request), "The value of 'model.EndDate' should not be null"),
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

        public static Database.Dto.SearchAuditLogsInput ToDatabaseInput(this SearchAuditLogsInput input)
        {
            return new Database.Dto.SearchAuditLogsInput
            {
                CommandName = input.CommandName,
                UserEmail = input.UserEmail,
                TenantId = input.TenantId,
                StartDate = input.StartDate,
                EndDate = input.EndDate,
                Page = input.Page,
                PageSize = input.PageSize
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

        public static SearchAuditLogsOutput ToSystemAdminOutput(this Database.Dto.SearchAuditLogsOutput output)
        {
            return new SearchAuditLogsOutput
            {
                Logs = output.Logs,
                TotalCount = output.TotalCount
            };
        }
    }
}

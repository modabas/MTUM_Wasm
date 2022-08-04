using MTUM_Wasm.Shared.Core.Common.Validation;
using MTUM_Wasm.Shared.Core.TenantAdmin.Dto;
using FluentValidation;

namespace MTUM_Wasm.Shared.Core.TenantAdmin.Validation;

public class SearchAuditLogsRequestValidator : ValidatorBase<SearchAuditLogsRequest>
{
    public SearchAuditLogsRequestValidator()
    {
        RuleFor(x => x.Page)
            .NotEmpty()
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .NotEmpty()
            .GreaterThanOrEqualTo(10)
            .LessThanOrEqualTo(50);

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(new System.DateTime(2022,03,01))
            .LessThanOrEqualTo(x => x.EndDate);

        RuleFor(x => x.EndDate)
            .NotEmpty();

        RuleFor(x => x.UserEmail)
            .EmailAddress().Unless(x => string.IsNullOrWhiteSpace(x.UserEmail));
    }

}

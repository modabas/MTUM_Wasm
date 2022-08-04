using MTUM_Wasm.Shared.Core.Common.Validation;
using MTUM_Wasm.Shared.Core.SystemAdmin.Dto;
using FluentValidation;

namespace MTUM_Wasm.Shared.Core.SystemAdmin.Validation;

public class GetTenantUsersRequestValidator : ValidatorBase<GetTenantUsersRequest>
{
    public GetTenantUsersRequestValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty();
    }
}

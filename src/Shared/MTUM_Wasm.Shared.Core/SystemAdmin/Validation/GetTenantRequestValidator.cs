using MTUM_Wasm.Shared.Core.Common.Validation;
using MTUM_Wasm.Shared.Core.SystemAdmin.Dto;
using FluentValidation;

namespace MTUM_Wasm.Shared.Core.SystemAdmin.Validation;

public class GetTenantRequestValidator : ValidatorBase<GetTenantRequest>
{
    public GetTenantRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}

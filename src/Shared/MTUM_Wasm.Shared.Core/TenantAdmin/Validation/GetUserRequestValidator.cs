using MTUM_Wasm.Shared.Core.Common.Validation;
using MTUM_Wasm.Shared.Core.TenantAdmin.Dto;
using FluentValidation;

namespace MTUM_Wasm.Shared.Core.TenantAdmin.Validation;

public class GetUserRequestValidator : ValidatorBase<GetUserRequest>
{
    public GetUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .NotEmpty();
    }
}

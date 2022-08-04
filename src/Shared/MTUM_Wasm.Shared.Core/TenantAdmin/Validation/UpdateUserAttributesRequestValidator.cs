using MTUM_Wasm.Shared.Core.Common.Validation;
using MTUM_Wasm.Shared.Core.TenantAdmin.Dto;
using FluentValidation;

namespace MTUM_Wasm.Shared.Core.TenantAdmin.Validation;

public class UpdateUserAttributesRequestValidator : ValidatorBase<UpdateUserAttributesRequest>
{
    public UpdateUserAttributesRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.MiddleName)
            .Matches(Validator.Expression.EmptyOrOnlyWords).WithMessage(Validator.Message.EmptyOrOnlyWords);

        RuleFor(x => x.GivenName)
            .NotEmpty()
            .Matches(Validator.Expression.OnlyWords).WithMessage(Validator.Message.OnlyWords);

        RuleFor(x => x.FamilyName)
            .NotEmpty()
            .Matches(Validator.Expression.OnlyWords).WithMessage(Validator.Message.OnlyWords);
    }

}

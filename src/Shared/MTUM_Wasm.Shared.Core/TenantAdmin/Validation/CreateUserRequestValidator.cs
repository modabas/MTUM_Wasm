using MTUM_Wasm.Shared.Core.Common.Validation;
using MTUM_Wasm.Shared.Core.TenantAdmin.Dto;
using FluentValidation;
using System;

namespace MTUM_Wasm.Shared.Core.TenantAdmin.Validation;

public class CreateUserRequestValidator : ValidatorBase<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .NotEmpty();

        RuleFor(x => x.MiddleName)
            .Matches(Validator.Expression.EmptyOrOnlyWords).WithMessage(Validator.Message.EmptyOrOnlyWords);

        RuleFor(x => x.GivenName)
            .NotEmpty()
            .Matches(Validator.Expression.OnlyWords).WithMessage(Validator.Message.OnlyWords);

        RuleFor(x => x.FamilyName)
            .NotEmpty()
            .Matches(Validator.Expression.OnlyWords).WithMessage(Validator.Message.OnlyWords);

        RuleFor(x => x.TemporaryPassword)
            .NotEmpty()
            .Matches(Validator.Expression.Password).WithMessage(Validator.Message.Password)
            .MinimumLength(8);

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.TemporaryPassword, StringComparer.Ordinal)
            .WithMessage("'Confirm Password' must be equal to 'Temporary Password'");

    }

}

using MTUM_Wasm.Shared.Core.Common.Validation;
using MTUM_Wasm.Shared.Core.Identity.Dto;
using FluentValidation;
using System;

namespace MTUM_Wasm.Shared.Core.Identity.Validation;

public class ResetPasswordRequestValidator : ValidatorBase<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.EmailAddress)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.ResetToken)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .Matches(Validator.Expression.Password).WithMessage(Validator.Message.Password)
            .MinimumLength(8);

        RuleFor(x => x.ConfirmNewPassword)
            .Equal(x => x.NewPassword, StringComparer.Ordinal)
            .WithMessage("'Confirm New Password' and 'New Password' must be same.");
    }
}

using MTUM_Wasm.Shared.Core.Common.Validation;
using MTUM_Wasm.Shared.Core.Identity.Dto;
using FluentValidation;

namespace MTUM_Wasm.Shared.Core.Identity.Validation;

public class ForgotPasswordRequestValidator : ValidatorBase<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(x => x.EmailAddress)
            .NotEmpty()
            .EmailAddress();

    }
}
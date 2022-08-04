using MTUM_Wasm.Shared.Core.Common.Validation;
using MTUM_Wasm.Shared.Core.Identity.Dto;
using FluentValidation;

namespace MTUM_Wasm.Shared.Core.Identity.Validation;

public class LoginRequestValidator:ValidatorBase<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.EmailAddress)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}

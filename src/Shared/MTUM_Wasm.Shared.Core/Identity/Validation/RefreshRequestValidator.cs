using MTUM_Wasm.Shared.Core.Common.Validation;
using MTUM_Wasm.Shared.Core.Identity.Dto;
using FluentValidation;

namespace MTUM_Wasm.Shared.Core.Identity.Validation;

public class RefreshRequestValidator : ValidatorBase<RefreshRequest>
{
    public RefreshRequestValidator()
    {
        RuleFor(x => x.RefreshTokenExpiresAt)
            .NotNull();

        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}

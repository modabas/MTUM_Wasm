using MTUM_Wasm.Shared.Core.Common.Validation;
using MTUM_Wasm.Shared.Core.SystemAdmin.Dto;
using FluentValidation;

namespace MTUM_Wasm.Shared.Core.SystemAdmin.Validation;

public class ChangeUserStateRequestValidator : ValidatorBase<ChangeUserStateRequest>
{
    public ChangeUserStateRequestValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .NotEmpty();
    }

}

using MTUM_Wasm.Shared.Core.Common.Validation;
using MTUM_Wasm.Shared.Core.Identity.Validation;
using MTUM_Wasm.Shared.Core.SystemAdmin.Dto;
using FluentValidation;

namespace MTUM_Wasm.Shared.Core.SystemAdmin.Validation;

public class UpdateUserNacPolicyRequestValidator : ValidatorBase<UpdateUserNacPolicyRequest>
{
    public UpdateUserNacPolicyRequestValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .NotEmpty();

        RuleFor(x => x.NacPolicy!)
            .NotNull()
            .SetValidator(new NacPolicyValidator()).Unless(x => x.NacPolicy is null);
    }
}
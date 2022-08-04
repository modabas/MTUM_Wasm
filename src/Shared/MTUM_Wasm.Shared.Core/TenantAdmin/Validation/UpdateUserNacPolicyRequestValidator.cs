using MTUM_Wasm.Shared.Core.Common.Validation;
using MTUM_Wasm.Shared.Core.Identity.Validation;
using MTUM_Wasm.Shared.Core.TenantAdmin.Dto;
using FluentValidation;

namespace MTUM_Wasm.Shared.Core.TenantAdmin.Validation;

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

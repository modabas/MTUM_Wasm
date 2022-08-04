using MTUM_Wasm.Shared.Core.Common.Validation;
using MTUM_Wasm.Shared.Core.Identity.Validation;
using MTUM_Wasm.Shared.Core.TenantAdmin.Dto;
using FluentValidation;

namespace MTUM_Wasm.Shared.Core.TenantAdmin.Validation;

public class UpdateTenantNacPolicyRequestValidator : ValidatorBase<UpdateTenantNacPolicyRequest>
{
    public UpdateTenantNacPolicyRequestValidator()
    {
        RuleFor(x => x.NacPolicy!)
            .NotNull()
            .SetValidator(new NacPolicyValidator()).Unless(x => x.NacPolicy is null);
    }
}

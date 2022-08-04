using MTUM_Wasm.Shared.Core.Common.Validation;
using MTUM_Wasm.Shared.Core.Identity.Entity;
using FluentValidation;

namespace MTUM_Wasm.Shared.Core.Identity.Validation;

public class NacPolicyValidator : ValidatorBase<NacPolicy>
{
    public NacPolicyValidator()
    {
        RuleFor(x => x.UseSafelist)
            .NotNull();

        RuleFor(x => x.Safelist)
            .NotNull();

        RuleForEach(x => x.Safelist)
            .NotNull()
            .Matches(Validator.Expression.IPAddress).WithMessage(Validator.Message.IPAddress); 

        RuleFor(x => x.Blacklist)
            .NotNull();

        RuleForEach(x => x.Blacklist)
            .NotNull()
            .Matches(Validator.Expression.IPAddress).WithMessage(Validator.Message.IPAddress);
    }
}

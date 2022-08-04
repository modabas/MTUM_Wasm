using MTUM_Wasm.Shared.Core.Common.Validation;
using FluentValidation;

namespace MTUM_Wasm.Client.Core.Utility.Validation;

internal class IPAddressValidator : ValidatorBase<string>
{
    public IPAddressValidator()
    {
        RuleFor(x => x)
            .NotNull()
            .Matches(Validator.Expression.IPAddress).WithMessage(Validator.Message.IPAddress);
    }
}

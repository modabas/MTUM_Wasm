using MTUM_Wasm.Shared.Core.Common.Validation;
using MTUM_Wasm.Shared.Core.TenantAdmin.Dto;
using FluentValidation;
using System;

namespace MTUM_Wasm.Shared.Core.TenantAdmin.Validation;

public class ChangeUserStateRequestValidator : ValidatorBase<ChangeUserStateRequest>
{
    public ChangeUserStateRequestValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .NotEmpty();
    }

}

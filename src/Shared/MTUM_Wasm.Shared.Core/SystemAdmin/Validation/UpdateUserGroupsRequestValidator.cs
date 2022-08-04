using MTUM_Wasm.Shared.Core.Common.Validation;
using MTUM_Wasm.Shared.Core.SystemAdmin.Dto;
using FluentValidation;

namespace MTUM_Wasm.Shared.Core.SystemAdmin.Validation;

public class UpdateUserGroupsRequestValidator : ValidatorBase<UpdateUserGroupsRequest>
{
    public UpdateUserGroupsRequestValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .NotEmpty();

        RuleFor(x => x.OpMode)
            .IsInEnum()
            .NotEmpty();

        RuleFor(x => x.NewGroups)
            .NotNull();
    }

}
using MTUM_Wasm.Shared.Core.Common.Validation;
using MTUM_Wasm.Shared.Core.TenantAdmin.Dto;
using FluentValidation;

namespace MTUM_Wasm.Shared.Core.TenantAdmin.Validation;

public class UpdateUserGroupsRequestValidator : ValidatorBase<UpdateUserGroupsRequest>
{
    public UpdateUserGroupsRequestValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .NotEmpty();

        RuleFor(x => x.NewGroups)
            .NotNull();
    }

}

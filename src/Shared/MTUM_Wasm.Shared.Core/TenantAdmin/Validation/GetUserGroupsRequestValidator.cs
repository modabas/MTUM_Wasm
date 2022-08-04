using MTUM_Wasm.Shared.Core.Common.Validation;
using MTUM_Wasm.Shared.Core.TenantAdmin.Dto;
using FluentValidation;

namespace MTUM_Wasm.Shared.Core.TenantAdmin.Validation;

public class GetUserGroupsRequestValidator : ValidatorBase<GetUserGroupsRequest>
{
    public GetUserGroupsRequestValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .NotEmpty();
    }

}

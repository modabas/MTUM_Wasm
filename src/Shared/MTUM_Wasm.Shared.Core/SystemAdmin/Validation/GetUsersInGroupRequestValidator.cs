using MTUM_Wasm.Shared.Core.Common.Authorization;
using MTUM_Wasm.Shared.Core.Common.Validation;
using MTUM_Wasm.Shared.Core.SystemAdmin.Dto;
using FluentValidation;
using System;
using System.Linq;

namespace MTUM_Wasm.Shared.Core.SystemAdmin.Validation;

public class GetUsersInGroupRequestValidator : ValidatorBase<GetUsersInGroupRequest>
{
    public GetUsersInGroupRequestValidator()
    {
        RuleFor(x => x.GroupName)
            .Must(g => Role.Name.AllPossibleGroups.Any(r => r.Equals(g, StringComparison.OrdinalIgnoreCase)))
            .WithMessage($"Must be one of {string.Join(", ", Role.Name.AllPossibleGroups)}.");

    }
}

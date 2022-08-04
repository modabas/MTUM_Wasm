using MTUM_Wasm.Server.Core.Common.Entity;
using MTUM_Wasm.Server.Core.SystemAdmin.Dto;
using MTUM_Wasm.Server.Core.SystemAdmin.Service;
using MTUM_Wasm.Shared.Core.Common.Result;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.SystemAdmin.Mediatr;

internal class GetTenantsQuery : IRequest<IServiceResult<GetTenantsOutput>>
{
}

internal class GetTenantsQueryHandler : IRequestHandler<GetTenantsQuery, IServiceResult<GetTenantsOutput>>
{
    private readonly ISystemAdminService _systemAdminService;

    public GetTenantsQueryHandler(ISystemAdminService systemAdminService)
    {
        _systemAdminService = systemAdminService;
    }

    public async Task<IServiceResult<GetTenantsOutput>> Handle(GetTenantsQuery query, CancellationToken cancellationToken)
    {
        var ret = await _systemAdminService.GetTenants(cancellationToken);
        if (ret.Succeeded)
            return Result<GetTenantsOutput>.Success(new GetTenantsOutput() { Tenants = ret.Data ?? Enumerable.Empty<TenantEntity>() });

        return Result<GetTenantsOutput>.Fail(ret.Messages);
    }
}


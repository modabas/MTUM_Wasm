using MTUM_Wasm.Server.Core.TenantViewerOrUp.Mapping;
using MTUM_Wasm.Server.Core.TenantViewerOrUp.Mediatr;
using MTUM_Wasm.Shared.Core.Common.Authorization;
using MTUM_Wasm.Shared.Core.Common.Result;
using MTUM_Wasm.Shared.Core.TenantViewerOrUp.Dto;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policy.Name.UserNac)]
    [Authorize(Policy = Policy.Name.TenantNac)]
    [Authorize(Policy = Policy.Name.IsTenantViewerOrUp)]
    [Authorize(Policy = Policy.Name.HasEnabledTenant)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(429)]
    [ProducesResponseType(500)]
    public class TenantViewerOrUpController : ControllerBase
    {
        private readonly ILogger<TenantViewerOrUpController> _logger;
        private readonly IMediator _mediator;

        public TenantViewerOrUpController(ILogger<TenantViewerOrUpController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet("getTenant")]
        [ProducesResponseType(200, Type = typeof(IServiceResult<GetTenantResponse>))]
        public async Task<IActionResult> GetTenant(CancellationToken cancellationToken)
        {
            var dtoResult = await _mediator.Send(new GetTenantQuery() { }, cancellationToken);
            if (dtoResult.Succeeded)
                return Ok(Result<GetTenantResponse>.Success(dtoResult.Data?.ToResponse()));
            return Ok(Result<GetTenantResponse>.Fail(dtoResult.Messages));
        }
    }
}

using MTUM_Wasm.Server.Core.TenantAdmin.Mapping;
using MTUM_Wasm.Server.Core.TenantAdmin.Mediatr;
using MTUM_Wasm.Shared.Core.Common.Authorization;
using MTUM_Wasm.Shared.Core.Common.Result;
using MTUM_Wasm.Shared.Core.TenantAdmin.Dto;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policy.Name.UserNac)]
    [Authorize(Policy = Policy.Name.TenantNac)]
    [Authorize(Policy = Policy.Name.IsTenantAdmin)]
    [Authorize(Policy = Policy.Name.HasEnabledTenant)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(429)]
    [ProducesResponseType(500)]
    public class TenantAdministrationController : ControllerBase
    {
        private readonly ILogger<TenantAdministrationController> _logger;
        private readonly IMediator _mediator;

        public TenantAdministrationController(ILogger<TenantAdministrationController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet("getUsers")]
        [ProducesResponseType(200, Type = typeof(IServiceResult<GetUsersResponse>))]
        public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
        {
            var dtoResult = await _mediator.Send(new GetUsersQuery(), cancellationToken);
            if (dtoResult.Succeeded)
                return Ok(Result<GetUsersResponse>.Success(dtoResult.Data?.ToResponse()));
            return Ok(Result<GetUsersResponse>.Fail(dtoResult.Messages));
        }

        [HttpPost("updateUserAttributes")]
        [ProducesResponseType(200, Type = typeof(IServiceResult))]
        public async Task<IActionResult> UpdateUserAttributes([FromBody]UpdateUserAttributesRequest request, CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var ret = await _mediator.Send(new UpdateUserAttributesCommand() { Request = request.ToInput() }, cancellationToken);

            return Ok(ret);
        }

        [HttpPost("createUser")]
        [ProducesResponseType(200, Type = typeof(IServiceResult))]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var ret = await _mediator.Send(new CreateUserCommand() { Request = request.ToInput() }, cancellationToken);

            return Ok(ret);
        }

        [HttpPost("changeUserState")]
        [ProducesResponseType(200, Type = typeof(IServiceResult))]
        public async Task<IActionResult> ChangeUserState([FromBody] ChangeUserStateRequest request, CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var ret = await _mediator.Send(new ChangeUserStateCommand() { Request = request.ToInput() }, cancellationToken);

            return Ok(ret);
        }

        [HttpGet("getUserGroups")]
        [ProducesResponseType(200, Type = typeof(IServiceResult<GetUserGroupsResponse>))]
        public async Task<IActionResult> GetUserGroups([FromQuery]GetUserGroupsRequest request, CancellationToken cancellationToken)
        {
            var dtoResult = await _mediator.Send(new GetUserGroupsQuery() { Request = request.ToInput() }, cancellationToken);
            if (dtoResult.Succeeded)
                return Ok(Result<GetUserGroupsResponse>.Success(dtoResult.Data?.ToResponse()));
            return Ok(Result<GetUserGroupsResponse>.Fail(dtoResult.Messages));
        }

        [HttpGet("getUser")]
        [ProducesResponseType(200, Type = typeof(IServiceResult<GetUserResponse>))]
        public async Task<IActionResult> GetUser([FromQuery] GetUserRequest request, CancellationToken cancellationToken)
        {
            var dtoResult = await _mediator.Send(new GetUserQuery() { Request = request.ToInput() }, cancellationToken);
            if (dtoResult.Succeeded)
                return Ok(Result<GetUserResponse>.Success(dtoResult.Data?.ToResponse()));
            return Ok(Result<GetUserResponse>.Fail(dtoResult.Messages));
        }

        [HttpPost("updateUserGroups")]
        [ProducesResponseType(200, Type = typeof(IServiceResult))]
        public async Task<IActionResult> UpdateUserGroups([FromBody] UpdateUserGroupsRequest request, CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var ret = await _mediator.Send(new UpdateUserGroupsCommand() { Request = request.ToInput() }, cancellationToken);

            return Ok(ret);
        }

        [HttpPost("searchAuditLogs")]
        [ProducesResponseType(200, Type = typeof(IServiceResult<SearchAuditLogsResponse>))]
        public async Task<IActionResult> SearchAuditLogs([FromBody] SearchAuditLogsRequest request, CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var dtoResult = await _mediator.Send(new SearchAuditLogsQuery() { Request = request.ToInput() }, cancellationToken);

            if (dtoResult.Succeeded)
                return Ok(Result<SearchAuditLogsResponse>.Success(dtoResult.Data?.ToResponse()));
            return Ok(Result<SearchAuditLogsResponse>.Fail(dtoResult.Messages));
        }

        [HttpPost("updateUserNacPolicy")]
        [ProducesResponseType(200, Type = typeof(IServiceResult))]
        public async Task<IActionResult> UpdateUserNacPolicy([FromBody] UpdateUserNacPolicyRequest request, CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var ret = await _mediator.Send(new UpdateUserNacPolicyCommand() { Request = request.ToInput() }, cancellationToken);

            return Ok(ret);
        }

        [HttpPost("updateTenantNacPolicy")]
        [ProducesResponseType(200, Type = typeof(IServiceResult))]
        public async Task<IActionResult> UpdateTenantNacPolicy([FromBody] UpdateTenantNacPolicyRequest request, CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var ret = await _mediator.Send(new UpdateTenantNacPolicyCommand() { Request = request.ToInput() }, cancellationToken);

            return Ok(ret);
        }
    }
}

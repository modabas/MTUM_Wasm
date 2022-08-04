using MTUM_Wasm.Server.Core.Identity.Mapping;
using MTUM_Wasm.Server.Core.Identity.Mediatr;
using MTUM_Wasm.Shared.Core.Common.Result;
using MTUM_Wasm.Shared.Core.Identity.Dto;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(429)]
    [ProducesResponseType(500)]
    public class IdentityController : ControllerBase
    {
        private readonly IMediator _mediator;

        public IdentityController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        [ProducesResponseType(200, Type = typeof(IServiceResult<LoginResponse>))]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody]LoginRequest request, CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var dtoResult = await _mediator.Send(new LoginCommand() { Request = request.ToInput() }, cancellationToken);
            if (dtoResult.Succeeded)
                return Ok(Result<LoginResponse>.Success(dtoResult.Data?.ToResponse()));
            return Ok(Result<LoginResponse>.Fail(dtoResult.Messages));
        }


        [HttpPost("refresh")]
        [ProducesResponseType(200, Type = typeof(IServiceResult<RefreshResponse>))]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request, CancellationToken cancellationToken)
        {
            var dtoResult = await _mediator.Send(new RefreshCommand() { Request = request.ToInput() }, cancellationToken);
            if (dtoResult.Succeeded)
                return Ok(Result<RefreshResponse>.Success(dtoResult.Data?.ToResponse()));
            return Ok(Result<RefreshResponse>.Fail(dtoResult.Messages));
        }

        [HttpGet("logout")]
        [ProducesResponseType(200, Type = typeof(IServiceResult))]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            var ret = await _mediator.Send(new LogoutCommand() { }, cancellationToken);

            return Ok(ret);
        }

        [HttpPost("changePassword")]
        [ProducesResponseType(200, Type = typeof(IServiceResult))]
        [AllowAnonymous]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var ret = await _mediator.Send(new ChangePasswordCommand() { Request = request.ToInput() }, cancellationToken);

            return Ok(ret);
        }

        [HttpPost("forgotPassword")]
        [ProducesResponseType(200, Type = typeof(IServiceResult))]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var ret = await _mediator.Send(new ForgotPasswordCommand() { Request = request.ToInput() }, cancellationToken);

            return Ok(ret);
        }

        [HttpPost("resetPassword")]
        [ProducesResponseType(200, Type = typeof(IServiceResult))]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var ret = await _mediator.Send(new ResetPasswordCommand() { Request = request.ToInput() }, cancellationToken);

            return Ok(ret);
        }
    }

}

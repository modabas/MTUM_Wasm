using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MTUM_Wasm.Server.Core.Common.Extension;
using MTUM_Wasm.Server.Core.Common.Utility;
using System.Net;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Web.Filters;

internal class SystemAccessControlMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SystemAccessControlMiddleware> _logger;
    private readonly IOptions<SystemAccessControlOptions> _systemAccessControlOptions;

    public SystemAccessControlMiddleware(RequestDelegate next,
        ILogger<SystemAccessControlMiddleware> logger,
        IOptions<SystemAccessControlOptions> systemAccessControlOptions)
    {
        _next = next;
        _logger = logger;
        _systemAccessControlOptions = systemAccessControlOptions;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var remoteIpAddressResult = httpContext.GetRequestIP(true);

        //if NacPolicy is null, no nac policy is defined. Allow access.
        //if NacPolicy validates, allow access.
        if (_systemAccessControlOptions.Value.NacPolicy?.Validate(remoteIpAddressResult).Succeeded ?? true)
            await _next(httpContext);
        else
        {
            if (remoteIpAddressResult.Succeeded && remoteIpAddressResult.Data is not null)
                _logger.LogWarning("System access control nac policy fail for remote ip: {remoteIp}. Prevented access.", remoteIpAddressResult.Data);
            else
                _logger.LogWarning("System access control nac policy fail. Prevented access.");
            httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            await httpContext.Response.StartAsync(httpContext.RequestAborted);
        }
    }
}


using MTUM_Wasm.Server.Core.Common.Utility;
using MTUM_Wasm.Server.Core.Identity.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Web.Filters;

internal class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;
    private readonly RouteData _emptyRouteData = new();
    private readonly ActionDescriptor _emptyActionDescriptor = new ActionDescriptor();

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private Task WriteResultAsync<TResult>(HttpContext context, TResult result)
        where TResult : IActionResult
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var executor = context.RequestServices.GetRequiredService<IActionResultExecutor<TResult>>();

        var routeData = context.GetRouteData() ?? _emptyRouteData;

        var actionContext = new ActionContext(context, routeData, _emptyActionDescriptor);

        return executor.ExecuteAsync(actionContext, result);        
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var problemDetailsFactory = context.RequestServices.GetRequiredService<ProblemDetailsFactory>();

        if (ex is UserNotAuthenticated)
        {
            var problem = problemDetailsFactory.CreateProblemDetails(context, statusCode: 401, title: ex.Message);
            await WriteResultAsync(context, new ObjectResult(problem)
            {
                StatusCode = problem.Status,
            });
            
        }
        else if (ex is ThrottlingException)
        {
            var problem = problemDetailsFactory.CreateProblemDetails(context, statusCode: 429, title: ex.Message);
            await WriteResultAsync(context, new ObjectResult(problem)
            {
                StatusCode = problem.Status,
            });
        }
        else
        {
            var problem = problemDetailsFactory.CreateProblemDetails(context);
            await WriteResultAsync(context, new ObjectResult(problem)
            {
                StatusCode = problem.Status,
            });
        }
        var requestId = Activity.Current?.Id ?? context.TraceIdentifier;
        _logger.LogError(ex, "{requestId}", requestId);
    }
}

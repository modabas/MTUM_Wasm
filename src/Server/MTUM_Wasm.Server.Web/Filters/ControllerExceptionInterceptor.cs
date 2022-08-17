using MTUM_Wasm.Server.Core.Common.Utility;
using MTUM_Wasm.Server.Core.Identity.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using FluentValidation;
using System.Linq;

namespace MTUM_Wasm.Server.Web.Filters;

internal class ControllerExceptionInterceptor : IAsyncExceptionFilter
{
    private readonly ILogger<ControllerExceptionInterceptor> _logger;
    public ControllerExceptionInterceptor(ILogger<ControllerExceptionInterceptor> logger)
    {
        _logger = logger;
    }

    public Task OnExceptionAsync(ExceptionContext context)
    {
        if (!context.ExceptionHandled)
        {
            var ex = context.Exception;
            var problemDetailsFactory = context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();

            if (ex is ArgumentException || ex is ArgumentNullException)
            {
                var modelState = context.ModelState;
                if (modelState.IsValid)
                    modelState.AddModelError("$", ex.Message);
                var validationProblem = problemDetailsFactory.CreateValidationProblemDetails(context.HttpContext, modelState);
                context.Result = new BadRequestObjectResult(validationProblem);
            }
            else if (ex is ValidationException vex)
            {
                var modelState = context.ModelState;
                if (modelState.IsValid)
                {
                    vex.Errors.Select(e => new { Key = e.PropertyName, ErrorMessage = e.ErrorMessage }).ToList().ForEach(e => modelState.AddModelError(e.Key, e.ErrorMessage));
                }
                var validationProblem = problemDetailsFactory.CreateValidationProblemDetails(context.HttpContext, modelState);
                context.Result = new BadRequestObjectResult(validationProblem);
            }
            else
            {
                return Task.CompletedTask;
            }
            _logger.LogError(ex, "{actionDescriptor}", context.ActionDescriptor.DisplayName);
            context.ExceptionHandled = true;
        }
        return Task.CompletedTask;
    }
}

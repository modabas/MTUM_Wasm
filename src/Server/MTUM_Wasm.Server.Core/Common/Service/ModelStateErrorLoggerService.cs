using MTUM_Wasm.Server.Core.Common.Extension;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTUM_Wasm.Server.Core.Common.Service;

internal class ModelStateErrorLoggerService : IModelStateErrorLoggerService
{
    private readonly ILogger<ModelStateErrorLoggerService> _logger;

    public ModelStateErrorLoggerService(ILogger<ModelStateErrorLoggerService> logger)
    {
        _logger = logger;
    }

    public void LogError(ActionContext actionContext)
    {
        var errors = actionContext.ModelState.CreateErrorDictionary();
        var argException = new ArgumentException(string.Join(Environment.NewLine, errors.SelectMany(x => x.Value.Select(v => $"{x.Key}:{v}"))));
        _logger.LogError(argException, "{actionDescriptor}", actionContext.ActionDescriptor.DisplayName);
    }
}

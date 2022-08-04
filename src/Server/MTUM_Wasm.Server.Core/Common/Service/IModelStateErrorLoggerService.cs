using Microsoft.AspNetCore.Mvc;

namespace MTUM_Wasm.Server.Core.Common.Service;

internal interface IModelStateErrorLoggerService
{
    void LogError(ActionContext actionContext);
}

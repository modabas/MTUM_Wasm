using MTUM_Wasm.Shared.Core.Common.Result;
using Microsoft.AspNetCore.Http;

namespace MTUM_Wasm.Server.Core.Common.Extension;

internal static class HttpContextAccessorExtensions
{
    public static IServiceResult<string> GetRequestIP(this IHttpContextAccessor httpContextAccessor, bool tryUseXForwardHeader = true)
    {
        if (httpContextAccessor.HttpContext is null)
            return Result<string>.Fail("Unable to determine caller's IP.");
        return httpContextAccessor.HttpContext.GetRequestIP(tryUseXForwardHeader);
    }
}
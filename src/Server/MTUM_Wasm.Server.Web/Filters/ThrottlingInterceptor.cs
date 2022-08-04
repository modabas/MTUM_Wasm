using MTUM_Wasm.Server.Core.Common.Extension;
using MTUM_Wasm.Server.Core.Common.Orleans;
using MTUM_Wasm.Server.Core.Identity.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Orleans;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Web.Filters;

internal class ThrottlingInterceptor : IAsyncActionFilter
{
    private readonly IGrainFactory _grainFactory;
    private readonly IHttpContextAccessor _httpContextAcessor;
    private readonly IUserAccessor _userAccessor;

    public ThrottlingInterceptor(IGrainFactory grainFactory, IHttpContextAccessor httpContextAcessor, IUserAccessor userAccessor)
    {
        _grainFactory = grainFactory;
        _httpContextAcessor = httpContextAcessor;
        _userAccessor = userAccessor;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var userIdentifier = await GetUserIdentifier();
        await _grainFactory.GetGrain<IThrottlingGrain>(userIdentifier).Poke();

        await next();
    }

    private async Task<string> GetUserIdentifier()
    {
        var userIdentifier = "anonymous";
        if (_userAccessor.User?.Identity?.IsAuthenticated ?? false)
        {
            userIdentifier = (await _userAccessor.GetCurrentUser(_httpContextAcessor.HttpContext?.RequestAborted ?? default)).EmailAddress;
        }
        else
        {
            var clientIpResult = _httpContextAcessor.GetRequestIP();
            if (clientIpResult.Succeeded && clientIpResult.Data is not null)
            {
                userIdentifier = clientIpResult.Data;
            }
        }
        return userIdentifier;
    }
}

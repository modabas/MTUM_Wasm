using MTUM_Wasm.Client.Core.Identity.Service;
using MTUM_Wasm.Shared.Core.Identity.Service;
using MTUM_Wasm.Shared.Infrastructure.Identity.AwsCognito;
using Microsoft.Extensions.DependencyInjection;

namespace MTUM_Wasm.Client.Infrastructure.Identity.AwsCognito.Utility;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAwsCognitoIdentityFlow(this IServiceCollection services)
    {
        services.AddScoped<ITokenClaimResolver, AwsCognitoTokenClaimResolver>();
        services.AddScoped<IIdentityService, IdentityService>();
        
        return services;
    }
}

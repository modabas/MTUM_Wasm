using MTUM_Wasm.Client.Core.Identity.Http;
using MTUM_Wasm.Client.Core.Identity.Service;
using MTUM_Wasm.Client.Core.SystemAdmin.Service;
using MTUM_Wasm.Client.Core.TenantAdmin.Service;
using MTUM_Wasm.Client.Core.TenantViewerOrUp.Service;
using MTUM_Wasm.Client.Core.Utility;
using MTUM_Wasm.Client.Core.Utility.MessageDisplay;
using MTUM_Wasm.Client.Core.Utility.Preferences;
using MTUM_Wasm.Client.Core.Utility.Spinner;
using MTUM_Wasm.Client.Infrastructure.Identity.AwsCognito.Utility;
using MTUM_Wasm.Client.Web;
using MTUM_Wasm.Shared.Core.Common.Authorization;
using MTUM_Wasm.Shared.Core.Common.Extension;
using MTUM_Wasm.Shared.Core.Identity.Policy;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using System;
using System.Net.Http;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//Add authorization policies and requirement handlers
builder.Services.AddSingleton<IAuthorizationHandler, TenantClaimHandler>();
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy(Policy.Name.IsTenantAdmin, builder => builder.AddIsTenantAdminPolicy());
    options.AddPolicy(Policy.Name.IsTenantUserOrUp, builder => builder.AddIsTenantUserOrUpPolicy());
    options.AddPolicy(Policy.Name.IsTenantViewerOrUp, builder => builder.AddIsTenantViewerOrUpPolicy());
    options.AddPolicy(Policy.Name.IsSystemAdmin, builder => builder.AddIsSystemAdminPolicy());
});

//add services
builder.Services.AddAwsCognitoIdentityFlow();
builder.Services.AddScoped<ITokenStore, LocalStorageTokenStore>();
builder.Services.AddScoped<RefreshingAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<RefreshingAuthenticationStateProvider>());
builder.Services.AddScoped<IMessageDisplayService, MessageDisplayService>();
builder.Services.AddScoped<ITenantViewerOrUpService, TenantViewerOrUpService>();
builder.Services.AddScoped<ITenantAdminService, TenantAdminService>();
builder.Services.AddScoped<ISystemAdminService, SystemAdminService>();
builder.Services.AddScoped<ISpinnerService, SpinnerService>();

//add http client along with custom message handler and interceptor
builder.Services
                .AddTransient<AuthenticationHeaderHandler>()
                .AddScoped(sp => sp
                    .GetRequiredService<IHttpClientFactory>()
                    .CreateClient(Definitions.HttpClientNameForServerApi).EnableIntercept(sp))
                .AddHttpClient(Definitions.HttpClientNameForServerApi, client =>
                {
                    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
                })
                .AddHttpMessageHandler<AuthenticationHeaderHandler>();
builder.Services.AddHttpClientInterceptor();
builder.Services.AddScoped<IHttpInterceptorService, HttpInterceptorService>();

//add local storage and client preferences
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<IPreferencesService, PreferencesService>();

//add MudBlazor
builder.Services.AddMudServices(conf =>
{
    conf.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.BottomRight;
    conf.SnackbarConfiguration.PreventDuplicates = false;
    conf.SnackbarConfiguration.HideTransitionDuration = 100;
    conf.SnackbarConfiguration.ShowTransitionDuration = 100;
    conf.SnackbarConfiguration.VisibleStateDuration = 7000;
    conf.SnackbarConfiguration.ShowCloseIcon = true;
});

//builder.Logging.ClearProviders();
//builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
//builder.Logging.AddConsole();

await builder.Build().RunAsync();

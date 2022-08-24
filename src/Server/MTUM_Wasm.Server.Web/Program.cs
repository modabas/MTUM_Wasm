using MTUM_Wasm.Server.Core.Common.Orleans;
using MTUM_Wasm.Server.Core.Common.Policy.Nac;
using MTUM_Wasm.Server.Core.Common.Policy.TenantEnabled;
using MTUM_Wasm.Server.Core.Common.Service;
using MTUM_Wasm.Server.Core.Common.Utility;
using MTUM_Wasm.Server.Core.Database.Entity;
using MTUM_Wasm.Server.Core.Database.Service;
using MTUM_Wasm.Server.Core.Identity.Service;
using MTUM_Wasm.Server.Core.SystemAdmin.Service;
using MTUM_Wasm.Server.Core.TenantAdmin.Service;
using MTUM_Wasm.Server.Infrastructure.Database.Postgres.Service;
using MTUM_Wasm.Server.Infrastructure.Identity.AwsCognito.Utility;
using MTUM_Wasm.Server.Web.Filters;
using MTUM_Wasm.Shared.Core.Common.Authorization;
using MTUM_Wasm.Shared.Core.Common.Extension;
using MTUM_Wasm.Shared.Core.Identity.Policy;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Db Services
builder.Services.Configure<ServiceDbOptions>(builder.Configuration.GetSection("ServiceDbOptions"));
builder.Services.AddSingleton<IDbContext, DbContext>();
builder.Services.AddSingleton<ITenantRepo, TenantRepo>();
builder.Services.AddSingleton<IAuditRepo, AuditRepo>();

//Mediatr
builder.Services.AddMediatR(typeof(MTUM_Wasm.Server.Core.Identity.Mediatr.LoginCommand).Assembly);

//add
//exception encapsulator to catch all unhandled error from api controllers
//throttling interceptor to throttle incoming requests from a client
//model state error logger service to automate bad request logging
builder.Services.AddSingleton<IModelStateErrorLoggerService, ModelStateErrorLoggerService>();
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<ControllerExceptionInterceptor>();
    options.Filters.Add<ThrottlingInterceptor>();
})
//...and automatic bad request logging
.ConfigureApiBehaviorOptions(options =>
{
    // To preserve the default behavior, capture the original delegate to call later.
    var builtInFactory = options.InvalidModelStateResponseFactory;

    options.InvalidModelStateResponseFactory = context =>
    {
        var loggerService = context.HttpContext.RequestServices.GetRequiredService<IModelStateErrorLoggerService>();

        // Perform logging here.
        // ...
        loggerService.LogError(context);

        // Invoke the default behavior, which produces a ValidationProblemDetails response.
        // To produce a custom response, return a different implementation of IActionResult instead.
        return builtInFactory(context);
    };
});
//...and fluent validation
builder.Services.AddValidatorsFromAssemblyContaining<MTUM_Wasm.Shared.Core.Identity.Validation.LoginRequestValidator>();

//add razor pages 
builder.Services.AddRazorPages();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference()
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
    options.CustomSchemaIds(type => type.ToString());
});

builder.Services.AddAwsCognitoAuthentication(builder.Configuration);

builder.Services.AddSingleton<ISystemAdminService, SystemAdminService>();

builder.Services.AddSingleton<ITenantAdminService, TenantAdminService>();

builder.Services.AddSingleton<IAuthorizationHandler, UserNacRequirementHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, HasEnabledTenantRequirementHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, TenantNacRequirementHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, TenantClaimHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policy.Name.UserNac, builder =>
    {
        builder.AddRequirements(new UserNacRequirement());
    });
    options.AddPolicy(Policy.Name.HasEnabledTenant, builder =>
    {
        builder.AddRequirements(new HasEnabledTenantRequirement());
    });
    options.AddPolicy(Policy.Name.TenantNac, builder =>
    {
        builder.AddRequirements(new TenantNacRequirement());
    });

    options.AddPolicy(Policy.Name.IsTenantAdmin, builder => builder.AddIsTenantAdminPolicy());
    options.AddPolicy(Policy.Name.IsTenantUserOrUp, builder => builder.AddIsTenantUserOrUpPolicy());
    options.AddPolicy(Policy.Name.IsTenantViewerOrUp, builder => builder.AddIsTenantViewerOrUpPolicy());
    options.AddPolicy(Policy.Name.IsSystemAdmin, builder => builder.AddIsSystemAdminPolicy());
});

//inject SystemAccessControlOptions from configuration
builder.Services.Configure<SystemAccessControlOptions>(builder.Configuration.GetSection("SystemAccessControlOptions"));

builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<IUserAccessor, UserAccessor>();

builder.Host.UseOrleans(siloBuilder =>
{
    var orleansMgmtDbConnStr = builder.Configuration.GetConnectionString("ManagementDbConnStr");
    var adoNetInvariant = "Npgsql";
    siloBuilder
    .Configure<ClusterOptions>(options =>
    {
        //this is a unique ID for the Orleans cluster. All clients and silos that use this ID will be able to talk directly to each other. You can choose to use a different ClusterId for different deployments, though.
        //for heteregenous cluster (cluster that constains silos that implement different kinds of grains), orleans will handle the communication with correct silos that support that kind of grain
        options.ClusterId = Definitions.SiloClusterId;
        //this is a unique ID for your application that will be used by some providers, such as persistence providers.This ID should remain stable and not change across deployments.
        options.ServiceId = Definitions.SiloServiceId;
    });

    siloBuilder
    //.UseLocalhostClustering()
    //.AddMemoryGrainStorage(Definitions.StateStorageName)
    .UseAdoNetClustering(options =>
    {
        options.Invariant = adoNetInvariant;
        options.ConnectionString = orleansMgmtDbConnStr;
    })
    .AddAdoNetGrainStorage(Definitions.StateStorageName, options =>
    {
        options.Invariant = adoNetInvariant;
        options.ConnectionString = orleansMgmtDbConnStr;
        options.UseJsonFormat = true;
    })
    .UseAdoNetReminderService(options =>
    {
        options.Invariant = adoNetInvariant;
        options.ConnectionString = orleansMgmtDbConnStr;
    })
    //Configure Orleans memory stream
    //.AddMemoryStreams<DefaultMemoryMessageBodySerializer>(Definitions.MemoryStreamProviderName)
    //.AddAdoNetGrainStorage(Definitions.PubSubStorageName, options =>
    //{
    //    options.Invariant = adoNetInvariant;
    //    options.ConnectionString = orleansMgmtDbConnStr;
    //    options.UseJsonFormat = true;
    //})
    .AddStartupTask<HeartbeatStartupTask>()

    //Disable orleans client connections by setting gateway port to 0.
    //Orleans server runs in same generic host with web api and doesn't need orleans client
    .ConfigureEndpoints(siloPort: 11111, gatewayPort: 0);
});

builder.Logging.ClearProviders();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddConsole();

//response compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});


if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddHsts(options => options.IncludeSubDomains = true);
    //Production environment uses load balancer/port mapping so local service's https port is not what is seen from outside
    builder.Services.AddHttpsRedirection(options =>
    {
        options.RedirectStatusCode = StatusCodes.Status301MovedPermanently;
        options.HttpsPort = 443;
    });
}

builder.WebHost.UseKestrel(so =>
{
    so.Limits.MaxRequestBodySize = 8 * 1024;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMiddleware<ExceptionHandlerMiddleware>();
    app.UseMiddleware<SystemAccessControlMiddleware>();
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    //use exception handler cannot handle errors from UseAuthentication
    //use middleware instead
    app.UseMiddleware<ExceptionHandlerMiddleware>();
    app.UseMiddleware<SystemAccessControlMiddleware>();
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();

app.UseResponseCompression();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    //Set vary headers so client is hinted on response may have different content based on these request header values
    //context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] =
    //    new string[] { "Accept-Encoding, User-Agent" };

    //set security header to prevent mime sniffing and mime based attacks to client
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    //set security header so the site cannot be put in a frame no matter who it is (Including the site framing itself). Prevent clickjacking
    context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.XFrameOptions] = "DENY";

    //set cache control headers
    context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.CacheControl] = "no-cache";
    context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Expires] = DateTime.UnixEpoch.AddSeconds(1).ToString("R");
    context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Pragma] = "no-cache";

    await next();
});

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();

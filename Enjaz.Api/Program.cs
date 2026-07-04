using Enjaz.Api.Realtime;
using Enjaz.AI.Endpoints;
using Enjaz.BuildingBlocks.Auth;
using Enjaz.BuildingBlocks.Exceptions;
using Enjaz.Catalog.Endpoints;
using Enjaz.Customers.Endpoints;
using Enjaz.Identity.Endpoints;
using Enjaz.Jobs.Endpoints;
using Enjaz.Jobs.Endpoints.Realtime;
using Enjaz.Maps.Endpoints;
using Enjaz.Maps.Endpoints.Realtime;
using Enjaz.Pricing.Endpoints;
using Enjaz.Technicians.Endpoints;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.OpenApi;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/enjaz-api-.log", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Enjaz API.");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, loggerConfiguration) =>
    {
        loggerConfiguration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("logs/enjaz-api-.log", rollingInterval: RollingInterval.Day);
    });

    var postgreSqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

    var redisConnectionString = builder.Configuration.GetConnectionString("Redis")
        ?? throw new InvalidOperationException("Connection string 'Redis' is not configured.");

    builder.Services.AddControllers();
    builder.Services.AddOpenApi();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Enjaz API",
            Version = "v1",
            Description = "Backend API for Enjaz maintenance marketplace"
        });

        var securityScheme = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Enter a JWT Bearer token.",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        };

        options.AddSecurityDefinition("Bearer", securityScheme);

        options.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", null, null)] = []
        });
    });

    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddJwtAuthentication(builder.Configuration);
    builder.Services.AddAuthorization();

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnectionString;
    });

    builder.Services
        .AddHealthChecks()
        .AddNpgSql(postgreSqlConnectionString, name: "postgresql")
        .AddRedis(redisConnectionString, name: "redis");

    builder.Services.AddHangfire(configuration =>
    {
        configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(options =>
                options.UseNpgsqlConnection(postgreSqlConnectionString, _ => { }));
    });

    if (builder.Environment.IsDevelopment())
    {
        builder.Services.AddHangfireServer();
    }

    builder.Services.AddSignalR();
    builder.Services.AddIdentityModule(builder.Configuration);
    builder.Services.AddCustomersModule(builder.Configuration);
    builder.Services.AddCatalogModule(builder.Configuration);
    builder.Services.AddTechniciansModule(builder.Configuration);
    builder.Services.AddMapsModule(builder.Configuration);
    builder.Services.AddAiModule(builder.Configuration);
    builder.Services.AddPricingModule(builder.Configuration);
    builder.Services.AddJobsModule(builder.Configuration);

    var app = builder.Build();

    app.UseGlobalExceptionHandling();
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHangfireDashboard("/hangfire");
        Enjaz.Jobs.Endpoints.DependencyInjection.AddJobsRecurringJobs();
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapHealthChecks("/health");
    app.MapHub<SystemHub>("/hubs/system");
    app.MapHub<TrackingHub>("/hubs/tracking");
    app.MapHub<JobsHub>("/hubs/jobs");

    app.Run();
}
catch (Microsoft.Extensions.Hosting.HostAbortedException)
{
    // EF Core design-time discovery aborts the host after services are resolved.
}
catch (Exception exception)
{
    Log.Fatal(exception, "Enjaz API terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}

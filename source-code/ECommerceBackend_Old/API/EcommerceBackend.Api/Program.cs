using EcommerceBackend.Api.Extensions;
using EcommerceBackend.Api.Middleware;
using ECommerceBackend.Common.Application;
using ECommerceBackend.Common.Infrastructure;
using ECommerceBackend.Common.Presentation.Endpoints;
using ECommerceBackend.Modules.Users.Infrastructure;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Config Serilog
builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));     // this use config from appsettings.json


// OpenAPI and Swagger configuration
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

// Add Global Exception Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Add Modules Applications
builder.Services.AddApplication([
    ECommerceBackend.Modules.Users.Application.AssemblyReference.Assembly
]);


// Add Modules Infrastructure
string databaseConnectionString = builder.Configuration.GetConnectionString("Database")!; // PostgreSQL
string redisConnectionString = builder.Configuration.GetConnectionString("Cache")!; // Redis

builder.Services.AddInfrastructure(
    databaseConnectionString,
    redisConnectionString);

builder.Configuration.AddModuleConfiguration([
    "users"
    ]);

// Add modules
builder.Services.AddUsersModule(builder.Configuration);

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(databaseConnectionString)
    .AddRedis(redisConnectionString)
    .AddUrlGroup(new Uri(builder.Configuration.GetValue<string>("KeyCloak:HealthUrl")!), HttpMethod.Get, "keycloak");


// APP BUILD //
WebApplication app = builder.Build();

app.MapGet("/", () => "Welcome to the Ecommerce Backend API!");

app.MapEndpoints();

// Add Health Check endpoint
app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });

    app.ApplyMigrations();
}

app.UseExceptionHandler();

// Use logging
app.UseSerilogRequestLogging();

// Use Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

await app.RunAsync();

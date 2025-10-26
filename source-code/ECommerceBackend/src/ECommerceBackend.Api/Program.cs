using ECommerceBackend.Api.Extensions;
using ECommerceBackend.Api.Filters;
using ECommerceBackend.Api.Middlewares;
using ECommerceBackend.Application;
using ECommerceBackend.Infrastructure;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<TraceIdFilter>(); // Auto inject TraceId
})
.AddJsonOptions(options =>
{
    // Configure JSON serialization to use string for enums
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});


// Add Cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });

    options.AddPolicy("AllowFrontendApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});


// Serilog
builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "ECommerce Backend API",
        Version = "v1",
        Description = "API for ECommerce Backend with Product Management"
    });

    // Support for multipart/form-data (file uploads)
    options.OperationFilter<SwaggerFileOperationFilter>();
});

// Add global exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Add application and infrastructure services, dependency injection
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add health checks
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!)
    .AddRedis(builder.Configuration.GetConnectionString("Cache")!);


// =========== Build and configure the app ===========
WebApplication app = builder.Build();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});


// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ECommerce API v1");
    options.RoutePrefix = "swagger"; // Swagger UI tại /swagger
    options.DocumentTitle = "ECommerce Backend API";
});

if (app.Environment.IsDevelopment())
{
    app.ApplyMigrations();
}

app.MapGet("/", () => "Hello from Ecommerce backend API!!");

// Enable static files for uploaded media
app.UseStaticFiles();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();


app.UseCors("AllowFrontendApp");

// Use Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

await app.RunAsync();

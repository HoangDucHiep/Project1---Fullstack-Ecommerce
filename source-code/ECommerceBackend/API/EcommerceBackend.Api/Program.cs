using EcommerceBackend.Api.Extensions;
using ECommerceBackend.Common.Application;
using ECommerceBackend.Common.Infrastructure;
using ECommerceBackend.Common.Presentation.Endpoints;
using ECommerceBackend.Modules.Users.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// OpenAPI and Swagger configuration
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();


// Add Modules Applications
builder.Services.AddApplication([
    ECommerceBackend.Modules.Users.Application.AssemblyReference.Assembly
]);


// Add Modules Infrastructure
string databaseConnectionString = builder.Configuration.GetConnectionString("Database")!;
string redisConnectionString = builder.Configuration.GetConnectionString("Cache")!;

builder.Services.AddInfrastructure(
    databaseConnectionString,
    redisConnectionString);

builder.Configuration.AddModuleConfiguration([
    "users"
    ]);


// Add modules
builder.Services.AddUsersModule(builder.Configuration);

WebApplication app = builder.Build();



app.MapGet("/", () => "Welcome to the Ecommerce Backend API!");

app.MapEndpoints();

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

await app.RunAsync();

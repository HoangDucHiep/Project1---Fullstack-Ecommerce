WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// OpenAPI and Swagger
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });
}

app.MapGet("/", () => "Hello from Ecommerce backend API!!");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

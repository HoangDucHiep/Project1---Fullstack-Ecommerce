WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// OpenAPI and Swagger configuration
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

app.MapGet("/", () => "Welcome to the Ecommerce Backend API!");

app.UseHttpsRedirection();

await app.RunAsync();

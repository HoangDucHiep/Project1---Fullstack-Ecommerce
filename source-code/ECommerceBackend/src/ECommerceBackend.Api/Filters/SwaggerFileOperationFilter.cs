#pragma warning disable IDE0008 // Use explicit type
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ECommerceBackend.Api.Filters;

/// <summary>
/// Swagger operation filter to properly handle file upload endpoints
/// </summary>
public class SwaggerFileOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var fileParameters = context.MethodInfo.GetParameters()
            .Where(p => p.ParameterType == typeof(IFormFile) || 
                       p.ParameterType == typeof(IEnumerable<IFormFile>) ||
                       p.ParameterType == typeof(List<IFormFile>))
            .ToList();

        if (!fileParameters.Any())
        {
            return;
        }

        operation.RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties = new Dictionary<string, OpenApiSchema>(),
                        Required = new HashSet<string>()
                    }
                }
            }
        };

        var schema = operation.RequestBody.Content["multipart/form-data"].Schema;

        foreach (var parameter in context.MethodInfo.GetParameters())
        {
            if (parameter.ParameterType == typeof(IFormFile))
            {
                schema.Properties[parameter.Name ?? "file"] = new OpenApiSchema
                {
                    Type = "string",
                    Format = "binary"
                };
                schema.Required.Add(parameter.Name ?? "file");
            }
            else if (parameter.ParameterType == typeof(List<IFormFile>) || 
                     parameter.ParameterType == typeof(IEnumerable<IFormFile>))
            {
                schema.Properties[parameter.Name ?? "files"] = new OpenApiSchema
                {
                    Type = "array",
                    Items = new OpenApiSchema
                    {
                        Type = "string",
                        Format = "binary"
                    }
                };
                schema.Required.Add(parameter.Name ?? "files");
            }
            else if (parameter.ParameterType == typeof(string))
            {
                schema.Properties[parameter.Name ?? "unknown"] = new OpenApiSchema
                {
                    Type = "string"
                };
            }
        }
    }
}


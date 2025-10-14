using System.Reflection;
using ECommerceBackend.Api.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog.Context;

namespace ECommerceBackend.Api.Filters;

/// <summary>
/// Action filter that injects TraceId into responses and enriches logs
/// </summary>
public class TraceIdFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Enrich Serilog logs with RequestId
        LogContext.PushProperty("RequestId", context.HttpContext.TraceIdentifier);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is ObjectResult objectResult && objectResult.Value != null)
        {
            string traceId = context.HttpContext.TraceIdentifier;

            switch (objectResult.Value)
            {
                case ApiErrorResponse errorResponse:
                    errorResponse.TraceId = traceId;
                    break;
                default:
                    PropertyInfo? prop = objectResult.Value.GetType().GetProperty("TraceId");
                    prop?.SetValue(objectResult.Value, traceId);
                    break;
            }
        }
    }
}

using ECommerceBackend.Domain.Abstracts;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace ECommerceBackend.Application.Abstracts.Behaviors;

/// HDHiep - 09/24/2025
/// <summary>
/// A MediatR pipeline behavior that logs the start and completion of each request.
/// It logs the request type and any errors that occur during processing.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
internal sealed class RequestLoggingPipelineBehavior<TRequest, TResponse>(
    ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger
    ) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    where TResponse : Result
{

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        string moduleName = GetModuleName(typeof(TRequest).FullName!);
        string requestName = typeof(TRequest).Name;

        using (LogContext.PushProperty("Module", moduleName))
        {
            logger.LogInformation("Processing request {RequestName}", requestName);

            TResponse result = await next(cancellationToken);

            if (result.IsSuccess)
            {
                logger.LogInformation("Completed request {RequestName}", requestName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    logger.LogError("Completed request {RequestName} with error", requestName);
                }
            }

            return result;
        }
    }

    private static string GetModuleName(string requestName) => requestName.Split('.')[2];
}

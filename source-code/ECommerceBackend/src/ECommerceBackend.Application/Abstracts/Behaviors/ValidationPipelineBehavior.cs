using System.Reflection;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Domain.Abstracts;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace ECommerceBackend.Application.Abstracts.Behaviors;


/// HDHiep - 09/24/2025
/// <summary>
/// A MediatR pipeline behavior that validates requests before they are handled.
/// </summary>
/// <typeparam name="TRequest">The request type implementing <see cref="IBaseCommand"/>.</typeparam>
/// <typeparam name="TResponse">The response type, usually a <see cref="Result"/> or <see cref="Result{T}"/>.</typeparam>
internal sealed class ValidationPipelineBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
{
    /// <summary>
    /// Handles the request by running validation before calling the next handler in the pipeline.
    /// </summary>
    /// <param name="request">The request being processed.</param>
    /// <param name="next">The delegate to invoke the next behavior or handler.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The response from the next handler if validation succeeds,
    /// or a failure <see cref="Result"/>/<see cref="Result{T}"/> if validation fails.
    /// </returns>
    /// <exception cref="ValidationException">
    /// Thrown if validation fails and the response type is not compatible with <see cref="Result"/>.
    /// </exception>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ValidationFailure[] validationFailures = await ValidateAsync(request);

        if (validationFailures.Length == 0)
        {
            return await next(cancellationToken);
        }

        if (typeof(TResponse).IsGenericType &&
            typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            Type resultType = typeof(TResponse).GetGenericArguments()[0];

            MethodInfo? failureMethod = typeof(Result<>)
                .MakeGenericType(resultType)
                .GetMethod(nameof(Result<object>.ValidationFailure));

            if (failureMethod is not null)
            {
                return (TResponse)failureMethod.Invoke(null, [CreateValidationError(validationFailures)]);
            }
        }
        else if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(CreateValidationError(validationFailures));
        }

        throw new ValidationException(validationFailures);
    }

    /// <summary>
    /// Validates the request using all configured validators.
    /// </summary>
    /// <param name="request">The request instance to validate.</param>
    /// <returns>
    /// An array of <see cref="ValidationFailure"/> objects, or an empty array if validation passes.
    /// </returns>
    private async Task<ValidationFailure[]> ValidateAsync(TRequest request)
    {
        if (!validators.Any())
        {
            return [];
        }

        var context = new ValidationContext<TRequest>(request);

        ValidationResult[] validationResults = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(context)));

        ValidationFailure[] validationFailures = validationResults
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)
            .ToArray();

        return validationFailures;
    }

    /// <summary>
    /// Creates a <see cref="ValidationError"/> object from a collection of <see cref="ValidationFailure"/>s.
    /// </summary>
    /// <param name="validationFailures">The validation failures to convert.</param>
    /// <returns>A <see cref="ValidationError"/> representing the validation issues.</returns>
    private static ValidationError CreateValidationError(ValidationFailure[] validationFailures) =>
        new(validationFailures.Select(f => Error.Problem(f.ErrorCode, f.ErrorMessage)).ToArray());
}

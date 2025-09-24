using ECommerceBackend.Common.Domain;

namespace ECommerceBackend.Common.Presentation.Results;
/// <summary>
/// Provides helper extension methods to pattern-match over <see cref="Result"/> and <see cref="Result{TValue}"/>.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Executes <paramref name="onSuccess"/> when <paramref name="result"/> is successful;
    /// otherwise executes <paramref name="onFailure"/>.
    /// </summary>
    /// <typeparam name="TOut">The return type of both match branches.</typeparam>
    /// <param name="result">The result to match on.</param>
    /// <param name="onSuccess">Function executed when the result is successful.</param>
    /// <param name="onFailure">Function executed when the result is a failure.</param>
    /// <returns>The value produced by the executed branch.</returns>
    public static TOut Match<TOut>(
        this Result result,
        Func<TOut> onSuccess,
        Func<Result, TOut> onFailure)
    {
        return result.IsSuccess ? onSuccess() : onFailure(result);
    }

    /// <summary>
    /// Executes <paramref name="onSuccess"/> with the contained value when <paramref name="result"/> is successful;
    /// otherwise executes <paramref name="onFailure"/>.
    /// </summary>
    /// <typeparam name="TIn">The wrapped value type.</typeparam>
    /// <typeparam name="TOut">The return type of both match branches.</typeparam>
    /// <param name="result">The result to match on.</param>
    /// <param name="onSuccess">Function executed when the result is successful, receiving the wrapped value.</param>
    /// <param name="onFailure">Function executed when the result is a failure.</param>
    /// <returns>The value produced by the executed branch.</returns>
    public static TOut Match<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, TOut> onSuccess,
        Func<Result<TIn>, TOut> onFailure)
    {
        return result.IsSuccess ? onSuccess(result.Value) : onFailure(result);
    }
}

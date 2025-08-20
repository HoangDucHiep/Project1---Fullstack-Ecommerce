using System.Diagnostics.CodeAnalysis;

namespace ECommerceBackend.Common.Domain;

/// <summary>
/// Represents the result of an operation, containing information about
/// whether it succeeded or failed, and an error if applicable.
/// </summary>
public class Result
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the error associated with this result, or <see cref="Error.None"/>
    /// if the operation was successful.
    /// </summary>
    public Error Error { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the operation was successful.</param>
    /// <param name="error">The error associated with the result.</param>
    /// <exception cref="ArgumentException">
    /// Thrown if the <paramref name="error"/> value is inconsistent with the <paramref name="isSuccess"/> flag.
    /// </exception>
    public Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static Result Success() => new(true, Error.None);

    /// <summary>
    /// Creates a failed result with the specified error.
    /// </summary>
    /// <param name="error">The error describing the failure.</param>
    public static Result Failure(Error error) => new(false, error);

    /// <summary>
    /// Creates a failed generic result with the specified error.
    /// </summary>
    /// <typeparam name="TValue">The type of the result value.</typeparam>
    /// <param name="error">The error describing the failure.</param>
    public static Result<TValue> Failure<TValue>(Error error) =>
        new(default, false, error);

    /// <summary>
    /// Creates a successful generic result with the specified value.
    /// </summary>
    /// <typeparam name="TValue">The type of the result value.</typeparam>
    /// <param name="value">The value to wrap in the result.</param>
    public static Result<TValue> Success<TValue>(TValue value) =>
        new(value, true, Error.None);
}

/// <summary>
/// Represents the result of an operation that produces a value of type <typeparamref name="TValue"/>.
/// </summary>
/// <typeparam name="TValue">The type of the result value.</typeparam>
public class Result<TValue> : Result
{
    private readonly TValue? _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue}"/> class.
    /// </summary>
    /// <param name="value">The value produced by the operation, if successful.</param>
    /// <param name="isSuccess">Indicates whether the operation was successful.</param>
    /// <param name="error">The error associated with the result, if any.</param>
    public Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the value of the result if the operation was successful.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the result represents a failure and the value cannot be accessed.
    /// </exception>
    [NotNull]
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result can't be accessed.");

    /// <summary>
    /// Implicitly converts a value of type <typeparamref name="TValue"/> into a successful <see cref="Result{TValue}"/>.
    /// If the value is null, a failed result is returned with <see cref="Error.NullValue"/>.
    /// </summary>
    /// <param name="value">The value to wrap in a result.</param>
    public static implicit operator Result<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);

    /// <summary>
    /// Creates a failed result that specifically represents a validation failure.
    /// </summary>
    /// <param name="error">The validation error.</param>
    public static Result<TValue> ValidationFailure(Error error) =>
        new(default, false, error);
}

namespace ECommerceBackend.Common.Domain;

/// <summary>
/// Represents a validation error that may contain multiple individual errors.
/// </summary>
public sealed record ValidationError : Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationError"/> class
    /// with a collection of underlying <see cref="Error"/> objects.
    /// </summary>
    /// <param name="errors">The collection of validation errors.</param>
    public ValidationError(Error[] errors)
        : base(
            "General.Validation",
            "One or more validation errors occurred",
            ErrorType.Validation)
    {
        Errors = errors;
    }

    /// <summary>
    /// Gets the collection of validation errors associated with this instance.
    /// </summary>
    public Error[] Errors { get; }

    /// <summary>
    /// Creates a <see cref="ValidationError"/> from a collection of <see cref="Result"/> objects,
    /// selecting only those that represent failures.
    /// </summary>
    /// <param name="results">The collection of results to extract errors from.</param>
    /// <returns>
    /// A <see cref="ValidationError"/> containing the errors of all failed results.
    /// </returns>
    public static ValidationError FromResults(IEnumerable<Result> results) =>
        new(results.Where(r => r.IsFailure).Select(r => r.Error).ToArray());
}

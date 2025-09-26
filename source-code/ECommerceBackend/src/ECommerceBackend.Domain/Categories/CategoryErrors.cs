using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Domain.Categories;

/// HDHiep - 09/27/2025
/// <summary>
/// Provides standardized error instances for category-related operations.
/// Each method returns an <see cref="Error"/> with a specific code and description
/// to represent common failure scenarios in category management.
/// </summary>
/// <remarks>
/// These errors can be used throughout the domain and application layers
/// to ensure consistent error handling and messaging.
/// </remarks>
internal static class CategoryErrors
{
    /// <summary>
    /// Creates an error indicating that a category with the specified ID was not found.
    /// </summary>
    /// <param name="categoryId">The ID of the category that was not found.</param>
    /// <returns>An <see cref="Error"/> with NotFound type and appropriate error message.</returns>
    public static Error NotFound(Guid categoryId) =>
        Error.NotFound("Category.NotFound", $"Category with ID '{categoryId}' was not found.");

    /// <summary>
    /// Creates an error indicating that a parent category with the specified ID was not found.
    /// </summary>
    /// <param name="parentId">The ID of the parent category that was not found.</param>
    /// <returns>An <see cref="Error"/> with NotFound type and appropriate error message.</returns>
    public static Error ParentNotFound(Guid parentId) =>
        Error.NotFound("Category.ParentNotFound", $"Parent category with ID '{parentId}' was not found.");

    /// <summary>
    /// Creates an error indicating that a category with the specified name already exists.
    /// </summary>
    /// <param name="name">The name of the category that already exists.</param>
    /// <param name="parentId">The ID of the parent category. If null, indicates a root category conflict.</param>
    /// <returns>An <see cref="Error"/> with Conflict type and appropriate error message.</returns>
    public static Error DuplicateName(string name, Guid? parentId = null) =>
        Error.Conflict("Category.DuplicateName",
            parentId.HasValue
                ? $"A category with name '{name}' already exists under the specified parent."
                : $"A root category with name '{name}' already exists.");

    /// <summary>
    /// Creates an error indicating that moving a category to the specified parent would create an invalid hierarchy.
    /// </summary>
    /// <param name="categoryId">The ID of the category being moved.</param>
    /// <param name="parentId">The ID of the target parent category.</param>
    /// <returns>An <see cref="Error"/> with BadRequest type and appropriate error message.</returns>
    public static Error InvalidParent(Guid categoryId, Guid parentId) =>
        Error.BadRequest("Category.InvalidParent",
            $"Category '{categoryId}' cannot be moved under '{parentId}' - would create circular reference or invalid hierarchy.");

    /// <summary>
    /// Creates an error indicating that a category cannot be set as its own parent.
    /// </summary>
    /// <param name="categoryId">The ID of the category attempting to be its own parent.</param>
    /// <returns>An <see cref="Error"/> with BadRequest type and appropriate error message.</returns>
    public static Error SelfParentReference(Guid categoryId) =>
        Error.BadRequest("Category.SelfParentReference",
            $"Category '{categoryId}' cannot be its own parent.");

    /// <summary>
    /// Creates an error indicating that a category cannot be deleted because it has child categories.
    /// </summary>
    /// <param name="categoryId">The ID of the category that has child categories.</param>
    /// <returns>An <see cref="Error"/> with BadRequest type and appropriate error message.</returns>
    public static Error HasChildren(Guid categoryId) =>
        Error.BadRequest("Category.HasChildren",
            $"Category '{categoryId}' cannot be deleted because it has child categories. Remove children first or use cascade delete.");

    /// <summary>
    /// Creates an error indicating that a category cannot be deleted because it contains products.
    /// </summary>
    /// <param name="categoryId">The ID of the category that contains products.</param>
    /// <returns>An <see cref="Error"/> with BadRequest type and appropriate error message.</returns>
    public static Error HasProducts(Guid categoryId) =>
        Error.BadRequest("Category.HasProducts",
            $"Category '{categoryId}' cannot be deleted because it contains products. Move products to another category first.");

    /// <summary>
    /// Creates an error indicating that a replacement category is required for deletion due to associated products.
    /// </summary>
    /// <param name="categoryId">The ID of the category being deleted that has associated products.</param>
    /// <returns>An <see cref="Error"/> with BadRequest type and appropriate error message.</returns>
    public static Error ReplacementRequired(Guid categoryId) =>
        Error.BadRequest("Category.ReplacementRequired",
            $"Category '{categoryId}' has associated products and requires a replacement category for deletion.");

    /// <summary>
    /// Creates an error indicating that the specified replacement category is not valid.
    /// </summary>
    /// <param name="replacementId">The ID of the invalid replacement category.</param>
    /// <returns>An <see cref="Error"/> with BadRequest type and appropriate error message.</returns>
    public static Error InvalidReplacement(Guid replacementId) =>
        Error.BadRequest("Category.InvalidReplacement",
            $"Replacement category '{replacementId}' is not valid - it may be inactive, deleted, or the same as the category being replaced.");

    /// <summary>
    /// Creates an error indicating that an inactive category cannot be used for new assignments.
    /// </summary>
    /// <param name="categoryId">The ID of the inactive category.</param>
    /// <returns>An <see cref="Error"/> with BadRequest type and appropriate error message.</returns>
    public static Error CategoryInactive(Guid categoryId) =>
        Error.BadRequest("Category.CategoryInactive",
            $"Category '{categoryId}' is inactive and cannot be used for new assignments.");

    /// <summary>
    /// Creates an error indicating that the category hierarchy has exceeded the maximum allowed depth.
    /// </summary>
    /// <param name="maxDepth">The maximum allowed depth for the category hierarchy.</param>
    /// <returns>An <see cref="Error"/> with BadRequest type and appropriate error message.</returns>
    public static Error MaxDepthExceeded(int maxDepth) =>
        Error.BadRequest("Category.MaxDepthExceeded",
            $"Category hierarchy cannot exceed {maxDepth} levels deep.");

    /// <summary>
    /// Creates an error indicating that the category name cannot be empty or whitespace.
    /// </summary>
    /// <returns>An <see cref="Error"/> with Validation type and appropriate error message.</returns>
    public static Error EmptyName() =>
        Error.Validation("Category.EmptyName", "Category name cannot be empty or whitespace.");

    /// <summary>
    /// Creates an error indicating that the category name exceeds the maximum allowed length.
    /// </summary>
    /// <param name="maxLength">The maximum allowed length for the category name.</param>
    /// <returns>An <see cref="Error"/> with Validation type and appropriate error message.</returns>
    public static Error NameTooLong(int maxLength) =>
        Error.Validation("Category.NameTooLong",
            $"Category name cannot exceed {maxLength} characters.");

    /// <summary>
    /// Creates an error indicating that the provided icon URL is not valid.
    /// </summary>
    /// <param name="iconUrl">The invalid icon URL.</param>
    /// <returns>An <see cref="Error"/> with Validation type and appropriate error message.</returns>
    public static Error InvalidIconUrl(string iconUrl) =>
        Error.Validation("Category.InvalidIconUrl",
            $"Icon URL '{iconUrl}' is not valid.");

    /// <summary>
    /// Creates an error indicating that the category tree structure is corrupted and requires administrator intervention.
    /// </summary>
    /// <returns>An <see cref="Error"/> with Failure type and appropriate error message.</returns>
    public static Error TreeCorrupted() =>
        Error.Failure("Category.TreeCorrupted",
            "Category tree structure is corrupted. Please contact system administrator.");

    /// <summary>
    /// Creates an error indicating that the nested set tree position values are invalid.
    /// </summary>
    /// <param name="lft">The left boundary value.</param>
    /// <param name="rgt">The right boundary value.</param>
    /// <returns>An <see cref="Error"/> with BadRequest type and appropriate error message.</returns>
    public static Error InvalidTreePosition(int lft, int rgt) =>
        Error.BadRequest("Category.InvalidTreePosition",
            $"Invalid tree position: left value ({lft}) must be less than right value ({rgt}).");

    /// <summary>
    /// Creates an error indicating that updating the category tree structure has failed.
    /// </summary>
    /// <returns>An <see cref="Error"/> with Failure type and appropriate error message.</returns>
    public static Error TreeUpdateFailed() =>
        Error.Failure("Category.TreeUpdateFailed",
            "Failed to update category tree structure. Operation has been rolled back.");

    /// <summary>
    /// Creates an error indicating that the search query is too short to perform a meaningful search.
    /// </summary>
    /// <param name="minLength">The minimum required length for the search query.</param>
    /// <returns>An <see cref="Error"/> with BadRequest type and appropriate error message.</returns>
    public static Error QueryTooShort(int minLength) =>
        Error.BadRequest("Category.QueryTooShort",
            $"Search query must be at least {minLength} characters long.");

    /// <summary>
    /// Creates an error indicating that no categories were found matching the search criteria.
    /// </summary>
    /// <param name="query">The search query that returned no results.</param>
    /// <returns>An <see cref="Error"/> with NotFound type and appropriate error message.</returns>
    public static Error NoSearchResults(string query) =>
        Error.NotFound("Category.NoSearchResults",
            $"No categories found matching '{query}'.");

    /// <summary>
    /// Creates an error indicating that the user does not have permission to perform the specified operation.
    /// </summary>
    /// <param name="operation">The operation that was denied.</param>
    /// <returns>An <see cref="Error"/> with Forbidden type and appropriate error message.</returns>
    public static Error AccessDenied(string operation) =>
        Error.Forbidden("Category.AccessDenied",
            $"You do not have permission to perform '{operation}' on categories.");

    /// <summary>
    /// Creates an error indicating that the category was modified by another user and needs to be refreshed.
    /// </summary>
    /// <param name="categoryId">The ID of the category that was concurrently modified.</param>
    /// <returns>An <see cref="Error"/> with Conflict type and appropriate error message.</returns>
    public static Error ConcurrentModification(Guid categoryId) =>
        Error.Conflict("Category.ConcurrentModification",
            $"Category '{categoryId}' was modified by another user. Please refresh and try again.");

    /// <summary>
    /// Creates an error indicating that the category status transition is not allowed.
    /// </summary>
    /// <param name="from">The current status of the category.</param>
    /// <param name="to">The target status that is not allowed.</param>
    /// <returns>An <see cref="Error"/> with BadRequest type and appropriate error message.</returns>
    public static Error InvalidStatusTransition(CategoryStatus from, CategoryStatus to) =>
        Error.BadRequest("Category.InvalidStatusTransition",
            $"Cannot change category status from '{from}' to '{to}'.");

    /// <summary>
    /// Creates an error indicating that root categories cannot be deleted.
    /// </summary>
    /// <param name="categoryId">The ID of the root category that cannot be deleted.</param>
    /// <returns>An <see cref="Error"/> with BadRequest type and appropriate error message.</returns>
    public static Error CannotDeleteRootCategory(Guid categoryId) =>
        Error.BadRequest("Category.CannotDeleteRootCategory",
            $"Root category '{categoryId}' cannot be deleted.");

    /// <summary>
    /// Creates an error indicating that a batch operation partially failed.
    /// </summary>
    /// <param name="successCount">The number of categories that were processed successfully.</param>
    /// <param name="totalCount">The total number of categories in the batch operation.</param>
    /// <returns>An <see cref="Error"/> with Problem type and appropriate error message.</returns>
    public static Error BatchOperationFailed(int successCount, int totalCount) =>
        Error.Problem("Category.BatchOperationFailed",
            $"Batch operation partially failed: {successCount}/{totalCount} categories processed successfully.");
}

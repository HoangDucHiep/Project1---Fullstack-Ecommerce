namespace ECommerceBackend.Application.Contracts.Products;


/// <summary>
/// DTO for product options (e.g., Color, Size, RAM)
/// </summary>
public record ProductOptionDto(
    string Name,
    List<string> Values
);


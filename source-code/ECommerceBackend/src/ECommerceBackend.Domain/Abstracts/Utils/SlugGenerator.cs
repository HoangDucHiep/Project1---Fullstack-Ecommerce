namespace ECommerceBackend.Domain.Abstracts.Utils;
public static class SlugGenerator
{
    public static string GenerateSlug(string name)
    {
        // Remove invalid characters, convert to lowercase, and replace spaces with hyphens
        string slug = name.ToLowerInvariant();
        string invalidChars = @"[^a-z0-9\s-]";
        slug = System.Text.RegularExpressions.Regex.Replace(slug, invalidChars, "");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", " ").Trim();
        slug = slug.Replace(" ", "-");

        // Additional logic can be added here to ensure uniqueness using timestamp
        slug += "-" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        return slug;
    }
}

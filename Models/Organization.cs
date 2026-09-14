namespace Kaad.Api.Models;

public class Organization
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CacNumber { get; set; } = string.Empty;
    public string Tin { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
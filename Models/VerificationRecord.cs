namespace Kaad.Api.Models;

public class VerificationRecord
{
    public Guid Id { get; set; }
    public string SubjectType { get; set; } = string.Empty;

    public Guid SubjectId { get; set; }

    public string CheckType { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime CheckedAt  { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; } 
}
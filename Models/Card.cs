namespace Kaad.Api.Models;

public class Card
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Slug { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string JobTitle { get; set; } = string.Empty;

    public string Company { get; set; } = string.Empty;

    public int Views { get; set; } = 0;


}
namespace Kaad.Api.DTOs; // Defines the namespace for the LoginDto class. It helps organize the code and avoid naming conflicts with other classes in the application.

public class LoginDto
{
    public string Email { get; set; } =  string.Empty;
    public string Password { get; set; } = string.Empty;
}

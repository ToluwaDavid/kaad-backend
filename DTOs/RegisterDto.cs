namespace Kaad.Api.DTOs; // Defines the namespace for the RegisterDto class. It helps organize the code and avoid naming conflicts with other classes in the application.

public class RegisterDto // 
{
   public string Email { get; set; } = string.Empty; // Gets or sets the email address for the user.
   public string Password { get; set; } = string.Empty; // Gets or sets the password for the user.
}
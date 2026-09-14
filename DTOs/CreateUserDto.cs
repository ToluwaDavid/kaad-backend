namespace Kaad.Api.DTOs; // Defines the namespace for the CreateUserDto class. It helps organize the code and avoid naming conflicts with other classes in the application.

public class CreateUserDto // This line defines the CreateUserDto class, which is a Data Transfer Object (DTO) used for creating a new user. DTOs are simple objects that are used to transfer data between layers of an application, typically between the client and server. The CreateUserDto class contains properties that represent the data required to create a new user, such as Email and Password.
{
    public string Email { get; set; } = string.Empty; // This line defines a public property named Email of type string. It represents the email address of the user being created. The property is initialized with an empty string to ensure that it has a default value and avoids null reference issues.

    public string Password { get; set; } = string.Empty; // This line defines a public property named Password of type string. It represents the password of the user being created. The property is initialized with an empty string to ensure that it has a default value and avoids null reference issues.
}
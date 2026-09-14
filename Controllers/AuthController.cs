using Microsoft.AspNetCore.Mvc; // Brings in the Microsoft.AspNetCore.Mvc namespace, which is necessary for working with controllers, actions, and other MVC-related features. It provides the necessary classes and attributes for building web APIs and handling HTTP requests and responses.

using Microsoft.EntityFrameworkCore; // Brings in the Microsoft.EntityFrameworkCore namespace, which is necessary for working with the DbContext and other EF Core features. It provides the necessary classes and methods for interacting with the database using Entity Framework Core.

using Microsoft.IdentityModel.Tokens; // Brings in the Microsoft.IdentityModel.Tokens namespace, which provides classes for working with security tokens. This is used for creating and validating the JWT token.
using System.IdentityModel.Tokens.Jwt; // Brings in the System.IdentityModel.Tokens.Jwt namespace, which provides classes for working with JWT tokens. This is used for creating and validating the JWT token.
using System.Security.Claims; // Brings in the System.Security.Claims namespace, which provides classes for working with security claims. This is used for creating the JWT token.
using System.Text; // Brings in the System.Text namespace, which provides classes for encoding and decoding strings. This is used for creating the JWT token.


using Kaad.Api.Data; // Brings in the Kaad.Api.Data namespace, which contains the AppDbContext class. This allows the AuthController to access the database context and perform CRUD operations on the User entity.
using Kaad.Api.Models; // Brings in the Kaad.Api.Models namespace, which contains the User class. This allows the AuthController to work with the User entity and perform operations related to user authentication and management.
using Kaad.Api.DTOs; // Brings in the Kaad.Api.DTOs namespace, which contains the LoginDto and RegisterDto classes. This allows the AuthController to work with these Data Transfer Objects (DTOs) for handling user login and registration requests.

namespace Kaad.Api.Controllers; // Defines the namespace for the AuthController class. It helps organize the code and avoid naming conflicts with other classes in the application.


[ApiController] // This attribute indicates that the AuthController is an API controller. It enables automatic model validation, response formatting, and other features specific to API controllers.


[Route("api/auth")] // This attribute defines the route template for the AuthController. It specifies that all actions in this controller will be prefixed with "api/auth" in the URL. For example, the Login action will be accessible at "api/auth/login".
public class AuthController : ControllerBase // this line defines the AuthController class, which inherits from ControllerBase. ControllerBase provides the basic functionality for handling HTTP requests and responses in an API controller.
{
    private readonly AppDbContext _db; 

    private readonly IConfiguration _config;

    public AuthController(AppDbContext db, IConfiguration config)
    {
        _db = db; // This line assigns the injected AppDbContext instance to the _db field. It allows the AuthController to use the database context for data access.

        _config = config; // This line assigns the injected IConfiguration instance to the _config field. It allows the AuthController to access configuration settings, such as the JWT secret key, from the appsettings.json file or other configuration sources.             
    }


    [HttpPost("register")] // This attribute indicates that the following action method will handle HTTP POST requests to the "register" endpoint. It allows clients to send registration information to the server.
    public async Task<IActionResult> Register(RegisterDto  dto)
    {
         if(dto is null || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password)) // This line checks if the RegisterDto object is null or if the Email or Password properties are null or empty. It ensures that the registration request contains valid data before proceeding with user creation.
        {
            return BadRequest(new { message = "Email and password are required." });
        }

        var email = dto.Email.Trim().ToLowerInvariant();

        var emailTaken = await _db.Users.AnyAsync(u => u.Email == email); // This line checks if the email provided in the RegisterDto is already taken by another user in the database. It uses the AnyAsync() method to perform an asynchronous query on the Users DbSet, returning true if any user with the same email exists.
       
        if (emailTaken)
        {
            return Conflict("Email is already taken."); // This line returns an HTTP 409 Conflict response with a message indicating that the email is already taken. It prevents the creation of a new user with a duplicate email address.
        }

        var user = new User
        {
            Id = Guid.NewGuid(), // This line generates a new unique identifier (GUID) for the user being created. It ensures that each user has a unique Id value.

            Email = email, // This line assigns the Email property of the User object to the value provided in the RegisterDto. It sets the email address of the user being created.

            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password), // This line hashes the password provided in the RegisterDto using the BCrypt hashing algorithm. It ensures that the password is securely stored in the database and not in plain text.

            CreatedAt = DateTime.UtcNow // This line sets the CreatedAt property of the User object to the current UTC date and time. It records the timestamp when the user was created.
        };

        _db.Users.Add(user); // This line adds the newly created User object to the Users DbSet in the database context. It prepares the user to be saved to the database.

        await _db.SaveChangesAsync(); // This line saves the changes made to the database context asynchronously. It commits the new user record to the database.   

        return Ok(new { message = "User registered successfully." });
    
    }  


    [HttpPost("login")] // This attribute indicates that the following action method will handle HTTP POST requests to the "login" endpoint. It allows clients to send login credentials to the server.
    public async Task<IActionResult> Login(LoginDto dto)
    {
        if (dto is null || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password)) // This line checks if the LoginDto object is null or if the Email or Password properties are null or empty. It ensures that the login request contains valid data before proceeding with user authentication.
        {
            return BadRequest(new { message = "Email and password are required." });
        }

        var email = dto.Email.Trim().ToLowerInvariant(); // This line normalizes the email address provided in the LoginDto by trimming any leading or trailing whitespace and converting it to lowercase. It ensures that the email comparison is case-insensitive and consistent.

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email); // This line queries the Users DbSet in the database context to find the first user with a matching email address. It uses the FirstOrDefaultAsync() method to perform an asynchronous query, returning null if no user is found.

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)) // This line checks if the user was not found in the database or if the provided password does not match the stored password hash. It uses BCrypt's Verify method to compare the plain text password with the hashed password.
        {
            return Unauthorized("Invalid email and Password."); // This line returns an HTTP 401 Unauthorized response with a message indicating that the email and password combination is invalid. It prevents unauthorized access to the application.
        }

         var token = CreateToken(user);

        return Ok(new { message = "Login successful." , token }); // This line returns an HTTP 200 OK response with a message indicating that the login was successful. It signifies that the user has been authenticated successfully and can proceed to access protected resources.
    }

    private string CreateToken(User user) // This line defines a private method named CreateToken that takes a User object as a parameter. It is responsible for creating a JWT token for the authenticated user. The method will generate the token based on the user's information and return it as a string.
    {
       var claims = new[] // This line defines an array of claims that will be included in the JWT token. Claims are pieces of information about the user that can be used for authorization and authentication purposes. In this case, the claims include the user's unique identifier (Id) and email address (Email).
       {
           new Claim("id" , user.Id.ToString()), // This line creates a new claim with the type "id" and the value of the user's unique identifier (Id) converted to a string. It represents the user's identity in the token.
           new Claim("email", user.Email) // This line creates a new claim with the type "email" and the value of the user's email address. It represents the user's email in the token.
       };

       var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)); // This line retrieves the secret key for signing the JWT token from the configuration settings. It uses the IConfiguration instance (_config) to access the "Jwt:Key" value, which is expected to be stored in the appsettings.json file or other configuration sources. The key is then converted to a byte array using UTF-8 encoding and wrapped in a SymmetricSecurityKey object, which will be used for signing the token.

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // This line creates a new instance of the SigningCredentials class, which represents the credentials used to sign the JWT token. It takes the symmetric security key (key) and specifies the HMAC-SHA256 algorithm for signing the token. The signing credentials will be used to ensure the integrity and authenticity of the token.

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"], // This line sets the issuer of the JWT token. The issuer is typically the entity that creates and signs the token. It retrieves the value from the configuration settings using the IConfiguration instance (_config) and accesses the "Jwt:Issuer" key.    
            claims: claims, // This line sets the claims that will be included in the JWT token. The claims array defined earlier is passed as an argument, which contains information about the user, such as their unique identifier (Id) and email address (Email). These claims can be used for authorization and authentication purposes.
            expires: DateTime.UtcNow.AddDays(7), // This line sets the expiration time of the JWT token. It specifies that the token will expire 7 days from the current UTC date and time. After this period, the token will no longer be valid, and the user will need to re-authenticate to obtain a new token.
            signingCredentials: creds // This line sets the signing credentials for the JWT token. The SigningCredentials object (creds) created earlier is passed as an argument, which contains the symmetric security key and the specified signing algorithm (HMAC-SHA256). The signing credentials ensure that the token can be verified and trusted by the server when it is received in subsequent requests.
        );

        return new JwtSecurityTokenHandler().WriteToken(token); // This line creates a new instance of the JwtSecurityTokenHandler class, which is responsible for creating and validating JWT tokens. It calls the WriteToken method, passing in the JwtSecurityToken object (token) created earlier. The WriteToken method serializes the token into a string representation, which can be returned to the client as the JWT token for authentication and authorization purposes.
    }
}
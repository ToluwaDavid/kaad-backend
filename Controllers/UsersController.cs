using Microsoft.AspNetCore.Mvc; // Brings in the Microsoft.AspNetCore.Mvc namespace, which is necessary for working with controllers, actions, and other MVC-related features. It provides the necessary classes and attributes for building web APIs and handling HTTP requests and responses.

using Microsoft.EntityFrameworkCore; // Brings in the Microsoft.EntityFrameworkCore namespace, which is necessary for working with the DbContext and other EF Core features. It provides the necessary classes and methods for interacting with the database using Entity Framework Core.
using Microsoft.AspNetCore.Authorization;
using Kaad.Api.Data;
using Kaad.Api.DTOs;
using Kaad.Api.Models; // Brings in the Kaad.Api.Data namespace, which contains the AppDbContext class. This allows the UsersController to access the database context and perform CRUD operations on the User entity.

namespace Kaad.Api.Controllers; // Defines the namespace for the UsersController class. It helps organize the code and avoid naming conflicts with other classes in the application.


[ApiController] // This attribute indicates that the UsersController is an API controller. It enables automatic model validation, response formatting, and other features specific to API controllers.


[Route("api/users")] // This attribute defines the route template for the UsersController. It specifies that all actions in this controller will be prefixed with "api/users" in the URL. For example, the GetUser action will be accessible at "api/users/{id}".

public class UsersController : ControllerBase // This line defines the UsersController class, which inherits from ControllerBase. ControllerBase provides the basic functionality for handling HTTP requests and responses in an API controller.
{
    private readonly AppDbContext _db; // This line declares a private readonly field named _db of type AppDbContext. It will be used to access the database context and perform CRUD operations on the User entity.

    public UsersController(AppDbContext db) // This is the constructor for the UsersController class. It takes an instance of AppDbContext as a parameter, which is injected by the dependency injection framework. The constructor initializes the _db field with the provided AppDbContext instance, allowing the controller to access the database context for data operations.
    {
        _db = db; // This line assigns the injected AppDbContext instance to the _db field. It allows the UsersController to use the database context for data access.
    }

    [HttpGet] // This attribute indicates that the following action method will handle HTTP GET requests. It allows clients to retrieve data from the server.
    [Authorize]
    public async Task<IActionResult> GetAllUsers() // This is the action method named GetAllUsers, which is responsible for handling HTTP GET requests to retrieve all users from the database. It returns a Task<IActionResult>, indicating that it is an asynchronous method that will return an IActionResult, which represents the result of an action method and can be used to generate an HTTP response.
    {
        var users = await _db.Users.ToListAsync(); // This line retrieves all users from the database asynchronously using Entity Framework Core. It calls the ToListAsync() method on the Users DbSet, which executes a SQL query to fetch all records from the Users table and returns them as a list of User objects.

        return Ok(users); // This line returns an HTTP 200 OK response with the list of users as the response body. The Ok() method is a helper method provided by ControllerBase to create a successful response.
    }

    [HttpPost] // This attribute indicates that the following action method will handle HTTP POST requests. It allows clients to send data to the server to create a new resource.
    public async Task<IActionResult> CreateUser(CreateUserDto dto)
    { 
        var user = new User // This line
        {
            
            Id = Guid.NewGuid(), // This line generates a new unique identifier (GUID) for the user being created. It ensures that each user has a unique Id value.

            Email = dto.Email, // This line assigns the Email property of the User object to the value provided in the CreateUserDto. It sets the email address of the user being created.

            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password), // This line hashes the password provided in the CreateUserDto using the BCrypt hashing algorithm. It ensures that the password is securely stored in the database and not in plain text.

            CreatedAt = DateTime.UtcNow // This line sets the CreatedAt property of the User object to the current UTC date and time. It records the timestamp when the user was created.
        };

        _db.Users.Add(user);

        await _db.SaveChangesAsync(); // This line saves the changes made to the database context asynchronously. It commits the new user record to the database.

        return Ok(user); // This line returns an HTTP 200 OK response with the newly created user as the response body. The Ok() method is a helper method provided by ControllerBase to create a successful response.
        
    }
}
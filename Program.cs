using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
// Brings in the Entity Framework Core namespace, which is necessary for working with the DbContext and other EF Core features. Brings in EF core - the translator toolkit that lets  our c# code talk to the database. It translates our C# code into SQL queries that the database can understand and execute.
using Microsoft.EntityFrameworkCore; 

using Kaad.Api.Data;
using Microsoft.Extensions.Options; // Brings in the Kaad.Api.Data folder, so this program cs file can see the AppDbContext class and use it to interact with the database.

var builder = WebApplication.CreateBuilder(args); // Creates a new web application builder instance. It is used to configure the application, set up services, and build the final web application. It is the construction site where we setup the application before it is built and run. It is the starting point for configuring the application and its services.


// Registers the AppDbContext with the dependency injection container. 
// Registers our database bridge (AppDbContext) with the app, and tells it to use PostgreSQL as the database provider. 
// It also retrieves the connection string named "KaadDb" from the application's configuration settings.
// It configures the DbContext to use PostgreSQL as the database provider and retrieves the connection string named "KaadDb" from the application's configuration settings. 
// This allows the application to connect to the PostgreSQL database using the specified connection string.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("KaadDb")));    
// Add services to the container.

// Adds the MVC controllers to the service collection. 
// services.AddControllers() registers the necessary services for handling HTTP requests and routing them to the appropriate controller actions. 
// It enables the application to handle incoming requests and return responses based on the defined controllers and their actions.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// Turns on OpenAPI (Swagger) support for the application. 
// It enables automatic generation of API documentation and provides a user-friendly interface for exploring and testing the API endpoints.
builder.Services.AddOpenApi();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});


var app = builder.Build(); // This line builds the web application using the configured services and middleware. It creates an instance of the WebApplication class, which represents the final application that can be run and handle incoming requests.

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) // This line checks if the application is running in the development environment. The app.Environment.IsDevelopment() method returns true if the current environment is set to "Development". This allows you to conditionally execute code based on the environment, enabling you to have different behavior for development, testing, and production environments.
{
    app.MapOpenApi(); // This line maps the OpenAPI (Swagger) endpoint to the application. It enables the generation of API documentation and provides a user-friendly interface for exploring and testing the API endpoints. This is typically done in the development environment to facilitate API testing and documentation.
}

app.UseHttpsRedirection(); // This line adds middleware to the request pipeline that redirects HTTP requests to HTTPS. It ensures that all incoming requests are served over a secure connection, enhancing the security of the application by encrypting the data transmitted between the client and server.   

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers(); // This line maps the controller routes to the application. It enables the routing of incoming HTTP requests to the appropriate controller actions based on the defined routes. This allows the application to handle requests and return responses based on the logic implemented in the controllers.

app.Run(); // This line starts the web application and begins listening for incoming HTTP requests. It blocks the execution of the program until the application is stopped, allowing it to continuously handle requests and provide responses.


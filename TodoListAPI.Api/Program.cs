using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoListAPI.Api.Middleware;
using TodoListAPI.Repository;
using TodoListAPI.Repository.Models;
using TodoListAPI.Repository.Repositories;
using TodoListAPI.Services.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Configure ProblemDetails for validation errors
        options.InvalidModelStateResponseFactory = context =>
        {
            var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Error",
                Detail = "One or more validation errors occurred.",
                Instance = context.HttpContext.Request.Path
            };

            // Add validation errors to the response
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            problemDetails.Extensions.Add("errors", errors);

            return new BadRequestObjectResult(problemDetails)
            {
                ContentTypes = { "application/problem+json" }
            };
        };
    });

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DbContext with SQLite (Scoped lifetime - one per HTTP request)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TodoListDbContext>(options =>
    options.UseSqlite(connectionString));

// Register Repositories (Scoped - one per HTTP request)
builder.Services.AddScoped<IListRepository, ListRepository>();
builder.Services.AddScoped<IListItemRepository, ListItemRepository>();

// Register Services (Scoped - one per HTTP request)
builder.Services.AddScoped<IListService, ListService>();
builder.Services.AddScoped<IListItemService, ListItemService>();

var app = builder.Build();

// Ensure database is created and migrations are applied
// Also create test user if it doesn't exist (for placeholder userId before authentication)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TodoListDbContext>();
    dbContext.Database.Migrate();
    
    // Create test user for placeholder userId (00000000-0000-0000-0000-000000000001)
    // This is needed until Task #20 (JWT Authentication) is implemented
    var testUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    var testUserExists = dbContext.Users.Any(u => u.Id == testUserId);
    
    if (!testUserExists)
    {
        var testUser = new User
        {
            Id = testUserId,
            Email = "test@example.com",
            Username = "TestUser",
            PasswordHash = "PLACEHOLDER_HASH", // Not used until authentication is implemented
            Role = "User",
            CreatedDate = DateTime.UtcNow
        };
        
        dbContext.Users.Add(testUser);
        dbContext.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add global exception handling middleware (must be before MapControllers)
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.MapControllers();

app.Run();

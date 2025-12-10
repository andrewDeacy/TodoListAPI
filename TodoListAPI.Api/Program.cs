using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "TodoList API",
        Description = "A RESTful API for managing todo lists and todo items. Built with .NET 8.0, EF Core, and SQLite.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "TodoList API Support"
        }
    });

    // Include XML comments in Swagger documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Include XML comments from Core project (for request/response models)
    var coreXmlFile = "TodoListAPI.Core.xml";
    var coreXmlPath = Path.Combine(AppContext.BaseDirectory, coreXmlFile);
    if (File.Exists(coreXmlPath))
    {
        options.IncludeXmlComments(coreXmlPath);
    }

    // Configure Swagger to support JWT Bearer authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.\nExample: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure CORS for frontend integration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // In development, allow common localhost ports for frontend frameworks
            policy.WithOrigins(
                "http://localhost:3000",  // React default
                "http://localhost:5173",  // Vite default
                "http://localhost:8080",  // Vue CLI default
                "http://localhost:4200",  // Angular default
                "http://localhost:5174",  // Vite alternate
                "http://localhost:5175"   // Vite alternate
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        }
        else
        {
            // In production, use configured origins from appsettings.json
            var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins")
                .Get<string[]>() ?? Array.Empty<string>();
            
            if (allowedOrigins.Length > 0)
            {
                policy.WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            }
            else
            {
                // Fallback: allow any origin (not recommended for production, but configurable)
                policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            }
        }
    });
});

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"];

if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("JWT SecretKey is not configured in appsettings.json");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// Configure Authorization
builder.Services.AddAuthorization();

// Configure DbContext with SQLite (Scoped lifetime - one per HTTP request)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TodoListDbContext>(options =>
    options.UseSqlite(connectionString));

// Register Repositories (Scoped - one per HTTP request)
builder.Services.AddScoped<IListRepository, ListRepository>();
builder.Services.AddScoped<IListItemRepository, ListItemRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Register Services (Scoped - one per HTTP request)
builder.Services.AddScoped<IListService, ListService>();
builder.Services.AddScoped<IListItemService, ListItemService>();
builder.Services.AddScoped<IAuthService, AuthService>();

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

// Add CORS middleware (must be before UseAuthentication and MapControllers)
app.UseCors("AllowFrontend");

// Add Authentication and Authorization middleware (must be before MapControllers)
app.UseAuthentication();
app.UseAuthorization();

// Add global exception handling middleware (must be before MapControllers)
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.MapControllers();

app.Run();

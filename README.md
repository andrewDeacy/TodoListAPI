# TodoList API

A production-quality RESTful API for managing todo lists and todo items. Built with .NET 8.0, Entity Framework Core, SQLite, and JWT authentication.

## Table of Contents

- [Features](#features)
- [Prerequisites](#prerequisites)
- [Setup Instructions](#setup-instructions)
- [Running the Application](#running-the-application)
- [Testing the API](#testing-the-api)
- [API Documentation](#api-documentation)
- [Project Structure](#project-structure)
- [Architecture Decisions](#architecture-decisions)
- [Assumptions](#assumptions)
- [Trade-offs and Decisions](#trade-offs-and-decisions)
- [Future Improvements](#future-improvements)

## Features

- ✅ **JWT Authentication** - Secure user registration and login
- ✅ **Todo List Management** - Create, read, update, and delete todo lists
- ✅ **Todo Item Management** - Add, update, delete, and mark items as complete
- ✅ **User Isolation** - Each user can only access their own lists and items
- ✅ **Input Validation** - Comprehensive validation on all request models
- ✅ **Global Error Handling** - Consistent error responses (RFC 7807 ProblemDetails)
- ✅ **Swagger Documentation** - Interactive API documentation with JWT support
- ✅ **Unit Tests** - 48 comprehensive unit tests covering service layer
- ✅ **Clean Architecture** - Separation of concerns with Repository and Service layers

## Prerequisites

- **.NET 8.0 SDK** or later ([Download](https://dotnet.microsoft.com/download))
- **SQLite** (included with .NET, no separate installation needed)
- **IDE** (optional): Visual Studio, Visual Studio Code, or Rider

### Verify Installation

```bash
dotnet --version
# Should output: 8.0.x or higher
```

## Setup Instructions

### 1. Clone the Repository

```bash
git clone <repository-url>
cd TodoListAPI
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Build the Solution

```bash
dotnet build
```

### 4. Run Database Migrations

Migrations are automatically applied when the application starts. The database file (`todolist.db`) will be created in the `TodoListAPI.Api` directory on first run.

### 5. Configure JWT Settings (Optional)

The default JWT configuration in `appsettings.json` is suitable for development. For production, update the `Jwt:SecretKey` to a secure, randomly generated key (at least 32 characters).

```json
{
  "Jwt": {
    "SecretKey": "YourSuperSecretKeyForJWTTokenGenerationMustBeAtLeast32CharactersLong",
    "Issuer": "TodoListAPI",
    "Audience": "TodoListAPIUsers",
    "ExpirationMinutes": 60
  }
}
```

## Running the Application

### Development Mode

```bash
cd TodoListAPI.Api
dotnet run
```

The API will start on:
- **HTTP**: `http://localhost:5074`
- **HTTPS**: `https://localhost:7254`

### Access Swagger UI

Once the application is running, navigate to:
- **Swagger UI**: `http://localhost:5074/swagger` or `https://localhost:7254/swagger`

The Swagger UI provides interactive API documentation where you can:
- View all available endpoints
- Test API calls directly from the browser
- Authenticate using JWT tokens (click "Authorize" button)

## Testing the API

### Using Swagger UI

1. Start the application (`dotnet run`)
2. Navigate to `http://localhost:5074/swagger`
3. Register a new user:
   - Click on `POST /api/auth/register`
   - Click "Try it out"
   - Enter user details (email, username, password)
   - Click "Execute"
   - Copy the `token` from the response
4. Authenticate:
   - Click the "Authorize" button at the top of the Swagger UI
   - Enter: `Bearer <your-token>` (replace `<your-token>` with the token from step 3)
   - Click "Authorize"
5. Test protected endpoints:
   - All endpoints with a lock icon now require authentication
   - Try creating a list, adding items, etc.

### Using HTTP Client File

The project includes `TodoListAPI.Api.http` with pre-configured requests:

1. Open the file in Visual Studio Code (with REST Client extension) or IntelliJ IDEA
2. Register a user and copy the token
3. Update the `@token` variable at the top of the file
4. Execute requests directly from the file

### Using cURL

#### 1. Register a New User

```bash
curl -X POST "http://localhost:5074/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "username": "testuser",
    "password": "Password123"
  }'
```

#### 2. Login

```bash
curl -X POST "http://localhost:5074/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "emailOrUsername": "user@example.com",
    "password": "Password123"
  }'
```

#### 3. Create a Todo List (with JWT token)

```bash
curl -X POST "http://localhost:5074/api/List" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE" \
  -d '{
    "name": "My Todo List",
    "description": "Things I need to do"
  }'
```

#### 4. Get All Lists (with JWT token)

```bash
curl -X GET "http://localhost:5074/api/List" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE"
```

### Running Unit Tests

```bash
dotnet test
```

All 48 unit tests should pass, covering:
- Service layer business logic
- Authentication service (registration, login, JWT generation)
- Error handling scenarios
- Edge cases

## API Documentation

### Base URL

- **Development**: `http://localhost:5074` or `https://localhost:7254`
- **Swagger UI**: `http://localhost:5074/swagger`

### Authentication

All endpoints except `/api/auth/register` and `/api/auth/login` require JWT authentication.

Include the JWT token in the `Authorization` header:
```
Authorization: Bearer <your-jwt-token>
```

### Endpoints

#### Authentication

- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login and receive JWT token

#### Todo Lists

- `GET /api/List` - Get all todo lists for the authenticated user
- `GET /api/List/{id}` - Get a specific todo list by ID
- `POST /api/List` - Create a new todo list
- `PUT /api/List/{id}` - Update a todo list
- `DELETE /api/List/{id}` - Delete a todo list

#### Todo Items

- `GET /api/lists/{listId}/items` - Get all items for a specific list
- `GET /api/lists/{listId}/items/{itemId}` - Get a specific item
- `POST /api/lists/{listId}/items` - Add an item to a list
- `PUT /api/lists/{listId}/items/{itemId}` - Update an item
- `DELETE /api/lists/{listId}/items/{itemId}` - Delete an item
- `PATCH /api/lists/{listId}/items/{itemId}/complete` - Mark item as complete/incomplete

For detailed API documentation with request/response examples, see the **Swagger UI** at `/swagger` when the application is running.

## Project Structure

```
TodoListAPI/
├── TodoListAPI.Api/          # Web API layer (Controllers, Middleware, Responses)
│   ├── Controllers/          # API endpoints
│   ├── Middleware/           # Global exception handling
│   ├── Models/              # Response models (API contract)
│   └── Extensions/          # Extension methods (JWT claims extraction)
├── TodoListAPI.Core/         # Shared models (DTOs, Requests)
│   ├── DTOs/                # Data Transfer Objects
│   └── Models/Requests/     # Request models
├── TodoListAPI.Services/     # Business logic layer
│   └── Services/            # Service implementations
├── TodoListAPI.Repository/   # Data access layer
│   ├── Models/              # Entity models (EF Core)
│   ├── Repositories/        # Repository implementations
│   └── Migrations/         # EF Core migrations
└── TodoListAPI.Tests/       # Unit tests
    └── Services/           # Service layer tests
```

### Architecture Layers

1. **Api Layer**: Controllers, middleware, response models
2. **Services Layer**: Business logic, validation, DTO mapping
3. **Repository Layer**: Data access, EF Core entities
4. **Core Layer**: Shared DTOs and request models

## Architecture Decisions

### 3-Layer Model Architecture

The project uses a 3-layer model architecture for separation of concerns:

1. **Entities** (Repository Layer)
   - Database representation with EF Core tracking
   - Internal to the repository layer

2. **DTOs** (Core Layer)
   - Internal service layer contract
   - Shared between Services and Api layers
   - Excludes sensitive fields (e.g., PasswordHash)

3. **Response Models** (Api Layer)
   - Public API contract
   - Explicitly excludes internal fields (e.g., UserId in responses)
   - Supports API versioning

**Rationale**: 
- Separation of concerns
- API versioning support
- Security by design (never expose internal fields)
- Industry standard for enterprise APIs

### Repository Pattern

- **Why**: Abstracts data access, enables testability, supports multiple data sources
- **Implementation**: Interface-based repositories with EF Core

### Service Layer

- **Why**: Encapsulates business logic, validation, and orchestration
- **Benefits**: Testable, reusable, maintains single responsibility

### JWT Authentication

- **Why**: Stateless, scalable, standard for REST APIs
- **Implementation**: BCrypt for password hashing, JWT tokens with 60-minute expiration

## Assumptions

1. **Single User Per Email/Username**: Each email and username must be unique across all users
2. **User Isolation**: Users can only access their own todo lists and items (enforced via JWT claims)
3. **Cascade Deletion**: Deleting a user deletes their lists; deleting a list deletes its items
4. **Default Role**: New users are assigned the "User" role by default
5. **Development Environment**: Default JWT secret key is acceptable for development (must be changed for production)
6. **SQLite Database**: SQLite is sufficient for this MVP (can be swapped for SQL Server/PostgreSQL in production)
7. **Frontend Integration**: CORS is configured for common frontend development ports (3000, 5173, 8080, 4200)

## Trade-offs and Decisions

### 1. SQLite vs. SQL Server/PostgreSQL

**Decision**: SQLite for MVP  
**Rationale**: 
- Zero configuration required
- Easy to run and test
- Sufficient for MVP scale
- Can be easily migrated to SQL Server/PostgreSQL later

**Trade-off**: Limited concurrent writes, not suitable for high-scale production

### 2. In-Memory Database vs. SQLite with Migrations

**Decision**: SQLite with EF Core migrations  
**Rationale**: 
- Reproducible database schema
- Easy to reset and test
- Production-like environment
- Follows assessment requirements

**Trade-off**: Requires file system access (not an issue for this use case)

### 3. 3-Layer Model (Entities, DTOs, Responses)

**Decision**: Separate models for each layer  
**Rationale**: 
- Clear separation of concerns
- API versioning support
- Security (never expose internal fields)
- Industry best practice

**Trade-off**: More mapping code, but provides better maintainability and security

### 4. JWT Token Expiration

**Decision**: 60 minutes  
**Rationale**: 
- Balance between security and user experience
- Configurable via appsettings.json

**Trade-off**: Users need to re-authenticate after 60 minutes (refresh tokens not implemented)

### 5. Password Hashing

**Decision**: BCrypt  
**Rationale**: 
- Industry standard
- Automatic salt generation
- Proven security

**Trade-off**: Slightly slower than other algorithms, but security is prioritized

### 6. Global Exception Handling

**Decision**: Custom middleware with ProblemDetails (RFC 7807)  
**Rationale**: 
- Consistent error responses
- Standard format
- Better developer experience

**Trade-off**: All exceptions go through same handler (can be extended for specific exception types)

### 7. Eager Loading vs. Lazy Loading

**Decision**: Eager loading for lists with items  
**Rationale**: 
- Predictable performance
- No N+1 query issues
- Explicit control over data loading

**Trade-off**: Always loads items even if not needed (acceptable for this use case)

## Future Improvements

If given more time, the following features would be implemented:

### High Priority

1. **Refresh Tokens**
   - Implement refresh token mechanism for seamless authentication
   - Store refresh tokens in database
   - Automatic token refresh on expiration

2. **Password Reset**
   - Email-based password reset flow
   - Secure token generation and expiration
   - Password strength requirements

3. **Integration Tests**
   - End-to-end API tests
   - Database integration tests
   - Authentication flow tests

4. **Logging and Monitoring**
   - Structured logging (Serilog)
   - Application Insights or similar
   - Performance metrics

5. **Rate Limiting**
   - Protect against abuse
   - Per-user and per-endpoint limits

### Medium Priority

6. **Pagination**
   - Paginated responses for lists and items
   - Cursor-based or offset-based pagination

7. **Filtering and Sorting**
   - Filter lists by completion status
   - Sort by date, name, etc.
   - Search functionality

8. **Soft Delete**
   - Archive deleted lists/items instead of hard delete
   - Recovery functionality

9. **List Sharing**
   - Share lists with other users
   - Permission levels (read-only, read-write)

10. **Due Date Reminders**
    - Email/SMS notifications for upcoming due dates
    - Configurable reminder preferences

### Low Priority

11. **API Versioning**
    - Support multiple API versions
    - Backward compatibility

12. **Caching**
    - Redis caching for frequently accessed data
    - Cache invalidation strategies

13. **Background Jobs**
    - Cleanup of old/completed items
    - Scheduled reminders

14. **Multi-tenancy**
    - Support for organizations/teams
    - Team-level todo lists

15. **Export/Import**
    - Export lists to JSON/CSV
    - Import from other todo apps

## License

This project is part of a take-home assessment and is not licensed for production use.

## Contact

For questions or issues, please contact the development team.

---

**Built with ❤️ using .NET 8.0**

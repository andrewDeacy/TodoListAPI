# TodoList API

A RESTful API for managing todo lists and todo items. Built with .NET 8.0, Entity Framework Core, SQLite, and JWT authentication.

## Requirements Overview

[Placeholder: Insert Reqs.png mind map here]

## Prerequisites

- .NET 8.0 SDK or later ([Download](https://dotnet.microsoft.com/download))
- SQLite (included with .NET, no separate installation needed)

Verify installation:
```bash
dotnet --version
# Should output: 8.0.x or higher
```

## Quick Start

### 1. Clone and Navigate

```bash
git clone <repository-url>
cd TodoListAPI
```

### 2. Restore and Build

```bash
dotnet restore
dotnet build
```

### 3. Run the Application

```bash
cd TodoListAPI.Api
dotnet run
```

The API will start on `http://localhost:5074`. Swagger UI is available at `http://localhost:5074/swagger`.

### 4. Database Setup

Migrations are automatically applied on first run. The database file (`todolist.db`) will be created in the `TodoListAPI.Api` directory.

## Testing the API

### Using Swagger UI

1. Navigate to `http://localhost:5074/swagger`
2. Register a new user via `POST /api/auth/register`
3. Copy the token from the response
4. Click "Authorize" and enter: `Bearer <your-token>`
5. Test protected endpoints

### Using cURL

Register a user:
```bash
curl -X POST "http://localhost:5074/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{"email": "user@example.com", "username": "testuser", "password": "Password123"}'
```

Login:
```bash
curl -X POST "http://localhost:5074/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"emailOrUsername": "user@example.com", "password": "Password123"}'
```

Create a list (with JWT token):
```bash
curl -X POST "http://localhost:5074/api/List" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE" \
  -d '{"name": "My Todo List", "description": "Things I need to do"}'
```

### Running Tests

```bash
dotnet test
```

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login and receive JWT token

### Todo Lists
- `GET /api/List` - Get all lists for authenticated user
- `GET /api/List/{id}` - Get a specific list
- `POST /api/List` - Create a new list
- `PUT /api/List/{id}` - Update a list
- `DELETE /api/List/{id}` - Delete a list

### Todo Items
- `GET /api/lists/{listId}/items` - Get all items in a list
- `POST /api/lists/{listId}/items` - Add an item to a list
- `PUT /api/lists/{listId}/items/{itemId}` - Update an item
- `DELETE /api/lists/{listId}/items/{itemId}` - Delete an item
- `PATCH /api/lists/{listId}/items/{itemId}/complete` - Mark item complete/incomplete
- `PATCH /api/lists/{listId}/items/reorder` - Reorder items within a list

All endpoints except authentication require JWT token in the `Authorization` header:
```
Authorization: Bearer <your-jwt-token>
```

For detailed API documentation, see Swagger UI at `/swagger` when running.

## Project Structure

```
TodoListAPI/
├── TodoListAPI.Api/          # Web API layer (Controllers, Middleware)
├── TodoListAPI.Core/          # Shared models (DTOs, Requests)
├── TodoListAPI.Services/      # Business logic layer
├── TodoListAPI.Repository/    # Data access layer (EF Core)
└── TodoListAPI.Tests/        # Unit tests
```

## Architecture Overview

The API uses a layered architecture with clear separation of concerns:

- **Api Layer**: Controllers handle HTTP requests/responses, middleware for global error handling
- **Services Layer**: Business logic, validation, DTO mapping
- **Repository Layer**: Data access with EF Core, entity models
- **Core Layer**: Shared DTOs and request models

Key decisions:
- DTOs for all API responses (never return EF entities directly)
- Global error handling with consistent ProblemDetails responses
- JWT authentication with BCrypt password hashing
- SQLite with EF Core migrations for easy setup

## Assumptions

- Each email and username must be unique
- Users can only access their own lists and items (enforced via JWT)
- Deleting a list cascades to delete all items
- New users are assigned "User" role by default
- SQLite is sufficient for MVP (can migrate to SQL Server/PostgreSQL later)

## Future Improvements

If given more time:
- Refresh tokens for seamless authentication
- Password reset functionality
- Integration tests
- Structured logging and monitoring
- Rate limiting
- Pagination for large datasets
- List sharing and collaboration features

## Configuration

JWT settings can be configured in `appsettings.json`:

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

For production, use a secure, randomly generated key (at least 32 characters).

---

**Built with .NET 8.0**

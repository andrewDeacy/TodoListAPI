using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace TodoListAPI.Api.Middleware;

/// <summary>
/// Global exception handling middleware that catches all unhandled exceptions
/// and returns consistent error responses using ProblemDetails format.
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    /// <summary>
    /// Initializes a new instance of the GlobalExceptionHandlerMiddleware class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">Logger for recording exceptions.</param>
    /// <param name="environment">Hosting environment to determine if detailed errors should be shown.</param>
    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
    }

    /// <summary>
    /// Invokes the middleware to handle exceptions.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred. {ExceptionType}: {Message}", 
                ex.GetType().Name, ex.Message);
            
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Handles the exception and returns an appropriate HTTP response.
    /// </summary>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var problemDetails = CreateProblemDetails(context, exception);
        
        context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(problemDetails, jsonOptions);
        await context.Response.WriteAsync(json);
    }

    /// <summary>
    /// Creates a ProblemDetails object based on the exception type.
    /// </summary>
    private ProblemDetails CreateProblemDetails(HttpContext context, Exception exception)
    {
        var problemDetails = new ProblemDetails
        {
            Instance = context.Request.Path,
            Detail = _environment.IsDevelopment() ? exception.ToString() : null
        };

        switch (exception)
        {
            case ArgumentNullException argNullEx:
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Title = "Invalid Request";
                problemDetails.Detail = _environment.IsDevelopment() 
                    ? exception.ToString() 
                    : $"Required parameter '{argNullEx.ParamName}' is missing or null.";
                break;

            case ArgumentException argEx:
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Title = "Invalid Request";
                problemDetails.Detail = _environment.IsDevelopment() 
                    ? exception.ToString() 
                    : argEx.Message;
                break;

            case InvalidOperationException invalidOpEx:
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Title = "Invalid Operation";
                problemDetails.Detail = invalidOpEx.Message;
                break;

            case KeyNotFoundException keyNotFoundEx:
                problemDetails.Status = (int)HttpStatusCode.NotFound;
                problemDetails.Title = "Resource Not Found";
                problemDetails.Detail = _environment.IsDevelopment() 
                    ? exception.ToString() 
                    : keyNotFoundEx.Message;
                break;

            case UnauthorizedAccessException unauthorizedEx:
                problemDetails.Status = (int)HttpStatusCode.Unauthorized;
                problemDetails.Title = "Unauthorized";
                problemDetails.Detail = _environment.IsDevelopment() 
                    ? exception.ToString() 
                    : "You are not authorized to perform this action.";
                break;

            case Microsoft.EntityFrameworkCore.DbUpdateException dbUpdateEx:
                problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                problemDetails.Title = "Database Error";
                problemDetails.Detail = _environment.IsDevelopment() 
                    ? exception.ToString() 
                    : "An error occurred while processing your request.";
                break;

            default:
                problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                problemDetails.Title = "Internal Server Error";
                problemDetails.Detail = _environment.IsDevelopment() 
                    ? exception.ToString() 
                    : "An unexpected error occurred while processing your request.";
                break;
        }

        return problemDetails;
    }
}


using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using VariBillWebAPI.Exceptions;

namespace VariBillWebAPI.Middleware;

/// <summary>
/// Global exception handling middleware.
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Unhandled exception occurred. Path: {Path}", context.Request.Path);

        var response = context.Response;
        response.ContentType = "application/problem+json";

        // Default error response
        var errorResponse = new
        {
            type = "https://tools.ietf.org/html/rfc7807",
            title = "An error occurred",
            status = 500,
            detail = _env.IsDevelopment() ? exception.Message : "Internal server error",
            instance = context.Request.Path,
            traceId = context.TraceIdentifier
        };

        // Handle specific exception types
        switch (exception)
        {
            // Custom validation exception
            case Exceptions.ValidationException valEx:
                response.StatusCode = 400;
                errorResponse = new
                {
                    type = "validation",
                    title = "Validation failed",
                    status = 400,
                    detail = _env.IsDevelopment() ? valEx.Message : "One or more validation errors occurred",
                    instance = context.Request.Path,
                    traceId = context.TraceIdentifier
                };
                break;

            // Not found exception
            case NotFoundException:
                response.StatusCode = 404;
                errorResponse = new
                {
                    type = "notfound",
                    title = "Resource not found",
                    status = 404,
                    detail = _env.IsDevelopment() ? exception.Message : "The requested resource was not found",
                    instance = context.Request.Path,
                    traceId = context.TraceIdentifier
                };
                break;

            // Business rule exception
            case BusinessRuleException:
                response.StatusCode = 409;
                errorResponse = new
                {
                    type = "businessrule",
                    title = "Business rule violation",
                    status = 409,
                    detail = _env.IsDevelopment() ? exception.Message : "The operation violates a business rule",
                    instance = context.Request.Path,
                    traceId = context.TraceIdentifier
                };
                break;

            // Database exceptions (merged from commented code)
            case DbUpdateException dbEx when dbEx.InnerException is SqlException sqlEx:
                response.StatusCode = 409;
                errorResponse = new
                {
                    type = "database",
                    title = "Database conflict",
                    status = 409,
                    detail = sqlEx.Number switch
                    {
                        547 => "Foreign key violation - referenced record does not exist",
                        2601 => "Duplicate key violation - record already exists",
                        2627 => "Duplicate key violation - record already exists",
                        _ => _env.IsDevelopment() ? sqlEx.Message : "Database error occurred"
                    },
                    instance = context.Request.Path,
                    traceId = context.TraceIdentifier
                };
                break;

            // Unauthorized access
            case UnauthorizedAccessException:
                response.StatusCode = 401;
                errorResponse = new
                {
                    type = "unauthorized",
                    title = "Authentication required",
                    status = 401,
                    detail = "You must be authenticated to access this resource",
                    instance = context.Request.Path,
                    traceId = context.TraceIdentifier
                };
                break;

            // Argument exceptions (invalid parameters)
            case ArgumentException argEx:
                response.StatusCode = 400;
                errorResponse = new
                {
                    type = "badrequest",
                    title = "Invalid request",
                    status = 400,
                    detail = _env.IsDevelopment() ? argEx.Message : "The request contains invalid parameters",
                    instance = context.Request.Path,
                    traceId = context.TraceIdentifier
                };
                break;
        }

        // Ensure status code is set (fallback to 500 if not set by specific handlers)
        if (response.StatusCode == 0)
        {
            response.StatusCode = 500;
        }

        var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        await response.WriteAsync(json);
    }
}

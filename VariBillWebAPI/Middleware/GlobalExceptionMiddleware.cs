using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using VariBillWebAPI.Exceptions;

namespace VariBillWebAPI.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _env;

    // Inject IHostEnvironment to check for Development/Production
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception occurred. Path: {Path}", httpContext.Request.Path);

        // Base ProblemDetails object
        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path,
            Extensions = { ["traceId"] = httpContext.TraceIdentifier }
        };

        // Map specific exceptions to HTTP status codes and messages
        switch (exception)
        {
            case ValidationException valEx:
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                problemDetails.Type = "validation";
                problemDetails.Title = "Validation failed";
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Detail = _env.IsDevelopment() ? valEx.Message : "One or more validation errors occurred";
                break;

            case NotFoundException: // Assuming you have this custom exception
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                problemDetails.Type = "notfound";
                problemDetails.Title = "Resource not found";
                problemDetails.Status = StatusCodes.Status404NotFound;
                problemDetails.Detail = _env.IsDevelopment() ? exception.Message : "The requested resource was not found";
                break;

            case BusinessRuleException:
                httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
                problemDetails.Type = "businessrule";
                problemDetails.Title = "Business rule violation";
                problemDetails.Status = StatusCodes.Status409Conflict;
                problemDetails.Detail = _env.IsDevelopment() ? exception.Message : "The operation violates a business rule";
                break;

            case DbUpdateException dbEx when dbEx.InnerException is SqlException sqlEx:
                httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
                problemDetails.Type = "database";
                problemDetails.Title = "Database conflict";
                problemDetails.Status = StatusCodes.Status409Conflict;
                problemDetails.Detail = sqlEx.Number switch
                {
                    547 => "Foreign key violation - referenced record does not exist",
                    2601 => "Duplicate key violation - record already exists",
                    2627 => "Duplicate key violation - record already exists",
                    _ => _env.IsDevelopment() ? sqlEx.Message : "Database error occurred"
                };
                break;

            case UnauthorizedAccessException:
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                problemDetails.Type = "unauthorized";
                problemDetails.Title = "Authentication required";
                problemDetails.Status = StatusCodes.Status401Unauthorized;
                problemDetails.Detail = "You must be authenticated to access this resource";
                break;

            case ArgumentException argEx:
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                problemDetails.Type = "badrequest";
                problemDetails.Title = "Invalid request";
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Detail = _env.IsDevelopment() ? argEx.Message : "The request contains invalid parameters";
                break;

            default:
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "An error occurred";
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Detail = _env.IsDevelopment() ? exception.Message : "Internal server error";
                break;
        }

        // Write the structured JSON response
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        // Return true to stop the pipeline (the exception has been handled)
        return true;
    }
}

//using System.Text.Json;
//using Microsoft.Data.SqlClient;
//using Microsoft.EntityFrameworkCore;
//using VariBillWebAPI.Exceptions;

//namespace VariBillWebAPI.Middleware;

///// <summary>
///// Global exception handling middleware.
///// </summary>
//public class GlobalExceptionMiddleware
//{
//    private readonly RequestDelegate _next;
//    private readonly ILogger<GlobalExceptionMiddleware> _logger;
//    private readonly IHostEnvironment _env;

//    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment env)
//    {
//        _next = next;
//        _logger = logger;
//        _env = env;
//    }

//    public async Task InvokeAsync(HttpContext context)
//    {
//        try
//        {
//            await _next(context);
//        }
//        catch (Exception ex)
//        {
//            await HandleExceptionAsync(context, ex);
//        }
//    }

//    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
//    {
//        _logger.LogError(exception, "Unhandled exception occurred. Path: {Path}", context.Request.Path);

//        var response = context.Response;
//        response.ContentType = "application/problem+json";

//        // Default error response
//        var errorResponse = new
//        {
//            type = "https://tools.ietf.org/html/rfc7807",
//            title = "An error occurred",
//            status = 500,
//            detail = _env.IsDevelopment() ? exception.Message : "Internal server error",
//            instance = context.Request.Path,
//            traceId = context.TraceIdentifier
//        };

//        // Handle specific exception types
//        switch (exception)
//        {
//            // Custom validation exception
//            case Exceptions.ValidationException valEx:
//                response.StatusCode = 400;
//                errorResponse = new
//                {
//                    type = "validation",
//                    title = "Validation failed",
//                    status = 400,
//                    detail = _env.IsDevelopment() ? valEx.Message : "One or more validation errors occurred",
//                    instance = context.Request.Path,
//                    traceId = context.TraceIdentifier
//                };
//                break;

//            // Not found exception
//            case NotFoundException:
//                response.StatusCode = 404;
//                errorResponse = new
//                {
//                    type = "notfound",
//                    title = "Resource not found",
//                    status = 404,
//                    detail = _env.IsDevelopment() ? exception.Message : "The requested resource was not found",
//                    instance = context.Request.Path,
//                    traceId = context.TraceIdentifier
//                };
//                break;

//            // Business rule exception
//            case BusinessRuleException:
//                response.StatusCode = 409;
//                errorResponse = new
//                {
//                    type = "businessrule",
//                    title = "Business rule violation",
//                    status = 409,
//                    detail = _env.IsDevelopment() ? exception.Message : "The operation violates a business rule",
//                    instance = context.Request.Path,
//                    traceId = context.TraceIdentifier
//                };
//                break;

//            // Database exceptions (merged from commented code)
//            case DbUpdateException dbEx when dbEx.InnerException is SqlException sqlEx:
//                response.StatusCode = 409;
//                errorResponse = new
//                {
//                    type = "database",
//                    title = "Database conflict",
//                    status = 409,
//                    detail = sqlEx.Number switch
//                    {
//                        547 => "Foreign key violation - referenced record does not exist",
//                        2601 => "Duplicate key violation - record already exists",
//                        2627 => "Duplicate key violation - record already exists",
//                        _ => _env.IsDevelopment() ? sqlEx.Message : "Database error occurred"
//                    },
//                    instance = context.Request.Path,
//                    traceId = context.TraceIdentifier
//                };
//                break;

//            // Unauthorized access
//            case UnauthorizedAccessException:
//                response.StatusCode = 401;
//                errorResponse = new
//                {
//                    type = "unauthorized",
//                    title = "Authentication required",
//                    status = 401,
//                    detail = "You must be authenticated to access this resource",
//                    instance = context.Request.Path,
//                    traceId = context.TraceIdentifier
//                };
//                break;

//            // Argument exceptions (invalid parameters)
//            case ArgumentException argEx:
//                response.StatusCode = 400;
//                errorResponse = new
//                {
//                    type = "badrequest",
//                    title = "Invalid request",
//                    status = 400,
//                    detail = _env.IsDevelopment() ? argEx.Message : "The request contains invalid parameters",
//                    instance = context.Request.Path,
//                    traceId = context.TraceIdentifier
//                };
//                break;
//        }

//        // Ensure status code is set (fallback to 500 if not set by specific handlers)
//        if (response.StatusCode == 0)
//        {
//            response.StatusCode = 500;
//        }

//        var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
//        {
//            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
//        });
//        await response.WriteAsync(json);
//    }
//}

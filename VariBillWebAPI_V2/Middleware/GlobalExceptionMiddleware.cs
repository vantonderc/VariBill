using System.ComponentModel.DataAnnotations;
using System.Text.Json;
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
        _logger.LogError(exception, "Unhandled exception occurred.");

        var response = context.Response;
        response.ContentType = "application/problem+json";

        var errorResponse = new
        {
            type = "https://tools.ietf.org/html/rfc7807",
            title = "An error occurred",
            status = 500,
            detail = _env.IsDevelopment() ? exception.Message : "Internal server error",
            instance = context.Request.Path,
            traceId = context.TraceIdentifier
        };

        switch (exception)
        {
            case Exceptions.ValidationException valEx:
                response.StatusCode = 400;
                errorResponse = new
                {
                    type = "validation",
                    title = valEx.Message,
                    status = 400,
                    detail = valEx.Message,
                    instance = context.Request.Path,
                    traceId = context.TraceIdentifier
                };
                break;

            case NotFoundException:
                response.StatusCode = 404;
                errorResponse = new
                {
                    type = "notfound",
                    title = exception.Message,
                    status = 404,
                    detail = exception.Message,
                    instance = context.Request.Path,
                    traceId = context.TraceIdentifier
                };
                break;

            case BusinessRuleException:
                response.StatusCode = 409;
                errorResponse = new
                {
                    type = "businessrule",
                    title = exception.Message,
                    status = 409,
                    detail = exception.Message,
                    instance = context.Request.Path,
                    traceId = context.TraceIdentifier
                };
                break;
        }

        var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await response.WriteAsync(json);
    }
}






//using Microsoft.Data.SqlClient;
//using Microsoft.EntityFrameworkCore;
//using System.ComponentModel.DataAnnotations;
//using System.Text.Json;
//using VariBillWebAPI.Exceptions;

//namespace VariBillWebAPI.Middleware
//{
//    public class GlobalExceptionMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly ILogger<GlobalExceptionMiddleware> _logger;
//        private readonly IHostEnvironment _env;

//        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment env)
//        {
//            _next = next;
//            _logger = logger;
//            _env = env;
//        }

//        public async Task InvokeAsync(HttpContext context)
//        {
//            try
//            {
//                await _next(context);
//            }
//            catch (Exception ex)
//            {
//                await HandleExceptionAsync(context, ex);
//            }
//        }

//        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
//        {
//            _logger.LogError(exception, "Unhandled exception occurred");

//            var response = context.Response;
//            response.ContentType = "application/problem+json";

//            var errorResponse = new
//            {
//                type = "https://tools.ietf.org/html/rfc7807",
//                title = "An error occurred",
//                status = 500,
//                detail = _env.IsDevelopment() ? exception.Message : "Internal server error",
//                instance = context.Request.Path,
//                traceId = context.TraceIdentifier
//            };

//            if (exception is ValidationException valEx)
//            {
//                response.StatusCode = 400;
//                errorResponse = new { type = "validation", title = valEx.Message, status = 400, detail = valEx.Message, instance = context.Request.Path, traceId = context.TraceIdentifier };
//            }
//            else if (exception is NotFoundException)
//            {
//                response.StatusCode = 404;
//                errorResponse = new { type = "notfound", title = exception.Message, status = 404, detail = exception.Message, instance = context.Request.Path, traceId = context.TraceIdentifier };
//            }
//            else if (exception is DbUpdateException dbEx && dbEx.InnerException is SqlException sqlEx)
//            {
//                response.StatusCode = 409;
//                errorResponse = new
//                {
//                    type = "database",
//                    title = "Database conflict",
//                    status = 409,
//                    detail = sqlEx.Number == 547 ? "Foreign key violation." : "Database error.",
//                    instance = context.Request.Path,
//                    traceId = context.TraceIdentifier
//                };
//            }
//            else
//            {
//                response.StatusCode = 500;
//            }

//            await response.WriteAsJsonAsync(errorResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
//        }
//    }
//}

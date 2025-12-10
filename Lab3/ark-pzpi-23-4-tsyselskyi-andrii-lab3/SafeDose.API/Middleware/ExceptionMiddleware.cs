using SafeDose.API.Models;
using SafeDose.Application.Exceptions;
using System.Net;
using System.Text.Json;
using UnauthorizedAccessException = SafeDose.Application.Exceptions.UnauthorizedAccessException;

namespace SafeDose.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            var problem = new CustomProblemDetails();
            string logMessage;

            switch (ex)
            {
                case BadRequestException badRequest:
                    statusCode = HttpStatusCode.BadRequest;
                    logMessage = $"BadRequest: {badRequest.Message}";
                    problem = new CustomProblemDetails
                    {
                        Title = "Bad Request",
                        Status = (int)statusCode,
                        Type = nameof(BadRequestException),
                        Errors = badRequest.ValidationErrors
                    };
                    _logger.LogWarning(ex, logMessage);
                    break;

                case NotFoundException notFound:
                    statusCode = HttpStatusCode.NotFound;
                    logMessage = $"NotFound: {notFound.Message}";
                    problem = new CustomProblemDetails
                    {
                        Title = "Resource not found",
                        Status = (int)statusCode,
                        Type = nameof(NotFoundException)
                    };
                    _logger.LogWarning(ex, logMessage);
                    break;

                case ForbiddenException forbidden:
                    statusCode = HttpStatusCode.Forbidden;
                    logMessage = $"Forbidden: {forbidden.Message}";
                    problem = new CustomProblemDetails
                    {
                        Title = "Access forbidden",
                        Status = (int)statusCode,
                        Type = nameof(ForbiddenException)
                    };
                    _logger.LogWarning(ex, logMessage);
                    break;

                case UnauthorizedAccessException unauthorized:
                    statusCode = HttpStatusCode.Unauthorized;
                    logMessage = $"Unauthorized: {unauthorized.Message}";
                    problem = new CustomProblemDetails
                    {
                        Title = "Unauthorized access",
                        Status = (int)statusCode,
                        Type = nameof(UnauthorizedAccessException)
                    };
                    _logger.LogWarning(ex, logMessage);
                    break;

                case EmailNotSentException emailError:
                    statusCode = HttpStatusCode.ServiceUnavailable;
                    logMessage = $"Email service error: {emailError.Message}";
                    problem = new CustomProblemDetails
                    {
                        Title = "Email service unavailable",
                        Status = (int)statusCode,
                        Type = nameof(EmailNotSentException)
                    };
                    _logger.LogError(ex, logMessage);
                    break;

                case InternalServerException internalError:
                    statusCode = HttpStatusCode.InternalServerError;
                    logMessage = $"Internal server error: {internalError.Message}";
                    problem = new CustomProblemDetails
                    {
                        Title = "Internal server error",
                        Status = (int)statusCode,
                        Type = nameof(InternalServerException)
                    };
                    _logger.LogError(ex, logMessage);
                    break;

                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    logMessage = $"Unhandled exception: {ex.Message}";
                    problem = new CustomProblemDetails
                    {
                        Title = "An unexpected error occurred.",
                        Status = (int)statusCode,
                        Type = nameof(HttpStatusCode.InternalServerError)
                    };
                    _logger.LogError(ex, logMessage);
                    break;
            }

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)statusCode;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(problem, options));
        }
    }
}

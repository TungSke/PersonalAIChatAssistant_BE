using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace WaifuAIAssistant.API.Middleware
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                // Trường hợp request đi qua mà ra 401/403 từ pipeline (Auth middleware)
                if (!context.Response.HasStarted &&
                    (context.Response.StatusCode == StatusCodes.Status401Unauthorized ||
                     context.Response.StatusCode == StatusCodes.Status403Forbidden))
                {
                    await WriteErrorResponseAsync(context, context.Response.StatusCode,
                        context.Response.StatusCode == 401 ? "Unauthorized access" : "Forbidden access");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                if (!context.Response.HasStarted)
                {
                    await HandleExceptionAsync(context, ex);
                }
                else
                {
                    _logger.LogWarning("Response has already started, unable to write error response.");
                }
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var statusCode = HttpStatusCode.InternalServerError;

            switch (ex)
            {
                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    break;
                case NotSupportedException:
                    statusCode = HttpStatusCode.NotImplemented;
                    break;
                case ArgumentException:
                case NullReferenceException:
                    statusCode = HttpStatusCode.BadRequest;
                    break;
                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Unauthorized;
                    break;
            }

            return WriteErrorResponseAsync(context, (int)statusCode, ex.Message, ex.StackTrace);
        }

        private static Task WriteErrorResponseAsync(HttpContext context, int statusCode, string message, string? details = null)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new
            {
                Code = statusCode,
                Message = message,
                Details = details
            };

            return context.Response.WriteAsJsonAsync(response);
        }
    }

    public static class GlobalExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        }
    }
}

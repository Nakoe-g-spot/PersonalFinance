using System.Text.Json;

namespace PersonalFinance.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // tiếp tục pipeline nếu không có lỗi
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❗Lỗi không mong muốn xảy ra");

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var result = JsonSerializer.Serialize(new
                {
                    error = "Có lỗi xảy ra trong hệ thống.",
                    detail = ex.Message
                });

                await context.Response.WriteAsync(result);
            }
        }
    }

    // Extension method để đăng ký middleware này dễ dàng hơn trong Program.cs
    public static class GlobalExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}
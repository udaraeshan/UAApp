using System.Text;
using UAApp.Shared.Log;

namespace UAApp.Server.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IApplicationLogger _logger;

        public RequestLoggingMiddleware(RequestDelegate next, IApplicationLogger logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
            string requestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            _logger.Log(LogLevel.Information, new LogFormat
            {
                Message = $"API Request: {context.Request.Method} {context.Request.Path}",
                Description = $"Query: {context.Request.QueryString}\nBody: {requestBody}",
                UserID = context.User.Identity?.Name ?? "Anonymous"
            });
            await _next(context);
        }
    }
}

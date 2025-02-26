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

            //// Capture the response
            var originalBodyStream = context.Response.Body;
            using var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            await _next(context);

            // Log Response
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            string responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            _logger.Log(LogLevel.Information, new LogFormat
            {
                Message = $"API Response: {context.Request.Method} {context.Request.Path}",
                Description = $"Status Code: {context.Response.StatusCode}\nBody: {responseBody}",
                UserID = context.User.Identity?.Name ?? "Anonymous"
            });

            // Copy the response back to the original stream
            await responseBodyStream.CopyToAsync(originalBodyStream);
        }
    }
}

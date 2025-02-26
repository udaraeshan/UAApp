using UAApp.Shared.Log;

namespace UAApp.Server.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IApplicationLogger _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, IApplicationLogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, new LogFormat
                {
                    Message = ex.Message,
                    Description = ex.InnerException?.Message,
                });
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = exception.Message,
                Details = exception.InnerException?.Message
            };

            return context.Response.WriteAsJsonAsync(response);
        }
    }

}

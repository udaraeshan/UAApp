using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace UAApp.Shared.Log
{
    public class ApplicationLogger : IApplicationLogger
    {
        private readonly ILogger _logger;

        public ApplicationLogger(ILogger<ApplicationLogger> logger)
        {
            _logger = logger;
        }
        public object? Log(LogLevel level, LogFormat logFormat)
        {

            switch (level)
            {
                case LogLevel.Information:
                    logFormat.Severity = LogLevelProperties.INFO;
                    logFormat.StatusCode = logFormat.StatusCode == 0 ? (int)HttpStatusCode.OK : logFormat.StatusCode;
                    _logger.LogInformation("Message : {Message}, UserId : {UserId}", JsonSerializer.Serialize(logFormat), logFormat.UserID);
                    break;
                case LogLevel.Error:
                    logFormat.Severity = LogLevelProperties.ERROR;
                    logFormat.StatusCode = logFormat.StatusCode == 0 ? (int)HttpStatusCode.InternalServerError : logFormat.StatusCode;
                    _logger.LogError("Message : {Message}, UserId : {UserId}", JsonSerializer.Serialize(logFormat), logFormat.UserID);
                    break;
                case LogLevel.Warning:
                    logFormat.StatusCode = logFormat.StatusCode == 0 ? (int)HttpStatusCode.OK : logFormat.StatusCode;
                    logFormat.Severity = LogLevelProperties.WARNING;
                    _logger.LogWarning("Message : {Message}, UserId : {UserId}", JsonSerializer.Serialize(logFormat), logFormat.UserID);
                    break;
                case LogLevel.Critical:
                    logFormat.Severity = LogLevelProperties.FATAL;
                    logFormat.StatusCode = logFormat.StatusCode == 0 ? (int)HttpStatusCode.InternalServerError : logFormat.StatusCode;
                    _logger.LogCritical("Message : {Message},  UserId : {UserId}", JsonSerializer.Serialize(logFormat), logFormat.UserID);
                    break;
                case LogLevel.Debug:
                    logFormat.Severity = LogLevelProperties.DEBUG;
                    logFormat.StatusCode = logFormat.StatusCode == 0 ? (int)HttpStatusCode.OK : logFormat.StatusCode;
                    _logger.LogDebug("Message : {Message},UserId : {UserId}", JsonSerializer.Serialize(logFormat), logFormat.UserID);
                    break;

                case LogLevel.Trace:
                    logFormat.Severity = LogLevelProperties.TRACE;
                    logFormat.StatusCode = logFormat.StatusCode == 0 ? (int)HttpStatusCode.OK : logFormat.StatusCode;
                    _logger.LogTrace("Message : {Message},  UserId : {UserId}", JsonSerializer.Serialize(logFormat), logFormat.UserID);
                    break;
            }

            return default;
        }
    }
}

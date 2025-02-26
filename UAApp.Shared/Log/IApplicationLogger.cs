using Microsoft.Extensions.Logging;

namespace UAApp.Shared.Log
{
    public interface IApplicationLogger
    {
        /// <summary>
        /// Logging all the details using serilog
        /// </summary>
        /// <param name="level"></param>
        /// <param name="logFormat"></param>
        /// <returns>void</returns>
        public object Log(LogLevel level, LogFormat logFormat);
    }
}

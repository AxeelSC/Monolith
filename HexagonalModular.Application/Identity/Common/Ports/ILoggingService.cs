using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexagonalModular.Application.Identity.Common.Ports
{
    public interface ILoggingService
    {
        void LogWarning(string message, string traceId, params object[] args);
        void LogError(string message, string traceId, params object[] args);
        void LogInfo(string message, string traceId, params object[] args);
    }

    public class LoggingService : ILoggingService
    {
        private readonly ILogger<LoggingService> _logger;

        public LoggingService(ILogger<LoggingService> logger)
        {
            _logger = logger;
        }

        public void LogWarning(string message, string traceId, params object[] args)
        {
            _logger.LogWarning(message + " TraceId={TraceId}", args.Append(traceId).ToArray());
        }

        public void LogError(string message, string traceId, params object[] args)
        {
            _logger.LogError(message + " TraceId={TraceId}", args.Append(traceId).ToArray());
        }

        public void LogInfo(string message, string traceId, params object[] args)
        {
            _logger.LogInformation(message + " TraceId={TraceId}", args.Append(traceId).ToArray());
        }
    }

}

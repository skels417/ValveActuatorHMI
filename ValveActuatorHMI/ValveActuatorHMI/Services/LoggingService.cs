using NLog;
using ValveActuatorHMI.Models;

namespace ValveActuatorHMI.Services
{
    public class LoggingService : ILoggingService
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public void LogDebug(string message) => Logger.Debug(message);
        public void LogInfo(string message) => Logger.Info(message);
        public void LogWarning(string message) => Logger.Warn(message);
        public void LogError(string message) => Logger.Error(message);
        public void LogFatal(string message) => Logger.Fatal(message);
    }
}
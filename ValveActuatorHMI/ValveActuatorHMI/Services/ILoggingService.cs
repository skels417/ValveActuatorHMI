namespace ValveActuatorHMI.Services
{
    public interface ILoggingService
    {
        void LogDebug(string message);
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message);
        void LogFatal(string message);
    }
}
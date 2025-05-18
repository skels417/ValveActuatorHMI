using System;

namespace ValveActuatorHMI.Models
{
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Level { get; set; }
        public string Source { get; set; } // Добавляем это свойство
        public string Message { get; set; }

        // Конструктор для удобства
        public LogEntry(DateTime timestamp, string level, string source, string message)
        {
            Timestamp = timestamp;
            Level = level;
            Source = source;
            Message = message;
        }
    }
}
namespace ValveActuatorHMI.Models
{
    public class TestSettings
    {
        public double PartialMovePosition { get; set; } = 50.0;
        public double IntermediateStopDelay { get; set; } = 5.0;
        public double WrongPositionAlarmDelay { get; set; } = 10.0;
        public double TimeModeDuration { get; set; } = 60.0;
        public int CircleModeCycles { get; set; } = 100;
        public double CommandDelay { get; set; } = 1.0;

        // Добавленные свойства
        public double MoveTorque { get; set; } = 0.0;
        public double ClosedTorque { get; set; } = 0.0;
        public double OpenTorque { get; set; } = 0.0;
        public int CurrentCycles { get; set; } = 0;
        public int TestMode { get; set; } = 0;
    }
}
namespace ValveActuatorHMI.Models
{
    public class AlarmSettings
    {
        public double VoltageHighLevel { get; set; } = 250.0; // В
        public double VoltageLowLevel { get; set; } = 180.0; // В
        public double VoltageHighDelay { get; set; } = 5.0; // с
        public double VoltageLowDelay { get; set; } = 5.0; // с

        public double OverheatLevel { get; set; } = 80.0; // °C
        public double OvercoolLevel { get; set; } = -20.0; // °C
        public double OverheatDelay { get; set; } = 10.0; // с
        public double OvercoolDelay { get; set; } = 10.0; // с

        public double NoMovementDelay { get; set; } = 30.0; // с
        public bool ProtectionsEnabled { get; set; } = true;
    }
}
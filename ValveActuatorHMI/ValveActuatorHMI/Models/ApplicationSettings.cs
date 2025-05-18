namespace ValveActuatorHMI.Models
{
    public class ApplicationSettings
    {
        public AlarmSettings AlarmSettings { get; set; } = new AlarmSettings();
        public CalibrationSettings CalibrationSettings { get; set; } = new CalibrationSettings();
        public TestSettings TestSettings { get; set; } = new TestSettings();
        public InterfaceSettings InterfaceSettings { get; set; } = new InterfaceSettings();
    }
}
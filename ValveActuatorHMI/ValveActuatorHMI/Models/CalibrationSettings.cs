namespace ValveActuatorHMI.Models
{
    public class CalibrationSettings
    {
        public bool IsOpenPositionCalibrated { get; set; }
        public bool IsClosedPositionCalibrated { get; set; }
        public bool IsDeviceCalibrated { get; set; }
        public bool PositionSensorError { get; set; }

        public int OpenPositionRevolutions { get; set; } = 10;
        public int ClosedPositionRevolutions { get; set; } = 10;
    }
}
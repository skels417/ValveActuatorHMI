namespace ValveActuatorHMI.Models
{
    public class CalibrationStatus
    {
        public string OpenPositionText => IsOpenPositionCalibrated ? "Открытое положение откалибровано" : "Открытое положение не откалибровано";
        public string ClosedPositionText => IsClosedPositionCalibrated ? "Закрытое положение откалибровано" : "Закрытое положение не откалибровано";
        public string DeviceCalibrationText => IsDeviceCalibrated ? "Устройство откалибровано" : "Устройство не откалибровано";

        public bool IsOpenPositionCalibrated { get; set; }
        public bool IsClosedPositionCalibrated { get; set; }
        public bool IsDeviceCalibrated { get; set; }
        public bool PositionSensorError { get; set; }
    }
}
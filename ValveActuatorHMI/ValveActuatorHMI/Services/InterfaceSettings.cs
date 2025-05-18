namespace ValveActuatorHMI.Models
{
    public class InterfaceSettings
    {
        public int ModbusDeviceId { get; set; } = 1;
        public int BaudRate { get; set; } = 115200;
        public string Parity { get; set; } = "None";

        public double AnalogInputLevel { get; set; } = 5.0; // В
        public double AnalogOutputLevel { get; set; } = 5.0; // В
        public double AnalogInputCurrent { get; set; } = 20.0; // мА
        public double AnalogOutputCurrent { get; set; } = 20.0; // мА
        public double DeadZone { get; set; } = 5.0; // %
        public bool AnalogInverted { get; set; } = false;
    }
}
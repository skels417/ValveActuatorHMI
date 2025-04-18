namespace ValveActuatorHMI.Models
{
    public class VariableMap
    {
        public string Name { get; set; }
        public ushort Address { get; set; }
        public string DataType { get; set; }
        public string Unit { get; set; }
        public double ScaleFactor { get; set; }
        public double Offset { get; set; }
    }
}
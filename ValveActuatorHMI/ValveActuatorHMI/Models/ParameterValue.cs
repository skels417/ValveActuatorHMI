using System;

namespace ValveActuatorHMI.Models
{
    public class ParameterValue
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string ParameterName { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }
    }
}
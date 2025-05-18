using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValveActuatorHMI.Models
{
    public class DeviceStatus
    {
        public double Position { get; set; } // %
        public double Speed { get; set; } // об/мин
        public double Torque { get; set; } // Н*м
        public double RadiatorTemp { get; set; } // °C
        public double IndicatorTemp { get; set; } // °C
        public double SensorTemp { get; set; } // °C
        public double InputVoltage { get; set; } // В
        public double MotorCurrent { get; set; } // А
        public string Status { get; set; } // Текстовый статус
    }
}

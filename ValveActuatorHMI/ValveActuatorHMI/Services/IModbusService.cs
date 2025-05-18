using System.IO.Ports;
using System.Threading.Tasks;

namespace ValveActuatorHMI.Services
{
    public interface IModbusService
    {
        bool IsConnected { get; }
        bool Connect(string portName, int baudRate = 115200, Parity parity = Parity.None,
                    int dataBits = 8, StopBits stopBits = StopBits.One);
        void Disconnect();
        ushort[] ReadHoldingRegisters(byte slaveAddress, ushort startAddress, ushort numberOfPoints);
        void WriteSingleRegister(byte slaveAddress, ushort registerAddress, ushort value);
    }
}
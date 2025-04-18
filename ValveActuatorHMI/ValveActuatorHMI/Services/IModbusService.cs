using System.IO.Ports;

namespace ValveActuatorHMI.Services
{
    public interface IModbusService
    {
        bool IsConnected { get; }
        bool Connect(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits);
        void Disconnect();
        ushort[] ReadHoldingRegisters(byte slaveAddress, ushort startAddress, ushort numberOfPoints);
        void WriteSingleRegister(byte slaveAddress, ushort registerAddress, ushort value);
    }
}
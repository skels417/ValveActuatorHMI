using NModbus;
using System.IO.Ports;

namespace ValveActuatorHMI.Services
{
    public class ModbusService : IModbusService
    {
        private SerialPort _serialPort;
        private IModbusSerialMaster _modbusMaster;
        private bool _isConnected;

        public bool IsConnected => _isConnected;

        public bool Connect(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            try
            {
                _serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
                _serialPort.Open();

                var factory = new ModbusFactory();
                _modbusMaster = factory.CreateRtuMaster(new SerialPortAdapter(_serialPort));

                _isConnected = true;
                return true;
            }
            catch
            {
                _isConnected = false;
                return false;
            }
        }

        public void Disconnect()
        {
            _modbusMaster?.Dispose();
            _serialPort?.Close();
            _isConnected = false;
        }

        public ushort[] ReadHoldingRegisters(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            return _modbusMaster.ReadHoldingRegisters(slaveAddress, startAddress, numberOfPoints);
        }

        public void WriteSingleRegister(byte slaveAddress, ushort registerAddress, ushort value)
        {
            _modbusMaster.WriteSingleRegister(slaveAddress, registerAddress, value);
        }
    }
}
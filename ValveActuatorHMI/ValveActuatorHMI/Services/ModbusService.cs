using NModbus;
using System.IO.Ports;
using System;
using NLog;
using NModbus.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace ValveActuatorHMI.Services
{
    public class ModbusService : IModbusService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private SerialPort _serialPort;
        private IModbusSerialMaster _modbusMaster;
        private bool _isConnected;

        public bool IsConnected => _isConnected;

        public bool Connect(string portName, int baudRate = 115200, Parity parity = Parity.None,
                   int dataBits = 8, StopBits stopBits = StopBits.One)
        {
            try
            {
                Disconnect();

                _serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits)
                {
                    ReadTimeout = 1000,
                    WriteTimeout = 1000,
                    Handshake = Handshake.None,
                    DtrEnable = true,
                    RtsEnable = true
                };

                // Добавляем небольшую задержку перед открытием порта
                Thread.Sleep(100);

                _serialPort.Open();

                // Проверяем, что порт действительно открыт
                if (!_serialPort.IsOpen)
                {
                    Logger.Error($"Не удалось открыть порт {portName}");
                    return false;
                }

                // Добавляем небольшую задержку после открытия порта
                Thread.Sleep(100);

                var adapter = new SerialPortAdapter(_serialPort);
                var factory = new ModbusFactory();
                _modbusMaster = factory.CreateRtuMaster(adapter);
                _modbusMaster.Transport.ReadTimeout = 1000;
                _modbusMaster.Transport.WriteTimeout = 1000;

                // Проверяем соединение, читая регистр
                try
                {
                    var testRegister = _modbusMaster.ReadHoldingRegisters(1, 0, 1);
                    Logger.Info($"Успешное подключение к {portName}, тестовое чтение: {testRegister[0]}");
                }
                catch
                {
                    Logger.Error($"Не удалось прочитать тестовый регистр с {portName}");
                    Disconnect();
                    return false;
                }

                _isConnected = true;
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Ошибка подключения к {portName}");
                Disconnect();
                return false;
            }
        }

        public void Disconnect()
        {
            try
            {
                _modbusMaster?.Dispose();
                _modbusMaster = null;

                if (_serialPort?.IsOpen == true)
                {
                    _serialPort.Close();
                }
                _serialPort?.Dispose();
                _serialPort = null;

                _isConnected = false;
                Logger.Info("Устройство отключено");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка при отключении");
                throw;
            }
        }

        public ushort[] ReadHoldingRegisters(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            if (!_isConnected || _modbusMaster == null)
                throw new InvalidOperationException("Устройство не подключено");

            try
            {
                return _modbusMaster.ReadHoldingRegisters(slaveAddress, startAddress, numberOfPoints);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Ошибка чтения регистров {startAddress}-{startAddress + numberOfPoints}");
                Disconnect();
                throw;
            }
        }

        public void WriteSingleRegister(byte slaveAddress, ushort registerAddress, ushort value)
        {
            if (!_isConnected || _modbusMaster == null)
                throw new InvalidOperationException("Устройство не подключено");

            try
            {
                _modbusMaster.WriteSingleRegister(slaveAddress, registerAddress, value);
                Logger.Debug($"Запись в регистр {registerAddress}: {value}");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Ошибка записи в регистр {registerAddress}");
                Disconnect();
                throw;
            }
        }

        public async Task<ushort[]> ReadHoldingRegistersAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            if (!_isConnected || _modbusMaster == null)
                throw new InvalidOperationException("Устройство не подключено");

            try
            {
                return await Task.Run(() =>
                    _modbusMaster.ReadHoldingRegisters(slaveAddress, startAddress, numberOfPoints));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Ошибка асинхронного чтения регистров {startAddress}-{startAddress + numberOfPoints}");
                Disconnect();
                throw;
            }
        }

        public async Task WriteSingleRegisterAsync(byte slaveAddress, ushort registerAddress, ushort value)
        {
            if (!_isConnected || _modbusMaster == null)
                throw new InvalidOperationException("Устройство не подключено");

            try
            {
                await Task.Run(() =>
                    _modbusMaster.WriteSingleRegister(slaveAddress, registerAddress, value));
                Logger.Debug($"Асинхронная запись в регистр {registerAddress}: {value}");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Ошибка асинхронной записи в регистр {registerAddress}");
                Disconnect();
                throw;
            }
        }

        private readonly Dictionary<string, ushort> _parameterAddressMap = new Dictionary<string, ushort>
        {
            // From Excel file: A0.10 Положение 0-1000
            { "Position", 123 },
            // A0.7 Текущая скорость вращения
            { "Speed", 116 },
            // A0.9 Момент
            { "Torque", 115 },
            // A1.0 Температура радиатора
            { "RadiatorTemp", 117 },
            // A1.2 Температура индикатора
            { "IndicatorTemp", 401 },
            // A1.1 Температура датчика положения
            { "SensorTemp", 305 },
            // A0.5 Входное напряжение
            { "InputVoltage", 114 },
            // A0.8 Текущий ток двигателя
            { "MotorCurrent", 118 },
            // A0.3 Статус блока
            { "Status", 122 }
        };
    }
}
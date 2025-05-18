using NModbus.IO;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using System;

namespace ValveActuatorHMI.Services
{
    public class SerialPortAdapter : IStreamResource
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly SerialPort _serialPort;

        public SerialPortAdapter(SerialPort serialPort)
        {
            _serialPort = serialPort;
            // Устанавливаем стандартные таймауты
            _serialPort.ReadTimeout = 1000;
            _serialPort.WriteTimeout = 1000;
        }

        public int InfiniteTimeout => SerialPort.InfiniteTimeout;

        public int ReadTimeout
        {
            get => _serialPort.ReadTimeout;
            set => _serialPort.ReadTimeout = value;
        }

        public int WriteTimeout
        {
            get => _serialPort.WriteTimeout;
            set => _serialPort.WriteTimeout = value;
        }

        public void DiscardInBuffer()
        {
            try
            {
                _serialPort.DiscardInBuffer();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка при очистке буфера порта");
                throw;
            }
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            try
            {
                return _serialPort.BaseStream.Read(buffer, offset, count);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка чтения из порта");
                throw;
            }
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            try
            {
                // Проверяем, открыт ли порт
                if (!_serialPort.IsOpen)
                {
                    Logger.Error("Попытка записи в закрытый порт");
                    throw new InvalidOperationException("Порт не открыт");
                }

                _serialPort.BaseStream.Write(buffer, offset, count);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка записи в порт");
                throw;
            }
        }

        public Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            try
            {
                return _serialPort.BaseStream.ReadAsync(buffer, offset, count, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка асинхронного чтения из порта");
                throw;
            }
        }

        public Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            try
            {
                if (!_serialPort.IsOpen)
                {
                    Logger.Error("Попытка асинхронной записи в закрытый порт");
                    throw new InvalidOperationException("Порт не открыт");
                }

                return _serialPort.BaseStream.WriteAsync(buffer, offset, count, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка асинхронной записи в порт");
                throw;
            }
        }

        public void Dispose()
        {
            try
            {
                _serialPort?.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка при освобождении ресурсов порта");
            }
        }
    }
}
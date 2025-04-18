using NModbus.IO;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace ValveActuatorHMI.Services
{
    public class SerialPortAdapter : IStreamResource
    {
        private readonly SerialPort _serialPort;

        public SerialPortAdapter(SerialPort serialPort)
        {
            _serialPort = serialPort;
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
            _serialPort.DiscardInBuffer();
        }

        // Синхронные методы, которые отсутствовали
        public int Read(byte[] buffer, int offset, int count)
        {
            return _serialPort.BaseStream.Read(buffer, offset, count);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            _serialPort.BaseStream.Write(buffer, offset, count);
        }

        // Асинхронные методы
        public Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return _serialPort.BaseStream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return _serialPort.BaseStream.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public void Dispose()
        {
            _serialPort?.Dispose();
        }
    }
}
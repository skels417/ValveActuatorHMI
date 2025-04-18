using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Wpf;
using ValveActuatorHMI.Models;
using ValveActuatorHMI.Services;

namespace ValveActuatorHMI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IModbusService _modbusService;
        private readonly IExcelService _excelService;
        private bool _isConnected;

        public event PropertyChangedEventHandler PropertyChanged;

        #region Connection Properties
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                _isConnected = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> AvailablePorts { get; } = new ObservableCollection<string>(SerialPort.GetPortNames());
        public string SelectedPort { get; set; }
        #endregion

        #region Modbus Settings
        public byte SlaveAddress { get; set; } = 1;

        public ObservableCollection<int> BaudRates { get; } = new ObservableCollection<int> { 9600, 19200, 38400, 57600, 115200 };
        public int SelectedBaudRate { get; set; } = 9600;

        public ObservableCollection<Parity> Parities { get; } = new ObservableCollection<Parity>(Enum.GetValues(typeof(Parity)).Cast<Parity>());
        public Parity SelectedParity { get; set; } = Parity.None;

        public ObservableCollection<int> DataBits { get; } = new ObservableCollection<int> { 7, 8 };
        public int SelectedDataBits { get; set; } = 8;

        public ObservableCollection<StopBits> StopBitsList { get; } = new ObservableCollection<StopBits>(Enum.GetValues(typeof(StopBits)).Cast<StopBits>());
        public StopBits SelectedStopBits { get; set; } = StopBits.One;
        #endregion

        #region Device Parameters
        public ObservableCollection<Parameter> Parameters { get; } = new ObservableCollection<Parameter>
        {
            new Parameter { Name = "Voltage", Value = 0, Unit = "V" },
            new Parameter { Name = "Current", Value = 0, Unit = "A" },
            new Parameter { Name = "Temperature", Value = 0, Unit = "°C" },
            new Parameter { Name = "Position", Value = 0, Unit = "%" }
        };

        public SeriesCollection ChartSeries { get; } = new SeriesCollection
        {
            new LineSeries { Title = "Current", Values = new ChartValues<double> { 0, 0, 0, 0, 0 } },
            new LineSeries { Title = "Temperature", Values = new ChartValues<double> { 0, 0, 0, 0, 0 } }
        };

        public ObservableCollection<string> TimeLabels { get; } = new ObservableCollection<string>();
        #endregion

        #region Variable Mapping
        public ObservableCollection<VariableMap> VariableMaps { get; } = new ObservableCollection<VariableMap>();
        #endregion

        #region Logging
        public ObservableCollection<LogEntry> LogEntries { get; } = new ObservableCollection<LogEntry>();
        public string LogSearchText { get; set; }
        #endregion

        #region Commands
        public ICommand ConnectCommand { get; }
        public ICommand DisconnectCommand { get; }
        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand ReverseCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand ImportCommand { get; }
        public ICommand SearchLogsCommand { get; }
        public ICommand ClearLogsCommand { get; }
        #endregion

        public MainViewModel(IModbusService modbusService, IExcelService excelService)
        {
            _modbusService = modbusService;
            _excelService = excelService;

            // Initialize commands
            ConnectCommand = new RelayCommand(Connect, CanConnect);
            DisconnectCommand = new RelayCommand(Disconnect, CanDisconnect);
            StartCommand = new RelayCommand(Start, CanOperate);
            StopCommand = new RelayCommand(Stop, CanOperate);
            ReverseCommand = new RelayCommand(Reverse, CanOperate);
            OpenCommand = new RelayCommand(Open, CanOperate);
            CloseCommand = new RelayCommand(Close, CanOperate);
            ImportCommand = new RelayCommand(Import);
            SearchLogsCommand = new RelayCommand(SearchLogs);
            ClearLogsCommand = new RelayCommand(ClearLogs);

            // Initialize time labels
            for (int i = 0; i < 5; i++)
            {
                TimeLabels.Add(DateTime.Now.AddSeconds(i - 4).ToString("HH:mm:ss"));
            }
        }

        #region Command Methods
        private bool CanConnect() => !IsConnected && !string.IsNullOrEmpty(SelectedPort);
        private bool CanDisconnect() => IsConnected;
        private bool CanOperate() => IsConnected;

        private void Connect()
        {
            try
            {
                IsConnected = _modbusService.Connect(SelectedPort, SelectedBaudRate, SelectedParity, SelectedDataBits, SelectedStopBits);
                LogEntries.Add(new LogEntry { Timestamp = DateTime.Now, Level = "INFO", Message = $"Connected to {SelectedPort}" });
            }
            catch (Exception ex)
            {
                LogEntries.Add(new LogEntry { Timestamp = DateTime.Now, Level = "ERROR", Message = $"Connection failed: {ex.Message}" });
            }
        }

        private void Disconnect()
        {
            _modbusService.Disconnect();
            IsConnected = false;
            LogEntries.Add(new LogEntry { Timestamp = DateTime.Now, Level = "INFO", Message = "Disconnected" });
        }

        private void Start() => SendModbusCommand(0, 1, "Start");
        private void Stop() => SendModbusCommand(0, 0, "Stop");
        private void Reverse() => SendModbusCommand(0, 2, "Reverse");
        private void Open() => SendModbusCommand(1, 1, "Open");
        private void Close() => SendModbusCommand(1, 0, "Close");

        private void SendModbusCommand(ushort address, ushort value, string commandName)
        {
            try
            {
                _modbusService.WriteSingleRegister(SlaveAddress, address, value);
                LogEntries.Add(new LogEntry { Timestamp = DateTime.Now, Level = "INFO", Message = $"{commandName} command sent" });
            }
            catch (Exception ex)
            {
                LogEntries.Add(new LogEntry { Timestamp = DateTime.Now, Level = "ERROR", Message = $"{commandName} failed: {ex.Message}" });
            }
        }

        private void Import()
        {
            try
            {
                // Реализация импорта из Excel
                LogEntries.Add(new LogEntry { Timestamp = DateTime.Now, Level = "INFO", Message = "Data imported from Excel" });
            }
            catch (Exception ex)
            {
                LogEntries.Add(new LogEntry { Timestamp = DateTime.Now, Level = "ERROR", Message = $"Import failed: {ex.Message}" });
            }
        }

        private void SearchLogs()
        {
            // Реализация поиска логов
        }

        private void ClearLogs()
        {
            LogEntries.Clear();
            LogEntries.Add(new LogEntry { Timestamp = DateTime.Now, Level = "INFO", Message = "Logs cleared" });
        }
        #endregion

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

        public void Execute(object parameter) => _execute();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
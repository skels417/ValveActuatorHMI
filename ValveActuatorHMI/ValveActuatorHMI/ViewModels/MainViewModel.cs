using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Windows;
using MaterialDesignThemes.Wpf;
using NModbus;
using NLog;
using ValveActuatorHMI.Models;
using ValveActuatorHMI.Services;
using System.Windows.Input;
using LiveCharts.Wpf;
using LiveCharts;
using System.Collections.Generic;
using System.Linq;

namespace ValveActuatorHMI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IModbusService _modbusService;
        private readonly ISettingsService _settingsService;
        private readonly ISnackbarMessageQueue _notificationQueue;
        private readonly IThemeService _themeService;
        private readonly IExcelService _excelService;

        private DeviceStatus _deviceStatus = new DeviceStatus();
        private AlarmSettings _alarmSettings = new AlarmSettings();
        private CalibrationSettings _calibrationSettings = new CalibrationSettings();
        private TestSettings _testSettings = new TestSettings();
        private InterfaceSettings _interfaceSettings = new InterfaceSettings();

        private string _connectionStatus = "Не подключено";
        private string _lastUpdateTime;
        private bool _isConnected;
        private bool _isDarkTheme;
        private string _selectedPort;
        private string _selectedExcelFile;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand ConnectCommand { get; }
        public ICommand DisconnectCommand { get; }
        public ICommand RefreshPortsCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand MoveCommand { get; }
        public ICommand SaveSettingsCommand { get; }
        public ICommand LoadSettingsCommand { get; }
        public ICommand CalibrateOpenCommand { get; }
        public ICommand CalibrateCloseCommand { get; }
        public ICommand StartTestCommand { get; }
        public ICommand StopTestCommand { get; }
        public ICommand ToggleThemeCommand { get; private set; }
        public ICommand LoadExcelCommand { get; private set; }


        public MainViewModel(IModbusService modbusService, ISettingsService settingsService, IExcelService excelService)
        {
            _modbusService = modbusService;
            _excelService = excelService;
            _settingsService = settingsService;
            _notificationQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
            _themeService = new ThemeService();

            // Initialize commands
            ConnectCommand = new RelayCommand(Connect, CanConnect);
            DisconnectCommand = new RelayCommand(Disconnect, CanDisconnect);
            RefreshPortsCommand = new RelayCommand(RefreshPorts);
            OpenCommand = new RelayCommand(ExecuteOpen, CanExecuteCommand);
            CloseCommand = new RelayCommand(ExecuteClose, CanExecuteCommand);
            StopCommand = new RelayCommand(ExecuteStop, CanExecuteCommand);
            MoveCommand = new RelayCommand(ExecuteMove, CanExecuteCommand);
            SaveSettingsCommand = new RelayCommand(SaveSettings);
            LoadSettingsCommand = new RelayCommand(LoadSettings);
            CalibrateOpenCommand = new RelayCommand(CalibrateOpenPosition, CanExecuteCommand);
            CalibrateCloseCommand = new RelayCommand(CalibrateClosePosition, CanExecuteCommand);
            StartTestCommand = new RelayCommand(StartTest, CanExecuteCommand);
            StopTestCommand = new RelayCommand(StopTest, CanExecuteCommand);
            ToggleThemeCommand = new RelayCommand(ToggleTheme);
            LoadExcelCommand = new RelayCommand(LoadExcel);


            LoadSettings();
            RefreshPorts();
            InitializeCharts();

            var timer = new System.Timers.Timer(1000); // Update every second
            timer.Elapsed += async (sender, e) => await UpdateDeviceData();
            timer.Start();
        }

        public DeviceStatus DeviceStatus
        {
            get => _deviceStatus;
            set
            {
                _deviceStatus = value;
                OnPropertyChanged(nameof(DeviceStatus));
            }
        }

        private CalibrationStatus _calibrationStatus = new CalibrationStatus();
        private SeriesCollection _chartSeries = new SeriesCollection();
        private string[] _timeLabels = Array.Empty<string>();

        public CalibrationStatus CalibrationStatus
        {
            get => _calibrationStatus;
            set
            {
                _calibrationStatus = value;
                OnPropertyChanged(nameof(CalibrationStatus));
            }
        }

        public SeriesCollection ChartSeries
        {
            get => _chartSeries;
            set
            {
                _chartSeries = value;
                OnPropertyChanged(nameof(ChartSeries));
            }
        }

        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                _isDarkTheme = value;
                OnPropertyChanged(nameof(IsDarkTheme));
            }
        }



        public string[] TimeLabels
        {
            get => _timeLabels;
            set
            {
                _timeLabels = value;
                OnPropertyChanged(nameof(TimeLabels));
            }
        }

        private void InitializeCharts()
        {
            ChartSeries = new SeriesCollection
    {
        new LineSeries
        {
            Title = "Положение",
            Values = new ChartValues<double>(),
            PointGeometry = null,
            LineSmoothness = 0
        }
    };

            TimeLabels = new string[0];
        }

        public AlarmSettings AlarmSettings
        {
            get => _alarmSettings;
            set
            {
                _alarmSettings = value;
                OnPropertyChanged(nameof(AlarmSettings));
            }
        }

        public CalibrationSettings CalibrationSettings
        {
            get => _calibrationSettings;
            set
            {
                _calibrationSettings = value;
                OnPropertyChanged(nameof(CalibrationSettings));
            }
        }

        public TestSettings TestSettings
        {
            get => _testSettings;
            set
            {
                _testSettings = value;
                OnPropertyChanged(nameof(TestSettings));
            }
        }

        public InterfaceSettings InterfaceSettings
        {
            get => _interfaceSettings;
            set
            {
                _interfaceSettings = value;
                OnPropertyChanged(nameof(InterfaceSettings));
            }
        }

        public string ConnectionStatus
        {
            get => _connectionStatus;
            set
            {
                _connectionStatus = value;
                OnPropertyChanged(nameof(ConnectionStatus));
            }
        }

        public string LastUpdateTime
        {
            get => _lastUpdateTime;
            set
            {
                _lastUpdateTime = value;
                OnPropertyChanged(nameof(LastUpdateTime));
            }
        }

        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                _isConnected = value;
                OnPropertyChanged(nameof(IsConnected));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string SelectedPort
        {
            get => _selectedPort;
            set
            {
                _selectedPort = value;
                OnPropertyChanged(nameof(SelectedPort));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public ObservableCollection<string> AvailablePorts { get; } = new ObservableCollection<string>();
        public ISnackbarMessageQueue NotificationQueue => _notificationQueue;

        private bool CanConnect() => !IsConnected && !string.IsNullOrEmpty(SelectedPort);
        private bool CanDisconnect() => IsConnected;
        private bool CanExecuteCommand() => IsConnected;

        private void Connect()
        {
            try
            {
                IsConnected = _modbusService.Connect(SelectedPort);
                if (IsConnected)
                {
                    ConnectionStatus = $"Подключено к {SelectedPort}";
                    Logger.Info($"Успешное подключение к {SelectedPort}");
                    ShowNotification("Успешное подключение");
                }
                else
                {
                    ConnectionStatus = "Ошибка подключения";
                    Logger.Error($"Не удалось подключиться к {SelectedPort}");
                    ShowNotification("Ошибка подключения", true);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка при подключении");
                ShowNotification($"Ошибка: {ex.Message}", true);
                IsConnected = false;
                ConnectionStatus = "Ошибка подключения";
            }
        }

        private void Disconnect()
        {
            try
            {
                _modbusService.Disconnect();
                IsConnected = false;
                ConnectionStatus = "Отключено";
                Logger.Info("Успешное отключение");
                ShowNotification("Устройство отключено");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка при отключении");
                ShowNotification($"Ошибка отключения: {ex.Message}", true);
            }
        }

        private void RefreshPorts()
        {
            try
            {
                AvailablePorts.Clear();
                foreach (var port in SerialPort.GetPortNames())
                {
                    AvailablePorts.Add(port);
                }
                Logger.Debug($"Обновлены доступные порты: {string.Join(", ", AvailablePorts)}");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка при обновлении списка портов");
                ShowNotification("Ошибка обновления портов", true);
            }
        }

        private void ToggleTheme()
        {
            _themeService.ToggleTheme();
            OnPropertyChanged(nameof(IsDarkTheme)); // Важно: уведомляем об изменении
        }

        // Команда "Открыть"
        private void ExecuteOpen()
        {
            ExecuteCommand("Команда на движение", 3, "Открытие");
        }

        // Команда "Закрыть"
        private void ExecuteClose()
        {
            ExecuteCommand("Команда на движение", 2, "Закрытие");
        }

        // Команда "Стоп"
        private void ExecuteStop()
        {
            ExecuteCommand("Команда на движение", 1, "Остановка");
        }

        // Команда "Переместить"
        private void ExecuteMove()
        {
            try
            {
                if (!IsConnected)
                {
                    ShowNotification("Устройство не подключено", true);
                    return;
                }

                var positionParam = _excelService.FindParameterByCode("Заданное положение");
                var commandParam = _excelService.FindParameterByCode("Команда на движение");

                if (positionParam == null || commandParam == null)
                {
                    ShowNotification("Не найдены параметры управления в Excel", true);
                    return;
                }

                ushort positionValue = (ushort)(TestSettings.PartialMovePosition * 10);
                _modbusService.WriteSingleRegister(1, positionParam.Address, positionValue);
                _modbusService.WriteSingleRegister(1, commandParam.Address, 6); // 6 = ПЕРЕМЕСТ

                DeviceStatus.Status = $"Перемещение в {TestSettings.PartialMovePosition}%";
                ShowNotification($"Перемещение в {TestSettings.PartialMovePosition}%");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка команды 'Переместить'");
                ShowNotification($"Ошибка: {ex.Message}", true);
            }
        }

        // Команда "Начать тест"
        private void StartTest()
        {
            ExecuteCommand("Включение тестового прогона", 2, "Начало теста");
        }

        // Команда "Остановить тест"
        private void StopTest()
        {
            ExecuteCommand("Включение тестового прогона", 1, "Остановка теста");
        }

        // Команда "Выставить ЗАКРЫТО"
        private void CalibrateClosePosition()
        {
            ExecuteCommand("Команда на калибровку", 4, "Калибровка закрытого положения");
        }

        // Команда "Выставить ОТКРЫТО"
        private void CalibrateOpenPosition()
        {
            ExecuteCommand("Команда на калибровку", 5, "Калибровка открытого положения");
        }

        // Команда "Сброс аварий"
        private void ResetAlarms()
        {
            ExecuteCommand("Команда на движение", 5, "Сброс аварий");
        }

        // Команда "Тест частичным ходом"
        private void ExecutePartialMoveTest()
        {
            ExecuteCommand("Команда на движение", 4, "Тест частичным ходом");
        }

        // Общий метод для выполнения команд
        private void ExecuteCommand(string parameterIdentifier, ushort commandValue, string commandName)
        {
            try
            {
                if (!IsConnected)
                {
                    ShowNotification("Устройство не подключено", true);
                    return;
                }

                var parameter = _excelService.FindParameterByCode(parameterIdentifier);
                if (parameter == null)
                {
                    ShowNotification($"Не найден параметр управления: {parameterIdentifier}", true);
                    return;
                }

                _modbusService.WriteSingleRegister(1, parameter.Address, commandValue);
                DeviceStatus.Status = commandName;
                Logger.Info($"Команда '{commandName}' отправлена на адрес {parameter.Address}");
                ShowNotification($"Команда '{commandName}' выполнена");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Ошибка команды '{commandName}'");
                ShowNotification($"Ошибка: {ex.Message}", true);
            }
        }



        // Поиск параметра по коду
        private VariableMap FindParameterByCode(string code)
        {
            // Удаляем возможные пробелы и лишние символы
            code = code.Trim();

            // Ищем в загруженных параметрах
            foreach (var param in _excelService.GetAllParameters())
            {
                if (param.Code?.Trim() == code)
                {
                    return param;
                }
            }

            return null;
        }

        private void SaveSettings()
        {
            try
            {
                var settings = new ApplicationSettings
                {
                    AlarmSettings = AlarmSettings,
                    CalibrationSettings = CalibrationSettings,
                    TestSettings = TestSettings,
                    InterfaceSettings = InterfaceSettings
                };

                _settingsService.SaveSettings(settings);
                Logger.Info("Настройки сохранены");
                ShowNotification("Настройки сохранены");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка сохранения настроек");
                ShowNotification($"Ошибка сохранения: {ex.Message}", true);
            }
        }

        private void LoadSettings()
        {
            try
            {
                var settings = _settingsService.LoadSettings();
                AlarmSettings = settings.AlarmSettings;
                CalibrationSettings = settings.CalibrationSettings;
                TestSettings = settings.TestSettings;
                InterfaceSettings = settings.InterfaceSettings;

                Logger.Info("Настройки загружены");
                ShowNotification("Настройки загружены");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка загрузки настроек");
                ShowNotification($"Ошибка загрузки: {ex.Message}", true);
            }
        }

        public string SelectedExcelFile
        {
            get => _selectedExcelFile;
            set
            {
                _selectedExcelFile = value;
                OnPropertyChanged(nameof(SelectedExcelFile));
                LoadExcelFile();
            }
        }

        private void LoadExcel()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Excel Files|*.xls;*.xlsx;*.xlsm",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                SelectedExcelFile = openFileDialog.FileName;
                LoadExcelFile(); // Вызываем без параметра
            }
        }

        private void LoadExcelFile()
        {
            if (string.IsNullOrEmpty(SelectedExcelFile))
            {
                ShowNotification("Файл не выбран", true);
                return;
            }

            try
            {
                var importedMaps = _excelService.ImportVariableMap(SelectedExcelFile);

                // Проверяем наличие обязательных параметров
                var requiredParams = new Dictionary<string, string>
        {
            {"Команда на движение", "D0.0 или 3.0.0"},
            {"Заданное положение", "D0.2 или 3.0.2"},
            {"Включение тестового прогона", "D1.0 или 3.1.0"},
            {"Команда на калибровку", "D0.1 или 3.0.1"}
        };

                var missingParams = new List<string>();
                foreach (var param in requiredParams)
                {
                    if (_excelService.FindParameterByCode(param.Key) == null)
                    {
                        missingParams.Add($"{param.Key} ({param.Value})");
                    }
                }

                if (missingParams.Any())
                {
                    ShowNotification($"Не найдены параметры: {string.Join(", ", missingParams)}", true);
                }
                else
                {
                    ShowNotification("Excel файл успешно загружен, все параметры найдены");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка загрузки Excel файла");
                ShowNotification($"Ошибка загрузки Excel: {ex.Message}", true);
            }
        }

        private async Task UpdateDeviceData()
        {
            if (!IsConnected) return;

            try
            {
                var positionMap = _excelService.GetVariableMap("A0.10 Положение 0-1000");
                if (positionMap == null) return;

                var positionRegister = await Task.Run(() =>
                    _modbusService.ReadHoldingRegisters(1, positionMap.Address, 1));

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    // Обновляем данные графика
                    if (ChartSeries[0].Values.Count >= 100)
                    {
                        ChartSeries[0].Values.RemoveAt(0);
                    }
                    ChartSeries[0].Values.Add(positionRegister[0] / 10.0);

                    // Обновляем метки времени
                    var now = DateTime.Now.ToString("HH:mm:ss");
                    if (TimeLabels.Length >= 100)
                    {
                        TimeLabels = TimeLabels.Skip(1).Concat(new[] { now }).ToArray();
                    }
                    else
                    {
                        TimeLabels = TimeLabels.Concat(new[] { now }).ToArray();
                    }

                    OnPropertyChanged(nameof(TimeLabels));
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка обновления данных устройства");
                Disconnect();
            }
        }

        private string GetStatusDescription(System.Collections.BitArray statusBits)
        {
            var statusDescriptions = new List<string>();

            // Из Excel: A0.3 Статус блока
            if (statusBits[0]) statusDescriptions.Add("Авария");
            if (statusBits[1]) statusDescriptions.Add("ШИМ вкл");
            if (statusBits[2]) statusDescriptions.Add("Упор");
            if (statusBits[3]) statusDescriptions.Add("Нагрев");
            if (statusBits[4]) statusDescriptions.Add("Нет питания");
            if (statusBits[5]) statusDescriptions.Add("Связь MB");
            if (statusBits[6]) statusDescriptions.Add("CAN PDO");
            if (statusBits[7]) statusDescriptions.Add("CAN SDO");
            if (statusBits[8]) statusDescriptions.Add("Закрытие");
            if (statusBits[9]) statusDescriptions.Add("Открытие");
            if (statusBits[10]) statusDescriptions.Add("Готов");
            if (statusBits[11]) statusDescriptions.Add("Многооборотный");
            if (statusBits[12]) statusDescriptions.Add("Неполноповоротный");
            if (statusBits[13]) statusDescriptions.Add("Линейный");
            if (statusBits[14]) statusDescriptions.Add("ТЧХ пр");
            if (statusBits[15]) statusDescriptions.Add("ТЧХ непр");

            return statusDescriptions.Count > 0 ? string.Join(", ", statusDescriptions) : "Норма";
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

        private void ShowNotification(string message, bool isError = false)
        {
            _notificationQueue.Enqueue(message, null, null, null, false, !isError, TimeSpan.FromSeconds(3));
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ushort GetRegisterAddress(string parameterName)
        {
            var map = _excelService.GetVariableMap(parameterName);
            return map?.Address ?? 0;
        }
    }
}
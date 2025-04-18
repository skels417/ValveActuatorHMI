using System.Windows;
using ValveActuatorHMI.Data;
using ValveActuatorHMI.Services;
using ValveActuatorHMI.ViewModels;

namespace ValveActuatorHMI
{
    public partial class App : Application
    {
        public static IModbusService ModbusService { get; private set; }
        public static ILoggingService LoggingService { get; private set; }
        public static IExcelService ExcelService { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize services
            var modbusService = new ModbusService();
            var excelService = new ExcelService();

            // Initialize main window
            var mainWindow = new MainWindow();
            mainWindow.DataContext = new MainViewModel(modbusService, excelService);
            mainWindow.Show();
        
    }
    }
}
using System.Windows;
using ValveActuatorHMI.Services;
using ValveActuatorHMI.ViewModels;

namespace ValveActuatorHMI
{
    public partial class App : Application
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ConfigureLogger();

            var modbusService = new ModbusService();
            var settingsService = new SettingsService();
            var excelService = new ExcelService(); // Важно создать экземпляр ExcelService

            var mainViewModel = new MainViewModel(modbusService, settingsService, excelService);
            var mainWindow = new MainWindow { DataContext = mainViewModel };
            mainWindow.Show();

            Logger.Info("Приложение запущено");
        }

        private void ConfigureLogger()
        {
            var config = new NLog.Config.LoggingConfiguration();
            var fileTarget = new NLog.Targets.FileTarget("file")
            {
                FileName = "${basedir}/logs/app.log",
                Layout = "${longdate} ${level:uppercase=true} ${message} ${exception:format=ToString}"
            };
            config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, fileTarget);
            NLog.LogManager.Configuration = config;
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Logger.Info("Приложение завершает работу");
            NLog.LogManager.Shutdown();
        }
    }
}
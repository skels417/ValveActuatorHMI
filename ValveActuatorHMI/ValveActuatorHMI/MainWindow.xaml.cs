using System.Windows;
using ValveActuatorHMI.ViewModels;

namespace ValveActuatorHMI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext is MainViewModel viewModel && viewModel.IsConnected)
            {
                var result = MessageBox.Show(
                    "Устройство подключено. Закрыть приложение?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                e.Cancel = result == MessageBoxResult.No;
            }
        }
    }
}
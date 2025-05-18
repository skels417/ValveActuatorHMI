using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using ValveActuatorHMI.Services;

namespace ValveActuatorHMI.ViewModels
{
    public class ExcelFilesViewModel : INotifyPropertyChanged
    {
        private readonly IExcelService _excelService;
        private string _selectedExcelFile;

        public ExcelFilesViewModel(IExcelService excelService)
        {
            _excelService = excelService;
            InitializeCommands();
            RefreshExcelFiles();
        }

        public string SelectedExcelFile
        {
            get => _selectedExcelFile;
            set
            {
                _selectedExcelFile = value;
                OnPropertyChanged(nameof(SelectedExcelFile));
            }
        }

        public ObservableCollection<string> ExcelFiles { get; } = new ObservableCollection<string>();

        public ICommand AddExcelFileCommand { get; private set; }
        public ICommand RemoveExcelFileCommand { get; private set; }
        public ICommand RefreshExcelFilesCommand { get; private set; }

        private void InitializeCommands()
        {
            AddExcelFileCommand = new RelayCommand(AddExcelFile);
            RemoveExcelFileCommand = new RelayCommand(RemoveExcelFile, CanRemoveExcelFile);
            RefreshExcelFilesCommand = new RelayCommand(RefreshExcelFiles);
        }

        private void AddExcelFile()
        {
            string filePath = _excelService.SelectExcelFile();
            if (!string.IsNullOrEmpty(filePath))
            {
                _excelService.AddExcelFile(filePath);
                RefreshExcelFiles();
            }
        }

        private void RemoveExcelFile()
        {
            if (!string.IsNullOrEmpty(SelectedExcelFile))
            {
                _excelService.RemoveExcelFile(SelectedExcelFile);
                RefreshExcelFiles();
            }
        }

        private bool CanRemoveExcelFile()
        {
            return !string.IsNullOrEmpty(SelectedExcelFile);
        }

        private void RefreshExcelFiles()
        {
            ExcelFiles.Clear();
            foreach (var file in _excelService.GetAvailableExcelFiles())
            {
                ExcelFiles.Add(file);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
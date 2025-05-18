using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

public class AsyncCommand : ICommand
{
    private readonly Func<Task> _execute;
    private readonly Func<bool> _canExecute;

    public AsyncCommand(Func<Task> execute, Func<bool> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

    public async void Execute(object parameter)
    {
        await ExecuteAsync();
    }

    private async Task ExecuteAsync()
    {
        try
        {
            await _execute();
        }
        catch (Exception ex)
        {
            // Логирование ошибки
            Debug.WriteLine($"Command error: {ex}");
        }
    }

    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }
}
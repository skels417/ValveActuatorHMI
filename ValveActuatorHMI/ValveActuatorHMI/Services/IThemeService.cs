using MaterialDesignThemes.Wpf;

namespace ValveActuatorHMI.Services
{
    public interface IThemeService
    {
        void ToggleTheme();
        bool IsDarkTheme { get; }
    }
}
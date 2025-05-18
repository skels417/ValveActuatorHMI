using MaterialDesignThemes.Wpf;

namespace ValveActuatorHMI.Services
{
    public class ThemeService : IThemeService
    {
        private readonly PaletteHelper _paletteHelper = new PaletteHelper();
        private bool _isDarkTheme;

        public ThemeService()
        {
            _isDarkTheme = _paletteHelper.GetTheme().GetBaseTheme() == BaseTheme.Dark;
        }

        public bool IsDarkTheme => _isDarkTheme;

        public void ToggleTheme()
        {
            ITheme theme = _paletteHelper.GetTheme();

            if (_isDarkTheme)
            {
                theme.SetBaseTheme(new MaterialDesignLightTheme());
            }
            else
            {
                theme.SetBaseTheme(new MaterialDesignDarkTheme());
            }

            _paletteHelper.SetTheme(theme);
            _isDarkTheme = !_isDarkTheme;
        }
    }
}
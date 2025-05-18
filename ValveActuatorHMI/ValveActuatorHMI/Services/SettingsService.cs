using System;
using System.IO;
using Newtonsoft.Json;
using ValveActuatorHMI.Models;

namespace ValveActuatorHMI.Services
{
    public class SettingsService : ISettingsService
    {
        private const string SettingsFileName = "appsettings.json";

        public void SaveSettings(ApplicationSettings settings)
        {
            try
            {
                var json = JsonConvert.SerializeObject(settings, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(SettingsFileName, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка сохранения настроек: {ex.Message}", ex);
            }
        }

        public ApplicationSettings LoadSettings()
        {
            try
            {
                if (!File.Exists(SettingsFileName))
                    return new ApplicationSettings();

                var json = File.ReadAllText(SettingsFileName);
                return JsonConvert.DeserializeObject<ApplicationSettings>(json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка загрузки настроек: {ex.Message}", ex);
            }
        }
    }
}
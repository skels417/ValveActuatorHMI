using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveActuatorHMI.Models;

namespace ValveActuatorHMI.Services
{
    public interface ISettingsService
    {
        void SaveSettings(ApplicationSettings settings);
        ApplicationSettings LoadSettings();
    }
}

using System.Globalization;
using ValveActuatorHMI.Models;

namespace ValveActuatorHMI.Services
{
    public static class Validators
    {
        public static bool ValidateAlarmSettings(AlarmSettings settings, out string error)
        {
            if (settings.VoltageHighLevel <= settings.VoltageLowLevel)
            {
                error = "Верхний уровень напряжения должен быть выше нижнего";
                return false;
            }

            if (settings.OverheatLevel <= settings.OvercoolLevel)
            {
                error = "Уровень перегрева должен быть выше уровня переохлаждения";
                return false;
            }

            error = null;
            return true;
        }

        public static bool ValidateTestSettings(TestSettings settings, out string error)
        {
            if (settings.PartialMovePosition < 0 || settings.PartialMovePosition > 100)
            {
                error = "Положение для перемещения должно быть от 0 до 100%";
                return false;
            }

            if (settings.TimeModeDuration <= 0)
            {
                error = "Длительность теста должна быть положительной";
                return false;
            }

            error = null;
            return true;
        }

        public static bool ValidateNumericInput(string text, out double value)
        {
            if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            {
                return true;
            }

            return false;
        }
    }
}
using System.Collections.Generic;
using ValveActuatorHMI.Models;

namespace ValveActuatorHMI.Services
{
    public interface IExcelService
    {
        List<VariableMap> ImportVariableMap(string filePath);
    }
}
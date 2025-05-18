using System.Collections.Generic;
using ValveActuatorHMI.Models;

namespace ValveActuatorHMI.Services
{
    public interface IExcelService
    {
        // Основные методы работы с Excel
        List<VariableMap> ImportVariableMap(string filePath);
        VariableMap GetVariableMap(string parameterName);

        // Методы для поиска параметров
        VariableMap FindParameterByCode(string code);
        IEnumerable<VariableMap> GetAllParameters();

        // Методы для управления файлами
        void AddExcelFile(string filePath);
        void RemoveExcelFile(string filePath);
        List<string> GetAvailableExcelFiles();
        string SelectExcelFile();
    }
}
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using ValveActuatorHMI.Models;
using System.Linq;

namespace ValveActuatorHMI.Services
{
    public class ExcelService : IExcelService
    {
        private readonly Dictionary<string, VariableMap> _variableMaps = new Dictionary<string, VariableMap>();
        private readonly List<VariableMap> _allParameters = new List<VariableMap>();
        private readonly List<string> _availableExcelFiles = new List<string>();

        public ExcelService()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public List<VariableMap> ImportVariableMap(string filePath)
        {
            _variableMaps.Clear();
            _allParameters.Clear();

            try
            {
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    if (package.Workbook.Worksheets.Count == 0)
                        throw new InvalidOperationException("Excel файл не содержит листов");

                    var worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension?.Rows ?? 0;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        try
                        {
                            var code = worksheet.Cells[row, 1]?.Text?.Trim();
                            var addressText = worksheet.Cells[row, 7]?.Text?.Trim();

                            if (!string.IsNullOrEmpty(code) && ushort.TryParse(addressText, out ushort address))
                            {
                                var variableMap = new VariableMap
                                {
                                    Code = code,
                                    Name = worksheet.Cells[row, 2]?.Text?.Trim(),
                                    Address = address,
                                    DataType = worksheet.Cells[row, 5]?.Text?.Trim(),
                                    Unit = worksheet.Cells[row, 4]?.Text?.Trim()
                                };

                                _variableMaps[code] = variableMap;
                                _allParameters.Add(variableMap);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Ошибка обработки строки {row}: {ex.Message}");
                        }
                    }
                }
                return new List<VariableMap>(_allParameters);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ошибка загрузки Excel файла: {ex.Message}", ex);
            }
        }

        public VariableMap GetVariableMap(string parameterName)
        {
            return _variableMaps.TryGetValue(parameterName, out var map) ? map : null;
        }

        public VariableMap FindParameterByCode(string identifier)
        {
            // Сначала ищем по полному коду (например "3.0.0")
            var param = _allParameters.FirstOrDefault(p => p.Code == identifier);

            // Если не нашли, ищем по частичному совпадению в названии
            if (param == null)
            {
                param = _allParameters.FirstOrDefault(p =>
                    p.Name != null &&
                    p.Name.IndexOf(identifier, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            return param;
        }

        public IEnumerable<VariableMap> GetAllParameters() => _allParameters;

        public void AddExcelFile(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && !_availableExcelFiles.Contains(filePath))
            {
                _availableExcelFiles.Add(filePath);
            }
        }

        public void RemoveExcelFile(string filePath)
        {
            if (_availableExcelFiles.Contains(filePath))
            {
                _availableExcelFiles.Remove(filePath);
            }
        }

        public List<string> GetAvailableExcelFiles()
        {
            return new List<string>(_availableExcelFiles);
        }

        public string SelectExcelFile()
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "Excel Files|*.xls;*.xlsx;*.xlsm",
                    Title = "Выберите Excel файл",
                    Multiselect = false
                };

                return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выборе файла: {ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
                return null;
            }
        }
    }
}
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using ValveActuatorHMI.Models;

namespace ValveActuatorHMI.Services
{
    public class ExcelService : IExcelService
    {
        public ExcelService()
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        }

        public List<VariableMap> ImportVariableMap(string filePath)
        {
            var variableMaps = new List<VariableMap>();

            var fileInfo = new FileInfo(filePath);
            using (var package = new ExcelPackage(fileInfo))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    variableMaps.Add(new VariableMap
                    {
                        Name = worksheet.Cells[row, 1].Text,
                        Address = ushort.Parse(worksheet.Cells[row, 2].Text),
                        DataType = worksheet.Cells[row, 3].Text,
                        Unit = worksheet.Cells[row, 4].Text,
                        ScaleFactor = double.Parse(worksheet.Cells[row, 5].Text),
                        Offset = double.Parse(worksheet.Cells[row, 6].Text)
                    });
                }
            }

            return variableMaps;
        }
    }
}
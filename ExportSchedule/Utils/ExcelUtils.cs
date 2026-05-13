using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
using Range = Microsoft.Office.Interop.Excel.Range;

namespace ADSK.JExtRAC.ExportSchedule.Utils
{
    public class ExcelUtils
    {
        public static Workbook GetExcelWorkBook(bool isAlwaysNewBook = true)
        {
            Application application = new Application();
            application.IgnoreRemoteRequests = true;
            application.Visible = false;

            try
            {
                Workbook workbook = !isAlwaysNewBook ? application.ActiveWorkbook ?? application.Workbooks.Add(Missing.Value) : application.Workbooks.Add(Missing.Value);
                return workbook;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public static void FormatTitleCell(int iRow, int iCol, string title, float width, Worksheet sh)
        {
            Range cell = sh.Cells[iRow, iCol];
            cell.Value = title;
            cell.HorizontalAlignment = Constants.xlCenter;
            cell.VerticalAlignment = Constants.xlCenter;
            cell.Font.Size = (float)10.5;
            cell.Font.ColorIndex = 2;
            cell.Interior.ColorIndex = 54;
            cell.Interior.Pattern = Constants.xlBoth;
            cell.Interior.PatternColorIndex = Constants.xlAutomatic;
            cell.ColumnWidth = width;
        }

        public static string GetExcelColumnName(int columnNumber)
        {
            string columnName = "";
            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }
            return columnName;
        }
    }
}

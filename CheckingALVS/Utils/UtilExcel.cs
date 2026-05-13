using System;
using ExcelApp = Microsoft.Office.Interop.Excel.Application;
using ExcelRange = Microsoft.Office.Interop.Excel.Range;
using Microsoft.Office.Interop.Excel;

namespace ADSK.JExtRAC.CheckingALVS.Utils
{
    /// <summary>Excel automation (replaces DnfCom.UtilExcel).</summary>
    public sealed class UtilExcel
    {
        private readonly ExcelApp _app;
        private Workbook _workbook;
        private Worksheet _worksheet;

        public bool ExistXlsApp { get; private set; }
        public bool FlagNewXlsApp { get; private set; }
        public bool FlagNewXlsbook { get; private set; }

        public int SelectionRowNo { get; private set; } = 1;
        public int SelectionColumnNo { get; private set; } = 1;

        /// <param name="mode">0 = new app, 1 = attempt same (always new app in standalone)</param>
        public UtilExcel(int mode)
        {
            _ = mode;
            _app = new ExcelApp();
            _app.Visible = false;
            _workbook = _app.Workbooks.Add(Type.Missing);
            _worksheet = (Worksheet)_workbook.Sheets[1];
            ExistXlsApp = true;
            FlagNewXlsApp = true;
            FlagNewXlsbook = true;
            SelectionRowNo = 1;
            SelectionColumnNo = 1;
        }

        public bool SetExcelActiveWorkbook()
        {
            try
            {
                _workbook = _app.ActiveWorkbook;
                return _workbook != null;
            }
            catch { return false; }
        }

        public void SetExcelWorkbook(string nameIgnored)
        {
            try
            {
                if (_workbook == null)
                    _workbook = _app.Workbooks.Add(Type.Missing);
            }
            catch { }
        }

        public void SetExcelWorksheet(string nameIgnored)
        {
            try
            {
                _worksheet = (Worksheet)_workbook.Sheets[1];
            }
            catch { }
        }

        public void SetExcelVisible(bool visible)
        {
            try { _app.Visible = visible; } catch { }
        }

        public void SetCellValue(int row1b, int col1b, object value)
        {
            if (_worksheet == null) SetExcelWorksheet(null);
            ExcelRange cell = (ExcelRange)_worksheet.Cells[row1b, col1b];
            cell.Value2 = value;
        }

        public void SetAlignmentHorizontalCell(int row, int col, int horizontalMode)
        {
            try
            {
                if (_worksheet == null) SetExcelWorksheet(null);
                ExcelRange range = row <= 0
                    ? (ExcelRange)_worksheet.Columns[col]
                    : (ExcelRange)_worksheet.Cells[row, col];

                if (horizontalMode == 1) range.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                else if (horizontalMode == 2) range.HorizontalAlignment = XlHAlign.xlHAlignRight;
                else range.HorizontalAlignment = XlHAlign.xlHAlignLeft;
            }
            catch { }
        }

        public void SetWidthCells(int row, int col, double width)
        {
            try
            {
                if (_worksheet == null) SetExcelWorksheet(null);
                ExcelRange rng = row <= 0 ? (ExcelRange)_worksheet.Columns[col] : (ExcelRange)_worksheet.Cells[row, col];
                rng.ColumnWidth = width <= 0 ? 12 : width;
            }
            catch { }
        }

        public void SetNumberFormatCells(int row, int col, string format)
        {
            try
            {
                if (_worksheet == null) SetExcelWorksheet(null);
                ExcelRange rng = row <= 0 ? (ExcelRange)_worksheet.Columns[col] : (ExcelRange)_worksheet.Cells[row, col];
                rng.NumberFormat = format ?? "General";
            }
            catch { }
        }

        public void SetBordersCells(int row, int col, int unusedRow2, int lastColInclusive, bool unusedInner,
            int unused1, int unused2)
        {
            try
            {
                if (_worksheet == null) SetExcelWorksheet(null);
                ExcelRange rng = (ExcelRange)_worksheet.Cells[row, col];
                rng.Cells.Borders[XlBordersIndex.xlEdgeBottom].LineStyle = XlLineStyle.xlContinuous;
            }
            catch { }
        }

        public void CloseExcel()
        {
            try
            {
                if (_workbook != null)
                {
                    _workbook.Close(false);
                    _workbook = null;
                }

                _app.DisplayAlerts = false;
                _app.Quit();
            }
            catch { }

            try { System.Runtime.InteropServices.Marshal.FinalReleaseComObject(_worksheet); } catch { }
            try { System.Runtime.InteropServices.Marshal.FinalReleaseComObject(_app); } catch { }
        }
    }
}

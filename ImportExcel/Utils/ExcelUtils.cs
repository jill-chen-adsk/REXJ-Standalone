using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Office.Interop.Excel;
using Application = Microsoft.Office.Interop.Excel.Application;
using Excel = Microsoft.Office.Interop.Excel;

namespace ADSK.JExtRAC.ImportExcel.Utils
{
    public class ExcelUtils
    {
        public static bool IsEditing(Application excelApp)
        {
            if (excelApp.Interactive == false)
                return false;

            try
            {
                excelApp.Interactive = false;
                excelApp.Interactive = true;
            }
            catch
            {
                return true;
            }

            return false;
        }

        public static List<ExcelInfo> GetAllInstanceExcel()
        {
            List<ExcelInfo> excelInfos = new List<ExcelInfo>();

            try
            {
                Excel.Application app = new Excel.Application();
                EnumChildCallback cb;
                List<Process> procs = new List<Process>();
                procs.AddRange(Process.GetProcessesByName("excel"));

                foreach (Process p in procs)
                {
                    if ((int)p.MainWindowHandle > 0)
                    {
                        int childWindow = 0;
                        cb = new EnumChildCallback(EnumChildProc);
                        EnumChildWindows((int)p.MainWindowHandle, cb, ref childWindow);

                        if (childWindow > 0)
                        {
                            const uint OBJID_NATIVEOM = 0xFFFFFFF0;
                            Guid IID_IDispatch = new Guid("{00020400-0000-0000-C000-000000000046}");
                            Excel.Window window = null;
                            int res = AccessibleObjectFromWindow(childWindow, OBJID_NATIVEOM, IID_IDispatch.ToByteArray(), ref window);
                            if (res >= 0)
                            {
                                app = window.Application;

                                var excelApplication = (Excel.Application)app;
                                for (int i = 0; i < excelApplication.Workbooks.Count; i++)
                                {
                                    object idx = i + 1;

                                    Excel.Workbook workbook = excelApplication.Workbooks[idx];

                                    List<Excel.Worksheet> sheets = new List<Excel.Worksheet>();
                                    foreach (Worksheet worksheet in workbook.Worksheets)
                                    {
                                        sheets.Add(worksheet);
                                    }

                                    if (sheets.Count != 0)
                                    {
                                        ExcelInfo excelInfo = new ExcelInfo(app, workbook, sheets);
                                        excelInfos.Add(excelInfo);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                string mess = ex.Message;
            }

            return excelInfos;
        }

        [DllImport("Oleacc.dll")]
        public static extern int AccessibleObjectFromWindow(
                 int hwnd, uint dwObjectID, byte[] riid,
                 ref Microsoft.Office.Interop.Excel.Window ptr);

        public delegate bool EnumChildCallback(int hwnd, ref int lParam);

        [DllImport("User32.dll")]
        public static extern bool EnumChildWindows(
              int hWndParent, EnumChildCallback lpEnumFunc,
              ref int lParam);

        [DllImport("User32.dll")]
        public static extern int GetClassName(
              int hWnd, StringBuilder lpClassName, int nMaxCount);

        public static bool EnumChildProc(int hwndChild, ref int lParam)
        {
            StringBuilder buf = new StringBuilder(128);
            GetClassName(hwndChild, buf, 128);
            if (buf.ToString() == "EXCEL7")
            {
                lParam = hwndChild;
                return false;
            }
            return true;
        }
    }

    public class ExcelInfo
    {
        public Excel.Application _App = null;
        public Excel.Workbook _Workbook = null;
        public List<Excel.Worksheet> _Worksheets = null;

        public ExcelInfo(Excel.Application app, Excel.Workbook workbook, List<Excel.Worksheet> worksheets)
        {
            _App = app;
            _Workbook = workbook;
            _Worksheets = worksheets;
        }
    }
}

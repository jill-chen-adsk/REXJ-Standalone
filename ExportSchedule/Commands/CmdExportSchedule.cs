using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Reflection;
using Microsoft.Office.Interop.Excel;
using ADSK.JExtRAC.ExportSchedule.Utils;
using System.Runtime.InteropServices;
using RvtExtApp = ADSK.JExtRAC.ExportSchedule;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using ADSK.JExtRAC.ExportSchedule.UserControls;
using Range = Microsoft.Office.Interop.Excel.Range;

namespace ADSK.JExtRAC.ExportSchedule.Commands
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class CmdExportSchedule : IExternalCommand
    {
        private RvtExtApp.Components.Attribute _CmpAttribute = null;
        private System.Text.StringBuilder strLog = new System.Text.StringBuilder();
        private string errMsg = string.Empty;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            CultureHelper.InitializeCulture();
            _CmpAttribute = new RvtExtApp.Components.Attribute();
            var view = commandData.Application.ActiveUIDocument.ActiveView;

            if (view is ViewSchedule == false)
            {
                MessageBox.Show(_CmpAttribute.ResourceText("IDS_INFO_NO_ACTIVE_SCHEDULE"), _CmpAttribute.ResourceText("IDS_TXT_INFORMATION"));
                return Result.Failed;
            }
            Transaction trans = new Transaction(commandData.Application.ActiveUIDocument.Document);

            try
            {
                var schedule = view as ViewSchedule;
                var myUserControl = new MyUserControl(_CmpAttribute);
                myUserControl.Itemized = schedule.Definition.IsItemized;

                string path = string.Empty;

                while (true)
                {
                    CustomSaveFileDialog cSaveDialog = new CustomSaveFileDialog(_CmpAttribute, myUserControl);
                    cSaveDialog._SaveFileDialog.Title = _CmpAttribute.ResourceText("IDS_SAVE_EXCEL");
                    cSaveDialog._SaveFileDialog.Filter = "Excel Workbook | *.xlsx|Excel Files | *.xls";
                    cSaveDialog._SaveFileDialog.CheckPathExists = true;
                    cSaveDialog._SaveFileDialog.RestoreDirectory = true;
                    cSaveDialog._SaveFileDialog.FileName = schedule.Name + myUserControl.GetCurrentDateTime();
                    myUserControl._CustomSaveFileDialog = cSaveDialog;

                    DialogResult res = cSaveDialog._SaveFileDialog.ShowDialog();
                    if (res != System.Windows.Forms.DialogResult.OK)
                        return Result.Cancelled;

                    path = cSaveDialog._SaveFileDialog.FileName;

                    if (File.Exists(path) == true)
                    {
                        try
                        {
                            File.Delete(path);
                            break;
                        }
                        catch (System.Exception ex)
                        {
                            string mess = ex.Message;
                            MessageBox.Show(_CmpAttribute.ResourceText("IDS_INFO_CANNOT_REMOVE_EXIST"), _CmpAttribute.ResourceText("IDS_TXT_INFORMATION"));
                        }
                    }
                    else
                        break;
                }

                if (path == string.Empty)
                {
                    MessageBox.Show(_CmpAttribute.ResourceText("IDS_INFO_PATH_EMPTY"), _CmpAttribute.ResourceText("IDS_TXT_INFORMATION"));
                    return Result.Cancelled;
                }

                trans.Start("ExportSchedule");
                if (schedule.Definition.IsKeySchedule == false)
                    schedule.Definition.IsItemized = myUserControl.Itemized;

                Export(commandData.Application.Application, commandData.Application.ActiveUIDocument, schedule, path, myUserControl.ForImport);
                trans.RollBack();
            }
            catch (System.Exception ex)
            {
                string mess = ex.Message;
                strLog.AppendLine("-----------------------");
                strLog.AppendLine(_CmpAttribute.ResourceText("IDS_ERROR_CANNOT_EXPORT_SCHEDULE"));
                strLog.AppendLine(mess);
                strLog.AppendLine("-----------------------");
                if (strLog.Length != 0)
                {
                    RvtExtApp.UI.FormLog frmLog = new RvtExtApp.UI.FormLog(_CmpAttribute, strLog);
                    frmLog.ShowDialog();
                }
                if (trans.HasStarted() == true)
                    trans.RollBack();
            }
            return Result.Cancelled;
        }

        public Result Export(Autodesk.Revit.ApplicationServices.Application revitApp, UIDocument uiDoc, ViewSchedule schedule, string pathExcel, bool forImport)
        {
            ScheduleExporter scheduleExporter = null;
            try
            {
                Workbook workbook = ExcelUtils.GetExcelWorkBook(true);
                if (workbook == null)
                    return Result.Cancelled;

                scheduleExporter = new ScheduleExporter(_CmpAttribute, revitApp, uiDoc, schedule, forImport);

                string warningMess = string.Empty;
                if (scheduleExporter._GroupByFamilyType == false && schedule.Definition.IsItemized == false)
                    warningMess = _CmpAttribute.ResourceText("IDS_EXPORT_WARNINGFAMILYANDTYPE");
                if (schedule.Definition.ShowHeaders == false && forImport == false)
                {
                    if (string.IsNullOrEmpty(warningMess))
                        warningMess = _CmpAttribute.ResourceText("IDS_EXPORT_WARNINGSHOWHEADER");
                    else
                        warningMess = _CmpAttribute.ResourceText("IDS_EXPORT_WARNING_FAMILYANDTYPE_SHOWHEADER");
                }

                if (string.IsNullOrEmpty(warningMess) == false)
                    if (DialogResult.Cancel == MessageBox.Show(warningMess, _CmpAttribute.ResourceText("IDS_TXT_WARNING"), MessageBoxButtons.OKCancel))
                        return Result.Cancelled;

                var application = workbook.Application;
                var worksheets = workbook.Sheets as Sheets;
                bool existSheet1 = false;
                if (worksheets.Count == 1)
                    existSheet1 = true;

                Worksheet worksheet_all = (Worksheet)workbook.Worksheets.Add(After: workbook.Sheets[workbook.Sheets.Count]);
                worksheet_all.Name = scheduleExporter._SheetScheduleName;
                scheduleExporter.ExportViewSchedule(uiDoc.Document, schedule, worksheet_all, forImport, ScheduleExporter.UIDFlagExport.None, out bool isHasError);
                if (isHasError)
                    return Result.Failed;

                Range excelRange = worksheet_all.Range["B:B"];
                try { excelRange.Delete(Type.Missing); }
                finally { if (null != excelRange) Marshal.ReleaseComObject(excelRange); }
                worksheet_all.Columns.EntireColumn.AutoFit();

                Worksheet worksheet_type = (Worksheet)workbook.Worksheets.Add(After: workbook.Sheets[workbook.Sheets.Count]);
                worksheet_type.Name = scheduleExporter._SheetTypeName;
                scheduleExporter.ExportViewSchedule(uiDoc.Document, schedule, worksheet_type, forImport, ScheduleExporter.UIDFlagExport.Type, out bool ishasError);
                if (ishasError)
                    return Result.Failed;
                worksheet_type.Columns.EntireColumn.AutoFit();

                Worksheet worksheet_instance = (Worksheet)workbook.Worksheets.Add(After: workbook.Sheets[workbook.Sheets.Count]);
                worksheet_instance.Name = scheduleExporter._SheetInstanceName;
                scheduleExporter.ExportViewSchedule(uiDoc.Document, schedule, worksheet_instance, forImport, ScheduleExporter.UIDFlagExport.Instance, out bool IshasError);
                if (IshasError)
                    return Result.Failed;
                worksheet_instance.Columns.EntireColumn.AutoFit();

                if (existSheet1)
                    ((Worksheet)worksheets[1]).Delete();

                worksheet_all.Select(Type.Missing);

                string ext = Path.GetExtension(pathExcel);
                if (ext.ToUpper() == ".XLS")
                    workbook.SaveAs(pathExcel, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
                else
                    workbook.SaveAs(pathExcel, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook);

                workbook.Close(false);
                application.IgnoreRemoteRequests = false;
                application.Quit();

                if (worksheet_all != null) Marshal.ReleaseComObject(worksheet_all);
                if (worksheet_type != null) Marshal.ReleaseComObject(worksheet_type);
                if (worksheet_instance != null) Marshal.ReleaseComObject(worksheet_instance);
                if (worksheets != null) Marshal.ReleaseComObject(worksheets);
                if (workbook != null) Marshal.ReleaseComObject(workbook);
                if (application != null) Marshal.ReleaseComObject(application);

                MessageBox.Show(_CmpAttribute.ResourceText("IDS_EXPORT_SUCCESS"), _CmpAttribute.ResourceText("IDS_TXT_INFORMATION"));
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                strLog.AppendLine("-----------------------");
                strLog.AppendLine(_CmpAttribute.ResourceText("IDS_ERROR_CANNOT_EXPORT_SCHEDULE"));
                strLog.AppendLine(errMsg);
                strLog.AppendLine("-----------------------");
                if (strLog.Length != 0)
                {
                    RvtExtApp.UI.FormLog frmLog = new RvtExtApp.UI.FormLog(_CmpAttribute, strLog);
                    frmLog.ShowDialog();
                }
                return Result.Failed;
            }
        }
    }
}

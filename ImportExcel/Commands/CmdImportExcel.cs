using ADSK.JExtRAC.ImportExcel.Utils;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Office.Interop.Excel;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Application = Microsoft.Office.Interop.Excel.Application;
using Constants = Microsoft.Office.Interop.Excel.Constants;
using Parameter = Autodesk.Revit.DB.Parameter;
using RvtExtApp = ADSK.JExtRAC.ImportExcel;
using ADSK.JExtRAC.ImportExcel.UI;

namespace ADSK.JExtRAC.ImportExcel.Commands
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class CmdImportExcel : IExternalCommand
    {
        #region Member Functions

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            CultureHelper.InitializeCulture();

            var cmpAttribute = new RvtExtApp.Components.Attribute();
            var cmpParameters = new RvtExtApp.Components.Parameters(commandData.Application.ActiveUIDocument);

            var excelInfos = ExcelUtils.GetAllInstanceExcel();

            Application application = null;
            Workbook activeWorkbook = null;
            Worksheet activeSheet = null;
            var showForm = true;
            var isBackGround = false;

            if (excelInfos == null || excelInfos.Count == 0)
            {
                (application, activeWorkbook, activeSheet) = OpenExcelFileDialog(cmpAttribute);
                showForm = false;
                if (application is null)
                {
                    MessageBox.Show(cmpAttribute.ResourceText("IDS_ERROR_NO_FILE_SELECTED"), cmpAttribute.ResourceText("IDS_TXT_ERROR"));
                    return Result.Failed;
                }
                else
                {
                    isBackGround = true;
                }
            }

            if (excelInfos?.Count == 1 && application == null)
            {
                var first = excelInfos[0];
                if (first._Worksheets.Count == 1)
                {
                    application = first._App;
                    activeWorkbook = first._Workbook;
                    activeSheet = first._Worksheets[0];
                    showForm = false;
                }
            }
            if (showForm)
            {
                var form = new SelectExcelForm(excelInfos, cmpAttribute);
                if (form.ShowDialog() != DialogResult.OK)
                    return Result.Failed;

                activeSheet = form.SheetSelected;
                activeWorkbook = (Workbook)activeSheet.Parent;
                application = activeWorkbook.Application;
            }

            if (ExcelUtils.IsEditing(application))
            {
                MessageBox.Show(cmpAttribute.ResourceText("IDS_ERROR_EXCEL_IS_EDITING"), cmpAttribute.ResourceText("IDS_TXT_ERROR"));
                return Result.Failed;
            }

            if (activeWorkbook == null)
            {
                MessageBox.Show(cmpAttribute.ResourceText("IDS_ERROR_NO_ACTIVE_WORKBOOK"), cmpAttribute.ResourceText("IDS_TXT_ERROR"));
                return Result.Failed;
            }

            if (activeSheet == null)
            {
                MessageBox.Show(cmpAttribute.ResourceText("IDS_ERROR_NO_ACTIVE_SHEET"), cmpAttribute.ResourceText("IDS_TXT_ERROR"));
                return Result.Failed;
            }
            else
            {
                Dictionary<string, int> parameterNames = null;
                var startRowData = 0;

                var result = GetInformation(activeSheet, ref message, out parameterNames, out startRowData);
                if (result == false || parameterNames == null || parameterNames.Count == 0)
                {
                    MessageBox.Show(cmpAttribute.ResourceText("IDS_ERROR_TITLE_COLUMN"), cmpAttribute.ResourceText("IDS_TXT_ERROR"));
                    return Result.Cancelled;
                }

                if (activeSheet.UsedRange == null)
                {
                    return Result.Failed;
                }

                int startRow = activeSheet.UsedRange.Row;
                int endRow = startRow + activeSheet.UsedRange.Rows.Count;

                int startCol = activeSheet.UsedRange.Column;

                Document document = commandData.Application.ActiveUIDocument.Document;

                List<Element> elementList = new List<Element>();

                string pseudoCount = Resources.Text.IDS_PSEUDO_COUNT;

                using (Transaction transaction = new Transaction(document, "ImportExcel"))
                {
                    transaction.Start();

                    try
                    {
                        for (int row = startRowData; row <= endRow; row++)
                        {
                            var cell = activeSheet.Cells[row, startCol];
                            var value = cell.Value;

                            if (value == null)
                                continue;

                            var strValue = value.ToString().Trim();
                            if (strValue == string.Empty)
                                continue;

                            string guid = strValue;

                            var element = document.GetElement(guid);
                            if (element == null)
                                continue;
                            else if (element.GroupId != ElementId.InvalidElementId)
                            {
                                elementList.Add(element);
                                continue;
                            }

                            try
                            {
                                foreach (KeyValuePair<string, int> keyPair in parameterNames)
                                {
                                    string parameterName = keyPair.Key;
                                    if (keyPair.Key.StartsWith("T:") || keyPair.Key.StartsWith("I:"))
                                        parameterName = parameterName.Remove(0, 2);
                                    else if (keyPair.Key == pseudoCount || keyPair.Key == "個数")
                                    {
                                        continue;
                                    }

                                    Parameter parameter;
                                    var parameters = element.GetParameters(parameterName);

                                    if (parameters.Count > 0)
                                    {
                                        parameter = parameters.FirstOrDefault(x => !x.IsReadOnly);
                                    }
                                    else
                                    {
                                        parameter = cmpParameters.GetParameter(element, parameterName, null, new ForgeTypeId(string.Empty));
                                    }

                                    if (parameter == null || parameter.IsReadOnly ||
                                        parameter.StorageType == StorageType.ElementId || parameter.StorageType == StorageType.None)
                                        continue;

                                    cell = activeSheet.Cells[row, keyPair.Value];
                                    value = cell.Value;

                                    strValue = string.Empty;
                                    if (value != null)
                                    {
                                        strValue = value.ToString().Trim();
                                    }

                                    var setColor = false;
                                    try
                                    {
                                        if (parameter.StorageType == StorageType.String)
                                        {
                                            if (parameter.Set(strValue))
                                                setColor = true;
                                        }
                                        else if (parameter.StorageType == StorageType.Double)
                                        {
                                            if (parameter.SetValueString(strValue))
                                                setColor = true;
                                        }
                                        else if (parameter.StorageType == StorageType.Integer)
                                        {
                                            if (int.TryParse(strValue, out int intValue))
                                            {
                                                if (parameter.Set(intValue))
                                                    setColor = true;
                                            }
                                        }
                                        else
                                            continue;

                                        if (setColor)
                                        {
                                            cell.Interior.ColorIndex = 15;
                                            cell.Interior.Pattern = Constants.xlBoth;
                                        }
                                    }
                                    catch (System.Exception ex)
                                    {
                                        string mess = ex.Message;
                                    }
                                }
                            }
                            catch (System.Exception ex)
                            {
                                string mess = ex.Message;
                            }
                        }

                        document.Regenerate();
                        transaction.Commit();
                    }
                    catch (System.Exception ex)
                    {
                        CloseSheetIfIsBackground();
                        var mess = ex.Message;
                        transaction.RollBack();
                        return Result.Failed;
                    }
                    if (elementList.Count == 0)
                    {
                        var resIdStr = isBackGround ? "IDS_INFO_IMPORT_EXCEL_BACKGROUND_SUCCESS" : "IDS_INFO_IMPORT_EXCEL_SUCCESS";
                        MessageBox.Show(cmpAttribute.ResourceText(resIdStr), cmpAttribute.ResourceText("IDS_TXT_INFORMATION"));
                    }
                    else
                    {
                        var resIdStr = isBackGround ? "IDS_WARNING_IMPORT_EXCEL_BACKGROUND_GROUP" : "IDS_WARNING_IMPORT_EXCEL_GROUP";
                        var mess = string.Format(cmpAttribute.ResourceText(resIdStr), elementList.Count.ToString());
                        MessageBox.Show(mess, cmpAttribute.ResourceText("IDS_TXT_INFORMATION"));
                    }

                    CloseSheetIfIsBackground();

                    return Result.Succeeded;

                    void CloseSheetIfIsBackground()
                    {
                        if (isBackGround)
                        {
                            activeWorkbook.Close();
                            application.Quit();
                            Marshal.ReleaseComObject(activeSheet);
                            Marshal.ReleaseComObject(activeWorkbook);
                            Marshal.ReleaseComObject(application);
                        }
                    }
                }
            }
        }

        private static (Application, Workbook, Worksheet) OpenExcelFileDialog(RvtExtApp.Components.Attribute cmpAttribute)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Title = cmpAttribute.ResourceText("IDS_TXT_LOAD_FILE");
            openFileDialog.Filter = cmpAttribute.ResourceText("IDS_FILETYPES");

            var result = openFileDialog.ShowDialog();
            var filePath = openFileDialog.FileName;

            openFileDialog.Dispose();
            if (result != DialogResult.OK) return (null, null, null);

            var excelApp = new Application();
            var workbook = excelApp.Workbooks.Open(filePath);
            var workSheet = (Worksheet)workbook.Sheets[1];
            excelApp.DisplayAlerts = false;

            return (excelApp, workbook, workSheet);
        }

        private bool GetInformation(Worksheet activeSheet, ref string message, out Dictionary<string, int> parameterNames, out int startRowData)
        {
            parameterNames = null;
            startRowData = 0;

            if (activeSheet.UsedRange == null)
                return false;

            parameterNames = new Dictionary<string, int>();

            int startRow = activeSheet.UsedRange.Row;

            int startCol = activeSheet.UsedRange.Column;
            int endCol = startCol + activeSheet.UsedRange.Columns.Count;

            try
            {
                bool flag = false;
                int endRowParameter = startRow + 2;

                for (int current = startRow; current <= endRowParameter; current++)
                {
                    for (int beginCol = 1; beginCol <= endCol; beginCol++)
                    {
                        var cell = activeSheet.Cells[current, beginCol];
                        var value = cell.Value;

                        if (value == null)
                            continue;

                        var strValue = value.ToString().Trim();

                        if (strValue == "UID" || strValue == "I:UID" || strValue == "T:UID")
                        {
                            if (cell.MergeCells)
                            {
                                var top = cell.MergeArea.Row;
                                endRowParameter = top + cell.MergeArea.Rows.Count - 1;
                            }
                            else
                            {
                                endRowParameter = current;
                            }
                            flag = true;
                            continue;
                        }

                        if (strValue != string.Empty && flag)
                        {
                            {
                                if (strValue.StartsWith("T:") || strValue.StartsWith("I:"))
                                    if (parameterNames.ContainsKey(strValue) == false)
                                        parameterNames.Add(strValue, beginCol);
                            }
                        }
                    }
                }

                startRowData = endRowParameter + 1;
                return true;
            }
            catch (System.Exception ex)
            {
                var mess = ex.Message;

                return false;
            }
        }

        #endregion Member Functions
    }
}

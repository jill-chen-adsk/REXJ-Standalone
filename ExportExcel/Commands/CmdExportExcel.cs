using ADSK.JExtRAC.ExportExcel.Entities;
using ADSK.JExtRAC.ExportExcel.UI;
using ADSK.JExtRAC.ExportExcel.Utils;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using RvtExtApp = ADSK.JExtRAC.ExportExcel;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace ADSK.JExtRAC.ExportExcel.Commands
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class CmdExportExcel : IExternalCommand
    {
        private RvtExtApp.Components.Attribute _CmpAttribute = null;
        private RvtExtApp.Components.Parameters _CmpParameters = null;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            CultureHelper.InitializeCulture();

            _CmpAttribute = new RvtExtApp.Components.Attribute();
            _CmpParameters = new RvtExtApp.Components.Parameters(commandData.Application.ActiveUIDocument);

            Document doc = commandData.Application.ActiveUIDocument.Document;

            eSelectMode mode = SelectMode();

            if (mode == eSelectMode.Invalid)
                return Result.Failed;

            var elementList = GetElements(commandData.Application.ActiveUIDocument, mode);

            if (elementList == null || elementList.Count == 0)
            {
                MessageBox.Show(_CmpAttribute.ResourceText("IDS_INFO_NO_EXIST_ELEMENTS"), _CmpAttribute.ResourceText("IDS_TXT_INFORMATION"));
                return Result.Cancelled;
            }

            List<CategoryItem> categories = GetData.GetCategories(elementList);

            if (categories == null || categories.Count == 0)
                return Result.Cancelled;

            var form = new FormExportExcel(doc, _CmpAttribute, elementList, categories);
            if (form.ShowDialog() != DialogResult.OK)
                return Result.Failed;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = _CmpAttribute.ResourceText("IDS_SAVE_DIALOG_TITLE");
            saveFileDialog.Filter = $"{Resources.Text.TXT_FILETYPE_XLSX}|{Resources.Text.TXT_FILETYPE_XLS}|{Resources.Text.TXT_FILETYPE_CSV}";
            saveFileDialog.CheckPathExists = true;
            saveFileDialog.RestoreDirectory = true;

            string path = string.Empty;
            while (true)
            {
                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return Result.Cancelled;
                }
                path = saveFileDialog.FileName;

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
                return Result.Cancelled;

            var result = ExcelUtils.Export(doc, _CmpParameters, form.GetExportData(), elementList, path);
            if (result != Result.Succeeded)
            {
                MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERROR_CANNOT_EXPORT_EXCEL"), _CmpAttribute.ResourceText("IDS_TXT_ERROR"));
            }
            else
            {
                MessageBox.Show(_CmpAttribute.ResourceText("IDS_EXPORT_SUCCESS"), _CmpAttribute.ResourceText("IDS_TXT_INFORMATION"));
            }

            return result;
        }

        private List<Element> GetElements(UIDocument uiDoc, eSelectMode mode)
        {
            if (mode == eSelectMode.Invalid)
                return null;

            List<Element> elementList = new List<Element>();
            try
            {
                if (mode == eSelectMode.All)
                {
                    elementList = Common.GetElements(uiDoc.Document, ElementId.InvalidElementId, true);
                }
                else if (mode == eSelectMode.CurrentView)
                {
                    elementList = Common.GetElements(uiDoc.Document, uiDoc.ActiveView.Id, false);
                }
                else if (mode == eSelectMode.Selection)
                {
                    foreach (ElementId elementId in uiDoc.Selection.GetElementIds())
                    {
                        var element = uiDoc.Document.GetElement(elementId);
                        elementList.Add(element);
                    }
                }

                return elementList;
            }
            catch (Exception ex)
            {
                string errMsg = ex.Message;
                return null;
            }
        }

        private eSelectMode SelectMode()
        {
            var taskDialog = new TaskDialog(_CmpAttribute.ResourceText("IDS_TITILE_SELECT_MODE") + string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version));
            taskDialog.TitleAutoPrefix = false;
            taskDialog.MainInstruction = _CmpAttribute.ResourceText("IDS_MAIN_INSTRUCTION");
            taskDialog.MainContent = _CmpAttribute.ResourceText("IDS_MAIN_CONTENT");
            taskDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, _CmpAttribute.ResourceText("IDS_SELECT_ALL"));
            taskDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, _CmpAttribute.ResourceText("IDS_IN_CURRENT_VIEW"));
            taskDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink3, _CmpAttribute.ResourceText("IDS_SELECTION_ELEMENT"));
            TaskDialogResult taskDialogResult = taskDialog.Show();

            eSelectMode selectMode = eSelectMode.Invalid;
            if (taskDialogResult == TaskDialogResult.CommandLink1)
                selectMode = eSelectMode.All;
            else if (taskDialogResult == TaskDialogResult.CommandLink2)
                selectMode = eSelectMode.CurrentView;
            else if (taskDialogResult == TaskDialogResult.CommandLink3)
                selectMode = eSelectMode.Selection;

            return selectMode;
        }
    }
}

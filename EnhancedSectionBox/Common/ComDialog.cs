using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.EnhancedSectionBox.Common
{
    /// <summary>
    /// タスクダイアログ用クラス
    /// </summary>
    public static class ComDialog
    {
        public static TaskDialogResult ShowDialog(string title, TaskDialogIcon icon, string message, bool buttonFlag)
        {
            TaskDialog taskDialog = new TaskDialog(title);
            taskDialog.MainIcon = icon;
            taskDialog.MainInstruction = message;
            if (buttonFlag) {
                TaskDialogCommonButtons buttons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No;
                taskDialog.CommonButtons = buttons;
            }
            else {
                TaskDialogCommonButtons buttons = TaskDialogCommonButtons.Ok;
                taskDialog.CommonButtons = buttons;
            }
            return taskDialog.Show();
        }
    }
}
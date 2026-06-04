using Autodesk.Revit.UI;
using ADSK.JExtRAC.EnhancedSectionBox.Screen;

namespace ADSK.JExtRAC.EnhancedSectionBox.Common
{
    public static class ComDialog
    {
        public static TaskDialogResult ShowDialog(string title, TaskDialogIcon icon, string message, bool buttonFlag)
        {
            if (!buttonFlag)
            {
                MessageWindow.Show(title, message);
                return TaskDialogResult.Ok;
            }

            TaskDialog taskDialog = new TaskDialog(title);
            taskDialog.MainIcon = icon;
            taskDialog.MainInstruction = message;
            TaskDialogCommonButtons buttons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No;
            taskDialog.CommonButtons = buttons;
            return taskDialog.Show();
        }
    }
}
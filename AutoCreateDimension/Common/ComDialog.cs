using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.AutoCreateDimension.Common
{
    /// ================================================================================
    /// <summary>タスクダイアログ用クラス</summary>
    /// ================================================================================
    public static class ComDialog
    {
        /// <summary>タスクダイアログ表示処理</summary>
        /// <param name="title" >タイトル</param>
        /// <param name="icon">アイコン</param>
        /// <param name="buttonFlag">ボタン制御フラグ</param>
        /// <returns>タスクダイアログ実行結果</returns>
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
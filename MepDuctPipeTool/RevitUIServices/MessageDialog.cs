using Autodesk.Revit.UI;
using MepDuctPipeTool.Utils;

namespace MepDuctPipeTool.RevitUIServices
{
  public static class MessageDialog
  {
    public static void ShowInformation( string title, string msg )
    {
      ShowTaskDialog(
        title,
        msg,
        TaskDialogCommonButtons.Ok,
        TaskDialogIcon.TaskDialogIconInformation
      );
    }

    public static void ShowWarning( string title, string msg )
    {
      ShowTaskDialogWithManualLink(
        title,
        msg,
        TaskDialogCommonButtons.Ok,
        TaskDialogIcon.TaskDialogIconWarning
      );
    }

    public static void ShowError( string title, string msg )
    {
      ShowTaskDialog(
        title,
        msg,
        TaskDialogCommonButtons.Ok,
        TaskDialogIcon.TaskDialogIconError
      );
    }

    public static void ShowErrorWithManualLink( string title, string msg )
    {
      ShowTaskDialogWithManualLink(
        title,
        msg,
        TaskDialogCommonButtons.Ok,
        TaskDialogIcon.TaskDialogIconError
      );
    }

    private static TaskDialogResult ShowTaskDialog( string title, string msg, TaskDialogCommonButtons buttons, TaskDialogIcon icon )
    {
      using var dlg = new TaskDialog( title );
      dlg.TitleAutoPrefix = false;
      dlg.MainContent = msg;
      dlg.CommonButtons = buttons;
      dlg.MainIcon = icon;
      return dlg.Show();
    }

    private static TaskDialogResult ShowTaskDialogWithManualLink( string title, string msg, TaskDialogCommonButtons buttons, TaskDialogIcon icon )
    {
      var helpPath = ResourceUtils.GetHelpPath();

      using var dlg = new TaskDialog( title );
      dlg.TitleAutoPrefix = false;
      dlg.MainContent = msg + $"\n \n<a href=\"{helpPath}\">User Manual</a>";
      dlg.CommonButtons = buttons;
      dlg.MainIcon = icon;
      return dlg.Show();
    }
  }
}
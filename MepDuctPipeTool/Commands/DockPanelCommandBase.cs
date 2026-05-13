using System;
using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MepDuctPipeTool.RevitUIServices;
using MepDuctPipeTool.Utils;
using MepDuctPipeTool.ViewModels;

namespace MepDuctPipeTool.Commands
{
  internal abstract class DockPanelCommandBase : IExternalCommand, MyCommand
  {
    private AccessoryFlangeSettingViewModel? _viewModel;
    private protected abstract string CommandName { get; }

    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      var uiApp = commandData.Application;
      var uiDoc = uiApp.ActiveUIDocument;

      return Execute( uiDoc, true );
    }

    public Result Execute( UIDocument uiDoc, bool isRibbon )
    {
      try
      {
        _viewModel = DockPanelViewUtils.GetViewModel<AccessoryFlangeSettingViewModel>( GlobalSettings.Instance.DockPanelPage );
        _viewModel.OnPanelCommandExecuted();
        return ExecuteImpl( uiDoc, _viewModel );
      }
      catch ( Autodesk.Revit.Exceptions.OperationCanceledException )
      {
        return Result.Cancelled;
      }
      catch ( Exception e )
      {
        Debug.WriteLine( e );
        // MyCommand経由で本メソッドを呼び出した場合、IExternalCommand.Executeのref string messageでメッセージを出すことができないので、自前でメッセージBOXを出す。
        MessageDialog.ShowError( CommandName, e.Message );
        ErrorLogger.LogException( e );
        return Result.Failed;
      }
      finally
      {
        _viewModel?.OnPanelCommandFinished();
      }
    }

    private protected abstract Result ExecuteImpl( UIDocument uiDoc, AccessoryFlangeSettingViewModel viewModel );
  }
}
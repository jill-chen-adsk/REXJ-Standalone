using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using MepDuctPipeTool.ViewModels;
using System.Diagnostics; 

namespace MepDuctPipeTool.EventHandlers
{
  public class IdlingEventListenerForDockPanelRefresh
  {
    private readonly DockPanelInitializer _dockPanelInitializer;
    private readonly long _throttleIntervalMilliseconds = 500;
    private readonly Stopwatch _throttleStopwatch = Stopwatch.StartNew();
    
    public IdlingEventListenerForDockPanelRefresh( DockPanelInitializer? dockPanelInitializer )
    {
      _dockPanelInitializer = dockPanelInitializer ?? throw new ArgumentException();
    }

    public void OnIdling( object? sender, IdlingEventArgs args )
    {
      if ( GlobalSettings.Instance.DockPanelPage?.DataContext is not AccessoryFlangeSettingViewModel { NeedsRefresh: true } viewModel ) return ;
      
      if ( _throttleStopwatch.ElapsedMilliseconds < _throttleIntervalMilliseconds ) return ;

      if ( sender is not UIApplication uiApp ) return ;
      if ( uiApp.ActiveUIDocument?.Document is not { } document ) return ;

      _dockPanelInitializer.InitializePipeAccessoryFlangeModelAndViewModel( document ) ;
      _throttleStopwatch.Restart();
    }
  }
}
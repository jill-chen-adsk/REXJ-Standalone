using System;

namespace MepDuctPipeTool.Utils
{
  public static class DockPanelViewUtils
  {
    public static T GetViewModel<T>( DockPanelPage? dockPanelPage ) where T : class
    {
      return dockPanelPage?.DataContext as T ?? throw new InvalidOperationException();
    }
  }
}
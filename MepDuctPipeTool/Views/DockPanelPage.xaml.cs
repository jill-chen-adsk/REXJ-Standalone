using System.Windows.Controls;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DockPosition = Autodesk.Revit.UI.DockPosition;
using IDockablePaneProvider = Autodesk.Revit.UI.IDockablePaneProvider;

namespace MepDuctPipeTool
{
  public partial class DockPanelPage : Page, IDockablePaneProvider
  {
    public DockPanelPage()
    {
      InitializeComponent();
    }

    public void SetupDockablePane( DockablePaneProviderData data )
    {
      data.FrameworkElement = this;
      data.InitialState = new DockablePaneState();
      data.InitialState.DockPosition = DockPosition.Floating;
      data.InitialState.MinimumWidth = 170;
      data.InitialState.MinimumHeight = 440;
      data.InitialState.SetFloatingRectangle( new Rectangle( 100, 100, 270, 480 ) );
      data.VisibleByDefault = true;
      data.EditorInteraction = new EditorInteraction { InteractionType = EditorInteractionType.KeepAlive };
    }
  }
}
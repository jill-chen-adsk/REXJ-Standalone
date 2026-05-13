using System;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using MepDuctPipeTool.Utils;
using MepDuctPipeTool.ViewModels;

namespace MepDuctPipeTool.EventHandlers
{
  public static class FamilyAndSymbolChangedListener
  {
    public static void OnDocumentChanged( object? sender, DocumentChangedEventArgs e )
    {
      var familyFilter = new ElementClassFilter( typeof( Family ) );
      var familySymbolFilter = new ElementClassFilter( typeof( FamilySymbol ) );
      var familyAndSymbolFilter = new LogicalOrFilter( familyFilter, familySymbolFilter );

      var addedElementIds = e.GetAddedElementIds( familyAndSymbolFilter ).Cast<ElementId>();
      var modifiedElementIds = e.GetModifiedElementIds( familyAndSymbolFilter ).Cast<ElementId>();

      if ( addedElementIds.Any() || modifiedElementIds.Any() )
      {
        var dockPanelPage = GlobalSettings.Instance.DockPanelPage ?? throw new InvalidOperationException();
        var viewModel = DockPanelViewUtils.GetViewModel<AccessoryFlangeSettingViewModel>( dockPanelPage );
        viewModel.NeedsRefresh = true;
      }

      // ファミリが削除された際に飛ぶイベントがRevitAPIで用意されていないので、
      // 削除されたかの判定はViewActivatedEventListener.OnViewActivated内部で行っている。
    }
  }
}
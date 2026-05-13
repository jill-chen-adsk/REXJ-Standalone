using System;
using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Events;

namespace MepDuctPipeTool.EventHandlers
{
  internal class ViewActivatedEventListener
  {
    private readonly DockPanelInitializer _dockPanelInitializer;

    public ViewActivatedEventListener( DockPanelInitializer? dockPanelInitializer )
    {
      _dockPanelInitializer = dockPanelInitializer ?? throw new ArgumentException();
    }

    internal void OnViewActivated( object? sender, ViewActivatedEventArgs e )
    {
      Debug.WriteLine( "View activated." );
      var doc = e.Document;
      if ( doc is null ) return;

      var previousDoc = e.PreviousActiveView?.Document;
      if ( previousDoc is null || IsDocumentSwitched( doc, previousDoc ) )
      {
        GlobalSettings.Instance.Init( doc );
        _dockPanelInitializer.InitializeOrRestoreModelAndViewModel( doc );
      }

      Debug.WriteLine( "On View activated finished." );
    }

    private static bool IsDocumentSwitched( Document currentDoc, Document previousDoc )
      => ! currentDoc.Equals( previousDoc );
  }
}
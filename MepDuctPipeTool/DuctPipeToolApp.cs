using System ;
using System.Collections.Generic ;
using System.Diagnostics ;
using System.Reflection ;
using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using Autodesk.Revit.UI.Events ;
using MepDuctPipeTool.EventHandlers ;
using MepDuctPipeTool.Models ;
using MepDuctPipeTool.RevitUIServices ;
using MepDuctPipeTool.Utils ;
using MepDuctPipeTool.ViewModels ;

namespace MepDuctPipeTool
{
  public class DuctPipeToolApp : IExternalApplication, IExternalCommandAvailability
  {
    private DockPanelPage? _dockPanelPage;
    private DockPanelInitializer? _dockPanelInitializer;
    private RevitCommandCustomizer? _commandCustomizer;
    private IdlingEventListenerForDockPanelRefresh? _panelRefresh;
    private ViewActivatedEventListener? _viewActivatedEventListener;

    public Result OnStartup( UIControlledApplication application )
    {
      var thisAssemblyPath = Assembly.GetExecutingAssembly().Location;
      RegisterDockPanel( application );

      var ribbonPanelInitializer = new RibbonPanelInitializer( application, thisAssemblyPath );
      ribbonPanelInitializer.Initialize();

      // Events
      _panelRefresh = new IdlingEventListenerForDockPanelRefresh( _dockPanelInitializer );
      _viewActivatedEventListener = new ViewActivatedEventListener( _dockPanelInitializer );
      application.ViewActivated += _viewActivatedEventListener.OnViewActivated;
      application.Idling += _panelRefresh.OnIdling;
      application.ControlledApplication.DocumentChanged += FamilyAndSymbolChangedListener.OnDocumentChanged;
      _commandCustomizer = new RevitCommandCustomizer( application )
      {
        BeforeSelectionCommandEvent = OnBeforeSelectionCommand
      };
      _commandCustomizer.RegisterBeforeExecuteEventHandlers();

      var handler = new ExtEventHandler();
      var extEvent = ExternalEvent.Create( handler );
      GlobalSettings.Instance.ExternalEventHandler = handler;
      GlobalSettings.Instance.ExtEvent = extEvent;

      return Result.Succeeded;
    }

    public Result OnShutdown( UIControlledApplication application )
    {
      if(_viewActivatedEventListener != null ) application.ViewActivated -= _viewActivatedEventListener.OnViewActivated;
      if ( _panelRefresh != null ) application.Idling -= _panelRefresh.OnIdling ;
      application.ControlledApplication.DocumentChanged -= FamilyAndSymbolChangedListener.OnDocumentChanged;
      _commandCustomizer?.RemoveBeforeExecuteEventHandlers();
      return Result.Succeeded;
    }

    private void RegisterDockPanel( UIControlledApplication app )
    {
      _dockPanelPage = new DockPanelPage();
      GlobalSettings.Instance.DockPanelPage = _dockPanelPage;

      var dpId = new DockablePaneId( new Guid( Resources.DOCKABLE_PANE_ID ) );
      app.RegisterDockablePane( dpId, Resources.DOCKABLE_PANE_NAME, _dockPanelPage );

      _dockPanelInitializer = new DockPanelInitializer( _dockPanelPage );
    }

    public bool IsCommandAvailable( UIApplication a, CategorySet b )
    {
      // パイピングが使えるときだけコマンドが存在するように
      if ( ! a.Application.IsPipingEnabled ) return false;

      if ( a.ActiveUIDocument != null )
      {
        if ( a.ActiveUIDocument.Document.IsFamilyDocument ) return false;

        return true;
      }

      return false;
    }


    private void OnBeforeSelectionCommand( object? sender, BeforeExecutedEventArgs args )
    {
      Debug.WriteLine( "Before selection: " + DateTime.Now );
      var viewModel = DockPanelViewUtils.GetViewModel<AccessoryFlangeSettingViewModel>( _dockPanelPage );

      viewModel.OnBeforeSelectionCommand();
    }
  }


  public class DockPanelInitializer
  {
    private readonly DockPanelPage _dockPanelPage;
    private readonly Dictionary<Guid, AccessoryFlangeSettingModel> _modelCache = new();

    public DockPanelInitializer( DockPanelPage dockPanelPage )
    {
      _dockPanelPage = dockPanelPage;
    }

    public void InitializeOrRestoreModelAndViewModel( Document doc )
    {
      var propInfo = typeof( Document ).GetProperty( "CreationGUID" ) ;
      var guid = propInfo == null ? doc.ProjectInformation.VersionGuid : (Guid)propInfo.GetValue( doc ) ;
      
      if ( _modelCache.TryGetValue( guid, out var model ) && ! model.HasDeletedFamily() )
      {
        // restore
        var accessoryFlangeSettingViewModel = new AccessoryFlangeSettingViewModel( model );
        _dockPanelPage.DataContext = accessoryFlangeSettingViewModel;
        return;
      }

      InitializePipeAccessoryFlangeModelAndViewModel( doc );
    }

    public void InitializePipeAccessoryFlangeModelAndViewModel( Document doc )
    {
      Debug.WriteLine( "InitializePipeAccessoryFlangeModelAndViewModel start" );
      var accessoryFlangeSettingModel = AccessoryFlangeSettingModel.Create( doc );
      var accessoryFlangeSettingViewModel = new AccessoryFlangeSettingViewModel( accessoryFlangeSettingModel );

      _dockPanelPage.DataContext = accessoryFlangeSettingViewModel;
      
      accessoryFlangeSettingViewModel.NeedsRefresh = false; 
      
      var propInfo = typeof( Document ).GetProperty( "CreationGUID" ) ;
      var documentGuid = propInfo == null ? doc.ProjectInformation.VersionGuid : (Guid)propInfo.GetValue( doc ) ;
      _modelCache.Remove( documentGuid );
      _modelCache.Add( documentGuid, accessoryFlangeSettingModel );
    }
  }
}
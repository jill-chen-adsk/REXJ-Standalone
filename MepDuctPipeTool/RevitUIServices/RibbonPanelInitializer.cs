using System;
using Autodesk.Revit.UI;
using MepDuctPipeTool.Commands;
using MepDuctPipeTool.Commands.InsertFlange;
using MepDuctPipeTool.Commands.InsertPipeAccessoryWithFlange;
using MepDuctPipeTool.Utils;
using AdWindows = Autodesk.Windows ;

namespace MepDuctPipeTool.RevitUIServices
{
  public class RibbonPanelInitializer
  {
    private readonly UIControlledApplication _application;
    private readonly string _thisAssemblyPath;
    // private static readonly string TabName = Resources.RIBBON_TAB_NAME;
    private static readonly string PanelName = Resources.RIBBON_PANEL_NAME;
    private readonly string _tabName ;
    
    public RibbonPanelInitializer( UIControlledApplication application, string thisAssemblyPath )
    {
      _application = application;
      _thisAssemblyPath = thisAssemblyPath;

      _tabName = Resources.RIBBON_TAB_NAME ;
    }

    public void Initialize()
    {
      var editPanel = GetOrCreateEditRibbonPanel();
      var help = CreateContextualHelp();
      RegisterButtons( editPanel, help );
    }

    private static ContextualHelp CreateContextualHelp()
    {
      var helpPath = ResourceUtils.GetHelpPath();
      return new ContextualHelp( ContextualHelpType.Url, helpPath );
    }

    private RibbonPanel GetOrCreateEditRibbonPanel()
    {
      
      return FindEditPanel() ?? _application.CreateRibbonPanel( _tabName, PanelName );
    }

    private RibbonPanel? FindEditPanel()
    {
      var tab = _application.GetOrCreateTab( _tabName ) ;

      var panels = _application.GetRibbonPanels( _tabName );
      var panel = panels?.Find( p => p.Name == PanelName ) ?? _application.CreateRibbonPanel( _tabName, PanelName ) ;

      return panel;

    }
    

    private void RegisterButtons( RibbonPanel panel, ContextualHelp help )
    {
      // AddSeparatorの仕様: パネルに要素が無ければ非表示になる
      panel.AddSeparator();

      var showDockablePaneCmdName = GetFullClassName( typeof( ShowDockablePaneCmd ) );
      RegisterButton(
        panel,
        "ShowDockPanel",
        Resources.RIBBON_BTN_TXT_SHOW_SETTINGS,
        showDockablePaneCmdName,
        Resources.RIBBON_IMG_SHOW_SETTINGS_32,
        Resources.RIBBON_IMG_SHOW_SETTINGS_16,
        Resources.RIBBON_BTN_TOOLTIP_SHOW_SETTING,
        help
      );

      var insertAccessoriesWithFlangeCmdName = GetFullClassName( typeof( InsertPipeAccessoriesWithFlangeCommand ) );
      RegisterButton(
        panel,
        "InsertAccessoriesWithFlange",
        Resources.RIBBON_BTN_TXT_INSERT_ACCESSORY_WITH_FLANGE,
        insertAccessoriesWithFlangeCmdName,
        Resources.RIBBON_IMG_INSERT_ACCESORRY_WITH_FLANGE_32,
        Resources.RIBBON_IMG_INSERT_ACCESORRY_WITH_FLANGE_16,
        Resources.RIBBON_BTN_TOOLTIP_INSERT_ACCESSORY_WITH_FLANGE,
        help
      );

      var insertFlangeCmdName = GetFullClassName( typeof( InsertFlangeCommand ) );
      RegisterButton(
        panel,
        "InsertFlange",
        Resources.RIBBON_BTN_TXT_INSERT_FLANGE,
        insertFlangeCmdName,
        Resources.RIBBON_IMG_INSERT_FLANGE_32,
        Resources.RIBBON_IMG_INSERT_FLANGE_16,
        Resources.RIBBON_BTN_TOOLTIP_INSERT_FLANGE,
        help
      );
    }

    private string GetFullClassName( Type commandClassType ) => commandClassType.FullName ?? throw new InvalidOperationException();

    private void RegisterButton( RibbonPanel panel, string name, string buttonText, string commandClassName, string largeImagePath,string smallImagePath , string toolTipText, ContextualHelp help )
    {
      var buttonData = new PushButtonData( name, buttonText, _thisAssemblyPath, commandClassName );
      buttonData.AvailabilityClassName = typeof( DuctPipeToolApp ).FullName;
      if ( panel.AddItem( buttonData ) is not PushButton pushButton ) throw new InvalidOperationException();
      pushButton.LargeImage = largeImagePath.ToResImageInPack();
      pushButton.Image = smallImagePath.ToResImageInPack();
      pushButton.ToolTip = toolTipText;
      pushButton.SetContextualHelp( help );
    }
  }
}
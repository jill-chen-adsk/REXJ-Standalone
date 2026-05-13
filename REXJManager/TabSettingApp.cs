using System ;
using System.IO ;
using Autodesk.Revit.UI ;
using Autodesk.Revit.UI.Events ;
using System.Reflection ;
using Autodesk.Revit.ApplicationServices ;
using UIFramework ;
using AdWindows = Autodesk.Windows ;

namespace REXJManager
{
  public class TabSettingApp : IExternalApplication
  {
    private static UIControlledApplication _application ;
    private static RibbonPanel _ribbonPanel ;

    /// <summary>
    /// アドインのエントリーポイント
    /// </summary>
    /// <param name="application"></param>
    /// <returns></returns>
    public Result OnStartup( UIControlledApplication application )
    {
      _application = application ;
      application.ViewActivated += Application_ViewActivated ;

      return Result.Succeeded ;
    }

    /// <summary>
    /// 各ボタン類の表示非表示の状態を変更するには
    /// タブ内の他のパネルが全て用意されてからの必要がある
    /// なので、ドキュメントが読み込まれたタイミングでリボンパネルを初期化する
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Application_ViewActivated( object sender, ViewActivatedEventArgs e )
    {
      var doc = e.Document ;
      if ( doc == null ) return ;
      if ( _ribbonPanel == null ) InitRibbonPanel() ;
    }

    /// <summary>
    /// リボンパネルの初期化
    /// </summary>
    private void InitRibbonPanel()
    {
      if ( _ribbonPanel != null ) return ;

      // Get addin tab
      var tabName = Resource.TAB_NAME ;
      var tab = AddInTab( tabName, _application ) ;

      //create ribbon Panel
      var assemblyPath = Assembly.GetExecutingAssembly().Location ;
      _ribbonPanel = _application.CreateRibbonPanel( tabName, Resource.TXT_CMD_SETTING ) ;
      var settingButtonData = new PushButtonData( Resource.TXT_NAME_SETTING_BTN, Resource.TXT_CMD_SETTING, assemblyPath, typeof( TabSettingCmd ).FullName ) { LargeImage = Resource.ICON_GEAR.ToResImageInPack() } ;
      var helpDir = Path.Combine( Path.GetDirectoryName( assemblyPath ) ?? "", "Help" ) ;
      var helpFiles = Directory.Exists( helpDir ) ? Directory.GetFiles( helpDir, "*.pdf" ) : Array.Empty<string>() ;
      var helpPath = helpFiles.Length > 0 ? helpFiles[ 0 ] : Path.Combine( helpDir, "REXJ\u8a2d\u5b9a.pdf" ) ;
      if ( File.Exists( helpPath ) )
        settingButtonData.SetContextualHelp( new ContextualHelp( ContextualHelpType.Url, new Uri( helpPath ).AbsoluteUri ) ) ;
      _ = _ribbonPanel.AddItem( settingButtonData ) as PushButton ;

      MoveAddInPanelToFirst( tab, _ribbonPanel ) ;

      Preset.IsLt = _application.ControlledApplication.VersionName.ToUpper().Contains( Resource.TXT_LT ) ;
      Preset.GenerateSystemPresets() ;
      Preset.LoadConf() ;
      Preset.SetSystemPresetsToReadOnly() ;

      RevitRibbonControl.RibbonControl.Extract<MyTreeNode>().Load( Preset.Name ) ;
    }


    /// <summary>
    /// シャットダウン時処理
    /// </summary>
    /// <param name="application"></param>
    /// <returns></returns>
    public Result OnShutdown( UIControlledApplication application )
    {
      return Result.Succeeded ;
    }


    /// <summary>
    /// REXJタブの初期化
    /// タブが存在しない場合は作成。
    /// </summary>
    /// <param name="tabName"></param>
    /// <param name="application"></param>
    /// <returns></returns>
    private static AdWindows.RibbonTab AddInTab( string tabName, UIControlledApplication application )
    {
      var ribbon = AdWindows.ComponentManager.Ribbon ;

      AdWindows.RibbonTab tab = null ;

      GetAddInTab() ;

      if ( tab != null ) return tab ;
      try {
        application.CreateRibbonTab( tabName ) ;
      }
      catch {
        /* tab may already exist from another standalone add-in */
      }
      GetAddInTab() ;

      return tab ;

      void GetAddInTab()
      {
        foreach ( var ribbonTab in ribbon.Tabs ) {
          if ( ribbonTab.Id != tabName ) continue ;
          tab = ribbonTab ;
          break ;
        }
      }
    }

    /// <summary>
    /// 設定パネルを一番左にする。
    /// 他コマンドをリボンパネルから検索して表示非表示を変えるためにこのアドインが最後に読み込まれる必要がある。
    /// </summary>
    /// <param name="ribbonTab"></param>
    /// <param name="ribbonPanel"></param>
    private static void MoveAddInPanelToFirst( AdWindows.RibbonTab ribbonTab, RibbonPanel ribbonPanel )
    {
      var i = ribbonTab.Panels.Count - 1 ;
      foreach ( var iPanel in ribbonTab.Panels ) {
        if ( iPanel.AutomationName == ribbonPanel.Name ) {
          i = ribbonTab.Panels.IndexOf( iPanel ) ;
        }
      }

      ribbonTab.Panels.Move( i, 0 ) ;
    }
  }
}
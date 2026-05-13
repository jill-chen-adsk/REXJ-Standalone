#region Namespaces

using System ;
using System.Collections.Generic ;
using Autodesk.Revit.ApplicationServices ;
using Autodesk.Revit.Attributes ;
using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using System.Reflection ;
using System.Windows.Media.Imaging ;
using System.IO ;
using CmdDuctDisplacement ;
using RevitMEPAddinApp.Properties ;

#endregion

namespace RevitMEPAddinApp
{
  public class App : IExternalApplication
  {
    static readonly string ExecutingAssemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location ;
    string tabName = ExResources.TAB_MEP_ADDIN() ;
    // private AppDocEvents hoge;

    public Result OnStartup( UIControlledApplication app )
    {
      try {
        app.CreateRibbonTab( tabName ) ;
      }
      catch ( Autodesk.Revit.Exceptions.ArgumentException ) {
        // ??????A?h?C?????[?h????????^?u????????????????A??????????i??
      }

      AddMenu( app ) ;
      AddMenuForRotateTees( app ) ;
      AddMenuForMoveConnector( app ) ;
      //  AddAppDocEvents(a.ControlledApplication);

      return Result.Succeeded ;
    }

    public Result OnShutdown( UIControlledApplication app )
    {
      // RemoveAppDocEvents();
      return Result.Succeeded ;
    }
    private ContextualHelp GetContextualHelp( string path )
    {
      // help(F1?L?[)
      ContextualHelp contHelp = null ;
      string contHelpPath = System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location ) + @path ;
      if ( System.IO.File.Exists( contHelpPath ) == true ) {
        contHelp = new ContextualHelp( ContextualHelpType.Url, contHelpPath ) ;
      }

      return contHelp ;
    }

    private PushButton CreatePushButton( UIControlledApplication app, string assyPath, string buttonName, string commandName, string iconNameSmall, string iconNameLarge, string toolTip, string helpPath )
    {
      string resolvedAssyPath = System.IO.Path.IsPathRooted( assyPath )
        ? assyPath
        : System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location ) + assyPath ;
      PushButton pushButton = null ;
      {
        RibbonPanel ribbonPanel = null ;
        {
          string panelName = ExResources.PLN_EDIT() ;
          var panels = app.GetRibbonPanels( tabName ) ;
          ribbonPanel = panels.Find( p => p.Name == panelName ) ;
          if ( ribbonPanel == null ) {
            ribbonPanel = app.CreateRibbonPanel( tabName, panelName ) ;
          }
        }
        var items = ribbonPanel.GetItems() ;
        foreach ( RibbonItem rItem in items ) {
          if ( rItem.ItemText == buttonName )
            if ( rItem.ItemType == RibbonItemType.PushButton )
              pushButton = rItem as PushButton ;
        }

        if ( pushButton == null ) {
          pushButton = ribbonPanel.AddItem( new PushButtonData( buttonName, buttonName, resolvedAssyPath, commandName ) ) as PushButton ;
        }
      }

      var smallImg = SafeLoadPackImage( iconNameSmall ) ;
      var largeImg = SafeLoadPackImage( iconNameLarge ) ;
      if ( smallImg != null ) pushButton.Image = smallImg ;
      if ( largeImg != null ) pushButton.LargeImage = largeImg ;
      pushButton.ToolTip = toolTip ;
      var contHelp = GetContextualHelp( helpPath ) ;
      if ( contHelp != null ) pushButton.SetContextualHelp( contHelp ) ;
      return pushButton ;
    }

    private void AddMenuForRotateTees( UIControlledApplication app )
    {
      string assyPath = System.Reflection.Assembly.GetExecutingAssembly().Location ;
      string buttonName = ExResources.BTN_ROTATE_TEES() ;
      string commandName = "CmdRotateTees.CmdRotateTees" ;
      string iconNameSmall = "RotateTeesS.png" ;
      string iconNameLarge = "RotateTeesL.png" ;
      string toolTip = ExResources.T_TIP_BTN_ROTATE_TEES() ;
      string helpPath = "\\Resources\\?y?w???v?z?t???i???].pdf" ;
      CreatePushButton( app, assyPath, buttonName, commandName, iconNameSmall, iconNameLarge, toolTip, helpPath ) ;
    }

    private void AddMenuForMoveConnector( UIControlledApplication app )
    {
      string assyPath = System.Reflection.Assembly.GetExecutingAssembly().Location ;
      string buttonName = ExResources.BTN_MOVE_CONNECTOR() ;
      string commandName = "CmdMoveConnector.MoveConnector" ;
      string iconNameSmall = "MoveConnectorS.png" ;
      string iconNameLarge = "MoveConnectorL.png" ;
      string toolTip = ExResources.T_TIP_BTN_MOVE_CONNECTOR() ;
      string helpPath = "\\Resources\\?y?w???v?z?t???i????.pdf" ;
      CreatePushButton( app, assyPath, buttonName, commandName, iconNameSmall, iconNameLarge, toolTip, helpPath ) ;
    }
    private void AddMenu( UIControlledApplication app )
    {
      string panelName = ExResources.PLN_EDIT() ;
      string pulldownBtnName = ExResources.BTN_EDIT_LEVEL() ;

      List<RibbonPanel> panels = app.GetRibbonPanels( tabName ) ;
      RibbonPanel ribbonpanel = panels.Find( p => p.Name == panelName ) ;
      if ( ribbonpanel == null ) {
        ribbonpanel = app.CreateRibbonPanel( tabName, panelName ) ;
      }

      IList<RibbonItem> items = ribbonpanel.GetItems() ;
      PulldownButton pdBtn = null ;
      foreach ( RibbonItem rItem in items ) {
        if ( rItem.ItemText == pulldownBtnName ) {
          if ( rItem.ItemType == RibbonItemType.PulldownButton ) {
            pdBtn = rItem as PulldownButton ;
          }
          else {
            // ????
          }
        }
      }

      if ( pdBtn == null ) {
        PulldownButtonData data = new PulldownButtonData( "Options", pulldownBtnName ) ;
        RibbonItem item = ribbonpanel.AddItem( data ) ;
        pdBtn = item as PulldownButton ;
      }


      //?v???_?E???{?^????A?C?R???C???[?W
      var pdSmall = SafeLoadPackImage( "pdBtnIconPathSmall.png" ) ;
      var pdLarge = SafeLoadPackImage( "pdBtnIconPathLarge.png" ) ;
      if ( pdSmall != null ) pdBtn.Image = pdSmall ;
      if ( pdLarge != null ) pdBtn.LargeImage = pdLarge ;

      PushButton twoPickBtn = pdBtn.AddPushButton( new PushButtonData( ExResources.P_BTN_2PICK(), ExResources.P_BTN_2PICK(), ExecutingAssemblyPath, "CmdDuctDisplacement.ModDuctLevelPartiallyCommand" ) ) ;
      PushButton threePickBtn = pdBtn.AddPushButton( new PushButtonData( ExResources.P_BTN_3PICK(), ExResources.P_BTN_3PICK(), ExecutingAssemblyPath, "CmdDuctDisplacement.ModDuctLevelPartiallyCommand_3Pick" ) ) ;
      PushButton threePickBtn_selectobjLinkd = pdBtn.AddPushButton( new PushButtonData( ExResources.P_BTN_3PICK_Linkd(), ExResources.P_BTN_3PICK_Linkd(), ExecutingAssemblyPath, "CmdDuctDisplacement.ModDuctLevelPartiallyCommand_3Pick_LinkdModel" ) ) ;

      var twoSmall = SafeLoadPackImage( "twoPickBtnIconPathSmall.png" ) ;
      var twoLarge = SafeLoadPackImage( "twoPickBtnIconPathLarge.png" ) ;
      if ( twoSmall != null ) twoPickBtn.Image = twoSmall ;
      if ( twoLarge != null ) twoPickBtn.LargeImage = twoLarge ;

      var threeSmall = SafeLoadPackImage( "threePickBtnIconPathSmall.png" ) ;
      var threeLarge = SafeLoadPackImage( "threePickBtnIconPathLarge.png" ) ;
      if ( threeSmall != null ) threePickBtn.Image = threeSmall ;
      if ( threeLarge != null ) threePickBtn.LargeImage = threeLarge ;

      var threeLinkSmall = SafeLoadPackImage( "threePickLinkBtnIconPathSmall.png" ) ;
      var threeLinkLarge = SafeLoadPackImage( "threePickLinkBtnIconPathLarge.png" ) ;
      if ( threeLinkSmall != null ) threePickBtn_selectobjLinkd.Image = threeLinkSmall ;
      if ( threeLinkLarge != null ) threePickBtn_selectobjLinkd.LargeImage = threeLinkLarge ;


      // ToolTip
      pdBtn.ToolTip = ExResources.T_TIP_PD_BTN() ;
      twoPickBtn.ToolTip = ExResources.T_TIP_P_BTN_2PICK() ;
      threePickBtn.ToolTip = ExResources.T_TIP_P_BTN_3PICK() ;
      threePickBtn_selectobjLinkd.ToolTip = ExResources.T_TIP_P_BTN_3PICK_Linkd() ;

      // help(F1?L?[)
      ContextualHelp contHelp = null ;
      string contHelpPath = System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location ) + @"\Resources\?y?w???v?z?_?N?g?E?z??i??????.pdf" ;
      if ( System.IO.File.Exists( contHelpPath ) == true ) {
        contHelp = new ContextualHelp( ContextualHelpType.Url, contHelpPath ) ;
      }

      if ( contHelp != null ) {
        pdBtn.SetContextualHelp( contHelp ) ;
        twoPickBtn.SetContextualHelp( contHelp ) ;
        threePickBtn.SetContextualHelp( contHelp ) ;
        threePickBtn_selectobjLinkd.SetContextualHelp( contHelp ) ;
      }
    }


    private static BitmapImage SafeLoadPackImage( string iconName )
    {
      try {
        string dir = System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location ) ;
        string path = System.IO.Path.Combine( dir, "Icons", iconName ) ;
        if ( !System.IO.File.Exists( path ) ) return null ;
        var bmp = new BitmapImage() ;
        bmp.BeginInit() ;
        bmp.CacheOption = BitmapCacheOption.OnLoad ;
        bmp.UriSource = new Uri( path, UriKind.Absolute ) ;
        bmp.EndInit() ;
        bmp.Freeze() ;
        return bmp ;
      }
      catch {
        return null ;
      }
    }
  }
}

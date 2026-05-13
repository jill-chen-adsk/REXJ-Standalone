using System ;
using System.IO ;
using System.Linq ;
using System.Reflection ;
using Autodesk.Revit.ApplicationServices ;
using Autodesk.Revit.UI ;
using MEPConnectTool.Commands ;
using MEPConnectTool.Utils ;

namespace MEPConnectTool.RibbonPanel
{
  public class MyRibbonPanel
  {
    public MyRibbonPanel( UIControlledApplication application )
    {
      var assemblyPath = Assembly.GetExecutingAssembly().Location ;
      var tabName = "REXJ Standalone" ;
      var panel = application.CreateRibbonPanel( tabName, "Connection Tool" ) ;
      var helpPath = $@"{Path.GetDirectoryName( assemblyPath )}\Help\ダクト・配管接続ツール.pdf" ;
      ContextualHelp? help = null ;
      if ( File.Exists( helpPath ) ) {
        help = new ContextualHelp( ContextualHelpType.Url, helpPath ) ;
      }
      
      var connectRectDuctButtonData = new PushButtonData( "connectRectDuct", "Rect Duct\nConnection", assemblyPath, typeof( ConnectRectDuctCmd ).FullName ) ;
      connectRectDuctButtonData.AvailabilityClassName = typeof( App ).FullName ;
      connectRectDuctButtonData.LargeImage = "ConnectRectDuctCmd.png".ToResImageInPack() ;
      if ( help != null ) connectRectDuctButtonData.SetContextualHelp( help ) ;
      panel.AddItem( connectRectDuctButtonData ) ;

      var connectElbowButtonData = new PushButtonData( "connectElbow", "Rect Elbow\nConnection", assemblyPath, typeof( ConnectElbowCmd ).FullName ) ;
      connectElbowButtonData.AvailabilityClassName = typeof( App ).FullName ;
      connectElbowButtonData.LargeImage = "ConnectElbowCmd.png".ToResImageInPack() ;
      if ( help != null ) connectElbowButtonData.SetContextualHelp( help ) ;
      panel.AddItem( connectElbowButtonData ) ;

      var connectRoundDuctButtonData = new PushButtonData( "connectRoundDuct", "Round Duct\nConnection", assemblyPath, typeof( ConnectRoundDuctCmd ).FullName ) ;
      connectRoundDuctButtonData.AvailabilityClassName = typeof( App ).FullName ;
      connectRoundDuctButtonData.LargeImage = "ConnectRoundDuctCmd.png".ToResImageInPack() ;
      if ( help != null ) connectRoundDuctButtonData.SetContextualHelp( help ) ;
      panel.AddItem( connectRoundDuctButtonData ) ;
      
      var connect45degDuctPipeButtonData = new PushButtonData( "connect45degDuctPipe", "Duct/Pipe\nT-45° Connection", assemblyPath, typeof( Connect45degDuctPipe ).FullName ) ;
      connect45degDuctPipeButtonData.AvailabilityClassName = typeof( App ).FullName ;
      connect45degDuctPipeButtonData.LargeImage = "Connect45degDuctPipe.png".ToResImageInPack() ;
      if ( help != null ) connect45degDuctPipeButtonData.SetContextualHelp( help ) ;
      panel.AddItem( connect45degDuctPipeButtonData ) ;
      
    }

    // private string TabName( ControlledApplication controlledApplication )
    // {
    //   var verName = controlledApplication.VersionName ;
    //   if ( verName.Contains( "2022" ) || verName.Contains( "2023" ) ) return "JP設備" ;
    //   return "REXJ" ;
    // }

    
  }
}

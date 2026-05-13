using System;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI ;
using Autodesk.Revit.UI.Events ;
using System.Reflection ;
using Autodesk.Revit.ApplicationServices ;
using AdWindows = Autodesk.Windows ;

namespace MepDuctPipeTool.Utils
{
  public static class UiUtils
  {
    public static BitmapImage ToResImageInPack( this string path )
    {
      return new BitmapImage( new Uri( $@"pack://application:,,,/MepDuctPipeTool;component/{path}", UriKind.Absolute ) );
    }

    /// <summary>
    /// アドインのタブを取得、できない場合は作成
    /// </summary>
    /// <param name="application"></param>
    /// <param name="tabName"></param>
    /// <returns></returns>
    public static AdWindows.RibbonTab GetOrCreateTab( this UIControlledApplication application , string tabName)
    {
      var ribbon = AdWindows.ComponentManager.Ribbon ;

      AdWindows.RibbonTab? tab = null;

      GetAddInTab() ;

      if ( tab != null ) return tab ;
      application.CreateRibbonTab( tabName ) ;
      GetAddInTab() ;

      return tab! ;

      void GetAddInTab()
      {
        foreach ( var ribbonTab in ribbon.Tabs ) {
          if ( ribbonTab.Id != tabName ) continue ;
          tab = ribbonTab ;
          break ;
        }
      }
    }
  }
}
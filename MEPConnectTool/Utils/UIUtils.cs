using System ;
using System.Collections.Generic ;
using System.Windows.Media.Imaging ;
using Autodesk.Revit.ApplicationServices ;
using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using Autodesk.Revit.UI.Selection ;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException ;
using MEPConnectTool.SelectionFilter;

namespace MEPConnectTool.Utils ;

public static class UIUtils
{
  ///タブ名の指定
  public static string TabName( ControlledApplication controlledApplication )
  {
    return "REXJ Standalone" ;
  }
  
  
  /// <summary>
  /// 要素の取得
  /// </summary>
  /// <param name="uiDoc"></param>
  /// <returns></returns>
  public static Element? PickElement( this UIDocument uiDoc)
  {
    var selection = uiDoc.Selection ;
    
    var tg = new TransactionGroup( uiDoc.Document, "PickElement" ) ;
    tg.Start() ;
    try {
      var categories = new List<BuiltInCategory>(){BuiltInCategory.OST_DuctCurves, BuiltInCategory.OST_PipeCurves} ;
      
      //要素色付けBuiltInCategory.OST_DuctCurves
      var reference = selection.PickObject( ObjectType.Element,  new CategoriesFilter( categories ), "Select an element." ) ;
      var elem = uiDoc.Document.GetElement( reference ) ;

      tg.RollBack() ;
      return elem ;
    }
    catch ( OperationCanceledException ) {
      tg.RollBack() ;
      return null ;
    }
  }
  
  /// <summary>
  ///   特定パスの画像リソースをBitmapImageとして取り出す
  /// </summary>
  /// <param name="path"></param>
  /// <returns></returns>
  public static BitmapImage ToResImageInPack( this string path )
  {
    return new BitmapImage( new Uri( $@"pack://application:,,,/MEPConnectTool;component/Res/{path}", UriKind.Absolute ) ) ;
  }
  
}
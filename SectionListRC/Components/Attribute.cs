using System;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
namespace SectionListRC.Components
{
  /// ================================================================================
  /// <summary>属性</summary>
  /// ================================================================================
  public class Attribute : SectionListRC.JExtComCompat.UtilAttrib
  {
    // コンストラクタ
    #region Member Constructor
      /// ================================================================================
      /// <summary>アセンブリ設定</summary>
      ///
      /// <history><p>2013/02/19 Created  GSA,Inc. Shinichi Ishii</p></history>
      /// ================================================================================
      public Attribute() : base()
      {
        // 属性
        base.SetAssembly(System.Reflection.Assembly.GetExecutingAssembly(),
                         "SectionListRC.Resources.Text",
                         "SectionListRC.Resources.Image",
                         System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
      }
    #endregion
  }
}

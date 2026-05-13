using System;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
namespace SectionListRC.Components
{
  /// ================================================================================
  /// <summary>設定</summary>
  /// ================================================================================
  public class Settings : SectionListRC.JExtComCompat.RvtSettings
  {
    // コンストラクタ
    #region Constructor
      /// ================================================================================
      /// <summary>コンストラクタ</summary>
      /// 
      /// <param name="rvtUIDoc">Revit UIドキュメント</p></param>
      /// 
      /// <history><p>2013/04/03 Created GSA,Inc. Ryo Kuroda</p></history>
      /// ================================================================================
      public Settings(Revit.UI.UIDocument rvtUIDoc)
        : base(rvtUIDoc)
      {
        
      }

    #endregion

    // メンバ関数
    #region Member Functions
    #endregion

    // プロパティ
    #region Properties

      /// ================================================================================
      /// <summary>カテゴリ - プロジェクト情報</summary>
      /// <history>2013/04/03 Created GSA,Inc. Ryo Kuroda</history>
      /// ================================================================================
      public Revit.DB.Category CategoryProjInfo
      {
        get
        {
          return base.GetCategory(Revit.DB.BuiltInCategory.OST_ProjectInformation);
        }
      }

      /// ================================================================================
      /// <summary>カテゴリ - 柱</summary>
      /// <history>2013/04/15 Created GSA,Inc. Ryo Kuroda</history>
      /// ================================================================================
      public Collections.Generic.IList<Revit.DB.Category> CategoryColumn
      {
        get
        {
          Collections.Generic.IList<Revit.DB.Category> ret = new Collections.Generic.List<Revit.DB.Category>();
          ret.Add(base.GetCategory(Revit.DB.BuiltInCategory.OST_Columns));
          ret.Add(base.GetCategory(Revit.DB.BuiltInCategory.OST_StructuralColumns));

          return ret;
        }
      }

      /// ================================================================================
      /// <summary>カテゴリ - 梁</summary>
      /// <history>2013/04/25 Created GSA,Inc. Ryo Kuroda</history>
      /// ================================================================================
      public Collections.Generic.IList<Revit.DB.Category> CategoryBeam
      {
        get
        {
          Collections.Generic.IList<Revit.DB.Category> ret = new Collections.Generic.List<Revit.DB.Category>();
          ret.Add(base.GetCategory(Revit.DB.BuiltInCategory.OST_StructuralFraming));

          return ret;
        }
      }

    #endregion
  }
}

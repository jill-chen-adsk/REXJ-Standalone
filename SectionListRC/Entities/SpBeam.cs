using System;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
namespace SectionListRC.Entities
{
  /// ================================================================================
  /// <summary>共有パラメータ - 梁</summary>
  /// ================================================================================
  public class SpBeam : SectionListRC.Entities.SpBase
  {
    // メンバ変数
    #region

      private Collections.Generic.IList<Revit.DB.Category> _ParamCategories;

    #endregion

    // コンストラクタ
    #region Constructor

      /// ================================================================================
      /// <summary>コンストラクタ</summary>
      ///
      /// <param name="cmpAttribute"  >属性</param>
      /// <param name="cmpParameters" >パラメータ</param>
      /// <param name="cmpSettings"   >設定</param>
      ///
      /// <history>2013/04/15 Created  GSA,Inc. Ryo Kuroda</history>
      /// ================================================================================
      public SpBeam(SectionListRC.Components.Attribute cmpAttribute,
                    SectionListRC.Components.Parameters cmpParameters,
                    SectionListRC.Components.Settings cmpSettings) :
             base(cmpAttribute, cmpParameters, cmpSettings)
      {

        // パラメータカテゴリ
        _ParamCategories = base.CmpSettings.CategoryBeam;
        base.SetDefCatName(_ParamCategories);
      }

    #endregion

    // メンバ関数
    #region Member Functions

    #endregion

    // プロパティ
    #region Properties

    #endregion
  }
}

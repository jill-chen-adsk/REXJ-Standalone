using System;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
namespace SectionListSteel.Components
{
  /// ================================================================================
  /// <summary>図形</summary>
  /// ================================================================================
  public class Geometry : SectionListSteel.JExtComCompat.RvtGeometry
  {
    // メンバ変数
    #region Member Variables

    /// <summary>属性</summary>
    private SectionListSteel.Components.Attribute _CmpAttribute;

    #endregion

    // コンストラクタ
    #region Constructor
    /// ================================================================================
    /// <summary>コンストラクタ</summary>
    /// 
    /// <param name="cmpAttribute">属性</param>
    /// <param name="rvtUIDoc"    >Revit UI ドキュメント</param>
    /// 
    /// <history>2016/08/05 Created Ryo Kuroda</history>
    /// ================================================================================
    public
    Geometry(SectionListSteel.Components.Attribute cmpAttribute,
             Revit.UI.UIDocument rvtUIDoc) :
      base (rvtUIDoc)
    {
      _CmpAttribute = cmpAttribute;
    }
    #endregion

    // メンバ関数
    #region Member Functions

    /// ================================================================================
    /// <summary>有限線分作成</summary>
    /// 
    /// <param name="p1">始点</param>
    /// <param name="p2">終点</param>
    /// 
    /// <history>2016/09/01 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.Line CreateBoundLine(Revit.DB.XYZ p1,  
                                  Revit.DB.XYZ p2)
    {
      Revit.DB.Line l = null;

      try
      {
        l = Revit.DB.Line.CreateBound(p1, p2);
      }
      catch
      {
      }

      return l;
    }

    /// ================================================================================
    /// <summary>非Nullカーブをセット</summary>
    /// 
    /// <param name="crvAry">カーブリスト</param>
    /// <param name="crv"   >カーブ</param>
    /// 
    /// <history><p>2016/09/01 Created CST,Co.Ltd. Ryo Kuroda</p>
    ///           <p>2017/06/24 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    void NotNullCurveSet(ref Collections.Generic.IList<Revit.DB.Curve> crvAry,
                         Revit.DB.Curve crv)
    {
      if (crv != null)
      {
        // 0長さ除外
        if (base.Approx0Len < crv.Length)
        {
          crvAry.Add(crv);
        }
      }
    }

    #endregion

    // プロパティ
    #region Properties

    #endregion
  }
}

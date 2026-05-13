using System;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
namespace SectionListRC.Entities
{
  /// ================================================================================
  /// <summary>データテーブル - コマンド</summary>
  /// ================================================================================
  public class DtCmd : SectionListRC.Entities.DtBase
  {
    // メンバ変数
    #region Memeber Variables

    // 共有パラメータ
    private SectionListRC.Entities.SpCmd _EntSpCmd;
    // データ
    private Collections.Generic.IList<string> _Data;

    #endregion

    // コンストラクタ
    #region Constructor
      /// ================================================================================
      /// <summary>コンストラクタ</summary>
      ///
      /// <param name="cmpAttribute"  >属性</param>
      /// <param name="cmpElements"   >要素</param>
      /// <param name="cmpGeometry"   >図形</param>
      /// <param name="cmpParameters" >パラメータ</param>
      /// <param name="cmpSettings"   >設定</param>
      /// <param name="projInfo"      >プロジェクト情報</param>
      /// <param name="defName"       >定義名</param>
      /// <param name="itemNum"       >項目数</param>
      /// 
      /// <history><p>2013/04/03 Created GSA,Inc. Ryo Kuroda</p></history>
      /// ================================================================================
      public DtCmd(SectionListRC.Components.Attribute cmpAttribute,
                   SectionListRC.Components.Elements cmpElements,
                   SectionListRC.Components.Geometry cmpGeometry,
                   SectionListRC.Components.Parameters cmpParameters,
                   SectionListRC.Components.Settings cmpSettings,
                   Revit.DB.ProjectInfo projInfo,
                   string defName,
                   int itemNum) : 
             base(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings)
    {
      //_CmpParameters = cmpParameters;

      _EntSpCmd = new SectionListRC.Entities.SpCmd(cmpAttribute,
      cmpParameters,
                                                 cmpSettings,
                                                 projInfo,
                                                 defName,
                                                 itemNum);

      if (_EntSpCmd.DefSuccess == false)
      {
        base.ErrMsg = cmpAttribute.ResourceText("IDS_ERR_PARAMDEF");

          cmpParameters.SetSharedParamDefault();
          _Data = _EntSpCmd.GetData();
          base.GetCmdValue(_Data);
        
      }
      else
      {
        _Data = _EntSpCmd.GetData();
        base.GetCmdValue(_Data);
      }
    }
    #endregion

    // メンバ関数
    #region Member Functions
      /// ================================================================================
      /// <summary>データ設定</summary>
      ///
      /// <param name="elems">要素</param>
      ///
      /// <history>2013/04/05 Created GSA,Inc. Ryo Kuroda</history>
      /// ================================================================================
      public
      void SetData()
      {
        base.SetCmdValue(ref _Data);
        _EntSpCmd.SetData(_Data);
      }
    #endregion
  }
}

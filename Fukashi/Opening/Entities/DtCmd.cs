using System;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
using RvtExtApp   = ADSK.Ext.Fukashi.Opening;

namespace ADSK.Ext.Fukashi.Opening.Entities
{
  /// ================================================================================
  /// <summary>データテーブル - コマンド</summary>
  /// ================================================================================
  public class DtCmd : DtBase
  {
    // メンバ変数
    #region Memeber Variables

    /// <summary>共有パラメータ</summary>
    private RvtExtApp.Entities.SpCmd _EntSpCmd;

    /// <summary>データ</summary>
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
    /// <history><p>2016/12/05 Created CST,Co.Ltd. Ryo Kuroda</p></history>
    /// ================================================================================
    public DtCmd(RvtExtApp.Components.Attribute cmpAttribute,
                 RvtExtApp.Components.Elements cmpElements,
                 RvtExtApp.Components.Geometry cmpGeometry,
                 RvtExtApp.Components.Parameters cmpParameters,
                 RvtExtApp.Components.Settings cmpSettings,
                 Revit.DB.ProjectInfo projInfo,
                 string defName,
                 int itemNum) :
           base(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings)
    {
      _EntSpCmd = new RvtExtApp.Entities.SpCmd(cmpAttribute,
                                               cmpParameters,
                                               cmpSettings,
                                               projInfo,
                                               defName,
                                               itemNum);

      if (_EntSpCmd.DefSuccess == false)
      {
        base.ErrMsg = cmpAttribute.ResourceText("IDS_ERR_PARAMDEF");

        // 
        if (cmpParameters.SetSharedParamDefault() == false)
        {
          string folderName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
          string fileName = "AdskFukashiShParam.txt";

          cmpParameters.SetSharedParameterFile(folderName, fileName);
        }
        else
        {
          _Data = _EntSpCmd.GetData();
          base.GetCmdValue(_Data);
        }
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
    /// <history>2016/12/05 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    void SetData()
    {
      base.SetCmdValue(ref _Data);
      _EntSpCmd.SetData(_Data);
    }
    #endregion

    // プロパティ
    #region Properties

    #endregion
  }
}

using System;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
using RvtExtApp   = ADSK.Ext.Fukashi;

namespace ADSK.Ext.Fukashi.Face.Entities
{
  /// ================================================================================
  /// <summary>データテーブル - 基底</summary>
  /// ================================================================================
  public abstract class DtBase
  {
    // メンバ変数
    #region Member Variables

    /// <summary>属性</summary>
    private RvtExtApp.Face.Components.Attribute _CmpAttribute;

    /// <summary>要素</summary>
    private RvtExtApp.Face.Components.Elements _CmpElements;

    /// <summary>図形</summary>
    private RvtExtApp.Face.Components.Geometry _CmpGeometry;

    /// <summary>パラメータ</summary>
    private RvtExtApp.Face.Components.Parameters _CmpParameters;

    /// <summary>設定</summary>
    private RvtExtApp.Face.Components.Settings _CmpSettings;

    /// <summary>エラーメッセージ</summary>
    private string _ErrMsg;

    /// <summary>マテリアル</summary>
    private string _Material;

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
    /// 
    /// <history>2016/12/05 Created Ryo Kuroda</history>
    /// ================================================================================
    public
    DtBase(RvtExtApp.Face.Components.Attribute cmpAttribute,
           RvtExtApp.Face.Components.Elements cmpElements,
           RvtExtApp.Face.Components.Geometry cmpGeometry,
           RvtExtApp.Face.Components.Parameters cmpParameters,
           RvtExtApp.Face.Components.Settings cmpSettings)
    {
      _CmpAttribute = cmpAttribute;
      _CmpElements = cmpElements;
      _CmpGeometry = cmpGeometry;
      _CmpParameters = cmpParameters;
      _CmpSettings = cmpSettings;

      _ErrMsg = "";
    }
    #endregion

    // メンバ関数
    #region Member Functions

    /// ================================================================================
    /// <summary>コマンド値取得</summary>
    ///
    /// <param name="dataAry">コマンドデータ</param>
    /// 
    /// <history>2016/12/05 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    protected
    void GetCmdValue(Collections.Generic.IList<string> dataAry)
    {
      string sValue = "";

      // マテリアル
      _Material = "";
      if (dataAry.Count > 0)
      {
        sValue = dataAry[0];
        if (sValue != null && sValue != "")
        {
          _Material = sValue;
        }
      }
    }

    /// ================================================================================
    /// <summary>コマンド値設定</summary>
    ///
    /// <param name="dataAry">コマンドデータ</param>
    /// 
    /// <history>2016/12/05 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    protected
    void SetCmdValue(ref Collections.Generic.IList<string> dataAry)
    {
      _CmpParameters.GetStrVal(ref _Material);

      if (dataAry.Count > 0)
      {
        dataAry[0] = _Material;
      }
    }

    #endregion

    // プロパティ
    #region Properties

    /// ================================================================================
    /// <summary>エラーメッセージ</summary>
    /// 
    /// <history>2016/12/05 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    string ErrMsg
    {
      get
      {
        return _ErrMsg;
      }
      set
      {
        _ErrMsg = value;
      }
    }

    /// ================================================================================
    /// <summary>コマンド値 - マテリアル</summary>
    /// 
    /// <history>2016/12/05 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    string SettingFileDirectory
    {
      get
      {
        return _Material;
      }
      set
      {
        _Material = value;
      }
    }

    #endregion
  }
}

using System;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
using RvtExtApp   = ADSK.Ext.Fukashi.Opening;

namespace ADSK.Ext.Fukashi.Opening.Entities
{
  /// ================================================================================
  /// <summary>データテーブル - 基底</summary>
  /// ================================================================================
  public abstract class DtBase
  {
    // メンバ変数
    #region Member Variables

    /// <summary>属性</summary>
    private RvtExtApp.Components.Attribute _CmpAttribute;

    /// <summary>要素</summary>
    private RvtExtApp.Components.Elements _CmpElements;

    /// <summary>図形</summary>
    private RvtExtApp.Components.Geometry _CmpGeometry;

    /// <summary>パラメータ</summary>
    private RvtExtApp.Components.Parameters _CmpParameters;

    /// <summary>設定</summary>
    private RvtExtApp.Components.Settings _CmpSettings;

    /// <summary>エラーメッセージ</summary>
    private string _ErrMsg;

    /// <summary>マテリアル</summary>
    private string _Material;

    /// <summary>オフセット値</summary>
    private string _Offset;

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
    /// <history>2016/12/05 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    DtBase(RvtExtApp.Components.Attribute cmpAttribute,
           RvtExtApp.Components.Elements cmpElements,
           RvtExtApp.Components.Geometry cmpGeometry,
           RvtExtApp.Components.Parameters cmpParameters,
           RvtExtApp.Components.Settings cmpSettings)
    {
      _CmpAttribute   = cmpAttribute;
      _CmpElements    = cmpElements;
      _CmpGeometry    = cmpGeometry;
      _CmpParameters  = cmpParameters;
      _CmpSettings    = cmpSettings;

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
    /// <history>2016/12/05 Created CST,Co.Ltd. Ryo Kuroda</history>
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

      // オフセット値
      _Offset = "";
      if (dataAry.Count > 1)
      {
        sValue = dataAry[1];
        if (sValue != null && sValue != "")
        {
          _Offset = sValue;
        }
      }
    }

    /// ================================================================================
    /// <summary>コマンド値設定</summary>
    ///
    /// <param name="dataAry">コマンドデータ</param>
    /// 
    /// <history>2016/12/05 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    protected
    void SetCmdValue(ref Collections.Generic.IList<string> dataAry)
    {
      _CmpParameters.GetStrVal(ref _Material, ref _Offset);

      if (dataAry.Count > 0)
      {
        dataAry[0] = _Material;
      }

      if (dataAry.Count > 1)
      {
        dataAry[1] = _Offset;
      }
    }
    
    #endregion

    // プロパティ
    #region Properties

    /// ================================================================================
    /// <summary>エラーメッセージ</summary>
    /// 
    /// <history>2016/12/05 Created CST,Co.Ltd. Ryo Kuroda</history>
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
    /// <history>2016/12/05 Created CST,Co.Ltd. Ryo Kuroda</history>
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

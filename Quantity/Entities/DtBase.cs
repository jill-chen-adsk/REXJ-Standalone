using System;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
namespace Quantity.Entities
{
  /// ================================================================================
  /// <summary>データテーブル - 基底</summary>
  /// ================================================================================
  public abstract class DtBase
  {
    // メンバ変数
    #region Member Variables

    /// <summary>属性</summary>
    private Quantity.Components.Attribute _CmpAttribute;

    /// <summary>要素</summary>
    private Quantity.Components.Elements _CmpElements;

    /// <summary>図形</summary>
    private Quantity.Components.Geometry _CmpGeometry;

    /// <summary>パラメータ</summary>
    private Quantity.Components.Parameters _CmpParameters;

    /// <summary>設定</summary>
    private Quantity.Components.Settings _CmpSettings;

    /// <summary>エラーメッセージ</summary>
    private string _ErrMsg;

    /// <summary>寸法タイプ 名前</summary>
    private string _DimTypeName;

    /// <summary>寸法タイプ ID</summary>
    private string _DimTypeId;

    /// <summary>文字タイプ 名前</summary>
    private string _TextTypeName;

    /// <summary>文字タイプ ID</summary>
    private string _TextTypeId;

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
    /// <history>2017/07/19 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    DtBase(Quantity.Components.Attribute cmpAttribute,
           Quantity.Components.Elements cmpElements,
           Quantity.Components.Geometry cmpGeometry,
           Quantity.Components.Parameters cmpParameters,
           Quantity.Components.Settings cmpSettings)
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
    /// <history>2017/07/19 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    protected
    void GetCmdValue(Collections.Generic.IList<string> dataAry)
    {
      string sValue = "";

      // 寸法タイプ 名前
      _DimTypeName = "";
      if (dataAry.Count > 0)
      {
        sValue = dataAry[0];
        if (sValue != null && sValue != "")
        {
          _DimTypeName = sValue;
        }
      }

      // 寸法タイプ ID
      _DimTypeId = "";
      if (dataAry.Count > 1)
      {
        sValue = dataAry[1];
        if (sValue != null && sValue != "")
        {
          _DimTypeId = sValue;
        }
      }

      // 文字タイプ 名前
      _TextTypeName = "";
      if (dataAry.Count > 2)
      {
        sValue = dataAry[2];
        if (sValue != null && sValue != "")
        {
          _TextTypeName = sValue;
        }
      }

      // 文字タイプ ID
      _TextTypeId = "";
      if (dataAry.Count > 3)
      {
        sValue = dataAry[3];
        if (sValue != null && sValue != "")
        {
          _TextTypeId = sValue;
        }
      }

    }

    /// ================================================================================
    /// <summary>コマンド値設定</summary>
    ///
    /// <param name="dataAry">コマンドデータ</param>
    /// 
    /// <history>2017/07/19 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    protected
    void SetCmdValue(ref Collections.Generic.IList<string> dataAry)
    {
      //_CmpParameters.GetStrVal(ref _DimTypeName, ref _DimTypeId, ref _TextTypeName, ref _TextTypeId);

      if (dataAry.Count > 0)
      {
        dataAry[0] = _DimTypeName;
      }

      if (dataAry.Count > 1)
      {
        dataAry[1] = _DimTypeId;
      }

      if (dataAry.Count > 2)
      {
        dataAry[2] = _TextTypeName;
      }

      if (dataAry.Count > 3)
      {
        dataAry[3] = _TextTypeId;
      }
    }
    #endregion

    // プロパティ
    #region Properties

    /// ================================================================================
    /// <summary>エラーメッセージ</summary>
    /// <history>2017/07/19 Created GSA,Inc. Ryo Kuroda</history>
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
    /// <summary>寸法タイプ 名前</summary>
    /// <history>2017/07/19 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    string DimTypeName
    {
      get
      {
        return _DimTypeName;
      }
      set
      {
        _DimTypeName = value;
      }
    }

    /// ================================================================================
    /// <summary>寸法タイプ ID</summary>
    /// <history>2017/07/19 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    string DimTypeId
    {
      get
      {
        return _DimTypeId;
      }
      set
      {
        _DimTypeId = value;
      }
    }

    /// ================================================================================
    /// <summary>文字タイプ 名前</summary>
    /// <history>2017/07/19 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    string TextTypeName
    {
      get
      {
        return _TextTypeName;
      }
      set
      {
        _TextTypeName = value;
      }
    }

    /// ================================================================================
    /// <summary>文字タイプ ID</summary>
    /// <history>2017/07/19 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    string TextTypeId
    {
      get
      {
        return _TextTypeId;
      }
      set
      {
        _TextTypeId = value;
      }
    }
    #endregion
  }
}

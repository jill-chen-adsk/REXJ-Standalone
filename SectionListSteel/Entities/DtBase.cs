using System;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
namespace SectionListSteel.Entities
{
  /// ================================================================================
  /// <summary>データテーブル - 基底</summary>
  /// ================================================================================
  public abstract class DtBase
  {
    // メンバ変数
    #region Member Variables

    /// <summary>属性</summary>
    private SectionListSteel.Components.Attribute _CmpAttribute;

    /// <summary>要素</summary>
    private SectionListSteel.Components.Elements _CmpElements;

    /// <summary>図形</summary>
    private SectionListSteel.Components.Geometry _CmpGeometry;

    /// <summary>パラメータ</summary>
    private SectionListSteel.Components.Parameters _CmpParameters;

    /// <summary>設定</summary>
    private SectionListSteel.Components.Settings _CmpSettings;

    /// <summary>エラーメッセージ</summary>
    private string _ErrMsg;

    /// <summary>設定ファイル名</summary>
    private string _SettingFileName;

    /// <summary>設定ファイルディレクトリ</summary>
    private string _SettingFileDirectory;

    /// <summary>階記号ソート順序</summary>
    private string _LevelSortOrder;

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
    /// <history>2016/08/05 Created Ryo Kuroda</history>
    /// ================================================================================
    public
    DtBase(SectionListSteel.Components.Attribute cmpAttribute,
           SectionListSteel.Components.Elements cmpElements,
           SectionListSteel.Components.Geometry cmpGeometry,
           SectionListSteel.Components.Parameters cmpParameters,
           SectionListSteel.Components.Settings cmpSettings)
    {
      _CmpAttribute   = cmpAttribute;
      _CmpElements    = cmpElements;
      _CmpGeometry    = cmpGeometry;
      _CmpParameters  = cmpParameters;
      _CmpSettings    = cmpSettings;
      _ErrMsg         = "";
    }
    #endregion

    // メンバ関数
    #region Member Functions

    /// ================================================================================
    /// <summary>コマンド値取得</summary>
    ///
    /// <param name="dataAry">コマンドデータ</param>
    /// 
    /// <history>2016/08/05 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    protected
    void GetCmdValue(Collections.Generic.IList<string> dataAry)
    {
      string sValue = "";

      // 設定ファイルディレクトリ
      _SettingFileDirectory = "";
      if (dataAry.Count > 0)
      {
        sValue = dataAry[0];
        if (sValue != null && sValue != "")
        {
          _SettingFileDirectory = sValue;
        }
      }

      // 設定ファイル名
      _SettingFileName = "";
      if (dataAry.Count > 1)
      {
        sValue = dataAry[1];
        if (sValue != null && sValue != "")
        {
          _SettingFileName = sValue;
        }
      }

      // 階記号ソート順序
      _LevelSortOrder = "";
      if (dataAry.Count > 2)
      {
        sValue = dataAry[2];
        if (sValue != null && sValue != "")
        {
          _LevelSortOrder = sValue;
        }
      }
    }

    /// ================================================================================
    /// <summary>コマンド値設定</summary>
    ///
    /// <param name="dataAry">コマンドデータ</param>
    /// 
    /// <history>2016/08/05 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    protected
    void SetCmdValue(ref Collections.Generic.IList<string> dataAry)
    {
      _CmpParameters.GetStrVal(ref _SettingFileName, ref _SettingFileDirectory, ref _LevelSortOrder);

      if (dataAry.Count > 0)
      {
        dataAry[0] = _SettingFileDirectory;
      }

      if (dataAry.Count > 1)
      {
        dataAry[1] = _SettingFileName;
      }

      if (dataAry.Count > 2)
      {
        dataAry[2] = _LevelSortOrder;
      }
    }

    #endregion

    // プロパティ
    #region Properties
    
    /// ================================================================================
    /// <summary>エラーメッセージ</summary>
    /// <history>2016/08/05 Created GSA,Inc. Ryo Kuroda</history>
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
    /// <summary>コマンド値 - 設定ファイル名</summary>
    /// <history>2016/08/05 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    string SettingFileName
    {
      get
      {
        return _SettingFileName;
      }
      set
      {
        _SettingFileName = value;
      }
    }

    /// ================================================================================
    /// <summary>コマンド値 - 設定ファイルディレクトリ</summary>
    /// <history>2016/08/05 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    string SettingFileDirectory
    {
      get
      {
        return _SettingFileDirectory;
      }
      set
      {
        _SettingFileDirectory = value;
      }
    }

    /// ================================================================================
    /// <summary>コマンド値 - 階記号ソート順序</summary>
    /// <history>2016/08/05 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    string LevelSortOrdeer
    {
      get
      {
        return _LevelSortOrder;
      }
      set
      {
        _LevelSortOrder = value;
      }
    }

    #endregion
  }
}

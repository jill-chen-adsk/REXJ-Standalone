using System;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
using RvtExtApp   = ADSK.Ext.Fukashi;

namespace ADSK.Ext.Fukashi.Face.Entities
{
  /// ================================================================================
  /// <summary>共有パラメータ - 基底</summary>
  /// ================================================================================
  public abstract class SpBase
  {
    // メンバ変数
    #region Member Variables

    /// <summary>属性</summary>
    private RvtExtApp.Face.Components.Attribute _CmpAttribute;

    /// <summary>パラメータ</summary>
    private RvtExtApp.Face.Components.Parameters _CmpParameters;

    /// <summary>設定</summary>
    private RvtExtApp.Face.Components.Settings _CmpSettings;

    /// <summary>定義成功</summary>
    private bool _DefSuccess;

    /// <summary>エラー定義名</summary>
    private string _ErrDefName;

    /// <summary>定義カテゴリ名</summary>
    private string _DefCatName;

    /// <summary>現在要素</summary>
    private Revit.DB.Element _CurrentElem;

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
    /// <history><p>2016/12/05 Created GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    protected SpBase(RvtExtApp.Face.Components.Attribute cmpAttribute,
                     RvtExtApp.Face.Components.Parameters cmpParameters,
                     RvtExtApp.Face.Components.Settings cmpSettings)
    {
      _CmpAttribute = cmpAttribute;
      _CmpParameters = cmpParameters;
      _CmpSettings = cmpSettings;
      _DefSuccess = true;
      _ErrDefName = "";
      _DefCatName = "";
    }

    #endregion

    // メンバ関数
    #region Member Functions

    /// <remarks>Standalone replacement for JExtCom.Dnf.UtilValue.SplitString.</remarks>
    protected static Collections.Generic.IList<string> SplitDelimitedStrings(string raw, string separator)
    {
      Collections.Generic.List<string> list = new Collections.Generic.List<string>();
      if (string.IsNullOrEmpty(raw))
      {
        return list;
      }
      string sep = string.IsNullOrEmpty(separator) ? "," : separator;
      foreach (string p in raw.Split(new[] { sep }, StringSplitOptions.None))
      {
        list.Add(p);
      }
      return list;
    }

    /// ================================================================================
    /// <summary>定義カテゴリ名設定</summary>
    ///
    /// <param name="category">カテゴリ</param>
    /// 
    /// <history>2016/12/05 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    protected
    void SetDefCatName(Revit.DB.Category category)
    {
      Collections.Generic.IList<string> defCatNames =
        SplitDelimitedStrings(_DefCatName, ",");

      if (defCatNames.Contains(category.Name) == false)
      {
        if (_DefCatName != "")
        {
          _DefCatName += ",";
        }

        _DefCatName += category.Name;
      }
    }

    /// ================================================================================
    /// <summary>定義カテゴリ名設定(オーバーロード)</summary>
    ///
    /// <param name="category">カテゴリ</param>
    /// 
    /// <history>2016/12/05 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    protected
    void SetDefCatName(Collections.Generic.IList<Revit.DB.Category> categories)
    {
      foreach (Revit.DB.Category cat in categories)
      {
        SetDefCatName(cat);
      }
    }

    #endregion

    // プロパティ
    #region Properties

    /// ================================================================================
    /// <summary>属性</summary>
    /// 
    /// <history>2016/12/05 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    protected
    RvtExtApp.Face.Components.Attribute CmpAttribute
    {
      get
      {
        return _CmpAttribute;
      }
    }

    /// ================================================================================
    /// <summary>パラメータ</summary>
    /// 
    /// <history>2016/12/05 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    protected
    RvtExtApp.Face.Components.Parameters CmpParameters
    {
      get
      {
        return _CmpParameters;
      }
    }

    /// ================================================================================
    /// <summary>設定</summary>
    /// 
    /// <history>2016/12/05 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    protected
    RvtExtApp.Face.Components.Settings CmpSettings
    {
      get
      {
        return _CmpSettings;
      }
    }

    /// ================================================================================
    /// <summary>定義成功</summary>
    /// 
    /// <history>2016/12/05 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    bool DefSuccess
    {
      get
      {
        return _DefSuccess;
      }
      set
      {
        _DefSuccess = value;
      }
    }

    /// ================================================================================
    /// <summary>エラー定義名</summary>
    /// 
    /// <history>2016/12/05 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    string ErrDefName
    {
      get
      {
        return _ErrDefName;
      }
      set
      {
        _ErrDefName = value;
      }
    }

    /// ================================================================================
    /// <summary>定義カテゴリ名</summary>
    /// 
    /// <history>2016/12/05 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    string DefCatName
    {
      get
      {
        return _DefCatName;
      }
    }

    /// ================================================================================
    /// <summary>現在要素</summary>
    /// 
    /// <history>2016/12/05 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.Element CurrentElem
    {
      get
      {
        return _CurrentElem;
      }
      set
      {
        _CurrentElem = value;
      }
    }

    #endregion
  }
}

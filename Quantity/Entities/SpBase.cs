using System;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
namespace Quantity.Entities
{
  /// ================================================================================
  /// <summary>共有パラメータ - 基底</summary>
  /// ================================================================================
  public abstract class SpBase
  {
    // メンバ変数
    #region Member Variables

    /// <summary>属性</summary>
    private Quantity.Components.Attribute _CmpAttribute;

    /// <summary>パラメータ</summary>
    private Quantity.Components.Parameters _CmpParameters;

    /// <summary>設定</summary>
    private Quantity.Components.Settings _CmpSettings;

    /// <summary>定義成功</summary>
    private bool _DefSuccess;

    /// <summary>エラー定義名</summary>
    private string _ErrDefName;
    
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
    /// <history>2017/07/19 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    protected SpBase(Quantity.Components.Attribute cmpAttribute,
                     Quantity.Components.Parameters cmpParameters,
                     Quantity.Components.Settings cmpSettings)
    {
      _CmpAttribute   = cmpAttribute;
      _CmpParameters  = cmpParameters;
      _CmpSettings    = cmpSettings;
      _DefSuccess     = true;
      _ErrDefName     = "";
    }

    #endregion

    // メンバ関数
    #region Member Functions

    #endregion

    // プロパティ
    #region Properties

    /// ================================================================================
    /// <summary>属性</summary>
    /// 
    /// <history>2017/07/19 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    protected
    Quantity.Components.Attribute CmpAttribute
    {
      get
      {
        return _CmpAttribute;
      }
    }

    /// ================================================================================
    /// <summary>パラメータ</summary>
    /// 
    /// <history>2017/07/19 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    protected
    Quantity.Components.Parameters CmpParameters
    {
      get
      {
        return _CmpParameters;
      }
    }

    /// ================================================================================
    /// <summary>設定</summary>
    /// 
    /// <history>2017/07/19 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    protected
    Quantity.Components.Settings CmpSettings
    {
      get
      {
        return _CmpSettings;
      }
    }

    /// ================================================================================
    /// <summary>定義成功</summary>
    /// 
    /// <history>2017/07/19 Created CST,Co.Ltd. Ryo Kuroda</history>
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
    /// <history>2017/07/19 Created CST,Co.Ltd. Ryo Kuroda</history>
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
    /// <summary>現在要素</summary>
    /// 
    /// <history>2017/07/19 Created CST,Co.Ltd. Ryo Kuroda</history>
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

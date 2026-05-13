using System ;
using System.Text ;
using Autodesk.Revit.DB ;
using Collections = System.Collections ;
using Revit = Autodesk.Revit ;
using RvtExtApp = ADSK.JExtRAC.AreaSchedule ;

namespace ADSK.JExtRAC.AreaSchedule.Entities
{
  /// ================================================================================
  /// <summary>共有パラメータ - エリア</summary>
  /// ================================================================================
  public class SpArea : RvtExtApp.Entities.SpBase
  {
    // メンバ変数

    #region Memeber Variables

    /// <summary>定義値 部屋名</summary>
    private ParamDefStrc _DefValRoomName ;

    /// <summary>定義値 部屋番号</summary>
    private ParamDefStrc _DefValRoomNo ;

    /// <summary>定義値 面積根拠式</summary>
    private ParamDefStrc _DefValAreaExpn ;

    /// <summary>定義値 計算面積</summary>
    private ParamDefStrc _DefValAreaCalc ;

    /// <summary>定義値 枝番号</summary>
    private ParamDefStrc _DefValBranchNo ;

    /// <summary>パラメータカテゴリ</summary>
    private Revit.DB.Category _ParamCategory ;

    #endregion Memeber Variables

    // コンストラクタ

    #region Constructor

    /// ================================================================================
    /// <summary>コンストラクタ</summary>
    ///
    /// <param name="cmpAttribute"  >属性</param>
    /// <param name="cmpParameters" >パラメータ</param>
    /// <param name="cmpSettings"   >設定</param>
    ///
    /// <history>2011/07/27 Created  GSA,Inc. Shinichi Ishii</history>
    /// ================================================================================
    public SpArea( RvtExtApp.Components.Attribute cmpAttribute, RvtExtApp.Components.Parameters cmpParameters, RvtExtApp.Components.Settings cmpSettings ) : base( cmpAttribute, cmpParameters, cmpSettings )
    {
      // パラメータカテゴリ
      _ParamCategory = base.CmpSettings.CategoryArea ;
      base.SetDefCatName( _ParamCategory ) ;

      // 定義設定
      SetDef() ;
    }

    #endregion Constructor

    // メンバ関数

    #region Member Functions

    /// ================================================================================
    /// <summary>定義設定</summary>
    ///
    /// <returns><p>結果</p>
    ///             <p>True  = 成功</p>
    ///             <p>False = 失敗</p></returns>
    ///
    /// <history>2011/07/27 Created GSA,Inc. Shinichi Ishii</history>
    /// ================================================================================
    private void SetDef()
    {
      // 初期化
      bool success = true ;

      // 部屋名
      if ( success == true ) {
        success = DefRoomName() ;
      }

      // 部屋番号
      if ( success == true ) {
        success = DefRoomNo() ;
      }

      // 面積根拠式
      if ( success == true ) {
        success = DefAreaExpn() ;
      }

      // 計算面積
      if ( success == true ) {
        success = DefAreaCalc() ;
      }

      // 枝番号
      if ( success == true ) {
        success = DefBranchNo() ;
      }

      // 実行状態
      base.DefSuccess = success ;
    }

    /// ================================================================================
    /// <summary>定義 部屋名</summary>
    ///
    /// <returns><p>結果</p>
    ///             <p>True  = 成功</p>
    ///             <p>False = 失敗</p></returns>
    ///
    /// <history>2011/07/27 Created GSA,Inc. Shinichi Ishii</history>
    /// ================================================================================
    private bool DefRoomName()
    {

      _DefValRoomName = new ParamDefStrc( _ParamCategory, base.CmpAttribute.ResourceText( "IDS_SHPARAM_DEF_AREA_ROOMNAME" ), Revit.DB.SpecTypeId.String.Text, new ForgeTypeId(string.Empty), true, 0 ) ;
      

      bool ret = base.CmpParameters.SetDefinition( null, _DefValRoomName.Category, _DefValRoomName.DefName, _DefValRoomName.ParamType, _DefValRoomName.BltParamGroup, _DefValRoomName.Visible, _DefValRoomName.BindingMode ) ;
      if ( ret == false ) {
        base.ErrDefName = _DefValRoomName.DefName ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>定義 部屋番号</summary>
    ///
    /// <returns><p>結果</p>
    ///             <p>True  = 成功</p>
    ///             <p>False = 失敗</p></returns>
    ///
    /// <history>2011/07/27 Created GSA,Inc. Shinichi Ishii</history>
    /// ================================================================================
    private bool DefRoomNo()
    {

      _DefValRoomNo = new ParamDefStrc( _ParamCategory, base.CmpAttribute.ResourceText( "IDS_SHPARAM_DEF_AREA_ROOMNO" ), Revit.DB.SpecTypeId.String.Text, new ForgeTypeId(string.Empty), true, 0 ) ;


      bool ret = base.CmpParameters.SetDefinition( null, _DefValRoomNo.Category, _DefValRoomNo.DefName, _DefValRoomNo.ParamType, _DefValRoomNo.BltParamGroup, _DefValRoomNo.Visible, _DefValRoomNo.BindingMode ) ;
      if ( ret == false ) {
        base.ErrDefName = _DefValRoomNo.DefName ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>定義 面積根拠式</summary>
    ///
    /// <returns><p>結果</p>
    ///             <p>True  = 成功</p>
    ///             <p>False = 失敗</p></returns>
    ///
    /// <history>2011/07/27 Created GSA,Inc. Shinichi Ishii</history>
    /// ================================================================================
    private bool DefAreaExpn()
    {

      _DefValAreaExpn = new ParamDefStrc( _ParamCategory, base.CmpAttribute.ResourceText( "IDS_SHPARAM_DEF_AREA_AREAEXPN" ), Revit.DB.SpecTypeId.String.Text, GroupTypeId.Geometry, true, 0 ) ;

      bool ret = base.CmpParameters.SetDefinition( null, _DefValAreaExpn.Category, _DefValAreaExpn.DefName, _DefValAreaExpn.ParamType, _DefValAreaExpn.BltParamGroup, _DefValAreaExpn.Visible, _DefValAreaExpn.BindingMode ) ;
      if ( ret == false ) {
        base.ErrDefName = _DefValAreaExpn.DefName ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>定義 計算面積</summary>
    ///
    /// <returns><p>結果</p>
    ///             <p>True  = 成功</p>
    ///             <p>False = 失敗</p></returns>
    ///
    /// <history>2011/07/27 Created GSA,Inc. Shinichi Ishii</history>
    /// ================================================================================
    private bool DefAreaCalc()
    {

      _DefValAreaCalc = new ParamDefStrc( _ParamCategory, base.CmpAttribute.ResourceText( "IDS_SHPARAM_DEF_AREA_AREACALC" ), Revit.DB.SpecTypeId.Area, GroupTypeId.Geometry, true, 0 ) ;


      bool ret = base.CmpParameters.SetDefinition( null, _DefValAreaCalc.Category, _DefValAreaCalc.DefName, _DefValAreaCalc.ParamType, _DefValAreaCalc.BltParamGroup, _DefValAreaCalc.Visible, _DefValAreaCalc.BindingMode ) ;
      if ( ret == false ) {
        base.ErrDefName = _DefValAreaCalc.DefName ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>定義 枝番号</summary>
    ///
    /// <returns><p>結果</p>
    ///             <p>True  = 成功</p>
    ///             <p>False = 失敗</p></returns>
    ///
    /// <history>2011/07/27 Created GSA,Inc. Shinichi Ishii</history>
    /// ================================================================================
    private bool DefBranchNo()
    {

      _DefValBranchNo = new ParamDefStrc( _ParamCategory, base.CmpAttribute.ResourceText( "IDS_SHPARAM_DEF_AREA_BRANCHNUMBER" ), Revit.DB.SpecTypeId.String.Text, GroupTypeId.IdentityData, true, 0 ) ;

      bool ret = base.CmpParameters.SetDefinition( null, _DefValBranchNo.Category, _DefValBranchNo.DefName, _DefValBranchNo.ParamType, _DefValBranchNo.BltParamGroup, _DefValBranchNo.Visible, _DefValBranchNo.BindingMode ) ;
      if ( ret == false ) {
        base.ErrDefName = _DefValBranchNo.DefName ;
      }

      return ret ;
    }

    #endregion Member Functions

    // プロパティ

    #region Properties

    /// ================================================================================
    /// <summary>部屋名</summary>
    /// <history><p>2011/07/27 Created  GSA,Inc. Shinichi Ishii</p>
    ///          <p>2012/04/19 Modified GSA,Inc. Shinichi Ishii</p></history>
    /// ================================================================================
    public string RoomName
    {
      get
      {
        string ret = "" ;
        if ( base.CurrentElem != null ) {
          if ( base.CmpParameters.GetValue( base.CurrentElem, _DefValRoomName.DefName, _DefValRoomName.ParamType, _DefValRoomName.BltParamGroup, ref ret ) < -1 ) {
          }
        }

        return ret ;
      }
      set
      {
        if ( base.CurrentElem != null ) {
          if ( base.CmpParameters.SetValue( base.CurrentElem, _DefValRoomName.DefName, _DefValRoomName.ParamType, _DefValRoomName.BltParamGroup, value ) < -1 ) {
          }
        }
      }
    }

    /// ================================================================================
    /// <summary>部屋番号</summary>
    /// <history><p>2011/07/27 Created  GSA,Inc. Shinichi Ishii</p>
    ///          <p>2012/04/19 Modified GSA,Inc. Shinichi Ishii</p></history>
    /// ================================================================================
    public string RoomNo
    {
      get
      {
        string ret = "" ;
        if ( base.CurrentElem != null && _DefValRoomNo != null ) {
          if ( base.CmpParameters.GetValue( base.CurrentElem, _DefValRoomNo.DefName, _DefValRoomNo.ParamType, _DefValRoomNo.BltParamGroup, ref ret ) < -1 ) {
          }
        }

        return ret ;
      }
      set
      {
        if ( base.CurrentElem != null ) {
          if ( base.CmpParameters.SetValue( base.CurrentElem, _DefValRoomNo.DefName, _DefValRoomNo.ParamType, _DefValRoomNo.BltParamGroup, value ) < -1 ) {
          }
        }
      }
    }

    /// ================================================================================
    /// <summary>面積根拠式</summary>
    /// <history><p>2011/07/27 Created  GSA,Inc. Shinichi Ishii</p>
    ///          <p>2012/04/19 Modified GSA,Inc. Shinichi Ishii</p></history>
    /// ================================================================================
    public string AreaExpn
    {
      get
      {
        string ret = "" ;
        if ( base.CurrentElem != null ) {
          if ( base.CmpParameters.GetValue( base.CurrentElem, _DefValAreaExpn.DefName, _DefValAreaExpn.ParamType, _DefValAreaExpn.BltParamGroup, ref ret ) < -1 ) {
          }
        }

        return ret ;
      }
      set
      {
        base.CurrentElem?.LookupParameter( _DefValAreaExpn?.DefName )?.Set( value ) ;
      }
    }

    /// ================================================================================
    /// <summary>計算面積</summary>
    /// <history><p>2011/07/27 Created  GSA,Inc. Shinichi Ishii</p>
    ///          <p>2012/04/19 Modified GSA,Inc. Shinichi Ishii</p></history>
    /// ================================================================================
    public double AreaCalc
    {
      get
      {
        double ret = 0.0 ;
        if ( base.CurrentElem != null && _DefValAreaCalc != null ) {
          if ( base.CmpParameters.GetValue( base.CurrentElem, _DefValAreaCalc.DefName, _DefValAreaCalc.ParamType, _DefValAreaCalc.BltParamGroup, ref ret ) < -1 ) {
          }
        }

        return ret ;
      }
    }

    /// ================================================================================
    /// <summary>計算面積</summary>
    /// <history><p>2011/07/27 Created  GSA,Inc. Shinichi Ishii</p>
    ///          <p>2012/04/19 Modified GSA,Inc. Shinichi Ishii</p></history>
    /// ================================================================================
    public string AreaCalcStr
    {
      set
      {
        if ( base.CurrentElem != null && _DefValAreaCalc != null) {
          base.CurrentElem.LookupParameter( _DefValAreaCalc?.DefName )?.SetValueString( value ) ;
        }
      }
    }

    /// ================================================================================
    /// <summary>枝番号</summary>
    /// <history><p>2011/07/27 Created  GSA,Inc. Shinichi Ishii</p>
    ///          <p>2012/04/19 Modified GSA,Inc. Shinichi Ishii</p></history>
    /// ================================================================================
    public string BranchNo
    {
      get
      {
        string ret = "" ;
        if ( base.CurrentElem != null ) {
          if ( base.CmpParameters.GetValue( base.CurrentElem, _DefValBranchNo.DefName, _DefValBranchNo.ParamType, _DefValBranchNo.BltParamGroup, ref ret ) < -1 ) {
          }
        }

        return ret ;
      }
      set
      {
        if ( base.CurrentElem != null && _DefValBranchNo != null ) {
          if ( base.CmpParameters.SetValue( base.CurrentElem, _DefValBranchNo.DefName, _DefValBranchNo.ParamType, _DefValBranchNo.BltParamGroup, value ) < -1 ) {
          }
        }
      }
    }

    /// ================================================================================
    /// <summary>エリア名</summary>
    /// <history><p>2011/07/27 Created  GSA,Inc. Shinichi Ishii</p>
    ///          <p>2012/04/19 Modified GSA,Inc. Shinichi Ishii</p></history>
    /// ================================================================================
    public string AreaName
    {
      get
      {
        string ret = "" ;
        if ( base.CurrentElem != null ) {
          if ( base.CmpParameters.GetValue( base.CurrentElem, Revit.DB.BuiltInParameter.ROOM_NAME, ref ret ) < -1 ) {
          }
        }

        return ret ;
      }
      set
      {
        if ( base.CurrentElem != null ) {
          if ( base.CmpParameters.SetValue( base.CurrentElem, Revit.DB.BuiltInParameter.ROOM_NAME, value ) < -1 ) {
          }
        }
      }
    }

    #endregion Properties
  }
}
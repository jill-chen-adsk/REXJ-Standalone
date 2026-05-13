using System ;
using Autodesk.Revit.DB ;
using Collections = System.Collections ;
using Revit = Autodesk.Revit ;
using RvtExtApp = ADSK.JExtRAC.AreaSchedule ;
using ADSK.JExtRAC.AreaSchedule.Utils;

namespace ADSK.JExtRAC.AreaSchedule.Entities
{
  /// ================================================================================
  /// <summary>共有パラメータ - コマンド</summary>
  /// ================================================================================
  public class SpCmd : RvtExtApp.Entities.SpBase
  {
    // メンバ変数

    #region Memeber Variables

    /// <summary>プロジェクト情報要素</summary>
    private Revit.DB.ProjectInfo _ElemProjInfo ;

    /// <summary>パラメータ名 コマンド</summary>
    private string _ParamNameCmd ;

    /// <summary>項目数</summary>
    private int _ItemNum ;

    #endregion Memeber Variables

    // コンストラクタ

    #region Constructor

    /// ================================================================================
    /// <summary>コンストラクタ</summary>
    ///
    /// <param name="cmpAttribute"  >属性</param>
    /// <param name="cmpParameters" >パラメータ</param>
    /// <param name="cmpSettings"   >設定</param>
    /// <param name="elemProjInfo"  >プロジェクト情報</param>
    /// <param name="defName"       >定義名</param>
    /// <param name="itemNum"       >項目数</param>
    ///
    /// <history>2011/07/27 Created  GSA,Inc. Shinichi Ishii</history>
    /// ================================================================================
    public SpCmd( RvtExtApp.Components.Attribute cmpAttribute, RvtExtApp.Components.Parameters cmpParameters, RvtExtApp.Components.Settings cmpSettings, Revit.DB.ProjectInfo elemProjInfo, string defName, int itemNum ) : base( cmpAttribute, cmpParameters, cmpSettings )
    {
      // 初期化
      _ElemProjInfo = elemProjInfo ;

      // パラメータ名
      _ParamNameCmd = defName ;

      // 項目数
      _ItemNum = itemNum ;

      // 定義設定
      base.DefSuccess = SetDef() ;
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
    private bool SetDef()
    {

      return base.CmpParameters.SetDefinition( null, base.CmpSettings.CategoryProjInfo, _ParamNameCmd, Revit.DB.SpecTypeId.String.Text, new ForgeTypeId(string.Empty), false, 0 ) ;
    }

    /// ================================================================================
    /// <summary>データ取得</summary>
    ///
    /// <returns>データ</returns>
    ///
    /// <history><p>2011/07/27 Created  GSA,Inc. Shinichi Ishii</p>
    ///          <p>2012/04/19 Modified GSA,Inc. Shinichi Ishii</p></history>
    /// ================================================================================
    public Collections.Generic.IList<string> GetData()
    {
      string sValue = "" ;
      Collections.Generic.IList<string> valueSplit ;

      // 戻り値
      Collections.Generic.IList<string> ret = new Collections.Generic.List<string>() ;

      // データ分割

      if ( base.CmpParameters.GetValue( _ElemProjInfo, _ParamNameCmd, Revit.DB.SpecTypeId.String.Text, new ForgeTypeId(string.Empty), ref sValue ) < -1 ) {
      }


      valueSplit = UtilValue.SplitString( sValue, "," ) ;
      bool flag = false ;
      if ( _ItemNum == valueSplit.Count ) {
        flag = true ;
      }

      // 値取得
      if ( _ItemNum > 0 ) {
        for ( int i = 0 ; i < _ItemNum ; ++i ) {
          if ( flag == true ) {
            ret.Add( valueSplit[ i ] ) ;
          }
          else {
            ret.Add( "" ) ;
          }
        }
      }
      else {
        if ( valueSplit.Count > 0 ) {
          for ( int i = 0 ; i < valueSplit.Count ; ++i ) {
            ret.Add( valueSplit[ i ] ) ;
          }
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>データ設定</summary>
    ///
    /// <param name="value" >データ</param>
    ///
    /// <returns><p>結果</p>
    ///             <p>True  = 成功</p>
    ///             <p>False = 失敗</p></returns>
    ///
    /// <history>2011/07/27 Created GSA,Inc. Shinichi Ishii</history>
    /// ================================================================================
    public bool SetData( Collections.Generic.IList<string> value )
    {
      string valueStr = null ;
      string separator = "," ;

      // 戻り値
      bool ret = false ;

      // 値設定
      if ( value != null ) {
        foreach ( string str in value ) {
          valueStr += str + separator ;
        }
      }

      if ( valueStr != null ) {
        valueStr = valueStr.Substring( 0, valueStr.Length - 1 ) ;
      }

      // パラメータ値設定
      if ( valueStr != null ) {

        base.CmpParameters.SetValue( _ElemProjInfo, _ParamNameCmd, Revit.DB.SpecTypeId.String.Text, new ForgeTypeId(string.Empty), valueStr ) ;

        ret = true ;
      }

      return ret ;
    }

    #endregion Member Functions

    // プロパティ
  }
}
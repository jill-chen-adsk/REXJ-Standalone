using System ;
using Collections = System.Collections ;
using Revit = Autodesk.Revit ;
using System.Collections.Generic ;
using System.Data ;
using Autodesk.Revit.DB ;
using System.Linq ;
using System.Text;
using SectionListRC.Utils;

namespace SectionListRC.Components
{
  /// ================================================================================
  /// <summary>サービス</summary>
  /// ================================================================================
  internal class Service
  {
    // メンバ変数

    #region Memeber Variables

    /// <summary>属性</summary>
    private SectionListRC.Components.Attribute _CmpAttribute ;

    private SectionListRC.Components.Elements _CmpElements ;
    private SectionListRC.Components.Geometry _CmpGeometry ;
    private SectionListRC.Components.Parameters _CmpParameters ;
    private SectionListRC.Components.Settings _CmpSettings ;

    private SectionListRC.Entities.DtCmd entDtCmd ;

    /// <summary>X2段筋有無</summary>
    private bool _X2ndRebarIs ;

    /// <summary>Y2段筋有無</summary>
    private bool _Y2ndRebarIs ;

    /// <summary>X2段筋間隔</summary>
    private double _X2ndRebarDistance ;

    /// <summary>Y2段筋間隔</summary>
    private double _Y2ndRebarDistance ;

    /// <summary>主筋太径</summary>
    private double _SyukinHutokei ;

    /// <summary>上端筋最下段</summary>
    private Revit.DB.XYZ _InnerTop ;

    /// <summary>下端筋最上段</summary>
    private Revit.DB.XYZ _InnerBottom ;

    /// <summary>上端筋中間 </summary>
    private Revit.DB.XYZ _RebarTop ;

    /// <summary>下端筋中間</summary>
    private Revit.DB.XYZ _RebarBtm ;

    /// <summary>トランザクション</summary>
    public Revit.DB.Transaction trans ;

    /// <summary>タイプIDとイメージパス</summary>
    private Collections.Generic.IDictionary<long, string> _DicTypeId_ImagePath ;

    #endregion Memeber Variables

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
    /// <history>2013/04/03 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public Service( SectionListRC.Components.Attribute cmpAttribute, SectionListRC.Components.Elements cmpElements, SectionListRC.Components.Geometry cmpGeometry, SectionListRC.Components.Parameters cmpParameters, SectionListRC.Components.Settings cmpSettings )
    {
      _CmpAttribute = cmpAttribute ;
      _CmpElements = cmpElements ;
      _CmpGeometry = cmpGeometry ;
      _CmpParameters = cmpParameters ;
      _CmpSettings = cmpSettings ;

      _X2ndRebarIs = false ;
      _Y2ndRebarIs = false ;

      _DicTypeId_ImagePath = new Collections.Generic.Dictionary<long, string>() ;
    }

    #endregion Constructor

    // メンバ関数

    #region Member Functions

    /// ================================================================================
    /// <summary>ワークフロー</summary>
    /// <history>2013/04/05 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string WorkFlow( string parameterName )
    {
      string ret = null ;

      // プロジェクト情報
      Revit.DB.ProjectInfo projInfo = _CmpElements.ProjectInfo ;

      entDtCmd = new SectionListRC.Entities.DtCmd( _CmpAttribute, _CmpElements, _CmpGeometry, _CmpParameters, _CmpSettings, projInfo, parameterName, 3 ) ;

      return ret ;
    }

    /// ================================================================================
    /// <summary>設定</summary>
    /// <history>2013/04/05 Created  GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public void Set()
    {
      entDtCmd.SetData() ;
    }

    /// ================================================================================
    /// <summary>設定ファイル情報</summary>
    /// <history>2013/07/31 Created  GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public void SetInfoFile( string settingFileName, string settingFileDirectory, Revit.DB.Transaction trans )
    {
      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
      string infoDirectory = System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location ) ;
      string infoName = "SettingFlieInfo.txt" ;

      string infoFile = infoDirectory + "\\" + infoName ;

      if ( System.IO.File.Exists( infoFile ) ) {
        System.Text.Encoding enc = System.Text.Encoding.GetEncoding( "shift_jis" ) ;

        string write = settingFileDirectory + "\r\n" + settingFileName ;

        trans.Start( "write" ) ;
        System.IO.File.WriteAllText( infoFile, write, enc ) ;
        trans.Commit() ;
      }
      else {
        trans.Start( "create" ) ;
        System.IO.File.Create( infoFile ).Close() ;
        trans.Commit() ;

        System.Text.Encoding enc = System.Text.Encoding.GetEncoding( "shift_jis" ) ;

        string write = settingFileDirectory + "\r\n" + settingFileName ;

        trans.Start( "write" ) ;
        System.IO.File.WriteAllText( infoFile, write, enc ) ;
        trans.Commit() ;
      }
    }

    /// ================================================================================
    /// <summary>文字列取得</summary>
    ///
    /// <history><p>2013/04/05 Created  GSA,Inc. Ryo Kuroda
    ///             2013/07/31 Modified GSA,Inc. Ryo Kuroda
    ///             2013/09/03 Modified GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public void GetString( ref string settingFileName, ref string settingFileDirectory, ref string levelSortOrder )
    {
      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
      settingFileName = entDtCmd.SettingFileName ;
      settingFileDirectory = entDtCmd.SettingFileDirectory ;
      levelSortOrder = entDtCmd.LevelSortOrdeer ;

      settingFileName = "" ;
      settingFileDirectory = "" ;

      string infoDirectory = System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location ) ;
      string infoName = "SettingFlieInfo.txt" ;

      string infoFile = infoDirectory + "\\" + infoName ;

      if ( System.IO.File.Exists( infoFile ) ) {
        System.Text.Encoding enc = System.Text.Encoding.GetEncoding( "shift_jis" ) ;
        string[] strAry = System.IO.File.ReadAllLines( infoFile, enc ) ;

        settingFileDirectory = strAry[ 0 ] ;

        if ( strAry.Length > 1 ) {
          settingFileName = strAry[ 1 ] ;
        }

        // 初回設定ファイル
        if ( settingFileDirectory == "Default" ) {
          settingFileDirectory = System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location ) + "\\" ;
          settingFileName = "SettingFile.txt" ;
        }

        string fullName = settingFileDirectory + settingFileName ;
        if ( ! System.IO.File.Exists( fullName ) ) {
          settingFileName = "" ;
          settingFileDirectory = "" ;
        }
      }

      //if (settingFileName == null || settingFileName == "")
      //{
      //  string full = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + "SettingFile.txt";

      //  // デフォルトの設定ファイルがある場合
      //  if (System.IO.File.Exists(full))
      //  {
      //    settingFileName       = "SettingFile.txt";
      //    settingFileDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";
      //  }
      //  // ない場合
      //  else
      //  {
      //    // 実行フォルダにデフォルト値で作成

      //    settingFileName       = "";
      //    settingFileDirectory  = "";
      //  }
      //}
    }

    /// ================================================================================
    /// <summary>文字列取得 - 階記号ソート順序</summary>
    ///
    /// <history><p>2013/07/10 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public void GetString_LevelSortOrder( ref string levelSortOrder )
    {
      levelSortOrder = entDtCmd.LevelSortOrdeer ;
    }

    /// ================================================================================
    /// <summary>階記号ソート順 + 名前降順</summary>
    ///
    /// <history><p>2013/04/24 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Collections.Generic.IList<string> LevelSortOrder_NameDESC()
    {
      Collections.Generic.IList<string> ret = new Collections.Generic.List<string>() ;

      string sortedOrder = "" ; // _CmpParameters.LevelSortOrder;

      // ProjectInfoパラメータ
      GetString_LevelSortOrder( ref sortedOrder ) ;

      // ソート済みを追加
      while ( sortedOrder != "" ) {
        if ( sortedOrder.Contains( "/SortOrder" ) ) {
          string subs = sortedOrder.Substring( 0, sortedOrder.IndexOf( "/SortOrder" ) ) ;
          sortedOrder = sortedOrder.Substring( sortedOrder.IndexOf( "/SortOrder" ) + 10 ) ;

          ret.Add( subs ) ;
        }
        else {
          ret.Add( sortedOrder ) ;
          break ;
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>階記号ソート順 + 名前降順</summary>
    ///
    /// <history><p>2013/04/24 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Collections.Generic.IList<string> LevelSortOrder_NameDESC( Collections.Generic.IList<string> levelNames )
    {
      Collections.Generic.IList<string> ret = new Collections.Generic.List<string>() ;

      Collections.Generic.IList<string> sortedOrder = LevelSortOrder_NameDESC() ;

      foreach ( string str in sortedOrder ) {
        if ( levelNames.Contains( str ) ) {
          ret.Add( str ) ;
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>階記号ソート順 - 接頭語順</summary>
    ///
    /// <param name="levelNames">階記号</param>
    ///
    /// <history><p>2014/06/19 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Collections.Generic.List<string> LevelSortOrder_TopName( Collections.Generic.IList<string> levelNames )
    {
      // 戻り値
      Collections.Generic.List<string> ret = new Collections.Generic.List<string>() ;

      if ( levelNames.Count == 0 ) {
        return ret ;
      }

      if ( levelNames.Count == 1 ) {
        ret.Add( levelNames[ 0 ] ) ;
        return ret ;
      }

      // 符号名並び替え
      HugoNameComparer comparer = new HugoNameComparer() ;

      // 屋上レベル
      Collections.Generic.List<string> okujo = new Collections.Generic.List<string>() ;

      // 一般レベル
      Collections.Generic.List<string> ippan = new Collections.Generic.List<string>() ;

      // 中間レベル
      Collections.Generic.List<string> naka = new Collections.Generic.List<string>() ;

      // 地下レベル
      Collections.Generic.List<string> tika = new Collections.Generic.List<string>() ;

      // 地下中間レベル
      Collections.Generic.List<string> tikanaka = new Collections.Generic.List<string>() ;

      // 他
      Collections.Generic.List<string> hoka = new Collections.Generic.List<string>() ;

      foreach ( string str in levelNames ) {
        // 接頭文字で振り分け

        // 屋上
        if ( str.StartsWith( "R" ) || str.StartsWith( "P" ) || str.StartsWith( "PH" ) ) {
          okujo.Add( str ) ;
        }
        // 地下中間
        else if ( str.StartsWith( "MB" ) ) {
          tikanaka.Add( str ) ;
        }
        // 地下
        else if ( str.StartsWith( "B" ) ) {
          tika.Add( str ) ;
        }
        // 中間
        else if ( str.StartsWith( "M" ) ) {
          naka.Add( str ) ;
        }
        // 一般
        else if ( str.StartsWith( "1" ) || str.StartsWith( "2" ) || str.StartsWith( "3" ) || str.StartsWith( "4" ) || str.StartsWith( "5" ) || str.StartsWith( "6" ) || str.StartsWith( "7" ) || str.StartsWith( "8" ) || str.StartsWith( "9" ) ) {
          ippan.Add( str ) ;
        }
        else {
          hoka.Add( str ) ;
        }
      }

      // それそれでソートして追加

      // 屋上
      okujo.Sort() ;
      okujo.Reverse() ;

      Collections.Generic.List<string> okujo_R = new Collections.Generic.List<string>() ;
      Collections.Generic.List<string> okujo_P = new Collections.Generic.List<string>() ;
      Collections.Generic.List<string> okujo_PH = new Collections.Generic.List<string>() ;

      foreach ( string str in okujo ) {
        if ( str.StartsWith( "R" ) ) {
          // 2文字目から
          if ( str.Length > 1 ) {
            string s = str.Substring( 1 ) ;
            okujo_R.Add( s ) ;
          }
          else {
            okujo_R.Add( "" ) ;
          }
        }
        else if ( str.StartsWith( "PH" ) ) {
          // 3文字目から
          if ( str.Length > 2 ) {
            string s = str.Substring( 2 ) ;
            okujo_PH.Add( s ) ;
          }
          else {
            okujo_PH.Add( "" ) ;
          }
        }
        else if ( str.StartsWith( "P" ) ) {
          // 2文字目から
          if ( str.Length > 1 ) {
            string s = str.Substring( 1 ) ;
            okujo_P.Add( s ) ;
          }
          else {
            okujo_P.Add( "" ) ;
          }
        }
      }

      okujo_R.Sort( comparer ) ;
      okujo_P.Sort( comparer ) ;
      okujo_PH.Sort( comparer ) ;

      okujo_R.Reverse() ;
      okujo_P.Reverse() ;
      okujo_PH.Reverse() ;

      foreach ( string s in okujo_PH ) {
        ret.Add( "PH" + s ) ;
      }

      foreach ( string s in okujo_P ) {
        ret.Add( "P" + s ) ;
      }

      foreach ( string s in okujo_R ) {
        ret.Add( "R" + s ) ;
      }

      // 通常、中
      Collections.Generic.List<string> togo = new Collections.Generic.List<string>() ;

      foreach ( string s in ippan ) {
        togo.Add( s ) ;
      }

      foreach ( string s in naka ) {
        if ( s.Length > 1 ) {
          string sub = s.Substring( 1 ) ;
          togo.Add( sub ) ;
        }
        else {
          togo.Add( "" ) ;
        }
      }

      togo.Sort( comparer ) ;
      togo.Reverse() ;

      int i = 0 ;

      Collections.Generic.List<string> strAry = new Collections.Generic.List<string>() ;

      while ( i < togo.Count ) {
        string str = togo[ i ] ;

        if ( naka.Contains( "M" + str ) ) {
          if ( i == 0 ) {
            if ( togo.Count > 1 ) {
              string ato = togo[ 1 ] ;

              if ( str != ato ) {
                str = "M" + str ;
              }
            }
          }
          else if ( i > 0 ) {
            string mae = togo[ i - 1 ] ;

            if ( mae == str ) {
              str = "M" + str ;
            }
            else {
              if ( i < togo.Count - 1 ) {
                // 1つ後も違った場合
                string ato = togo[ i + 1 ] ;

                if ( str != ato ) {
                  str = "M" + str ;
                }
              }
            }
          }
        }

        ret.Add( str ) ;

        i += 1 ;
      }

      // 地下、地下中
      togo = new Collections.Generic.List<string>() ;

      foreach ( string s in tika ) {
        togo.Add( s ) ;
      }

      foreach ( string s in tikanaka ) {
        string sub = s.Substring( 1 ) ;
        togo.Add( sub ) ;
      }

      togo.Sort( comparer ) ;
      //togo.Reverse();

      i = 0 ;

      while ( i < togo.Count ) {
        string str = togo[ i ] ;

        if ( tikanaka.Contains( "M" + str ) ) {
          if ( i == 0 ) {
            str = "M" + str ;
          }
          else if ( i > 0 ) {
            if ( i < togo.Count - 1 ) {
              string ato = togo[ i + 1 ] ;

              if ( str == ato ) {
                str = "M" + str ;
              }
              else {
                string mae = togo[ i - 1 ] ;

                if ( str != mae ) {
                  str = "M" + str ;
                }
              }
            }
          }
        }

        ret.Add( str ) ;

        i += 1 ;
      }

      // そのほか
      hoka.Sort( comparer ) ;
      //hoka.Reverse();

      foreach ( string str in hoka ) {
        ret.Add( str ) ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>値文字列の重複判定</summary>
    ///
    /// <history><p>2014/06/16 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public string IsOverlapStrings( Collections.Generic.IDictionary<string, string> strs )
    {
      string ret = "" ;
      Collections.Generic.IList<string> laps = new Collections.Generic.List<string>() ;

      Collections.Generic.List<string> list = new Collections.Generic.List<string>() ;

      foreach ( string s in strs.Values ) {
        list.Add( s ) ;
      }

      for ( int i = 0 ; i < list.Count ; ++i ) {
        for ( int j = 0 ; j < list.Count ; ++j ) {
          if ( i == j || i > j ) {
            continue ;
          }

          string str1 = list[ i ] ;
          string str2 = list[ j ] ;

          if ( str1 == str2 ) {
            if ( laps.Contains( str1 ) == false ) {
              laps.Add( str1 ) ;

              if ( ret != "" ) {
                ret += "\r\n" ;
              }

              ret += str1 ;
            }
          }
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>設定文字列内の重複判定 - 柱</summary>
    ///
    /// <history><p>2013/12/19 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public string IsOverlapStrings_Kaku( Collections.Generic.IDictionary<string, string> strs )
    {
      Collections.Generic.IList<string> ret = new Collections.Generic.List<string>() ;
      string retStr = "" ;

      Collections.Generic.List<string> list = new Collections.Generic.List<string>() ;
      foreach ( string s in strs.Values ) {
        list.Add( s ) ;
      }

      // 次と比較
      for ( int i = 0 ; i < list.Count ; ++i ) {
        //// 対象外パラメータ
        //if (i <= 61 ||
        //    i == 89 ||
        //    i >= 105)
        //{
        //  continue;
        //}

        for ( int j = 0 ; j < list.Count ; ++j ) {
          //// 対象外パラメータ
          //if (i == j ||
          //    j <= 61 ||
          //    j == 89 ||
          //    j >= 105)
          //{
          //  continue;
          //}

          if ( i == j || i > j ) {
            continue ;
          }

          string str1 = list[ i ] ;
          string str2 = list[ j ] ;

          if ( str1 == str2 ) {
            if ( ret.Contains( str1 ) == false ) {
              ret.Add( str1 ) ;

              if ( retStr != "" ) {
                retStr += "\r\n" ;
              }

              retStr += str1 ;
            }
          }
        }
      }

      return retStr ;
    }

    /// ================================================================================
    /// <summary>設定文字列内の重複判定 - 円柱</summary>
    ///
    /// <history><p>2013/12/19 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public string IsOverlapStrings_En( Collections.Generic.IDictionary<string, string> strs )
    {
      Collections.Generic.IList<string> ret = new Collections.Generic.List<string>() ;
      string retStr = "" ;

      Collections.Generic.List<string> list = new Collections.Generic.List<string>() ;
      foreach ( string s in strs.Values ) {
        list.Add( s ) ;
      }

      // 次と比較
      for ( int i = 0 ; i < list.Count ; ++i ) {
        //// 対象外パラメータ
        //if (i <= 105 ||
        //    i >= 126)
        //{
        //  continue;
        //}

        for ( int j = 0 ; j < list.Count ; ++j ) {
          //// 対象外パラメータ
          //if (i == j ||
          //    j <= 105 ||
          //    j >= 126)
          //{
          //  continue;
          //}

          if ( i == j || i > j ) {
            continue ;
          }

          string str1 = list[ i ] ;
          string str2 = list[ j ] ;

          if ( str1 == str2 ) {
            if ( ret.Contains( str1 ) == false ) {
              ret.Add( str1 ) ;

              if ( retStr != "" ) {
                retStr += "\r\n" ;
              }

              retStr += str1 ;
            }
          }
        }
      }

      return retStr ;
    }

    /// ================================================================================
    /// <summary>設定文字列内の重複判定 - 梁</summary>
    ///
    /// <history><p>2013/12/19 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public string IsOverlapStrings_Hari( string[] strs )
    {
      Collections.Generic.IList<string> ret = new Collections.Generic.List<string>() ;
      string retStr = "" ;

      Collections.Generic.List<string> list = new Collections.Generic.List<string>() ;
      foreach ( string s in strs ) {
        list.Add( s ) ;
      }

      // 次と比較
      for ( int i = 0 ; i < list.Count ; ++i ) {
        //// 対象外パラメータ
        //if (i <= 126 ||
        //    i == 149 ||
        //    i == 180 ||
        //    i >= 209)
        //{
        //  continue;
        //}

        for ( int j = 0 ; j < list.Count ; ++j ) {
          //// 対象外パラメータ
          //if (i == j ||
          //    j <= 126 ||
          //    j == 149 ||
          //    j == 180 ||
          //    j >= 209)
          //{
          //  continue;
          //}

          if ( i == j || i > j ) {
            continue ;
          }

          string str1 = list[ i ] ;
          string str2 = list[ j ] ;

          if ( str1 == str2 ) {
            if ( ret.Contains( str1 ) == false ) {
              ret.Add( str1 ) ;

              if ( retStr != "" ) {
                retStr += "\r\n" ;
              }

              retStr += str1 ;
            }
          }
        }
      }

      return retStr ;
    }

    // ----- ----- ----- ----- ----- ----- ----- -- 柱 -- ----- ----- ----- ----- ----- ----- -----

    /// ================================================================================
    /// <summary>階別最大柱高さ</summary>
    ///
    /// <history><p>2013/04/17 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Collections.Generic.IList<double> ColumnHeightByLevel( System.Data.DataTable data )
    {
      Collections.Generic.IList<double> ret = new Collections.Generic.List<double>() ;

      // 既階
      Collections.Generic.IList<string> levels = new Collections.Generic.List<string>() ;

      for ( int i = 0 ; i < data.Rows.Count ; ++i ) {
        string level = (string)data.Rows[ i ][ _CmpParameters.LevelFrameTitle ] ;

        if ( levels.Contains( level ) ) {
          continue ;
        }

        levels.Add( level ) ;

        double maxY = 0 ;

        double y = (double)data.Rows[ i ][ _CmpParameters.DY_Kaku ] ;
        maxY = y ;

        for ( int j = 0 ; j < data.Rows.Count ; ++j ) {
          string level2 = (string)data.Rows[ j ][ _CmpParameters.LevelFrameTitle ] ;

          if ( level == level2 ) {
            double _y = (double)data.Rows[ j ][ _CmpParameters.DY_Kaku ] ;

            if ( y < _y ) {
              maxY = _y ;
            }
          }
        }

        ret.Add( maxY ) ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>特定階最大柱高さ</summary>
    ///
    /// <history><p>2013/04/24 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public double ColumnHeightByLevel( System.Data.DataTable kakuData, System.Data.DataTable enData, string level )
    {
      double ret = 0 ;

      if ( kakuData != null ) {
        for ( int i = 0 ; i < kakuData.Rows.Count ; ++i ) {
          string l = (string)kakuData.Rows[ i ][ _CmpParameters.LevelFrameTitle ] ;

          if ( l != level ) {
            continue ;
          }

          double h = (double)kakuData.Rows[ i ][ _CmpParameters.DY_Kaku ] ;

          if ( ret < h ) {
            ret = h ;
          }
        }
      }

      if ( enData != null ) {
        for ( int i = 0 ; i < enData.Rows.Count ; ++i ) {
          string l = (string)enData.Rows[ i ][ _CmpParameters.LevelFrameTitle ] ;

          if ( l != level ) {
            continue ;
          }

          double dia = (double)enData.Rows[ i ][ _CmpParameters.Tyokkei_En ] ;

          if ( ret < dia ) {
            ret = dia ;
          }
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>特定符号最大柱幅</summary>
    ///
    /// <history><p>2013/04/24 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public double ColumnWidthByMark( System.Data.DataTable data, string hugo, bool isRect )
    {
      double ret = 0 ;

      for ( int i = 0 ; i < data.Rows.Count ; ++i ) {
        string hugoName = (string)data.Rows[ i ][ _CmpParameters.RST_HasiraHugo_Kaku ] ;

        if ( hugoName != hugo ) {
          continue ;
        }

        if ( isRect == true ) {
          double x = (double)data.Rows[ i ][ _CmpParameters.DX_Kaku ] ;
          if ( ret < x ) {
            ret = x ;
          }
        }
        else {
          double dia = (double)data.Rows[ i ][ _CmpParameters.Tyokkei_En ] ;
          if ( ret < dia ) {
            ret = dia ;
          }
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>符号別最大柱幅</summary>
    ///
    /// <history><p>2013/04/17 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Collections.Generic.IList<double> ColumnWidthByMark( System.Data.DataTable data )
    {
      Collections.Generic.IList<double> ret = new Collections.Generic.List<double>() ;

      // 既符号名
      Collections.Generic.IList<string> names = new Collections.Generic.List<string>() ;

      for ( int i = 0 ; i < data.Rows.Count ; ++i ) {
        string typeName = (string)data.Rows[ i ][ _CmpParameters.RST_HasiraHugo_Kaku ] ;

        if ( names.Contains( typeName ) ) {
          continue ;
        }

        names.Add( typeName ) ;

        double maxX = 0 ;

        double x = (double)data.Rows[ i ][ _CmpParameters.DX_Kaku ] ;
        maxX = x ;

        for ( int j = 0 ; j < data.Rows.Count ; ++j ) {
          string typeName2 = (string)data.Rows[ j ][ _CmpParameters.RST_HasiraHugo_Kaku ] ;

          if ( typeName == typeName2 ) {
            double _x = (double)data.Rows[ j ][ _CmpParameters.DX_Kaku ] ;

            if ( x < _x ) {
              maxX = _x ;
            }
          }
        }

        ret.Add( maxX ) ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>符号別最大円柱径</summary>
    ///
    /// <history><p>2013/04/18 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Collections.Generic.IList<double> ColumnDiameterByMark( System.Data.DataTable data )
    {
      Collections.Generic.IList<double> ret = new Collections.Generic.List<double>() ;

      // 既符号名
      Collections.Generic.IList<string> names = new Collections.Generic.List<string>() ;

      for ( int i = 0 ; i < data.Rows.Count ; ++i ) {
        string typeName = (string)data.Rows[ i ][ _CmpParameters.RST_HasiraHugo_En ] ;

        if ( names.Contains( typeName ) ) {
          continue ;
        }

        names.Add( typeName ) ;

        double maxDia = 0 ;

        double dia = (double)data.Rows[ i ][ _CmpParameters.Tyokkei_En ] ;
        maxDia = dia ;

        for ( int j = 0 ; j < data.Rows.Count ; ++j ) {
          string typeName2 = (string)data.Rows[ j ][ _CmpParameters.RST_HasiraHugo_En ] ;

          if ( typeName == typeName2 ) {
            double _dia = (double)data.Rows[ j ][ _CmpParameters.Tyokkei_En ] ;

            if ( dia < _dia ) {
              maxDia = _dia ;
            }
          }
        }

        ret.Add( maxDia ) ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>階別最大円柱径</summary>
    ///
    /// <history><p>2013/04/18 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Collections.Generic.IList<double> ColumnDiameterByLevel( System.Data.DataTable data )
    {
      Collections.Generic.IList<double> ret = new Collections.Generic.List<double>() ;

      // 既階
      Collections.Generic.IList<string> levels = new Collections.Generic.List<string>() ;

      for ( int i = 0 ; i < data.Rows.Count ; ++i ) {
        string level = (string)data.Rows[ i ][ _CmpParameters.LevelFrameTitle ] ;

        if ( levels.Contains( level ) ) {
          continue ;
        }

        levels.Add( level ) ;

        double maxDia = 0 ;

        double dia = (double)data.Rows[ i ][ _CmpParameters.Tyokkei_En ] ;
        maxDia = dia ;

        for ( int j = 0 ; j < data.Rows.Count ; ++j ) {
          string level2 = (string)data.Rows[ j ][ _CmpParameters.LevelFrameTitle ] ;

          if ( level == level2 ) {
            double _dia = (double)data.Rows[ j ][ _CmpParameters.Tyokkei_En ] ;

            if ( dia < _dia ) {
              maxDia = _dia ;
            }
          }
        }

        ret.Add( maxDia ) ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>同一階で芯鉄筋があるか</summary>
    ///
    /// <param name="level">対象階</param>
    /// <returns>true = ある</returns>
    ///
    /// <history><p>2013/04/22 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public bool IsCoreRebarInLevel( System.Data.DataTable data, string level )
    {
      bool ret = false ;

      for ( int i = 0 ; i < data.Rows.Count ; ++i ) {
        if ( (string)data.Rows[ i ][ _CmpParameters.LevelFrameTitle ] == level ) {
          if ( data.Columns.Count >= 42 ) {
            if ( (int)data.Rows[ i ][ _CmpParameters.CoreRebar_Number_Kaku ] > 0 ) {
              ret = true ;
              break ;
            }
          }
          else {
            if ( (int)data.Rows[ i ][ _CmpParameters.RST_SintekkinHonsu_En ] > 0 ) {
              ret = true ;
              break ;
            }
          }
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>同一階で柱頭柱脚で配筋が違う柱があるか</summary>
    ///
    /// <param name="level">対象階</param>
    /// <returns>true = ある</returns>
    ///
    /// <history><p>2013/04/19 Created  GSA,Inc. Ryo Kuroda</p>
    ///           <p>2014/08/22 Modified GSA, Inc, Ryo Kuroda</p></history>
    /// ================================================================================
    public bool IsDifferenceTopBottomRebarInLevel( System.Data.DataTable data, string level )
    {
      bool ret = false ;

      for ( int i = 0 ; i < data.Rows.Count ; ++i ) {
        // 特定の階
        if ( (string)data.Rows[ i ][ _CmpParameters.LevelFrameTitle ] == level ) {
          // 2014/08/21
          // パラメータ名だと同じ名前をつけられたらアウト
          // テーブルの列数で判別
          //if (!data.Columns.Contains(_CmpParameters.Tyokkei_En))
          if ( data.Columns.Count >= 42 ) {
            if ( (string)data.Rows[ i ][ _CmpParameters.RST_ChutoSyukinHutokei_Kaku ] != (string)data.Rows[ i ][ _CmpParameters.RST_ChukyakuSyukinHutokei_Kaku ] ) {
              ret = true ;
              break ;
            }

            if ( (int)data.Rows[ i ][ _CmpParameters.RST_ChutoSyukinX1danHutokeiHonsu_Kaku ] != (int)data.Rows[ i ][ _CmpParameters.RST_ChukyakuSyukinX1danHutokeiHonsu_Kaku ] ) {
              ret = true ;
              break ;
            }

            if ( (int)data.Rows[ i ][ _CmpParameters.RST_ChutoSyukinX2danHutokeiHonsu_Kaku ] != (int)data.Rows[ i ][ _CmpParameters.RST_ChukyakuSyukinX2danHutokeiHonsu_Kaku ] ) {
              ret = true ;
              break ;
            }

            if ( (int)data.Rows[ i ][ _CmpParameters.RST_ChutoSyukinY1danHutokeiHonsu_Kaku ] != (int)data.Rows[ i ][ _CmpParameters.RST_ChukyakuSyukinY1danHutokeiHonsu_Kaku ] ) {
              ret = true ;
              break ;
            }

            if ( (int)data.Rows[ i ][ _CmpParameters.RST_ChutoSyukinY2danHutokeiHonsu_Kaku ] != (int)data.Rows[ i ][ _CmpParameters.RST_ChukyakuSyukinY2danHutokeiHonsu_Kaku ] ) {
              ret = true ;
              break ;
            }

            string chutoHosokei = (string)data.Rows[ i ][ _CmpParameters.RST_ChutoSyukinHosokei_Kaku ] == "なし" ? "" : (string)data.Rows[ i ][ _CmpParameters.RST_ChutoSyukinHosokei_Kaku ] ;
            string chukyakuHosokei = (string)data.Rows[ i ][ _CmpParameters.RST_ChukyakuSyukinHosokei_Kaku ] == "なし" ? "" : (string)data.Rows[ i ][ _CmpParameters.RST_ChukyakuSyukinHosokei_Kaku ] ;

            if ( chutoHosokei != chukyakuHosokei ) {
              ret = true ;
              break ;
            }

            if ( (int)data.Rows[ i ][ _CmpParameters.RST_ChutoSyukinX1danHosokeiHonsu_Kaku ] != (int)data.Rows[ i ][ _CmpParameters.RST_ChukyakuSyukinX1danHosokeiHonsu_Kaku ] ) {
              ret = true ;
              break ;
            }

            if ( (int)data.Rows[ i ][ _CmpParameters.RST_ChutoSyukinX2danHosokeiHonsu_Kaku ] != (int)data.Rows[ i ][ _CmpParameters.RST_ChukyakuSyukinX2danHosokeiHonsu_Kaku ] ) {
              ret = true ;
              break ;
            }

            if ( (int)data.Rows[ i ][ _CmpParameters.RST_ChutoSyukinY1danHosokeiHonsu_Kaku ] != (int)data.Rows[ i ][ _CmpParameters.RST_ChukyakuSyukinY1danHosokeiHonsu_Kaku ] ) {
              ret = true ;
              break ;
            }

            if ( (int)data.Rows[ i ][ _CmpParameters.RST_ChutoSyukinY2danHosokeiHonsu_Kaku ] != (int)data.Rows[ i ][ _CmpParameters.RST_ChukyakuSyukinY2danHosokeiHonsu_Kaku ] ) {
              ret = true ;
              break ;
            }

            if ( (string)data.Rows[ i ][ _CmpParameters.RST_ChutoHoopXKei_Kaku ] != (string)data.Rows[ i ][ _CmpParameters.RST_ChukyakuHoopXKei_Kaku ] ) {
              ret = true ;
              break ;
            }

            if ( (int)data.Rows[ i ][ _CmpParameters.RST_ChutoHoopXHonsu_Kaku ] != (int)data.Rows[ i ][ _CmpParameters.RST_ChukyakuHoopXHonsu_Kaku ] ) {
              ret = true ;
              break ;
            }

            if ( (int)data.Rows[ i ][ _CmpParameters.RST_ChutoHoopYHonsu_Kaku ] != (int)data.Rows[ i ][ _CmpParameters.RST_ChukyakuHoopYHonsu_Kaku ] ) {
              ret = true ;
              break ;
            }

            if ( (double)data.Rows[ i ][ _CmpParameters.RST_ChutoHoopPitch_Kaku ] != (double)data.Rows[ i ][ _CmpParameters.RST_ChukyakuHoopPitch_Kaku ] ) {
              ret = true ;
              break ;
            }

            if ( (int)data.Rows[ i ][ _CmpParameters.Top_Spacing_XDirectionNumber_Kaku ] != (int)data.Rows[ i ][ _CmpParameters.Bottom_Spacing_XDirectionNumber_Kaku ] ) {
              ret = true ;
              break ;
            }

            if ( (int)data.Rows[ i ][ _CmpParameters.Top_Spacing_YDirectionNumber_Kaku ] != (int)data.Rows[ i ][ _CmpParameters.Bottom_Spacing_YDirectionNumber_Kaku ] ) {
              ret = true ;
              break ;
            }
          }
          else if ( data.Columns.Count >= 20 ) {
            if ( (string)data.Rows[ i ][ _CmpParameters.RST_ChutoSyukinKei_En ] != (string)data.Rows[ i ][ _CmpParameters.RST_ChukyakuSyukinKei_En ] ) {
              ret = true ;
              break ;
            }

            if ( (int)data.Rows[ i ][ _CmpParameters.RST_ChutoSyukinHonsu_En ] != (int)data.Rows[ i ][ _CmpParameters.RST_ChukyakuSyukinHonsu_En ] ) {
              ret = true ;
              break ;
            }

            if ( (string)data.Rows[ i ][ _CmpParameters.RST_ChutoHoopXKei_En ] != (string)data.Rows[ i ][ _CmpParameters.RST_ChukyakuHoopXKei_En ] ) {
              ret = true ;
              break ;
            }

            if ( (double)data.Rows[ i ][ _CmpParameters.RST_ChutoHoopPitch_En ] != (double)data.Rows[ i ][ _CmpParameters.RST_ChukyakuHoopPitch_En ] ) {
              ret = true ;
              break ;
            }

            if ( (int)data.Rows[ i ][ _CmpParameters.Top_Spacing_XDirectionNumber_En ] != (int)data.Rows[ i ][ _CmpParameters.Bottom_Spacing_XDirectionNumber_En ] ) {
              ret = true ;
              break ;
            }

            if ( (int)data.Rows[ i ][ _CmpParameters.Top_Spacing_YDirectionNumber_En ] != (int)data.Rows[ i ][ _CmpParameters.Bottom_Spacing_YDirectionNumber_En ] ) {
              ret = true ;
              break ;
            }
          }
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>全柱符号名(名前順)</summary>
    ///
    /// <history><p>2013/04/23 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Collections.Generic.List<string> GetAllColumnHugoAry( Collections.Generic.IList<Revit.DB.FamilyInstance> columnAry )
    {
      Collections.Generic.List<string> ret = new Collections.Generic.List<string>() ;

      foreach ( Revit.DB.FamilyInstance famIns in columnAry ) {
        Revit.DB.Parameter param = famIns.Symbol.LookupParameter( _CmpParameters.RST_HasiraHugo_En ) ;
        string name = "" ;
        if ( param == null ) {
          param = famIns.Symbol.LookupParameter( _CmpParameters.RST_HasiraHugo_Kaku ) ;
          name = param.AsString() ;
        }
        else {
          name = param.AsString() ;
        }

        if ( ret.Contains( name ) == false ) {
          ret.Add( name ) ;
        }
      }

      // 名前順ソート
      ret.Sort() ;

      return ret ;
    }

    /// ================================================================================
    /// <summary>柱階名(名前降順)</summary>
    ///
    /// <history><p>2013/04/24 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Collections.Generic.IList<string> GetAllColumnLevelAry( Collections.Generic.IList<Revit.DB.FamilySymbol> columnAry )
    {
      Collections.Generic.List<string> ret = new Collections.Generic.List<string>() ;

      foreach ( Revit.DB.FamilySymbol column in columnAry ) {
        string level = "" ;

        Revit.DB.Parameter parX = column.LookupParameter( _CmpParameters.DX_Kaku ) ;
        Revit.DB.Parameter parY = column.LookupParameter( _CmpParameters.DY_Kaku ) ;
        Revit.DB.Parameter parDiameter = column.LookupParameter( _CmpParameters.Tyokkei_En ) ;

        if ( parX != null && parY != null ) {
          //if (parX.AsDouble() > 0 && parY.AsDouble() > 0)
          {
            level = _CmpElements.GetColumnTypeLevel( column, _CmpParameters.RST_HasiraHugo_Kaku ) ;

            if ( ! ret.Contains( level ) ) {
              ret.Add( level ) ;
            }

            continue ;
          }
        }

        if ( parDiameter != null ) {
          level = _CmpElements.GetColumnTypeLevel( column, _CmpParameters.RST_HasiraHugo_En ) ;

          if ( ! ret.Contains( level ) ) {
            ret.Add( level ) ;
          }
        }
      }

      if ( ret.Count < 2 ) {
        return ret ;
      }

      // 全柱のレベル
      // ソート
      ret.Sort() ;
      if ( string.Compare( ret[ 0 ], ret[ ret.Count - 1 ], false ) < 0 ) {
        ret.Reverse() ;
      }

      //Collections.Generic.IList<string> sorted = LevelSortOrder_NameDESC(levelNames);

      //// ソート済みに含まれない全柱を追加
      //foreach (string str in levelNames)
      //{
      //  if (!sorted.Contains(str))
      //  {
      //    sorted.Add(str);
      //  }
      //}

      return ret ;
    }

    /// ================================================================================
    /// <summary>柱タイプ符号取得</summary>
    ///
    /// <history>2013/04/23 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string GetColumnTypeName( Revit.DB.FamilyInstance column )
    {
      string ret = "" ;

      Revit.DB.FamilySymbol symbol = column.Symbol ;
      if ( symbol != null ) {
        Revit.DB.Parameter paramColumnHugo = symbol.LookupParameter( _CmpParameters.RST_HasiraHugo_En ) ;

        if ( paramColumnHugo == null ) {
          paramColumnHugo = symbol.LookupParameter( _CmpParameters.RST_HasiraHugo_Kaku ) ;
        }

        ret = paramColumnHugo.AsString() ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>角柱符号名</summary>
    ///
    /// <history><p>2013/04/18 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Collections.Generic.IList<string> KakuCollumnMarkName( System.Data.DataTable data )
    {
      Collections.Generic.List<string> ret = new Collections.Generic.List<string>() ;

      for ( int i = 0 ; i < data.Rows.Count ; ++i ) {
        string typeName = (string)data.Rows[ i ][ _CmpParameters.RST_HasiraHugo_Kaku ] ;

        if ( ! ret.Contains( typeName ) ) {
          ret.Add( typeName ) ;
        }
      }

      ret.Sort( new HugoNameComparer() ) ;

      return ret ;
    }

    /// ================================================================================
    /// <summary>円柱符号名</summary>
    ///
    /// <history><p>2013/04/18 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Collections.Generic.IList<string> EnCollumnMarkName( System.Data.DataTable data )
    {
      Collections.Generic.List<string> ret = new Collections.Generic.List<string>() ;

      for ( int i = 0 ; i < data.Rows.Count ; ++i ) {
        string typeName = (string)data.Rows[ i ][ _CmpParameters.RST_HasiraHugo_En ] ;

        if ( ! ret.Contains( typeName ) ) {
          ret.Add( typeName ) ;
        }
      }

      ret.Sort( new HugoNameComparer() ) ;

      return ret ;
    }

    /// ================================================================================
    /// <summary>既定のパラメータ項目を持つ柱か</summary>
    ///
    /// <history><p>2013/05/20 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public bool IsHaveColumnParam( Revit.DB.FamilySymbol famSym )
    {
      bool ret = true ;

      if ( famSym == null ) {
        ret = false ;
        return ret ;
      }

      // 角柱
      if ( IsHaveKakuColumnParam( famSym ) ) {
        ret = true ;
        return ret ;
      }

      // 円柱
      if ( IsHaveEnColumnParam( famSym ) ) {
        ret = true ;
        return ret ;
      }

      ret = false ;

      #region 2014/08/28 不使用

      //// 角柱
      //if (famSym.LookupParameter(_CmpParameters.HashiraBunrui_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.DX_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.DY_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChutoSyukinHutokei_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChukyakuSyukinHutokei_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChutoSyukinX1danHutokeiHonsu_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChukyakuSyukinX1danHutokeiHonsu_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChutoSyukinX2danHutokeiHonsu_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChukyakuSyukinX2danHutokeiHonsu_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChutoSyukinY1danHutokeiHonsu_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChukyakuSyukinY1danHutokeiHonsu_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChutoSyukinY2danHutokeiHonsu_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChukyakuSyukinY2danHutokeiHonsu_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChutoSyukinHosokei_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChukyakuSyukinHosokei_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChutoSyukinX1danHosokeiHonsu_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChukyakuSyukinX1danHosokeiHonsu_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChutoSyukinX2danHosokeiHonsu_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChukyakuSyukinX2danHosokeiHonsu_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChutoSyukinY1danHosokeiHonsu_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChukyakuSyukinY1danHosokeiHonsu_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChutoSyukinY2danHosokeiHonsu_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChukyakuSyukinY2danHosokeiHonsu_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_SintekkinKei_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.CoreRebar_Number_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_SintekkinIchiX_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_SintekkinIchiY_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChutoHoopXKei_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChukyakuHoopXKei_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChutoHoopXHonsu_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChukyakuHoopXHonsu_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChutoHoopYHonsu_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChukyakuHoopYHonsu_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChutoHoopPitch_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChukyakuHoopPitch_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_HabadomekinKei_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.Top_Spacing_XDirectionNumber_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.Bottom_Spacing_XDirectionNumber_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.Top_Spacing_YDirectionNumber_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.Bottom_Spacing_YDirectionNumber_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_HabadomekinPitch_Kaku) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_HasiraHugo_Kaku) == null)
      //{
      //  ret = false;
      //}

      //// すべての角柱パラメータがある
      //if (ret == true)
      //{
      //  return ret;
      //}

      //// リセット
      //ret = true;

      //// 円柱
      //if (famSym.LookupParameter(_CmpParameters.Column_Category_En) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.Tyokkei_En) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChutoSyukinKei_En) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChukyakuSyukinKei_En) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChutoSyukinHonsu_En) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChukyakuSyukinHonsu_En) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_SintekkinKei_En) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_SintekkinHonsu_En) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_SintekkinIchi_En) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChutoHoopXKei_En) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChukyakuHoopXKei_En) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChutoHoopPitch_En) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_ChukyakuHoopPitch_En) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_HabadomekinKei_En) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.Top_Spacing_XDirectionNumber_En) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.Bottom_Spacing_XDirectionNumber_En) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.Top_Spacing_YDirectionNumber_En) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.Bottom_Spacing_YDirectionNumber_En) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_HabadomekinPitch_En) == null)
      //{
      //  ret = false;
      //}
      //else if (famSym.LookupParameter(_CmpParameters.RST_HasiraHugo_En) == null)
      //{
      //  ret = false;
      //}

      //// すべての円柱パラメータがある
      //if (ret == true)
      //{
      //  return ret;
      //}

      #endregion 2014/08/28 不使用

      return ret ;
    }

    /// ================================================================================
    /// <summary>既定のパラメータ項目を持つ矩形柱か</summary>
    ///
    /// <history><p>2014/01/15 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public bool IsHaveKakuColumnParam( Revit.DB.FamilySymbol famSym )
    {
      bool ret = true ;

      if ( famSym == null ) {
        ret = false ;
        return ret ;
      }

      if ( famSym.LookupParameter( _CmpParameters.HashiraBunrui_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.DX_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.DY_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChutoSyukinHutokei_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChukyakuSyukinHutokei_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChutoSyukinX1danHutokeiHonsu_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChukyakuSyukinX1danHutokeiHonsu_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChutoSyukinX2danHutokeiHonsu_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChukyakuSyukinX2danHutokeiHonsu_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChutoSyukinY1danHutokeiHonsu_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChukyakuSyukinY1danHutokeiHonsu_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChutoSyukinY2danHutokeiHonsu_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChukyakuSyukinY2danHutokeiHonsu_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChutoSyukinHosokei_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChukyakuSyukinHosokei_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChutoSyukinX1danHosokeiHonsu_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChukyakuSyukinX1danHosokeiHonsu_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChutoSyukinX2danHosokeiHonsu_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChukyakuSyukinX2danHosokeiHonsu_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChutoSyukinY1danHosokeiHonsu_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChukyakuSyukinY1danHosokeiHonsu_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChutoSyukinY2danHosokeiHonsu_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChukyakuSyukinY2danHosokeiHonsu_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_SintekkinKei_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.CoreRebar_Number_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_SintekkinIchiX_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_SintekkinIchiY_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChutoHoopXKei_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChukyakuHoopXKei_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChutoHoopXHonsu_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChukyakuHoopXHonsu_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChutoHoopYHonsu_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChukyakuHoopYHonsu_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChutoHoopPitch_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChukyakuHoopPitch_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_HabadomekinKei_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.Top_Spacing_XDirectionNumber_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.Bottom_Spacing_XDirectionNumber_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.Top_Spacing_YDirectionNumber_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.Bottom_Spacing_YDirectionNumber_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_HabadomekinPitch_Kaku ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_HasiraHugo_Kaku ) == null ) {
        ret = false ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>既定のパラメータ項目を持つ円柱か</summary>
    ///
    /// <history><p>2013/05/20 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public bool IsHaveEnColumnParam( Revit.DB.FamilySymbol famSym )
    {
      bool ret = true ;

      if ( famSym == null ) {
        ret = false ;
        return ret ;
      }

      if ( famSym.LookupParameter( _CmpParameters.Column_Category_En ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.Tyokkei_En ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChutoSyukinKei_En ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChukyakuSyukinKei_En ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChutoSyukinHonsu_En ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChukyakuSyukinHonsu_En ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_SintekkinKei_En ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_SintekkinHonsu_En ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_SintekkinIchi_En ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChutoHoopXKei_En ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChukyakuHoopXKei_En ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChutoHoopPitch_En ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_ChukyakuHoopPitch_En ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_HabadomekinKei_En ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.Top_Spacing_XDirectionNumber_En ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.Bottom_Spacing_XDirectionNumber_En ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.Top_Spacing_YDirectionNumber_En ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.Bottom_Spacing_YDirectionNumber_En ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_HabadomekinPitch_En ) == null ) {
        ret = false ;
      }
      else if ( famSym.LookupParameter( _CmpParameters.RST_HasiraHugo_En ) == null ) {
        ret = false ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>角柱と円柱の分割</summary>
    /// <history>2014/01/15 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public void ColumnDivision( Collections.Generic.IList<Revit.DB.FamilySymbol> allColumns, ref Collections.Generic.IList<Revit.DB.FamilySymbol> kakuColumns, ref Collections.Generic.IList<Revit.DB.FamilySymbol> enColumns )
    {
      foreach ( Revit.DB.FamilySymbol famSym in allColumns ) {
        if ( IsHaveKakuColumnParam( famSym ) ) {
          kakuColumns.Add( famSym ) ;
        }

        if ( IsHaveEnColumnParam( famSym ) ) {
          enColumns.Add( famSym ) ;
        }
      }
    }

    // ----- ----- ----- ----- ----- ----- ----- -- 柱 -- ----- ----- ----- ----- ----- ----- -----
    // ----- ----- ----- ----- ----- ----- ----- -- 梁 -- ----- ----- ----- ----- ----- ----- -----

    /// ================================================================================
    /// <summary>階別最大梁高さ</summary>
    ///
    /// <history><p>2013/04/26 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Collections.Generic.IList<double> BeamHeightByLevel( System.Data.DataTable data )
    {
      Collections.Generic.IList<double> ret = new Collections.Generic.List<double>() ;

      // 既階
      Collections.Generic.IList<string> levels = new Collections.Generic.List<string>() ;

      for ( int i = 0 ; i < data.Rows.Count ; ++i ) {
        string level = (string)data.Rows[ i ][ _CmpParameters.LevelFrameTitle ] ;

        if ( levels.Contains( level ) ) {
          continue ;
        }

        levels.Add( level ) ;

        double maxY = 0 ;

        double y = (double)data.Rows[ i ][ _CmpParameters.s_D ] ;

        if ( y < (double)data.Rows[ i ][ _CmpParameters.c_D ] ) {
          y = (double)data.Rows[ i ][ _CmpParameters.c_D ] ;
        }

        if ( y < (double)data.Rows[ i ][ _CmpParameters.e_D ] ) {
          y = (double)data.Rows[ i ][ _CmpParameters.e_D ] ;
        }

        maxY = y ;

        for ( int j = 0 ; j < data.Rows.Count ; ++j ) {
          string level2 = (string)data.Rows[ j ][ _CmpParameters.LevelFrameTitle ] ;

          if ( level == level2 ) {
            double _y = (double)data.Rows[ j ][ _CmpParameters.s_D ] ;

            if ( _y < (double)data.Rows[ j ][ _CmpParameters.c_D ] ) {
              _y = (double)data.Rows[ j ][ _CmpParameters.c_D ] ;
            }

            if ( _y < (double)data.Rows[ j ][ _CmpParameters.e_D ] ) {
              _y = (double)data.Rows[ j ][ _CmpParameters.e_D ] ;
            }

            if ( y < _y ) {
              maxY = _y ;
            }
          }
        }

        ret.Add( maxY ) ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>階別最大梁高さ - 片持ち梁</summary>
    ///
    /// <history><p>2013/04/26 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Collections.Generic.IList<double> BeamHeightByLevel_Canti( System.Data.DataTable data )
    {
      Collections.Generic.IList<double> ret = new Collections.Generic.List<double>() ;

      // 既階
      Collections.Generic.IList<string> levels = new Collections.Generic.List<string>() ;

      for ( int i = 0 ; i < data.Rows.Count ; ++i ) {
        string level = (string)data.Rows[ i ][ _CmpParameters.LevelFrameTitle ] ;

        if ( levels.Contains( level ) ) {
          continue ;
        }

        levels.Add( level ) ;

        double maxY = 0 ;

        double y = (double)data.Rows[ i ][ _CmpParameters.MototanHarisei ] ;

        if ( y < (double)data.Rows[ i ][ _CmpParameters.SentanHarisei ] ) {
          y = (double)data.Rows[ i ][ _CmpParameters.SentanHarisei ] ;
        }

        maxY = y ;

        for ( int j = 0 ; j < data.Rows.Count ; ++j ) {
          string level2 = (string)data.Rows[ j ][ _CmpParameters.LevelFrameTitle ] ;

          if ( level == level2 ) {
            double _y = (double)data.Rows[ j ][ _CmpParameters.MototanHarisei ] ;

            if ( _y < (double)data.Rows[ j ][ _CmpParameters.SentanHarisei ] ) {
              _y = (double)data.Rows[ j ][ _CmpParameters.SentanHarisei ] ;
            }

            if ( y < _y ) {
              maxY = _y ;
            }
          }
        }

        ret.Add( maxY ) ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>符号別最大梁幅(複数断面の合計を含む)</summary>
    ///
    /// <history><p>2013/04/26 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Collections.Generic.IList<double> BeamWidthByMark( System.Data.DataTable data )
    {
      Collections.Generic.IList<double> ret = new Collections.Generic.List<double>() ;

      // 既符号名
      Collections.Generic.IList<string> names = new Collections.Generic.List<string>() ;

      for ( int i = 0 ; i < data.Rows.Count ; ++i ) {
        string typeName = (string)data.Rows[ i ][ _CmpParameters.RST_HariHugo ] ;

        if ( names.Contains( typeName ) ) {
          continue ;
        }

        names.Add( typeName ) ;

        double maxX = 0 ;

        double x = BeamFrameWidthBySectionType( data.Rows[ i ] ) ;
        maxX = x ;

        for ( int j = 0 ; j < data.Rows.Count ; ++j ) {
          string typeName2 = (string)data.Rows[ j ][ _CmpParameters.RST_HariHugo ] ;

          if ( typeName == typeName2 ) {
            double _x = BeamFrameWidthBySectionType( data.Rows[ j ] ) ;

            if ( x < _x ) {
              maxX = _x ;
            }
          }
        }

        ret.Add( maxX ) ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>符号別最大片持ち梁幅(複数断面の合計を含む)</summary>
    ///
    /// <history><p>2013/07/09 Created  GSA,Inc. Ryo Kuroda</p>
    ///           <p>2014/06/16 Modified GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Collections.Generic.IList<double> BeamWidthByMark_Canti( System.Data.DataTable data )
    {
      Collections.Generic.IList<double> ret = new Collections.Generic.List<double>() ;

      // 既符号名
      Collections.Generic.IList<string> names = new Collections.Generic.List<string>() ;

      for ( int i = 0 ; i < data.Rows.Count ; ++i ) {
        string typeName = (string)data.Rows[ i ][ _CmpParameters.HariHugo_Katamoti ] ;

        if ( names.Contains( typeName ) ) {
          continue ;
        }

        names.Add( typeName ) ;

        double maxX = 0 ;

        double x = BeamFrameWidthBySectionType_Canti( data.Rows[ i ] ) ;
        maxX = x ;

        for ( int j = 0 ; j < data.Rows.Count ; ++j ) {
          string typeName2 = (string)data.Rows[ j ][ _CmpParameters.HariHugo_Katamoti ] ;

          if ( typeName == typeName2 ) {
            double _x = BeamFrameWidthBySectionType_Canti( data.Rows[ j ] ) ;

            if ( x < _x ) {
              maxX = _x ;
            }
          }
        }

        ret.Add( maxX ) ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>断面タイプ別梁幅合計</summary>
    ///
    /// <history><p>2013/04/26 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public double BeamFrameWidthBySectionType( System.Data.DataRow row )
    {
      double ret = 0 ;

      int sectionType = BeamSectionTypeNum( row ) ;

      double i_W = (double)row[ _CmpParameters.s_B ] ;
      double c_W = (double)row[ _CmpParameters.c_B ] ;
      double j_W = (double)row[ _CmpParameters.e_B ] ;

      if ( sectionType == 0 ) {
        ret = c_W ;
      }

      if ( sectionType == 1 ) {
        ret = i_W + c_W ;
      }

      if ( sectionType == 2 ) {
        ret = i_W + c_W + j_W ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>断面タイプ別片持ち梁幅合計</summary>
    ///
    /// <history><p>2013/07/09 Created  GSA,Inc. Ryo Kuroda</p>
    ///           <p>2014/06/16 Modified GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public double BeamFrameWidthBySectionType_Canti( System.Data.DataRow row )
    {
      double ret = 0 ;

      int sectionType = BeamSectionTypeNum_Canti( row ) ;

      double i_W = (double)row[ _CmpParameters.MototanHarihaba ] ;
      double j_W = (double)row[ _CmpParameters.SentanHarihaba ] ;

      if ( sectionType == 0 ) {
        ret = i_W ;
      }

      if ( sectionType == 1 ) {
        ret = i_W + j_W ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>梁断面タイプ判定</summary>
    ///
    /// <returns>0 1断面(すべて同じ)
    ///          1 2断面(i端 = j端 ≠ 中央)
    ///          2 3断面(その他  i ≠ jなど)</returns>
    ///
    /// <history><p>2013/04/26 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public int BeamSectionTypeNum( System.Data.DataRow row )
    {
      int ret = 0 ;

      // すべて同じならそのまま
      // 中央だけ違えばflag1
      // それ以外ならばflag2

      bool isOnlyCenterDifferent = false ;
      bool isEdgeAndCenterDifferent = false ;

      // 梁幅
      if ( ( (double)row[ _CmpParameters.s_B ] == (double)row[ _CmpParameters.c_B ] ) && (double)row[ _CmpParameters.s_B ] == (double)row[ _CmpParameters.e_B ] ) {
      }
      else if ( ( (double)row[ _CmpParameters.s_B ] != (double)row[ _CmpParameters.c_B ] ) && (double)row[ _CmpParameters.s_B ] == (double)row[ _CmpParameters.e_B ] ) {
        isOnlyCenterDifferent = true ;
      }
      else {
        isEdgeAndCenterDifferent = true ;
      }

      // 梁成
      if ( ( (double)row[ _CmpParameters.s_D ] == (double)row[ _CmpParameters.c_D ] ) && (double)row[ _CmpParameters.s_D ] == (double)row[ _CmpParameters.e_D ] ) {
      }
      else if ( ( (double)row[ _CmpParameters.s_D ] != (double)row[ _CmpParameters.c_D ] ) && (double)row[ _CmpParameters.s_D ] == (double)row[ _CmpParameters.e_D ] ) {
        isOnlyCenterDifferent = true ;
      }
      else {
        isEdgeAndCenterDifferent = true ;
      }

      // 主筋上太径
      if ( (string)row[ _CmpParameters.RST_SyukinItanUeHutokei ] == (string)row[ _CmpParameters.RST_SyukinChuohUeHutokei ] && (string)row[ _CmpParameters.RST_SyukinItanUeHutokei ] == (string)row[ _CmpParameters.RST_SyukinJtanUeHutokei ] ) {
      }
      else if ( (string)row[ _CmpParameters.RST_SyukinItanUeHutokei ] != (string)row[ _CmpParameters.RST_SyukinChuohUeHutokei ] && (string)row[ _CmpParameters.RST_SyukinItanUeHutokei ] == (string)row[ _CmpParameters.RST_SyukinJtanUeHutokei ] ) {
        isOnlyCenterDifferent = true ;
      }
      else {
        isEdgeAndCenterDifferent = true ;
      }

      //主筋上1段筋太径本数
      if ( (int)row[ _CmpParameters.RST_SyukinItanUe1dankinHutokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinChuohUe1dankinHutokeiHonsu ] && (int)row[ _CmpParameters.RST_SyukinItanUe1dankinHutokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinJtanUe1dankinHutokeiHonsu ] ) {
      }
      else if ( (int)row[ _CmpParameters.RST_SyukinItanUe1dankinHutokeiHonsu ] != (int)row[ _CmpParameters.RST_SyukinChuohUe1dankinHutokeiHonsu ] && (int)row[ _CmpParameters.RST_SyukinItanUe1dankinHutokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinJtanUe1dankinHutokeiHonsu ] ) {
        isOnlyCenterDifferent = true ;
      }
      else {
        isEdgeAndCenterDifferent = true ;
      }

      //主筋上2段筋太径本数
      if ( (int)row[ _CmpParameters.RST_SyukinItanUe2dankinHutokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinChuohUe2dankinHutokeiHonsu ] && (int)row[ _CmpParameters.RST_SyukinItanUe2dankinHutokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinJtanUe2dankinHutokeiHonsu ] ) {
        //始端＝中央かつ始端＝終端、1列でよい
      }
      else if ( (int)row[ _CmpParameters.RST_SyukinItanUe2dankinHutokeiHonsu ] != (int)row[ _CmpParameters.RST_SyukinChuohUe2dankinHutokeiHonsu ] && (int)row[ _CmpParameters.RST_SyukinItanUe2dankinHutokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinJtanUe2dankinHutokeiHonsu ] ) {
        //始端!=中央かつ、始端=終端、3列
        isOnlyCenterDifferent = true ;
      }
      else {
        isEdgeAndCenterDifferent = true ;
      }

      //主筋上3段筋太径本数
      if ( (int)row[ _CmpParameters.RST_SyukinItanUe3dankinHutokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinChuohUe3dankinHutokeiHonsu ] && (int)row[ _CmpParameters.RST_SyukinItanUe3dankinHutokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinJtanUe3dankinHutokeiHonsu ] ) {
      }
      else if ( (int)row[ _CmpParameters.RST_SyukinItanUe3dankinHutokeiHonsu ] != (int)row[ _CmpParameters.RST_SyukinChuohUe3dankinHutokeiHonsu ] && (int)row[ _CmpParameters.RST_SyukinItanUe3dankinHutokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinJtanUe3dankinHutokeiHonsu ] ) {
        isOnlyCenterDifferent = true ;
      }
      else {
        isEdgeAndCenterDifferent = true ;
      }

      // 主筋下太径
      if ( (string)row[ _CmpParameters.RST_SyukinItanSitaHutokei ] == (string)row[ _CmpParameters.RST_SyukinChuohSitaHutokei ] && (string)row[ _CmpParameters.RST_SyukinItanSitaHutokei ] == (string)row[ _CmpParameters.RST_SyukinJtanSitaHutokei ] ) {
      }
      else if ( (string)row[ _CmpParameters.RST_SyukinItanSitaHutokei ] != (string)row[ _CmpParameters.RST_SyukinChuohSitaHutokei ] && (string)row[ _CmpParameters.RST_SyukinItanSitaHutokei ] == (string)row[ _CmpParameters.RST_SyukinJtanSitaHutokei ] ) {
        isOnlyCenterDifferent = true ;
      }
      else {
        isEdgeAndCenterDifferent = true ;
      }

      //主筋下1段筋太径本数
      if ( (int)row[ _CmpParameters.RST_SyukinItanSita1dankinHutokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinChuohSita1dankinHutokeiHonsu ] && (int)row[ _CmpParameters.RST_SyukinItanSita1dankinHutokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinJtanSita1dankinHutokeiHonsu ] ) {
      }
      else if ( (int)row[ _CmpParameters.RST_SyukinItanSita1dankinHutokeiHonsu ] != (int)row[ _CmpParameters.RST_SyukinChuohSita1dankinHutokeiHonsu ] && (int)row[ _CmpParameters.RST_SyukinItanSita1dankinHutokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinJtanSita1dankinHutokeiHonsu ] ) {
        isOnlyCenterDifferent = true ;
      }
      else {
        isEdgeAndCenterDifferent = true ;
      }

      //主筋下2段筋太径本数
      if ( (int)row[ _CmpParameters.RST_SyukinItanSita2dankinHutokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinChuohSita2dankinHutokeiHonsu ] && (int)row[ _CmpParameters.RST_SyukinItanSita2dankinHutokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinJtanSita2dankinHutokeiHonsu ] ) {
      }
      else if ( (int)row[ _CmpParameters.RST_SyukinItanSita2dankinHutokeiHonsu ] != (int)row[ _CmpParameters.RST_SyukinChuohSita2dankinHutokeiHonsu ] && (int)row[ _CmpParameters.RST_SyukinItanSita2dankinHutokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinJtanSita2dankinHutokeiHonsu ] ) {
        isOnlyCenterDifferent = true ;
      }
      else {
        isEdgeAndCenterDifferent = true ;
      }

      //主筋下3段筋太径本数
      if ( (int)row[ _CmpParameters.RST_SyukinItanSita3dankinHutokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinChuohSita3dankinHutokeiHonsu ] && (int)row[ _CmpParameters.RST_SyukinItanSita3dankinHutokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinJtanSita3dankinHutokeiHonsu ] ) {
      }
      else if ( (int)row[ _CmpParameters.RST_SyukinItanSita3dankinHutokeiHonsu ] != (int)row[ _CmpParameters.RST_SyukinChuohSita3dankinHutokeiHonsu ] && (int)row[ _CmpParameters.RST_SyukinItanSita3dankinHutokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinJtanSita3dankinHutokeiHonsu ] ) {
        isOnlyCenterDifferent = true ;
      }
      else {
        isEdgeAndCenterDifferent = true ;
      }

      // 主筋上細径
      // ("なし"の場合はブランク)
      string iUeHosokei = (string)row[ _CmpParameters.RST_SyukinItanUeHosokei ] == "なし" ? "" : (string)row[ _CmpParameters.RST_SyukinItanUeHosokei ] ;
      string chuohUeHosokei = (string)row[ _CmpParameters.RST_SyukinChuohUeHosokei ] == "なし" ? "" : (string)row[ _CmpParameters.RST_SyukinChuohUeHosokei ] ;
      string jUeHoeokei = (string)row[ _CmpParameters.RST_SyukinJtanUeHosokei ] == "なし" ? "" : (string)row[ _CmpParameters.RST_SyukinJtanUeHosokei ] ;

      if ( iUeHosokei == chuohUeHosokei && iUeHosokei == jUeHoeokei ) {
      }
      else if ( iUeHosokei != chuohUeHosokei && iUeHosokei == jUeHoeokei ) {
        isOnlyCenterDifferent = true ;
      }
      else {
        isEdgeAndCenterDifferent = true ;
      }

      //主筋上1段筋細径本数
      if ( (int)row[ _CmpParameters.RST_SyukinItanUe1dankinHosokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinChuohUe1dankinHosokeiHonsu ] && (int)row[ _CmpParameters.RST_SyukinItanUe1dankinHosokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinJtanUe1dankinHosokeiHonsu ] ) {
      }
      else if ( (int)row[ _CmpParameters.RST_SyukinItanUe1dankinHosokeiHonsu ] != (int)row[ _CmpParameters.RST_SyukinChuohUe1dankinHosokeiHonsu ] && (int)row[ _CmpParameters.RST_SyukinItanUe1dankinHosokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinJtanUe1dankinHosokeiHonsu ] ) {
        isOnlyCenterDifferent = true ;
      }
      else {
        isEdgeAndCenterDifferent = true ;
      }

      //主筋上2段筋細径本数
      if ( (int)row[ _CmpParameters.RST_SyukinItanUe2dankinHosokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinChuohUe2dankinHosokeiHonsu ] && (int)row[ _CmpParameters.RST_SyukinItanUe2dankinHosokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinJtanUe2dankinHosokeiHonsu ] ) {
      }
      else if ( (int)row[ _CmpParameters.RST_SyukinItanUe2dankinHosokeiHonsu ] != (int)row[ _CmpParameters.RST_SyukinChuohUe2dankinHosokeiHonsu ] && (int)row[ _CmpParameters.RST_SyukinItanUe2dankinHosokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinJtanUe2dankinHosokeiHonsu ] ) {
        isOnlyCenterDifferent = true ;
      }
      else {
        isEdgeAndCenterDifferent = true ;
      }

      //主筋上3段筋細径本数
      if ( (int)row[ _CmpParameters.RST_SyukinItanUe3dankinHosokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinChuohUe3dankinHosokeiHonsu ] && (int)row[ _CmpParameters.RST_SyukinItanUe3dankinHosokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinJtanUe3dankinHosokeiHonsu ] ) {
      }
      else if ( (int)row[ _CmpParameters.RST_SyukinItanUe3dankinHosokeiHonsu ] != (int)row[ _CmpParameters.RST_SyukinChuohUe3dankinHosokeiHonsu ] && (int)row[ _CmpParameters.RST_SyukinItanUe3dankinHosokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinJtanUe3dankinHosokeiHonsu ] ) {
        isOnlyCenterDifferent = true ;
      }
      else {
        isEdgeAndCenterDifferent = true ;
      }

      // 主筋下細径
      // ("なし"の場合はブランク)
      string iSitaHosokei = (string)row[ _CmpParameters.RST_SyukinItanSitaHosokei ] == "なし" ? "" : (string)row[ _CmpParameters.RST_SyukinItanSitaHosokei ] ;
      string chuohSitaHosokei = (string)row[ _CmpParameters.RST_SyukinChuohSitaHosokei ] == "なし" ? "" : (string)row[ _CmpParameters.RST_SyukinChuohSitaHosokei ] ;
      string jSitaHosokei = (string)row[ _CmpParameters.RST_SyukinJtanSitaHosokei ] == "なし" ? "" : (string)row[ _CmpParameters.RST_SyukinJtanSitaHosokei ] ;

      if ( iSitaHosokei == chuohSitaHosokei && iSitaHosokei == jSitaHosokei ) {
      }
      else if ( iSitaHosokei != chuohSitaHosokei && iSitaHosokei == jSitaHosokei ) {
        isOnlyCenterDifferent = true ;
      }
      else {
        isEdgeAndCenterDifferent = true ;
      }

      //主筋下1段筋細径本数
      if ( (int)row[ _CmpParameters.RST_SyukinItanSita1dankinHosokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinChuohSita1dankinHosokeiHonsu ] && (int)row[ _CmpParameters.RST_SyukinItanSita1dankinHosokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinJtanSita1dankinHosokeiHonsu ] ) {
      }
      else if ( (int)row[ _CmpParameters.RST_SyukinItanSita1dankinHosokeiHonsu ] != (int)row[ _CmpParameters.RST_SyukinChuohSita1dankinHosokeiHonsu ] && (int)row[ _CmpParameters.RST_SyukinItanSita1dankinHosokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinJtanSita1dankinHosokeiHonsu ] ) {
        isOnlyCenterDifferent = true ;
      }
      else {
        isEdgeAndCenterDifferent = true ;
      }

      //主筋下2段筋細径本数
      if ( (int)row[ _CmpParameters.RST_SyukinItanSita2dankinHosokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinChuohSita2dankinHosokeiHonsu ] && (int)row[ _CmpParameters.RST_SyukinItanSita2dankinHosokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinJtanSita2dankinHosokeiHonsu ] ) {
      }
      else if ( (int)row[ _CmpParameters.RST_SyukinItanSita2dankinHosokeiHonsu ] != (int)row[ _CmpParameters.RST_SyukinChuohSita2dankinHosokeiHonsu ] && (int)row[ _CmpParameters.RST_SyukinItanSita2dankinHosokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinJtanSita2dankinHosokeiHonsu ] ) {
        isOnlyCenterDifferent = true ;
      }
      else {
        isEdgeAndCenterDifferent = true ;
      }

      //主筋下3段筋細径本数
      if ( (int)row[ _CmpParameters.RST_SyukinItanSita3dankinHosokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinChuohSita3dankinHosokeiHonsu ] && (int)row[ _CmpParameters.RST_SyukinItanSita3dankinHosokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinJtanSita3dankinHosokeiHonsu ] ) {
      }
      else if ( (int)row[ _CmpParameters.RST_SyukinItanSita3dankinHosokeiHonsu ] != (int)row[ _CmpParameters.RST_SyukinChuohSita3dankinHosokeiHonsu ] && (int)row[ _CmpParameters.RST_SyukinItanSita3dankinHosokeiHonsu ] == (int)row[ _CmpParameters.RST_SyukinJtanSita3dankinHosokeiHonsu ] ) {
        isOnlyCenterDifferent = true ;
      }
      else {
        isEdgeAndCenterDifferent = true ;
      }

      // 肋筋径
      if ( (string)row[ _CmpParameters.s_Stirrup_Diameter ] == (string)row[ _CmpParameters.c_Stirrup_Diameter ] && (string)row[ _CmpParameters.s_Stirrup_Diameter ] == (string)row[ _CmpParameters.e_Stirrup_Diameter ] ) {
      }
      else if ( (string)row[ _CmpParameters.s_Stirrup_Diameter ] != (string)row[ _CmpParameters.c_Stirrup_Diameter ] && (string)row[ _CmpParameters.s_Stirrup_Diameter ] == (string)row[ _CmpParameters.e_Stirrup_Diameter ] ) {
        isOnlyCenterDifferent = true ;
      }
      else {
        isEdgeAndCenterDifferent = true ;
      }

      // 肋筋本数
      if ( (int)row[ _CmpParameters.s_Stirrup_Number ] == (int)row[ _CmpParameters.c_Stirrup_Number ] && (int)row[ _CmpParameters.s_Stirrup_Number ] == (int)row[ _CmpParameters.e_Stirrup_Number ] ) {
      }
      else if ( (int)row[ _CmpParameters.s_Stirrup_Number ] != (int)row[ _CmpParameters.c_Stirrup_Number ] && (int)row[ _CmpParameters.s_Stirrup_Number ] == (int)row[ _CmpParameters.e_Stirrup_Number ] ) {
        isOnlyCenterDifferent = true ;
      }
      else {
        isEdgeAndCenterDifferent = true ;
      }

      // 肋筋ピッチ
      if ( (double)row[ _CmpParameters.s_Stirrup_Pitch ] == (double)row[ _CmpParameters.c_Stirrup_Pitch ] && (double)row[ _CmpParameters.s_Stirrup_Pitch ] == (double)row[ _CmpParameters.e_Stirrup_Pitch ] ) {
      }
      else if ( (double)row[ _CmpParameters.s_Stirrup_Pitch ] != (double)row[ _CmpParameters.c_Stirrup_Pitch ] && (double)row[ _CmpParameters.s_Stirrup_Pitch ] == (double)row[ _CmpParameters.e_Stirrup_Pitch ] ) {
        isOnlyCenterDifferent = true ;
      }
      else {
        isEdgeAndCenterDifferent = true ;
      }

      // 腹筋径
      if ( (string)row[ _CmpParameters.s_Web_Diameter ] == (string)row[ _CmpParameters.c_Web_Diameter ] && (string)row[ _CmpParameters.s_Web_Diameter ] == (string)row[ _CmpParameters.e_Web_Diameter ] ) {
      }
      else if ( (string)row[ _CmpParameters.s_Web_Diameter ] != (string)row[ _CmpParameters.c_Web_Diameter ] && (string)row[ _CmpParameters.s_Web_Diameter ] == (string)row[ _CmpParameters.e_Web_Diameter ] ) {
        isOnlyCenterDifferent = true ;
      }
      else {
        isEdgeAndCenterDifferent = true ;
      }

      // 腹筋本数
      if ( (int)row[ _CmpParameters.s_Web_Number ] == (int)row[ _CmpParameters.c_Web_Number ] && (int)row[ _CmpParameters.s_Web_Number ] == (int)row[ _CmpParameters.e_Web_Number ] ) {
      }
      else if ( (int)row[ _CmpParameters.s_Web_Number ] != (int)row[ _CmpParameters.c_Web_Number ] && (int)row[ _CmpParameters.s_Web_Number ] == (int)row[ _CmpParameters.e_Web_Number ] ) {
        isOnlyCenterDifferent = true ;
      }
      else {
        isEdgeAndCenterDifferent = true ;
      }

      // 幅止筋径
      if ( (string)row[ _CmpParameters.s_Spacing_Diameter ] == (string)row[ _CmpParameters.c_Spacing_Diameter ] && (string)row[ _CmpParameters.s_Spacing_Diameter ] == (string)row[ _CmpParameters.e_Spacing_Diameter ] ) {
      }
      else if ( (string)row[ _CmpParameters.s_Spacing_Diameter ] != (string)row[ _CmpParameters.c_Spacing_Diameter ] && (string)row[ _CmpParameters.s_Spacing_Diameter ] == (string)row[ _CmpParameters.e_Spacing_Diameter ] ) {
        isOnlyCenterDifferent = true ;
      }
      else {
        isEdgeAndCenterDifferent = true ;
      }

      // 幅止筋本数
      if ( (int)row[ _CmpParameters.s_Spacing_Number ] == (int)row[ _CmpParameters.c_Spacing_Number ] && (int)row[ _CmpParameters.s_Spacing_Number ] == (int)row[ _CmpParameters.e_Spacing_Number ] ) {
      }
      else if ( (int)row[ _CmpParameters.s_Spacing_Number ] != (int)row[ _CmpParameters.c_Spacing_Number ] && (int)row[ _CmpParameters.s_Spacing_Number ] == (int)row[ _CmpParameters.e_Spacing_Number ] ) {
        isOnlyCenterDifferent = true ;
      }
      else {
        isEdgeAndCenterDifferent = true ;
      }

      //// 幅止筋ピッチ
      //if ((double)row[_CmpParameters.s_Spacing_Pitch] == (double)row[_CmpParameters.c_Spacing_Pitch] &&
      //    (double)row[_CmpParameters.s_Spacing_Pitch] == (double)row[_CmpParameters.e_Spacing_Pitch])
      //{
      //}
      //else if ((double)row[_CmpParameters.s_Spacing_Pitch] != (double)row[_CmpParameters.c_Spacing_Pitch] &&
      //         (double)row[_CmpParameters.s_Spacing_Pitch] == (double)row[_CmpParameters.e_Spacing_Pitch])
      //{
      //  flag1 = true;
      //}
      //else
      //{
      //  flag2 = true;
      //}

      if ( isOnlyCenterDifferent == true ) {
        ret = 1 ;
      }

      if ( isEdgeAndCenterDifferent == true ) {
        ret = 2 ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>片持ち梁断面タイプ判定</summary>
    ///
    /// <returns>0 1断面(すべて同じ)
    ///          1 2断面(元端 ≠ j端)</returns>
    ///
    /// <history><p>2013/07/09 Created  GSA,Inc. Ryo Kuroda</p>
    ///           <p>2014/06/16 Modified GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public int BeamSectionTypeNum_Canti( System.Data.DataRow row )
    {
      int ret = 0 ;

      // 両端が同じならそのまま
      // 両端が違えばflag1

      bool flag1 = false ;

      // 梁幅
      if ( (double)row[ _CmpParameters.MototanHarihaba ] == (double)row[ _CmpParameters.SentanHarihaba ] ) {
      }
      else {
        flag1 = true ;
      }

      // 梁成
      if ( (double)row[ _CmpParameters.MototanHarisei ] == (double)row[ _CmpParameters.SentanHarisei ] ) {
      }
      else {
        flag1 = true ;
      }

      // 主筋上太径
      if ( (string)row[ _CmpParameters.MototanUeSyukinHutokei ] == (string)row[ _CmpParameters.SentanUeSyukinHutokei ] ) {
      }
      else {
        flag1 = true ;
      }

      //主筋上1段筋太径本数
      if ( (int)row[ _CmpParameters.MototanUeSyukin1danHutokinHonsu ] == (int)row[ _CmpParameters.SentanUeSyukin1danHutokinHonsu ] ) {
      }
      else {
        flag1 = true ;
      }

      //主筋上2段筋太径本数
      if ( (int)row[ _CmpParameters.MototanUeSyukin2danHutokinHonsu ] == (int)row[ _CmpParameters.SentanUeSyukin2danHutokinHonsu ] ) {
      }
      else {
        flag1 = true ;
      }

      //主筋上3段筋太径本数
      if ( (int)row[ _CmpParameters.MototanUeSyukin3danHutokinHonsu ] == (int)row[ _CmpParameters.SentanUeSyukin3danHutokinHonsu ] ) {
      }
      else {
        flag1 = true ;
      }

      // 主筋下太径
      if ( (string)row[ _CmpParameters.MototanSitaSyukinHutokei ] == (string)row[ _CmpParameters.SentanSitaSyukinHutokei ] ) {
      }
      else {
        flag1 = true ;
      }

      //主筋下1段筋太径本数
      if ( (int)row[ _CmpParameters.MototanSitaSyukin1danHutokinHonsu ] == (int)row[ _CmpParameters.SentanSitaSyukin1danHutokinHonsu ] ) {
      }
      else {
        flag1 = true ;
      }

      //主筋下2段筋太径本数
      if ( (int)row[ _CmpParameters.MototanSitaSyukin2danHutokinHonsu ] == (int)row[ _CmpParameters.SentanSitaSyukin2danHutokinHonsu ] ) {
      }
      else {
        flag1 = true ;
      }

      //主筋下3段筋太径本数
      if ( (int)row[ _CmpParameters.MototanSitaSyukin3danHutokinHonsu ] == (int)row[ _CmpParameters.SentanSitaSyukin3danHutokinHonsu ] ) {
      }
      else {
        flag1 = true ;
      }

      // 主筋上細径
      string iUeHosokei = (string)row[ _CmpParameters.MototanUeSyukinHosokei ] == "なし" ? "" : (string)row[ _CmpParameters.MototanUeSyukinHosokei ] ;
      string jUeHosokei = (string)row[ _CmpParameters.SentanUeSyukinHosokei ] == "なし" ? "" : (string)row[ _CmpParameters.SentanUeSyukinHosokei ] ;

      if ( iUeHosokei == jUeHosokei ) {
      }
      else {
        flag1 = true ;
      }

      //主筋上1段筋細径本数
      if ( (int)row[ _CmpParameters.MototanUeSyukin1danHosokinHonsu ] == (int)row[ _CmpParameters.SentanUeSyukin1danHosokinHonsu ] ) {
      }
      else {
        flag1 = true ;
      }

      //主筋上2段筋細径本数
      if ( (int)row[ _CmpParameters.MototanUeSyukin2danHosokinHonsu ] == (int)row[ _CmpParameters.SentanUeSyukin2danHosokinHonsu ] ) {
      }
      else {
        flag1 = true ;
      }

      //主筋上3段筋細径本数
      if ( (int)row[ _CmpParameters.MototanUeSyukin3danHosokinHonsu ] == (int)row[ _CmpParameters.SentanUeSyukin3danHosokinHonsu ] ) {
      }
      else {
        flag1 = true ;
      }

      // 主筋下細径
      string iSitaHosokei = (string)row[ _CmpParameters.MototanSitaSyukinHosokei ] == "なし" ? "" : (string)row[ _CmpParameters.MototanSitaSyukinHosokei ] ;
      string jsitaHosokei = (string)row[ _CmpParameters.SentanSitaSyukinHosokei ] == "なし" ? "" : (string)row[ _CmpParameters.SentanSitaSyukinHosokei ] ;

      if ( iSitaHosokei == jsitaHosokei ) {
      }
      else {
        flag1 = true ;
      }

      //主筋下1段筋細径本数
      if ( (int)row[ _CmpParameters.MototanSitaSyukin1danHosokinHonsu ] == (int)row[ _CmpParameters.SentanSitaSyukin1danHosokinHonsu ] ) {
      }
      else {
        flag1 = true ;
      }

      //主筋下2段筋細径本数
      if ( (int)row[ _CmpParameters.MototanSitaSyukin2danHosokinHonsu ] == (int)row[ _CmpParameters.SentanSitaSyukin2danHosokinHonsu ] ) {
      }
      else {
        flag1 = true ;
      }

      //主筋下3段筋細径本数
      if ( (int)row[ _CmpParameters.MototanSitaSyukin3danHosokinHonsu ] == (int)row[ _CmpParameters.SentanSitaSyukin3danHosokinHonsu ] ) {
      }
      else {
        flag1 = true ;
      }

      // 肋筋径
      if ( (string)row[ _CmpParameters.MototanAbarakinkei ] == (string)row[ _CmpParameters.SentanAbarakinkei ] ) {
      }
      else {
        flag1 = true ;
      }

      // 肋筋本数
      if ( (int)row[ _CmpParameters.MototanAbarakinHonsu ] == (int)row[ _CmpParameters.SentanAbarakinHonsu ] ) {
      }
      else {
        flag1 = true ;
      }

      // 肋筋ピッチ
      if ( (double)row[ _CmpParameters.MototanAbarakinPitch ] == (double)row[ _CmpParameters.SentanAbarakinPitch ] ) {
      }
      else {
        flag1 = true ;
      }

      // 腹筋径
      if ( (string)row[ _CmpParameters.MototanHarakinkei ] == (string)row[ _CmpParameters.SentanHarakinkei ] ) {
      }
      else {
        flag1 = true ;
      }

      // 腹筋本数
      if ( (int)row[ _CmpParameters.MototanHarakinHonsu ] == (int)row[ _CmpParameters.SentanHarakinHonsu ] ) {
      }
      else {
        flag1 = true ;
      }

      // 幅止筋径
      if ( (string)row[ _CmpParameters.MototanHabadomekinkei ] == (string)row[ _CmpParameters.SentanHabadomekinkei ] ) {
      }
      else {
        flag1 = true ;
      }

      // 幅止筋本数
      if ( (int)row[ _CmpParameters.MototanHabadomekinHonsu ] == (int)row[ _CmpParameters.SentanHabadomekinHonsu ] ) {
      }
      else {
        flag1 = true ;
      }

      //// 幅止筋ピッチ
      //if ((double)row[_CmpParameters.MototanHabadomekinPitch] == (double)row[_CmpParameters.SentanHabadomekinPitch])
      //{
      //}
      //else
      //{
      //  flag1 = true;
      //}

      if ( flag1 == true ) {
        ret = 1 ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>特定符号最大梁幅(複数断面の合計)</summary>
    ///
    /// <history><p>2013/04/24 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public double BeamWidthByMark( System.Data.DataTable data, string hugo )
    {
      double ret = 0 ;

      for ( int i = 0 ; i < data.Rows.Count ; ++i ) {
        string hugoName = (string)data.Rows[ i ][ _CmpParameters.RST_HariHugo ] ;

        if ( hugoName != hugo ) {
          continue ;
        }

        double x = BeamFrameWidthBySectionType( data.Rows[ i ] ) ;

        if ( ret < x ) {
          ret = x ;
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>特定符号最大片持ち梁幅(複数断面の合計)</summary>
    ///
    /// <history><p>2013/07/09 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public double BeamWidthByMark_Canti( System.Data.DataTable data, string hugo )
    {
      double ret = 0 ;

      for ( int i = 0 ; i < data.Rows.Count ; ++i ) {
        string hugoName = (string)data.Rows[ i ][ _CmpParameters.HariHugo_Katamoti ] ;

        if ( hugoName != hugo ) {
          continue ;
        }

        double x = BeamFrameWidthBySectionType_Canti( data.Rows[ i ] ) ;

        if ( ret < x ) {
          ret = x ;
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>特定符号最大梁幅(個別断面)</summary>
    ///
    /// <param name="typeNum">梁断面タイプ</param>
    ///
    /// <history><p>2013/04/24 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Collections.Generic.IList<double> BeamSecWidthAry( System.Data.DataTable data, string hugo, ref int typeNum )
    {
      Collections.Generic.IList<double> ret = new Collections.Generic.List<double>() ;

      double d = 0 ;

      for ( int i = 0 ; i < data.Rows.Count ; ++i ) {
        string hugoName = (string)data.Rows[ i ][ _CmpParameters.RST_HariHugo ] ;
        if ( hugoName != hugo ) {
          continue ;
        }

        double x = BeamFrameWidthBySectionType( data.Rows[ i ] ) ;

        if ( d < x ) {
          d = x ;
          ret.Clear() ;

          double i_W = (double)data.Rows[ i ][ _CmpParameters.s_B ] ;
          double c_W = (double)data.Rows[ i ][ _CmpParameters.c_B ] ;
          double j_W = (double)data.Rows[ i ][ _CmpParameters.e_B ] ;

          int secTypeNum = BeamSectionTypeNum( data.Rows[ i ] ) ;
          
          if ( secTypeNum == 0 ) {
            // 全断
            typeNum = secTypeNum ;
            ret.Add( c_W ) ;
          }
          else if ( secTypeNum == 1 ) {
            // 始端 > 全断
            typeNum = secTypeNum ;
            ret.Add( i_W ) ;
            ret.Add( c_W ) ;
          }
          else if ( secTypeNum == 2 ) {
            // 始端 > 全断 > 終端
            typeNum = secTypeNum ;
            ret.Add( i_W ) ;
            ret.Add( c_W ) ;
            ret.Add( j_W ) ;
          }
        }
      }

      
      return ret ;
    }

    /// <summary>
    /// 特定符号＆特定レベルの最大梁幅(個別断面)
    /// public Collections.Generic.IList&lt;double&gt; BeamSecWidthAry( System.Data.DataTable data, string hugo, ref int typeNum )に対し
    /// levelAryを加え、row絞り込みをし、幅よりsecTypeNumを優先するようにした。
    /// 両端が違うのに分かれて作図されない不具合対応のために作成。
    /// </summary>
    /// <param name="data"></param>
    /// <param name="hugo">符号文字</param>
    /// <param name="level">レベル文字</param>
    /// <param name="typeNum"></param>
    /// <returns></returns>
    public Collections.Generic.IList<double> BeamSecWidthAry( System.Data.DataTable data, string hugo, IList<string> levelAry, ref int typeNum )
    {
      Collections.Generic.IList<double> ret = new Collections.Generic.List<double>() ;

      // data.Rowから該当する項目を絞り込んだリストを作成。
      var rowList = new List<DataRow>() ;
      for ( int i = 0 ; i < data.Rows.Count ; ++i ) {
        var hugoName = (string)data.Rows[ i ][ _CmpParameters.RST_HariHugo ] ;
        var levelName = (string)data.Rows[ i ][ _CmpParameters.LevelFrameTitle ] ;
        foreach ( var level in levelAry ) {
          if ( hugoName == hugo && levelName == level ) rowList.Add( data.Rows[ i ] ) ;
        }
      }

      double d = 0 ;

      foreach ( var row in rowList ) {
        double x = BeamFrameWidthBySectionType( row ) ; //断面タイプ別梁幅合計
        int secTypeNum = BeamSectionTypeNum( row ) ;  //断面分割タイプ
        if ( secTypeNum <= typeNum ) continue ;

        double i_W = (double)row[ _CmpParameters.s_B ] ;
        double c_W = (double)row[ _CmpParameters.c_B ] ;
        double j_W = (double)row[ _CmpParameters.e_B ] ;
        if ( secTypeNum == 0 ) {
          // 全断
          if ( d < x ) d = x ;
          typeNum = secTypeNum ;
          ret.Clear() ;
          ret.Add( c_W ) ;
        }
        else if ( secTypeNum == 1 ) {
          // 始端 > 全断
          if ( d < x ) d = x ;
          typeNum = secTypeNum ;
          ret.Clear() ;
          ret.Add( i_W ) ;
          ret.Add( c_W ) ;
        }
        else if ( secTypeNum == 2 ) {
          // 始端 > 全断 > 終端
          typeNum = secTypeNum ;
          if ( d < x ) d = x ;
          ret.Clear() ;
          ret.Add( i_W ) ;
          ret.Add( c_W ) ;
          ret.Add( j_W ) ;
        }

      }

      return ret ;
    }
    
    /// ================================================================================
    /// <summary>特定符号最大片持ち梁幅(個別断面)</summary>
    ///
    /// <param name="typeNum">梁断面タイプ</param>
    ///
    /// <history><p>2013/07/09 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Collections.Generic.IList<double> BeamSecWidthAry_Canti( System.Data.DataTable data, string hugo, ref int typeNum )
    {
      Collections.Generic.IList<double> ret = new Collections.Generic.List<double>() ;

      double d = 0 ;

      for ( int i = 0 ; i < data.Rows.Count ; ++i ) {
        string hugoName = (string)data.Rows[ i ][ _CmpParameters.HariHugo_Katamoti ] ;
        if ( hugoName != hugo ) {
          continue ;
        }

        double x = BeamFrameWidthBySectionType_Canti( data.Rows[ i ] ) ;

        if ( d < x ) {
          d = x ;
          ret.Clear() ;

          double i_W = (double)data.Rows[ i ][ _CmpParameters.MototanHarihaba ] ;
          double j_W = (double)data.Rows[ i ][ _CmpParameters.SentanHarihaba ] ;

          int secTypeNum = BeamSectionTypeNum_Canti( data.Rows[ i ] ) ;

          if ( secTypeNum == 0 ) {
            // 全断
            typeNum = secTypeNum ;
            ret.Add( i_W ) ;
          }
          else if ( secTypeNum == 1 ) {
            // 元端 > 先端
            typeNum = secTypeNum ;
            ret.Add( i_W ) ;
            ret.Add( j_W ) ;
          }
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>特定階最大梁高さ</summary>
    ///
    /// <history><p>2013/04/24 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public double BeamHeightByLevel( System.Data.DataTable data, string level )
    {
      double ret = 0 ;

      if ( data != null ) {
        for ( int i = 0 ; i < data.Rows.Count ; ++i ) {
          string l = (string)data.Rows[ i ][ _CmpParameters.LevelFrameTitle ] ;

          if ( l != level ) {
            continue ;
          }

          double h = (double)data.Rows[ i ][ _CmpParameters.s_D ] ;
          if ( ret < h ) {
            ret = h ;
          }

          h = (double)data.Rows[ i ][ _CmpParameters.c_D ] ;
          if ( ret < h ) {
            ret = h ;
          }

          h = (double)data.Rows[ i ][ _CmpParameters.e_D ] ;
          if ( ret < h ) {
            ret = h ;
          }
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>特定階最大梁高さ - 片持ち</summary>
    ///
    /// <history><p>2014/06/16 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public double BeamHeightByLevel_Canti( System.Data.DataTable data, string level )
    {
      double ret = 0 ;

      if ( data != null ) {
        for ( int i = 0 ; i < data.Rows.Count ; ++i ) {
          string l = (string)data.Rows[ i ][ _CmpParameters.LevelFrameTitle ] ;

          if ( l != level ) {
            continue ;
          }

          double h = (double)data.Rows[ i ][ _CmpParameters.MototanHarisei ] ;
          if ( ret < h ) {
            ret = h ;
          }

          h = (double)data.Rows[ i ][ _CmpParameters.SentanHarisei ] ;
          if ( ret < h ) {
            ret = h ;
          }
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>特定符号最大断面数</summary>
    ///
    /// <history><p>2013/04/24 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public int BeamSectionNum( System.Data.DataTable data, string hugo )
    {
      int ret = 0 ;

      for ( int i = 0 ; i < data.Rows.Count ; ++i ) {
        string hugoName = "" ;

        bool isKatamoti = false ;

        try {
          hugoName = (string)data.Rows[ i ][ _CmpParameters.RST_HariHugo ] ;
        }
        catch {
          isKatamoti = true ;
          hugoName = (string)data.Rows[ i ][ _CmpParameters.HariHugo_Katamoti ] ;
        }

        if ( hugo != hugoName ) {
          continue ;
        }

        int secTypeNum = 0 ;

        if ( isKatamoti == false ) {
          secTypeNum = BeamSectionTypeNum( data.Rows[ i ] ) ;
        }
        else {
          secTypeNum = BeamSectionTypeNum_Canti( data.Rows[ i ] ) ;
        }

        if ( secTypeNum == 0 ) {
          ret = 1 ;
        }
        else if ( secTypeNum == 1 ) {
          ret = 2 ;
        }
        else {
          ret = 3 ;
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>梁階名(名前降順)</summary>
    ///
    /// <history><p>2013/04/26 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Collections.Generic.List<string> GetAllBeamLevelAry( Collections.Generic.IList<Revit.DB.FamilySymbol> beamAry )
    {
      Collections.Generic.List<string> ret = new Collections.Generic.List<string>() ;

      foreach ( Revit.DB.FamilySymbol beam in beamAry ) {
        string level = _CmpElements.GetBeamTypeLevel( beam, _CmpParameters.RST_HariHugo ) ;

        if ( ! ret.Contains( level ) ) {
          ret.Add( level ) ;
        }
      }

      if ( ret.Count < 2 ) {
        return ret ;
      }

      ret.Sort() ;

      if ( string.Compare( ret[ 0 ], ret[ ret.Count - 1 ] ) < 0 ) {
        ret.Reverse() ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>梁階名(名前降順) - 片持ち梁</summary>
    ///
    /// <history><p>2014/06/16 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Collections.Generic.List<string> GetAllBeamLevelAry_Canti( Collections.Generic.IList<Revit.DB.FamilySymbol> beamAry )
    {
      Collections.Generic.List<string> ret = new Collections.Generic.List<string>() ;

      foreach ( Revit.DB.FamilySymbol beam in beamAry ) {
        string level = _CmpElements.GetBeamTypeLevel( beam, _CmpParameters.HariHugo_Katamoti ) ;

        if ( ! ret.Contains( level ) ) {
          ret.Add( level ) ;
        }
      }

      if ( ret.Count < 2 ) {
        return ret ;
      }

      ret.Sort() ;

      if ( string.Compare( ret[ 0 ], ret[ ret.Count - 1 ] ) < 0 ) {
        ret.Reverse() ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>全梁符号名(名前順)</summary>
    ///
    /// <history><p>2013/04/26 Created  GSA,Inc. Ryo Kuroda</p>
    ///           <p>2015/04/09 Modified GSA, Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Collections.Generic.List<string> GetAllBeamHugoAry( Collections.Generic.IList<Revit.DB.FamilySymbol> beamAry )
    {
      Collections.Generic.List<string> ret = new Collections.Generic.List<string>() ;

      foreach ( Revit.DB.FamilySymbol beam in beamAry ) {
        Revit.DB.Parameter param = beam.LookupParameter( _CmpParameters.RST_HariHugo ) ;
        string hugo = param.AsString() ;

        //if (hugo == "")
        //{
        //    continue;
        //}

        if ( ! ret.Contains( hugo ) ) {
          ret.Add( hugo ) ;
        }
      }

      ret.Sort( new HugoNameComparer() ) ;

      return ret ;
    }

    /// ================================================================================
    /// <summary>全梁符号名(名前順) - 片持ち</summary>
    ///
    /// <history><p>2013/04/26 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Collections.Generic.List<string> GetAllBeamHugoAry_Canti( Collections.Generic.IList<Revit.DB.FamilySymbol> beamAry )
    {
      Collections.Generic.List<string> ret = new Collections.Generic.List<string>() ;

      foreach ( Revit.DB.FamilySymbol beam in beamAry ) {
        Revit.DB.Parameter param = beam.LookupParameter( _CmpParameters.HariHugo_Katamoti ) ;
        string hugo = param.AsString() ;

        if ( ! ret.Contains( hugo ) ) {
          ret.Add( hugo ) ;
        }
      }

      ret.Sort( new HugoNameComparer() ) ;

      return ret ;
    }

    /// ================================================================================
    /// <summary>位置表示枠区切り線</summary>
    ///
    /// <param name="basePoint">左上の点</param>
    ///
    /// <history><p>2013/04/26 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Collections.Generic.IList<Revit.DB.Line> PositionFrameSpaceLines( Revit.DB.XYZ basePoint, double frameHeight, double leftSpace, double centerSpace, Collections.Generic.IList<double> beamWidthAry )
    {
      Collections.Generic.IList<Revit.DB.Line> ret = new Collections.Generic.List<Revit.DB.Line>() ;

      if ( beamWidthAry.Count == 1 ) {
        return ret ;
      }

      if ( frameHeight < 0 ) {
        frameHeight *= -1 ;
      }

      Revit.DB.Line l = null ;

      if ( beamWidthAry.Count > 1 ) {
        l = _CmpElements.CreateBoundLine( new Revit.DB.XYZ( basePoint.X + leftSpace + beamWidthAry[ 0 ] + centerSpace / 2, basePoint.Y, basePoint.Z ), new Revit.DB.XYZ( basePoint.X + leftSpace + beamWidthAry[ 0 ] + centerSpace / 2, basePoint.Y - frameHeight, basePoint.Z ) ) ;
        ret.Add( l ) ;
      }

      if ( beamWidthAry.Count > 2 ) {
        l = _CmpElements.CreateBoundLine( new Revit.DB.XYZ( basePoint.X + leftSpace + beamWidthAry[ 0 ] + centerSpace + beamWidthAry[ 1 ] + centerSpace / 2, basePoint.Y, basePoint.Z ), new Revit.DB.XYZ( basePoint.X + leftSpace + beamWidthAry[ 0 ] + centerSpace + beamWidthAry[ 1 ] + centerSpace / 2, basePoint.Y - frameHeight, basePoint.Z ) ) ;
        ret.Add( l ) ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>位置表示枠タイトル座標</summary>
    ///
    /// <param name="basePoint">左上の点</param>
    ///
    /// <history><p>2013/04/26 Created  GSA,Inc. Ryo Kuroda</p>
    ///           <p>2014/01/10 Modified  GSA,Inc.Ryo Kuroda</p></history>
    /// ================================================================================
    public Collections.Generic.IList<Revit.DB.XYZ> PositionFrameTitlePoints( Revit.DB.XYZ basePoint, double frameHeight, double leftSpace, double centerSpace, double rightSpace, Collections.Generic.IList<double> beamWidthAry )
    {
      Collections.Generic.IList<Revit.DB.XYZ> ret = new Collections.Generic.List<Revit.DB.XYZ>() ;

      if ( frameHeight < 0 ) {
        frameHeight *= -1 ;
      }

      Revit.DB.XYZ point = null ;

      // 断面数別
      if ( beamWidthAry.Count == 1 ) {
        point = _CmpGeometry.Center2Point( basePoint, new Revit.DB.XYZ( basePoint.X + leftSpace + beamWidthAry[ 0 ] + rightSpace, basePoint.Y - frameHeight, basePoint.Z ) ) ;

        ret.Add( point ) ;
      }
      else if ( beamWidthAry.Count == 2 ) {
        point = _CmpGeometry.Center2Point( basePoint, new Revit.DB.XYZ( basePoint.X + leftSpace + beamWidthAry[ 0 ] + centerSpace / 2, basePoint.Y - frameHeight, basePoint.Z ) ) ;
        ret.Add( point ) ;

        point = _CmpGeometry.Center2Point( new Revit.DB.XYZ( basePoint.X + leftSpace + beamWidthAry[ 0 ] + centerSpace / 2, basePoint.Y, basePoint.Z ), new Revit.DB.XYZ( basePoint.X + leftSpace + beamWidthAry[ 0 ] + centerSpace + beamWidthAry[ 1 ] + rightSpace, basePoint.Y - frameHeight, basePoint.Z ) ) ;

        ret.Add( point ) ;
      }
      else if ( beamWidthAry.Count == 3 ) {
        point = _CmpGeometry.Center2Point( basePoint, new Revit.DB.XYZ( basePoint.X + leftSpace + beamWidthAry[ 0 ] + centerSpace / 2, basePoint.Y - frameHeight, basePoint.Z ) ) ;
        ret.Add( point ) ;

        point = _CmpGeometry.Center2Point( new Revit.DB.XYZ( basePoint.X + leftSpace + beamWidthAry[ 0 ] + centerSpace / 2, basePoint.Y, basePoint.Z ), new Revit.DB.XYZ( basePoint.X + leftSpace + beamWidthAry[ 0 ] + centerSpace + beamWidthAry[ 1 ] + centerSpace / 2, basePoint.Y - frameHeight, basePoint.Z ) ) ;
        ret.Add( point ) ;

        point = _CmpGeometry.Center2Point( new Revit.DB.XYZ( basePoint.X + leftSpace + beamWidthAry[ 0 ] + centerSpace + beamWidthAry[ 1 ] + centerSpace / 2, basePoint.Y, basePoint.Z ), new Revit.DB.XYZ( basePoint.X + leftSpace + beamWidthAry[ 0 ] + centerSpace + beamWidthAry[ 1 ] + centerSpace + beamWidthAry[ 2 ] + rightSpace, basePoint.Y - frameHeight, basePoint.Z ) ) ;
        ret.Add( point ) ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>腹筋同一判定</summary>
    ///
    /// <returns>trueならば同一</returns>
    ///
    /// <history><p>2013/04/30 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public bool IsSameWebBySection( System.Data.DataRow row )
    {
      bool ret = true ;

      try {
        if ( (string)row[ _CmpParameters.s_Web_Diameter ] != (string)row[ _CmpParameters.c_Web_Diameter ] ) {
          ret = false ;
          return ret ;
        }

        if ( (string)row[ _CmpParameters.s_Web_Diameter ] != (string)row[ _CmpParameters.e_Web_Diameter ] ) {
          ret = false ;
          return ret ;
        }

        if ( (string)row[ _CmpParameters.c_Web_Diameter ] != (string)row[ _CmpParameters.e_Web_Diameter ] ) {
          ret = false ;
          return ret ;
        }

        if ( (int)row[ _CmpParameters.s_Web_Number ] != (int)row[ _CmpParameters.c_Web_Number ] ) {
          ret = false ;
          return ret ;
        }

        if ( (int)row[ _CmpParameters.s_Web_Number ] != (int)row[ _CmpParameters.e_Web_Number ] ) {
          ret = false ;
          return ret ;
        }

        if ( (int)row[ _CmpParameters.c_Web_Number ] != (int)row[ _CmpParameters.e_Web_Number ] ) {
          ret = false ;
          return ret ;
        }
      }
      catch {
        ret = IsSameWebBySection_Canti( row ) ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>腹筋同一判定 - 片持ち</summary>
    ///
    /// <returns>trueならば同一</returns>
    ///
    /// <history><p>2013/04/30 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public bool IsSameWebBySection_Canti( System.Data.DataRow row )
    {
      bool ret = true ;

      if ( (string)row[ _CmpParameters.MototanHarakinkei ] != (string)row[ _CmpParameters.SentanHarakinkei ] ) {
        ret = false ;
        return ret ;
      }

      if ( (int)row[ _CmpParameters.MototanHarakinHonsu ] != (int)row[ _CmpParameters.SentanHarakinHonsu ] ) {
        ret = false ;
        return ret ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>肋筋同一判定</summary>
    ///
    /// <returns>trueならば同一</returns>
    ///
    /// <history><p>2013/04/30 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public bool IsSameStirrupBySection( System.Data.DataRow row )
    {
      bool ret = true ;
      try {
        if ( (string)row[ _CmpParameters.s_Stirrup_Diameter ] != (string)row[ _CmpParameters.c_Stirrup_Diameter ] ) {
          ret = false ;
          return ret ;
        }

        if ( (string)row[ _CmpParameters.s_Stirrup_Diameter ] != (string)row[ _CmpParameters.e_Stirrup_Diameter ] ) {
          ret = false ;
          return ret ;
        }

        if ( (string)row[ _CmpParameters.c_Stirrup_Diameter ] != (string)row[ _CmpParameters.e_Stirrup_Diameter ] ) {
          ret = false ;
          return ret ;
        }

        if ( (int)row[ _CmpParameters.s_Stirrup_Number ] != (int)row[ _CmpParameters.c_Stirrup_Number ] ) {
          ret = false ;
          return ret ;
        }

        if ( (int)row[ _CmpParameters.s_Stirrup_Number ] != (int)row[ _CmpParameters.e_Stirrup_Number ] ) {
          ret = false ;
          return ret ;
        }

        if ( (int)row[ _CmpParameters.c_Stirrup_Number ] != (int)row[ _CmpParameters.e_Stirrup_Number ] ) {
          ret = false ;
          return ret ;
        }

        if ( (double)row[ _CmpParameters.s_Stirrup_Pitch ] != (double)row[ _CmpParameters.c_Stirrup_Pitch ] ) {
          ret = false ;
          return ret ;
        }

        if ( (double)row[ _CmpParameters.s_Stirrup_Pitch ] != (double)row[ _CmpParameters.e_Stirrup_Pitch ] ) {
          ret = false ;
          return ret ;
        }

        if ( (double)row[ _CmpParameters.c_Stirrup_Pitch ] != (double)row[ _CmpParameters.e_Stirrup_Pitch ] ) {
          ret = false ;
          return ret ;
        }
      }
      catch {
        ret = IsSameStirrupBySection_Canti( row ) ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>肋筋同一判定 - 片持ち</summary>
    ///
    /// <returns>trueならば同一</returns>
    ///
    /// <history><p>2013/04/30 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public bool IsSameStirrupBySection_Canti( System.Data.DataRow row )
    {
      bool ret = true ;

      if ( (string)row[ _CmpParameters.MototanAbarakinkei ] != (string)row[ _CmpParameters.SentanAbarakinkei ] ) {
        ret = false ;
        return ret ;
      }

      if ( (int)row[ _CmpParameters.MototanAbarakinHonsu ] != (int)row[ _CmpParameters.SentanAbarakinHonsu ] ) {
        ret = false ;
        return ret ;
      }

      if ( (double)row[ _CmpParameters.MototanAbarakinPitch ] != (double)row[ _CmpParameters.SentanAbarakinPitch ] ) {
        ret = false ;
        return ret ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>既定のパラメータ項目を持つ梁か</summary>
    ///
    /// <history><p>2013/05/20 Created  GSA,Inc. Ryo Kuroda</p>
    ///           <p>2014/06/16 Modified GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public bool IsHaveGirderParam( Revit.DB.FamilySymbol famSym )
    {
      bool ret = true ;

      if ( famSym == null ) {
        ret = false ;
        return ret ;
      }

      // 梁種別
      Revit.DB.Parameter syubetuParam = famSym.LookupParameter( _CmpParameters.Girder_Category ) ;
      // 梁種別 片持ち梁
      Revit.DB.Parameter syubetuParam_Katamoti = famSym.LookupParameter( _CmpParameters.HariSyubetu_Katamoti ) ;

      if ( syubetuParam == null && syubetuParam_Katamoti == null ) {
        ret = false ;
        return ret ;
      }

      // 一般
      if ( syubetuParam != null && syubetuParam_Katamoti == null ) {
        string strSyubetu = syubetuParam.AsString() ;

        if ( strSyubetu == _CmpAttribute.ResourceText( "IDS_TXT_GIRDER" ) || strSyubetu == _CmpAttribute.ResourceText( "IDS_TXT_BEAM" ) || strSyubetu == _CmpAttribute.ResourceText( "IDS_TXT_FOUNDATION_GIRDER" ) || strSyubetu == _CmpAttribute.ResourceText( "IDS_TXT_FOUNDATION_BEAM" ) ) {
          #region

          // 梁
          if ( famSym.LookupParameter( _CmpParameters.Girder_Category ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.s_B ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.c_B ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.e_B ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.s_D ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.c_D ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.e_D ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanUeHutokei ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohUeHutokei ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanUeHutokei ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanUe1dankinHutokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohUe1dankinHutokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanUe1dankinHutokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanUe2dankinHutokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohUe2dankinHutokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanUe2dankinHutokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanUe3dankinHutokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohUe3dankinHutokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanUe3dankinHutokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanSitaHutokei ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohSitaHutokei ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanSitaHutokei ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanSita1dankinHutokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohSita1dankinHutokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanSita1dankinHutokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanSita2dankinHutokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohSita2dankinHutokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanSita2dankinHutokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanSita3dankinHutokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohSita3dankinHutokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanSita3dankinHosokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanUeHosokei ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohUeHosokei ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanUeHosokei ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanUe1dankinHosokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohUe1dankinHosokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanUe1dankinHosokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanUe2dankinHosokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohUe2dankinHosokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanUe2dankinHosokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanUe3dankinHosokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohUe3dankinHosokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanUe3dankinHosokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanSitaHosokei ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohSitaHosokei ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanSitaHosokei ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanSita1dankinHosokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohSita1dankinHosokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanSita1dankinHosokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanSita2dankinHosokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohSita2dankinHosokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanSita2dankinHosokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanSita3dankinHosokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohSita3dankinHosokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanSita3dankinHosokeiHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.s_Stirrup_Diameter ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.c_Stirrup_Diameter ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.e_Stirrup_Diameter ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.s_Stirrup_Number ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.c_Stirrup_Number ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.e_Stirrup_Number ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.s_Stirrup_Pitch ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.c_Stirrup_Pitch ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.e_Stirrup_Pitch ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.s_Web_Diameter ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.c_Web_Diameter ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.e_Web_Diameter ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.s_Web_Number ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.c_Web_Number ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.e_Web_Number ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.s_Spacing_Diameter ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.c_Spacing_Diameter ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.e_Spacing_Diameter ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.s_Spacing_Number ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.c_Spacing_Number ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.e_Spacing_Number ) == null ) {
            ret = false ;
          }
          //else if (famSym.LookupParameter(_CmpParameters.s_Spacing_Pitch) == null)
          //{
          //  ret = false;
          //}
          //else if (famSym.LookupParameter(_CmpParameters.c_Spacing_Pitch) == null)
          //{
          //  ret = false;
          //}
          //else if (famSym.LookupParameter(_CmpParameters.e_Spacing_Pitch) == null)
          //{
          //  ret = false;
          //}
          else if ( famSym.LookupParameter( _CmpParameters.RST_HariHugo ) == null ) {
            ret = false ;
          }

          #endregion Member Functions
        }
      }
      // 片持ち
      else if ( syubetuParam == null && syubetuParam_Katamoti != null ) {
        string strSyubetu = syubetuParam_Katamoti.AsString() ;

        if ( strSyubetu == _CmpAttribute.ResourceText( "IDS_TXT_CANTILEVER_GIRDER" ) || strSyubetu == _CmpAttribute.ResourceText( "IDS_TXT_CANTILEVER_BEAM" ) || strSyubetu == _CmpAttribute.ResourceText( "IDS_TXT_CANTILEVER_FOUNDATION_GIRDER" ) || strSyubetu == _CmpAttribute.ResourceText( "IDS_TXT_CANTILEVER_FOUNDATION_BEAM" ) ) {
          #region

          if ( famSym.LookupParameter( _CmpParameters.HariSyubetu_Katamoti ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.MototanHarihaba ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.SentanHarihaba ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.MototanHarisei ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.SentanHarihaba ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.MototanUeSyukinHutokei ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.SentanUeSyukinHutokei ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.MototanUeSyukin1danHutokinHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.SentanUeSyukin1danHutokinHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.MototanUeSyukin2danHutokinHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.SentanUeSyukin2danHutokinHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.MototanUeSyukin3danHutokinHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.SentanUeSyukin3danHutokinHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.MototanSitaSyukinHutokei ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.SentanSitaSyukinHutokei ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.MototanSitaSyukin1danHutokinHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.SentanSitaSyukin1danHutokinHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.MototanSitaSyukin2danHutokinHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.SentanSitaSyukin2danHutokinHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.MototanSitaSyukin3danHutokinHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.SentanSitaSyukin3danHutokinHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.MototanUeSyukinHosokei ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.SentanUeSyukinHosokei ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.MototanUeSyukin1danHosokinHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.SentanUeSyukin1danHosokinHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.MototanUeSyukin2danHosokinHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.SentanUeSyukin2danHosokinHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.MototanUeSyukin3danHosokinHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.SentanUeSyukin3danHosokinHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.MototanSitaSyukinHosokei ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.SentanSitaSyukinHosokei ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.MototanSitaSyukin1danHosokinHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.SentanSitaSyukin1danHosokinHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.MototanSitaSyukin2danHosokinHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.SentanSitaSyukin2danHosokinHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.MototanSitaSyukin3danHosokinHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.SentanSitaSyukin3danHosokinHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.MototanAbarakinkei ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.SentanAbarakinkei ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.MototanAbarakinHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.SentanAbarakinHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.MototanAbarakinPitch ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.SentanAbarakinPitch ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.MototanHarakinkei ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.SentanHarakinkei ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.MototanHarakinHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.SentanHarakinHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.MototanHabadomekinkei ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.SentanHabadomekinkei ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.MototanHabadomekinHonsu ) == null ) {
            ret = false ;
          }
          else if ( famSym.LookupParameter( _CmpParameters.SentanHabadomekinHonsu ) == null ) {
            ret = false ;
          }
          //else if (famSym.LookupParameter(_CmpParameters.MototanHabadomekinPitch) == null)
          //{
          //  ret = false;
          //}
          //else if (famSym.LookupParameter(_CmpParameters.SentanHabadomekinPitch) == null)
          //{
          //  ret = false;
          //}
          else if ( famSym.LookupParameter( _CmpParameters.HariHugo_Katamoti ) == null ) {
            ret = false ;
          }

          #endregion
        }
      }
      else {
        // 両方の梁種別が取れた場合、値を確かめる？

        // 値が同じ場合
        if ( syubetuParam.AsString() == syubetuParam_Katamoti.AsString() ) {
          string strSyubetu = syubetuParam.AsString() ;

          if ( strSyubetu == _CmpAttribute.ResourceText( "IDS_TXT_GIRDER" ) || strSyubetu == _CmpAttribute.ResourceText( "IDS_TXT_BEAM" ) || strSyubetu == _CmpAttribute.ResourceText( "IDS_TXT_FOUNDATION_GIRDER" ) || strSyubetu == _CmpAttribute.ResourceText( "IDS_TXT_FOUNDATION_BEAM" ) ) {
            #region

            // 梁
            if ( famSym.LookupParameter( _CmpParameters.Girder_Category ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.s_B ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.c_B ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.e_B ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.s_D ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.c_D ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.e_D ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanUeHutokei ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohUeHutokei ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanUeHutokei ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanUe1dankinHutokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohUe1dankinHutokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanUe1dankinHutokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanUe2dankinHutokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohUe2dankinHutokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanUe2dankinHutokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanUe3dankinHutokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohUe3dankinHutokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanUe3dankinHutokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanSitaHutokei ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohSitaHutokei ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanSitaHutokei ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanSita1dankinHutokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohSita1dankinHutokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanSita1dankinHutokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanSita2dankinHutokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohSita2dankinHutokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanSita2dankinHutokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanSita3dankinHutokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohSita3dankinHutokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanSita3dankinHosokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanUeHosokei ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohUeHosokei ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanUeHosokei ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanUe1dankinHosokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohUe1dankinHosokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanUe1dankinHosokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanUe2dankinHosokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohUe2dankinHosokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanUe2dankinHosokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanUe3dankinHosokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohUe3dankinHosokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanUe3dankinHosokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanSitaHosokei ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohSitaHosokei ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanSitaHosokei ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanSita1dankinHosokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohSita1dankinHosokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanSita1dankinHosokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanSita2dankinHosokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohSita2dankinHosokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanSita2dankinHosokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinItanSita3dankinHosokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinChuohSita3dankinHosokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.RST_SyukinJtanSita3dankinHosokeiHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.s_Stirrup_Diameter ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.c_Stirrup_Diameter ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.e_Stirrup_Diameter ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.s_Stirrup_Number ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.c_Stirrup_Number ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.e_Stirrup_Number ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.s_Stirrup_Pitch ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.c_Stirrup_Pitch ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.e_Stirrup_Pitch ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.s_Web_Diameter ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.c_Web_Diameter ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.e_Web_Diameter ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.s_Web_Number ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.c_Web_Number ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.e_Web_Number ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.s_Spacing_Diameter ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.c_Spacing_Diameter ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.e_Spacing_Diameter ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.s_Spacing_Number ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.c_Spacing_Number ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.e_Spacing_Number ) == null ) {
              ret = false ;
            }
            //else if (famSym.LookupParameter(_CmpParameters.s_Spacing_Pitch) == null)
            //{
            //  ret = false;
            //}
            //else if (famSym.LookupParameter(_CmpParameters.c_Spacing_Pitch) == null)
            //{
            //  ret = false;
            //}
            //else if (famSym.LookupParameter(_CmpParameters.e_Spacing_Pitch) == null)
            //{
            //  ret = false;
            //}
            else if ( famSym.LookupParameter( _CmpParameters.RST_HariHugo ) == null ) {
              ret = false ;
            }

            #endregion
          }
          else if ( strSyubetu == _CmpAttribute.ResourceText( "IDS_TXT_CANTILEVER_GIRDER" ) || strSyubetu == _CmpAttribute.ResourceText( "IDS_TXT_CANTILEVER_BEAM" ) || strSyubetu == _CmpAttribute.ResourceText( "IDS_TXT_CANTILEVER_FOUNDATION_GIRDER" ) || strSyubetu == _CmpAttribute.ResourceText( "IDS_TXT_CANTILEVER_FOUNDATION_BEAM" ) ) {
            #region

            if ( famSym.LookupParameter( _CmpParameters.HariSyubetu_Katamoti ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.MototanHarihaba ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.SentanHarihaba ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.MototanHarisei ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.SentanHarihaba ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.MototanUeSyukinHutokei ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.SentanUeSyukinHutokei ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.MototanUeSyukin1danHutokinHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.SentanUeSyukin1danHutokinHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.MototanUeSyukin2danHutokinHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.SentanUeSyukin2danHutokinHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.MototanUeSyukin3danHutokinHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.SentanUeSyukin3danHutokinHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.MototanSitaSyukinHutokei ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.SentanSitaSyukinHutokei ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.MototanSitaSyukin1danHutokinHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.SentanSitaSyukin1danHutokinHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.MototanSitaSyukin2danHutokinHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.SentanSitaSyukin2danHutokinHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.MototanSitaSyukin3danHutokinHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.SentanSitaSyukin3danHutokinHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.MototanUeSyukinHosokei ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.SentanUeSyukinHosokei ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.MototanUeSyukin1danHosokinHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.SentanUeSyukin1danHosokinHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.MototanUeSyukin2danHosokinHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.SentanUeSyukin2danHosokinHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.MototanUeSyukin3danHosokinHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.SentanUeSyukin3danHosokinHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.MototanSitaSyukinHosokei ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.SentanSitaSyukinHosokei ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.MototanSitaSyukin1danHosokinHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.SentanSitaSyukin1danHosokinHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.MototanSitaSyukin2danHosokinHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.SentanSitaSyukin2danHosokinHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.MototanSitaSyukin3danHosokinHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.SentanSitaSyukin3danHosokinHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.MototanAbarakinkei ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.SentanAbarakinkei ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.MototanAbarakinHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.SentanAbarakinHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.MototanAbarakinPitch ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.SentanAbarakinPitch ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.MototanHarakinkei ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.SentanHarakinkei ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.MototanHarakinHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.SentanHarakinHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.MototanHabadomekinkei ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.SentanHabadomekinkei ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.MototanHabadomekinHonsu ) == null ) {
              ret = false ;
            }
            else if ( famSym.LookupParameter( _CmpParameters.SentanHabadomekinHonsu ) == null ) {
              ret = false ;
            }
            //else if (famSym.LookupParameter(_CmpParameters.MototanHabadomekinPitch) == null)
            //{
            //  ret = false;
            //}
            //else if (famSym.LookupParameter(_CmpParameters.SentanHabadomekinPitch) == null)
            //{
            //  ret = false;
            //}
            else if ( famSym.LookupParameter( _CmpParameters.HariHugo_Katamoti ) == null ) {
              ret = false ;
            }

            #endregion
          }
        }
        // 値が違う場合
        else {
          ret = false ;
        }
      }

      return ret ;
    }

    // ----- ----- ----- ----- ----- ----- ----- --梁-- ----- ----- ----- ----- ----- ----- -----
    // ----- ----- ----- ----- ----- ----- ----- --配筋-- ----- ----- ----- ----- ----- ----- -----

    /// ================================================================================
    /// <summary>IList(int)順序の逆転</summary>
    ///
    /// <history><p>2013/07/11 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public void IListReverse( ref Collections.Generic.IList<int> ints )
    {
      Collections.Generic.List<int> ary = new Collections.Generic.List<int>() ;

      foreach ( int i in ints ) {
        ary.Add( i ) ;
      }

      ary.Reverse() ;

      ints = ary ;
    }

    /// ================================================================================
    /// <summary>配筋太径細径順序(四隅を含む)</summary>
    ///
    /// <history><p>2013/05/08 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Collections.Generic.IList<int> RebarOrder( int hutokeiHonsu, int hosokeiHonsu )
    {
      Collections.Generic.List<int> ret = new Collections.Generic.List<int>() ;

      // 太径が2本より少ない
      if ( hutokeiHonsu < 2 ) {
        return ret ;
      }

      // 細径本数が太径本数以上
      if ( hosokeiHonsu >= hutokeiHonsu ) {
        return ret ;
      }

      // 太径のみ
      if ( hosokeiHonsu < 1 ) {
        for ( int i = 0 ; i < hutokeiHonsu ; ++i ) {
          ret.Add( 0 ) ;
        }

        return ret ;
      }
      else {
        // 総本数
        int sum = hutokeiHonsu + hosokeiHonsu ;

        // 配置済み鉄筋
        Collections.Generic.List<int> already = new Collections.Generic.List<int>() ;

        // 始端
        already.Add( 0 ) ;
        ret.Add( 0 ) ;

        // 端部からの太径連続回数(端部自体は除く)
        double subtraction = hutokeiHonsu - hosokeiHonsu ;
        double division = subtraction / 2 ;
        double ceiling = System.Math.Ceiling( division ) ;
        int continuity = (int)ceiling - 1 ;

        for ( int i = 0 ; i < continuity ; ++i ) {
          already.Add( 0 ) ;
          ret.Add( 0 ) ;
        }

        // 偶数
        if ( subtraction % 2 == 0 ) {
          // 中央の1つ前まで
          while ( already.Count < sum / 2 - 1 ) {
            already.Add( 1 ) ;
            ret.Add( 1 ) ;

            if ( ( already.Count < sum / 2 - 1 ) == false ) {
              break ;
            }

            already.Add( 0 ) ;
            ret.Add( 0 ) ;
          }

          // 細径が偶数本
          if ( hosokeiHonsu % 2 == 0 ) {
            ret.Add( 0 ) ;

            // <中間>

            ret.Add( 0 ) ;
          }
          else {
            //
            ret.Add( 1 ) ;

            // <中間>

            ret.Add( 0 ) ;
          }
        }
        // 奇数本
        else {
          bool isLastHuto = true ;

          while ( already.Count < sum / 2 ) {
            already.Add( 1 ) ;
            ret.Add( 1 ) ;

            isLastHuto = false ;

            if ( ( already.Count < sum / 2 ) == false ) {
              break ;
            }

            already.Add( 0 ) ;
            ret.Add( 0 ) ;

            isLastHuto = true ;
          }

          // 直前が太径
          if ( isLastHuto == true ) {
            ret.Add( 1 ) ;
          }
          else {
            ret.Add( 0 ) ;
          }
        }

        // 終端側
        for ( int i = 0 ; i < already.Count ; ++i ) {
          ret.Add( already[ already.Count - ( i + 1 ) ] ) ;
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>配筋太径細径順序(四隅を含む) - 梁</summary>
    ///
    /// <history><p>2013/05/08 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Collections.Generic.IList<int> RebarOrder_Beam( int hutokeiHonsu, int hosokeiHonsu )
    {
      Collections.Generic.IList<int> ret = new Collections.Generic.List<int>() ;

      // 太径が1本より少ない
      if ( hutokeiHonsu < 1 ) {
        return ret ;
      }

      // 細径本数が太径本数以上
      if ( hosokeiHonsu >= hutokeiHonsu ) {
        return ret ;
      }

      // 太径のみ
      if ( hosokeiHonsu < 1 ) {
        //ret.Add(0);

        for ( int i = 0 ; i < hutokeiHonsu ; ++i ) {
          ret.Add( 0 ) ;
        }

        //ret.Add(0);

        return ret ;
      }
      else {
        // 総本数
        int sum = hutokeiHonsu + hosokeiHonsu ;

        // 配置済み鉄筋
        Collections.Generic.IList<int> already = new Collections.Generic.List<int>() ;

        // 始端
        already.Add( 0 ) ;
        ret.Add( 0 ) ;

        // 端部からの太径連続回数(端部自体は除く)
        double subtraction = hutokeiHonsu - hosokeiHonsu ;
        double division = subtraction / 2 ;
        double ceiling = System.Math.Ceiling( division ) ;
        int continuity = (int)ceiling - 1 ;

        for ( int i = 0 ; i < continuity ; ++i ) {
          already.Add( 0 ) ;
          ret.Add( 0 ) ;
        }

        // 偶数
        if ( subtraction % 2 == 0 ) {
          // 中央の1つ前まで
          while ( already.Count < sum / 2 - 1 ) {
            already.Add( 1 ) ;
            ret.Add( 1 ) ;

            if ( ( already.Count < sum / 2 - 1 ) == false ) {
              break ;
            }

            already.Add( 0 ) ;
            ret.Add( 0 ) ;
          }

          // 細径が偶数本
          if ( hosokeiHonsu % 2 == 0 ) {
            ret.Add( 0 ) ;

            // <中間>

            ret.Add( 0 ) ;
          }
          else {
            //
            ret.Add( 1 ) ;

            // <中間>

            ret.Add( 0 ) ;
          }
        }
        // 奇数本
        else {
          bool isLastHuto = true ;

          while ( already.Count < sum / 2 ) {
            already.Add( 1 ) ;
            ret.Add( 1 ) ;

            isLastHuto = false ;

            if ( ( already.Count < sum / 2 ) == false ) {
              break ;
            }

            already.Add( 0 ) ;
            ret.Add( 0 ) ;

            isLastHuto = true ;
          }

          // 直前が太径
          if ( isLastHuto == true ) {
            ret.Add( 1 ) ;
          }
          else {
            ret.Add( 0 ) ;
          }
        }

        // 終端側
        for ( int i = 0 ; i < already.Count ; ++i ) {
          ret.Add( already[ already.Count - ( i + 1 ) ] ) ;
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>配筋太径細径順序(四隅を含む) - 柱</summary>
    ///
    /// <param name="isOrthgonal">直交方向あり</param>
    ///
    /// <history><p>2013/05/08 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Collections.Generic.IList<int> RebarOrder_Column( int hutokeiHonsu, int hosokeiHonsu, bool isOrthgonal )
    {
      Collections.Generic.List<int> ret = new Collections.Generic.List<int>() ;

      // 太径が2本より少ない
      if ( hutokeiHonsu < 2 ) {
        return ret ;
      }

      // 細径本数が太径本数以上
      if ( hosokeiHonsu >= hutokeiHonsu ) {
        return ret ;
      }

      // 太径のみ
      if ( hosokeiHonsu < 1 ) {
        for ( int i = 0 ; i < hutokeiHonsu ; ++i ) {
          ret.Add( 0 ) ;
        }

        return ret ;
      }
      else {
        // 総本数
        int sum = hutokeiHonsu + hosokeiHonsu ;

        // 配置済み鉄筋
        Collections.Generic.List<int> already = new Collections.Generic.List<int>() ;

        // 始端
        already.Add( 0 ) ;
        ret.Add( 0 ) ;

        // 端部からの太径連続回数(端部自体は除く)
        double subtraction = hutokeiHonsu - hosokeiHonsu ;
        double division = subtraction / 2 ;
        double ceiling = System.Math.Ceiling( division ) ;
        int continuity = (int)ceiling - 1 ;

        for ( int i = 0 ; i < continuity ; ++i ) {
          already.Add( 0 ) ;
          ret.Add( 0 ) ;
        }

        // 偶数
        if ( subtraction % 2 == 0 ) {
          // 中央の1つ前まで
          while ( already.Count < sum / 2 - 1 ) {
            already.Add( 1 ) ;
            ret.Add( 1 ) ;

            if ( ( already.Count < sum / 2 - 1 ) == false ) {
              break ;
            }

            already.Add( 0 ) ;
            ret.Add( 0 ) ;
          }

          // 細径が偶数本
          if ( hosokeiHonsu % 2 == 0 ) {
            ret.Add( 0 ) ;

            // <中間>

            ret.Add( 0 ) ;
          }
          // 細径が奇数本
          else {
            // 中央の左または下
            ret.Add( 0 ) ;

            // <中間>

            // 中央の右または上
            ret.Add( 1 ) ;
          }
        }
        // 奇数本
        else {
          bool isLastHuto = true ;

          while ( already.Count < sum / 2 ) {
            already.Add( 1 ) ;
            ret.Add( 1 ) ;

            isLastHuto = false ;

            if ( ( already.Count < sum / 2 ) == false ) {
              break ;
            }

            already.Add( 0 ) ;
            ret.Add( 0 ) ;

            isLastHuto = true ;
          }

          // 直前が太径
          if ( isLastHuto == true ) {
            ret.Add( 1 ) ;
          }
          else {
            ret.Add( 0 ) ;
          }
        }

        // 終端側
        for ( int i = 0 ; i < already.Count ; ++i ) {
          ret.Add( already[ already.Count - ( i + 1 ) ] ) ;
        }
      }

      // 太径が4本以上かつ直交方向配筋あり
      if ( hutokeiHonsu >= 4 && isOrthgonal == true ) {
        // 端から2番目が細径の場合
        // 前
        if ( ret[ 1 ] == 1 ) {
          if ( ret[ 2 ] == 0 ) {
            ret[ 1 ] = 0 ;
            ret[ 2 ] = 1 ;
          }
        }

        // 後
        if ( ret[ ret.Count - 2 ] == 1 ) {
          if ( ret[ ret.Count - 3 ] == 0 ) {
            ret[ ret.Count - 2 ] = 0 ;
            ret[ ret.Count - 3 ] = 1 ;
          }
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>芯鉄筋XY振り分け</summary>
    ///
    /// <param name="xRebar">X段芯鉄筋</param>
    /// <param name="yRebar">Y段芯鉄筋</param>
    ///
    /// <summary>それぞれの方向にいくつあるか(※総数ではない = 四隅は除く)</summary>
    ///
    /// <history><p>2013/05/10 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public void CoreRebarXYDivision( int coreRebarNum, double x, double y, ref int xRebar, ref int yRebar )
    {
      // 4本以上偶数本
      if ( coreRebarNum >= 4 && coreRebarNum % 2 == 0 ) {
        // 四隅を除く(XYで重複しているため)
        int divisionNum = coreRebarNum - 4 ;

        // 4本
        if ( divisionNum == 0 ) {
          xRebar = divisionNum / 2 ;
          yRebar = divisionNum / 2 ;
        }
        // 4の倍数本
        else if ( divisionNum % 4 == 0 ) {
          // (n - 4) / 4 + 2
          xRebar = divisionNum / 4 + 2 ;
          yRebar = divisionNum / 4 + 2 ;
        }
        // 4の倍数本 + 2本
        else {
          // 長辺に2本多く配置

          xRebar = ( divisionNum - 2 ) / 4 + 2 ;
          yRebar = ( divisionNum - 2 ) / 4 + 2 ;

          if ( y > x ) {
            xRebar += 1 ;
          }
          else if ( y < x ) {
            yRebar += 1 ;
          }
          else {
            xRebar += 1 ;
          }
        }
      }
    }

    /// ================================================================================
    /// <summary>Y座標が小さい順に並び替え</summary>
    ///
    /// <history><p>2013/05/10 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Collections.Generic.IList<Revit.DB.XYZ> SortByYAry( Collections.Generic.IList<Revit.DB.XYZ> points )
    {
      Collections.Generic.IList<Revit.DB.XYZ> ret = new Collections.Generic.List<Revit.DB.XYZ>() ;

      Collections.Generic.IList<Revit.DB.XYZ> list = new Collections.Generic.List<Revit.DB.XYZ>() ;
      foreach ( Revit.DB.XYZ p in points ) {
        list.Add( p ) ;
      }

      while ( list.Count > 0 ) {
        Revit.DB.XYZ point = null ;

        foreach ( Revit.DB.XYZ p in list ) {
          if ( point == null ) {
            point = p ;
            continue ;
          }

          // 一番小さい点
          if ( point.Y > p.Y ) {
            point = p ;
          }
        }

        ret.Add( point ) ;
        list.Remove( point ) ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>X座標が小さい順に並び替え</summary>
    ///
    /// <history><p>2013/05/10 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Collections.Generic.IList<Revit.DB.XYZ> SortByXAry( Collections.Generic.IList<Revit.DB.XYZ> points )
    {
      Collections.Generic.IList<Revit.DB.XYZ> ret = new Collections.Generic.List<Revit.DB.XYZ>() ;

      Collections.Generic.IList<Revit.DB.XYZ> list = new Collections.Generic.List<Revit.DB.XYZ>() ;
      foreach ( Revit.DB.XYZ p in points ) {
        list.Add( p ) ;
      }

      while ( list.Count > 0 ) {
        Revit.DB.XYZ point = null ;

        foreach ( Revit.DB.XYZ p in list ) {
          if ( point == null ) {
            point = p ;
            continue ;
          }

          // 一番小さい点
          if ( point.X > p.X ) {
            point = p ;
          }
        }

        ret.Add( point ) ;
        list.Remove( point ) ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>指定点にi番目に近い点(Y座標)</summary>
    ///
    /// <history><p>2013/05/10 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public bool NearPointY( Revit.DB.XYZ basePoint, Collections.Generic.IList<Revit.DB.XYZ> points, int i, ref Revit.DB.XYZ nearBottom, ref Revit.DB.XYZ nearTop )
    {
      bool ret = true ;

      Collections.Generic.IList<Revit.DB.XYZ> sortYAry = SortByYAry( points ) ;

      if ( sortYAry.Count % 2 == 0 ) {
        for ( int n = 0 ; n < sortYAry.Count ; ++n ) {
          if ( n + i == sortYAry.Count ) {
            ret = false ;
            break ;
          }

          if ( basePoint.Y < sortYAry[ n ].Y ) {
            nearTop = sortYAry[ n + i ] ;
            nearBottom = sortYAry[ n - ( i + 1 ) ] ;

            break ;
          }
        }
      }
      else {
        double count = sortYAry.Count ;
        double halfCount = count / 2 ;
        int half = (int)System.Math.Ceiling( halfCount ) - 1 ;

        for ( int n = 0 ; n < sortYAry.Count ; ++n ) {
          if ( n + i + 1 == sortYAry.Count ) {
            ret = false ;
            break ;
          }

          if ( n == half ) {
            nearTop = sortYAry[ n + i + 1 ] ;
            nearBottom = sortYAry[ n - i - 1 ] ;

            break ;
          }
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>指定点にi番目に近い点(X座標)</summary>
    ///
    /// <history><p>2013/05/10 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public bool NearPointX( Revit.DB.XYZ basePoint, Collections.Generic.IList<Revit.DB.XYZ> points, int i, ref Revit.DB.XYZ nearLeft, ref Revit.DB.XYZ nearRight )
    {
      bool ret = true ;

      Collections.Generic.IList<Revit.DB.XYZ> sortXAry = SortByXAry( points ) ;

      if ( sortXAry.Count % 2 == 0 ) {
        for ( int n = 0 ; n < sortXAry.Count ; ++n ) {
          if ( n + i == sortXAry.Count ) {
            ret = false ;
            break ;
          }

          if ( basePoint.X < sortXAry[ n ].X ) {
            nearRight = sortXAry[ n + i ] ;
            nearLeft = sortXAry[ n - ( i + 1 ) ] ;

            break ;
          }
        }
      }
      else {
        double count = sortXAry.Count ;
        double halfCount = count / 2 ;
        int half = (int)System.Math.Ceiling( halfCount ) - 1 ;

        for ( int n = 0 ; n < sortXAry.Count ; ++n ) {
          if ( n + i + 1 == sortXAry.Count ) {
            ret = false ;
            break ;
          }

          if ( n == half ) {
            nearRight = sortXAry[ n + i + 1 ] ;
            nearLeft = sortXAry[ n - i - 1 ] ;

            break ;
          }
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>芯鉄筋の基点1段筋</summary>
    ///
    /// <history><p>2013/05/10 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public bool CoreRebarBasePoint( double span, Collections.Generic.IList<Revit.DB.XYZ> base1dankinPoints, ref Revit.DB.XYZ startPoint, ref Revit.DB.XYZ endPoint, bool vertical )
    {
      bool ret = true ;

      // 縦(X)方向
      if ( vertical == true ) {
        // 最大、最小
        Revit.DB.XYZ max = null ;
        Revit.DB.XYZ min = null ;

        foreach ( Revit.DB.XYZ p in base1dankinPoints ) {
          if ( max == null ) {
            max = p ;
            min = p ;

            continue ;
          }

          if ( max.Y < p.Y ) {
            max = p ;
          }

          if ( min.Y > p.Y ) {
            min = p ;
          }
        }

        // 距離
        double distance = _CmpGeometry.Distance2D( max, min ) ;
        double halfDist = distance / 2 ;

        // 中間
        Revit.DB.XYZ center = min + new Revit.DB.XYZ( 0, halfDist, 0 ) ;

        for ( int i = 0 ; i < base1dankinPoints.Count ; ++i ) {
          Revit.DB.XYZ p1 = null ;
          Revit.DB.XYZ p2 = null ;

          ret = NearPointY( center, base1dankinPoints, i, ref p1, ref p2 ) ;

          if ( ret == false ) {
            return ret ;
          }

          double d = _CmpGeometry.Distance2D( p1, p2 ) ;

          if ( span <= d ) {
            startPoint = p1 ;
            endPoint = p2 ;

            break ;
          }
        }
      }
      // 横(Y)方向
      else {
        // 最大、最小
        Revit.DB.XYZ max = null ;
        Revit.DB.XYZ min = null ;

        foreach ( Revit.DB.XYZ p in base1dankinPoints ) {
          if ( max == null ) {
            max = p ;
            min = p ;

            continue ;
          }

          if ( max.X < p.X ) {
            max = p ;
          }

          if ( min.X > p.X ) {
            min = p ;
          }
        }

        // 距離
        double distance = _CmpGeometry.Distance2D( max, min ) ;
        double halfDist = distance / 2 ;

        // 中間
        Revit.DB.XYZ center = min + new Revit.DB.XYZ( halfDist, 0, 0 ) ;

        for ( int i = 0 ; i < base1dankinPoints.Count ; ++i ) {
          Revit.DB.XYZ p1 = null ;
          Revit.DB.XYZ p2 = null ;

          ret = NearPointX( center, base1dankinPoints, i, ref p1, ref p2 ) ;

          if ( ret == false ) {
            return ret ;
          }

          double d = _CmpGeometry.Distance2D( p1, p2 ) ;

          if ( span <= d ) {
            startPoint = p1 ;
            endPoint = p2 ;

            break ;
          }
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>位置順と径</summary>
    ///
    /// <history>2013/07/12 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public Collections.Generic.IList<Collections.Generic.IList<double>> OrderAndDiameter( Collections.Generic.IList<Revit.DB.XYZ> hutoPoints, Collections.Generic.IList<Revit.DB.XYZ> hosoPoints )
    {
      Collections.Generic.IList<Collections.Generic.IList<double>> ret = new Collections.Generic.List<Collections.Generic.IList<double>>() ;

      Collections.Generic.IList<Revit.DB.XYZ> hutos = new Collections.Generic.List<Revit.DB.XYZ>() ;
      Collections.Generic.IList<Revit.DB.XYZ> hosos = new Collections.Generic.List<Revit.DB.XYZ>() ;

      foreach ( Revit.DB.XYZ p in hutoPoints ) {
        hutos.Add( p ) ;
      }

      foreach ( Revit.DB.XYZ p in hosoPoints ) {
        hosos.Add( p ) ;
      }

      while ( hutos.Count > 0 || hosos.Count > 0 ) {
        Revit.DB.XYZ minX = null ;

        bool isHuto = false ;
        bool isHoso = false ;

        double diaType = 0 ;

        if ( hutos.Count > 0 ) {
          minX = hutos[ 0 ] ;

          isHuto = true ;
          isHoso = false ;
        }
        else if ( hosos.Count > 0 ) {
          minX = hosos[ 0 ] ;

          isHuto = false ;
          isHoso = true ;
        }

        foreach ( Revit.DB.XYZ p in hutos ) {
          if ( minX.X > p.X ) {
            minX = p ;

            isHuto = true ;
            isHoso = false ;
          }
        }

        foreach ( Revit.DB.XYZ p in hosos ) {
          if ( minX.X > p.X ) {
            minX = p ;

            isHuto = false ;
            isHoso = true ;
          }
        }

        if ( isHuto == true ) {
          hutos.Remove( minX ) ;

          diaType = 0 ;
        }
        else if ( isHoso == true ) {
          hosos.Remove( minX ) ;

          diaType = 1 ;
        }

        Collections.Generic.IList<double> pntAndDiameter = new Collections.Generic.List<double>() ;
        pntAndDiameter.Add( minX.X ) ;
        pntAndDiameter.Add( diaType ) ;

        ret.Add( pntAndDiameter ) ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>前後の位置順と径</summary>
    ///
    /// <history>2013/07/12 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public void StirrupBeforeAfterPoint( Revit.DB.XYZ stirrupPoint, Collections.Generic.IList<Collections.Generic.IList<double>> orderAndDiameter, ref int beforeNum, ref int afterNum, Revit.DB.XYZ wideCenter )
    {
      for ( int i = 0 ; i < orderAndDiameter.Count ; ++i ) {
        Collections.Generic.IList<double> beforePointAndDiameter = orderAndDiameter[ i ] ;
        Collections.Generic.IList<double> afterPointAndDiameter = orderAndDiameter[ i + 1 ] ;

        double beforeX = beforePointAndDiameter[ 0 ] ;
        double afterX = afterPointAndDiameter[ 0 ] ;

        // 中心以前
        if ( _CmpGeometry.ToHalfAdjust( stirrupPoint.X, -9 ) <= _CmpGeometry.ToHalfAdjust( wideCenter.X, -9 ) ) {
          if ( stirrupPoint.X > beforeX && stirrupPoint.X <= afterX ) {
            beforeNum = i ;
            afterNum = i + 1 ;

            break ;
          }
        }
        // 中心より後
        else {
          if ( stirrupPoint.X >= beforeX && stirrupPoint.X < afterX ) {
            beforeNum = i ;
            afterNum = i + 1 ;

            break ;
          }
        }

        //if (_CmpParameters.ToHalfAdjust(stirrupPoint.X, -5) == _CmpParameters.ToHalfAdjust(beforeX, -5))
        //{
        //  beforeNum = i;
        //  afterNum = i + 1;

        //  break;
        //}
        //else if (stirrupPoint.X > beforeX && stirrupPoint.X<afterX)
        //{
        //}
        //else if (_CmpParameters.ToHalfAdjust(stirrupPoint.X, -5) == _CmpParameters.ToHalfAdjust(afterX, -5))
        //{
        //}

        //if (stirrupPoint.X >= beforeX && stirrupPoint.X <= afterX)
        //{
        //  beforeNum = i;
        //  afterNum = i + 1;

        //  break;
        //}
      }
    }

    /// ================================================================================
    /// <summary>肋筋を配置する細径位置</summary>
    ///
    /// <history>2013/07/12 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public Collections.Generic.IList<Revit.DB.XYZ> StirrupSetHoso( int overStirrupNum, Collections.Generic.IList<Revit.DB.XYZ> hosoPoints, Revit.DB.XYZ wideCenter )
    {
      Collections.Generic.IList<Revit.DB.XYZ> ret = new Collections.Generic.List<Revit.DB.XYZ>() ;

      for ( int i = 0 ; i < overStirrupNum ; ++i ) {
        Revit.DB.XYZ nearCenter = null ;

        foreach ( Revit.DB.XYZ point in hosoPoints ) {
          if ( ret.Contains( point ) ) {
            continue ;
          }

          if ( nearCenter == null ) {
            nearCenter = point ;
            continue ;
          }

          // 中心に近い方
          double distanceX = System.Math.Abs( nearCenter.X - wideCenter.X ) ;
          double distanceX2 = System.Math.Abs( point.X - wideCenter.X ) ;

          if ( distanceX > distanceX2 ) {
            nearCenter = point ;
          }
          // 同じ場合は左側
          else if ( distanceX == distanceX2 ) {
            if ( nearCenter.X > point.X ) {
              nearCenter = point ;
            }
          }
        }

        ret.Add( nearCenter ) ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>少数筋・肋筋位置の調整</summary>
    ///
    /// <history>2013/08/05 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public void MoveMinorAndStirrup( Collections.Generic.IList<Revit.DB.XYZ> major, ref Collections.Generic.IList<Revit.DB.XYZ> minor, ref Revit.DB.XYZ stirrup, Revit.DB.XYZ wideCenter, ref Collections.Generic.IList<int> usedMajorNumber, ref Collections.Generic.IList<int> usedMinorNumber, int stirrupNumber )
    {
      // 中心より前
      Collections.Generic.IList<Revit.DB.XYZ> beforeCenter = new Collections.Generic.List<Revit.DB.XYZ>() ;
      // 中心より後
      Collections.Generic.IList<Revit.DB.XYZ> afterCenter = new Collections.Generic.List<Revit.DB.XYZ>() ;

      minor = SortByXAry( minor ) ;

      foreach ( Revit.DB.XYZ point in minor ) {
        if ( _CmpGeometry.ToHalfAdjust( point.X, -9 ) <= _CmpGeometry.ToHalfAdjust( wideCenter.X, -9 ) ) {
          beforeCenter.Add( point ) ;
        }
        else {
          afterCenter.Add( point ) ;
        }
      }

      int majorNum = 0 ;
      int minorNum = 0 ;
      // 上下筋が同じ位置にあるか
      bool isNearMajority = IsStirrupNearXPoint( stirrup, major, ref majorNum, usedMajorNumber ) ;
      bool isNearMinority = IsStirrupNearXPoint( stirrup, minor, ref minorNum, usedMinorNumber ) ;

      // ともにある場合
      if ( isNearMajority == true && isNearMinority == true && usedMajorNumber.Contains( majorNum ) == false && usedMinorNumber.Contains( minorNum ) == false && usedMinorNumber.Contains( minorNum ) ) {
        // そのまま
        usedMajorNumber.Add( majorNum ) ;
        usedMinorNumber.Add( minorNum ) ;

        return ;
      }
      else {
        // 中心から前
        if ( _CmpGeometry.ToHalfAdjust( stirrup.X, -9 ) <= _CmpGeometry.ToHalfAdjust( wideCenter.X, -9 ) ) {
          // 肋筋位置と中央より小さい多数派の最大の位置
          majorNum = 0 ;
          Revit.DB.XYZ minMax = XPointNearMax_Major_2( stirrup, major, ref majorNum, usedMajorNumber, stirrupNumber, minor, usedMinorNumber, wideCenter ) ;

          usedMajorNumber.Add( majorNum ) ;

          stirrup = new Revit.DB.XYZ( minMax.X, stirrup.Y, stirrup.Z ) ;

          // 肋筋位置と中央より大きい少数派の最小の位置
          minorNum = 0 ;
          Revit.DB.XYZ maxMin = XPointNearMin_Minor_2( stirrup, minor, ref minorNum, usedMinorNumber ) ;

          usedMinorNumber.Add( minorNum ) ;

          minor[ minorNum ] = new Revit.DB.XYZ( stirrup.X, maxMin.Y, maxMin.Z ) ;

          // 肋筋が奇数本で少数筋が偶数本の場合、
          // 少数筋中央左側は肋筋位置に移動し、
          // 少数筋中央右側は対象位置に移動するので、
          // 少数筋中央右側も使用済みにする。
          if ( stirrupNumber % 2 != 0 && minor.Count % 2 == 0 ) {
            if ( minorNum == minor.Count / 2 - 1 ) {
              usedMinorNumber.Add( minorNum + 1 ) ;

              minor[ minorNum + 1 ] = _CmpGeometry.ReversePoint_X( wideCenter, minor[ minorNum ] ) ;
            }
          }

          // 逆
          //int gyaku = minor.Count - 1 - minorNum;

          //if (gyaku != minorNum)
          //{
          //  if (!usedMinorNumber.Contains(gyaku))
          //  {
          //    minor[gyaku] = _CmpGeometry.ReversePoint_X(wideCenter, minor[gyaku]);
          //  }
          //}
        }
        // 中心より後
        else {
          // 肋筋位置と中間より大きい最小の位置
          majorNum = 0 ;
          Revit.DB.XYZ minMax = XPointNearMin_Major_2( stirrup, major, ref majorNum, usedMajorNumber, stirrupNumber, minor, usedMinorNumber, wideCenter ) ;

          usedMajorNumber.Add( majorNum ) ;

          stirrup = new Revit.DB.XYZ( minMax.X, stirrup.Y, stirrup.Z ) ;

          // 肋筋位置と中央より小さい少数派の最大の位置
          minorNum = 0 ;
          Revit.DB.XYZ maxMin = XPointNearMax_Minor_2( stirrup, minor, ref minorNum, usedMinorNumber ) ;

          usedMinorNumber.Add( minorNum ) ;

          minor[ minorNum ] = new Revit.DB.XYZ( stirrup.X, maxMin.Y, maxMin.Z ) ;

          // 逆
          //int gyaku = minor.Count - 1 - minorNum;

          //if (gyaku != minorNum)
          //{
          //  if (!usedMinorNumber.Contains(gyaku))
          //  {
          //    minor[gyaku] = _CmpGeometry.ReversePoint_X(wideCenter, minor[gyaku]);
          //  }
          //}
        }
      }
    }

    /// ================================================================================
    /// <summary>上下筋・肋筋位置の調整</summary>
    ///
    /// <history>2013/05/13 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public void MoveToMajorityRebar( Collections.Generic.IList<Revit.DB.XYZ> majority, ref Collections.Generic.IList<Revit.DB.XYZ> minority, ref Collections.Generic.IList<Revit.DB.XYZ> stirrupXPoints, double diameter, Revit.DB.XYZ wideCenter )
    {
      Collections.Generic.IList<Revit.DB.XYZ> _Minority = new Collections.Generic.List<Revit.DB.XYZ>() ;
      Collections.Generic.IList<Revit.DB.XYZ> _Stirrups = new Collections.Generic.List<Revit.DB.XYZ>() ;

      // 中心より前
      Collections.Generic.IList<Revit.DB.XYZ> beforeCenter = new Collections.Generic.List<Revit.DB.XYZ>() ;
      // 中心より後
      Collections.Generic.IList<Revit.DB.XYZ> afterCenter = new Collections.Generic.List<Revit.DB.XYZ>() ;

      foreach ( Revit.DB.XYZ point in minority ) {
        if ( _CmpGeometry.ToHalfAdjust( point.X, -9 ) <= _CmpGeometry.ToHalfAdjust( wideCenter.X, -9 ) ) {
          beforeCenter.Add( point ) ;
        }
        else {
          afterCenter.Add( point ) ;
        }
      }

      // 肋筋位置を調整してから少数派を調整
      for ( int i = 0 ; i < stirrupXPoints.Count ; ++i ) {
        #region 肋筋

        Revit.DB.XYZ point = stirrupXPoints[ i ] ;

        int usedMajor = 0 ;
        int usedMinor = 0 ;

        // 上下筋とも同じ位置にあるか
        bool isNearMajority = IsHaveNearXPoint( point, majority, ref usedMajor ) ;
        bool isNearMinority = IsHaveNearXPoint( point, minority, ref usedMinor ) ;

        // ある場合
        if ( isNearMajority == true && isNearMinority == true ) {
          continue ;
        }

        // ない場合
        // 中心から前
        if ( _CmpGeometry.ToHalfAdjust( point.X, -9 ) <= _CmpGeometry.ToHalfAdjust( wideCenter.X, -9 ) ) {
          int num = 0 ;
          Revit.DB.XYZ nearMajor = XPointNearMax( point, majority, ref num ) ;

          stirrupXPoints[ i ] = new Revit.DB.XYZ( nearMajor.X, point.Y, point.Z ) ;
        }
        // 中心より後
        else {
          int num = 0 ;
          Revit.DB.XYZ nearMajor = XPointNearMin( point, majority, ref num ) ;

          stirrupXPoints[ i ] = new Revit.DB.XYZ( nearMajor.X, point.Y, point.Z ) ;
        }

        #endregion

        #region 少数派

        point = stirrupXPoints[ i ] ;
        isNearMinority = IsHaveNearXPoint( point, minority, ref usedMinor ) ;

        // 肋筋と同じ位置にある場合
        if ( isNearMinority == true ) {
          continue ;
        }

        // 中心より前
        if ( _CmpGeometry.ToHalfAdjust( point.X, -9 ) <= _CmpGeometry.ToHalfAdjust( wideCenter.X, -9 ) ) {
          int num = 0 ;
          Revit.DB.XYZ nearMinor = XPointNearMin( point, minority, ref num ) ;

          minority[ num ] = new Revit.DB.XYZ( point.X, nearMinor.Y, nearMinor.Z ) ;

          // 逆位置
          // 奇数本の中心は除く
          if ( minority.Count - 1 - num != num ) {
            Revit.DB.XYZ reversePnt = _CmpGeometry.ReversePoint_X( wideCenter, minority[ num ] ) ;
            minority[ minority.Count - 1 - num ] = reversePnt ;
          }

          // より前の鉄筋も移動
          for ( int j = 0 ; j < minority.Count ; ++j ) {
            int numBefore = num - j - 1 ;

            if ( numBefore < 0 ) {
              break ;
            }

            int sute = 0 ;
            nearMinor = XPointNearMax( minority[ numBefore ], majority, ref sute ) ;
            minority[ numBefore ] = new Revit.DB.XYZ( nearMinor.X, minority[ numBefore ].Y, minority[ numBefore ].Z ) ;

            // 逆位置
            int numReverse = minority.Count - numBefore - 1 ;
            nearMinor = XPointNearMin( minority[ numReverse ], majority, ref sute ) ;
            minority[ numReverse ] = new Revit.DB.XYZ( nearMinor.X, minority[ numReverse ].Y, minority[ numReverse ].Z ) ;
          }
        }
        // 中心より後
        else {
          int num = 0 ;
          Revit.DB.XYZ nearMinor = XPointNearMax( point, minority, ref num ) ;

          minority[ num ] = new Revit.DB.XYZ( point.X, nearMinor.Y, nearMinor.Z ) ;

          // 逆位置
          if ( minority.Count - 1 - num != num ) {
            Revit.DB.XYZ reversePnt = _CmpGeometry.ReversePoint_X( wideCenter, minority[ num ] ) ;
            minority[ minority.Count - 1 - num ] = reversePnt ;
          }

          // より後の鉄筋も移動
          for ( int j = 0 ; j < minority.Count ; ++j ) {
            int numAfter = num - j + 1 ;

            if ( numAfter >= minority.Count ) {
              break ;
            }

            int sute = 0 ;
            nearMinor = XPointNearMin( minority[ numAfter ], majority, ref sute ) ;
            minority[ numAfter ] = new Revit.DB.XYZ( nearMinor.X, minority[ numAfter ].Y, minority[ numAfter ].Z ) ;

            // 逆位置
            int numReberse = minority.Count - numAfter - 1 ;
            nearMinor = XPointNearMax( minority[ numReberse ], majority, ref sute ) ;
            minority[ numReberse ] = new Revit.DB.XYZ( nearMinor.X, minority[ numReberse ].Y, minority[ numReberse ].Z ) ;
          }
        }

        #endregion
      }

      // 肋筋を鉄筋幅の半分ずらす
      for ( int i = 0 ; i < stirrupXPoints.Count ; ++i ) {
        Revit.DB.XYZ point = stirrupXPoints[ i ] ;
        double half = diameter / 2 ;

        // 中心より前
        if ( _CmpGeometry.ToHalfAdjust( point.X, -9 ) <= _CmpGeometry.ToHalfAdjust( wideCenter.X, -9 ) ) {
          stirrupXPoints[ i ] = new Revit.DB.XYZ( point.X - half, point.Y, point.Z ) ;
        }
        // 中心より後
        else {
          stirrupXPoints[ i ] = new Revit.DB.XYZ( point.X + half, point.Y, point.Z ) ;
        }
      }
    }

    /// ================================================================================
    /// <summary>上下筋・肋筋位置の調整 - テスト</summary>
    ///
    /// <history>2013/08/05 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public void MoveToMajorityRebar_Test( Collections.Generic.IList<Revit.DB.XYZ> majority_Huto, Collections.Generic.IList<Revit.DB.XYZ> majority_Hoso, Collections.Generic.IList<Revit.DB.XYZ> majority, ref Collections.Generic.IList<Revit.DB.XYZ> minority, ref Collections.Generic.IList<Revit.DB.XYZ> stirrupXPoints, double diameter, Revit.DB.XYZ wideCenter, Revit.DB.XYZ left, Revit.DB.XYZ right )
    {
      // 使用済み多数派
      Collections.Generic.IList<int> usedMajorNumber = new Collections.Generic.List<int>() ;
      // 使用済み少数派
      Collections.Generic.IList<int> usedMinorNumber = new Collections.Generic.List<int>() ;

      // 肋筋位置
      for ( int i = 0 ; i < stirrupXPoints.Count ; ++i ) {
        // 肋筋
        Revit.DB.XYZ stirrupPoint = stirrupXPoints[ i ] ;

        int usedMajorityNum = 0 ;
        int usedMinorityNum = 0 ;

        // 上下筋とも同じ位置にあるか
        bool isNearMajority = IsHaveNearXPoint( stirrupPoint, majority, ref usedMajorityNum ) ;
        bool isNearMinority = IsHaveNearXPoint( stirrupPoint, minority, ref usedMinorityNum ) ;

        // ある場合
        if ( isNearMajority == true && isNearMinority == true ) {
          usedMajorNumber.Add( usedMajorityNum ) ;
          usedMinorNumber.Add( usedMinorityNum ) ;

          continue ;
        }
        else {
          MoveMinorAndStirrup( majority, ref minority, ref stirrupPoint, wideCenter, ref usedMajorNumber, ref usedMinorNumber, stirrupXPoints.Count ) ;
        }

        stirrupXPoints[ i ] = stirrupPoint ;
      }

      // 肋筋を鉄筋幅の半分ずらす
      for ( int i = 0 ; i < stirrupXPoints.Count ; ++i ) {
        Revit.DB.XYZ point = stirrupXPoints[ i ] ;
        double half = diameter / 2 ;

        // 中心より前
        if ( _CmpGeometry.ToHalfAdjust( point.X, -9 ) <= _CmpGeometry.ToHalfAdjust( wideCenter.X, -9 ) ) {
          stirrupXPoints[ i ] = new Revit.DB.XYZ( point.X - half, point.Y, point.Z ) ;
        }
        // 中心より後
        else {
          stirrupXPoints[ i ] = new Revit.DB.XYZ( point.X + half, point.Y, point.Z ) ;
        }
      }

      // 肋筋取付き少数派
      for ( int i = 0 ; i < usedMinorNumber.Count ; ++i ) {
        // 少数派の端から2番目
        if ( usedMinorNumber[ i ] == 0 || usedMinorNumber[ i ] == minority.Count - 1 ) {
          // 逆側を修正
          int gyaku = minority.Count - 1 ;

          if ( gyaku != i ) {
            // 未使用
            if ( ! usedMinorNumber.Contains( gyaku ) ) {
              minority[ gyaku ] = _CmpGeometry.ReversePoint_X( wideCenter, minority[ i ] ) ;
            }
          }

          continue ;
        }

        Revit.DB.XYZ point = minority[ usedMinorNumber[ i ] ] ;
        int num = 0 ;

        // 中央から前
        if ( _CmpGeometry.ToHalfAdjust( point.X, -9 ) <= _CmpGeometry.ToHalfAdjust( wideCenter.X, -9 ) ) {
          // 肋筋の取付いている前側の少数筋
          Revit.DB.XYZ mae = null ;

          if ( i == 0 ) {
            mae = left ;

            num = usedMinorNumber[ i ] ;
          }
          else {
            //// 隣同士
            //if (usedMinorNumber[i] - usedMinorNumber[i - 1] == 1)
            //{
            //  continue;
            //}

            num = usedMinorNumber[ i ] - usedMinorNumber[ i - 1 ] ;

            mae = minority[ usedMinorNumber[ i - 1 ] ] ;
          }

          double distance = _CmpGeometry.Distance2D( new Revit.DB.XYZ( mae.X, point.Y, point.Z ), point ) ;

          double pitch = distance / ( num + 1 ) ;

          int start = 0 ;
          if ( i != 0 ) {
            start = usedMinorNumber[ i - 1 ] + 1 ;
          }

          int k = 0 ;

          // 再均等配置
          for ( int j = start ; j < usedMinorNumber[ i ] ; ++j ) {
            k += 1 ;
            minority[ j ] = new Revit.DB.XYZ( mae.X + pitch * k, minority[ j ].Y, minority[ j ].Z ) ;

            // 逆側対称
            int gyaku = minority.Count - j - 1 ;

            if ( j != gyaku ) {
              // 未使用
              if ( ! usedMinorNumber.Contains( gyaku ) ) {
                minority[ gyaku ] = _CmpGeometry.ReversePoint_X( wideCenter, minority[ j ] ) ;
              }
            }
          }
        }
        else {
          Revit.DB.XYZ ato = null ;

          if ( i == usedMinorNumber.Count - 1 ) {
            ato = right ;

            num = minority.Count - usedMinorNumber[ i ] ;
          }
          else {
            //// 隣同士
            //if (usedMinorNumber[i + 1] - usedMinorNumber[i] == 1)
            //{
            //  continue;
            //}

            num = usedMinorNumber[ i + 1 ] - usedMinorNumber[ i ] ;
            ato = minority[ usedMinorNumber[ i + 1 ] ] ;
          }

          double distance = _CmpGeometry.Distance2D( new Revit.DB.XYZ( ato.X, point.Y, point.Z ), point ) ;

          double pitch = distance / ( num ) ;

          int end = minority.Count ;
          if ( i != usedMinorNumber.Count - 1 ) {
            end = usedMinorNumber[ i + 1 ] ;
          }

          int k = 0 ;

          // 均等配置
          for ( int j = usedMinorNumber[ i ] + 1 ; j < end ; ++j ) {
            k += 1 ;
            minority[ j ] = new Revit.DB.XYZ( point.X + pitch * k, minority[ j ].Y, minority[ j ].Z ) ;

            // 逆側対称
            int gyaku = minority.Count - j - 1 ;

            if ( j != gyaku ) {
              // 未使用
              if ( ! usedMinorNumber.Contains( gyaku ) ) {
                minority[ gyaku ] = _CmpGeometry.ReversePoint_X( wideCenter, minority[ j ] ) ;
              }
            }
          }
        }
      }

      // 肋筋の取り付いた主筋間ごとの均等割りのやり直し
      //Revit.DB.XYZ before = left;

      //Collections.Generic.IList<Revit.DB.XYZ> points = new Collections.Generic.List<Revit.DB.XYZ>();
      //Collections.Generic.IList<int> nums = new Collections.Generic.List<int>();

      //for (int i = 0; i < minority.Count; ++i)
      //{
      //  Revit.DB.XYZ point = minority[i];
      //
      //  if (usedMinorNumber.Contains(i))
      //  {
      //    // beforeとpointでpointsを均等割り
      //    if (points.Count > 0)
      //    {
      //      double distance = _CmpGeometry.Distance2D(new Revit.DB.XYZ(before.X, point.Y, point.Z), point);
      //
      //      double pitch = distance / (points.Count + 1);
      //
      //      for (int j = 0; j < nums.Count; ++j)
      //      {
      //        minority[nums[j]] = new Revit.DB.XYZ(before.X + pitch * (j + 1), point.Y, point.Z);
      //      }
      //    }
      //
      //    // beforeをpointに変更
      //    // pointsをクリア
      //    before = point;
      //    points.Clear();
      //    nums.Clear();
      //  }
      //  else
      //  {
      //    points.Add(point);
      //    nums.Add(i);
      //  }
      //}
      //
      //// beforeとrightでpointsを均等割り
      //if (points.Count > 0)
      //{
      //  double distance = _CmpGeometry.Distance2D(new Revit.DB.XYZ(before.X, points[0].Y, points[0].Z), new Revit.DB.XYZ(right.X, points[0].Y, points[0].Z));
      //
      //  double pitch = distance / (points.Count + 1);
      //
      //  for (int i = 0; i < nums.Count; ++i)
      //  {
      //    minority[nums[i]] = new Revit.DB.XYZ(before.X + pitch * (i + 1), points[0].Y, points[0].Z);
      //  }
      //}
    }

    /// ================================================================================
    /// <summary>上下筋・肋筋位置の調整 - テスト2</summary>
    ///
    /// <history>2013/12/25 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public void MoveToMajorityRebar_Test2( Collections.Generic.IList<Revit.DB.XYZ> majority_Huto, Collections.Generic.IList<Revit.DB.XYZ> majority_Hoso, Collections.Generic.IList<Revit.DB.XYZ> majority, ref Collections.Generic.IList<Revit.DB.XYZ> minority, ref Collections.Generic.IList<Revit.DB.XYZ> stirrupXPoints, double diameter, Revit.DB.XYZ wideCenter, Revit.DB.XYZ left, Revit.DB.XYZ right )
    {
      // 使用済み多数派
      Collections.Generic.IList<int> usedMajorNumber = new Collections.Generic.List<int>() ;
      // 使用済み少数派
      Collections.Generic.IList<int> usedMinorNumber = new Collections.Generic.List<int>() ;

      // 元の等間隔位置
      Collections.Generic.IList<Revit.DB.XYZ> defMinorPnts = new Collections.Generic.List<Revit.DB.XYZ>() ;
      foreach ( Revit.DB.XYZ p in minority ) {
        defMinorPnts.Add( p ) ;
      }

      #region 肋筋

      // 肋筋位置

      stirrupXPoints = SortXByCenter( wideCenter, stirrupXPoints ) ;

      for ( int i = 0 ; i < stirrupXPoints.Count ; ++i ) {
        // 肋筋
        Revit.DB.XYZ stirrupPoint = stirrupXPoints[ i ] ;

        int usedMajorityNum = 0 ;
        int usedMinorityNum = 0 ;

        // 上下筋とも同じ位置にあるか
        bool isNearMajority = IsHaveNearXPoint( stirrupPoint, majority, ref usedMajorityNum ) ;
        bool isNearMinority = IsHaveNearXPoint( stirrupPoint, minority, ref usedMinorityNum ) ;

        // ある場合
        if ( isNearMajority == true && isNearMinority == true && usedMajorNumber.Contains( usedMajorityNum ) == false && usedMinorNumber.Contains( usedMinorityNum ) == false ) {
          usedMajorNumber.Add( usedMajorityNum ) ;
          usedMinorNumber.Add( usedMinorityNum ) ;

          continue ;
        }
        else {
          MoveMinorAndStirrup( majority, ref minority, ref stirrupPoint, wideCenter, ref usedMajorNumber, ref usedMinorNumber, stirrupXPoints.Count ) ;
        }

        stirrupXPoints[ i ] = stirrupPoint ;
      }

      // 肋筋を鉄筋幅の半分ずらす
      for ( int i = 0 ; i < stirrupXPoints.Count ; ++i ) {
        Revit.DB.XYZ point = stirrupXPoints[ i ] ;
        double half = diameter / 2 ;

        // 中心より前
        if ( _CmpGeometry.ToHalfAdjust( point.X, -9 ) <= _CmpGeometry.ToHalfAdjust( wideCenter.X, -9 ) ) {
          stirrupXPoints[ i ] = new Revit.DB.XYZ( point.X - half, point.Y, point.Z ) ;
        }
        // 中心より後
        else {
          stirrupXPoints[ i ] = new Revit.DB.XYZ( point.X + half, point.Y, point.Z ) ;
        }
      }

      #endregion

      //
      Collections.Generic.IList<int> gyakuNums = new Collections.Generic.List<int>() ;

      // 少数派
      // 肋筋取り付き主筋の逆側を移動
      for ( int i = 0 ; i < minority.Count ; ++i ) {
        // 後半は不要
        if ( minority.Count % 2 == 0 ) {
          if ( i == minority.Count / 2 ) {
            break ;
          }
        }
        else {
          if ( i == ( minority.Count - 1 ) / 2 ) {
            break ;
          }
        }

        // 左側
        Revit.DB.XYZ p1 = minority[ i ] ;

        // 右側
        Revit.DB.XYZ p2 = minority[ minority.Count - 1 - i ] ;

        // 左側に肋筋が付いている
        if ( usedMinorNumber.Contains( i ) ) {
          // 右側が元の等間隔位置
          if ( IsContainPoint( p2, defMinorPnts ) && ! usedMinorNumber.Contains( minority.Count - 1 - i ) ) {
            // 移動
            minority[ minority.Count - 1 - i ] = _CmpGeometry.ReversePoint_X( wideCenter, p1 ) ;

            gyakuNums.Add( minority.Count - 1 - i ) ;
          }
        }
        // 右側に肋筋が付いている
        else if ( usedMinorNumber.Contains( minority.Count - 1 - i ) ) {
          // 左側が元の等間隔位置
          if ( IsContainPoint( p1, defMinorPnts ) && ! usedMinorNumber.Contains( i ) ) {
            // 移動
            minority[ i ] = _CmpGeometry.ReversePoint_X( wideCenter, p2 ) ;

            gyakuNums.Add( i ) ;
          }
        }
      }

      // 肋筋取り付きと逆位置の統合(usedMinorNumber + gyakuNums)
      Collections.Generic.List<int> allUsedMinor = new Collections.Generic.List<int>() ;
      foreach ( int num in usedMinorNumber ) {
        allUsedMinor.Add( num ) ;
      }

      foreach ( int num in gyakuNums ) {
        allUsedMinor.Add( num ) ;
      }

      allUsedMinor.Sort() ;

      if ( allUsedMinor.Count > 1 ) {
        if ( allUsedMinor[ 0 ] > allUsedMinor[ 1 ] ) {
          allUsedMinor.Reverse() ;
        }
      }

      // 未使用の再等間隔配置

      Revit.DB.XYZ saki = null ;
      int sakiNum = -1 ;

      // 最大回数
      int forMax = 0 ;
      if ( allUsedMinor.Count % 2 == 0 ) {
        forMax = allUsedMinor.Count / 2 ;
      }
      else {
        forMax = ( allUsedMinor.Count + 1 ) / 2 ;
      }

      for ( int i = 0 ; i < forMax ; ++i ) {
        //// 後半は不要
        //if (minority.Count % 2 == 0)
        //{
        //  if (i == minority.Count / 2 + 1)
        //  {
        //    break;
        //  }
        //}
        //else
        //{
        //  if (i == (minority.Count + 1) / 2)
        //  {
        //    break;
        //  }
        //}

        // 中心までで行い、逆位置を移動
        // 使用済みはいどうしない

        Revit.DB.XYZ p = minority[ allUsedMinor[ i ] ] ;

        // 前
        if ( i == 0 ) {
          saki = left ;
        }
        else {
          saki = minority[ allUsedMinor[ i - 1 ] ] ;

          sakiNum = allUsedMinor[ i - 1 ] ;
        }

        // 前と隣でない
        if ( allUsedMinor[ i ] - sakiNum > 1 ) {
          double distance = System.Math.Abs( p.X - saki.X ) ;
          //double distance = _CmpGeometry.Distance2D(saki, p);
          double pitch = distance / ( allUsedMinor[ i ] - sakiNum ) ;

          int k = 0 ;

          for ( int j = sakiNum ; j < allUsedMinor[ i ] - 1 ; ++j ) {
            k += 1 ;

            minority[ sakiNum + k ] = new Revit.DB.XYZ( saki.X + pitch * k, minority[ sakiNum + k ].Y, minority[ sakiNum + k ].Z ) ;

            if ( ! allUsedMinor.Contains( minority.Count - 1 - ( sakiNum + k ) ) ) {
              minority[ minority.Count - 1 - ( sakiNum + k ) ] = _CmpGeometry.ReversePoint_X( wideCenter, minority[ sakiNum + k ] ) ;
            }
          }
        }
      }
    }

    /// ================================================================================
    /// <summary>上下筋・肋筋位置の調整</summary>
    ///
    /// <history>2014/01/14 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public void MoveToMajorityRebar( Collections.Generic.IList<Revit.DB.XYZ> majority_Huto, Collections.Generic.IList<Revit.DB.XYZ> majority_Hoso, Collections.Generic.IList<Revit.DB.XYZ> majority, ref Collections.Generic.IList<Revit.DB.XYZ> minority, ref Collections.Generic.IList<Revit.DB.XYZ> stirrupXPoints, double diameter, Revit.DB.XYZ wideCenter, Revit.DB.XYZ left, Revit.DB.XYZ right )
    {
      // 使用済み多数派鉄筋位置
      Collections.Generic.IList<Revit.DB.XYZ> usedMajor = new Collections.Generic.List<Revit.DB.XYZ>() ;
      // 使用済み少数派鉄筋位置
      Collections.Generic.IList<Revit.DB.XYZ> usedMinor = new Collections.Generic.List<Revit.DB.XYZ>() ;

      minority = SortByXAry( minority ) ;

      // 少数派鉄筋の位置合わせ
      for ( int i = 0 ; i < minority.Count ; ++i ) {
        Revit.DB.XYZ minorPnt = minority[ i ] ;

        // 同じ位置に多数派があるか
        int index = 0 ;
        bool isHave = IsHaveNearXPoint( minorPnt, majority, ref index ) ;

        if ( isHave == true ) {
          continue ;
        }
        else {
          // 1つ外の多数派鉄筋位置へ
          // 中央から前
          if ( _CmpGeometry.ToHalfAdjust( minorPnt.X, -9 ) <= _CmpGeometry.ToHalfAdjust( wideCenter.X, -9 ) ) {
            Revit.DB.XYZ nearX = XPointNearMax( minorPnt, majority, ref index ) ;

            minorPnt = new Revit.DB.XYZ( nearX.X, minorPnt.Y, minorPnt.Z ) ;
            minority[ i ] = minorPnt ;
          }
          // 後ろ
          else {
            Revit.DB.XYZ nearX = XPointNearMin( minorPnt, majority, ref index ) ;

            minorPnt = new Revit.DB.XYZ( nearX.X, minorPnt.Y, minorPnt.Z ) ;
            minority[ i ] = minorPnt ;
          }
        }
      }

      // 肋筋位置の調整
      for ( int i = 0 ; i < stirrupXPoints.Count ; ++i ) {
        // 肋筋
        Revit.DB.XYZ stirrupPoint = stirrupXPoints[ i ] ;

        int usedMajorityNum = 0 ;
        int usedMinorityNum = 0 ;

        // 上下筋とも同じ位置にあるか
        bool isNearMajority = IsHaveNearXPoint( stirrupPoint, majority, ref usedMajorityNum ) ;
        bool isNearMinority = IsHaveNearXPoint( stirrupPoint, minority, ref usedMinorityNum ) ;

        // ある場合
        if ( isNearMajority == true && isNearMinority == true ) {
          continue ;
        }
        else {
          // 1つ外の多数派鉄筋位置へ
          int index = 0 ;

          // 中央から前
          if ( _CmpGeometry.ToHalfAdjust( stirrupPoint.X, -9 ) <= _CmpGeometry.ToHalfAdjust( wideCenter.X, -9 ) ) {
            Revit.DB.XYZ nearX = XPointNearMax( stirrupPoint, minority, ref index ) ;
            stirrupPoint = new Revit.DB.XYZ( nearX.X, stirrupPoint.Y, stirrupPoint.Z ) ;
          }
          // 後ろ
          else {
            Revit.DB.XYZ nearX = XPointNearMin( stirrupPoint, minority, ref index ) ;
            stirrupPoint = new Revit.DB.XYZ( nearX.X, stirrupPoint.Y, stirrupPoint.Z ) ;
          }
        }

        stirrupXPoints[ i ] = stirrupPoint ;
      }

      // 肋筋位置の小数筋間での再配置
      // 肋筋が付かず、かつ逆にも肋筋が付かない

      // 前半分まで行い、逆位置を移動
      // 最大回数
      int forMax = 0 ;
      if ( minority.Count % 2 == 0 ) {
        forMax = ( minority.Count + 2 ) / 2 ;
      }
      else {
        forMax = ( minority.Count + 2 + 1 ) / 2 ;
      }

      Collections.Generic.IList<Revit.DB.XYZ> allMinorX = new Collections.Generic.List<Revit.DB.XYZ>() ;
      allMinorX.Add( left ) ;
      foreach ( Revit.DB.XYZ p in minority ) {
        allMinorX.Add( p ) ;
      }

      allMinorX.Add( right ) ;

      allMinorX = SortByXAry( allMinorX ) ;

      Collections.Generic.IList<Revit.DB.XYZ> allStirrupX = new Collections.Generic.List<Revit.DB.XYZ>() ;
      allStirrupX.Add( left ) ;
      foreach ( Revit.DB.XYZ p in stirrupXPoints ) {
        allStirrupX.Add( p ) ;
      }

      allStirrupX.Add( right ) ;

      allStirrupX = SortByXAry( allStirrupX ) ;

      int mae = 0 ;

      for ( int i = 0 ; i < allMinorX.Count - 1 ; ++i ) {
        if ( i == 0 ) {
          continue ;
        }

        Revit.DB.XYZ p = allMinorX[ i ] ;

        int stirrupIndex = 0 ;

        // 肋筋位置か
        bool isStirrupMinor = IsHaveNearXPoint( p, allStirrupX, ref stirrupIndex ) ;

        if ( isStirrupMinor ) {
          if ( i - mae > 1 ) {
            Revit.DB.XYZ maeP = allMinorX[ mae ] ;

            double distance = p.X - maeP.X ;
            double pitch = distance / ( i - mae ) ;

            for ( int j = 1 ; j < i - mae ; ++j ) {
              Revit.DB.XYZ newPnt = new Revit.DB.XYZ( allMinorX[ mae ].X + pitch * j, allMinorX[ mae + j ].Y, allMinorX[ mae + j ].Z ) ;

              // 逆位置が肋筋位置でないか
              Revit.DB.XYZ newGyakuPnt = _CmpGeometry.ReversePoint_X( wideCenter, newPnt ) ;
              isStirrupMinor = IsHaveNearXPoint( newGyakuPnt, allStirrupX, ref stirrupIndex ) ;

              if ( isStirrupMinor == false ) {
                allMinorX[ mae + j ] = new Revit.DB.XYZ( allMinorX[ mae ].X + pitch * j, allMinorX[ mae + j ].Y, allMinorX[ mae + j ].Z ) ;

                //allMinorX[allMinorX.Count - 1 - (mae + j)] = _CmpGeometry.ReversePoint_X(wideCenter, allMinorX[mae + j]);
              }
            }
          }

          mae = i ;
        }
      }

      for ( int i = 0 ; i < minority.Count ; ++i ) {
        minority[ i ] = allMinorX[ i + 1 ] ;
      }

      // 肋筋を鉄筋幅の半分ずらす
      for ( int i = 0 ; i < stirrupXPoints.Count ; ++i ) {
        Revit.DB.XYZ point = stirrupXPoints[ i ] ;
        double half = diameter / 2 ;

        // 中心より前
        if ( _CmpGeometry.ToHalfAdjust( point.X, -9 ) <= _CmpGeometry.ToHalfAdjust( wideCenter.X, -9 ) ) {
          stirrupXPoints[ i ] = new Revit.DB.XYZ( point.X - half, point.Y, point.Z ) ;
        }
        // 中心より後
        else {
          stirrupXPoints[ i ] = new Revit.DB.XYZ( point.X + half, point.Y, point.Z ) ;
        }
      }
    }

    /// ================================================================================
    /// <summary>中央に近い座標順</summary>
    ///
    /// <history>2014/01/20 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public Collections.Generic.IList<Revit.DB.XYZ> SortXByCenter( Revit.DB.XYZ center, Collections.Generic.IList<Revit.DB.XYZ> points )
    {
      Collections.Generic.IList<Revit.DB.XYZ> ret = new Collections.Generic.List<Revit.DB.XYZ>() ;

      Collections.Generic.IList<Revit.DB.XYZ> list = new Collections.Generic.List<Revit.DB.XYZ>() ;

      foreach ( Revit.DB.XYZ p in points ) {
        list.Add( p ) ;
      }

      while ( list.Count > 0 ) {
        Revit.DB.XYZ point = null ;

        foreach ( Revit.DB.XYZ p in list ) {
          if ( point == null ) {
            point = p ;
            continue ;
          }

          // 一番中央に近い点
          if ( _CmpGeometry.ToHalfAdjust( _CmpGeometry.Distance2D( point, center ), -9 ) > _CmpGeometry.ToHalfAdjust( _CmpGeometry.Distance2D( p, center ), -9 ) ) {
            point = p ;
          }
          // 同じ場合は左
          else if ( _CmpGeometry.ToHalfAdjust( _CmpGeometry.Distance2D( point, center ), -9 ) == _CmpGeometry.ToHalfAdjust( _CmpGeometry.Distance2D( p, center ), -9 ) ) {
            if ( point.X > p.X ) {
              point = p ;
            }
          }
        }

        ret.Add( point ) ;
        list.Remove( point ) ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>含まれる点か(2次元で1mm以下)</summary>
    ///
    /// <history>2013/12/25 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public bool IsContainPoint( Revit.DB.XYZ p, Collections.Generic.IList<Revit.DB.XYZ> points )
    {
      bool ret = false ;

      foreach ( Revit.DB.XYZ pnt in points ) {
        if ( _CmpGeometry.Distance2D( p, pnt ) <= 1 / 304.8 ) {
          ret = true ;
          break ;
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>上下筋・肋筋位置の調整</summary>
    ///
    /// <history>2013/07/12 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public void MoveToMajorityRebar( Collections.Generic.IList<Revit.DB.XYZ> majority_Huto, Collections.Generic.IList<Revit.DB.XYZ> majority_Hoso, Collections.Generic.IList<Revit.DB.XYZ> majority, ref Collections.Generic.IList<Revit.DB.XYZ> minority, ref Collections.Generic.IList<Revit.DB.XYZ> stirrupXPoints, double diameter, Revit.DB.XYZ wideCenter )
    {
      // 多数派太径未満
      if ( stirrupXPoints.Count <= majority_Huto.Count ) {
        #region 多数派太径のみで配筋

        for ( int i = 0 ; i < stirrupXPoints.Count ; ++i ) {
          #region 肋筋位置

          Revit.DB.XYZ point = stirrupXPoints[ i ] ;

          int usedMajor = 0 ;
          int usedMinor = 0 ;

          // 上下にある
          bool isNearMajor = IsHaveNearXPoint( point, majority_Huto, ref usedMajor ) ;
          bool isNearMinor = IsHaveNearXPoint( point, minority, ref usedMinor ) ;

          if ( isNearMajor == true && isNearMinor == true ) {
            continue ;
          }

          Collections.Generic.IList<Collections.Generic.IList<double>> orderAndDiameter = OrderAndDiameter( majority_Huto, majority_Hoso ) ;

          int beforeNum = 0 ;
          int afterNum = 0 ;

          StirrupBeforeAfterPoint( point, orderAndDiameter, ref beforeNum, ref afterNum, wideCenter ) ;

          Collections.Generic.IList<double> beforePointAndDiameter = orderAndDiameter[ beforeNum ] ;
          Collections.Generic.IList<double> afterPointAndDiameter = orderAndDiameter[ afterNum ] ;

          double beforePointX = beforePointAndDiameter[ 0 ] ;
          double beforeDiameter = beforePointAndDiameter[ 1 ] ;

          double afterPointX = afterPointAndDiameter[ 0 ] ;
          double afterDiameter = afterPointAndDiameter[ 1 ] ;

          // 前後とも太径
          if ( beforeDiameter == 0 && afterDiameter == 0 ) {
            // 中心より前
            if ( _CmpParameters.ToHalfAdjust( point.X, -9 ) <= _CmpParameters.ToHalfAdjust( wideCenter.X, -9 ) ) {
              stirrupXPoints[ i ] = new Revit.DB.XYZ( beforePointX, point.Y, point.Z ) ;
            }
            // 中心より後
            else {
              stirrupXPoints[ i ] = new Revit.DB.XYZ( afterPointX, point.Y, point.Z ) ;
            }
          }
          // どちらかが太径
          else {
            if ( beforeDiameter == 0 ) {
              stirrupXPoints[ i ] = new Revit.DB.XYZ( beforePointX, point.Y, point.Z ) ;
            }
            else if ( afterDiameter == 0 ) {
              stirrupXPoints[ i ] = new Revit.DB.XYZ( afterPointX, point.Y, point.Z ) ;
            }
          }

          #endregion
        }

        #endregion
      }
      // 多数派太径と同じ
      else if ( stirrupXPoints.Count == majority_Huto.Count ) {
        #region すべての太径に配筋

        for ( int i = 0 ; i < stirrupXPoints.Count ; ++i ) {
          Revit.DB.XYZ stirrupPnt = stirrupXPoints[ i ] ;
          Revit.DB.XYZ hutoPnt = majority_Huto[ i ] ;

          stirrupXPoints[ i ] = new Revit.DB.XYZ( hutoPnt.X, stirrupPnt.Y, stirrupPnt.Z ) ;
        }

        #endregion
      }
      // 多数派太径より多い
      else {
        #region 多数派太径 + 多数派細径

        int overStirrup = stirrupXPoints.Count - majority_Huto.Count ;

        Collections.Generic.IList<Revit.DB.XYZ> stirrupHosoSet = StirrupSetHoso( overStirrup, majority_Hoso, wideCenter ) ;

        Collections.Generic.IList<Revit.DB.XYZ> sumPoints = new Collections.Generic.List<Revit.DB.XYZ>() ;
        foreach ( Revit.DB.XYZ p in majority_Huto ) {
          sumPoints.Add( p ) ;
        }

        foreach ( Revit.DB.XYZ p in stirrupHosoSet ) {
          sumPoints.Add( p ) ;
        }

        for ( int i = 0 ; i < stirrupXPoints.Count ; ++i ) {
          Revit.DB.XYZ stirrupPnt = stirrupXPoints[ i ] ;
          Revit.DB.XYZ Pnt = sumPoints[ i ] ;

          stirrupXPoints[ i ] = new Revit.DB.XYZ( Pnt.X, stirrupPnt.Y, stirrupPnt.Z ) ;
        }

        #endregion
      }

      #region 少数派

      // 少数派 < 肋筋
      if ( minority.Count < stirrupXPoints.Count ) {
        // エラー
      }
      // 少数派 = 肋筋
      else if ( minority.Count == stirrupXPoints.Count ) {
        // 同じ位置
        for ( int i = 0 ; i < stirrupXPoints.Count ; ++i ) {
          Revit.DB.XYZ point = stirrupXPoints[ i ] ;
          Revit.DB.XYZ minorPnt = minority[ i ] ;

          minority[ i ] = new Revit.DB.XYZ( point.X, minorPnt.Y, minorPnt.Z ) ;
        }
      }
      // 少数派 > 肋筋
      else if ( minority.Count > stirrupXPoints.Count ) {
        // 少数派が奇数で肋筋が偶数の場合、少数派の中間は対象外
        bool match = false ;
        Revit.DB.XYZ centerPnt = null ;
        int half = 0 ;

        if ( minority.Count % 2 != 0 && stirrupXPoints.Count % 2 == 0 ) {
          match = true ;
          half = (int)System.Math.Floor( (double)( minority.Count / 2 ) ) ;

          centerPnt = minority[ half ] ;
          minority.Remove( centerPnt ) ;
        }

        for ( int i = 0 ; i < stirrupXPoints.Count ; ++i ) {
          int usedMinor = 0 ;

          Revit.DB.XYZ point = stirrupXPoints[ i ] ;
          bool isNearMinor = IsHaveNearXPoint( point, minority, ref usedMinor ) ;

          if ( isNearMinor == true ) {
            continue ;
          }

          // 中心より前
          if ( _CmpGeometry.ToHalfAdjust( point.X, -9 ) <= _CmpGeometry.ToHalfAdjust( wideCenter.X, -9 ) ) {
            int num = 0 ;
            Revit.DB.XYZ nearMinor = XPointNearMin( point, minority, ref num ) ; // XPointNear(point, minority, ref num);

            minority[ num ] = new Revit.DB.XYZ( point.X, nearMinor.Y, nearMinor.Z ) ;

            // 逆位置
            // 奇数本の中心は除く
            if ( minority.Count - 1 - num != num ) {
              Revit.DB.XYZ reversePnt = _CmpGeometry.ReversePoint_X( wideCenter, minority[ num ] ) ;
              minority[ minority.Count - 1 - num ] = reversePnt ;
            }

            // より前の鉄筋も移動
            for ( int j = 0 ; j < minority.Count ; ++j ) {
              int numBefore = num - j - 1 ;

              if ( numBefore < 0 || numBefore >= minority.Count ) {
                break ;
              }

              int sute = 0 ;
              nearMinor = XPointNearMax( minority[ numBefore ], majority, ref sute ) ;
              minority[ numBefore ] = new Revit.DB.XYZ( nearMinor.X, minority[ numBefore ].Y, minority[ numBefore ].Z ) ;

              // 逆位置
              int numReverse = minority.Count - numBefore - 1 ;
              nearMinor = XPointNearMin( minority[ numReverse ], majority, ref sute ) ;
              minority[ numReverse ] = new Revit.DB.XYZ( nearMinor.X, minority[ numReverse ].Y, minority[ numReverse ].Z ) ;
            }
          }
          // 中心より後
          else {
            int num = 0 ;
            Revit.DB.XYZ nearMinor = XPointNearMax( point, minority, ref num ) ; // XPointNear(point, minority, ref num);

            minority[ num ] = new Revit.DB.XYZ( point.X, nearMinor.Y, nearMinor.Z ) ;

            // 逆位置
            if ( minority.Count - 1 - num != num ) {
              Revit.DB.XYZ reversePnt = _CmpGeometry.ReversePoint_X( wideCenter, minority[ num ] ) ;
              minority[ minority.Count - 1 - num ] = reversePnt ;
            }

            // より後の鉄筋も移動
            for ( int j = 0 ; j < minority.Count ; ++j ) {
              int numAfter = num - j + 1 ;

              if ( numAfter >= minority.Count || numAfter < 0 ) {
                break ;
              }

              int sute = 0 ;
              nearMinor = XPointNearMin( minority[ numAfter ], majority, ref sute ) ;
              minority[ numAfter ] = new Revit.DB.XYZ( nearMinor.X, minority[ numAfter ].Y, minority[ numAfter ].Z ) ;

              // 逆位置
              int numReberse = minority.Count - numAfter - 1 ;
              nearMinor = XPointNearMax( minority[ numReberse ], majority, ref sute ) ;
              minority[ numReberse ] = new Revit.DB.XYZ( nearMinor.X, minority[ numReberse ].Y, minority[ numReberse ].Z ) ;
            }
          }
        }

        // 少数派の中間を戻す
        if ( match == true ) {
          Collections.Generic.IList<Revit.DB.XYZ> pnts = new Collections.Generic.List<Revit.DB.XYZ>() ;

          for ( int i = 0 ; i < minority.Count ; ++i ) {
            if ( i == half ) {
              pnts.Add( centerPnt ) ;
            }

            Revit.DB.XYZ p = minority[ i ] ;
            pnts.Add( p ) ;
          }

          minority = pnts ;
        }
      }

      #endregion

      #region 肋筋を鉄筋幅の半分ずらす

      for ( int i = 0 ; i < stirrupXPoints.Count ; ++i ) {
        Revit.DB.XYZ point = stirrupXPoints[ i ] ;
        double half = diameter / 2 ;

        // 中心より前
        if ( _CmpGeometry.ToHalfAdjust( point.X, -9 ) <= _CmpGeometry.ToHalfAdjust( wideCenter.X, -9 ) ) {
          stirrupXPoints[ i ] = new Revit.DB.XYZ( point.X - half, point.Y, point.Z ) ;
        }
        // 中心より後
        else {
          stirrupXPoints[ i ] = new Revit.DB.XYZ( point.X + half, point.Y, point.Z ) ;
        }
      }

      #endregion
    }

    /// ================================================================================
    /// <summary>X座標が同じ点があるか</summary>
    ///
    /// <param name="point"     >任意点</param>
    /// <param name="points"    >複数点</param>
    /// <param name="nearPntNum">インデックス</param>
    ///
    /// <history><p>2013/05/14 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public bool IsHaveNearXPoint( Revit.DB.XYZ point, Collections.Generic.IList<Revit.DB.XYZ> points, ref int nearPntNum )
    {
      bool ret = false ;

      for ( int i = 0 ; i < points.Count ; ++i ) {
        Revit.DB.XYZ p = points[ i ] ;

        if ( System.Math.Abs( point.X - p.X ) <= 1 / 304.8 ) {
          nearPntNum = i ;
          ret = true ;
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>肋筋のX座標が同じ点があるか</summary>
    ///
    /// <history><p>2013/08/05 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public bool IsStirrupNearXPoint( Revit.DB.XYZ point, Collections.Generic.IList<Revit.DB.XYZ> points, ref int indexNumber, Collections.Generic.IList<int> usedNumber )
    {
      bool ret = false ;

      foreach ( Revit.DB.XYZ p in points ) {
        if ( System.Math.Abs( point.X - p.X ) <= 1 / 304.8 ) {
          indexNumber = points.IndexOf( p ) ;

          if ( ! usedNumber.Contains( indexNumber ) ) {
            ret = true ;
          }
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>指定座標にX座標が1番近い座標</summary>
    ///
    /// <history><p>2013/07/12 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Revit.DB.XYZ XPointNear( Revit.DB.XYZ point, Collections.Generic.IList<Revit.DB.XYZ> points, ref int num )
    {
      Revit.DB.XYZ ret = null ;

      Collections.Generic.IList<Revit.DB.XYZ> sortXPoints = SortByXAry( points ) ;

      Revit.DB.XYZ nearPoint = null ;
      double distance = 0 ;

      for ( int i = 0 ; i < sortXPoints.Count ; ++i ) {
        Revit.DB.XYZ p = sortXPoints[ i ] ;

        if ( i == 0 ) {
          nearPoint = p ;
          distance = System.Math.Abs( point.X - nearPoint.X ) ;
        }

        double distance2 = System.Math.Abs( point.X - p.X ) ;

        if ( distance > distance2 ) {
          nearPoint = p ;
          distance = distance2 ;

          num = i ;
        }

        if ( distance == distance2 ) {
        }
      }

      ret = nearPoint ;
      return ret ;
    }

    /// ================================================================================
    /// <summary>指定座標に1番近い座標</summary>
    ///
    /// <history><p>2013/11/06 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Revit.DB.XYZ PointNear( Revit.DB.XYZ point, Collections.Generic.IList<Revit.DB.XYZ> points )
    {
      Revit.DB.XYZ nearPoint = null ;
      double distance = 0 ;

      for ( int i = 0 ; i < points.Count ; ++i ) {
        Revit.DB.XYZ p = points[ i ] ;

        if ( i == 0 ) {
          nearPoint = p ;
          distance = _CmpGeometry.Distance2D( point, nearPoint ) ;

          continue ;
        }

        double distance2 = _CmpGeometry.Distance2D( point, p ) ;

        if ( _CmpParameters.ToHalfAdjust( distance, -9 ) > _CmpParameters.ToHalfAdjust( distance2, -9 ) ) {
          nearPoint = p ;
          distance = distance2 ;
        }
      }

      return nearPoint ;
    }

    /// ================================================================================
    /// <summary>指定座標よりX座標が小さい最大な座標</summary>
    ///
    /// <history><p>2013/05/14 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Revit.DB.XYZ XPointNearMax( Revit.DB.XYZ point, Collections.Generic.IList<Revit.DB.XYZ> points, ref int num )
    {
      Revit.DB.XYZ ret = null ;

      Collections.Generic.IList<Revit.DB.XYZ> sortXPoints = SortByXAry( points ) ;

      for ( int i = 0 ; i < sortXPoints.Count ; ++i ) {
        if ( i - 1 >= 0 ) {
          if ( _CmpGeometry.ToHalfAdjust( point.X, -9 ) <= _CmpGeometry.ToHalfAdjust( sortXPoints[ i ].X, -9 ) ) {
            ret = sortXPoints[ i - 1 ] ;
            num = i - 1 ;

            break ;
          }
        }
      }

      if ( ret == null ) {
        ret = sortXPoints[ 0 ] ;
        num = 0 ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>指定座標よりX座標が大きい最小な座標</summary>
    ///
    /// <history><p>2013/05/14 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Revit.DB.XYZ XPointNearMin( Revit.DB.XYZ point, Collections.Generic.IList<Revit.DB.XYZ> points, ref int num )
    {
      Revit.DB.XYZ ret = null ;

      Collections.Generic.IList<Revit.DB.XYZ> sortXPoints = SortByXAry( points ) ;

      for ( int i = 0 ; i < sortXPoints.Count ; ++i ) {
        if ( _CmpGeometry.ToHalfAdjust( point.X, -9 ) < _CmpGeometry.ToHalfAdjust( sortXPoints[ i ].X, -9 ) ) {
          ret = sortXPoints[ i ] ;
          num = i ;

          break ;
        }
        else {
          ret = sortXPoints[ i ] ;
          num = i ;
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>多数派の指定座標よりX座標が小さい最大な座標</summary>
    ///
    /// <history><p>2013/08/05 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Revit.DB.XYZ XPointNearMax_Major( Revit.DB.XYZ point, Collections.Generic.IList<Revit.DB.XYZ> points, ref int num, Collections.Generic.IList<int> usedNumber, int stirrupNumber, Collections.Generic.IList<Revit.DB.XYZ> minor, Revit.DB.XYZ wideCenter )
    {
      Revit.DB.XYZ ret = null ;

      Collections.Generic.IList<Revit.DB.XYZ> sortXPoints = SortByXAry( points ) ;

      for ( int i = 0 ; i < sortXPoints.Count ; ++i ) {
        if ( i - 1 >= 0 ) {
          if ( _CmpGeometry.ToHalfAdjust( point.X, -9 ) <= _CmpGeometry.ToHalfAdjust( sortXPoints[ i ].X, -9 ) ) {
            if ( ! usedNumber.Contains( i ) ) {
              ret = sortXPoints[ i - 1 ] ;
              num = i - 1 ;

              bool isInner = IsInner( ret, minor, wideCenter, stirrupNumber ) ;

              if ( isInner == false ) {
                ret = sortXPoints[ i - 2 ] ;
                num = i - 2 ;
              }

              break ;
            }
          }
        }
      }

      if ( ret == null ) {
        ret = sortXPoints[ sortXPoints.Count - 1 ] ;
        num = sortXPoints.Count - 1 ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>多数派の指定座標よりX座標が大きい最小な座標</summary>
    ///
    /// <history><p>2013/08/05 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Revit.DB.XYZ XPointNearMin_Major( Revit.DB.XYZ point, Collections.Generic.IList<Revit.DB.XYZ> major, ref int num, Collections.Generic.IList<int> usedNumber, int stirrupNumber, Collections.Generic.IList<Revit.DB.XYZ> minor, Revit.DB.XYZ wideCenter )
    {
      Revit.DB.XYZ ret = null ;

      Collections.Generic.IList<Revit.DB.XYZ> sortXPoints = SortByXAry( major ) ;

      for ( int i = 0 ; i < sortXPoints.Count ; ++i ) {
        if ( _CmpGeometry.ToHalfAdjust( point.X, -9 ) < _CmpGeometry.ToHalfAdjust( sortXPoints[ i ].X, -9 ) ) {
          if ( ! usedNumber.Contains( i ) ) {
            ret = sortXPoints[ i ] ;
            num = i ;

            // 内側に少数派がない場合
            bool isInner = IsInner( ret, minor, wideCenter, stirrupNumber ) ;

            if ( isInner == false ) {
              if ( i + 1 < sortXPoints.Count ) {
                ret = sortXPoints[ i + 1 ] ;
                num = i + 1 ;
              }
            }

            break ;
          }
        }
        else {
          ret = sortXPoints[ i ] ;
          num = i ;
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>多数派の指定座標よりX座標が小さい最大な座標</summary>
    ///
    /// <history><p>2014/01/20 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Revit.DB.XYZ XPointNearMax_Major_2( Revit.DB.XYZ defPnt, Collections.Generic.IList<Revit.DB.XYZ> majorPnts, ref int num, Collections.Generic.IList<int> usedNumbers, int stirrupNumber, Collections.Generic.IList<Revit.DB.XYZ> minorPnts, Collections.Generic.IList<int> usedMinorNumber, Revit.DB.XYZ wideCenter )
    {
      Revit.DB.XYZ ret = null ;

      Collections.Generic.IList<Revit.DB.XYZ> sortXPnts = SortByXAry( majorPnts ) ;

      for ( int i = 0 ; i < sortXPnts.Count ; ++i ) {
        if ( i - 1 >= 0 ) {
          if ( _CmpGeometry.ToHalfAdjust( defPnt.X, -9 ) < _CmpGeometry.ToHalfAdjust( sortXPnts[ i ].X, -9 ) ) {
            if ( ! usedNumbers.Contains( i ) ) {
              ret = sortXPnts[ i - 1 ] ;
              num = i - 1 ;

              bool isInner = IsMinorPntOver( ret, minorPnts, wideCenter, usedMinorNumber, stirrupNumber ) ;

              if ( isInner == false ) {
                ret = sortXPnts[ i - 2 ] ;
                num = i - 2 ;
              }

              break ;
            }
            else {
              ret = sortXPnts[ i - 1 ] ;
              num = i - 1 ;

              bool isInner = IsMinorPntOver( ret, minorPnts, wideCenter, usedMinorNumber, stirrupNumber ) ; // IsInner(ret, minorPnts, wideCenter, stirrupNumber);

              if ( isInner == false ) {
                ret = sortXPnts[ i - 2 ] ;
                num = i - 2 ;
              }

              break ;
            }
          }
        }
      }

      if ( ret == null ) {
        ret = sortXPnts[ sortXPnts.Count - 1 ] ;
        num = sortXPnts.Count - 1 ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>多数派の指定座標よりX座標が大きい最小な座標</summary>
    ///
    /// <history><p>2013/08/05 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Revit.DB.XYZ XPointNearMin_Major_2( Revit.DB.XYZ point, Collections.Generic.IList<Revit.DB.XYZ> major, ref int num, Collections.Generic.IList<int> usedNumber, int stirrupNumber, Collections.Generic.IList<Revit.DB.XYZ> minor, Collections.Generic.IList<int> usedMinorNumber, Revit.DB.XYZ wideCenter )
    {
      Revit.DB.XYZ ret = null ;

      Collections.Generic.IList<Revit.DB.XYZ> sortXPoints = SortByXAry( major ) ;

      for ( int i = 0 ; i < sortXPoints.Count ; ++i ) {
        if ( _CmpGeometry.ToHalfAdjust( point.X, -9 ) <= _CmpGeometry.ToHalfAdjust( sortXPoints[ i ].X, -9 ) ) {
          if ( ! usedNumber.Contains( i ) ) {
            ret = sortXPoints[ i ] ;
            num = i ;

            // 内側に少数派がない場合
            bool isInner = IsMinorPntOver( ret, minor, wideCenter, usedMinorNumber, stirrupNumber ) ; // IsInner(ret, minor, wideCenter, stirrupNumber);

            if ( isInner == false ) {
              if ( i + 1 < sortXPoints.Count ) {
                ret = sortXPoints[ i + 1 ] ;
                num = i + 1 ;
              }
            }

            break ;
          }
          else {
            //ret = sortXPoints[i];
            //num = i;

            //// 内側に少数派がない場合
            //bool isInner = IsMinorPntOver(ret, minor, wideCenter, usedMinorNumber, stirrupNumber);// IsInner(ret, minor, wideCenter, stirrupNumber);

            //if (isInner == false)
            //{
            //  if (i + 1 < sortXPoints.Count)
            //  {
            //    ret = sortXPoints[i + 1];
            //    num = i + 1;
            //  }
            //}

            //break;
          }
        }
        else {
          ret = sortXPoints[ i ] ;
          num = i ;
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary></summary>
    ///
    /// <history><p>2014/01/20 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public bool IsMinorPntOver( Revit.DB.XYZ defPnt, Collections.Generic.IList<Revit.DB.XYZ> minorPnts, Revit.DB.XYZ centerPnt, Collections.Generic.IList<int> usedMinorNumber, int stirrupNumber )
    {
      bool ret = false ;

      // 中央から前
      if ( _CmpGeometry.ToHalfAdjust( defPnt.X, -9 ) <= _CmpGeometry.ToHalfAdjust( centerPnt.X, -9 ) ) {
        for ( int i = 0 ; i < minorPnts.Count ; ++i ) {
          Revit.DB.XYZ p = minorPnts[ i ] ;

          if ( usedMinorNumber.Contains( i ) ) {
            continue ;
          }

          // 指定点より小さい
          if ( _CmpGeometry.ToHalfAdjust( p.X, -9 ) < _CmpGeometry.ToHalfAdjust( defPnt.X, -9 ) ) {
            continue ;
          }

          // 中央を超えた
          if ( _CmpGeometry.ToHalfAdjust( p.X, -9 ) > _CmpGeometry.ToHalfAdjust( centerPnt.X, -9 ) ) {
            break ;
          }

          // 肋筋が偶数本のときの中央
          if ( stirrupNumber % 2 == 0 ) {
            if ( minorPnts.Count - 1 - i == i ) {
              continue ;
            }
          }

          ret = true ;
        }
      }
      // 後
      else {
        for ( int i = 0 ; i < minorPnts.Count ; ++i ) {
          Revit.DB.XYZ p = minorPnts[ i ] ;

          if ( usedMinorNumber.Contains( i ) ) {
            continue ;
          }

          // 指定点を超えた
          if ( _CmpGeometry.ToHalfAdjust( p.X, -9 ) > _CmpGeometry.ToHalfAdjust( defPnt.X, -9 ) ) {
            break ;
          }

          // 中央に満たない
          if ( _CmpGeometry.ToHalfAdjust( p.X, -9 ) < _CmpGeometry.ToHalfAdjust( centerPnt.X, -9 ) ) {
            continue ;
          }

          // 肋筋が偶数本のときの中央
          if ( stirrupNumber % 2 == 0 ) {
            if ( minorPnts.Count - 1 - i == i ) {
              continue ;
            }
          }

          ret = true ;
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>指定点より内側に点があるか</summary>
    ///
    /// <history><p>2013/08/05 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public bool IsInner( Revit.DB.XYZ point, Collections.Generic.IList<Revit.DB.XYZ> points, Revit.DB.XYZ center, int stirrupNumber )
    {
      bool ret = false ;

      // 中央から前
      if ( _CmpGeometry.ToHalfAdjust( point.X, -9 ) <= _CmpGeometry.ToHalfAdjust( center.X, -9 ) ) {
        for ( int i = 0 ; i < points.Count ; ++i ) {
          Revit.DB.XYZ p = points[ i ] ;

          // 指定点より小さい
          if ( _CmpGeometry.ToHalfAdjust( p.X, -9 ) < _CmpGeometry.ToHalfAdjust( point.X, -9 ) ) {
            continue ;
          }

          // 中央を超えた
          if ( _CmpGeometry.ToHalfAdjust( p.X, -9 ) > _CmpGeometry.ToHalfAdjust( center.X, -9 ) ) {
            break ;
          }

          // 肋筋が偶数本のときの中央
          if ( stirrupNumber % 2 == 0 ) {
            if ( points.Count - 1 - i == i ) {
              continue ;
            }
          }

          ret = true ;
        }
      }
      // 後
      else {
        for ( int i = 0 ; i < points.Count ; ++i ) {
          Revit.DB.XYZ p = points[ i ] ;

          // 指定点を超えた
          if ( _CmpGeometry.ToHalfAdjust( p.X, -9 ) > _CmpGeometry.ToHalfAdjust( point.X, -9 ) ) {
            break ;
          }

          // 中央に満たない
          if ( _CmpGeometry.ToHalfAdjust( p.X, -9 ) < _CmpGeometry.ToHalfAdjust( center.X, -9 ) ) {
            continue ;
          }

          // 肋筋が偶数本のときの中央
          if ( stirrupNumber % 2 == 0 ) {
            if ( points.Count - 1 - i == i ) {
              continue ;
            }
          }

          ret = true ;
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>少数派の指定座標よりX座標が小さい最大な座標</summary>
    ///
    /// <history><p>2013/08/05 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Revit.DB.XYZ XPointNearMax_Minor( Revit.DB.XYZ point, Collections.Generic.IList<Revit.DB.XYZ> points, ref int num, Collections.Generic.IList<int> usedNumber )
    {
      Revit.DB.XYZ ret = null ;

      Collections.Generic.IList<Revit.DB.XYZ> sortXPoints = SortByXAry( points ) ;

      for ( int i = 0 ; i < sortXPoints.Count ; ++i ) {
        if ( i - 1 >= 0 ) {
          if ( _CmpGeometry.ToHalfAdjust( point.X, -9 ) < _CmpGeometry.ToHalfAdjust( sortXPoints[ i ].X, -9 ) ) {
            if ( ! usedNumber.Contains( i ) ) {
              ret = sortXPoints[ i - 1 ] ;
              num = i - 1 ;

              break ;
            }
          }
        }
      }

      if ( ret == null ) {
        ret = sortXPoints[ sortXPoints.Count - 1 ] ;
        num = sortXPoints.Count - 1 ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>少数派の指定座標よりX座標が大きい最小な座標</summary>
    ///
    /// <history><p>2013/08/05 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Revit.DB.XYZ XPointNearMin_Minor( Revit.DB.XYZ point, Collections.Generic.IList<Revit.DB.XYZ> points, ref int num, Collections.Generic.IList<int> usedNumber )
    {
      Revit.DB.XYZ ret = null ;

      Collections.Generic.IList<Revit.DB.XYZ> sortXPoints = SortByXAry( points ) ;

      for ( int i = 0 ; i < sortXPoints.Count ; ++i ) {
        if ( _CmpGeometry.ToHalfAdjust( point.X, -9 ) <= _CmpGeometry.ToHalfAdjust( sortXPoints[ i ].X, -9 ) ) {
          if ( ! usedNumber.Contains( i ) ) {
            ret = sortXPoints[ i ] ;
            num = i ;

            break ;
          }
        }
        else {
          ret = sortXPoints[ i ] ;
          num = i ;
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>少数派の指定座標よりX座標が小さい最大な座標</summary>
    ///
    /// <history><p>2014/01/20 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Revit.DB.XYZ XPointNearMax_Minor_2( Revit.DB.XYZ point, Collections.Generic.IList<Revit.DB.XYZ> points, ref int num, Collections.Generic.IList<int> usedNumber )
    {
      Revit.DB.XYZ ret = null ;

      Collections.Generic.IList<Revit.DB.XYZ> sortXPoints = SortByXAry( points ) ;

      for ( int i = 0 ; i < sortXPoints.Count ; ++i ) {
        if ( i - 1 >= 0 ) {
          if ( _CmpGeometry.ToHalfAdjust( point.X, -9 ) < _CmpGeometry.ToHalfAdjust( sortXPoints[ i ].X, -9 ) ) {
            if ( ! usedNumber.Contains( i ) ) {
              ret = sortXPoints[ i - 1 ] ;
              num = i - 1 ;

              break ;
            }
          }
        }
      }

      if ( ret == null ) {
        ret = sortXPoints[ sortXPoints.Count - 1 ] ;
        num = sortXPoints.Count - 1 ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>少数派の指定座標よりX座標が大きい最小な座標</summary>
    ///
    /// <history><p>2014/01/20 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Revit.DB.XYZ XPointNearMin_Minor_2( Revit.DB.XYZ point, Collections.Generic.IList<Revit.DB.XYZ> points, ref int num, Collections.Generic.IList<int> usedNumber )
    {
      Revit.DB.XYZ ret = null ;

      Collections.Generic.IList<Revit.DB.XYZ> sortXPoints = SortByXAry( points ) ;

      for ( int i = 0 ; i < sortXPoints.Count ; ++i ) {
        if ( _CmpGeometry.ToHalfAdjust( point.X, -9 ) <= _CmpGeometry.ToHalfAdjust( sortXPoints[ i ].X, -9 ) ) {
          if ( ! usedNumber.Contains( i ) ) {
            ret = sortXPoints[ i ] ;
            num = i ;

            break ;
          }
        }
        else {
          ret = sortXPoints[ i ] ;
          num = i ;
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>X方向の点または1つ外の点</summary>
    ///
    /// <history><p>2013/05/10 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Revit.DB.XYZ HoopNearXPoint( Revit.DB.XYZ point, Collections.Generic.IList<Revit.DB.XYZ> points, ref bool before )
    {
      Revit.DB.XYZ ret = null ;

      Collections.Generic.IList<Revit.DB.XYZ> sortX = SortByXAry( points ) ;

      for ( int i = 0 ; i < sortX.Count ; ++i ) {
        Revit.DB.XYZ p = sortX[ i ] ;

        // 1mm以下のずれならOK
        if ( System.Math.Abs( point.X - p.X ) <= 1 / 304.8 ) {
          ret = p ;

          double count = sortX.Count ;
          double half = count / 2 ;

          // 芯鉄筋径ずらす
          if ( i < half ) {
            before = true ;
          }
          else {
            before = false ;
          }

          break ;
        }
      }

      if ( ret == null ) {
        Revit.DB.XYZ min = sortX[ 0 ] ;
        Revit.DB.XYZ max = sortX[ sortX.Count - 1 ] ;

        double distance = _CmpGeometry.Distance2D( min, max ) ;
        double halfDist = distance / 2 ;

        // 中間より前
        if ( point.X <= min.X + halfDist ) {
          for ( int i = 0 ; i < sortX.Count ; ++i ) {
            if ( point.X < sortX[ i ].X ) {
              ret = sortX[ i - 1 ] ;
              before = true ;

              break ;
            }
          }
        }
        // 中間より後
        else {
          for ( int i = 0 ; i < sortX.Count ; ++i ) {
            if ( point.X < sortX[ i ].X ) {
              ret = sortX[ i ] ;
              before = false ;

              break ;
            }
          }
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>Y方向の点または1つ外の点</summary>
    ///
    /// <history><p>2013/05/10 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Revit.DB.XYZ HoopNearYPoint( Revit.DB.XYZ point, Collections.Generic.IList<Revit.DB.XYZ> points, ref bool before )
    {
      Revit.DB.XYZ ret = null ;
      Collections.Generic.IList<Revit.DB.XYZ> sortY = SortByYAry( points ) ;

      for ( int i = 0 ; i < sortY.Count ; ++i ) {
        Revit.DB.XYZ p = sortY[ i ] ;

        // 1mm以下のずれならOK
        if ( System.Math.Abs( point.Y - p.Y ) <= 1 / 304.8 ) {
          ret = p ;

          double count = sortY.Count ;
          double half = count / 2 ;

          // 芯鉄筋径ずらす
          if ( i < half ) {
            before = true ;
          }
          else {
            before = false ;
          }

          break ;
        }
      }

      if ( ret == null ) {
        Revit.DB.XYZ min = sortY[ 0 ] ;
        Revit.DB.XYZ max = sortY[ sortY.Count - 1 ] ;

        double distance = _CmpGeometry.Distance2D( min, max ) ;
        double halfDist = distance / 2 ;

        // 中間より前
        if ( point.Y <= min.Y + halfDist ) {
          for ( int i = 0 ; i < sortY.Count ; ++i ) {
            if ( point.Y < sortY[ i ].Y ) {
              ret = sortY[ i - 1 ] ;
              before = true ;

              break ;
            }
          }
        }
        // 中間より後
        else {
          for ( int i = 0 ; i < sortY.Count ; ++i ) {
            if ( point.Y < sortY[ i ].Y ) {
              ret = sortY[ i ] ;
              before = false ;

              break ;
            }
          }
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>同心円上の芯鉄筋位置</summary>
    /// <history>2013/05/10 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public Collections.Generic.IList<Revit.DB.XYZ> SintekkinPointOnCircle( Revit.DB.XYZ center, int pointNum, double diaSintekkin )
    {
      Collections.Generic.IList<Revit.DB.XYZ> ret = new Collections.Generic.List<Revit.DB.XYZ>() ;

      double round = System.Math.PI * 2 ;
      double rad = round / pointNum ;

      // 半径
      double radius = diaSintekkin * 1.5 * System.Math.Cos( rad / 2 ) / System.Math.Sin( rad ) ;

      // 現在の角度
      double currentRad = 0 ;

      for ( int i = 0 ; i < pointNum ; ++i ) {
        currentRad = rad * i ;

        double x = System.Math.Cos( currentRad ) * radius ;
        double y = System.Math.Sin( currentRad ) * radius ;

        Revit.DB.XYZ newPoint = center + new Revit.DB.XYZ( x, y, center.Z ) ;

        ret.Add( newPoint ) ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>最大間隔の前後座標</summary>
    /// <history>2013/11/06 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public void MaxDistancePoints( Collections.Generic.IList<Revit.DB.XYZ> points, ref Revit.DB.XYZ beforePoint, ref Revit.DB.XYZ afterPoint )
    {
      Revit.DB.XYZ retBefore = null ;
      Revit.DB.XYZ retAfter = null ;

      double maxDistance = 0 ;

      for ( int i = 0 ; i < points.Count ; ++i ) {
        // 最後は不要
        if ( i == points.Count - 1 ) {
          break ;
        }

        Revit.DB.XYZ p0 = points[ i ] ;
        Revit.DB.XYZ p1 = points[ i + 1 ] ;

        double distance = _CmpGeometry.Distance2D( p0, p1 ) ;

        if ( _CmpParameters.ToHalfAdjust( distance, -9 ) > _CmpParameters.ToHalfAdjust( maxDistance, -9 ) ) {
          maxDistance = distance ;

          retBefore = p0 ;
          retAfter = p1 ;
        }
      }

      beforePoint = retBefore ;
      afterPoint = retAfter ;
    }

    /// ================================================================================
    /// <summary>含まれていない座標</summary>
    /// <history>2013/11/06 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public Collections.Generic.IList<Revit.DB.XYZ> UnUsedPoints( Collections.Generic.IList<Revit.DB.XYZ> allPoints, Collections.Generic.IList<Revit.DB.XYZ> usedPoints )
    {
      Collections.Generic.IList<Revit.DB.XYZ> ret = new Collections.Generic.List<Revit.DB.XYZ>() ;

      foreach ( Revit.DB.XYZ p in allPoints ) {
        bool isContains = false ;

        // 含まれているか
        foreach ( Revit.DB.XYZ used in usedPoints ) {
          // 1mm以下
          if ( _CmpGeometry.Distance2D( p, used ) <= 1 / 304.8 ) {
            isContains = true ;
            break ;
          }
        }

        if ( isContains == false ) {
          ret.Add( p ) ;
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>対称ではない座標</summary>
    /// <history>2013/11/06 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public Revit.DB.XYZ UnSymmetryPoint( Collections.Generic.IList<Revit.DB.XYZ> allSortedPoints, Collections.Generic.IList<Revit.DB.XYZ> usedPoints )
    {
      Revit.DB.XYZ ret = null ;

      foreach ( Revit.DB.XYZ p in usedPoints ) {
        for ( int i = 0 ; i < allSortedPoints.Count ; ++i ) {
          Revit.DB.XYZ pnt = allSortedPoints[ i ] ;

          // 同じ位置
          if ( _CmpGeometry.Distance2D( p, pnt ) <= 1 / 304.8 ) {
            // 中央は除外
            if ( i == allSortedPoints.Count - 1 - i ) {
              break ;
            }

            // 逆位置
            Revit.DB.XYZ gyakuPnt = allSortedPoints[ allSortedPoints.Count - 1 - i ] ;

            // 逆位置が既に使われているか
            bool isContains = false ;
            foreach ( Revit.DB.XYZ used in usedPoints ) {
              // 1mm以下
              if ( _CmpGeometry.Distance2D( gyakuPnt, used ) <= 1 / 304.8 ) {
                isContains = true ;
                break ;
              }
            }

            if ( isContains == false ) {
              ret = gyakuPnt ;
            }
          }
          else {
            continue ;
          }
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>幅止筋配置位置</summary>
    ///
    /// <param name="base1dankinPoints" >1段筋主筋</param>
    /// <param name="usedPoints"        >使用済位置</param>
    /// <param name="diameter"          >直径</param>
    /// <param name="vertical"          >縦方向か</param>
    ///
    /// <history><p>2013/05/13 Created GSA,Inc. Ryo Kuroda</p>
    ///           <p>2013/11/06 Modified GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Revit.DB.XYZ SpaceRebarPoint( Collections.Generic.IList<Revit.DB.XYZ> base1dankinPoints, Collections.Generic.IList<Revit.DB.XYZ> usedPoints, double diameter, bool vertical )
    {
      // 主筋が3本未満または主筋本数と使用済み本数が同じ
      if ( base1dankinPoints.Count < 3 || base1dankinPoints.Count == usedPoints.Count ) {
        return null ;
      }

      if ( vertical == true ) {
        // Y座標の小さい方から
        Collections.Generic.IList<Revit.DB.XYZ> sortY = SortByYAry( base1dankinPoints ) ;
        Collections.Generic.IList<Revit.DB.XYZ> sortYUsed = SortByYAry( usedPoints ) ;

        // 主筋が奇数の場合
        if ( sortY.Count % 2 != 0 ) {
          // 中央から配置

          // 中央を求める
          double midCount = sortY.Count / 2 ;
          midCount = System.Math.Floor( midCount ) ;
          Revit.DB.XYZ midPoint = sortY[ (int)midCount ] ;

          bool isContains = false ;

          // 配置されているか
          foreach ( Revit.DB.XYZ used in sortYUsed ) {
            // 1mm以下
            if ( _CmpGeometry.Distance2D( midPoint, used ) <= 1 / 304.8 ) {
              isContains = true ;
            }
          }

          // 中央がsortYUsedに含まれていない場合
          if ( isContains == false ) {
            // 中央に配置
            return midPoint ;
          }

          // 対称に配置
          // 対称に配置されていない点
          Revit.DB.XYZ unSymmetryPnt = UnSymmetryPoint( sortY, sortYUsed ) ;

          if ( unSymmetryPnt != null ) {
            return unSymmetryPnt ;
          }
        }

        // 主筋が偶数本または中央と対称に配置されている場合

        // 1段筋の両端も追加
        Collections.Generic.IList<Revit.DB.XYZ> sortPoints = sortYUsed ;
        sortPoints.Add( sortY[ 0 ] ) ;
        sortPoints.Add( sortY[ sortY.Count - 1 ] ) ;
        sortPoints = SortByYAry( sortPoints ) ;

        Revit.DB.XYZ mae = null ;
        Revit.DB.XYZ ato = null ;

        // 間隔の一番広い2点
        MaxDistancePoints( sortPoints, ref mae, ref ato ) ;

        // 中間
        Revit.DB.XYZ mid = ( mae + ato ) / 2 ;

        // 一番近い点
        Revit.DB.XYZ retPoint = PointNear( mid, sortY ) ;

        return retPoint ;
      }
      else {
        // X座標の小さい方から = 左側優先
        Collections.Generic.IList<Revit.DB.XYZ> sortX = SortByXAry( base1dankinPoints ) ;
        Collections.Generic.IList<Revit.DB.XYZ> sortXUsed = SortByXAry( usedPoints ) ;

        Collections.Generic.IList<Revit.DB.XYZ> sortPoints = sortXUsed ;
        sortPoints.Add( sortX[ 0 ] ) ;
        sortPoints.Add( sortX[ sortX.Count - 1 ] ) ;
        sortPoints = SortByXAry( sortPoints ) ;

        // sortXが奇数の場合
        if ( sortX.Count % 2 != 0 ) {
          // 中央を求める
          double midCount = sortX.Count / 2 ;
          midCount = System.Math.Floor( midCount ) ;
          Revit.DB.XYZ midPoint = sortX[ (int)midCount ] ;

          bool isContains = false ;

          // 含まれているか
          foreach ( Revit.DB.XYZ used in sortXUsed ) {
            // 1mm以下
            if ( _CmpGeometry.Distance2D( midPoint, used ) <= 1 / 304.8 ) {
              isContains = true ;
            }
          }

          // 中央がsortXUsedに含まれていない場合
          if ( isContains == false ) {
            // 戻りは中央
            return midPoint ;
          }

          // 対称に配置
          Revit.DB.XYZ unSymmetryPnt = UnSymmetryPoint( sortX, sortXUsed ) ;

          if ( unSymmetryPnt != null ) {
            return unSymmetryPnt ;
          }
        }

        Revit.DB.XYZ mae = null ;
        Revit.DB.XYZ ato = null ;

        MaxDistancePoints( sortPoints, ref mae, ref ato ) ;

        // 中間
        Revit.DB.XYZ mid = ( mae + ato ) / 2 ;

        Revit.DB.XYZ retPoint = PointNear( mid, sortX ) ;

        return retPoint ;
      }
    }

    /// ================================================================================
    /// <summary>角柱配筋</summary>
    ///
    /// <param name="data"    >データテーブル</param>
    /// <param name="rowNum"  >行番号</param>
    /// <param name="center"  >作図中心位置</param>
    /// <param name="isTop"   >柱頭</param>
    /// <param name="rvtUiApp">UIアプリケーション</param>
    ///
    /// <history><p>2013/04/24 Created  GSA,Inc. Ryo Kuroda</p>
    ///           <p>2015/07/22 Modified GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public string CreateRebar_Kaku( System.Data.DataTable data, int rowNum, Revit.DB.XYZ center, bool isTop, Revit.UI.UIApplication rvtUiApp )
    {
      Revit.DB.Document rvtDbDoc = rvtUiApp.ActiveUIDocument.Document ;
      Revit.UI.UIDocument rvtUiDoc = rvtUiApp.ActiveUIDocument ;
      Revit.ApplicationServices.Application rvtSrvcApp = rvtUiApp.Application ;

      string ret = "" ;

      // かぶり厚
      double kaburi_kaku = 0 ;
      double.TryParse( _CmpParameters.ColumnProtectThick, out kaburi_kaku ) ;

      // 鉄筋ファミリ
      Revit.DB.Family rebarFam = null ;
      bool isHaveFam = _CmpElements.GetRebarFamily( ref rebarFam ) ;

      if ( isHaveFam == false ) {
        ret = _CmpAttribute.ResourceText( "IDS_ERR_NOREBARFAMILY" ) ;
        return ret ;
      }

      // 幅
      double x = (double)data.Rows[ rowNum ][ _CmpParameters.DX_Kaku ] ;
      if ( x == 0 ) {
        try {
          x = (int)data.Rows[ rowNum ][ _CmpParameters.DX_Kaku ] ;
        }
        catch {
        }
      }

      // 成
      double y = (double)data.Rows[ rowNum ][ _CmpParameters.DY_Kaku ] ;
      if ( y == 0 ) {
        try {
          y = (int)data.Rows[ rowNum ][ _CmpParameters.DY_Kaku ] ;
        }
        catch {
        }
      }

      // 芯鉄筋径
      string sintekkinkei = (string)data.Rows[ rowNum ][ _CmpParameters.RST_SintekkinKei_Kaku ] ;
      // 芯鉄筋本数
      int coreRebarNum = (int)data.Rows[ rowNum ][ _CmpParameters.CoreRebar_Number_Kaku ] ;
      // 芯鉄筋位置X
      double sintekkinIchiX = (double)data.Rows[ rowNum ][ _CmpParameters.RST_SintekkinIchiX_Kaku ] ;
      // 芯鉄筋位置Y
      double sintekkinIchiY = (double)data.Rows[ rowNum ][ _CmpParameters.RST_SintekkinIchiY_Kaku ] ;
      // 幅止筋径
      string habadomekei = (string)data.Rows[ rowNum ][ _CmpParameters.RST_HabadomekinKei_Kaku ] ;
      // 幅止筋ピッチ
      double habadomePitch = (double)data.Rows[ rowNum ][ _CmpParameters.RST_HabadomekinPitch_Kaku ] ;

      // 主筋太径
      string syukinHutokei = "" ;
      // 主筋X1段太径本数
      int syukinX1HutokeiHonsu = 0 ;
      // 主筋X2段太径本数
      int syukinX2HutokeiHonsu = 0 ;
      // 主筋Y1段太径本数
      int syukinY1HutokeiHonsu = 0 ;
      // 主筋Y2段太径本数
      int syukinY2HutokeiHonsu = 0 ;
      // 主筋細径
      string syukinHosokei = "" ;
      // 主筋X1段細径本数
      int syukinX1HosokeiHonsu = 0 ;
      // 主筋X2段細径本数
      int syukinX2HosokeiHonsu = 0 ;
      // 主筋Y1段細径本数
      int syukinY1HosokeiHonsu = 0 ;
      // 主筋Y2段細径本数
      int syukinY2HosokeiHonsu = 0 ;
      // フープX径
      string hoopXkei = "" ;
      // フープX本数
      int hoopXhonsu = 0 ;
      // フープY本数
      int hoopYhonsu = 0 ;
      // フープピッチ
      double hoopPitch = 0 ;
      // Spacing_XDirectionNumber
      int spacing_XDirectionNumber = 0 ;
      // Spacing_YDirectionNumber
      int spacing_YDirectionNumber = 0 ;
      // 2段筋コーナー配筋フラグ
      int flag2 = 0 ;
      int.TryParse( _CmpParameters.SecondRebarCornerSetFlag, out flag2 ) ;

      if ( isTop == true ) {
        syukinHutokei = (string)data.Rows[ rowNum ][ _CmpParameters.RST_ChutoSyukinHutokei_Kaku ] ;
        syukinX1HutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_ChutoSyukinX1danHutokeiHonsu_Kaku ] ;
        syukinX2HutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_ChutoSyukinX2danHutokeiHonsu_Kaku ] ;
        syukinY1HutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_ChutoSyukinY1danHutokeiHonsu_Kaku ] ;
        syukinY2HutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_ChutoSyukinY2danHutokeiHonsu_Kaku ] ;
        syukinHosokei = (string)data.Rows[ rowNum ][ _CmpParameters.RST_ChutoSyukinHosokei_Kaku ] ;
        syukinX1HosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_ChutoSyukinX1danHosokeiHonsu_Kaku ] ;
        syukinX2HosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_ChutoSyukinX2danHosokeiHonsu_Kaku ] ;
        syukinY1HosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_ChutoSyukinY1danHosokeiHonsu_Kaku ] ;
        syukinY2HosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_ChutoSyukinY2danHosokeiHonsu_Kaku ] ;
        hoopXkei = (string)data.Rows[ rowNum ][ _CmpParameters.RST_ChutoHoopXKei_Kaku ] ;
        hoopXhonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_ChutoHoopXHonsu_Kaku ] ;
        hoopYhonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_ChutoHoopYHonsu_Kaku ] ;
        hoopPitch = (double)data.Rows[ rowNum ][ _CmpParameters.RST_ChutoHoopPitch_Kaku ] ;
        spacing_XDirectionNumber = (int)data.Rows[ rowNum ][ _CmpParameters.Top_Spacing_XDirectionNumber_Kaku ] ;
        spacing_YDirectionNumber = (int)data.Rows[ rowNum ][ _CmpParameters.Top_Spacing_YDirectionNumber_Kaku ] ;
      }
      else {
        syukinHutokei = (string)data.Rows[ rowNum ][ _CmpParameters.RST_ChukyakuSyukinHutokei_Kaku ] ;
        syukinX1HutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_ChukyakuSyukinX1danHutokeiHonsu_Kaku ] ;
        syukinX2HutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_ChukyakuSyukinX2danHutokeiHonsu_Kaku ] ;
        syukinY1HutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_ChukyakuSyukinY1danHutokeiHonsu_Kaku ] ;
        syukinY2HutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_ChukyakuSyukinY2danHutokeiHonsu_Kaku ] ;
        syukinHosokei = (string)data.Rows[ rowNum ][ _CmpParameters.RST_ChukyakuSyukinHosokei_Kaku ] ;
        syukinX1HosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_ChukyakuSyukinX1danHosokeiHonsu_Kaku ] ;
        syukinX2HosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_ChukyakuSyukinX2danHosokeiHonsu_Kaku ] ;
        syukinY1HosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_ChukyakuSyukinY1danHosokeiHonsu_Kaku ] ;
        syukinY2HosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_ChukyakuSyukinY2danHosokeiHonsu_Kaku ] ;
        hoopXkei = (string)data.Rows[ rowNum ][ _CmpParameters.RST_ChukyakuHoopXKei_Kaku ] ;
        hoopXhonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_ChukyakuHoopXHonsu_Kaku ] ;
        hoopYhonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_ChukyakuHoopYHonsu_Kaku ] ;
        hoopPitch = (double)data.Rows[ rowNum ][ _CmpParameters.RST_ChukyakuHoopPitch_Kaku ] ;
        spacing_XDirectionNumber = (int)data.Rows[ rowNum ][ _CmpParameters.Bottom_Spacing_XDirectionNumber_Kaku ] ;
        spacing_YDirectionNumber = (int)data.Rows[ rowNum ][ _CmpParameters.Bottom_Spacing_YDirectionNumber_Kaku ] ;
      }

      // メートルからフィート
      kaburi_kaku /= 304.8 ;
      sintekkinIchiX /= 304.8 ;
      sintekkinIchiY /= 304.8 ;
      habadomePitch /= 304.8 ;
      hoopPitch /= 304.8 ;

      Revit.DB.Transaction trans = new Revit.DB.Transaction( rvtDbDoc ) ;

      // 鉄筋
      Collections.Generic.ISet<Revit.DB.ElementId> famSymSet = rebarFam.GetFamilySymbolIds() ;
      Revit.DB.FamilySymbol famSymSyukinHuto = null ;
      Revit.DB.FamilySymbol famSymSyukinHoso = null ;
      Revit.DB.FamilySymbol famSymSintekkin = null ;

      foreach ( Revit.DB.ElementId eid in famSymSet ) {
        Revit.DB.FamilySymbol fs = rvtDbDoc.GetElement( eid ) as Revit.DB.FamilySymbol ;
        string paramVal = fs.Name ;

        if ( fs.IsActive == false ) {
          trans.Start( "ファミリのアクティブ化" ) ;
          fs.Activate() ;
          trans.Commit() ;
        }

        if ( paramVal == syukinHutokei ) {
          famSymSyukinHuto = fs ;
        }

        if ( paramVal == syukinHosokei ) {
          famSymSyukinHoso = fs ;
        }

        if ( paramVal == sintekkinkei ) {
          famSymSintekkin = fs ;
        }
      }

      //////////////////////////////////////////////////////////////////////////
      bool X1HosoLessX1Huto = true ;
      bool Y1HosoLessY1Huto = true ;

      // 細径が太径本数以上
      // X
      if ( syukinX1HosokeiHonsu >= 1 ) {
        if ( syukinX1HosokeiHonsu >= syukinX1HutokeiHonsu ) {
          X1HosoLessX1Huto = false ;
        }
      }

      // Y
      if ( syukinY1HosokeiHonsu >= 1 ) {
        if ( syukinY1HosokeiHonsu >= syukinY1HutokeiHonsu ) {
          Y1HosoLessY1Huto = false ;
        }
      }

      Revit.DB.View actView = rvtDbDoc.ActiveView ;

      // 主筋太径
      Collections.Generic.IList<Revit.DB.XYZ> syukinHutokeiPoints = new Collections.Generic.List<Revit.DB.XYZ>() ;
      // 主筋細径
      Collections.Generic.IList<Revit.DB.XYZ> syukinHosokeiPoints = new Collections.Generic.List<Revit.DB.XYZ>() ;
      // 芯鉄筋
      Collections.Generic.IList<Revit.DB.XYZ> sintekkinPoints = new Collections.Generic.List<Revit.DB.XYZ>() ;
      // 帯筋
      Revit.DB.CurveArray hoopCrvAry = new Revit.DB.CurveArray() ;
      Revit.DB.Line hoopLine = null ;
      // 幅止筋
      Revit.DB.CurveArray spaceCrvAry = new Revit.DB.CurveArray() ;
      Revit.DB.Line spaceLine = null ;

      #region 全エラー文取得

      bool rectangle = false ;
      if ( x <= 0d || y <= 0d ) {
        ret += _CmpAttribute.ResourceText( "IDS_ERR_COLUMNXORY" ) ;

        rectangle = true ;
      }

      #region ファミリ取得エラー判定

      if ( famSymSyukinHuto == null ) {
        if ( ret != "" ) {
          ret += "\r\n" ;
        }

        ret += _CmpAttribute.ResourceText( "IDS_ERR_HUTO_FAMILY_NOTSET" ) ;
      }

      if ( famSymSyukinHoso == null ) {
        if ( syukinX1HosokeiHonsu > 0 || syukinY1HosokeiHonsu > 0 || syukinX2HosokeiHonsu > 0 || syukinY2HosokeiHonsu > 0 ) {
          if ( ret != "" ) {
            ret += "\r\n" ;
          }

          ret += _CmpAttribute.ResourceText( "IDS_ERR_HOSO_FAMILY_NOTSET" ) ;
        }
      }

      if ( famSymSintekkin == null ) {
        if ( coreRebarNum >= 4 ) {
          if ( ret != "" ) {
            ret += "\r\n" ;
          }

          ret += _CmpAttribute.ResourceText( "IDS_ERR_COREREBAR_FAMILY_NOTSET" ) ;
        }
      }

      #endregion

      #region 1段筋

      // X太径が2本未満
      if ( syukinX1HutokeiHonsu < 2 ) {
        if ( ret != "" ) {
          ret += "\r\n" ;
        }

        ret += _CmpAttribute.ResourceText( "IDS_ERR_LACKX1HUTOKEI" ) ;
      }

      // Y太径が2本未満
      if ( syukinY1HutokeiHonsu < 2 ) {
        if ( ret != "" ) {
          ret += "\r\n" ;
        }

        ret += _CmpAttribute.ResourceText( "IDS_ERR_LACKY1HUTOKEI" ) ;
      }

      // 細径が太径本数以上
      // X
      if ( syukinX1HosokeiHonsu >= 1 ) {
        if ( syukinX1HosokeiHonsu >= syukinX1HutokeiHonsu && X1HosoLessX1Huto ) {
          if ( ret != "" ) {
            ret += "\r\n" ;
          }

          ret += _CmpAttribute.ResourceText( "IDS_ERR_X1HUTO_ORUNDER_HOSO" ) ;
        }
      }

      // Y
      if ( syukinY1HosokeiHonsu >= 1 ) {
        if ( syukinY1HosokeiHonsu >= syukinY1HutokeiHonsu && Y1HosoLessY1Huto ) {
          if ( ret != "" ) {
            ret += "\r\n" ;
          }

          ret += _CmpAttribute.ResourceText( "IDS_ERR_Y1HUTO_ORUNDER_HOSO" ) ;
        }
      }

      #endregion

      #region 2段筋

      // X太径がある
      if ( syukinX2HutokeiHonsu > 0 ) {
        // 2本未満
        if ( syukinX2HutokeiHonsu < 2 ) {
          if ( ret != "" ) {
            ret += "\r\n" ;
          }

          ret += _CmpAttribute.ResourceText( "IDS_ERR_X2HUTO" ) ;
        }

        // 直交方向1段筋太径4本未満
        if ( syukinY1HutokeiHonsu < 4 ) {
          if ( ret != "" ) {
            ret += "\r\n" ;
          }

          ret += _CmpAttribute.ResourceText( "IDS_ERR_X2CROSSY1" ) ;
        }

        // 同一方向1段筋太径本数より多い
        if ( syukinX1HutokeiHonsu < syukinX2HutokeiHonsu ) {
          if ( ret != "" ) {
            ret += "\r\n" ;
          }

          ret += _CmpAttribute.ResourceText( "IDS_ERR_X2OVERX1HUTO" ) ;
        }

        // フラグが立っていない場合
        if ( flag2 == 0 ) {
          // 直交方向2段筋がない場合、1段筋をもとに配筋
          // 直交方向2段筋がある場合、フラグが立っていないためコーナーに配筋できない = 2段筋本数は(1段筋本数 - 2)以下

          // X2段筋
          // 直交方向2段筋あり
          if ( syukinY2HutokeiHonsu > 0 ) {
            // 1段筋本数-2以下または2段筋本数は2本以下
            if ( ( syukinX2HutokeiHonsu <= syukinX1HutokeiHonsu - 2 ) || syukinX2HutokeiHonsu <= 2 ) {
              // コーナー部分を除き配筋
            }
            else {
              if ( syukinX2HutokeiHonsu <= syukinX1HutokeiHonsu ) {
                if ( syukinX1HosokeiHonsu >= 1 ) {
                  if ( syukinX1HosokeiHonsu >= syukinX1HutokeiHonsu ) {
                    X1HosoLessX1Huto = false ;
                  }
                }

                // Y
                if ( syukinY1HosokeiHonsu >= 1 ) {
                  if ( syukinY1HosokeiHonsu >= syukinY1HutokeiHonsu ) {
                    Y1HosoLessY1Huto = false ;
                  }
                }

                if ( X1HosoLessX1Huto == true && Y1HosoLessY1Huto == true ) {
                  // 配筋不可
                  if ( ret != "" ) {
                    ret += "\r\n" ;
                  }

                  ret += _CmpAttribute.ResourceText( "IDS_ERR_X2OVERX1HUTO_NOFLAG" ) ;
                }
              }
            }
          }
          // 直交方向2段筋なし
          else {
            // X1段筋をもとに配筋
          }

          //if (syukinX1HutokeiHonsu - 2 < syukinX2HutokeiHonsu)
          //{
          //  if (ret != "")
          //  {
          //    ret += "\r\n";
          //  }

          //  ret += _CmpAttribute.ResourceText("IDS_ERR_X2OVERX1HUTO_NOFLAG");
          //}
        }

        // フラグが立っている場合
        if ( flag2 == 1 ) {
          // X2段筋
          // 直交方向2段筋あり
          if ( syukinY2HutokeiHonsu > 0 ) {
            // 2段筋本数4本以上かつ直交2段筋本数4本以上
            if ( syukinX2HutokeiHonsu >= 4 && syukinY2HutokeiHonsu >= 4 ) {
              // コーナー配筋を含む配筋
            }
            else {
              // 2段筋本数4本以上
              if ( syukinX2HutokeiHonsu >= 4 ) {
                // 2段本数は1段本数-2以下
                if ( ( syukinX2HutokeiHonsu <= syukinX1HutokeiHonsu - 2 ) ) {
                  // コーナーを除き配筋
                }
                else {
                  // 配筋不可
                  if ( ret != "" ) {
                    ret += "\r\n" ;
                  }

                  ret += _CmpAttribute.ResourceText( "IDS_ERR_X2" ) ;
                }
              }
              else {
                if ( syukinX2HutokeiHonsu == 3 ) {
                  // 1段筋本数5本以上
                  if ( syukinX1HutokeiHonsu >= 5 ) {
                    // コーナーを除き配筋
                  }
                  else {
                    // 配筋不可
                    if ( ret != "" ) {
                      ret += "\r\n" ;
                    }

                    ret += _CmpAttribute.ResourceText( "IDS_ERR_X2" ) ;
                  }
                }
              }
            }
          }
          // 直交方向2段筋なし
          else {
            // X1段筋をもとに配筋
          }
        }

        if ( syukinX2HutokeiHonsu == 3 && syukinX1HutokeiHonsu >= syukinX2HutokeiHonsu && syukinX1HosokeiHonsu >= syukinX1HutokeiHonsu && syukinY2HutokeiHonsu > 0 ) {
          // Y2太径配筋不可
          if ( ret != "" ) {
            ret += "\r\n" ;
          }

          ret += _CmpAttribute.ResourceText( "IDS_ERR_Y2" ) ;
        }
      }

      // X細径がある
      if ( syukinX2HosokeiHonsu > 0 ) {
        // 同一方向2段筋太径本数2本未満
        if ( syukinX2HutokeiHonsu < 2 ) {
          if ( ret != "" ) {
            ret += "\r\n" ;
          }

          ret += _CmpAttribute.ResourceText( "IDS_ERR_X2HOSO_X2HUTO_UNDER2" ) ;
        }

        // 同一方向2段筋太径本以上
        if ( syukinX2HosokeiHonsu >= syukinX2HutokeiHonsu ) {
          if ( syukinX1HosokeiHonsu >= 1 ) {
            if ( syukinX1HosokeiHonsu >= syukinX1HutokeiHonsu ) {
              X1HosoLessX1Huto = false ;
            }
          }

          // Y
          if ( syukinY1HosokeiHonsu >= 1 ) {
            if ( syukinY1HosokeiHonsu >= syukinY1HutokeiHonsu ) {
              Y1HosoLessY1Huto = false ;
            }
          }

          if ( X1HosoLessX1Huto == true && Y1HosoLessY1Huto == true ) {
            if ( ret != "" ) {
              ret += "\r\n" ;
            }

            ret += _CmpAttribute.ResourceText( "IDS_ERR_X2HUTO_ORUNDER_HOSO" ) ;
          }
        }

        // 直交方向1段筋太径本数4本未満
        if ( syukinY1HutokeiHonsu < 4 ) {
          //if (ret != "")
          //{
          //  ret += "\r\n";
          //}

          //ret += _CmpAttribute.ResourceText("IDS_ERR_X2HOSO_Y1HUTO_UNDER4");
        }
      }

      // 合計
      if ( flag2 == 0 ) {
        if ( syukinX2HosokeiHonsu > 0 ) {
          // 直交方向
          if ( syukinY2HutokeiHonsu > 0 ) {
            if ( syukinX1HutokeiHonsu + syukinX1HosokeiHonsu - 2 < syukinX2HutokeiHonsu + syukinX2HosokeiHonsu ) {
              if ( ret != "" ) {
                ret += "\r\n" ;
              }

              ret += _CmpAttribute.ResourceText( "IDS_ERR_X2OVERX1_NOFLAG" ) ;
            }
          }
          else {
            if ( syukinX1HutokeiHonsu + syukinX1HosokeiHonsu < syukinX2HutokeiHonsu + syukinX2HosokeiHonsu ) {
              if ( ret != "" ) {
                ret += "\r\n" ;
              }

              ret += _CmpAttribute.ResourceText( "IDS_ERR_X2_OVER_X1" ) ;
            }
          }

          if ( X1HosoLessX1Huto == false || Y1HosoLessY1Huto == false ) {
            if ( syukinX2HosokeiHonsu > syukinX1HosokeiHonsu ) {
              if ( ret != "" ) {
                ret += "\r\n" ;
              }

              ret += _CmpAttribute.ResourceText( "IDS_ERR_X2_OVER_X1_HOSO" ) ;
            }
          }
        }
      }

      if ( flag2 == 1 ) {
        if ( syukinX1HutokeiHonsu + syukinX1HosokeiHonsu < syukinX2HutokeiHonsu + syukinX2HosokeiHonsu ) {
          if ( ret != "" ) {
            ret += "\r\n" ;
          }

          ret += _CmpAttribute.ResourceText( "IDS_ERR_X2_OVER_X1" ) ;
        }

        if ( syukinX2HosokeiHonsu > 0 ) {
          if ( syukinY2HutokeiHonsu >= 2 ) {
            if ( syukinX1HutokeiHonsu + syukinX1HosokeiHonsu < 5 ) {
              if ( ret != "" ) {
                ret += "\r\n" ;
              }

              ret += _CmpAttribute.ResourceText( "IDS_ERR_X2HOSO_X1HUSOKU" ) ;
            }
          }
        }

        // 直交方向2段あり
        if ( syukinY2HutokeiHonsu > 0 ) {
          // 同一方向2段筋太径本数4本未満
          if ( syukinX2HutokeiHonsu < 4 ) {
            // コーナー部分への配筋が必要
            if ( syukinX1HutokeiHonsu + syukinX1HosokeiHonsu - 2 < syukinX2HutokeiHonsu + syukinX2HosokeiHonsu ) {
              //if (ret != "")
              //{
              //  ret += "\r\n";
              //}

              if ( ret == "" ) {
                ret += _CmpAttribute.ResourceText( "IDS_ERR_X2" ) ;
              }
            }
          }
        }
      }

      // Y太径
      if ( syukinY2HutokeiHonsu > 0 ) {
        // 2本未満
        if ( syukinY2HutokeiHonsu < 2 ) {
          if ( ret != "" ) {
            ret += "\r\n" ;
          }

          ret += _CmpAttribute.ResourceText( "IDS_ERR_Y2HUTO" ) ;
        }

        // 直交方向1段筋太径4本未満
        if ( syukinX1HutokeiHonsu < 4 ) {
          if ( ret != "" ) {
            ret += "\r\n" ;
          }

          ret += _CmpAttribute.ResourceText( "IDS_ERR_Y2CROSSX1" ) ;
        }

        // 直交方向2段筋太径4本未満
        if ( syukinX2HutokeiHonsu < 4 ) {
          //if (ret != "")
          //{
          //  ret += "\r\n";
          //}

          //ret += _CmpAttribute.ResourceText("IDS_ERR_Y2CROSSX2");
        }

        // 同一方向1段筋太径本数より多い
        if ( syukinY1HutokeiHonsu < syukinY2HutokeiHonsu ) {
          if ( ret != "" ) {
            ret += "\r\n" ;
          }

          ret += _CmpAttribute.ResourceText( "IDS_ERR_Y2OVERY1HUTO" ) ;
        }

        // フラグが立っていない場合
        if ( flag2 == 0 ) {
          // 直交方向2段筋がない場合、1段筋をもとに配筋
          // 直交方向2段筋がある場合、フラグが立っていないためコーナーに配筋できない = 2段筋本数は(1段筋本数 - 2)以下

          // Y2段筋
          // 直交方向2段筋あり
          if ( syukinX2HutokeiHonsu > 0 ) {
            // 1段筋本数-2以下または2段筋本数は2本以下
            if ( ( syukinY2HutokeiHonsu <= syukinY1HutokeiHonsu - 2 ) || syukinY2HutokeiHonsu <= 2 ) {
              // コーナー部分を除き配筋
            }
            else {
              if ( syukinX1HosokeiHonsu >= 1 ) {
                if ( syukinX1HosokeiHonsu >= syukinX1HutokeiHonsu ) {
                  X1HosoLessX1Huto = false ;
                }
              }

              // Y
              if ( syukinY1HosokeiHonsu >= 1 ) {
                if ( syukinY1HosokeiHonsu >= syukinY1HutokeiHonsu ) {
                  Y1HosoLessY1Huto = false ;
                }
              }

              if ( X1HosoLessX1Huto == true && Y1HosoLessY1Huto == true ) {
                if ( syukinY2HutokeiHonsu <= syukinY1HutokeiHonsu ) {
                  // 配筋不可
                  if ( ret != "" ) {
                    ret += "\r\n" ;
                  }

                  ret += _CmpAttribute.ResourceText( "IDS_ERR_Y2OVERY1HUTO_NOFLAG" ) ;
                }
              }
            }
          }
          // 直交方向2段筋なし
          else {
            // Y1段筋をもとに配筋
          }

          //if (syukinY1HutokeiHonsu - 2 < syukinY2HutokeiHonsu)
          //{
          //  if (ret != "")
          //  {
          //    ret += "\r\n";
          //  }

          //  ret += _CmpAttribute.ResourceText("IDS_ERR_Y2OVERY1HUTO_NOFLAG");
          //}
        }

        // フラグが立っている場合
        if ( flag2 == 1 ) {
          // Y2段筋
          // 直交方向2段筋あり
          if ( syukinX2HutokeiHonsu > 0 ) {
            // 2段筋本数4本以上かつ直交2段筋本数4本以上
            if ( syukinY2HutokeiHonsu >= 4 && syukinX2HutokeiHonsu >= 4 ) {
              // コーナー配筋を含む配筋
            }
            else {
              // 2段筋本数4本以上
              if ( syukinY2HutokeiHonsu >= 4 ) {
                // 2段本数は1段本数-2以下
                if ( ( syukinY2HutokeiHonsu <= syukinY1HutokeiHonsu - 2 ) ) {
                  // コーナーを除き配筋
                }
                else {
                  // 配筋不可
                  if ( ret != "" ) {
                    ret += "\r\n" ;
                  }

                  ret += _CmpAttribute.ResourceText( "IDS_ERR_Y2" ) ;
                }
              }
              else {
                if ( syukinY2HutokeiHonsu == 3 ) {
                  // 1段筋本数5本以上
                  if ( syukinY1HutokeiHonsu >= 5 ) {
                    // コーナーを除き配筋
                  }
                  else {
                    // 配筋不可
                    if ( ret != "" ) {
                      ret += "\r\n" ;
                    }

                    ret += _CmpAttribute.ResourceText( "IDS_ERR_Y2" ) ;
                  }
                }
              }
            }
          }
          // 直交方向2段筋なし
          else {
            // Y1段筋をもとに配筋
          }

          //// コーナー配筋するとき直交方向が4本未満
          //if (syukinY2HutokeiHonsu > 3 && syukinX2HutokeiHonsu < 4)
          //{
          //  if (ret != "")
          //  {
          //    ret += "\r\n";
          //  }

          //  ret += _CmpAttribute.ResourceText("IDS_ERR_Y2HUTO_X2HUTO_UNDER4");
          //}
          //// 2段筋が3本のときは1段筋は5本以上必要
          //if (syukinY2HutokeiHonsu + syukinY2HosokeiHonsu == 3 && syukinY1HutokeiHonsu + syukinY1HosokeiHonsu < 5)
          //{
          //  if (ret != "")
          //  {
          //    ret += "\r\n";
          //  }

          //  ret += _CmpAttribute.ResourceText("IDS_ERR_IFFLAG2_Y2IS3_Y1UNDER5");
          //}
        }

        if ( syukinY2HutokeiHonsu == 3 && syukinY1HutokeiHonsu >= syukinY2HutokeiHonsu && syukinY1HosokeiHonsu >= syukinY1HutokeiHonsu && syukinX1HutokeiHonsu > 0 ) {
          // X2太径配筋不可
          if ( ret != "" ) {
            ret += "\r\n" ;
          }

          ret += _CmpAttribute.ResourceText( "IDS_ERR_X2" ) ;
        }
      }

      // Y細径
      if ( syukinY2HosokeiHonsu > 0 ) {
        // 同一方向2段筋太径本数2本未満
        if ( syukinY2HutokeiHonsu < 2 ) {
          if ( ret != "" ) {
            ret += "\r\n" ;
          }

          ret += _CmpAttribute.ResourceText( "IDS_ERR_Y2HOSO_Y2HUTO_UNDER2" ) ;
        }

        // 同一方向2段筋太径本以上
        if ( syukinY2HosokeiHonsu >= syukinY2HutokeiHonsu ) {
          if ( syukinX1HosokeiHonsu >= 1 ) {
            if ( syukinX1HosokeiHonsu >= syukinX1HutokeiHonsu ) {
              X1HosoLessX1Huto = false ;
            }
          }

          // Y
          if ( syukinY1HosokeiHonsu >= 1 ) {
            if ( syukinY1HosokeiHonsu >= syukinY1HutokeiHonsu ) {
              Y1HosoLessY1Huto = false ;
            }
          }

          if ( X1HosoLessX1Huto == true && Y1HosoLessY1Huto == true ) {
            if ( ret != "" ) {
              ret += "\r\n" ;
            }

            ret += _CmpAttribute.ResourceText( "IDS_ERR_Y2HUTO_ORUNDER_HOSO" ) ;
          }
        }

        // 直交方向1段筋太径本数4本未満
        if ( syukinX1HutokeiHonsu < 4 ) {
          //if (ret != "")
          //{
          //  ret += "\r\n";
          //}

          //ret += _CmpAttribute.ResourceText("IDS_ERR_Y2HOSO_X1HUTO_UNDER4");
        }
      }

      // 合計
      if ( flag2 == 0 ) {
        if ( syukinY2HosokeiHonsu > 0 ) {
          // 直交方向
          if ( syukinX2HutokeiHonsu > 0 ) {
            if ( syukinY1HutokeiHonsu + syukinY1HosokeiHonsu - 2 < syukinY2HutokeiHonsu + syukinY2HosokeiHonsu ) {
              if ( ret != "" ) {
                ret += "\r\n" ;
              }

              ret += _CmpAttribute.ResourceText( "IDS_ERR_Y2OVERY1_NOFLAG" ) ;
            }
          }
          else {
            if ( syukinY1HutokeiHonsu + syukinY1HosokeiHonsu < syukinY2HutokeiHonsu + syukinY2HosokeiHonsu ) {
              if ( ret != "" ) {
                ret += "\r\n" ;
              }

              ret += _CmpAttribute.ResourceText( "IDS_ERR_Y2_OVER_Y1" ) ;
            }
          }
        }
      }

      if ( flag2 == 1 ) {
        if ( syukinY1HutokeiHonsu + syukinY1HosokeiHonsu < syukinY2HutokeiHonsu + syukinY2HosokeiHonsu ) {
          if ( ret != "" ) {
            ret += "\r\n" ;
          }

          ret += _CmpAttribute.ResourceText( "IDS_ERR_Y2_OVER_Y1" ) ;
        }

        if ( syukinY2HosokeiHonsu > 0 ) {
          if ( syukinX2HutokeiHonsu >= 2 ) {
            if ( syukinY1HutokeiHonsu + syukinY1HosokeiHonsu < 5 ) {
              if ( ret != "" ) {
                ret += "\r\n" ;
              }

              ret += _CmpAttribute.ResourceText( "IDS_ERR_Y2HOSO_Y1HUSOKU" ) ;
            }
          }
        }

        // 直交方向2段あり
        if ( syukinX2HutokeiHonsu > 0 ) {
          // 同一方向2段筋太径本数4本未満
          if ( syukinY2HutokeiHonsu < 4 ) {
            // コーナー部分への配筋が必要
            if ( syukinY1HutokeiHonsu + syukinY1HosokeiHonsu - 2 < syukinY2HutokeiHonsu + syukinY2HosokeiHonsu ) {
              //if (ret != "")
              //{
              //  ret += "\r\n";
              //}

              if ( ret == "" ) {
                ret += _CmpAttribute.ResourceText( "IDS_ERR_Y2" ) ;
              }
              else if ( ret == _CmpAttribute.ResourceText( "IDS_ERR_X2" ) ) {
                ret += "\r\n" + _CmpAttribute.ResourceText( "IDS_ERR_Y2" ) ;
              }
            }
          }
        }
      }

      #endregion

      #region 帯筋 エラー判定

      // X方向
      if ( syukinX1HutokeiHonsu >= 2 && ( hoopXhonsu < 2 || syukinX1HutokeiHonsu + syukinX1HosokeiHonsu < hoopXhonsu ) ) {
        if ( ret != "" ) {
          ret += "\r\n" ;
        }

        ret += _CmpAttribute.ResourceText( "IDS_ERR_HOOPXNUM" ) ;
      }

      // Y方向
      if ( syukinY1HutokeiHonsu >= 2 && ( hoopYhonsu < 2 || syukinY1HutokeiHonsu + syukinY1HosokeiHonsu < hoopYhonsu ) ) {
        if ( ret != "" ) {
          ret += "\r\n" ;
        }

        ret += _CmpAttribute.ResourceText( "IDS_ERR_HOOPYNUM" ) ;
      }

      if ( syukinX1HutokeiHonsu >= 2 && hoopXkei == "" ) {
        if ( ret != "" ) {
          ret += "\r\n" ;
        }

        ret += _CmpAttribute.ResourceText( "IDS_ERR_HOOPKEI" ) ;
      }

      if ( syukinX1HutokeiHonsu >= 2 && hoopPitch == 0 ) {
        if ( ret != "" ) {
          ret += "\r\n" ;
        }

        ret += _CmpAttribute.ResourceText( "IDS_ERR_HOOPPITCH" ) ;
      }

      #endregion

      #region 幅止筋 エラー判定

      //if (spacing_XDirectionNumber > 0 && spacing_XDirectionNumber > syukinX1HutokeiHonsu + syukinX1HosokeiHonsu - 2 && syukinX1HutokeiHonsu + syukinX1HosokeiHonsu > 2)
      //{
      //  if (ret != "")
      //  {
      //    ret += "\r\n";
      //  }

      //  ret += _CmpAttribute.ResourceText("IDS_ERR_SPACINGXNUM");
      //}
      //if (spacing_YDirectionNumber > 0 && spacing_YDirectionNumber >= syukinY1HutokeiHonsu + syukinY1HosokeiHonsu - 2 && syukinY1HutokeiHonsu + syukinY1HosokeiHonsu > 2)
      //{
      //  if (ret != "")
      //  {
      //    ret += "\r\n";
      //  }

      //  ret += _CmpAttribute.ResourceText("IDS_ERR_SPACINGYNUM");
      //}
      if ( spacing_XDirectionNumber > 0 && spacing_XDirectionNumber + hoopXhonsu > syukinX1HutokeiHonsu + syukinX1HosokeiHonsu ) {
        if ( ret != "" ) {
          ret += "\r\n" ;
        }

        ret += _CmpAttribute.ResourceText( "IDS_ERR_SPACINGXNUM2" ) ;
      }

      if ( spacing_YDirectionNumber > 0 && spacing_YDirectionNumber + hoopYhonsu > syukinY1HutokeiHonsu + syukinY1HosokeiHonsu ) {
        if ( ret != "" ) {
          ret += "\r\n" ;
        }

        ret += _CmpAttribute.ResourceText( "IDS_ERR_SPACINGYNUM2" ) ;
      }

      //if (habadomekei == "")
      //{
      //  if (ret != "")
      //  {
      //    ret += "\r\n";
      //  }

      //  ret += _CmpAttribute.ResourceText("IDS_ERR_SPACINGKEI");
      //}
      //if (habadomePitch == 0)
      //{
      //  if (ret != "")
      //  {
      //    ret += "\r\n";
      //  }

      //  ret += _CmpAttribute.ResourceText("IDS_ERR_SPACINGPITCH");
      //}

      #endregion

      #region 芯鉄筋 エラー判定

      if ( coreRebarNum > 0 ) {
        if ( coreRebarNum % 2 != 0 || coreRebarNum < 4 ) {
          if ( ret != "" ) {
            ret += "\r\n" ;
          }

          ret += _CmpAttribute.ResourceText( "IDS_ERR_COREREBARNUM" ) ;
        }

        if ( syukinX1HosokeiHonsu + syukinX1HutokeiHonsu < 4 && sintekkinIchiY == 0d ) {
          if ( ret != "" ) {
            ret += "\r\n" ;
          }

          ret += _CmpAttribute.ResourceText( "IDS_ERR_SYUKINX1SHORTAGE" ) ;
        }

        if ( syukinY1HosokeiHonsu + syukinY1HutokeiHonsu < 4 && sintekkinIchiX == 0d ) {
          if ( ret != "" ) {
            ret += "\r\n" ;
          }

          ret += _CmpAttribute.ResourceText( "IDS_ERR_SYUKINY1SHORTAGE" ) ;
        }

        // 4または5本
        if ( syukinX1HutokeiHonsu + syukinX1HosokeiHonsu == 4 || syukinX1HutokeiHonsu + syukinX1HosokeiHonsu == 5 ) {
          // 位置指定なし
          if ( sintekkinIchiY == 0d ) {
            // 直交方向2段筋あり
            if ( syukinY2HutokeiHonsu > 1 ) {
              if ( ret != "" ) {
                ret += "\r\n" ;
              }

              ret += _CmpAttribute.ResourceText( "IDS_ERR_COREREBARINY2" ) ;
            }
          }
        }

        if ( syukinY1HutokeiHonsu + syukinY1HosokeiHonsu == 4 || syukinY1HutokeiHonsu + syukinY1HosokeiHonsu == 5 ) {
          // 位置指定なし
          if ( sintekkinIchiX == 0d ) {
            // 直交方向2段筋あり
            if ( syukinX2HutokeiHonsu > 1 ) {
              if ( ret != "" ) {
                ret += "\r\n" ;
              }

              ret += _CmpAttribute.ResourceText( "IDS_ERR_COREREBARINX2" ) ;
            }
          }
        }
      }
      else if ( sintekkinkei != "" ) {
        if ( ret != "" ) {
          ret += "\r\n" ;
        }

        ret += _CmpAttribute.ResourceText( "IDS_ERR_0COREREBAR" ) ;
      }

      #endregion

      #endregion

      // 断面形状
      if ( rectangle == true ) {
        return ret ;
      }

      if ( famSymSyukinHuto == null ) {
        return ret ;
      }

      if ( famSymSyukinHoso == null ) {
        if ( syukinX1HosokeiHonsu > 0 || syukinY1HosokeiHonsu > 0 || syukinX2HosokeiHonsu > 0 || syukinY2HosokeiHonsu > 0 ) {
          return ret ;
        }
      }

      // 鉄筋記号幅
      double diaSyukinHuto = 0 ;
      double diaSyukinHoso = 0 ;
      double diaSintekkin = 0 ;

      trans.Start( "鉄筋記号幅" ) ;
      Revit.DB.FamilyInstance famInsDammyRebar = rvtDbDoc.Create.NewFamilyInstance( new Revit.DB.XYZ(), famSymSyukinHuto, actView ) ;
      trans.Commit() ;

      if ( famSymSyukinHuto != null ) {
        trans.Start( "鉄筋記号幅" ) ;
        famInsDammyRebar.Symbol = famSymSyukinHuto ;
        trans.Commit() ;
        diaSyukinHuto = famInsDammyRebar.Symbol.LookupParameter( _CmpAttribute.ResourceText( "IDS_TXT_ZUMENDSIZE" ) ).AsDouble() ;
        _SyukinHutokei = diaSyukinHuto ;
      }

      if ( famSymSyukinHoso != null ) {
        trans.Start( "鉄筋記号幅" ) ;
        famInsDammyRebar.Symbol = famSymSyukinHoso ;
        trans.Commit() ;
        diaSyukinHoso = famInsDammyRebar.Symbol.LookupParameter( _CmpAttribute.ResourceText( "IDS_TXT_ZUMENDSIZE" ) ).AsDouble() ;
      }

      if ( famSymSintekkin != null ) {
        trans.Start( "鉄筋記号幅" ) ;
        famInsDammyRebar.Symbol = famSymSintekkin ;
        trans.Commit() ;
        diaSintekkin = famInsDammyRebar.Symbol.LookupParameter( _CmpAttribute.ResourceText( "IDS_TXT_ZUMENDSIZE" ) ).AsDouble() ;
      }

      trans.Start( "ダミー削除" ) ;
      rvtDbDoc.Delete( famInsDammyRebar.Id ) ;
      trans.Commit() ;

      // ----- 配筋開始 -----

      // かぶり厚分内側の頂点
      Revit.DB.XYZ leftTop = null ;
      Revit.DB.XYZ leftBottom = null ;
      Revit.DB.XYZ rightTop = null ;
      Revit.DB.XYZ rightBottom = null ;
      _CmpGeometry.RectanglePointsInsideKaburi( center, x, y, kaburi_kaku, ref leftTop, ref leftBottom, ref rightTop, ref rightBottom ) ;

      #region 1段筋エラー判定

      // 1段筋
      // 太径が2本未満
      if ( syukinX1HutokeiHonsu < 2 || syukinY1HutokeiHonsu < 2 ) {
        return ret ;
      }

      #endregion

      #region X方向・Y方向の1段筋の細径本数 < 1段筋の太径本数

      if ( X1HosoLessX1Huto && Y1HosoLessY1Huto ) {
        // 主筋X1段筋座標
        Collections.Generic.IList<Revit.DB.XYZ> syukinX1Points = new Collections.Generic.List<Revit.DB.XYZ>() ;
        // 主筋Y1段筋座標
        Collections.Generic.IList<Revit.DB.XYZ> syukinY1Points = new Collections.Generic.List<Revit.DB.XYZ>() ;

        // 主筋X1段筋太径座標
        Collections.Generic.IList<Revit.DB.XYZ> syukinX1HutoPoints = new Collections.Generic.List<Revit.DB.XYZ>() ;
        // 主筋X1段筋細径座標
        Collections.Generic.IList<Revit.DB.XYZ> syukinX1HosoPoints = new Collections.Generic.List<Revit.DB.XYZ>() ;
        // 主筋Y1段筋太径座標
        Collections.Generic.IList<Revit.DB.XYZ> syukinY1HutoPoints = new Collections.Generic.List<Revit.DB.XYZ>() ;
        // 主筋Y1段筋細径座標
        Collections.Generic.IList<Revit.DB.XYZ> syukinY1HosoPoints = new Collections.Generic.List<Revit.DB.XYZ>() ;

        // 1段筋の配置
        // 1段筋四隅
        Revit.DB.XYZ leftTopRebar = leftTop + new Revit.DB.XYZ( diaSyukinHuto / 2, -( diaSyukinHuto / 2 ), 0 ) ;
        Revit.DB.XYZ leftBottomRebar = leftBottom + new Revit.DB.XYZ( diaSyukinHuto / 2, diaSyukinHuto / 2, 0 ) ;
        Revit.DB.XYZ rightTopRebar = rightTop + new Revit.DB.XYZ( -( diaSyukinHuto / 2 ), -( diaSyukinHuto / 2 ), 0 ) ;
        Revit.DB.XYZ rightBottomRebar = rightBottom + new Revit.DB.XYZ( -( diaSyukinHuto / 2 ), diaSyukinHuto / 2, 0 ) ;
        syukinHutokeiPoints.Add( leftTopRebar ) ;
        syukinHutokeiPoints.Add( leftBottomRebar ) ;
        syukinHutokeiPoints.Add( rightTopRebar ) ;
        syukinHutokeiPoints.Add( rightBottomRebar ) ;

        syukinX1Points.Add( leftTopRebar ) ;
        syukinX1Points.Add( leftBottomRebar ) ;

        syukinY1Points.Add( leftTopRebar ) ;
        syukinY1Points.Add( rightTopRebar ) ;

        // 2段筋端部兼1段筋
        Revit.DB.XYZ y2LeftTopPoint = null ;
        Revit.DB.XYZ y2RightTopPoint = null ;
        Revit.DB.XYZ y2LeftBottomPoint = null ;
        Revit.DB.XYZ y2RightBottomPoint = null ;
        Revit.DB.XYZ x2LeftTopPoint = null ;
        Revit.DB.XYZ x2RightTopPoint = null ;
        Revit.DB.XYZ x2LeftBottomPoint = null ;
        Revit.DB.XYZ x2RightBottomPoint = null ;

        // 1段筋寄せ筋判定
        bool xYosekin = false ;
        bool yYosekin = false ;
        _X2ndRebarIs = false ;
        _Y2ndRebarIs = false ;

        // 2本以上
        // 直交方向1段筋太径4本以上
        if ( syukinX2HutokeiHonsu >= 2 && syukinY1HutokeiHonsu >= 4 ) {
          // 同一方向1段筋太径本数以下
          if ( syukinX1HutokeiHonsu >= syukinX2HutokeiHonsu ) {
            yYosekin = true ;
            _Y2ndRebarIs = true ;
          }
        }

        if ( syukinY2HutokeiHonsu >= 2 && syukinX1HutokeiHonsu >= 4 ) {
          if ( syukinY1HutokeiHonsu >= syukinY2HutokeiHonsu ) {
            xYosekin = true ;
            _X2ndRebarIs = true ;
          }
        }

        // 1段筋配置太径細径記録(四隅を含む)
        // 0 = 太径、1 = 細径
        Collections.Generic.IList<int> x1RebarOrder = RebarOrder_Column( syukinX1HutokeiHonsu, syukinX1HosokeiHonsu, _X2ndRebarIs ) ;
        Collections.Generic.IList<int> y1RebarOrder = RebarOrder_Column( syukinY1HutokeiHonsu, syukinY1HosokeiHonsu, _Y2ndRebarIs ) ;

        // X1段筋

        #region

        // X1段配筋本数
        int x1RebarCount = syukinX1HutokeiHonsu + syukinX1HosokeiHonsu ;
        // X1段筋記号中心間距離
        double x1RebarDistance = ( leftTopRebar.Y - leftBottomRebar.Y ) / ( x1RebarCount - 1 ) ;
        // X1段筋中間距離
        double x1centerDistance = ( leftTopRebar.Y - leftBottomRebar.Y ) / 2 ;

        // 太径の1.5倍
        double diaSyukinHuto1_5 = diaSyukinHuto * 1.5 ;

        // Y2段筋兼X1段筋移動量
        double moveYValue = x1RebarDistance - diaSyukinHuto1_5 ;

        // X1段筋合計位置
        double x1SumDistance = 0 ;

        // 配筋
        for ( int i = 0 ; i < x1RebarOrder.Count ; ++i ) {
          int now = x1RebarOrder[ i ] ;
          Revit.DB.XYZ point = null ;

          // 四隅
          if ( i == 0 || i == x1RebarOrder.Count - 1 ) {
            continue ;
          }
          // 最初の1本(寄筋かもしれない鉄筋)
          else if ( i == 1 ) {
            // 太径かつ寄筋
            if ( now == 0 && xYosekin == true ) {
              x1SumDistance += diaSyukinHuto1_5 ;
              x1RebarDistance = ( _CmpGeometry.Distance2D( leftTopRebar, leftBottomRebar ) - diaSyukinHuto1_5 * 2 ) / ( x1RebarCount - 3 ) ;

              _X2ndRebarIs = true ;
              _X2ndRebarDistance = diaSyukinHuto * 2.5 ;
            }
            else {
              _X2ndRebarIs = false ;
              x1SumDistance += x1RebarDistance ;
            }

            _X2ndRebarDistance = diaSyukinHuto * 2.5 ;

            point = leftBottomRebar + new Revit.DB.XYZ( 0, x1SumDistance, 0 ) ;
            if ( now == 0 ) {
              syukinHutokeiPoints.Add( point ) ;
              y2LeftBottomPoint = point ;

              syukinX1HutoPoints.Add( point ) ;
            }

            if ( now == 1 ) {
              syukinHosokeiPoints.Add( point ) ;

              syukinX1HosoPoints.Add( point ) ;
            }

            syukinX1Points.Add( point ) ;

            point = rightBottomRebar + new Revit.DB.XYZ( 0, x1SumDistance, 0 ) ;
            if ( now == 0 ) {
              syukinHutokeiPoints.Add( point ) ;
              y2RightBottomPoint = point ;
            }

            if ( now == 1 ) {
              syukinHosokeiPoints.Add( point ) ;
            }
          }
          // 最後の1本(寄筋かもしれない鉄筋)
          else if ( i == x1RebarOrder.Count - 2 ) {
            x1SumDistance += x1RebarDistance ;

            point = leftBottomRebar + new Revit.DB.XYZ( 0, x1SumDistance, 0 ) ;
            if ( now == 0 ) {
              syukinHutokeiPoints.Add( point ) ;
              y2LeftTopPoint = point ;

              syukinX1HutoPoints.Add( point ) ;
            }

            if ( now == 1 ) {
              syukinHosokeiPoints.Add( point ) ;

              syukinX1HosoPoints.Add( point ) ;
            }

            syukinX1Points.Add( point ) ;

            point = rightBottomRebar + new Revit.DB.XYZ( 0, x1SumDistance, 0 ) ;
            if ( now == 0 ) {
              syukinHutokeiPoints.Add( point ) ;
              y2RightTopPoint = point ;
            }

            if ( now == 1 ) {
              syukinHosokeiPoints.Add( point ) ;
            }
          }
          // ほか
          else {
            x1SumDistance += x1RebarDistance ;

            point = leftBottomRebar + new Revit.DB.XYZ( 0, x1SumDistance, 0 ) ;
            if ( now == 0 ) {
              syukinHutokeiPoints.Add( point ) ;

              syukinX1HutoPoints.Add( point ) ;
            }

            if ( now == 1 ) {
              syukinHosokeiPoints.Add( point ) ;

              syukinX1HosoPoints.Add( point ) ;
            }

            syukinX1Points.Add( point ) ;

            point = rightBottomRebar + new Revit.DB.XYZ( 0, x1SumDistance, 0 ) ;
            if ( now == 0 ) {
              syukinHutokeiPoints.Add( point ) ;
            }

            if ( now == 1 ) {
              syukinHosokeiPoints.Add( point ) ;
            }
          }
        }

        #region 旧配筋

        /*
        // 細径がない場合
        if (syukinX1HosokeiHonsu == 0)
        {
          #region

          double setCount = System.Math.Ceiling((double)syukinX1HutokeiHonsu / 2) - 1;
          for (int i = 0; i < setCount; ++i)
          {
            Revit.DB.XYZ point = leftBottomRebar + new Revit.DB.XYZ(0, x1SumDistance, 0);

            // 最初
            if (i == 0)
            {
              if (xYosekin == true)
              {
                x1SumDistance += diaSyukinHuto1_5;
                point = leftBottomRebar + new Revit.DB.XYZ(0, x1SumDistance, 0);
                syukinHutokeiPoints.Add(point);
                y2LeftBottomPoint = point;

                point = rightBottomRebar + new Revit.DB.XYZ(0, x1SumDistance, 0);
                syukinHutokeiPoints.Add(point);
                y2RightBottomPoint = point;

                point = leftTopRebar + new Revit.DB.XYZ(0, -x1SumDistance, 0);
                syukinHutokeiPoints.Add(point);
                y2LeftTopPoint = point;

                point = rightTopRebar + new Revit.DB.XYZ(0, -x1SumDistance, 0);
                syukinHutokeiPoints.Add(point);
                y2RightTopPoint = point;

                x1RebarDistance = _CmpGeometry.Distance(y2LeftBottomPoint, y2LeftTopPoint) / (x1RebarCount - 3);
              }
              else
              {
                x1SumDistance += x1RebarDistance;
                point = leftBottomRebar + new Revit.DB.XYZ(0, x1SumDistance, 0);
                syukinHutokeiPoints.Add(point);
                y2LeftBottomPoint = point;

                point = rightBottomRebar + new Revit.DB.XYZ(0, x1SumDistance, 0);
                syukinHutokeiPoints.Add(point);
                y2RightBottomPoint = point;

                point = leftTopRebar + new Revit.DB.XYZ(0, -x1SumDistance, 0);
                syukinHutokeiPoints.Add(point);
                y2LeftTopPoint = point;

                point = rightTopRebar + new Revit.DB.XYZ(0, -x1SumDistance, 0);
                syukinHutokeiPoints.Add(point);
                y2RightTopPoint = point;
              }

              //
              x1SetedRebarNumStart.Add(0);
              x1SetedRebarNumEnd.Add(0);
            }
            // 最後
            else if (i == setCount)
            {
              // 偶数本
              if (setCount % 2 == 0)
              {
                x1SumDistance += x1RebarDistance;
                point = leftBottomRebar + new Revit.DB.XYZ(0, x1SumDistance, 0);
                syukinHutokeiPoints.Add(point);

                point = rightBottomRebar + new Revit.DB.XYZ(0, x1SumDistance, 0);
                syukinHutokeiPoints.Add(point);

                point = leftTopRebar + new Revit.DB.XYZ(0, -x1SumDistance, 0);
                syukinHutokeiPoints.Add(point);

                point = rightTopRebar + new Revit.DB.XYZ(0, -x1SumDistance, 0);
                syukinHutokeiPoints.Add(point);

                //
                x1SetedRebarNumStart.Add(0);
                x1SetedRebarNumEnd.Add(0);
              }
              // 奇数本
              else
              {
                x1SumDistance += x1RebarDistance;
                point = leftBottomRebar + new Revit.DB.XYZ(0, x1SumDistance, 0);
                syukinHutokeiPoints.Add(point);

                point = rightBottomRebar + new Revit.DB.XYZ(0, x1SumDistance, 0);
                syukinHutokeiPoints.Add(point);

                // 片方にのみ追加
                x1SetedRebarNumStart.Add(0);
              }
            }
            else
            {
              x1SumDistance += x1RebarDistance;
              point = leftBottomRebar + new Revit.DB.XYZ(0, x1SumDistance, 0);
              syukinHutokeiPoints.Add(point);

              point = rightBottomRebar + new Revit.DB.XYZ(0, x1SumDistance, 0);
              syukinHutokeiPoints.Add(point);

              point = leftTopRebar + new Revit.DB.XYZ(0, -x1SumDistance, 0);
              syukinHutokeiPoints.Add(point);

              point = rightTopRebar + new Revit.DB.XYZ(0, -x1SumDistance, 0);
              syukinHutokeiPoints.Add(point);

              //
              x1SetedRebarNumStart.Add(0);
              x1SetedRebarNumEnd.Add(0);
            }
          }
          #endregion
        }
        // 細径がある場合
        else if (syukinX1HosokeiHonsu > 0)
        {
          #region

          // 配置済本数(片側のみ)
          int setRebarCount = 2;

          double sumDistance = 0;

          // 四隅からの太径の連続回数 = Ceiling((太径本数 - 細径本数) / 2) - 1
          double countSubtraction = syukinX1HutokeiHonsu - syukinX1HosokeiHonsu;
          double division = countSubtraction / 2;
          int ceiling = (int)System.Math.Ceiling(division);
          ceiling -= 1;

          // 四隅からの連続する太径
          for (int i = 0; i < ceiling; ++i)
          {
            setRebarCount += 2;

            //
            x1SetedRebarNumStart.Add(0);
            x1SetedRebarNumEnd.Add(0);

            if (i == 0 && xYosekin == true)
            {
              sumDistance += diaSyukinHuto1_5;
              x1RebarDistance = (_CmpGeometry.Distance2D(leftTopRebar, leftBottomRebar) - diaSyukinHuto1_5 * 2) / (x1RebarCount - 3);
            }
            else
            {
              sumDistance += x1RebarDistance;
            }

            Revit.DB.XYZ bottomPoint = leftBottomRebar + new Revit.DB.XYZ(0, sumDistance, 0);

            // 直交方向2段筋太径本数
            if (syukinY2HutokeiHonsu >= 2)
            {
              if (i == 0)
              {
                //bottomPoint += new Revit.DB.XYZ(0, -moveYValue, 0);

                y2LeftBottomPoint = bottomPoint;
              }
            }
            syukinHutokeiPoints.Add(bottomPoint);

            bottomPoint = rightBottomRebar + new Revit.DB.XYZ(0, sumDistance, 0);

            if (syukinY2HutokeiHonsu >= 2)
            {
              if (i == 0)
              {
                //bottomPoint += new Revit.DB.XYZ(0, -moveYValue, 0);

                y2RightBottomPoint = bottomPoint;
              }
            }
            syukinHutokeiPoints.Add(bottomPoint);

            Revit.DB.XYZ topPoint = leftTopRebar + new Revit.DB.XYZ(0, -sumDistance, 0);

            if (syukinY2HutokeiHonsu >= 2)
            {
              if (i == 0)
              {
                //topPoint += new Revit.DB.XYZ(0, moveYValue, 0);

                y2LeftTopPoint = topPoint;
              }
            }
            syukinHutokeiPoints.Add(topPoint);

            topPoint = rightTopRebar + new Revit.DB.XYZ(0, -sumDistance, 0);

            if (syukinY2HutokeiHonsu >= 2)
            {
              if (i == 0)
              {
                //topPoint += new Revit.DB.XYZ(0, moveYValue, 0);

                y2RightTopPoint = topPoint;
              }
            }
            syukinHutokeiPoints.Add(topPoint);
          }

          // 偶数
          if (countSubtraction % 2 == 0)
          {
            while (setRebarCount < x1RebarCount - 2)
            {
              // 細径
              sumDistance += x1RebarDistance;

              setRebarCount += 2;

              Revit.DB.XYZ bottomPoint = leftBottomRebar + new Revit.DB.XYZ(0, sumDistance, 0);
              syukinHosokeiPoints.Add(bottomPoint);

              bottomPoint = rightBottomRebar + new Revit.DB.XYZ(0, sumDistance, 0);
              syukinHosokeiPoints.Add(bottomPoint);

              Revit.DB.XYZ topPoint = leftTopRebar + new Revit.DB.XYZ(0, -sumDistance, 0);
              syukinHosokeiPoints.Add(topPoint);

              topPoint = rightTopRebar + new Revit.DB.XYZ(0, -sumDistance, 0);
              syukinHosokeiPoints.Add(topPoint);

              //
              x1SetedRebarNumStart.Add(1);
              x1SetedRebarNumEnd.Add(1);

              if ((setRebarCount < x1RebarCount - 2) == false)
              {
                break;
              }

              // 太径
              sumDistance += x1RebarDistance;

              setRebarCount += 2;

              bottomPoint = leftBottomRebar + new Revit.DB.XYZ(0, sumDistance, 0);
              syukinHutokeiPoints.Add(bottomPoint);

              bottomPoint = rightBottomRebar + new Revit.DB.XYZ(0, sumDistance, 0);
              syukinHutokeiPoints.Add(bottomPoint);

              topPoint = leftTopRebar + new Revit.DB.XYZ(0, -sumDistance, 0);
              syukinHutokeiPoints.Add(topPoint);

              topPoint = rightTopRebar + new Revit.DB.XYZ(0, -sumDistance, 0);
              syukinHutokeiPoints.Add(topPoint);

              //
              x1SetedRebarNumStart.Add(0);
              x1SetedRebarNumEnd.Add(0);
            }

            // 細径が偶数本
            if (syukinX1HosokeiHonsu % 2 == 0)
            {
              sumDistance+=x1RebarDistance;

              Revit.DB.XYZ centerRebarPoint = leftBottomRebar + new Revit.DB.XYZ(0, sumDistance, 0);
              syukinHutokeiPoints.Add(centerRebarPoint);

              centerRebarPoint = leftTopRebar + new Revit.DB.XYZ(0, -sumDistance, 0);
              syukinHutokeiPoints.Add(centerRebarPoint);

              centerRebarPoint = rightBottomRebar + new Revit.DB.XYZ(0, sumDistance, 0);
              syukinHutokeiPoints.Add(centerRebarPoint);

              centerRebarPoint = rightTopRebar + new Revit.DB.XYZ(0, -sumDistance, 0);
              syukinHutokeiPoints.Add(centerRebarPoint);

              //
              x1SetedRebarNumStart.Add(0);
              x1SetedRebarNumEnd.Add(0);
            }
            else
            {
              sumDistance += x1RebarDistance;

              // 下が細径
              Revit.DB.XYZ centerRebarPoint = leftBottomRebar + new Revit.DB.XYZ(0, sumDistance, 0);
              syukinHosokeiPoints.Add(centerRebarPoint);

              centerRebarPoint = leftTopRebar + new Revit.DB.XYZ(0, -sumDistance, 0);
              syukinHutokeiPoints.Add(centerRebarPoint);

              centerRebarPoint = rightBottomRebar + new Revit.DB.XYZ(0, sumDistance, 0);
              syukinHosokeiPoints.Add(centerRebarPoint);

              centerRebarPoint = rightTopRebar + new Revit.DB.XYZ(0, -sumDistance, 0);
              syukinHutokeiPoints.Add(centerRebarPoint);

              //
              x1SetedRebarNumStart.Add(1);
              x1SetedRebarNumEnd.Add(0);
            }
          }
          // 奇数
          else
          {
            bool isLastHuto = true;

            while (setRebarCount < x1RebarCount - 1)
            {
              // 細径
              sumDistance += x1RebarDistance;

              setRebarCount += 2;
              isLastHuto = false;

              Revit.DB.XYZ bottomPoint = leftBottomRebar + new Revit.DB.XYZ(0, sumDistance, 0);
              syukinHosokeiPoints.Add(bottomPoint);

              bottomPoint = rightBottomRebar + new Revit.DB.XYZ(0, sumDistance, 0);
              syukinHosokeiPoints.Add(bottomPoint);

              Revit.DB.XYZ topPoint = leftTopRebar + new Revit.DB.XYZ(0, -sumDistance, 0);
              syukinHosokeiPoints.Add(topPoint);

              topPoint = rightTopRebar + new Revit.DB.XYZ(0, -sumDistance, 0);
              syukinHosokeiPoints.Add(topPoint);

              //
              x1SetedRebarNumStart.Add(1);
              x1SetedRebarNumEnd.Add(1);

              if ((setRebarCount < x1RebarCount - 1) == false)
              {
                break;
              }

              // 太径
              sumDistance += x1RebarDistance;

              setRebarCount += 2;
              isLastHuto = true;

              bottomPoint = leftBottomRebar + new Revit.DB.XYZ(0, sumDistance, 0);
              syukinHutokeiPoints.Add(bottomPoint);

              bottomPoint = rightBottomRebar + new Revit.DB.XYZ(0, sumDistance, 0);
              syukinHutokeiPoints.Add(bottomPoint);

              topPoint = leftTopRebar + new Revit.DB.XYZ(0, -sumDistance, 0);
              syukinHutokeiPoints.Add(topPoint);

              topPoint = rightTopRebar + new Revit.DB.XYZ(0, -sumDistance, 0);
              syukinHutokeiPoints.Add(topPoint);

              //
              x1SetedRebarNumStart.Add(0);
              x1SetedRebarNumEnd.Add(0);
            }

            // 中間部
            // 直前が太径
            if (isLastHuto == true)
            {
              Revit.DB.XYZ centerRebarPoint = leftBottomRebar + new Revit.DB.XYZ(0, x1centerDistance, 0);
              syukinHosokeiPoints.Add(centerRebarPoint);

              centerRebarPoint = rightBottomRebar + new Revit.DB.XYZ(0, x1centerDistance, 0);
              syukinHosokeiPoints.Add(centerRebarPoint);

              //
              x1SetedRebarNumStart.Add(1);
            }
            else
            {
              Revit.DB.XYZ centerRebarPoint = leftBottomRebar + new Revit.DB.XYZ(0, x1centerDistance, 0);
              syukinHutokeiPoints.Add(centerRebarPoint);

              centerRebarPoint = rightBottomRebar + new Revit.DB.XYZ(0, x1centerDistance, 0);
              syukinHutokeiPoints.Add(centerRebarPoint);

              //
              x1SetedRebarNumStart.Add(0);
            }
          }

          #endregion
        }

        // x1段筋配置順の統合
        for (int num = 0; num < x1SetedRebarNumEnd.Count; ++num)
        {
          x1SetedRebarNumStart.Add(x1SetedRebarNumEnd[x1SetedRebarNumEnd.Count - (num + 1)]);
        }
        */

        #endregion

        #endregion

        // Y1段筋

        #region

        // Y1段配筋本数
        int y1RebarCount = syukinY1HutokeiHonsu + syukinY1HosokeiHonsu ;
        // Y1段筋記号中心間距離
        double y1RebarDistance = ( rightTopRebar.X - leftTopRebar.X ) / ( y1RebarCount - 1 ) ;
        // Y1段筋中間距離
        double y1centerDistance = ( rightTopRebar.X - leftTopRebar.X ) / 2 ;

        // X2段筋兼Y1段筋移動量
        double moveXValue = y1RebarDistance - diaSyukinHuto * 1.5 ;

        // Y1段筋合計位置
        double y1SumDistance = 0 ;

        // 配筋
        for ( int i = 0 ; i < y1RebarOrder.Count ; ++i ) {
          int now = y1RebarOrder[ i ] ;
          Revit.DB.XYZ point = null ;

          // 四隅
          if ( i == 0 || i == y1RebarOrder.Count - 1 ) {
            continue ;
          }
          // 最初の1本(寄筋かもしれない鉄筋)
          else if ( i == 1 ) {
            // 太径かつ寄筋
            if ( now == 0 && yYosekin == true ) {
              y1SumDistance += diaSyukinHuto1_5 ;
              y1RebarDistance = ( _CmpGeometry.Distance2D( leftTopRebar, rightTopRebar ) - diaSyukinHuto1_5 * 2 ) / ( y1RebarCount - 3 ) ;

              _Y2ndRebarIs = true ;
              _Y2ndRebarDistance = diaSyukinHuto * 2.5 ;
            }
            else {
              y1SumDistance += y1RebarDistance ;

              _Y2ndRebarIs = false ;
            }

            _Y2ndRebarDistance = diaSyukinHuto * 2.5 ;

            point = leftTopRebar + new Revit.DB.XYZ( y1SumDistance, 0, 0 ) ;
            if ( now == 0 ) {
              syukinHutokeiPoints.Add( point ) ;
              x2LeftTopPoint = point ;

              syukinY1HutoPoints.Add( point ) ;
            }

            if ( now == 1 ) {
              syukinHosokeiPoints.Add( point ) ;

              syukinY1HosoPoints.Add( point ) ;
            }

            syukinY1Points.Add( point ) ;

            point = leftBottomRebar + new Revit.DB.XYZ( y1SumDistance, 0, 0 ) ;
            if ( now == 0 ) {
              syukinHutokeiPoints.Add( point ) ;
              x2LeftBottomPoint = point ;
            }

            if ( now == 1 ) {
              syukinHosokeiPoints.Add( point ) ;
            }
          }
          // 最後の1本(寄筋かもしれない鉄筋)
          else if ( i == y1RebarOrder.Count - 2 ) {
            y1SumDistance += y1RebarDistance ;

            point = leftTopRebar + new Revit.DB.XYZ( y1SumDistance, 0, 0 ) ;
            if ( now == 0 ) {
              syukinHutokeiPoints.Add( point ) ;
              x2RightTopPoint = point ;

              syukinY1HutoPoints.Add( point ) ;
            }

            if ( now == 1 ) {
              syukinHosokeiPoints.Add( point ) ;

              syukinY1HosoPoints.Add( point ) ;
            }

            syukinY1Points.Add( point ) ;

            point = leftBottomRebar + new Revit.DB.XYZ( y1SumDistance, 0, 0 ) ;
            if ( now == 0 ) {
              syukinHutokeiPoints.Add( point ) ;
              x2RightBottomPoint = point ;
            }

            if ( now == 1 ) {
              syukinHosokeiPoints.Add( point ) ;
            }
          }
          // ほか
          else {
            y1SumDistance += y1RebarDistance ;

            point = leftTopRebar + new Revit.DB.XYZ( y1SumDistance, 0, 0 ) ;
            if ( now == 0 ) {
              syukinHutokeiPoints.Add( point ) ;

              syukinY1HutoPoints.Add( point ) ;
            }

            if ( now == 1 ) {
              syukinHosokeiPoints.Add( point ) ;

              syukinY1HosoPoints.Add( point ) ;
            }

            syukinY1Points.Add( point ) ;

            point = leftBottomRebar + new Revit.DB.XYZ( y1SumDistance, 0, 0 ) ;
            if ( now == 0 ) {
              syukinHutokeiPoints.Add( point ) ;
            }

            if ( now == 1 ) {
              syukinHosokeiPoints.Add( point ) ;
            }
          }
        }

        #region 旧配筋

        /*
        // 細径がない場合
        if (syukinY1HosokeiHonsu == 0)
        {
          double setCount = System.Math.Ceiling((double)syukinY1HutokeiHonsu / 2) - 1;
          for (int i = 0; i < setCount; ++i)
          {
            Revit.DB.XYZ point = leftTopRebar + new Revit.DB.XYZ(y1SumDistance, 0, 0);

            // 最初
            if (i == 0)
            {
              if (yYosekin == true)
              {
                y1SumDistance += diaSyukinHuto1_5;
                point = leftTopRebar + new Revit.DB.XYZ(y1SumDistance, 0, 0);
                syukinHutokeiPoints.Add(point);
                x2LeftTopPoint = point;

                point = leftBottomRebar + new Revit.DB.XYZ(y1SumDistance, 0, 0);
                syukinHutokeiPoints.Add(point);
                x2LeftBottomPoint = point;

                point = rightTopRebar + new Revit.DB.XYZ(-y1SumDistance, 0, 0);
                syukinHutokeiPoints.Add(point);
                x2RightTopPoint = point;

                point = rightBottomRebar + new Revit.DB.XYZ(-y1SumDistance, 0, 0);
                syukinHutokeiPoints.Add(point);
                x2RightBottomPoint = point;

                y1RebarDistance = _CmpGeometry.Distance(x2LeftTopPoint, x2RightTopPoint) / (y1RebarCount - 3);
              }
              else
              {
                y1SumDistance += y1RebarDistance;
                point = leftBottomRebar + new Revit.DB.XYZ(y1SumDistance, 0, 0);
                syukinHutokeiPoints.Add(point);
                x2LeftBottomPoint = point;

                point = rightBottomRebar + new Revit.DB.XYZ(-y1SumDistance, 0, 0);
                syukinHutokeiPoints.Add(point);
                x2RightBottomPoint = point;

                point = leftTopRebar + new Revit.DB.XYZ(y1SumDistance, 0, 0);
                syukinHutokeiPoints.Add(point);
                x2LeftTopPoint = point;

                point = rightTopRebar + new Revit.DB.XYZ(-y1SumDistance, 0, 0);
                syukinHutokeiPoints.Add(point);
                x2RightTopPoint = point;
              }

              //
              y1SetedRebarNumStart.Add(0);
              y1SetedRebarNumEnd.Add(0);
            }
            // 最後
            else if (i == setCount)
            {
              // 偶数本
              if (setCount % 2 == 0)
              {
                y1SumDistance += y1RebarDistance;
                point = leftBottomRebar + new Revit.DB.XYZ(y1SumDistance, 0, 0);
                syukinHutokeiPoints.Add(point);

                point = leftTopRebar + new Revit.DB.XYZ(y1SumDistance, 0, 0);
                syukinHutokeiPoints.Add(point);

                point = rightBottomRebar + new Revit.DB.XYZ(-y1SumDistance, 0, 0);
                syukinHutokeiPoints.Add(point);

                point = rightTopRebar + new Revit.DB.XYZ(-y1SumDistance, 0, 0);
                syukinHutokeiPoints.Add(point);

                //
                y1SetedRebarNumStart.Add(0);
                y1SetedRebarNumEnd.Add(0);
              }
              // 奇数本
              else
              {
                y1SumDistance += y1RebarDistance;
                point = leftBottomRebar + new Revit.DB.XYZ(y1SumDistance, 0, 0);
                syukinHutokeiPoints.Add(point);

                point = leftTopRebar + new Revit.DB.XYZ(y1SumDistance, 0, 0);
                syukinHutokeiPoints.Add(point);

                //
                y1SetedRebarNumStart.Add(0);
              }
            }
            else
            {
              y1SumDistance += y1RebarDistance;
              point = leftBottomRebar + new Revit.DB.XYZ(y1SumDistance, 0, 0);
              syukinHutokeiPoints.Add(point);

              point = rightBottomRebar + new Revit.DB.XYZ(-y1SumDistance, 0, 0);
              syukinHutokeiPoints.Add(point);

              point = leftTopRebar + new Revit.DB.XYZ(y1SumDistance, 0, 0);
              syukinHutokeiPoints.Add(point);

              point = rightTopRebar + new Revit.DB.XYZ(-y1SumDistance, 0, 0);
              syukinHutokeiPoints.Add(point);

              //
              y1SetedRebarNumStart.Add(0);
              y1SetedRebarNumEnd.Add(0);
            }
          }
        }
        // 細径がある場合
        else if (syukinY1HosokeiHonsu > 0)
        {
          // 配置済本数(片側のみ)
          int setRebarCount = 2;

          double sumDistance = 0;

          // 四隅からの太径の連続回数 = Ceiling((太径本数 - 細径本数) / 2) - 1
          double countSubtraction = syukinY1HutokeiHonsu - syukinY1HosokeiHonsu;
          double division = countSubtraction / 2;
          int ceiling = (int)System.Math.Ceiling(division);
          ceiling -= 1;

          // 四隅からの連続する太径
          for (int i = 0; i < ceiling; ++i)
          {
            setRebarCount += 2;

            //
            y1SetedRebarNumStart.Add(0);
            y1SetedRebarNumEnd.Add(0);

            if (i == 0 && yYosekin == true)
            {
              sumDistance += diaSyukinHuto1_5;
              y1RebarDistance = (_CmpGeometry.Distance2D(leftTopRebar, rightTopRebar) - diaSyukinHuto1_5 * 2) / (y1RebarCount - 3);
            }
            else
            {
              sumDistance += y1RebarDistance;
            }

            Revit.DB.XYZ leftPoint = leftTopRebar + new Revit.DB.XYZ(sumDistance, 0, 0);

            // 直交方向2段筋太径本数
            if (syukinX2HutokeiHonsu >= 2)
            {
              if (i == 0)
              {
                //leftPoint += new Revit.DB.XYZ(-moveXValue, 0, 0);

                x2LeftTopPoint = leftPoint;
              }
            }
            syukinHutokeiPoints.Add(leftPoint);

            leftPoint = leftBottomRebar + new Revit.DB.XYZ(sumDistance, 0, 0);

            if (syukinX2HutokeiHonsu >= 2)
            {
              if (i == 0)
              {
                //leftPoint += new Revit.DB.XYZ(-moveXValue, 0, 0);

                x2LeftBottomPoint = leftPoint;
              }
            }
            syukinHutokeiPoints.Add(leftPoint);

            Revit.DB.XYZ rightPoint = rightTopRebar + new Revit.DB.XYZ(-sumDistance, 0, 0);

            if (syukinX2HutokeiHonsu >= 2)
            {
              if (i == 0)
              {
                //rightPoint += new Revit.DB.XYZ(moveXValue, 0, 0);

                x2RightTopPoint = rightPoint;
              }
            }
            syukinHutokeiPoints.Add(rightPoint);

            rightPoint = rightBottomRebar + new Revit.DB.XYZ(-sumDistance, 0, 0);

            if (syukinX2HutokeiHonsu >= 2)
            {
              if (i == 0)
              {
                //rightPoint += new Revit.DB.XYZ(moveXValue, 0, 0);

                x2RightBottomPoint = rightPoint;
              }
            }
            syukinHutokeiPoints.Add(rightPoint);
          }

          // 偶数
          if (countSubtraction % 2 == 0)
          {
            while (setRebarCount < y1RebarCount - 2)
            {
              // 細径
              sumDistance += y1RebarDistance;

              setRebarCount += 2;

              Revit.DB.XYZ leftPoint = leftTopRebar + new Revit.DB.XYZ(sumDistance, 0, 0);
              syukinHosokeiPoints.Add(leftPoint);

              leftPoint = leftBottomRebar + new Revit.DB.XYZ(sumDistance, 0, 0);
              syukinHosokeiPoints.Add(leftPoint);

              Revit.DB.XYZ rightPoint = rightTopRebar + new Revit.DB.XYZ(-sumDistance, 0, 0);
              syukinHosokeiPoints.Add(rightPoint);

              rightPoint = rightBottomRebar + new Revit.DB.XYZ(-sumDistance, 0, 0);
              syukinHosokeiPoints.Add(rightPoint);

              //
              y1SetedRebarNumStart.Add(1);
              y1SetedRebarNumEnd.Add(1);

              if ((setRebarCount < y1RebarCount - 2) == false)
              {
                break;
              }

              // 太径
              sumDistance += y1RebarDistance;

              setRebarCount += 2;

              leftPoint = leftTopRebar + new Revit.DB.XYZ(sumDistance, 0, 0);
              syukinHutokeiPoints.Add(leftPoint);

              leftPoint = leftBottomRebar + new Revit.DB.XYZ(sumDistance, 0, 0);
              syukinHutokeiPoints.Add(leftPoint);

              rightPoint = rightTopRebar + new Revit.DB.XYZ(-sumDistance, 0, 0);
              syukinHutokeiPoints.Add(rightPoint);

              rightPoint = rightBottomRebar + new Revit.DB.XYZ(-sumDistance, 0, 0);
              syukinHutokeiPoints.Add(rightPoint);

              //
              y1SetedRebarNumStart.Add(0);
              y1SetedRebarNumEnd.Add(0);
            }

            // 細径が偶数本
            if (syukinY1HosokeiHonsu % 2 == 0)
            {
              sumDistance += y1RebarDistance;

              Revit.DB.XYZ centerRebarPoint = leftTopRebar + new Revit.DB.XYZ(sumDistance, 0, 0);
              syukinHutokeiPoints.Add(centerRebarPoint);

              centerRebarPoint = rightTopRebar + new Revit.DB.XYZ(-sumDistance, 0, 0);
              syukinHutokeiPoints.Add(centerRebarPoint);

              centerRebarPoint = leftBottomRebar + new Revit.DB.XYZ(sumDistance, 0, 0);
              syukinHutokeiPoints.Add(centerRebarPoint);

              centerRebarPoint = rightBottomRebar + new Revit.DB.XYZ(-sumDistance, 0, 0);
              syukinHutokeiPoints.Add(centerRebarPoint);

              //
              y1SetedRebarNumStart.Add(0);
              y1SetedRebarNumEnd.Add(0);
            }
            else
            {
              sumDistance += y1RebarDistance;

              // 左が細径
              Revit.DB.XYZ centerRebarPoint = leftTopRebar + new Revit.DB.XYZ(sumDistance, 0, 0);
              syukinHosokeiPoints.Add(centerRebarPoint);

              centerRebarPoint = rightTopRebar + new Revit.DB.XYZ(-sumDistance, 0, 0);
              syukinHutokeiPoints.Add(centerRebarPoint);

              centerRebarPoint = leftBottomRebar + new Revit.DB.XYZ(sumDistance, 0, 0);
              syukinHosokeiPoints.Add(centerRebarPoint);

              centerRebarPoint = rightBottomRebar + new Revit.DB.XYZ(-sumDistance, 0, 0);
              syukinHutokeiPoints.Add(centerRebarPoint);

              //
              y1SetedRebarNumStart.Add(1);
              y1SetedRebarNumEnd.Add(0);
            }
          }
          // 奇数
          else
          {
            bool isLastHuto = true;

            while (setRebarCount < y1RebarCount - 1)
            {
              // 細径
              sumDistance += y1RebarDistance;

              setRebarCount += 2;
              isLastHuto = false;

              Revit.DB.XYZ leftPoint = leftTopRebar + new Revit.DB.XYZ(sumDistance, 0, 0);
              syukinHosokeiPoints.Add(leftPoint);

              leftPoint = leftBottomRebar + new Revit.DB.XYZ(sumDistance, 0, 0);
              syukinHosokeiPoints.Add(leftPoint);

              Revit.DB.XYZ rightPoint = rightTopRebar + new Revit.DB.XYZ(-sumDistance, 0, 0);
              syukinHosokeiPoints.Add(rightPoint);

              rightPoint = rightBottomRebar + new Revit.DB.XYZ(-sumDistance, 0, 0);
              syukinHosokeiPoints.Add(rightPoint);

              //
              y1SetedRebarNumStart.Add(1);
              y1SetedRebarNumEnd.Add(1);

              if ((setRebarCount < y1RebarCount - 1) == false)
              {
                break;
              }

              // 太径
              sumDistance += y1RebarDistance;

              setRebarCount += 2;
              isLastHuto = true;

              leftPoint = leftTopRebar + new Revit.DB.XYZ(sumDistance, 0, 0);
              syukinHutokeiPoints.Add(leftPoint);

              leftPoint = leftBottomRebar + new Revit.DB.XYZ(sumDistance, 0, 0);
              syukinHutokeiPoints.Add(leftPoint);

              rightPoint = rightTopRebar + new Revit.DB.XYZ(-sumDistance, 0, 0);
              syukinHutokeiPoints.Add(rightPoint);

              rightPoint = rightBottomRebar + new Revit.DB.XYZ(-sumDistance, 0, 0);
              syukinHutokeiPoints.Add(rightPoint);

              //
              y1SetedRebarNumStart.Add(0);
              y1SetedRebarNumEnd.Add(0);
            }

            // 中間部
            // 直前が太径
            if (isLastHuto == true)
            {
              Revit.DB.XYZ centerRebarPoint = leftTopRebar + new Revit.DB.XYZ(y1centerDistance, 0, 0);
              syukinHosokeiPoints.Add(centerRebarPoint);

              centerRebarPoint = leftBottomRebar + new Revit.DB.XYZ(y1centerDistance, 0, 0);
              syukinHosokeiPoints.Add(centerRebarPoint);

              //
              y1SetedRebarNumStart.Add(1);
            }
            else
            {
              Revit.DB.XYZ centerRebarPoint = leftTopRebar + new Revit.DB.XYZ(y1centerDistance, 0, 0);
              syukinHutokeiPoints.Add(centerRebarPoint);

              centerRebarPoint = leftBottomRebar + new Revit.DB.XYZ(y1centerDistance, 0, 0);
              syukinHutokeiPoints.Add(centerRebarPoint);

              //
              y1SetedRebarNumStart.Add(0);
            }
          }
        }

        // y1段筋配置順の統合
        for (int num = 0; num < y1SetedRebarNumEnd.Count; ++num)
        {
          y1SetedRebarNumStart.Add(y1SetedRebarNumEnd[y1SetedRebarNumEnd.Count - (num + 1)]);
        }
        */

        #endregion

        #endregion

        #region 2段筋エラー判定

        // 2段筋
        // X太径がある
        if ( syukinX2HutokeiHonsu > 0 ) {
          // 2本未満
          if ( syukinX2HutokeiHonsu < 2 ) {
            goto draw ;
          }

          // 直交方向1段筋太径4本未満
          if ( syukinY1HutokeiHonsu < 4 ) {
            goto draw ;
          }

          // 直交方向2段筋太径4本未満
          if ( syukinY2HutokeiHonsu < 4 ) {
            //goto draw;
          }

          // 同一方向1段筋太径本数より多い
          if ( syukinX1HutokeiHonsu < syukinX2HutokeiHonsu ) {
            goto draw ;
          }

          // フラグが立っていない場合
          if ( flag2 == 0 ) {
            // 直交方向2段筋がない場合、1段筋をもとに配筋
            // 直交方向2段筋がある場合、フラグが立っていないためコーナーに配筋できない = 2段筋本数は(1段筋本数 - 2)以下

            // X2段筋
            // 直交方向2段筋あり
            if ( syukinY2HutokeiHonsu > 0 ) {
              // 1段筋本数-2以下または2段筋本数は2本以下
              if ( ( syukinX2HutokeiHonsu <= syukinX1HutokeiHonsu - 2 ) || syukinX2HutokeiHonsu <= 2 ) {
                // コーナー部分を除き配筋
              }
              else {
                // 配筋不可
                goto draw ;
              }
            }
            // 直交方向2段筋なし
            else {
              // X1段筋をもとに配筋
            }

            //if (syukinX1HutokeiHonsu - 2 < syukinX2HutokeiHonsu)
            //{
            //  goto draw;
            //}
          }

          // フラグが立っている場合
          if ( flag2 == 1 ) {
            // X2段筋
            // 直交方向2段筋あり
            if ( syukinY2HutokeiHonsu > 0 ) {
              // 2段筋本数4本以上かつ直交2段筋本数4本以上
              if ( syukinX2HutokeiHonsu >= 4 && syukinY2HutokeiHonsu >= 4 ) {
                // コーナー配筋を含む配筋
              }
              else {
                // 2段筋本数4本以上
                if ( syukinX2HutokeiHonsu >= 4 ) {
                  // 2段本数は1段本数-2以下
                  if ( syukinX2HutokeiHonsu <= syukinX1HutokeiHonsu - 2 ) {
                    // コーナーを除き配筋
                  }
                  else {
                    // 配筋不可
                    goto draw ;
                  }
                }
                else {
                  if ( syukinX2HutokeiHonsu == 3 ) {
                    // 1段筋本数5本以上
                    if ( syukinX1HutokeiHonsu >= 5 ) {
                      // コーナーを除き配筋
                    }
                    else {
                      // 配筋不可
                      goto draw ;
                    }
                  }
                }
              }
            }
            // 直交方向2段筋なし
            else {
              // X1段筋をもとに配筋
            }

            //// コーナー配筋するとき直交方向が4本未満
            //if (syukinX2HutokeiHonsu > 3 && syukinY2HutokeiHonsu < 4)
            //{
            //  //goto draw;
            //  flag2 = 0;
            //}
            //// 2段筋が3本のときは1段筋は5本以上必要
            //if (syukinX2HutokeiHonsu + syukinX2HosokeiHonsu == 3 && syukinX1HutokeiHonsu + syukinX1HosokeiHonsu < 5)
            //{
            //  goto draw;
            //}
          }
        }

        // X細径がある
        if ( syukinX2HosokeiHonsu > 0 ) {
          // 同一方向2段筋太径本数2本未満
          if ( syukinX2HutokeiHonsu < 2 ) {
            goto draw ;
          }

          // 同一方向2段筋太径本以上
          if ( syukinX2HosokeiHonsu >= syukinX2HutokeiHonsu ) {
            goto draw ;
          }

          // 直交方向1段筋太径本数4本未満
          if ( syukinY1HutokeiHonsu < 4 ) {
            goto draw ;
          }
        }

        // 合計
        if ( flag2 == 0 ) {
          if ( syukinX2HosokeiHonsu > 0 ) {
            // 直交方向2段筋がある場合
            if ( syukinY2HutokeiHonsu > 0 ) {
              if ( syukinX1HutokeiHonsu + syukinX1HosokeiHonsu - 2 < syukinX2HutokeiHonsu + syukinX2HosokeiHonsu ) {
                goto draw ;
              }
            }
            else {
              if ( syukinX1HutokeiHonsu + syukinX1HosokeiHonsu < syukinX2HutokeiHonsu + syukinX2HosokeiHonsu ) {
                goto draw ;
              }
            }
          }
        }

        if ( flag2 == 1 ) {
          if ( syukinX1HutokeiHonsu + syukinX1HosokeiHonsu < syukinX2HutokeiHonsu + syukinX2HosokeiHonsu ) {
            goto draw ;
          }

          if ( syukinX2HosokeiHonsu > 0 ) {
            if ( syukinY2HutokeiHonsu >= 2 ) {
              if ( syukinX1HutokeiHonsu + syukinX1HosokeiHonsu < 5 ) {
                goto draw ;
              }
            }
          }

          // 直交方向2段あり
          if ( syukinY2HutokeiHonsu > 0 ) {
            // 同一方向2段筋太径本数4本未満
            if ( syukinX2HutokeiHonsu < 4 ) {
              // コーナー部分への配筋が必要
              if ( syukinX1HutokeiHonsu + syukinX1HosokeiHonsu - 2 < syukinX2HutokeiHonsu + syukinX2HosokeiHonsu ) {
                goto draw ;
              }
            }
          }
        }

        // Y太径
        if ( syukinY2HutokeiHonsu > 0 ) {
          // 2本未満
          if ( syukinY2HutokeiHonsu < 2 ) {
            goto draw ;
          }

          // 直交方向1段筋太径4本未満
          if ( syukinX1HutokeiHonsu < 4 ) {
            goto draw ;
          }

          // 直交方向2段筋太径4本未満
          if ( syukinX2HutokeiHonsu < 4 ) {
            //goto draw;
          }

          // 同一方向1段筋太径本数より多い
          if ( syukinY1HutokeiHonsu < syukinY2HutokeiHonsu ) {
            goto draw ;
          }

          // フラグがない
          if ( flag2 == 0 ) {
            // Y2段筋
            // 直交方向2段筋あり
            if ( syukinX2HutokeiHonsu > 0 ) {
              // 1段筋本数-2以下または2段筋本数は2本以下
              if ( ( syukinY2HutokeiHonsu <= syukinY1HutokeiHonsu - 2 ) || syukinY2HutokeiHonsu <= 2 ) {
                // コーナー部分を除き配筋
              }
              else {
                // 配筋不可
                goto draw ;
              }
            }
            // 直交方向2段筋なし
            else {
              // Y1段筋をもとに配筋
            }

            //if (syukinY1HutokeiHonsu - 2 < syukinY2HutokeiHonsu)
            //{
            //  goto draw;
            //}
          }

          // フラグが立っている場合
          if ( flag2 == 1 ) {
            // Y2段筋
            // 直交方向2段筋あり
            if ( syukinX2HutokeiHonsu > 0 ) {
              // 2段筋本数4本以上かつ直交2段筋本数4本以上
              if ( syukinY2HutokeiHonsu >= 4 && syukinX2HutokeiHonsu >= 4 ) {
                // コーナー配筋を含む配筋
              }
              else {
                // 2段筋本数4本以上
                if ( syukinY2HutokeiHonsu >= 4 ) {
                  // 2段本数は1段本数-2以下
                  if ( syukinY2HutokeiHonsu <= syukinY1HutokeiHonsu - 2 ) {
                    // コーナーを除き配筋
                  }
                  else {
                    // 配筋不可
                    goto draw ;
                  }
                }
                else {
                  if ( syukinY2HutokeiHonsu == 3 ) {
                    // 1段筋本数5本以上
                    if ( syukinY1HutokeiHonsu >= 5 ) {
                      // コーナーを除き配筋
                    }
                    else {
                      // 配筋不可
                      goto draw ;
                    }
                  }
                }
              }
            }
            // 直交方向2段筋なし
            else {
              // Y1段筋をもとに配筋
            }

            //// コーナー配筋するとき直交方向が4本未満
            //if (syukinY2HutokeiHonsu > 3 && syukinX2HutokeiHonsu < 4)
            //{
            //  //goto draw;
            //  flag2 = 0;
            //}
            //// 2段筋が3本のときは1段筋は5本以上必要
            //if (syukinY2HutokeiHonsu + syukinY2HosokeiHonsu == 3 && syukinY1HutokeiHonsu + syukinY1HosokeiHonsu < 5)
            //{
            //  goto draw;
            //}
          }
        }

        // Y細径
        if ( syukinY2HosokeiHonsu > 0 ) {
          // 同一方向2段筋太径本数2本未満
          if ( syukinY2HutokeiHonsu < 2 ) {
            goto draw ;
          }

          // 同一方向2段筋太径本以上
          if ( syukinY2HosokeiHonsu >= syukinY2HutokeiHonsu ) {
            goto draw ;
          }

          // 直交方向1段筋太径本数4本未満
          if ( syukinX1HutokeiHonsu < 4 ) {
            goto draw ;
          }
        }

        // 合計
        if ( flag2 == 0 ) {
          if ( syukinY2HosokeiHonsu > 0 ) {
            // 直交方向2段筋がある場合
            if ( syukinX2HutokeiHonsu > 0 ) {
              if ( syukinY1HutokeiHonsu + syukinY1HosokeiHonsu - 2 < syukinY2HutokeiHonsu + syukinY2HosokeiHonsu ) {
                goto draw ;
              }
            }
            else {
              if ( syukinY1HutokeiHonsu + syukinY1HosokeiHonsu < syukinY2HutokeiHonsu + syukinY2HosokeiHonsu ) {
                goto draw ;
              }
            }
          }
        }

        if ( flag2 == 1 ) {
          if ( syukinY1HutokeiHonsu + syukinY1HosokeiHonsu < syukinY2HutokeiHonsu + syukinY2HosokeiHonsu ) {
            goto draw ;
          }

          if ( syukinY2HosokeiHonsu > 0 ) {
            if ( syukinX2HutokeiHonsu >= 2 ) {
              if ( syukinY1HutokeiHonsu + syukinY2HosokeiHonsu < 5 ) {
                goto draw ;
              }
            }
          }

          // 直交方向2段あり
          if ( syukinX2HutokeiHonsu > 0 ) {
            // 同一方向2段筋太径本数4本未満
            if ( syukinY2HutokeiHonsu < 4 ) {
              // コーナー部分への配筋が必要
              if ( syukinY1HutokeiHonsu + syukinY1HosokeiHonsu - 2 < syukinY2HutokeiHonsu + syukinY2HosokeiHonsu ) {
                goto draw ;
              }
            }
          }
        }

        #endregion

        // 2段筋配置位置用を足す
        foreach ( Revit.DB.XYZ point in syukinX1HutoPoints ) {
          syukinX1HosoPoints.Add( point ) ;
        }

        foreach ( Revit.DB.XYZ point in syukinY1HutoPoints ) {
          syukinY1HosoPoints.Add( point ) ;
        }

        // 2段筋の配置
        bool canSet2ndRebar = true ;
        bool seted2ndCornerRebar = false ;

        // XY方向ともに2段筋が4本以上
        if ( syukinX2HutokeiHonsu >= 4 && syukinY2HutokeiHonsu >= 4 ) {
          if ( x2LeftTopPoint == null || y2LeftTopPoint == null || x2RightTopPoint == null || y2RightTopPoint == null || x2LeftBottomPoint == null || y2LeftBottomPoint == null || x2RightBottomPoint == null || y2RightBottomPoint == null ) {
            // 2段筋端部が太径ではないため
            canSet2ndRebar = false ;
          }
        }

        if ( canSet2ndRebar == true ) {
          // 1段筋と同じルールで割り付け
          Collections.Generic.IList<int> x2RebarOrder = RebarOrder_Column( syukinX2HutokeiHonsu, syukinX2HosokeiHonsu, _X2ndRebarIs ) ;
          Collections.Generic.IList<int> y2RebarOrder = RebarOrder_Column( syukinY2HutokeiHonsu, syukinY2HosokeiHonsu, _Y2ndRebarIs ) ;

          // 2段筋コーナー配筋
          Revit.DB.XYZ p1 = null ;
          Revit.DB.XYZ p2 = null ;
          Revit.DB.XYZ p3 = null ;
          Revit.DB.XYZ p4 = null ;

          if ( syukinX2HutokeiHonsu >= 4 && syukinY2HutokeiHonsu >= 4 &&
               //syukinX1HutokeiHonsu - syukinX2HutokeiHonsu >= 2 &&
               //syukinY1HutokeiHonsu - syukinY2HutokeiHonsu >= 2 &&
               //(syukinX1HutokeiHonsu + syukinX1HosokeiHonsu) - (syukinX2HutokeiHonsu + syukinX2HosokeiHonsu) >= 2 &&
               //(syukinY1HutokeiHonsu + syukinY1HosokeiHonsu) - (syukinY2HutokeiHonsu + syukinY2HosokeiHonsu) >= 2 &&
               x2RebarOrder[ 0 ] == 0 && x2RebarOrder[ x2RebarOrder.Count - 1 ] == 0 && // x2RebarOrder[0] == 0 && x2RebarOrder[1] == 0 && x2RebarOrder[x2RebarOrder.Count - 1] == 0 && x2RebarOrder[x2RebarOrder.Count - 2] == 0
               y2RebarOrder[ 0 ] == 0 && y2RebarOrder[ y2RebarOrder.Count - 1 ] == 0 ) {
            if ( flag2 == 1 ) {
              p1 = new Revit.DB.XYZ( x2LeftTopPoint.X, y2LeftTopPoint.Y, 0 ) ;
              p2 = new Revit.DB.XYZ( x2RightTopPoint.X, y2RightTopPoint.Y, 0 ) ;
              p3 = new Revit.DB.XYZ( x2LeftBottomPoint.X, y2LeftBottomPoint.Y, 0 ) ;
              p4 = new Revit.DB.XYZ( x2RightBottomPoint.X, y2RightBottomPoint.Y, 0 ) ;

              syukinHutokeiPoints.Add( p1 ) ;
              syukinHutokeiPoints.Add( p2 ) ;
              syukinHutokeiPoints.Add( p3 ) ;
              syukinHutokeiPoints.Add( p4 ) ;

              seted2ndCornerRebar = true ;
            }
          }

          // X2段筋

          #region

          // X2段筋本数
          int x2RebarCount = syukinX2HutokeiHonsu + syukinX2HosokeiHonsu ;

          // 条件
          if ( syukinX2HutokeiHonsu >= 2 && syukinY1HutokeiHonsu >= 4 && syukinX1HutokeiHonsu >= syukinX2HutokeiHonsu && x1RebarCount >= x2RebarCount && syukinX2HutokeiHonsu > syukinX2HosokeiHonsu ) {
            // X2段筋記号中心間距離 == X1段筋記号中心間距離
            double x2RebarDistance = 0 ;

            if ( y2LeftBottomPoint != null && y2LeftTopPoint != null ) {
              x2RebarDistance = _CmpGeometry.Distance2D( y2LeftBottomPoint, y2LeftTopPoint ) / ( x2RebarCount - 1 ) ;
            }
            else {
              x2RebarDistance = _CmpGeometry.Distance2D( leftBottomRebar, leftTopRebar ) / ( x2RebarCount - 1 ) ;
            }

            // 基準に四隅は含まない
            Collections.Generic.IList<Revit.DB.XYZ> hutoSeted = new Collections.Generic.List<Revit.DB.XYZ>() ;
            Collections.Generic.IList<Revit.DB.XYZ> hosoSeted = new Collections.Generic.List<Revit.DB.XYZ>() ;

            // 1段筋に細径がない場合
            if ( syukinX1HosokeiHonsu < 1 ) {
              _CmpGeometry.Set2ndRebar_No1danHoso( x2RebarOrder, syukinX1HutoPoints, ref hutoSeted, ref hosoSeted, center, true, false ) ;

              // 直交方向2段筋なし
              if ( syukinY2HutokeiHonsu == 0 ) {
                hutoSeted.Clear() ;
                hosoSeted.Clear() ;

                Collections.Generic.IList<Revit.DB.XYZ> defX1Pnts = new Collections.Generic.List<Revit.DB.XYZ>() ;
                foreach ( Revit.DB.XYZ defPnt in syukinX1HutoPoints ) {
                  defX1Pnts.Add( defPnt ) ;
                }

                // 四隅
                defX1Pnts.Add( leftBottomRebar ) ;
                defX1Pnts.Add( leftTopRebar ) ;

                _CmpGeometry.Set2ndRebar_No1danHoso( x2RebarOrder, SortByYAry( defX1Pnts ), ref hutoSeted, ref hosoSeted, center, true, false ) ;
              }

              // コーナー配筋あり判定
              if ( seted2ndCornerRebar == true ) {
                hutoSeted.Clear() ;
                hosoSeted.Clear() ;

                _CmpGeometry.Set2ndRebar_No1danHoso( x2RebarOrder, syukinX1HutoPoints, ref hutoSeted, ref hosoSeted, center, true, true ) ;
              }
            }
            else {
              hutoSeted = _CmpGeometry.Set2ndHutoRebar( syukinX2HutokeiHonsu, SortByYAry( syukinX1HutoPoints ), true ) ;
              hosoSeted = _CmpGeometry.Set2ndHosoRebar( syukinX2HosokeiHonsu, syukinX1Points, hutoSeted, center, true ) ;

              // 直交方向2段筋なし
              if ( syukinY2HutokeiHonsu == 0 ) {
                hutoSeted.Clear() ;
                hosoSeted.Clear() ;

                Collections.Generic.IList<Revit.DB.XYZ> defX1Pnts = new Collections.Generic.List<Revit.DB.XYZ>() ;
                foreach ( Revit.DB.XYZ defPnt in syukinX1HutoPoints ) {
                  defX1Pnts.Add( defPnt ) ;
                }

                // 四隅
                defX1Pnts.Add( leftBottomRebar ) ;
                defX1Pnts.Add( leftTopRebar ) ;

                hutoSeted = _CmpGeometry.Set2ndHutoRebar( syukinX2HutokeiHonsu, SortByYAry( defX1Pnts ), true ) ;
                hosoSeted = _CmpGeometry.Set2ndHosoRebar( syukinX2HosokeiHonsu, syukinX1Points, hutoSeted, center, true ) ;
              }

              // コーナー配筋あり判定
              if ( seted2ndCornerRebar == true ) {
                hutoSeted = _CmpGeometry.Set2ndHutoRebar( syukinX2HutokeiHonsu - 2, SortByYAry( syukinX1HutoPoints ), true ) ;
                hosoSeted = _CmpGeometry.Set2ndHosoRebar( syukinX2HosokeiHonsu, syukinX1Points, hutoSeted, center, true ) ;
              }
            }

            for ( int i = 0 ; i < hutoSeted.Count ; ++i ) {
              Revit.DB.XYZ p = hutoSeted[ i ] ;
              syukinHutokeiPoints.Add( new Revit.DB.XYZ( x2LeftBottomPoint.X, p.Y, p.Z ) ) ;
              syukinHutokeiPoints.Add( new Revit.DB.XYZ( x2RightBottomPoint.X, p.Y, p.Z ) ) ;
            }

            for ( int i = 0 ; i < hosoSeted.Count ; ++i ) {
              Revit.DB.XYZ p = hosoSeted[ i ] ;
              syukinHosokeiPoints.Add( new Revit.DB.XYZ( x2LeftBottomPoint.X, p.Y, p.Z ) ) ;
              syukinHosokeiPoints.Add( new Revit.DB.XYZ( x2RightBottomPoint.X, p.Y, p.Z ) ) ;
            }
          }

          #endregion

          // Y2段筋

          #region

          // Y2段筋本数
          int y2RebarCount = syukinY2HutokeiHonsu + syukinY2HosokeiHonsu ;

          // 条件
          if ( syukinY2HutokeiHonsu >= 2 && syukinX1HutokeiHonsu >= 4 && syukinY1HutokeiHonsu >= syukinY2HutokeiHonsu && y1RebarCount >= y2RebarCount && syukinY2HutokeiHonsu > syukinY2HosokeiHonsu ) {
            // Y2段筋記号中心間距離
            double y2RebarDistance = 0 ; // _CmpGeometry.Distance2D(x2LeftTopPoint, x2RightTopPoint) / (y2RebarCount - 1);

            if ( x2LeftTopPoint != null && x2RightTopPoint != null ) {
              y2RebarDistance = _CmpGeometry.Distance2D( x2LeftTopPoint, x2RightTopPoint ) / ( y2RebarCount - 1 ) ;
            }
            else {
              y2RebarDistance = _CmpGeometry.Distance2D( leftTopRebar, rightTopRebar ) / ( y2RebarCount - 1 ) ;
            }

            // Y2段筋中間距離
            //double y2centerDistance = y1centerDistance;

            // ---- 06/10 -----

            // 基準に四隅は含まない
            Collections.Generic.IList<Revit.DB.XYZ> hutoSeted = new Collections.Generic.List<Revit.DB.XYZ>() ;
            Collections.Generic.IList<Revit.DB.XYZ> hosoSeted = new Collections.Generic.List<Revit.DB.XYZ>() ;

            // 1段筋に細径がない場合
            if ( syukinY1HosokeiHonsu < 1 ) {
              _CmpGeometry.Set2ndRebar_No1danHoso( y2RebarOrder, syukinY1HutoPoints, ref hutoSeted, ref hosoSeted, center, false, false ) ;

              // 直交方向2段筋なし
              if ( syukinX2HutokeiHonsu == 0 ) {
                hutoSeted.Clear() ;
                hosoSeted.Clear() ;

                Collections.Generic.IList<Revit.DB.XYZ> defY1Pnts = new Collections.Generic.List<Revit.DB.XYZ>() ;
                foreach ( Revit.DB.XYZ defPnt in syukinY1HutoPoints ) {
                  defY1Pnts.Add( defPnt ) ;
                }

                // 四隅
                defY1Pnts.Add( leftTopRebar ) ;
                defY1Pnts.Add( rightTopRebar ) ;

                _CmpGeometry.Set2ndRebar_No1danHoso( y2RebarOrder, SortByXAry( defY1Pnts ), ref hutoSeted, ref hosoSeted, center, false, false ) ;
              }

              // コーナー配筋あり判定
              if ( seted2ndCornerRebar == true ) {
                hutoSeted.Clear() ;
                hosoSeted.Clear() ;

                _CmpGeometry.Set2ndRebar_No1danHoso( y2RebarOrder, syukinY1HutoPoints, ref hutoSeted, ref hosoSeted, center, false, true ) ;
              }
            }
            else {
              hutoSeted = _CmpGeometry.Set2ndHutoRebar( syukinY2HutokeiHonsu, SortByXAry( syukinY1HutoPoints ), false ) ;
              hosoSeted = _CmpGeometry.Set2ndHosoRebar( syukinY2HosokeiHonsu, syukinY1Points, hutoSeted, center, false ) ;

              // 直交方向2段筋なし
              if ( syukinX2HutokeiHonsu == 0 ) {
                hutoSeted.Clear() ;
                hosoSeted.Clear() ;

                Collections.Generic.IList<Revit.DB.XYZ> defY1Pnts = new Collections.Generic.List<Revit.DB.XYZ>() ;
                foreach ( Revit.DB.XYZ defPnt in syukinY1HutoPoints ) {
                  defY1Pnts.Add( defPnt ) ;
                }

                defY1Pnts.Add( leftTopRebar ) ;
                defY1Pnts.Add( rightTopRebar ) ;

                hutoSeted = _CmpGeometry.Set2ndHutoRebar( syukinY2HutokeiHonsu, SortByXAry( defY1Pnts ), false ) ;
                hosoSeted = _CmpGeometry.Set2ndHosoRebar( syukinY2HosokeiHonsu, syukinY1Points, hutoSeted, center, false ) ;
              }

              // コーナー配筋あり判定
              if ( seted2ndCornerRebar == true ) {
                hutoSeted = _CmpGeometry.Set2ndHutoRebar( syukinY2HutokeiHonsu - 2, SortByXAry( syukinY1HutoPoints ), false ) ;
                hosoSeted = _CmpGeometry.Set2ndHosoRebar( syukinY2HosokeiHonsu, syukinY1Points, hutoSeted, center, false ) ;

                //y2RebarDistance = (_CmpGeometry.Distance2D(leftTopRebar, rightTopRebar) - diaSyukinHuto1_5 * 2) / (y2RebarCount - 1);
                //y2SumDistance += diaSyukinHuto1_5;
              }
            }

            for ( int i = 0 ; i < hutoSeted.Count ; ++i ) {
              Revit.DB.XYZ p = hutoSeted[ i ] ;
              syukinHutokeiPoints.Add( new Revit.DB.XYZ( p.X, y2LeftTopPoint.Y, p.Z ) ) ;
              syukinHutokeiPoints.Add( new Revit.DB.XYZ( p.X, y2LeftBottomPoint.Y, p.Z ) ) ;
            }

            for ( int i = 0 ; i < hosoSeted.Count ; ++i ) {
              Revit.DB.XYZ p = hosoSeted[ i ] ;
              syukinHosokeiPoints.Add( new Revit.DB.XYZ( p.X, y2LeftTopPoint.Y, p.Z ) ) ;
              syukinHosokeiPoints.Add( new Revit.DB.XYZ( p.X, y2LeftBottomPoint.Y, p.Z ) ) ;
            }

            // ---- 06/10 -----

            // 配筋
            //for (int i = 0; i < y2RebarOrder.Count; ++i)
            //{
            //  int now = y2RebarOrder[i];
            //  Revit.DB.XYZ point = null;

            //  // 端部
            //  if (i == 0 || i == y2RebarOrder.Count - 1)
            //  {
            //    continue;
            //  }
            //  // 2段筋コーナー配筋あり
            //  else if (seted2ndCornerRebar == true && (i == 1 || i == y2RebarOrder.Count - 2))
            //  {
            //    continue;
            //  }
            //  // ほか
            //  else
            //  {
            //    y2SumDistance += y2RebarDistance;

            //    point = new Revit.DB.XYZ(x2LeftBottomPoint.X + y2SumDistance, y2LeftTopPoint.Y, y2LeftTopPoint.Z);
            //    if (now == 0)
            //    {
            //      _CmpGeometry.Move2ndRebar(ref point, syukinY1HutoPoints, false);
            //      syukinHutokeiPoints.Add(point);
            //    }
            //    if (now == 1)
            //    {
            //      _CmpGeometry.Move2ndRebar(ref point, syukinY1HosoPoints, false);
            //      syukinHosokeiPoints.Add(point);
            //    }

            //    point = new Revit.DB.XYZ(x2LeftBottomPoint.X + y2SumDistance, y2LeftBottomPoint.Y, y2LeftBottomPoint.Z);
            //    if (now == 0)
            //    {
            //      _CmpGeometry.Move2ndRebar(ref point, syukinY1HutoPoints, false);
            //      syukinHutokeiPoints.Add(point);
            //    }
            //    if (now == 1)
            //    {
            //      _CmpGeometry.Move2ndRebar(ref point, syukinY1HosoPoints, false);
            //      syukinHosokeiPoints.Add(point);
            //    }
            //  }
            //}
          }

          #endregion
        }
        // 寄せ筋が細径のため2段筋が配置できない
        else {
          // X2段筋本数
          int x2RebarCount = syukinX2HutokeiHonsu + syukinX2HosokeiHonsu ;
          // Y2段筋本数
          int y2RebarCount = syukinY2HutokeiHonsu + syukinY2HosokeiHonsu ;

          // X2条件
          if ( syukinX2HutokeiHonsu >= 2 && syukinY1HutokeiHonsu <= 3 && syukinX1HutokeiHonsu >= syukinX2HutokeiHonsu && x1RebarCount >= x2RebarCount && syukinX2HutokeiHonsu > syukinX2HosokeiHonsu ) {
            ret = _CmpAttribute.ResourceText( "IDS_ERR_2NDREBAR_COULDNOT" ) ;
            goto draw ;
          }

          // Y2条件
          if ( syukinY2HutokeiHonsu >= 2 && syukinX1HutokeiHonsu <= 3 && syukinY1HutokeiHonsu >= syukinY2HutokeiHonsu && y1RebarCount >= y2RebarCount && syukinY2HutokeiHonsu > syukinY2HosokeiHonsu ) {
            ret = _CmpAttribute.ResourceText( "IDS_ERR_2NDREBAR_COULDNOT" ) ;
            goto draw ;
          }
        }

        #region 帯筋 エラー判定

        // X方向
        if ( hoopXhonsu < 2 || syukinX1HutokeiHonsu + syukinX1HosokeiHonsu < hoopXhonsu ) {
          goto draw ;
        }

        // Y方向
        if ( hoopYhonsu < 2 || syukinY1HutokeiHonsu + syukinY1HosokeiHonsu < hoopYhonsu ) {
          goto draw ;
        }

        if ( hoopXkei == "" ) {
          goto draw ;
        }

        if ( hoopPitch == 0 ) {
          goto draw ;
        }

        #endregion

        // 帯筋

        #region

        // 帯筋使用済X1段筋位置
        Collections.Generic.IList<Revit.DB.XYZ> hoopUsedX1Point = new Collections.Generic.List<Revit.DB.XYZ>() ;
        // 帯筋使用済Y1段筋位置
        Collections.Generic.IList<Revit.DB.XYZ> hoopUsedY1Point = new Collections.Generic.List<Revit.DB.XYZ>() ;

        // X方向
        if ( hoopXhonsu >= 2 && x1RebarCount >= hoopXhonsu ) {
          // 帯筋外周部
          hoopLine = _CmpElements.CreateBoundLine( leftBottom, rightBottom ) ;
          _CmpElements.NotNullCurveSet( ref hoopCrvAry, hoopLine ) ;
          hoopLine = _CmpElements.CreateBoundLine( rightTop, leftTop ) ;
          _CmpElements.NotNullCurveSet( ref hoopCrvAry, hoopLine ) ;

          if ( hoopXhonsu > 2 ) {
            // 1段筋主筋端距離
            double distance = _CmpGeometry.Distance2D( leftBottomRebar, leftTopRebar ) ;

            // 1段筋主筋端中間位置
            //Revit.DB.XYZ x1CenterPoint = leftTopRebar + new Revit.DB.XYZ(distance / 2, 0, 0);

            // 帯筋基準間隔
            double pitch = distance / ( hoopXhonsu - 1 ) ;

            // 均等間隔で配置後、
            // 中間より前か後かでずらす方向を決める
            for ( int i = 0 ; i < hoopXhonsu - 2 ; ++i ) {
              // X方向の基準は、左下と右下の主筋
              Revit.DB.XYZ hoopLeftPoint = leftBottomRebar + new Revit.DB.XYZ( 0, pitch * ( i + 1 ), 0 ) ;
              Revit.DB.XYZ hoopRightPoint = rightBottomRebar + new Revit.DB.XYZ( 0, pitch * ( i + 1 ), 0 ) ;

              bool before = true ;
              // 近似または1つ外の点
              hoopLeftPoint = HoopNearYPoint( hoopLeftPoint, syukinX1Points, ref before ) ;
              hoopRightPoint = HoopNearYPoint( hoopRightPoint, syukinX1Points, ref before ) ;

              double diaHalf = diaSyukinHuto / 2 ;
              if ( before == true ) {
                diaHalf *= -1 ;
              }

              hoopUsedX1Point.Add( new Revit.DB.XYZ( hoopLeftPoint.X, hoopLeftPoint.Y, hoopLeftPoint.Z ) ) ;
              // hoopUsedX1Point.Add(new Revit.DB.XYZ(hoopLeftPoint.X, hoopLeftPoint.Y + diaHalf, hoopLeftPoint.Z));

              Revit.DB.XYZ hoopLeft = new Revit.DB.XYZ( leftBottomRebar.X - diaSyukinHuto / 2, hoopLeftPoint.Y + diaHalf, hoopLeftPoint.Z ) ;
              Revit.DB.XYZ hoopRight = new Revit.DB.XYZ( rightBottomRebar.X + diaSyukinHuto / 2, hoopRightPoint.Y + diaHalf, hoopRightPoint.Z ) ;

              hoopLine = _CmpElements.CreateBoundLine( hoopLeft, hoopRight ) ;
              _CmpElements.NotNullCurveSet( ref hoopCrvAry, hoopLine ) ;
            }
          }
        }

        // Y方向
        if ( hoopYhonsu >= 2 && y1RebarCount >= hoopYhonsu ) {
          // 帯筋外周部
          hoopLine = _CmpElements.CreateBoundLine( leftTop, leftBottom ) ;
          _CmpElements.NotNullCurveSet( ref hoopCrvAry, hoopLine ) ;
          hoopLine = _CmpElements.CreateBoundLine( rightBottom, rightTop ) ;
          _CmpElements.NotNullCurveSet( ref hoopCrvAry, hoopLine ) ;

          if ( hoopYhonsu > 2 ) {
            double distance = _CmpGeometry.Distance2D( leftTopRebar, rightTopRebar ) ;
            double pitch = distance / ( hoopYhonsu - 1 ) ;

            for ( int i = 0 ; i < hoopYhonsu - 2 ; ++i ) {
              // Y方向の基準は、左下と左上
              Revit.DB.XYZ hoopBottomPoint = leftBottomRebar + new Revit.DB.XYZ( pitch * ( i + 1 ), 0, 0 ) ;
              Revit.DB.XYZ hoopTopPoint = leftTopRebar + new Revit.DB.XYZ( pitch * ( i + 1 ), 0, 0 ) ;

              bool before = true ;

              hoopBottomPoint = HoopNearXPoint( hoopBottomPoint, syukinY1Points, ref before ) ;
              hoopTopPoint = HoopNearXPoint( hoopTopPoint, syukinY1Points, ref before ) ;

              // hoopUsedY1Point.Add(hoopBottomPoint);

              double diaHalf = diaSyukinHuto / 2 ;
              if ( before == true ) {
                diaHalf *= -1 ;
              }

              hoopUsedY1Point.Add( new Revit.DB.XYZ( hoopBottomPoint.X, hoopBottomPoint.Y, hoopBottomPoint.Z ) ) ;
              //hoopUsedY1Point.Add(new Revit.DB.XYZ(hoopBottomPoint.X + diaHalf, hoopBottomPoint.Y, hoopBottomPoint.Z));

              Revit.DB.XYZ hoopBottom = new Revit.DB.XYZ( hoopBottomPoint.X + diaHalf, leftBottomRebar.Y - diaSyukinHuto / 2, hoopBottomPoint.Z ) ;
              Revit.DB.XYZ hoopTop = new Revit.DB.XYZ( hoopTopPoint.X + diaHalf, leftTopRebar.Y + diaSyukinHuto / 2, hoopTopPoint.Z ) ;

              hoopLine = _CmpElements.CreateBoundLine( hoopBottom, hoopTop ) ;
              _CmpElements.NotNullCurveSet( ref hoopCrvAry, hoopLine ) ;
            }
          }
        }

        #endregion

        #region 幅止筋 エラー判定

        if ( spacing_XDirectionNumber > syukinX1HutokeiHonsu + syukinX1HosokeiHonsu - 2 ) {
          goto draw ;
        }

        if ( spacing_YDirectionNumber > syukinY1HutokeiHonsu + syukinY1HosokeiHonsu - 2 ) {
          goto draw ;
        }

        if ( spacing_XDirectionNumber + hoopXhonsu > syukinX1HutokeiHonsu + syukinX1HosokeiHonsu ) {
          goto draw ;
        }

        if ( spacing_YDirectionNumber + hoopYhonsu > syukinY1HutokeiHonsu + syukinY1HosokeiHonsu ) {
          goto draw ;
        }

        //if (habadomekei == "")
        //{
        //  goto draw;
        //}
        //if (habadomePitch == 0)
        //{
        //  goto draw;
        //}

        #endregion

        // 幅止筋

        #region

        // X方向
        if ( x1RebarCount - hoopXhonsu >= spacing_XDirectionNumber ) {
          for ( int i = 0 ; i < spacing_XDirectionNumber ; ++i ) {
            // 基準位置
            Revit.DB.XYZ spacePoint = SpaceRebarPoint( syukinX1Points, hoopUsedX1Point, diaSyukinHuto, true ) ;
            hoopUsedX1Point.Add( spacePoint ) ;

            // 鉄筋径の半分ずらす
            double halfDia = diaSyukinHuto / 2 ;
            if ( spacePoint.Y <= center.Y ) {
              halfDia *= -1 ;
            }

            spacePoint = new Revit.DB.XYZ( spacePoint.X, spacePoint.Y + halfDia, spacePoint.Z ) ;

            double length = _CmpGeometry.Distance2D( leftTopRebar, rightTopRebar ) ;

            Revit.DB.XYZ spacePoint2 = new Revit.DB.XYZ( spacePoint.X + length, spacePoint.Y, spacePoint.Z ) ;

            spaceLine = _CmpElements.CreateBoundLine( spacePoint + new Revit.DB.XYZ( -diaSyukinHuto / 2, 0, 0 ), spacePoint2 + new Revit.DB.XYZ( diaSyukinHuto / 2, 0, 0 ) ) ;
            _CmpElements.NotNullCurveSet( ref spaceCrvAry, spaceLine ) ;
          }
        }

        // Y方向
        if ( y1RebarCount - hoopYhonsu >= spacing_YDirectionNumber ) {
          for ( int i = 0 ; i < spacing_YDirectionNumber ; ++i ) {
            // 基準位置
            Revit.DB.XYZ spacePoint = SpaceRebarPoint( syukinY1Points, hoopUsedY1Point, diaSyukinHuto, false ) ;
            hoopUsedY1Point.Add( spacePoint ) ;

            // 鉄筋径の半分ずらす
            double halfDia = diaSyukinHuto / 2 ;
            if ( spacePoint.X <= center.X ) {
              halfDia *= -1 ;
            }

            spacePoint = new Revit.DB.XYZ( spacePoint.X + halfDia, spacePoint.Y, spacePoint.Z ) ;

            double length = _CmpGeometry.Distance2D( leftBottomRebar, leftTopRebar ) ;

            Revit.DB.XYZ spacePoint2 = new Revit.DB.XYZ( spacePoint.X, spacePoint.Y - length, spacePoint.Z ) ;

            spaceLine = _CmpElements.CreateBoundLine( spacePoint + new Revit.DB.XYZ( 0, diaSyukinHuto / 2, 0 ), spacePoint2 + new Revit.DB.XYZ( 0, -diaSyukinHuto / 2, 0 ) ) ;
            _CmpElements.NotNullCurveSet( ref spaceCrvAry, spaceLine ) ;
          }
        }

        #endregion

        #region 芯鉄筋 エラー判定

        if ( coreRebarNum > 0 ) {
          if ( coreRebarNum % 2 != 0 || coreRebarNum < 4 ) {
            goto draw ;
          }

          if ( syukinX1HosokeiHonsu + syukinX1HutokeiHonsu < 4 && sintekkinIchiY == 0d ) {
            goto draw ;
          }

          if ( syukinY1HosokeiHonsu + syukinY1HutokeiHonsu < 4 && sintekkinIchiX == 0d ) {
            goto draw ;
          }

          // 4または5本
          if ( syukinX1HutokeiHonsu + syukinX1HosokeiHonsu == 4 || syukinX1HutokeiHonsu + syukinX1HosokeiHonsu == 5 ) {
            // 位置指定なし
            if ( sintekkinIchiY == 0d ) {
              // 直交方向2段筋あり
              if ( syukinY2HutokeiHonsu > 1 ) {
                goto draw ;
              }
            }
          }

          if ( syukinY1HutokeiHonsu + syukinY1HosokeiHonsu == 4 || syukinY1HutokeiHonsu + syukinY1HosokeiHonsu == 5 ) {
            // 位置指定なし
            if ( sintekkinIchiX == 0d ) {
              // 直交方向2段筋あり
              if ( syukinX2HutokeiHonsu > 1 ) {
                goto draw ;
              }
            }
          }
        }

        if ( sintekkinkei == "" ) {
          goto draw ;
        }

        #endregion

        // 芯鉄筋

        #region

        // 4本以上の偶数
        if ( coreRebarNum >= 4 && coreRebarNum % 2 == 0 ) {
          int xCoreRebarNum = 0 ;
          int yCoreRebarNum = 0 ;
          CoreRebarXYDivision( coreRebarNum, x, y, ref xCoreRebarNum, ref yCoreRebarNum ) ;

          // 四隅
          Revit.DB.XYZ coreLeftTop = null ;
          Revit.DB.XYZ coreLeftBottom = null ;
          Revit.DB.XYZ coreRightTop = null ;
          Revit.DB.XYZ coreRightBottom = null ;

          // 基準点
          Revit.DB.XYZ xCoreRebarStartPoint = null ;
          Revit.DB.XYZ xCoreRebarEndPoint = null ;
          Revit.DB.XYZ yCoreRebarStartPoint = null ;
          Revit.DB.XYZ yCoreRebarEndPoint = null ;

          double yobikei = 0 ;
          string strYobikei = sintekkinkei.Substring( 1 ) ;
          double.TryParse( strYobikei, out yobikei ) ;

          // 最低あき寸法
          double sintekkinDistance = yobikei * 1.5 / 304.8 ;
          if ( sintekkinDistance < 25 / 304.8 ) {
            sintekkinDistance = 25 / 304.8 ;
          }

          // X(縦)方向
          bool getXPoint = true ;

          if ( sintekkinIchiY == 0 ) {
            // 必要最低スパン
            double xSpan = ( xCoreRebarNum - 1 ) * sintekkinDistance ;

            getXPoint = CoreRebarBasePoint( xSpan, syukinX1Points, ref xCoreRebarStartPoint, ref xCoreRebarEndPoint, true ) ;
          }
          else if ( sintekkinIchiY > 0 ) {
            xCoreRebarStartPoint = center + new Revit.DB.XYZ( 0, -y / 2, 0 ) + new Revit.DB.XYZ( 0, sintekkinIchiY, 0 ) ;
            xCoreRebarEndPoint = center + new Revit.DB.XYZ( 0, y / 2, 0 ) + new Revit.DB.XYZ( 0, -sintekkinIchiY, 0 ) ;
          }

          // Y(横)方向
          bool getYPoint = true ;

          if ( sintekkinIchiX == 0 ) {
            // 必要最低スパン
            double ySpan = ( yCoreRebarNum - 1 ) * sintekkinDistance ;

            getYPoint = CoreRebarBasePoint( ySpan, syukinY1Points, ref yCoreRebarStartPoint, ref yCoreRebarEndPoint, false ) ;
          }
          else if ( sintekkinIchiX > 0 ) {
            yCoreRebarStartPoint = center + new Revit.DB.XYZ( -x / 2, 0, 0 ) + new Revit.DB.XYZ( sintekkinIchiX, 0, 0 ) ;
            yCoreRebarEndPoint = center + new Revit.DB.XYZ( x / 2, 0, 0 ) + new Revit.DB.XYZ( -sintekkinIchiX, 0, 0 ) ;
          }

          // XY方向とも取得成功
          if ( getXPoint == true && getYPoint == true ) {
            // 四隅
            coreLeftTop = new Revit.DB.XYZ( yCoreRebarStartPoint.X, xCoreRebarEndPoint.Y, yCoreRebarStartPoint.Z ) ;
            coreLeftBottom = new Revit.DB.XYZ( yCoreRebarStartPoint.X, xCoreRebarStartPoint.Y, yCoreRebarStartPoint.Z ) ;
            coreRightTop = new Revit.DB.XYZ( yCoreRebarEndPoint.X, xCoreRebarEndPoint.Y, yCoreRebarStartPoint.Z ) ;
            coreRightBottom = new Revit.DB.XYZ( yCoreRebarEndPoint.X, xCoreRebarStartPoint.Y, yCoreRebarStartPoint.Z ) ;

            sintekkinPoints.Add( coreLeftTop ) ;
            sintekkinPoints.Add( coreLeftBottom ) ;
            sintekkinPoints.Add( coreRightTop ) ;
            sintekkinPoints.Add( coreRightBottom ) ;

            double dis = _CmpGeometry.Distance2D( coreLeftBottom, coreLeftTop ) ;
            double pitch = dis / ( xCoreRebarNum - 1 ) ;

            if ( sintekkinIchiY == 0 ) {
              for ( int i = 0 ; i < xCoreRebarNum - 2 ; ++i ) {
                Revit.DB.XYZ pnt = coreLeftBottom + new Revit.DB.XYZ( 0, pitch * ( i + 1 ), 0 ) ;
                sintekkinPoints.Add( pnt ) ;

                pnt = coreRightBottom + new Revit.DB.XYZ( 0, pitch * ( i + 1 ), 0 ) ;
                sintekkinPoints.Add( pnt ) ;
              }
            }
            else if ( sintekkinIchiY > 0 ) {
              for ( int i = 0 ; i < xCoreRebarNum - 2 ; ++i ) {
                Revit.DB.XYZ pnt = coreLeftBottom + new Revit.DB.XYZ( 0, pitch * ( i + 1 ), 0 ) ;
                sintekkinPoints.Add( pnt ) ;

                pnt = coreRightBottom + new Revit.DB.XYZ( 0, pitch * ( i + 1 ), 0 ) ;
                sintekkinPoints.Add( pnt ) ;
              }
            }

            dis = _CmpGeometry.Distance2D( coreLeftTop, coreRightTop ) ;
            pitch = dis / ( yCoreRebarNum - 1 ) ;

            if ( sintekkinIchiX == 0 ) {
              for ( int i = 0 ; i < yCoreRebarNum - 2 ; ++i ) {
                Revit.DB.XYZ pnt = coreLeftTop + new Revit.DB.XYZ( pitch * ( i + 1 ), 0, 0 ) ;
                sintekkinPoints.Add( pnt ) ;

                pnt = coreLeftBottom + new Revit.DB.XYZ( pitch * ( i + 1 ), 0, 0 ) ;
                sintekkinPoints.Add( pnt ) ;
              }
            }
            else if ( sintekkinIchiX > 0 ) {
              for ( int i = 0 ; i < yCoreRebarNum - 2 ; ++i ) {
                Revit.DB.XYZ pnt = coreLeftTop + new Revit.DB.XYZ( pitch * ( i + 1 ), 0, 0 ) ;
                sintekkinPoints.Add( pnt ) ;

                pnt = coreLeftBottom + new Revit.DB.XYZ( pitch * ( i + 1 ), 0, 0 ) ;
                sintekkinPoints.Add( pnt ) ;
              }
            }
          }
          // 取得失敗
          else {
          }
        }

        #endregion
      }

      #endregion X方向・Y方向の1段筋の細径本数 < 1段筋の太径本数

      #region X方向・Y方向の1段筋の細径本数 >= 1段筋の太径本数

      else {
        // X1段配筋本数
        int x1RebarCount = syukinX1HutokeiHonsu + syukinX1HosokeiHonsu ;
        // Y1段配筋本数
        int y1RebarCount = syukinY1HutokeiHonsu + syukinY1HosokeiHonsu ;

        // 主筋X1段筋座標
        Collections.Generic.IList<Revit.DB.XYZ> syukinX1Points = new Collections.Generic.List<Revit.DB.XYZ>() ;
        // 主筋Y1段筋座標
        Collections.Generic.IList<Revit.DB.XYZ> syukinY1Points = new Collections.Generic.List<Revit.DB.XYZ>() ;

        // 1段筋の配置
        // 1段筋四隅
        Revit.DB.XYZ leftTopRebar = leftTop + new Revit.DB.XYZ( diaSyukinHuto / 2, -( diaSyukinHuto / 2 ), 0 ) ;
        Revit.DB.XYZ leftBottomRebar = leftBottom + new Revit.DB.XYZ( diaSyukinHuto / 2, diaSyukinHuto / 2, 0 ) ;
        Revit.DB.XYZ rightTopRebar = rightTop + new Revit.DB.XYZ( -( diaSyukinHuto / 2 ), -( diaSyukinHuto / 2 ), 0 ) ;
        Revit.DB.XYZ rightBottomRebar = rightBottom + new Revit.DB.XYZ( -( diaSyukinHuto / 2 ), diaSyukinHuto / 2, 0 ) ;

        syukinX1Points.Add( leftTopRebar ) ;
        syukinX1Points.Add( leftBottomRebar ) ;

        syukinY1Points.Add( leftTopRebar ) ;
        syukinY1Points.Add( rightTopRebar ) ;

        //Sum
        var sumY1 = syukinY1HosokeiHonsu + syukinY1HutokeiHonsu ;
        var sumX1 = syukinX1HosokeiHonsu + syukinX1HutokeiHonsu ;

        double sumDistance = 0 ;
        sumDistance = rightTopRebar.X - leftTopRebar.X ;
        double stepY = sumDistance / ( sumY1 - 1 ) ;

        //////////////////////////////////////////////////////////////////////////
        var listY1 = RebarOrder_Y1( syukinY1HutokeiHonsu, syukinY1HosokeiHonsu ) ;
        var listY2 = RebarOrder_Y2( syukinY2HutokeiHonsu, syukinY2HosokeiHonsu, listY1 ) ;

        //Top
        Calculate_OY( syukinHutokeiPoints, syukinHosokeiPoints, listY1, leftTopRebar.X, leftTopRebar.Y, stepY, syukinY1Points ) ;

        //Bottom
        Calculate_OY( syukinHutokeiPoints, syukinHosokeiPoints, listY1, leftTopRebar.X, rightBottomRebar.Y, stepY, null ) ;

        sumDistance = leftTopRebar.Y - leftBottomRebar.Y ;
        double stepX = sumDistance / ( sumX1 - 1 ) ;

        var listX1 = RebarOrder_X1( syukinX1HutokeiHonsu, syukinX1HosokeiHonsu ) ;
        var listX2 = RebarOrder_X2( syukinX2HutokeiHonsu, syukinX2HosokeiHonsu, listX1 ) ;

        //Left
        Calculate_OX( syukinHutokeiPoints, syukinHosokeiPoints, listX1, leftBottomRebar.Y, leftTopRebar.X, stepX, syukinX1Points ) ;

        //Right
        Calculate_OX( syukinHutokeiPoints, syukinHosokeiPoints, listX1, leftBottomRebar.Y, rightBottomRebar.X, stepX, null ) ;

        //Lop 2

        //Top
        Calculate_OY( syukinHutokeiPoints, syukinHosokeiPoints, listY2, leftTopRebar.X, leftTopRebar.Y - stepX, stepY, null ) ;
        //
        //Bottom
        Calculate_OY( syukinHutokeiPoints, syukinHosokeiPoints, listY2, leftTopRebar.X, rightBottomRebar.Y + stepX, stepY, null ) ;

        //Left
        Calculate_OX( syukinHutokeiPoints, syukinHosokeiPoints, listX2, leftBottomRebar.Y, leftTopRebar.X + stepY, stepX, null ) ;
        //
        //Right
        Calculate_OX( syukinHutokeiPoints, syukinHosokeiPoints, listX2, leftBottomRebar.Y, rightBottomRebar.X - stepY, stepX, null ) ;

        //////////////////////////////////////////////////////////////////////////

        #region 帯筋 エラー判定

        // X方向
        if ( hoopXhonsu < 2 || syukinX1HutokeiHonsu + syukinX1HosokeiHonsu < hoopXhonsu ) {
          goto draw ;
        }

        // Y方向
        if ( hoopYhonsu < 2 || syukinY1HutokeiHonsu + syukinY1HosokeiHonsu < hoopYhonsu ) {
          goto draw ;
        }

        if ( hoopXkei == "" ) {
          goto draw ;
        }

        if ( hoopPitch == 0 ) {
          goto draw ;
        }

        #endregion

        // 帯筋

        #region

        // 帯筋使用済X1段筋位置
        Collections.Generic.IList<Revit.DB.XYZ> hoopUsedX1Point = new Collections.Generic.List<Revit.DB.XYZ>() ;
        // 帯筋使用済Y1段筋位置
        Collections.Generic.IList<Revit.DB.XYZ> hoopUsedY1Point = new Collections.Generic.List<Revit.DB.XYZ>() ;

        // X方向
        if ( hoopXhonsu >= 2 && x1RebarCount >= hoopXhonsu ) {
          // 帯筋外周部
          hoopLine = _CmpElements.CreateBoundLine( leftBottom, rightBottom ) ;
          _CmpElements.NotNullCurveSet( ref hoopCrvAry, hoopLine ) ;
          hoopLine = _CmpElements.CreateBoundLine( rightTop, leftTop ) ;
          _CmpElements.NotNullCurveSet( ref hoopCrvAry, hoopLine ) ;

          if ( hoopXhonsu > 2 ) {
            // 1段筋主筋端距離
            double distance = _CmpGeometry.Distance2D( leftBottomRebar, leftTopRebar ) ;

            // 1段筋主筋端中間位置
            //Revit.DB.XYZ x1CenterPoint = leftTopRebar + new Revit.DB.XYZ(distance / 2, 0, 0);

            // 帯筋基準間隔
            double pitch = distance / ( hoopXhonsu - 1 ) ;

            // 均等間隔で配置後、
            // 中間より前か後かでずらす方向を決める
            for ( int i = 0 ; i < hoopXhonsu - 2 ; ++i ) {
              // X方向の基準は、左下と右下の主筋
              Revit.DB.XYZ hoopLeftPoint = leftBottomRebar + new Revit.DB.XYZ( 0, pitch * ( i + 1 ), 0 ) ;
              Revit.DB.XYZ hoopRightPoint = rightBottomRebar + new Revit.DB.XYZ( 0, pitch * ( i + 1 ), 0 ) ;

              bool before = true ;
              // 近似または1つ外の点
              hoopLeftPoint = HoopNearYPoint( hoopLeftPoint, syukinX1Points, ref before ) ;
              hoopRightPoint = HoopNearYPoint( hoopRightPoint, syukinX1Points, ref before ) ;

              double diaHalf = diaSyukinHuto / 2 ;
              if ( before == true ) {
                diaHalf *= -1 ;
              }

              hoopUsedX1Point.Add( new Revit.DB.XYZ( hoopLeftPoint.X, hoopLeftPoint.Y, hoopLeftPoint.Z ) ) ;
              // hoopUsedX1Point.Add(new Revit.DB.XYZ(hoopLeftPoint.X, hoopLeftPoint.Y + diaHalf, hoopLeftPoint.Z));

              Revit.DB.XYZ hoopLeft = new Revit.DB.XYZ( leftBottomRebar.X - diaSyukinHuto / 2, hoopLeftPoint.Y + diaHalf, hoopLeftPoint.Z ) ;
              Revit.DB.XYZ hoopRight = new Revit.DB.XYZ( rightBottomRebar.X + diaSyukinHuto / 2, hoopRightPoint.Y + diaHalf, hoopRightPoint.Z ) ;

              hoopLine = _CmpElements.CreateBoundLine( hoopLeft, hoopRight ) ;
              _CmpElements.NotNullCurveSet( ref hoopCrvAry, hoopLine ) ;
            }
          }
        }

        // Y方向
        if ( hoopYhonsu >= 2 && y1RebarCount >= hoopYhonsu ) {
          // 帯筋外周部
          hoopLine = _CmpElements.CreateBoundLine( leftTop, leftBottom ) ;
          _CmpElements.NotNullCurveSet( ref hoopCrvAry, hoopLine ) ;
          hoopLine = _CmpElements.CreateBoundLine( rightBottom, rightTop ) ;
          _CmpElements.NotNullCurveSet( ref hoopCrvAry, hoopLine ) ;

          if ( hoopYhonsu > 2 ) {
            double distance = _CmpGeometry.Distance2D( leftTopRebar, rightTopRebar ) ;
            double pitch = distance / ( hoopYhonsu - 1 ) ;

            for ( int i = 0 ; i < hoopYhonsu - 2 ; ++i ) {
              // Y方向の基準は、左下と左上
              Revit.DB.XYZ hoopBottomPoint = leftBottomRebar + new Revit.DB.XYZ( pitch * ( i + 1 ), 0, 0 ) ;
              Revit.DB.XYZ hoopTopPoint = leftTopRebar + new Revit.DB.XYZ( pitch * ( i + 1 ), 0, 0 ) ;

              bool before = true ;

              hoopBottomPoint = HoopNearXPoint( hoopBottomPoint, syukinY1Points, ref before ) ;
              hoopTopPoint = HoopNearXPoint( hoopTopPoint, syukinY1Points, ref before ) ;

              // hoopUsedY1Point.Add(hoopBottomPoint);

              double diaHalf = diaSyukinHuto / 2 ;
              if ( before == true ) {
                diaHalf *= -1 ;
              }

              hoopUsedY1Point.Add( new Revit.DB.XYZ( hoopBottomPoint.X, hoopBottomPoint.Y, hoopBottomPoint.Z ) ) ;
              //hoopUsedY1Point.Add(new Revit.DB.XYZ(hoopBottomPoint.X + diaHalf, hoopBottomPoint.Y, hoopBottomPoint.Z));

              Revit.DB.XYZ hoopBottom = new Revit.DB.XYZ( hoopBottomPoint.X + diaHalf, leftBottomRebar.Y - diaSyukinHuto / 2, hoopBottomPoint.Z ) ;
              Revit.DB.XYZ hoopTop = new Revit.DB.XYZ( hoopTopPoint.X + diaHalf, leftTopRebar.Y + diaSyukinHuto / 2, hoopTopPoint.Z ) ;

              hoopLine = _CmpElements.CreateBoundLine( hoopBottom, hoopTop ) ;
              _CmpElements.NotNullCurveSet( ref hoopCrvAry, hoopLine ) ;
            }
          }
        }

        #endregion

        #region 幅止筋 エラー判定

        if ( spacing_XDirectionNumber > syukinX1HutokeiHonsu + syukinX1HosokeiHonsu - 2 ) {
          goto draw ;
        }

        if ( spacing_YDirectionNumber > syukinY1HutokeiHonsu + syukinY1HosokeiHonsu - 2 ) {
          goto draw ;
        }

        if ( spacing_XDirectionNumber + hoopXhonsu > syukinX1HutokeiHonsu + syukinX1HosokeiHonsu ) {
          goto draw ;
        }

        if ( spacing_YDirectionNumber + hoopYhonsu > syukinY1HutokeiHonsu + syukinY1HosokeiHonsu ) {
          goto draw ;
        }

        //if (habadomekei == "")
        //{
        //  goto draw;
        //}
        //if (habadomePitch == 0)
        //{
        //  goto draw;
        //}

        #endregion

        // 幅止筋

        #region 幅止筋

        // X方向
        if ( x1RebarCount - hoopXhonsu >= spacing_XDirectionNumber ) {
          for ( int i = 0 ; i < spacing_XDirectionNumber ; ++i ) {
            // 基準位置
            Revit.DB.XYZ spacePoint = SpaceRebarPoint( syukinX1Points, hoopUsedX1Point, diaSyukinHuto, true ) ;
            hoopUsedX1Point.Add( spacePoint ) ;

            // 鉄筋径の半分ずらす
            double halfDia = diaSyukinHuto / 2 ;
            if ( spacePoint.Y <= center.Y ) {
              halfDia *= -1 ;
            }

            spacePoint = new Revit.DB.XYZ( spacePoint.X, spacePoint.Y + halfDia, spacePoint.Z ) ;

            double length = _CmpGeometry.Distance2D( leftTopRebar, rightTopRebar ) ;

            Revit.DB.XYZ spacePoint2 = new Revit.DB.XYZ( spacePoint.X + length, spacePoint.Y, spacePoint.Z ) ;

            spaceLine = _CmpElements.CreateBoundLine( spacePoint + new Revit.DB.XYZ( -diaSyukinHuto / 2, 0, 0 ), spacePoint2 + new Revit.DB.XYZ( diaSyukinHuto / 2, 0, 0 ) ) ;
            _CmpElements.NotNullCurveSet( ref spaceCrvAry, spaceLine ) ;
          }
        }

        // Y方向
        if ( y1RebarCount - hoopYhonsu >= spacing_YDirectionNumber ) {
          for ( int i = 0 ; i < spacing_YDirectionNumber ; ++i ) {
            // 基準位置
            Revit.DB.XYZ spacePoint = SpaceRebarPoint( syukinY1Points, hoopUsedY1Point, diaSyukinHuto, false ) ;
            hoopUsedY1Point.Add( spacePoint ) ;

            // 鉄筋径の半分ずらす
            double halfDia = diaSyukinHuto / 2 ;
            if ( spacePoint.X <= center.X ) {
              halfDia *= -1 ;
            }

            spacePoint = new Revit.DB.XYZ( spacePoint.X + halfDia, spacePoint.Y, spacePoint.Z ) ;

            double length = _CmpGeometry.Distance2D( leftBottomRebar, leftTopRebar ) ;

            Revit.DB.XYZ spacePoint2 = new Revit.DB.XYZ( spacePoint.X, spacePoint.Y - length, spacePoint.Z ) ;

            spaceLine = _CmpElements.CreateBoundLine( spacePoint + new Revit.DB.XYZ( 0, diaSyukinHuto / 2, 0 ), spacePoint2 + new Revit.DB.XYZ( 0, -diaSyukinHuto / 2, 0 ) ) ;
            _CmpElements.NotNullCurveSet( ref spaceCrvAry, spaceLine ) ;
          }
        }

        #endregion 幅止筋

        #region 芯鉄筋 エラー判定

        if ( coreRebarNum > 0 ) {
          if ( coreRebarNum % 2 != 0 || coreRebarNum < 4 ) {
            goto draw ;
          }

          if ( syukinX1HosokeiHonsu + syukinX1HutokeiHonsu < 4 && sintekkinIchiY == 0d ) {
            goto draw ;
          }

          if ( syukinY1HosokeiHonsu + syukinY1HutokeiHonsu < 4 && sintekkinIchiX == 0d ) {
            goto draw ;
          }

          // 4または5本
          if ( syukinX1HutokeiHonsu + syukinX1HosokeiHonsu == 4 || syukinX1HutokeiHonsu + syukinX1HosokeiHonsu == 5 ) {
            // 位置指定なし
            if ( sintekkinIchiY == 0d ) {
              // 直交方向2段筋あり
              if ( syukinY2HutokeiHonsu > 1 ) {
                goto draw ;
              }
            }
          }

          if ( syukinY1HutokeiHonsu + syukinY1HosokeiHonsu == 4 || syukinY1HutokeiHonsu + syukinY1HosokeiHonsu == 5 ) {
            // 位置指定なし
            if ( sintekkinIchiX == 0d ) {
              // 直交方向2段筋あり
              if ( syukinX2HutokeiHonsu > 1 ) {
                goto draw ;
              }
            }
          }
        }

        if ( sintekkinkei == "" ) {
          goto draw ;
        }

        #endregion

        // 芯鉄筋

        #region

        // 4本以上の偶数
        if ( coreRebarNum >= 4 && coreRebarNum % 2 == 0 ) {
          int xCoreRebarNum = 0 ;
          int yCoreRebarNum = 0 ;
          CoreRebarXYDivision( coreRebarNum, x, y, ref xCoreRebarNum, ref yCoreRebarNum ) ;

          // 四隅
          Revit.DB.XYZ coreLeftTop = null ;
          Revit.DB.XYZ coreLeftBottom = null ;
          Revit.DB.XYZ coreRightTop = null ;
          Revit.DB.XYZ coreRightBottom = null ;

          // 基準点
          Revit.DB.XYZ xCoreRebarStartPoint = null ;
          Revit.DB.XYZ xCoreRebarEndPoint = null ;
          Revit.DB.XYZ yCoreRebarStartPoint = null ;
          Revit.DB.XYZ yCoreRebarEndPoint = null ;

          double yobikei = 0 ;
          string strYobikei = sintekkinkei.Substring( 1 ) ;
          double.TryParse( strYobikei, out yobikei ) ;

          // 最低あき寸法
          double sintekkinDistance = yobikei * 1.5 / 304.8 ;
          if ( sintekkinDistance < 25 / 304.8 ) {
            sintekkinDistance = 25 / 304.8 ;
          }

          // X(縦)方向
          bool getXPoint = true ;

          if ( sintekkinIchiY == 0 ) {
            // 必要最低スパン
            double xSpan = ( xCoreRebarNum - 1 ) * sintekkinDistance ;

            getXPoint = CoreRebarBasePoint( xSpan, syukinX1Points, ref xCoreRebarStartPoint, ref xCoreRebarEndPoint, true ) ;
          }
          else if ( sintekkinIchiY > 0 ) {
            xCoreRebarStartPoint = center + new Revit.DB.XYZ( 0, -y / 2, 0 ) + new Revit.DB.XYZ( 0, sintekkinIchiY, 0 ) ;
            xCoreRebarEndPoint = center + new Revit.DB.XYZ( 0, y / 2, 0 ) + new Revit.DB.XYZ( 0, -sintekkinIchiY, 0 ) ;
          }

          // Y(横)方向
          bool getYPoint = true ;

          if ( sintekkinIchiX == 0 ) {
            // 必要最低スパン
            double ySpan = ( yCoreRebarNum - 1 ) * sintekkinDistance ;

            getYPoint = CoreRebarBasePoint( ySpan, syukinY1Points, ref yCoreRebarStartPoint, ref yCoreRebarEndPoint, false ) ;
          }
          else if ( sintekkinIchiX > 0 ) {
            yCoreRebarStartPoint = center + new Revit.DB.XYZ( -x / 2, 0, 0 ) + new Revit.DB.XYZ( sintekkinIchiX, 0, 0 ) ;
            yCoreRebarEndPoint = center + new Revit.DB.XYZ( x / 2, 0, 0 ) + new Revit.DB.XYZ( -sintekkinIchiX, 0, 0 ) ;
          }

          // XY方向とも取得成功
          if ( getXPoint == true && getYPoint == true ) {
            // 四隅
            coreLeftTop = new Revit.DB.XYZ( yCoreRebarStartPoint.X, xCoreRebarEndPoint.Y, yCoreRebarStartPoint.Z ) ;
            coreLeftBottom = new Revit.DB.XYZ( yCoreRebarStartPoint.X, xCoreRebarStartPoint.Y, yCoreRebarStartPoint.Z ) ;
            coreRightTop = new Revit.DB.XYZ( yCoreRebarEndPoint.X, xCoreRebarEndPoint.Y, yCoreRebarStartPoint.Z ) ;
            coreRightBottom = new Revit.DB.XYZ( yCoreRebarEndPoint.X, xCoreRebarStartPoint.Y, yCoreRebarStartPoint.Z ) ;

            sintekkinPoints.Add( coreLeftTop ) ;
            sintekkinPoints.Add( coreLeftBottom ) ;
            sintekkinPoints.Add( coreRightTop ) ;
            sintekkinPoints.Add( coreRightBottom ) ;

            double dis = _CmpGeometry.Distance2D( coreLeftBottom, coreLeftTop ) ;
            double pitch = dis / ( xCoreRebarNum - 1 ) ;

            if ( sintekkinIchiY == 0 ) {
              for ( int i = 0 ; i < xCoreRebarNum - 2 ; ++i ) {
                Revit.DB.XYZ pnt = coreLeftBottom + new Revit.DB.XYZ( 0, pitch * ( i + 1 ), 0 ) ;
                sintekkinPoints.Add( pnt ) ;

                pnt = coreRightBottom + new Revit.DB.XYZ( 0, pitch * ( i + 1 ), 0 ) ;
                sintekkinPoints.Add( pnt ) ;
              }
            }
            else if ( sintekkinIchiY > 0 ) {
              for ( int i = 0 ; i < xCoreRebarNum - 2 ; ++i ) {
                Revit.DB.XYZ pnt = coreLeftBottom + new Revit.DB.XYZ( 0, pitch * ( i + 1 ), 0 ) ;
                sintekkinPoints.Add( pnt ) ;

                pnt = coreRightBottom + new Revit.DB.XYZ( 0, pitch * ( i + 1 ), 0 ) ;
                sintekkinPoints.Add( pnt ) ;
              }
            }

            dis = _CmpGeometry.Distance2D( coreLeftTop, coreRightTop ) ;
            pitch = dis / ( yCoreRebarNum - 1 ) ;

            if ( sintekkinIchiX == 0 ) {
              for ( int i = 0 ; i < yCoreRebarNum - 2 ; ++i ) {
                Revit.DB.XYZ pnt = coreLeftTop + new Revit.DB.XYZ( pitch * ( i + 1 ), 0, 0 ) ;
                sintekkinPoints.Add( pnt ) ;

                pnt = coreLeftBottom + new Revit.DB.XYZ( pitch * ( i + 1 ), 0, 0 ) ;
                sintekkinPoints.Add( pnt ) ;
              }
            }
            else if ( sintekkinIchiX > 0 ) {
              for ( int i = 0 ; i < yCoreRebarNum - 2 ; ++i ) {
                Revit.DB.XYZ pnt = coreLeftTop + new Revit.DB.XYZ( pitch * ( i + 1 ), 0, 0 ) ;
                sintekkinPoints.Add( pnt ) ;

                pnt = coreLeftBottom + new Revit.DB.XYZ( pitch * ( i + 1 ), 0, 0 ) ;
                sintekkinPoints.Add( pnt ) ;
              }
            }
          }
          // 取得失敗
          else {
          }
        }

        #endregion
      }

      #endregion X方向・Y方向の1段筋の細径本数 >= 1段筋の太径本数

      draw:

      // 線種
      Revit.DB.GraphicsStyle bodyLineType = _CmpElements.BodyLineStyle ;
      Revit.DB.GraphicsStyle spaceLineType = _CmpElements.SpacerLineStyle ;

      // 帯筋作成
      trans.Start( "帯筋" ) ;
      foreach ( Revit.DB.Curve crv in hoopCrvAry ) {
        Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve( actView, crv ) ;
        dc.LineStyle = bodyLineType ;
      }

      trans.Commit() ;

      // 幅止筋
      trans.Start( "幅止筋" ) ;
      foreach ( Revit.DB.Curve crv in spaceCrvAry ) {
        Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve( actView, crv ) ;
        dc.LineStyle = spaceLineType ;
      }

      trans.Commit() ;

      if ( famSymSyukinHuto != null ) {
        trans.Start( "主筋太径" ) ;
        foreach ( Revit.DB.XYZ pnt in syukinHutokeiPoints ) {
          Revit.DB.FamilyInstance famInsRebar = rvtDbDoc.Create.NewFamilyInstance( pnt, famSymSyukinHuto, actView ) ;
        }

        trans.Commit() ;
      }

      if ( famSymSyukinHoso != null ) {
        trans.Start( "主筋細径" ) ;
        foreach ( Revit.DB.XYZ pnt in syukinHosokeiPoints ) {
          Revit.DB.FamilyInstance famInsRebar = rvtDbDoc.Create.NewFamilyInstance( pnt, famSymSyukinHoso, actView ) ;
        }

        trans.Commit() ;
      }

      if ( famSymSintekkin != null ) {
        trans.Start( "芯鉄筋" ) ;
        foreach ( Revit.DB.XYZ pnt in sintekkinPoints ) {
          Revit.DB.FamilyInstance famInsRebar = rvtDbDoc.Create.NewFamilyInstance( pnt, famSymSintekkin, actView ) ;
        }

        trans.Commit() ;
      }

      return ret ;
    }

    //////////////////////////////////////////////////////////////////////////

    private void Calculate_OX( IList<XYZ> syukinHutokeiPoints, IList<XYZ> syukinHosokeiPoints, List<int> listX, double startY, double dX, double step, Collections.Generic.IList<Revit.DB.XYZ> syukinX1Points )
    {
      double dY = 0 ;
      for ( int i = 0 ; i < listX.Count ; i++ ) {
        int now = listX[ i ] ;
        dY = startY + ( i * step ) ;

        var p = new XYZ( dX, dY, 0 ) ;

        if ( now == 2 )
          continue ;
        else if ( now == 0 ) {
          if ( syukinHutokeiPoints.ToList().Where( item => item.X == dX && item.Y == dY ).ToList().Count == 0 ) {
            syukinHutokeiPoints.Add( p ) ;

            if ( syukinX1Points != null )
              syukinX1Points.Add( p ) ;
          }
        }
        else {
          if ( syukinHosokeiPoints.ToList().Where( item => item.X == dX && item.Y == dY ).ToList().Count == 0 ) {
            syukinHosokeiPoints.Add( p ) ;

            if ( syukinX1Points != null )
              syukinX1Points.Add( p ) ;
          }
        }
      }
    }

    private void Calculate_OY( IList<XYZ> syukinHutokeiPoints, IList<XYZ> syukinHosokeiPoints, List<int> listY, double startX, double dY, double step, Collections.Generic.IList<Revit.DB.XYZ> syukinY1Points )
    {
      double dX = 0 ;
      for ( int i = 0 ; i < listY.Count ; i++ ) {
        int now = listY[ i ] ;
        dX = startX + ( i * step ) ;

        var p = new XYZ( dX, dY, 0 ) ;

        if ( now == 2 )
          continue ;
        else if ( now == 0 ) {
          if ( syukinHutokeiPoints.ToList().Where( item => item.X == dX && item.Y == dY ).ToList().Count == 0 ) {
            syukinHutokeiPoints.Add( p ) ;
            if ( syukinY1Points != null )
              syukinY1Points.Add( p ) ;
          }
        }
        else {
          if ( syukinHosokeiPoints.ToList().Where( item => item.X == dX && item.Y == dY ).ToList().Count == 0 ) {
            syukinHosokeiPoints.Add( p ) ;
            if ( syukinY1Points != null )
              syukinY1Points.Add( p ) ;
          }
        }
      }
    }

    private List<int> RebarOrder_Y2( int syukinY2HutokeiHonsu, int syukinY2HosokeiHonsu, List<int> listY1 )
    {
      List<int> indexs = new List<int>() ;
      if ( syukinY2HutokeiHonsu == 0 && syukinY2HosokeiHonsu == 0 )
        return indexs ;

      int index_y_left = syukinY2HutokeiHonsu / 2 ;
      if ( syukinY2HutokeiHonsu % 2 == 1 )
        index_y_left++ ;

      int start_hoso = -1 ;
      int end_hoso = -1 ;
      for ( int i = 0 ; i < listY1.Count ; i++ ) {
        if ( i < index_y_left || i >= listY1.Count - syukinY2HutokeiHonsu / 2 ) {
          indexs.Add( listY1[ i ] ) ;
        }
        else
          indexs.Add( 2 ) ;

        if ( start_hoso == -1 && listY1[ i ] == 1 )
          start_hoso = i ;

        if ( end_hoso == -1 && listY1[ i ] == 0 && start_hoso != -1 )
          end_hoso = i - 1 ;
      }

      if ( start_hoso == -1 || end_hoso == -1 )
        return indexs ;

      int count = 0 ;
      int index = end_hoso ;
      for ( int i = start_hoso ; i <= index ; i++ ) {
        if ( count < syukinY2HosokeiHonsu ) {
          indexs[ i ] = 1 ;
          count++ ;
        }

        if ( count < syukinY2HosokeiHonsu ) {
          indexs[ index-- ] = 1 ;

          count++ ;
        }

        if ( count >= syukinY2HosokeiHonsu )
          break ;
      }

      return indexs ;
    }

    private List<int> RebarOrder_X2( int syukinX2HutokeiHonsu, int syukinX2HosokeiHonsu, List<int> listX1 )
    {
      List<int> indexs = new List<int>() ;
      if ( syukinX2HutokeiHonsu == 0 && syukinX2HosokeiHonsu == 0 )
        return indexs ;

      int index_y_left = syukinX2HutokeiHonsu / 2 ;
      if ( syukinX2HutokeiHonsu % 2 == 1 )
        index_y_left++ ;

      int start_hoso = -1 ;
      int end_hoso = -1 ;
      for ( int i = 0 ; i < listX1.Count ; i++ ) {
        if ( i < index_y_left || i >= listX1.Count - syukinX2HutokeiHonsu / 2 ) {
          indexs.Add( listX1[ i ] ) ;
        }
        else
          indexs.Add( 2 ) ;

        if ( start_hoso == -1 && listX1[ i ] == 1 )
          start_hoso = i ;

        if ( end_hoso == -1 && listX1[ i ] == 0 && start_hoso != -1 )
          end_hoso = i - 1 ;
      }

      if ( start_hoso == -1 || end_hoso == -1 )
        return indexs ;

      int count = 0 ;
      int index = end_hoso ;
      for ( int i = start_hoso ; i <= index ; i++ ) {
        if ( count < syukinX2HosokeiHonsu ) {
          indexs[ i ] = 1 ;
          count++ ;
        }

        if ( count < syukinX2HosokeiHonsu ) {
          indexs[ index-- ] = 1 ;

          count++ ;
        }

        if ( count >= syukinX2HosokeiHonsu )
          break ;
      }

      return indexs ;
    }

    private List<int> RebarOrder_Y1( int syukinY1HutokeiHonsu, int syukinY1HosokeiHonsu )
    {
      List<int> indexs = new List<int>() ;

      for ( int i = 0 ; i < syukinY1HutokeiHonsu / 2 ; i++ ) {
        indexs.Add( 0 ) ;
      }

      if ( syukinY1HutokeiHonsu % 2 == 1 )
        indexs.Add( 0 ) ;

      for ( int i = 0 ; i < syukinY1HosokeiHonsu ; i++ ) {
        indexs.Add( 1 ) ;
      }

      for ( int i = 0 ; i < syukinY1HutokeiHonsu / 2 ; i++ ) {
        indexs.Add( 0 ) ;
      }

      return indexs ;
    }

    private List<int> RebarOrder_X1( int syukinX1HutokeiHonsu, int syukinX1HosokeiHonsu )
    {
      List<int> indexs = new List<int>() ;

      for ( int i = 0 ; i < syukinX1HutokeiHonsu / 2 ; i++ ) {
        indexs.Add( 0 ) ;
      }

      if ( syukinX1HutokeiHonsu % 2 == 1 )
        indexs.Add( 0 ) ;

      for ( int i = 0 ; i < syukinX1HosokeiHonsu ; i++ ) {
        indexs.Add( 1 ) ;
      }

      for ( int i = 0 ; i < syukinX1HutokeiHonsu / 2 ; i++ ) {
        indexs.Add( 0 ) ;
      }

      return indexs ;
    }

    //////////////////////////////////////////////////////////////////////////

    /// ================================================================================
    /// <summary>円柱配筋</summary>
    ///
    /// <history><p>2013/05/01 Created  GSA,Inc. Ryo Kuroda</p>
    ///           <p>2015/07/22 Modified GSA, Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public string CreateRebar_En( System.Data.DataTable data, int rowNum, Revit.DB.XYZ center, bool isTop, Revit.UI.UIApplication rvtUiApp )
    {
      Revit.DB.Document rvtDbDoc = rvtUiApp.ActiveUIDocument.Document ;
      Revit.UI.UIDocument rvtUiDoc = rvtUiApp.ActiveUIDocument ;
      Revit.ApplicationServices.Application rvtSrvcApp = rvtUiApp.Application ;

      string ret = "" ;

      // かぶり厚
      double kaburi_En = 0 ;
      double.TryParse( _CmpParameters.CylinderProtectThick, out kaburi_En ) ;

      // 鉄筋ファミリ
      Revit.DB.Family rebarFam = null ;
      bool isHaveFam = _CmpElements.GetRebarFamily( ref rebarFam ) ;

      if ( isHaveFam == false ) {
        ret = _CmpAttribute.ResourceText( "IDS_ERR_NOREBARFAMILY" ) ;
        return ret ;
      }

      // 直径
      double diameter = (double)data.Rows[ rowNum ][ _CmpParameters.Tyokkei_En ] ;

      // 芯鉄筋径
      string sintekkinkei = (string)data.Rows[ rowNum ][ _CmpParameters.RST_SintekkinKei_En ] ;
      // 芯鉄筋本数
      int coreRebarNum = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SintekkinHonsu_En ] ;
      // 芯鉄筋位置
      double sintekkinIchi = (double)data.Rows[ rowNum ][ _CmpParameters.RST_SintekkinIchi_En ] ;
      // 幅止筋径
      string habadomekei = (string)data.Rows[ rowNum ][ _CmpParameters.RST_HabadomekinKei_En ] ;
      // 幅止筋ピッチ
      double habadomePitch = (double)data.Rows[ rowNum ][ _CmpParameters.RST_HabadomekinPitch_En ] ;

      // 主筋径
      string syukinKei = "" ;
      // 主筋太径本数
      int syukinHonsu = 0 ;
      // 帯筋径
      string hoopKei = "" ;
      // 帯筋ピッチ
      double hoopPitch = 0 ;
      // Spacing_XDirectionNumber
      int spacing_XDirectionNumber = 0 ;
      // Spacing_YDirectionNumber
      int spacing_YDirectionNumber = 0 ;

      if ( isTop == true ) {
        syukinKei = (string)data.Rows[ rowNum ][ _CmpParameters.RST_ChutoSyukinKei_En ] ;
        syukinHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_ChutoSyukinHonsu_En ] ;
        hoopKei = (string)data.Rows[ rowNum ][ _CmpParameters.RST_ChutoHoopXKei_En ] ;
        hoopPitch = (double)data.Rows[ rowNum ][ _CmpParameters.RST_ChutoHoopPitch_En ] ;
        spacing_XDirectionNumber = (int)data.Rows[ rowNum ][ _CmpParameters.Top_Spacing_XDirectionNumber_En ] ;
        spacing_YDirectionNumber = (int)data.Rows[ rowNum ][ _CmpParameters.Top_Spacing_YDirectionNumber_En ] ;
      }
      else {
        syukinKei = (string)data.Rows[ rowNum ][ _CmpParameters.RST_ChukyakuSyukinKei_En ] ;
        syukinHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_ChukyakuSyukinHonsu_En ] ;
        hoopKei = (string)data.Rows[ rowNum ][ _CmpParameters.RST_ChukyakuHoopXKei_En ] ;
        hoopPitch = (double)data.Rows[ rowNum ][ _CmpParameters.RST_ChukyakuHoopPitch_En ] ;
        spacing_XDirectionNumber = (int)data.Rows[ rowNum ][ _CmpParameters.Bottom_Spacing_XDirectionNumber_En ] ;
        spacing_YDirectionNumber = (int)data.Rows[ rowNum ][ _CmpParameters.Bottom_Spacing_YDirectionNumber_En ] ;
      }

      kaburi_En /= 304.8 ;
      sintekkinIchi /= 304.8 ;
      habadomePitch /= 304.8 ;
      hoopPitch /= 304.8 ;

      Revit.DB.Transaction trans = new Revit.DB.Transaction( rvtDbDoc ) ;

      // 鉄筋
      Collections.Generic.ISet<Revit.DB.ElementId> famSymSet = rebarFam.GetFamilySymbolIds() ;
      Revit.DB.FamilySymbol famSymSyukin = null ;
      Revit.DB.FamilySymbol famSymSintekkin = null ;

      foreach ( Revit.DB.ElementId eid in famSymSet ) {
        Revit.DB.FamilySymbol fs = rvtDbDoc.GetElement( eid ) as Revit.DB.FamilySymbol ;
        string paramVal = fs.Name ;

        if ( fs.IsActive == false ) {
          trans.Start( "ファミリのアクティブ化" ) ;
          fs.Activate() ;
          trans.Commit() ;
        }

        if ( paramVal == syukinKei ) {
          famSymSyukin = fs ;
        }

        if ( paramVal == sintekkinkei ) {
          famSymSintekkin = fs ;
        }
      }

      #region 全エラー文取得

      if ( diameter <= 0 ) {
        ret += _CmpAttribute.ResourceText( "IDS_ERR_COLUMNDIAMETER" ) ;
      }

      #region ファミリ取得エラー

      if ( famSymSyukin == null ) {
        if ( ret != "" ) {
          ret += "\r\n" ;
        }

        ret += _CmpAttribute.ResourceText( "IDS_ERR_SYUKIN_FAMILY_NOTSET" ) ;
      }

      if ( famSymSintekkin == null ) {
        if ( coreRebarNum >= 4 ) {
          if ( ret != "" ) {
            ret += "\r\n" ;
          }

          ret += _CmpAttribute.ResourceText( "IDS_ERR_COREREBAR_FAMILY_NOTSET" ) ;
        }
      }

      #endregion

      #region 主筋

      if ( syukinHonsu <= 0 || syukinHonsu % 4 != 0 ) {
        if ( ret != "" ) {
          ret += "\r\n" ;
        }

        ret += _CmpAttribute.ResourceText( "IDS_ERR_CIRCLEREBARNUM" ) ;
      }

      #endregion

      #region 芯鉄筋

      if ( coreRebarNum > 0 ) {
        if ( coreRebarNum % 4 != 0 ) {
          if ( ret != "" ) {
            ret += "\r\n" ;
          }

          ret += _CmpAttribute.ResourceText( "IDS_ERR_COREREBARNUM_ROUND" ) ;
        }
      }

      #endregion

      #endregion

      if ( diameter <= 0 ) {
        return ret ;
      }

      if ( famSymSyukin == null ) {
        return ret ;
      }

      Revit.DB.View actView = rvtDbDoc.ActiveView ;

      // 鉄筋記号幅
      double diaSyukin = 0 ;
      double diaSintekkin = 0 ;

      trans.Start( "鉄筋記号幅" ) ;
      Revit.DB.FamilyInstance famInsDammyRebar = rvtDbDoc.Create.NewFamilyInstance( new Revit.DB.XYZ(), famSymSyukin, actView ) ;
      //famInsDammyRebar.LookupParameter("図面スケール逆数").Set(actView.Scale);
      trans.Commit() ;
      diaSyukin = famInsDammyRebar.Symbol.LookupParameter( _CmpAttribute.ResourceText( "IDS_TXT_ZUMENDSIZE" ) ).AsDouble() ;

      if ( famSymSintekkin != null ) {
        trans.Start( "鉄筋記号幅" ) ;
        famInsDammyRebar.Symbol = famSymSintekkin ;
        //famInsDammyRebar.LookupParameter("図面スケール逆数").Set(actView.Scale);
        trans.Commit() ;
        diaSintekkin = famInsDammyRebar.Symbol.LookupParameter( _CmpAttribute.ResourceText( "IDS_TXT_ZUMENDSIZE" ) ).AsDouble() ;
      }

      trans.Start( "ダミー削除" ) ;
      rvtDbDoc.Delete( famInsDammyRebar.Id ) ;
      trans.Commit() ;

      // 主筋
      Collections.Generic.IList<Revit.DB.XYZ> syukinPoints = new Collections.Generic.List<Revit.DB.XYZ>() ;
      // 芯鉄筋
      Collections.Generic.IList<Revit.DB.XYZ> sintekkinPoints = new Collections.Generic.List<Revit.DB.XYZ>() ;

      // 帯筋
      Revit.DB.CurveArray hoopCrvAry = new Revit.DB.CurveArray() ;
      Revit.DB.Curve hoopCrv = null ;

      // 帯筋外周
      hoopCrv = _CmpGeometry.CrvCircle( diameter - kaburi_En * 2, center ) ;
      _CmpElements.NotNullCurveSet( ref hoopCrvAry, hoopCrv ) ;

      // 幅止筋
      Revit.DB.CurveArray spacingCrvAry = new Revit.DB.CurveArray() ;
      Revit.DB.Curve spacingCrv = null ;

      #region 主筋エラー判定

      if ( syukinHonsu <= 0 || syukinHonsu % 4 != 0 ) {
        goto draw ;
      }

      #endregion

      // 主筋中心直径
      double diameter_Rebar = diameter - kaburi_En * 2 - diaSyukin ;
      // 主筋配置位置
      Collections.Generic.IList<Revit.DB.XYZ> pointOnCircle = _CmpGeometry.PointOnCircle( diameter_Rebar, center, syukinHonsu ) ;

      foreach ( Revit.DB.XYZ pnt in pointOnCircle ) {
        syukinPoints.Add( pnt ) ;
      }

      if ( coreRebarNum > 0 ) {
        if ( coreRebarNum % 4 != 0 ) {
          goto draw ;
        }
        else {
          // 芯鉄筋配置
          if ( sintekkinIchi == 0 ) {
            Collections.Generic.IList<Revit.DB.XYZ> points = SintekkinPointOnCircle( center, coreRebarNum, diaSintekkin ) ;

            foreach ( Revit.DB.XYZ pnt in points ) {
              sintekkinPoints.Add( pnt ) ;
            }
          }
          else if ( sintekkinIchi > 0 ) {
            double diameter_Sintekkin = diameter - sintekkinIchi * 2 ;

            Collections.Generic.IList<Revit.DB.XYZ> points = _CmpGeometry.PointOnCircle( diameter_Sintekkin, center, coreRebarNum ) ;

            foreach ( Revit.DB.XYZ pnt in points ) {
              sintekkinPoints.Add( pnt ) ;
            }
          }
        }
      }

      // 幅止筋
      if ( spacing_XDirectionNumber > 0 ) {
        Revit.DB.XYZ left = center + new Revit.DB.XYZ( -diameter_Rebar / 2, -diaSyukin / 2, 0 ) ;
        Revit.DB.XYZ right = center + new Revit.DB.XYZ( diameter_Rebar / 2, -diaSyukin / 2, 0 ) ;

        spacingCrv = _CmpElements.CreateBoundLine( left, right ) ;

        double radius = ( diameter - kaburi_En * 2 ) / 2 ;

        Collections.Generic.IList<Revit.DB.XYZ> crsPnts = _CmpGeometry.CrossPoint( spacingCrv as Revit.DB.Line, center, radius ) ;
        if ( crsPnts.Count == 2 ) {
          spacingCrv = Revit.DB.Line.CreateBound( crsPnts[ 0 ], crsPnts[ 1 ] ) ;
        }

        _CmpElements.NotNullCurveSet( ref spacingCrvAry, spacingCrv ) ;
      }

      if ( spacing_YDirectionNumber > 0 ) {
        Revit.DB.XYZ top = center + new Revit.DB.XYZ( -diaSyukin / 2, diameter_Rebar / 2, 0 ) ;
        Revit.DB.XYZ bottom = center + new Revit.DB.XYZ( -diaSyukin / 2, -diameter_Rebar / 2, 0 ) ;

        spacingCrv = _CmpElements.CreateBoundLine( top, bottom ) ;

        double radius = ( diameter - kaburi_En * 2 ) / 2 ;

        Collections.Generic.IList<Revit.DB.XYZ> crsPnts = _CmpGeometry.CrossPoint( spacingCrv as Revit.DB.Line, center, radius ) ;
        if ( crsPnts.Count == 2 ) {
          spacingCrv = Revit.DB.Line.CreateBound( crsPnts[ 0 ], crsPnts[ 1 ] ) ;
        }

        _CmpElements.NotNullCurveSet( ref spacingCrvAry, spacingCrv ) ;
      }

      draw:

      // 線種
      Revit.DB.GraphicsStyle bodyLineType = _CmpElements.BodyLineStyle ;
      Revit.DB.GraphicsStyle spaceLineType = _CmpElements.SpacerLineStyle ;

      trans.Start( "円柱鉄筋" ) ;

      // 帯筋
      foreach ( Revit.DB.Curve crv in hoopCrvAry ) {
        Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve( actView, crv ) ;
        dc.LineStyle = bodyLineType ;
      }

      // 幅止筋
      foreach ( Revit.DB.Curve crv in spacingCrvAry ) {
        Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve( actView, crv ) ;
        dc.LineStyle = spaceLineType ;
      }

      // 芯鉄筋
      if ( famSymSintekkin != null ) {
        foreach ( Revit.DB.XYZ pnt in sintekkinPoints ) {
          Revit.DB.FamilyInstance famInsRebar = rvtDbDoc.Create.NewFamilyInstance( pnt, famSymSintekkin, actView ) ;
        }
      }

      // 主筋
      if ( famSymSyukin != null ) {
        foreach ( Revit.DB.XYZ pnt in syukinPoints ) {
          Revit.DB.FamilyInstance famInsRebar = rvtDbDoc.Create.NewFamilyInstance( pnt, famSymSyukin, actView ) ;
        }
      }

      trans.Commit() ;

      return ret ;
    }

    /// ================================================================================
    /// <summary>梁配筋</summary>
    ///
    /// <param name="secNum">0 = 始端(元端)、1 = 中央、2 = 終端(先端)</param>
    ///
    /// <history><p>2013/05/02 Created  GSA,Inc. Ryo Kuroda</p>
    ///           <p>2015/07/22 Modified GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public string CreateRebar_Beam( System.Data.DataTable data, int rowNum, Revit.DB.XYZ center, int secNum, Revit.UI.UIApplication rvtUiApp, ref int isSyukinUeSetNum, ref int isSyukinSitaNum, ref bool isStirrupSet, ref bool isWebSet, bool isCanti )
    {
      Revit.DB.Document rvtDbDoc = rvtUiApp.ActiveUIDocument.Document ;
      Revit.UI.UIDocument rvtUiDoc = rvtUiApp.ActiveUIDocument ;
      Revit.ApplicationServices.Application rvtSrvcApp = rvtUiApp.Application ;

      string ret = "" ;

      // かぶり厚
      double kaburi = 0 ;
      double.TryParse( _CmpParameters.BeamProtectThick, out kaburi ) ;

      // 鉄筋ファミリ
      Revit.DB.Family rebarFam = null ;
      bool isHaveFam = _CmpElements.GetRebarFamily( ref rebarFam ) ;

      if ( isHaveFam == false ) {
        ret = _CmpAttribute.ResourceText( "IDS_ERR_NOREBARFAMILY" ) ;
        return ret ;
      }

      // 幅
      double x = 0 ;
      // 成
      double y = 0 ;

      // 主筋上太径
      string syukinUeHutokei = "" ;
      // 主筋上1段筋太径本数
      int syukinUe1danHutokeiHonsu = 0 ;
      // 主筋上2段筋太径本数
      int syukinUe2danHutokeiHonsu = 0 ;
      // 主筋上3段筋太径本数
      int syukinUe3danHutokeiHonsu = 0 ;
      // 主筋下太径
      string syukinSitaHutokei = "" ;
      // 主筋下1段筋太径本数
      int syukinSita1danHutokeiHonsu = 0 ;
      // 主筋下2段筋太径本数
      int syukinSita2danHutokeiHonsu = 0 ;
      // 主筋下3段筋太径本数
      int syukinSita3danHutokeiHonsu = 0 ;

      // 主筋上細径
      string syukinUeHosokei = "" ;
      // 主筋上1段筋細径本数
      int syukinUe1danHosokeiHonsu = 0 ;
      // 主筋上2段筋細径本数
      int syukinUe2danHosokeiHonsu = 0 ;
      // 主筋上3段筋細径本数
      int syukinUe3danHosokeiHonsu = 0 ;
      // 主筋下細径
      string syukinSitaHosokei = "" ;
      // 主筋下1段筋細径本数
      int syukinSita1danHosokeiHonsu = 0 ;
      // 主筋下2段筋細径本数
      int syukinSita2danHosokeiHonsu = 0 ;
      // 主筋下3段筋細径本数
      int syukinSita3danHosokeiHonsu = 0 ;

      // StirrupDiameter
      string stirrupDiameter = "" ;
      // StirrupNumber
      int stirrupNumber = 0 ;
      // StirrupPitch
      double stirrupPitch = 0 ;

      // WebDiamter
      string webDiameter = "" ;
      // WebNumber
      int webNumber = 0 ;

      // SpacingDiameter
      string spacingDiameter = "" ;
      // SpacingNumber
      int spacingNumber = 0 ;
      // SpacingPitch
      //double spacingPitch = 0;

      // 一般梁と片持ち梁の
      // 始端、中央、終端のいずれか

      #region

      if ( isCanti == false ) {
        if ( secNum == 0 ) {
          x = (double)data.Rows[ rowNum ][ _CmpParameters.s_B ] ;
          y = (double)data.Rows[ rowNum ][ _CmpParameters.s_D ] ;

          syukinUeHutokei = (string)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinItanUeHutokei ] ;
          syukinUe1danHutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinItanUe1dankinHutokeiHonsu ] ;
          syukinUe2danHutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinItanUe2dankinHutokeiHonsu ] ;
          syukinUe3danHutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinItanUe3dankinHutokeiHonsu ] ;

          syukinSitaHutokei = (string)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinItanSitaHutokei ] ;
          syukinSita1danHutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinItanSita1dankinHutokeiHonsu ] ;
          syukinSita2danHutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinItanSita2dankinHutokeiHonsu ] ;
          syukinSita3danHutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinItanSita3dankinHutokeiHonsu ] ;

          syukinUeHosokei = (string)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinItanUeHosokei ] ;
          syukinUe1danHosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinItanUe1dankinHosokeiHonsu ] ;
          syukinUe2danHosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinItanUe2dankinHosokeiHonsu ] ;
          syukinUe3danHosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinItanUe3dankinHosokeiHonsu ] ;

          syukinSitaHosokei = (string)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinItanSitaHosokei ] ;
          syukinSita1danHosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinItanSita1dankinHosokeiHonsu ] ;
          syukinSita2danHosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinItanSita2dankinHosokeiHonsu ] ;
          syukinSita3danHosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinItanSita3dankinHosokeiHonsu ] ;

          stirrupDiameter = (string)data.Rows[ rowNum ][ _CmpParameters.s_Stirrup_Diameter ] ;
          stirrupNumber = (int)data.Rows[ rowNum ][ _CmpParameters.s_Stirrup_Number ] ;
          stirrupPitch = (double)data.Rows[ rowNum ][ _CmpParameters.s_Stirrup_Pitch ] ;

          webDiameter = (string)data.Rows[ rowNum ][ _CmpParameters.s_Web_Diameter ] ;
          webNumber = (int)data.Rows[ rowNum ][ _CmpParameters.s_Web_Number ] ;

          spacingDiameter = (string)data.Rows[ rowNum ][ _CmpParameters.s_Spacing_Diameter ] ;
          spacingNumber = (int)data.Rows[ rowNum ][ _CmpParameters.s_Spacing_Number ] ;
          //spacingPitch = (double)data.Rows[rowNum][_CmpParameters.s_Spacing_Pitch];
        }
        else if ( secNum == 1 ) {
          x = (double)data.Rows[ rowNum ][ _CmpParameters.c_B ] ;
          y = (double)data.Rows[ rowNum ][ _CmpParameters.c_D ] ;

          syukinUeHutokei = (string)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinChuohUeHutokei ] ;
          syukinUe1danHutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinChuohUe1dankinHutokeiHonsu ] ;
          syukinUe2danHutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinChuohUe2dankinHutokeiHonsu ] ;
          syukinUe3danHutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinChuohUe3dankinHutokeiHonsu ] ;

          syukinSitaHutokei = (string)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinChuohSitaHutokei ] ;
          syukinSita1danHutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinChuohSita1dankinHutokeiHonsu ] ;
          syukinSita2danHutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinChuohSita2dankinHutokeiHonsu ] ;
          syukinSita3danHutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinChuohSita3dankinHutokeiHonsu ] ;

          syukinUeHosokei = (string)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinChuohUeHosokei ] ;
          syukinUe1danHosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinChuohUe1dankinHosokeiHonsu ] ;
          syukinUe2danHosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinChuohUe2dankinHosokeiHonsu ] ;
          syukinUe3danHosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinChuohUe3dankinHosokeiHonsu ] ;

          syukinSitaHosokei = (string)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinChuohSitaHosokei ] ;
          syukinSita1danHosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinChuohSita1dankinHosokeiHonsu ] ;
          syukinSita2danHosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinChuohSita2dankinHosokeiHonsu ] ;
          syukinSita3danHosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinChuohSita3dankinHosokeiHonsu ] ;

          stirrupDiameter = (string)data.Rows[ rowNum ][ _CmpParameters.c_Stirrup_Diameter ] ;
          stirrupNumber = (int)data.Rows[ rowNum ][ _CmpParameters.c_Stirrup_Number ] ;
          stirrupPitch = (double)data.Rows[ rowNum ][ _CmpParameters.c_Stirrup_Pitch ] ;

          webDiameter = (string)data.Rows[ rowNum ][ _CmpParameters.c_Web_Diameter ] ;
          webNumber = (int)data.Rows[ rowNum ][ _CmpParameters.c_Web_Number ] ;

          spacingDiameter = (string)data.Rows[ rowNum ][ _CmpParameters.c_Spacing_Diameter ] ;
          spacingNumber = (int)data.Rows[ rowNum ][ _CmpParameters.c_Spacing_Number ] ;
          //spacingPitch = (double)data.Rows[rowNum][_CmpParameters.c_Spacing_Pitch];
        }
        else if ( secNum == 2 ) {
          x = (double)data.Rows[ rowNum ][ _CmpParameters.e_B ] ;
          y = (double)data.Rows[ rowNum ][ _CmpParameters.e_D ] ;

          syukinUeHutokei = (string)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinJtanUeHutokei ] ;
          syukinUe1danHutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinJtanUe1dankinHutokeiHonsu ] ;
          syukinUe2danHutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinJtanUe2dankinHutokeiHonsu ] ;
          syukinUe3danHutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinJtanUe3dankinHutokeiHonsu ] ;

          syukinSitaHutokei = (string)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinJtanSitaHutokei ] ;
          syukinSita1danHutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinJtanSita1dankinHutokeiHonsu ] ;
          syukinSita2danHutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinJtanSita2dankinHutokeiHonsu ] ;
          syukinSita3danHutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinJtanSita3dankinHutokeiHonsu ] ;

          syukinUeHosokei = (string)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinJtanUeHosokei ] ;
          syukinUe1danHosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinJtanUe1dankinHosokeiHonsu ] ;
          syukinUe2danHosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinJtanUe2dankinHosokeiHonsu ] ;
          syukinUe3danHosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinJtanUe3dankinHosokeiHonsu ] ;

          syukinSitaHosokei = (string)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinJtanSitaHosokei ] ;
          syukinSita1danHosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinJtanSita1dankinHosokeiHonsu ] ;
          syukinSita2danHosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinJtanSita2dankinHosokeiHonsu ] ;
          syukinSita3danHosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.RST_SyukinJtanSita3dankinHosokeiHonsu ] ;

          stirrupDiameter = (string)data.Rows[ rowNum ][ _CmpParameters.e_Stirrup_Diameter ] ;
          stirrupNumber = (int)data.Rows[ rowNum ][ _CmpParameters.e_Stirrup_Number ] ;
          stirrupPitch = (double)data.Rows[ rowNum ][ _CmpParameters.e_Stirrup_Pitch ] ;

          webDiameter = (string)data.Rows[ rowNum ][ _CmpParameters.e_Web_Diameter ] ;
          webNumber = (int)data.Rows[ rowNum ][ _CmpParameters.e_Web_Number ] ;

          spacingDiameter = (string)data.Rows[ rowNum ][ _CmpParameters.e_Spacing_Diameter ] ;
          spacingNumber = (int)data.Rows[ rowNum ][ _CmpParameters.e_Spacing_Number ] ;
          //spacingPitch = (double)data.Rows[rowNum][_CmpParameters.e_Spacing_Pitch];
        }
      }
      else {
        if ( secNum == 0 ) {
          x = (double)data.Rows[ rowNum ][ _CmpParameters.MototanHarihaba ] ;
          y = (double)data.Rows[ rowNum ][ _CmpParameters.MototanHarisei ] ;

          syukinUeHutokei = (string)data.Rows[ rowNum ][ _CmpParameters.MototanUeSyukinHutokei ] ;
          syukinUe1danHutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.MototanUeSyukin1danHutokinHonsu ] ;
          syukinUe2danHutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.MototanUeSyukin2danHutokinHonsu ] ;
          syukinUe3danHutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.MototanUeSyukin3danHutokinHonsu ] ;

          syukinSitaHutokei = (string)data.Rows[ rowNum ][ _CmpParameters.MototanSitaSyukinHutokei ] ;
          syukinSita1danHutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.MototanSitaSyukin1danHutokinHonsu ] ;
          syukinSita2danHutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.MototanSitaSyukin2danHutokinHonsu ] ;
          syukinSita3danHutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.MototanSitaSyukin3danHutokinHonsu ] ;

          syukinUeHosokei = (string)data.Rows[ rowNum ][ _CmpParameters.MototanUeSyukinHosokei ] ;
          syukinUe1danHosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.MototanUeSyukin1danHosokinHonsu ] ;
          syukinUe2danHosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.MototanUeSyukin2danHosokinHonsu ] ;
          syukinUe3danHosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.MototanUeSyukin3danHosokinHonsu ] ;

          syukinSitaHosokei = (string)data.Rows[ rowNum ][ _CmpParameters.MototanSitaSyukinHosokei ] ;
          syukinSita1danHosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.MototanSitaSyukin1danHosokinHonsu ] ;
          syukinSita2danHosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.MototanSitaSyukin2danHosokinHonsu ] ;
          syukinSita3danHosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.MototanSitaSyukin3danHosokinHonsu ] ;

          stirrupDiameter = (string)data.Rows[ rowNum ][ _CmpParameters.MototanAbarakinkei ] ;
          stirrupNumber = (int)data.Rows[ rowNum ][ _CmpParameters.MototanAbarakinHonsu ] ;
          stirrupPitch = (double)data.Rows[ rowNum ][ _CmpParameters.MototanAbarakinPitch ] ;

          webDiameter = (string)data.Rows[ rowNum ][ _CmpParameters.MototanHarakinkei ] ;
          webNumber = (int)data.Rows[ rowNum ][ _CmpParameters.MototanHarakinHonsu ] ;

          spacingDiameter = (string)data.Rows[ rowNum ][ _CmpParameters.MototanHabadomekinkei ] ;
          spacingNumber = (int)data.Rows[ rowNum ][ _CmpParameters.MototanHabadomekinHonsu ] ;
          //spacingPitch    = (double)data.Rows[rowNum][_CmpParameters.MototanHabadomekinPitch];
        }
        else if ( secNum == 2 ) {
          x = (double)data.Rows[ rowNum ][ _CmpParameters.SentanHarihaba ] ;
          y = (double)data.Rows[ rowNum ][ _CmpParameters.SentanHarisei ] ;

          syukinUeHutokei = (string)data.Rows[ rowNum ][ _CmpParameters.SentanUeSyukinHutokei ] ;
          syukinUe1danHutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.SentanUeSyukin1danHutokinHonsu ] ;
          syukinUe2danHutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.SentanUeSyukin2danHutokinHonsu ] ;
          syukinUe3danHutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.SentanUeSyukin3danHutokinHonsu ] ;

          syukinSitaHutokei = (string)data.Rows[ rowNum ][ _CmpParameters.SentanSitaSyukinHutokei ] ;
          syukinSita1danHutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.SentanSitaSyukin1danHutokinHonsu ] ;
          syukinSita2danHutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.SentanSitaSyukin2danHutokinHonsu ] ;
          syukinSita3danHutokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.SentanSitaSyukin3danHutokinHonsu ] ;

          syukinUeHosokei = (string)data.Rows[ rowNum ][ _CmpParameters.SentanUeSyukinHosokei ] ;
          syukinUe1danHosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.SentanUeSyukin1danHosokinHonsu ] ;
          syukinUe2danHosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.SentanUeSyukin2danHosokinHonsu ] ;
          syukinUe3danHosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.SentanUeSyukin3danHosokinHonsu ] ;

          syukinSitaHosokei = (string)data.Rows[ rowNum ][ _CmpParameters.SentanSitaSyukinHosokei ] ;
          syukinSita1danHosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.SentanSitaSyukin1danHosokinHonsu ] ;
          syukinSita2danHosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.SentanSitaSyukin2danHosokinHonsu ] ;
          syukinSita3danHosokeiHonsu = (int)data.Rows[ rowNum ][ _CmpParameters.SentanSitaSyukin3danHosokinHonsu ] ;

          stirrupDiameter = (string)data.Rows[ rowNum ][ _CmpParameters.SentanAbarakinkei ] ;
          stirrupNumber = (int)data.Rows[ rowNum ][ _CmpParameters.SentanAbarakinHonsu ] ;
          stirrupPitch = (double)data.Rows[ rowNum ][ _CmpParameters.SentanAbarakinPitch ] ;

          webDiameter = (string)data.Rows[ rowNum ][ _CmpParameters.SentanHarakinkei ] ;
          webNumber = (int)data.Rows[ rowNum ][ _CmpParameters.SentanHarakinHonsu ] ;

          spacingDiameter = (string)data.Rows[ rowNum ][ _CmpParameters.SentanHabadomekinkei ] ;
          spacingNumber = (int)data.Rows[ rowNum ][ _CmpParameters.SentanHabadomekinHonsu ] ;
          //spacingPitch    = (double)data.Rows[rowNum][_CmpParameters.SentanHabadomekinPitch];
        }
      }

      #endregion

      kaburi /= 304.8 ;
      stirrupPitch /= 304.8 ;
      //spacingPitch /= 304.8;

      Revit.DB.Transaction trans = new Revit.DB.Transaction( rvtDbDoc ) ;

      // 鉄筋
      Collections.Generic.ISet<Revit.DB.ElementId> famSymSet = rebarFam.GetFamilySymbolIds() ;
      Revit.DB.FamilySymbol famSymSyukinUeHuto = null ;
      Revit.DB.FamilySymbol famSymSyukinUeHoso = null ;
      Revit.DB.FamilySymbol famSymSyukinSitaHuto = null ;
      Revit.DB.FamilySymbol famSymSyukinSitaHoso = null ;
      Revit.DB.FamilySymbol famSymWeb = null ;

      foreach ( Revit.DB.ElementId eid in famSymSet ) {
        Revit.DB.FamilySymbol fs = rvtDbDoc.GetElement( eid ) as Revit.DB.FamilySymbol ;
        string paramVal = fs.Name ;

        if ( fs.IsActive == false ) {
          trans.Start( "ファミリのアクティブ化" ) ;
          fs.Activate() ;
          trans.Commit() ;
        }

        if ( paramVal == syukinUeHutokei ) {
          famSymSyukinUeHuto = fs ;
        }

        if ( paramVal == syukinUeHosokei ) {
          famSymSyukinUeHoso = fs ;
        }

        if ( paramVal == syukinSitaHutokei ) {
          famSymSyukinSitaHuto = fs ;
        }

        if ( paramVal == syukinSitaHosokei ) {
          famSymSyukinSitaHoso = fs ;
        }

        if ( paramVal == webDiameter ) {
          famSymWeb = fs ;
        }
      }

      #region 全エラー文取得

      bool rectangle = true ;
      bool isSyukinUeSet = true ;
      bool isSyukinSitaSet = true ;
      isSyukinUeSetNum = 0 ;
      isSyukinSitaNum = 0 ;
      isSyukinSitaSet = true ;
      isStirrupSet = true ;
      isWebSet = true ;

      if ( x <= 0d || y <= 0d ) {
        ret += _CmpAttribute.ResourceText( "IDS_ERR_GIRDERXORY" ) ;
        rectangle = false ;
      }

      #region ファミリ取得エラー判定

      if ( famSymSyukinUeHuto == null ) {
        if ( ret != "" ) {
          ret += "\r\n" ;
        }

        ret += _CmpAttribute.ResourceText( "IDS_TXT_UWABA" ) + _CmpAttribute.ResourceText( "IDS_ERR_HUTO_FAMILY_NOTSET" ) ;

        isSyukinUeSet = false ;
      }

      if ( famSymSyukinUeHoso == null ) {
        if ( syukinUe1danHosokeiHonsu > 0 || syukinUe2danHosokeiHonsu > 0 || syukinUe3danHosokeiHonsu > 0 ) {
          if ( ret != "" ) {
            ret += "\r\n" ;
          }

          ret += _CmpAttribute.ResourceText( "IDS_TXT_UWABA" ) + _CmpAttribute.ResourceText( "IDS_ERR_HOSO_FAMILY_NOTSET" ) ;

          isSyukinUeSet = false ;
        }
      }

      if ( famSymSyukinSitaHuto == null ) {
        if ( ret != "" ) {
          ret += "\r\n" ;
        }

        ret += _CmpAttribute.ResourceText( "IDS_TXT_SITABA" ) + _CmpAttribute.ResourceText( "IDS_ERR_HUTO_FAMILY_NOTSET" ) ;

        isSyukinSitaSet = false ;
      }

      if ( famSymSyukinSitaHoso == null ) {
        if ( syukinSita1danHosokeiHonsu > 0 || syukinSita2danHosokeiHonsu > 0 || syukinSita3danHosokeiHonsu > 0 ) {
          if ( ret != "" ) {
            ret += "\r\n" ;
          }

          ret += _CmpAttribute.ResourceText( "IDS_TXT_SITABA" ) + _CmpAttribute.ResourceText( "IDS_ERR_HOSO_FAMILY_NOTSET" ) ;

          isSyukinSitaSet = false ;
        }
      }

      if ( famSymWeb == null ) {
        if ( webNumber > 0 ) {
          if ( ret != "" ) {
            ret += "\r\n" ;
          }

          ret += _CmpAttribute.ResourceText( "IDS_ERR_WEB_FAMILY_NOTSET" ) ;

          isWebSet = false ;
        }
      }

      #endregion

      #region 1段筋

      // 上1段筋太径
      if ( syukinUe1danHutokeiHonsu < 2 ) {
        if ( ret != "" ) {
          ret += "\r\n" ;
        }

        ret += _CmpAttribute.ResourceText( "IDS_ERR_UE1LACKHUTOKEI" ) ;

        isSyukinUeSet = false ;
      }

      // 上1段筋細径
      if ( syukinUe1danHosokeiHonsu > 0 ) {
        if ( syukinUe1danHutokeiHonsu <= syukinUe1danHosokeiHonsu ) {
          if ( ret != "" ) {
            ret += "\r\n" ;
          }

          ret += _CmpAttribute.ResourceText( "IDS_ERR_UE1HUTO_ORUNDER_HOSO" ) ;

          isSyukinUeSet = false ;
        }
      }

      // 下1段筋太径
      if ( syukinSita1danHutokeiHonsu < 2 ) {
        if ( ret != "" ) {
          ret += "\r\n" ;
        }

        ret += _CmpAttribute.ResourceText( "IDS_ERR_SITA1LACKHUTOKEI" ) ;

        isSyukinSitaSet = false ;
      }

      // 下1段筋細径
      if ( syukinSita1danHosokeiHonsu > 0 ) {
        if ( syukinSita1danHutokeiHonsu <= syukinSita1danHosokeiHonsu ) {
          if ( ret != "" ) {
            ret += "\r\n" ;
          }

          ret += _CmpAttribute.ResourceText( "IDS_ERR_SITA1HUTO_ORUNDER_HOSO" ) ;

          isSyukinSitaSet = false ;
        }
      }

      if ( isSyukinUeSet == false ) {
        isSyukinSitaSet = false ;
      }
      else if ( isSyukinSitaSet == false ) {
        isSyukinUeSet = false ;
      }

      if ( isSyukinUeSet ) {
        isSyukinUeSetNum = 1 ;
      }

      if ( isSyukinSitaSet ) {
        isSyukinSitaNum = 1 ;
      }

      #endregion

      #region 2段筋

      // 上合計
      if ( syukinUe1danHutokeiHonsu + syukinUe1danHosokeiHonsu < syukinUe2danHutokeiHonsu + syukinUe2danHosokeiHonsu ) {
        if ( ret != "" ) {
          ret += "\r\n" ;
        }

        ret += _CmpAttribute.ResourceText( "IDS_ERR_UE2_OVER_UE1" ) ;

        isSyukinUeSet = false ;
      }
      else {
        // 2段筋太径
        // 上端
        if ( syukinUe2danHutokeiHonsu > 0 ) {
          if ( syukinUe1danHutokeiHonsu < syukinUe2danHutokeiHonsu ) {
            if ( ret != "" ) {
              ret += "\r\n" ;
            }

            ret += _CmpAttribute.ResourceText( "IDS_ERR_UE2HUTOOVER1" ) ;

            isSyukinUeSet = false ;
          }
        }

        // 2段筋細径
        // 上端
        if ( syukinUe2danHosokeiHonsu > 0 ) {
          if ( syukinUe2danHutokeiHonsu <= syukinUe2danHosokeiHonsu ) {
            if ( ret != "" ) {
              ret += "\r\n" ;
            }

            ret += _CmpAttribute.ResourceText( "IDS_ERR_UE2HUTO_ORUNDER_HOSO" ) ;

            isSyukinUeSet = false ;
          }
        }
      }

      // 下合計
      if ( syukinSita1danHutokeiHonsu + syukinSita1danHosokeiHonsu < syukinSita2danHutokeiHonsu + syukinSita2danHosokeiHonsu ) {
        if ( ret != "" ) {
          ret += "\r\n" ;
        }

        ret += _CmpAttribute.ResourceText( "IDS_ERR_SITA2_OVER_SITA1" ) ;

        isSyukinSitaSet = false ;
      }
      else {
        // 2段筋太径
        // 下端
        if ( syukinSita2danHutokeiHonsu > 0 ) {
          if ( syukinSita1danHutokeiHonsu < syukinSita2danHutokeiHonsu ) {
            if ( ret != "" ) {
              ret += "\r\n" ;
            }

            ret += _CmpAttribute.ResourceText( "IDS_ERR_SITA2HUTOOVER1" ) ;

            isSyukinSitaSet = false ;
          }
        }

        // 2段筋細径
        // 下端
        if ( syukinSita2danHosokeiHonsu > 0 ) {
          if ( syukinSita2danHutokeiHonsu <= syukinSita2danHosokeiHonsu ) {
            if ( ret != "" ) {
              ret += "\r\n" ;
            }

            ret += _CmpAttribute.ResourceText( "IDS_ERR_SITA2HUTO_ORUNDER_HOSO" ) ;

            isSyukinSitaSet = false ;
          }
        }
      }

      if ( isSyukinUeSet ) {
        isSyukinUeSetNum = 2 ;
      }

      if ( isSyukinSitaSet ) {
        isSyukinSitaNum = 2 ;
      }

      #endregion

      #region 3段筋

      // 上合計
      if ( syukinUe2danHutokeiHonsu + syukinUe2danHosokeiHonsu < syukinUe3danHutokeiHonsu + syukinUe3danHosokeiHonsu ) {
        if ( ret != "" ) {
          ret += "\r\n" ;
        }

        ret += _CmpAttribute.ResourceText( "IDS_ERR_UE3_OVER_UE2" ) ;

        isSyukinUeSet = false ;
      }
      else {
        // 3段筋太径
        // 上端
        if ( syukinUe3danHutokeiHonsu > 0 ) {
          if ( syukinUe2danHutokeiHonsu < syukinUe3danHutokeiHonsu ) {
            if ( ret != "" ) {
              ret += "\r\n" ;
            }

            ret += _CmpAttribute.ResourceText( "IDS_ERR_UE3HUTOOVER2" ) ;

            isSyukinUeSet = false ;
          }
        }

        // 3段筋細径
        // 上端
        if ( syukinUe3danHosokeiHonsu > 0 ) {
          if ( syukinUe3danHutokeiHonsu <= syukinUe3danHosokeiHonsu ) {
            if ( ret != "" ) {
              ret += "\r\n" ;
            }

            ret += _CmpAttribute.ResourceText( "IDS_ERR_UE3HUTO_ORUNDER_HOSO" ) ;

            isSyukinUeSet = false ;
          }
        }
      }

      // 下合計
      if ( syukinSita2danHutokeiHonsu + syukinSita2danHosokeiHonsu < syukinSita3danHutokeiHonsu + syukinSita3danHosokeiHonsu ) {
        if ( ret != "" ) {
          ret += "\r\n" ;
        }

        ret += _CmpAttribute.ResourceText( "IDS_ERR_SITA3_OVER_SITA2" ) ;

        isSyukinSitaSet = false ;
      }
      else {
        // 3段筋太径
        // 下端
        if ( syukinSita3danHutokeiHonsu > 0 ) {
          if ( syukinSita2danHutokeiHonsu < syukinSita3danHutokeiHonsu ) {
            if ( ret != "" ) {
              ret += "\r\n" ;
            }

            ret += _CmpAttribute.ResourceText( "IDS_ERR_SITA3HUTOOVER2" ) ;

            isSyukinSitaSet = false ;
          }
        }

        // 3段筋細径
        // 下端
        if ( syukinSita3danHosokeiHonsu > 0 ) {
          if ( syukinSita3danHutokeiHonsu <= syukinSita3danHosokeiHonsu ) {
            if ( ret != "" ) {
              ret += "\r\n" ;
            }

            ret += _CmpAttribute.ResourceText( "IDS_ERR_SITA3HUTO_ORUNDER_HOSO" ) ;

            isSyukinSitaSet = false ;
          }
        }
      }

      if ( isSyukinUeSet ) {
        isSyukinUeSetNum = 3 ;
      }

      if ( isSyukinSitaSet ) {
        isSyukinSitaNum = 3 ;
      }

      #endregion

      #region 肋筋

      if ( syukinUe1danHutokeiHonsu >= 2 && syukinSita1danHutokeiHonsu >= 2 ) {
        if ( stirrupNumber < 2 || syukinUe1danHutokeiHonsu + syukinUe1danHosokeiHonsu < stirrupNumber || syukinSita1danHutokeiHonsu + syukinSita1danHosokeiHonsu < stirrupNumber ) {
          if ( ret != "" ) {
            ret += "\r\n" ;
          }

          ret += _CmpAttribute.ResourceText( "IDS_ERR_STIRRUPNUM" ) ;

          isStirrupSet = false ;
        }
        else {
          if ( stirrupDiameter == "" ) {
            if ( ret != "" ) {
              ret += "\r\n" ;
            }

            ret += _CmpAttribute.ResourceText( "IDS_ERR_STIRRUPDIAMETERNOTHING" ) ;

            isStirrupSet = false ;
          }

          if ( stirrupPitch == 0.0 ) {
            if ( ret != "" ) {
              ret += "\r\n" ;
            }

            ret += _CmpAttribute.ResourceText( "IDS_ERR_STIRRUPPITCH" ) ;

            isStirrupSet = false ;
          }
        }
      }

      #endregion

      #region 腹筋

      if ( webNumber > 0 ) {
        if ( webNumber % 2 != 0 ) {
          if ( ret != "" ) {
            ret += "\r\n" ;
          }

          ret += _CmpAttribute.ResourceText( "IDS_ERR_WEBNUM" ) ;

          isWebSet = false ;
        }
      }

      #endregion

      #region 幅止筋

      if ( spacingNumber > 0 ) {
        if ( spacingNumber * 2 > webNumber ) {
          if ( ret != "" ) {
            ret += "\r\n" ;
          }

          ret += _CmpAttribute.ResourceText( "IDS_ERR_SPACINGNUM" ) ;
        }
        else if ( spacingDiameter == "" ) {
          if ( ret != "" ) {
            ret += "\r\n" ;
          }

          ret += _CmpAttribute.ResourceText( "IDS_ERR_SPACINGKEI" ) ;
        }
      }

      #endregion

      #endregion

      // 矩形作成判定
      if ( rectangle == false ) {
        return ret ;
      }

      #region ファミリ取得判定

      if ( famSymSyukinUeHuto == null ) {
        return ret ;
      }

      if ( famSymSyukinUeHoso == null ) {
        if ( syukinUe1danHosokeiHonsu > 0 || syukinUe2danHosokeiHonsu > 0 || syukinUe3danHosokeiHonsu > 0 ) {
          return ret ;
        }
      }

      if ( famSymSyukinSitaHuto == null ) {
        return ret ;
      }

      if ( famSymSyukinSitaHoso == null ) {
        if ( syukinSita1danHosokeiHonsu > 0 || syukinSita2danHosokeiHonsu > 0 || syukinSita3danHosokeiHonsu > 0 ) {
          return ret ;
        }
      }

      #endregion

      bool errorUe = false ;
      bool errorSita = false ;

      #region 1段筋エラー判定

      // 1段筋太径
      if ( syukinUe1danHutokeiHonsu < 2 || syukinSita1danHutokeiHonsu < 2 ) {
        return ret ;
      }

      // 1段筋細径
      // 上端
      if ( syukinUe1danHosokeiHonsu > 0 ) {
        if ( syukinUe1danHutokeiHonsu <= syukinUe1danHosokeiHonsu ) {
          return ret ;
        }
      }

      // 下端
      if ( syukinSita1danHosokeiHonsu > 0 ) {
        if ( syukinSita1danHutokeiHonsu <= syukinSita1danHosokeiHonsu ) {
          return ret ;
        }
      }

      #endregion

      Revit.DB.View actView = rvtDbDoc.ActiveView ;

      // 鉄筋記号幅
      double diaSyukinUeHuto = 0 ;
      double diaSyukinUeHoso = 0 ;
      double diaSyukinSitaHuto = 0 ;
      double diaSyukinSitaHoso = 0 ;
      double diaWeb = 0 ;

      trans.Start( "鉄筋記号幅" ) ;
      Revit.DB.FamilyInstance famInsDammyRebar = rvtDbDoc.Create.NewFamilyInstance( new Revit.DB.XYZ(), famSymSyukinUeHuto, actView ) ;
      trans.Commit() ;

      if ( famSymSyukinUeHuto != null ) {
        trans.Start( "鉄筋記号幅" ) ;
        famInsDammyRebar.Symbol = famSymSyukinUeHuto ;
        trans.Commit() ;
        diaSyukinUeHuto = famInsDammyRebar.Symbol.LookupParameter( _CmpAttribute.ResourceText( "IDS_TXT_ZUMENDSIZE" ) ).AsDouble() ;
      }

      if ( famSymSyukinUeHoso != null ) {
        trans.Start( "鉄筋記号幅" ) ;
        famInsDammyRebar.Symbol = famSymSyukinUeHoso ;
        trans.Commit() ;
        diaSyukinUeHoso = famInsDammyRebar.Symbol.LookupParameter( _CmpAttribute.ResourceText( "IDS_TXT_ZUMENDSIZE" ) ).AsDouble() ;
      }

      if ( famSymSyukinSitaHuto != null ) {
        trans.Start( "鉄筋記号幅" ) ;
        famInsDammyRebar.Symbol = famSymSyukinSitaHuto ;
        trans.Commit() ;
        diaSyukinSitaHuto = famInsDammyRebar.Symbol.LookupParameter( _CmpAttribute.ResourceText( "IDS_TXT_ZUMENDSIZE" ) ).AsDouble() ;
      }

      if ( famSymSyukinSitaHoso != null ) {
        trans.Start( "鉄筋記号幅" ) ;
        famInsDammyRebar.Symbol = famSymSyukinSitaHoso ;
        trans.Commit() ;
        diaSyukinSitaHoso = famInsDammyRebar.Symbol.LookupParameter( _CmpAttribute.ResourceText( "IDS_TXT_ZUMENDSIZE" ) ).AsDouble() ;
      }

      if ( famSymWeb != null ) {
        trans.Start( "鉄筋記号幅" ) ;
        famInsDammyRebar.Symbol = famSymWeb ;
        trans.Commit() ;
        diaWeb = famInsDammyRebar.Symbol.LookupParameter( _CmpAttribute.ResourceText( "IDS_TXT_ZUMENDSIZE" ) ).AsDouble() ;
      }

      trans.Start( "ダミー削除" ) ;
      rvtDbDoc.Delete( famInsDammyRebar.Id ) ;
      trans.Commit() ;

      // ----- 配筋開始 -----

      // かぶり厚分内側の頂点
      Revit.DB.XYZ leftTop = null ;
      Revit.DB.XYZ leftBottom = null ;
      Revit.DB.XYZ rightTop = null ;
      Revit.DB.XYZ rightBottom = null ;
      _CmpGeometry.RectanglePointsInsideKaburi( center, x, y, kaburi, ref leftTop, ref leftBottom, ref rightTop, ref rightBottom ) ;

      // 主筋上太径
      Collections.Generic.IList<Revit.DB.XYZ> syukinUeHutokeiPoints = new Collections.Generic.List<Revit.DB.XYZ>() ;
      // 主筋上細径
      Collections.Generic.IList<Revit.DB.XYZ> syukinUeHosokeiPoints = new Collections.Generic.List<Revit.DB.XYZ>() ;
      // 主筋下太径
      Collections.Generic.IList<Revit.DB.XYZ> syukinSitaHutokeiPoints = new Collections.Generic.List<Revit.DB.XYZ>() ;
      // 主筋下細径
      Collections.Generic.IList<Revit.DB.XYZ> syukinSitaHosokeiPoints = new Collections.Generic.List<Revit.DB.XYZ>() ;

      // 腹筋
      Collections.Generic.IList<Revit.DB.XYZ> webPoints = new Collections.Generic.List<Revit.DB.XYZ>() ;
      // 幅止筋
      Revit.DB.CurveArray spaceCrvAry = new Revit.DB.CurveArray() ;

      // 主筋上1段筋座標
      Collections.Generic.IList<Revit.DB.XYZ> syukinUe1danPoints = new Collections.Generic.List<Revit.DB.XYZ>() ;
      // 主筋下1段筋座標
      Collections.Generic.IList<Revit.DB.XYZ> syukinSita1danPoints = new Collections.Generic.List<Revit.DB.XYZ>() ;

      // 主筋上1段筋太径座標
      Collections.Generic.IList<Revit.DB.XYZ> syukinUe1danHutoPoints = new Collections.Generic.List<Revit.DB.XYZ>() ;
      // 主筋下1段筋太径座標
      Collections.Generic.IList<Revit.DB.XYZ> syukinSita1danHutoPoints = new Collections.Generic.List<Revit.DB.XYZ>() ;
      // 主筋上1段筋細径座標
      Collections.Generic.IList<Revit.DB.XYZ> syukinUe1danHosoPoints = new Collections.Generic.List<Revit.DB.XYZ>() ;
      // 主筋下1段筋細径座標
      Collections.Generic.IList<Revit.DB.XYZ> syukinSita1danHosoPoints = new Collections.Generic.List<Revit.DB.XYZ>() ;

      // 肋筋基準X座標
      Collections.Generic.IList<Revit.DB.XYZ> stirrupXPoints = new Collections.Generic.List<Revit.DB.XYZ>() ;

      // 上端筋最下基準座標
      Revit.DB.XYZ ueSaikakin = null ;
      // 下端筋最上基準座標
      Revit.DB.XYZ sitaSaijokin = null ;

      // 1段筋の配置
      // 1段筋四隅
      Revit.DB.XYZ leftTopRebar = leftTop + new Revit.DB.XYZ( diaSyukinUeHuto / 2, -( diaSyukinUeHuto / 2 ), 0 ) ;
      Revit.DB.XYZ rightTopRebar = rightTop + new Revit.DB.XYZ( -( diaSyukinUeHuto / 2 ), -( diaSyukinUeHuto / 2 ), 0 ) ;
      Revit.DB.XYZ leftBottomRebar = leftBottom + new Revit.DB.XYZ( diaSyukinSitaHuto / 2, diaSyukinSitaHuto / 2, 0 ) ;
      Revit.DB.XYZ rightBottomRebar = rightBottom + new Revit.DB.XYZ( -( diaSyukinSitaHuto / 2 ), diaSyukinSitaHuto / 2, 0 ) ;

      syukinUeHutokeiPoints.Add( leftTopRebar ) ;
      syukinSitaHutokeiPoints.Add( leftBottomRebar ) ;
      syukinUeHutokeiPoints.Add( rightTopRebar ) ;
      syukinSitaHutokeiPoints.Add( rightBottomRebar ) ;

      ueSaikakin = leftTopRebar ;
      sitaSaijokin = leftBottomRebar ;

      _InnerTop = ueSaikakin ;
      _InnerBottom = sitaSaijokin ;
      _RebarTop = leftTopRebar ;
      _RebarBtm = leftBottomRebar ;

      // 肋筋
      Revit.DB.CurveArray stirrupCrvAry = new Revit.DB.CurveArray() ;
      Revit.DB.Line stirrupLine = null ;

      // 肋筋外周部エラー
      if ( stirrupNumber >= 2 ) {
        // 肋筋外周部
        stirrupLine = _CmpElements.CreateBoundLine( leftTop, leftBottom ) ;
        _CmpElements.NotNullCurveSet( ref stirrupCrvAry, stirrupLine ) ;
        stirrupLine = _CmpElements.CreateBoundLine( leftBottom, rightBottom ) ;
        _CmpElements.NotNullCurveSet( ref stirrupCrvAry, stirrupLine ) ;
        stirrupLine = _CmpElements.CreateBoundLine( rightBottom, rightTop ) ;
        _CmpElements.NotNullCurveSet( ref stirrupCrvAry, stirrupLine ) ;
        stirrupLine = _CmpElements.CreateBoundLine( rightTop, leftTop ) ;
        _CmpElements.NotNullCurveSet( ref stirrupCrvAry, stirrupLine ) ;
      }

      // 1段筋配置太径細径順序記録(四隅を含む)
      // 0 = 太径、1 = 細径
      Collections.Generic.IList<int> ue1danRebarOrder = RebarOrder( syukinUe1danHutokeiHonsu, syukinUe1danHosokeiHonsu ) ;
      Collections.Generic.IList<int> sita1danRebarOrder = RebarOrder( syukinSita1danHutokeiHonsu, syukinSita1danHosokeiHonsu ) ;

      if ( stirrupNumber > 2 && stirrupNumber % 2 != 0 ) {
        IListReverse( ref ue1danRebarOrder ) ;
        IListReverse( ref sita1danRebarOrder ) ;
      }

      // 上1段筋太径の位置
      Collections.Generic.IList<Revit.DB.XYZ> syukinUe1HutoPoints = new Collections.Generic.List<Revit.DB.XYZ>() ;
      // 下1段筋太径の位置
      Collections.Generic.IList<Revit.DB.XYZ> syukinSita1HutoPoints = new Collections.Generic.List<Revit.DB.XYZ>() ;

      // 太径の1.5倍
      double diaSyukinUeHuto1_5 = diaSyukinUeHuto * 1.5 ;

      // 1段筋太径本数 > 1段筋細径本数
      if ( syukinUe1danHutokeiHonsu > syukinUe1danHosokeiHonsu && syukinSita1danHutokeiHonsu > syukinSita1danHosokeiHonsu ) {
        // 上1段筋本数
        int ue1danRebarCount = syukinUe1danHutokeiHonsu + syukinUe1danHosokeiHonsu ;
        // 上1段筋記号中心間距離
        double ue1danRebarDistance = ( rightTopRebar.X - leftTopRebar.X ) / ( ue1danRebarCount - 1 ) ;
        // 上1段筋中間距離
        double ue1danCenterDistance = ( rightTopRebar.X - leftTopRebar.X ) / 2 ;

        // 上1段筋合計位置
        double ue1SumDistance = 0 ;

        // 配筋基準位置
        for ( int i = 0 ; i < ue1danRebarOrder.Count ; ++i ) {
          Revit.DB.XYZ point = null ;

          // 四隅
          if ( i == 0 || i == ue1danRebarOrder.Count - 1 ) {
            continue ;
          }
          else {
            // 等間隔に配置
            ue1SumDistance += ue1danRebarDistance ;

            point = leftTopRebar + new Revit.DB.XYZ( ue1SumDistance, 0, 0 ) ;
            syukinUe1danPoints.Add( point ) ;

            if ( ue1danRebarOrder[ i ] == 0 ) {
              syukinUe1danHutoPoints.Add( point ) ;
            }
            else {
              syukinUe1danHosoPoints.Add( point ) ;
            }
          }
        }

        // 下1段筋本数
        int sita1danRebarCount = syukinSita1danHutokeiHonsu + syukinSita1danHosokeiHonsu ;
        // 下1段筋記号中心間距離
        double sita1danRebarDistance = ( rightBottomRebar.X - leftBottomRebar.X ) / ( sita1danRebarCount - 1 ) ;
        // 下1段筋中間距離
        double sita1danCenterDistance = ( rightBottomRebar.X - leftBottomRebar.X ) / 2 ;

        // 太径の1.5倍
        double diaSyukinSitaHuto1_5 = diaSyukinSitaHuto * 1.5 ;

        // 下1段筋合計位置
        double sita1SumDistance = 0 ;

        // 配筋基準位置
        for ( int i = 0 ; i < sita1danRebarOrder.Count ; ++i ) {
          Revit.DB.XYZ point = null ;

          // 四隅
          if ( i == 0 || i == sita1danRebarOrder.Count - 1 ) {
            continue ;
          }
          else {
            // 等間隔に配置
            sita1SumDistance += sita1danRebarDistance ;

            point = leftBottomRebar + new Revit.DB.XYZ( sita1SumDistance, 0, 0 ) ;
            syukinSita1danPoints.Add( point ) ;

            if ( sita1danRebarOrder[ i ] == 0 ) {
              syukinSita1danHutoPoints.Add( point ) ;
            }
            else {
              syukinSita1danHosoPoints.Add( point ) ;
            }
          }
        }

        // 肋筋間隔
        double stirrupDistance = ( rightBottomRebar.X - leftBottomRebar.X ) / ( stirrupNumber - 1 ) ;

        // 肋筋合計位置
        double stirrupSumDistance = 0 ;

        Revit.DB.XYZ wideCenter = leftBottomRebar + new Revit.DB.XYZ( sita1danCenterDistance, 0, 0 ) ;

        // 肋筋がある場合、上下筋の合計本数が多い方の位置に合わせる
        // 太径細径関わらず
        if ( stirrupNumber >= 3 && syukinUe1danPoints.Count >= stirrupNumber - 2 && syukinSita1danPoints.Count >= stirrupNumber - 2 ) {
          for ( int i = 0 ; i < stirrupNumber ; ++i ) {
            Revit.DB.XYZ point = null ;

            if ( i == 0 || i == stirrupNumber - 1 ) {
              continue ;
            }
            else {
              // 等間隔に配置
              stirrupSumDistance += stirrupDistance ;

              point = leftBottomRebar + new Revit.DB.XYZ( stirrupSumDistance, 0, 0 ) ;
              stirrupXPoints.Add( point ) ;
            }
          }

          // 上端が多い
          if ( syukinUe1danPoints.Count > syukinSita1danPoints.Count ) {
            // 下を合わせる
            //MoveMajorityRebar(syukinUe1danPoints, ref syukinSita1danPoints, ref stirrupXPoints, diaSyukinUeHuto, wideCenter);

            //MoveToMajorityRebar(syukinUe1danHutoPoints, syukinUe1danHosoPoints, syukinUe1danPoints, ref syukinSita1danPoints, ref stirrupXPoints, diaSyukinUeHuto, wideCenter, leftTopRebar, rightTopRebar);
            MoveToMajorityRebar_Test2( syukinUe1danHutoPoints, syukinUe1danHosoPoints, syukinUe1danPoints, ref syukinSita1danPoints, ref stirrupXPoints, diaSyukinUeHuto, wideCenter, leftTopRebar, rightTopRebar ) ;
          }
          // 下端が多い
          else if ( syukinUe1danPoints.Count < syukinSita1danPoints.Count ) {
            // 上を合わせる
            //MoveMajorityRebar(syukinSita1danPoints, ref syukinUe1danPoints, ref stirrupXPoints, diaSyukinSitaHuto, wideCenter);

            //MoveToMajorityRebar(syukinSita1danHutoPoints, syukinSita1danHosoPoints, syukinSita1danPoints, ref syukinUe1danPoints, ref stirrupXPoints, diaSyukinSitaHuto, wideCenter, leftTopRebar, rightTopRebar);
            MoveToMajorityRebar_Test2( syukinSita1danHutoPoints, syukinSita1danHosoPoints, syukinSita1danPoints, ref syukinUe1danPoints, ref stirrupXPoints, diaSyukinSitaHuto, wideCenter, leftTopRebar, rightTopRebar ) ;
          }
          // 同じ
          else {
            // 上を合わせる
            //MoveMajorityRebar(syukinSita1danPoints, ref syukinUe1danPoints, ref stirrupXPoints, diaSyukinSitaHuto, wideCenter);

            //MoveToMajorityRebar(syukinSita1danHutoPoints, syukinSita1danHosoPoints, syukinSita1danPoints, ref syukinUe1danPoints, ref stirrupXPoints, diaSyukinSitaHuto, wideCenter, leftTopRebar, rightTopRebar);
            MoveToMajorityRebar_Test2( syukinSita1danHutoPoints, syukinSita1danHosoPoints, syukinSita1danPoints, ref syukinUe1danPoints, ref stirrupXPoints, diaSyukinSitaHuto, wideCenter, leftTopRebar, rightTopRebar ) ;
          }
        }

        // 1段筋
        // 配筋
        for ( int i = 0 ; i < syukinUe1danPoints.Count ; ++i ) {
          int now = ue1danRebarOrder[ i + 1 ] ;
          Revit.DB.XYZ point = syukinUe1danPoints[ i ] ;

          if ( now == 0 ) {
            syukinUeHutokeiPoints.Add( point ) ;
            syukinUe1HutoPoints.Add( point ) ;
          }
          else if ( now == 1 ) {
            syukinUeHosokeiPoints.Add( point ) ;
          }
        }

        for ( int i = 0 ; i < syukinSita1danPoints.Count ; ++i ) {
          int now = sita1danRebarOrder[ i + 1 ] ;
          Revit.DB.XYZ point = syukinSita1danPoints[ i ] ;

          if ( now == 0 ) {
            syukinSitaHutokeiPoints.Add( point ) ;
            syukinSita1HutoPoints.Add( point ) ;
          }
          else if ( now == 1 ) {
            syukinSitaHosokeiPoints.Add( point ) ;
          }
        }
      } // 2013/11/28

      #region 上端2段筋エラー判定

      // 2段筋太径
      // 上端
      if ( syukinUe2danHutokeiHonsu > 0 ) {
        if ( syukinUe1danHutokeiHonsu < syukinUe2danHutokeiHonsu ) {
          //goto draw;
          errorUe = true ;
        }
      }

      // 2段筋細径
      // 上端
      if ( syukinUe2danHosokeiHonsu > 0 ) {
        if ( syukinUe2danHutokeiHonsu <= syukinUe2danHosokeiHonsu ) {
          //goto draw;
          errorUe = true ;
        }
      }

      // 合計
      if ( syukinUe1danHutokeiHonsu + syukinUe1danHosokeiHonsu < syukinUe2danHutokeiHonsu + syukinUe2danHosokeiHonsu ) {
        //goto draw;
        errorUe = true ;
      }

      #endregion

      // 上端2段筋太径本数 > 2段筋細径本数

      #region

      if ( syukinUe2danHutokeiHonsu > syukinUe2danHosokeiHonsu && errorUe == false ) {
        // 四隅を含めた点
        Collections.Generic.IList<Revit.DB.XYZ> syukinUe1danAllPoints = new Collections.Generic.List<Revit.DB.XYZ>() ;
        foreach ( Revit.DB.XYZ p in syukinUe1danPoints ) {
          syukinUe1danAllPoints.Add( p ) ;
        }

        syukinUe1danAllPoints.Add( leftTopRebar ) ;
        syukinUe1danAllPoints.Add( rightTopRebar ) ;

        // 主筋上2段筋座標
        Collections.Generic.IList<Revit.DB.XYZ> syukinUe2danPoints = new Collections.Generic.List<Revit.DB.XYZ>() ;

        // 2段筋
        // 2段筋配置太径細径順序記録(四隅を含む)
        // 0 = 太径、1 = 細径
        Collections.Generic.IList<int> ue2danRebarOrder = RebarOrder_Beam( syukinUe2danHutokeiHonsu, syukinUe2danHosokeiHonsu ) ;

        // 上2段筋本数
        int ue2danRebarCount = syukinUe2danHutokeiHonsu + syukinUe2danHosokeiHonsu ;
        // 上2段筋記号中心間距離
        double ue2danRebarDistance = ( rightBottomRebar.X - leftBottomRebar.X ) / ( ue2danRebarCount - 1 ) ;

        // 2段筋太径は1段筋太径の位置
        syukinUe1HutoPoints.Add( leftTopRebar ) ;
        syukinUe1HutoPoints.Add( rightTopRebar ) ;

        Collections.Generic.IList<Revit.DB.XYZ> syukinUe2HutoSeted = new Collections.Generic.List<Revit.DB.XYZ>() ; // _CmpGeometry.SetNextRowHutoRebar(syukinUe2danHutokeiHonsu, SortByXAry(syukinUe1HutoPoints), new Revit.DB.XYZ(0, -diaSyukinUeHuto1_5, 0));

        Collections.Generic.IList<Revit.DB.XYZ> syukinUe2HosoSeted = new Collections.Generic.List<Revit.DB.XYZ>() ; // _CmpGeometry.SetNextRowHosoRebar(syukinUe2danHosokeiHonsu, syukinUe1danAllPoints, syukinUe2HutoSeted, center, new Revit.DB.XYZ(0, -diaSyukinUeHuto1_5, 0), false);

        // 1段細径あり
        if ( syukinUe1danHosoPoints.Count > 0 && famSymSyukinUeHoso != null ) {
          syukinUe2HutoSeted = _CmpGeometry.SetNextRowHutoRebar( syukinUe2danHutokeiHonsu, SortByXAry( syukinUe1HutoPoints ), new Revit.DB.XYZ( 0, -diaSyukinUeHuto1_5, 0 ) ) ;
          syukinUe2HosoSeted = _CmpGeometry.SetNextRowHosoRebar( syukinUe2danHosokeiHonsu, syukinUe1danAllPoints, syukinUe2HutoSeted, center, new Revit.DB.XYZ( 0, -diaSyukinUeHuto1_5, 0 ), false ) ;
        }
        // 1段細径なし
        else {
          _CmpGeometry.SetNextRowRebar( ue2danRebarOrder, SortByXAry( syukinUe1HutoPoints ), ref syukinUe2HutoSeted, ref syukinUe2HosoSeted, new Revit.DB.XYZ( 0, -diaSyukinUeHuto1_5, 0 ) ) ;
        }

        foreach ( Revit.DB.XYZ point in syukinUe2HutoSeted ) {
          syukinUeHutokeiPoints.Add( point ) ;
          syukinUe2danPoints.Add( point ) ;

          ueSaikakin = point ;
        }

        foreach ( Revit.DB.XYZ point in syukinUe2HosoSeted ) {
          syukinUeHosokeiPoints.Add( point ) ;
          syukinUe2danPoints.Add( point ) ;
        }

        SortByXAry( syukinUe2danPoints ) ;

        #region 3段筋エラー判定

        // 3段筋太径
        // 上端
        if ( syukinUe3danHutokeiHonsu > 0 ) {
          if ( syukinUe2danHutokeiHonsu < syukinUe3danHutokeiHonsu ) {
            //goto draw;
            errorUe = true ;
          }
        }

        // 3段筋細径
        // 上端
        if ( syukinUe3danHosokeiHonsu > 0 ) {
          if ( syukinUe3danHutokeiHonsu <= syukinUe3danHosokeiHonsu ) {
            //goto draw;
            errorUe = true ;
          }
        }

        // 合計
        if ( syukinUe2danHutokeiHonsu + syukinUe2danHosokeiHonsu < syukinUe3danHutokeiHonsu + syukinUe3danHosokeiHonsu ) {
          //goto draw;
          errorUe = true ;
        }

        #endregion

        // 3段筋太径本数 > 3段筋細径本数

        #region

        if ( syukinUe2danPoints.Count >= syukinUe3danHutokeiHonsu + syukinUe3danHosokeiHonsu && syukinUe3danHutokeiHonsu > syukinUe3danHosokeiHonsu && errorUe == false ) {
          // 3段筋
          // 3段筋配置太径細径順序記録(四隅を含む)
          // 0 = 太径、1 = 細径
          Collections.Generic.IList<int> ue3danRebarOrder = RebarOrder_Beam( syukinUe3danHutokeiHonsu, syukinUe3danHosokeiHonsu ) ;

          // 上3段筋本数
          int ue3danRebarCount = syukinUe3danHutokeiHonsu + syukinUe3danHosokeiHonsu ;
          // 上3段筋記号中心間距離
          double ue3danRebarDistance = ( rightBottomRebar.X - leftBottomRebar.X ) / ( ue3danRebarCount - 1 ) ;

          // 並び
          Collections.Generic.IList<Revit.DB.XYZ> syukinUe3HutoSeted = new Collections.Generic.List<Revit.DB.XYZ>() ; // _CmpGeometry.SetNextRowHutoRebar(syukinUe3danHutokeiHonsu, SortByXAry(syukinUe2HutoSeted), new Revit.DB.XYZ(0, -diaSyukinUeHuto1_5, 0));
          Collections.Generic.IList<Revit.DB.XYZ> syukinUe3HosoSeted = new Collections.Generic.List<Revit.DB.XYZ>() ; // _CmpGeometry.SetNextRowHosoRebar(syukinUe3danHosokeiHonsu, syukinUe2danPoints, syukinUe3HutoSeted, center, new Revit.DB.XYZ(0, -diaSyukinUeHuto1_5, 0), false);

          // 2段細径あり
          if ( syukinUe1danHosoPoints.Count > 0 && famSymSyukinUeHoso != null ) {
            syukinUe3HutoSeted = _CmpGeometry.SetNextRowHutoRebar( syukinUe3danHutokeiHonsu, SortByXAry( syukinUe2HutoSeted ), new Revit.DB.XYZ( 0, -diaSyukinUeHuto1_5, 0 ) ) ;
            syukinUe3HosoSeted = _CmpGeometry.SetNextRowHosoRebar( syukinUe3danHosokeiHonsu, syukinUe2danPoints, syukinUe3HutoSeted, center, new Revit.DB.XYZ( 0, -diaSyukinUeHuto1_5, 0 ), false ) ;
          }
          // 2段細径なし
          else {
            _CmpGeometry.SetNextRowRebar( ue3danRebarOrder, SortByXAry( syukinUe2HutoSeted ), ref syukinUe3HutoSeted, ref syukinUe3HosoSeted, new Revit.DB.XYZ( 0, -diaSyukinUeHuto1_5, 0 ) ) ;
          }

          foreach ( Revit.DB.XYZ point in syukinUe3HutoSeted ) {
            syukinUeHutokeiPoints.Add( point ) ;

            ueSaikakin = point ;
          }

          foreach ( Revit.DB.XYZ point in syukinUe3HosoSeted ) {
            syukinUeHosokeiPoints.Add( point ) ;
          }
        }

        #endregion
      }

      #endregion

      #region 下端2段筋エラー判定

      // 2段筋太径
      // 下端
      if ( syukinSita2danHutokeiHonsu > 0 ) {
        if ( syukinSita1danHutokeiHonsu < syukinSita2danHutokeiHonsu ) {
          // goto draw;
          errorSita = true ;
        }
      }

      // 2段筋細径
      // 下端
      if ( syukinSita2danHosokeiHonsu > 0 ) {
        if ( syukinSita2danHutokeiHonsu <= syukinSita2danHosokeiHonsu ) {
          // goto draw;
          errorSita = true ;
        }
      }

      // 合計
      if ( syukinSita1danHutokeiHonsu + syukinSita1danHosokeiHonsu < syukinSita2danHutokeiHonsu + syukinSita2danHosokeiHonsu ) {
        // goto draw;
        errorSita = true ;
      }

      #endregion

      // 下端2段筋太径本数 > 2段筋細径本数

      #region

      if ( syukinSita2danHutokeiHonsu > syukinSita2danHosokeiHonsu && errorSita == false ) {
        Collections.Generic.IList<Revit.DB.XYZ> syukinSita1danAllPoints = new Collections.Generic.List<Revit.DB.XYZ>() ;
        foreach ( Revit.DB.XYZ p in syukinSita1danPoints ) {
          syukinSita1danAllPoints.Add( p ) ;
        }

        syukinSita1danAllPoints.Add( leftBottomRebar ) ;
        syukinSita1danAllPoints.Add( rightBottomRebar ) ;
        // 主筋下2段筋座標
        Collections.Generic.IList<Revit.DB.XYZ> syukinSita2danPoints = new Collections.Generic.List<Revit.DB.XYZ>() ;

        Collections.Generic.IList<int> sita2danRebarOrder = RebarOrder_Beam( syukinSita2danHutokeiHonsu, syukinSita2danHosokeiHonsu ) ;

        // 下2段筋本数
        int sita2danRebarCount = syukinSita2danHutokeiHonsu + syukinSita2danHosokeiHonsu ;
        // 下2段筋記号中心間距離
        double sita2danRebarDistance = ( rightBottomRebar.X - leftBottomRebar.X ) / ( sita2danRebarCount - 1 ) ;

        // 並び
        syukinSita1HutoPoints.Add( leftBottomRebar ) ;
        syukinSita1HutoPoints.Add( rightBottomRebar ) ;

        Collections.Generic.IList<Revit.DB.XYZ> syukinSita2HutoSeted = _CmpGeometry.SetNextRowHutoRebar( syukinSita2danHutokeiHonsu, SortByXAry( syukinSita1HutoPoints ), new Revit.DB.XYZ( 0, diaSyukinUeHuto1_5, 0 ) ) ;
        Collections.Generic.IList<Revit.DB.XYZ> syukinSita2HosoSeted = _CmpGeometry.SetNextRowHosoRebar( syukinSita2danHosokeiHonsu, syukinSita1danAllPoints, syukinSita2HutoSeted, center, new Revit.DB.XYZ( 0, diaSyukinUeHuto1_5, 0 ), false ) ;

        foreach ( Revit.DB.XYZ point in syukinSita2HutoSeted ) {
          syukinSitaHutokeiPoints.Add( point ) ;
          syukinSita2danPoints.Add( point ) ;

          sitaSaijokin = point ;
        }

        foreach ( Revit.DB.XYZ point in syukinSita2HosoSeted ) {
          syukinSitaHosokeiPoints.Add( point ) ;
          syukinSita2danPoints.Add( point ) ;
        }

        SortByXAry( syukinSita2danPoints ) ;

        #region 3段筋エラー判定

        // 3段筋太径
        // 下端
        if ( syukinSita3danHutokeiHonsu > 0 ) {
          if ( syukinSita2danHutokeiHonsu < syukinSita3danHutokeiHonsu ) {
            // goto draw;
            errorSita = true ;
          }
        }

        // 3段筋細径
        // 下端
        if ( syukinSita3danHosokeiHonsu > 0 ) {
          if ( syukinSita3danHutokeiHonsu <= syukinSita3danHosokeiHonsu ) {
            // goto draw;
            errorSita = true ;
          }
        }

        // 合計
        if ( syukinSita2danHutokeiHonsu + syukinSita2danHosokeiHonsu < syukinSita3danHutokeiHonsu + syukinSita3danHosokeiHonsu ) {
          // goto draw;
          errorSita = true ;
        }

        #endregion

        // 3段筋太径本数 > 3段筋細径本数

        #region

        if ( syukinSita2danPoints.Count >= syukinSita3danHutokeiHonsu + syukinSita3danHosokeiHonsu && syukinSita3danHutokeiHonsu > syukinSita3danHosokeiHonsu && errorSita == false ) {
          Collections.Generic.IList<int> sita3danRebarOrder = RebarOrder_Beam( syukinSita3danHutokeiHonsu, syukinSita3danHosokeiHonsu ) ;

          // 下3段筋本数
          int sita3danRebarCount = syukinSita3danHutokeiHonsu + syukinSita3danHosokeiHonsu ;
          // 下3段筋記号中心間距離
          double sita3danRebarDistance = ( rightBottomRebar.X - leftBottomRebar.X ) / ( sita3danRebarCount - 1 ) ;

          // 並び
          Collections.Generic.IList<Revit.DB.XYZ> syukinSita3HutoSeted = _CmpGeometry.SetNextRowHutoRebar( syukinSita3danHutokeiHonsu, SortByXAry( syukinSita2HutoSeted ), new Revit.DB.XYZ( 0, diaSyukinUeHuto1_5, 0 ) ) ;
          Collections.Generic.IList<Revit.DB.XYZ> syukinSita3HosoSeted = _CmpGeometry.SetNextRowHosoRebar( syukinSita3danHosokeiHonsu, syukinSita2danPoints, syukinSita3HutoSeted, center, new Revit.DB.XYZ( 0, diaSyukinUeHuto1_5, 0 ), false ) ;

          foreach ( Revit.DB.XYZ point in syukinSita3HutoSeted ) {
            syukinSitaHutokeiPoints.Add( point ) ;

            sitaSaijokin = point ;
          }

          foreach ( Revit.DB.XYZ point in syukinSita3HosoSeted ) {
            syukinSitaHosokeiPoints.Add( point ) ;
          }
        }

        #endregion
      }

      #endregion

      if ( errorUe == true || errorSita == true ) {
        goto draw ;
      }

      _InnerTop = ueSaikakin ;
      _InnerBottom = sitaSaijokin ;
      _RebarTop = ( leftTopRebar + _InnerTop ) / 2 ;
      _RebarBtm = ( leftBottomRebar + InnerBottom ) / 2 ;

      #region 肋筋エラー判定

      if ( stirrupNumber < 2 || syukinUe1danHutokeiHonsu + syukinUe1danHosokeiHonsu < stirrupNumber || syukinSita1danHutokeiHonsu + syukinSita1danHosokeiHonsu < stirrupNumber ) {
        goto draw ;
      }

      #endregion

      // 肋筋
      foreach ( Revit.DB.XYZ point in stirrupXPoints ) {
        Revit.DB.XYZ stirrupTop = new Revit.DB.XYZ( point.X, leftTopRebar.Y + diaSyukinUeHuto / 2, leftTopRebar.Z ) ;
        Revit.DB.XYZ stirrupBottom = new Revit.DB.XYZ( point.X, leftBottomRebar.Y - diaSyukinSitaHuto / 2, leftBottomRebar.Z ) ;

        stirrupLine = _CmpElements.CreateBoundLine( stirrupTop, stirrupBottom ) ;
        _CmpElements.NotNullCurveSet( ref stirrupCrvAry, stirrupLine ) ;
      }

      //}

      #region 腹筋エラー判定

      if ( webNumber > 0 ) {
        if ( webNumber % 2 != 0 ) {
          goto draw ;
        }
      }

      #endregion

      #region 腹筋、幅止筋

      if ( webNumber >= 2 && webNumber % 2 == 0 ) {
        // ueSaikakin、sitaSaijokinはYの基準
        // Xの基準は四隅筋

        double webDistance = ueSaikakin.Y - sitaSaijokin.Y ;
        double webPitch = webDistance / ( webNumber / 2 + 1 ) ;

        Revit.DB.XYZ baseLeft = new Revit.DB.XYZ( leftBottomRebar.X - diaSyukinSitaHuto / 2 + diaWeb / 2, sitaSaijokin.Y, leftBottomRebar.Z ) ;
        Revit.DB.XYZ baseRight = new Revit.DB.XYZ( rightBottomRebar.X + diaSyukinSitaHuto / 2 - diaWeb / 2, sitaSaijokin.Y, rightBottomRebar.Z ) ;

        for ( int i = 0 ; i < webNumber / 2 ; ++i ) {
          Revit.DB.XYZ leftWeb = baseLeft + new Revit.DB.XYZ( 0, webPitch * ( i + 1 ), 0 ) ;
          Revit.DB.XYZ rightWeb = baseRight + new Revit.DB.XYZ( 0, webPitch * ( i + 1 ), 0 ) ;

          webPoints.Add( leftWeb ) ;
          webPoints.Add( rightWeb ) ;
        }

        #region 幅止筋エラー判定

        if ( spacingNumber > 0 ) {
          if ( spacingNumber * 2 > webNumber ) {
            goto draw ;
          }
        }

        #endregion

        // 幅止筋
        if ( webPoints.Count >= 2 && webPoints.Count / 2 >= spacingNumber ) {
          for ( int i = 0 ; i < spacingNumber ; ++i ) {
            Revit.DB.XYZ leftSpacing = webPoints[ i * 2 ] ;
            Revit.DB.XYZ rightSpacing = webPoints[ i * 2 + 1 ] ;

            leftSpacing += new Revit.DB.XYZ( 0, diaWeb / 2, 0 ) ;
            rightSpacing += new Revit.DB.XYZ( 0, diaWeb / 2, 0 ) ;

            Revit.DB.Line spaceLine = _CmpElements.CreateBoundLine( leftSpacing, rightSpacing ) ;
            _CmpElements.NotNullCurveSet( ref spaceCrvAry, spaceLine ) ;
          }
        }
      }

      #endregion

      // エラー時のgoto先
      draw:

      // 線種
      Revit.DB.GraphicsStyle bodyLineType = _CmpElements.BodyLineStyle ;
      Revit.DB.GraphicsStyle spaceLineType = _CmpElements.SpacerLineStyle ;

      // 肋筋作成
      trans.Start( "肋筋" ) ;
      foreach ( Revit.DB.Curve crv in stirrupCrvAry ) {
        Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve( actView, crv ) ;
        dc.LineStyle = bodyLineType ;
      }

      trans.Commit() ;

      // 幅止筋
      trans.Start( "幅止筋" ) ;
      foreach ( Revit.DB.Curve crv in spaceCrvAry ) {
        Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve( actView, crv ) ;
        dc.LineStyle = spaceLineType ;
      }

      trans.Commit() ;

      if ( famSymSyukinUeHuto != null ) {
        trans.Start( "主筋上太径" ) ;
        foreach ( Revit.DB.XYZ pnt in syukinUeHutokeiPoints ) {
          Revit.DB.FamilyInstance famInsRebar = rvtDbDoc.Create.NewFamilyInstance( pnt, famSymSyukinUeHuto, actView ) ;
        }

        trans.Commit() ;
      }

      if ( famSymSyukinUeHoso != null ) {
        trans.Start( "主筋上細径" ) ;
        foreach ( Revit.DB.XYZ pnt in syukinUeHosokeiPoints ) {
          Revit.DB.FamilyInstance famInsRebar = rvtDbDoc.Create.NewFamilyInstance( pnt, famSymSyukinUeHoso, actView ) ;
        }

        trans.Commit() ;
      }

      if ( famSymSyukinSitaHuto != null ) {
        trans.Start( "主筋下太径" ) ;
        foreach ( Revit.DB.XYZ pnt in syukinSitaHutokeiPoints ) {
          Revit.DB.FamilyInstance famInsRebar = rvtDbDoc.Create.NewFamilyInstance( pnt, famSymSyukinSitaHuto, actView ) ;
        }

        trans.Commit() ;
      }

      if ( famSymSyukinSitaHoso != null ) {
        trans.Start( "主筋下細径" ) ;
        foreach ( Revit.DB.XYZ pnt in syukinSitaHosokeiPoints ) {
          Revit.DB.FamilyInstance famInsRebar = rvtDbDoc.Create.NewFamilyInstance( pnt, famSymSyukinSitaHoso, actView ) ;
        }

        trans.Commit() ;
      }

      if ( famSymWeb != null ) {
        trans.Start( "腹筋" ) ;
        foreach ( Revit.DB.XYZ pnt in webPoints ) {
          Revit.DB.FamilyInstance famInsRebar = rvtDbDoc.Create.NewFamilyInstance( pnt, famSymWeb, actView ) ;
        }

        trans.Commit() ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>設定ファイル書き出し - 名前を付けて保存</summary>
    ///
    /// <history><p>2013/04/22 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public void WriteSettingValues( Collections.Generic.IList<string> commonAry, Collections.Generic.IList<string> columnAry, Collections.Generic.IList<string> beamAry1, Collections.Generic.IList<string> beamAry2, Collections.Generic.IList<string> paramAry )
    {
      string settingFileName = "" ;
      string settingFileDirectory = "" ;
      string levelSortOrder = "" ;

      GetString( ref settingFileName, ref settingFileDirectory, ref levelSortOrder ) ;

      System.Windows.Forms.SaveFileDialog saveFileDlg = new System.Windows.Forms.SaveFileDialog() ;
      saveFileDlg.InitialDirectory = settingFileDirectory ;
      saveFileDlg.Filter = "Text File (*.txt)|*.txt" ;

      if ( saveFileDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK ) {
        string saveFileName = saveFileDlg.FileName ;
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        System.Text.Encoding enc = System.Text.Encoding.GetEncoding( "shift_jis" ) ;

        Collections.Generic.IList<string> mappingAry_LevelSort = new Collections.Generic.List<string>() ;

        if ( settingFileDirectory != "" && settingFileName != "" && settingFileDirectory != null && settingFileName != null && System.IO.File.Exists( settingFileDirectory + settingFileName ) == true ) {
          string[] strAry = System.IO.File.ReadAllLines( settingFileDirectory + settingFileName, enc ) ;

          int num = 0 ;
          foreach ( string str in strAry ) {
            if ( num > 62 ) {
              mappingAry_LevelSort.Add( str ) ;
            }

            num += 1 ;
          }
        }
        else {
          Collections.Generic.IList<string> defaultParam = _CmpParameters.DefaultRebarParameter() ;

          foreach ( string str in defaultParam ) {
            mappingAry_LevelSort.Add( str ) ;
          }

          mappingAry_LevelSort.Add( "" ) ;
          mappingAry_LevelSort.Add( "" ) ;
          mappingAry_LevelSort.Add( "***** end *****" ) ;
        }

        string write = "" ;

        foreach ( string str in commonAry ) {
          write += str + System.Environment.NewLine ;
        }

        write += System.Environment.NewLine ;
        foreach ( string str in columnAry ) {
          write += str + System.Environment.NewLine ;
        }

        write += System.Environment.NewLine ;
        foreach ( string str in beamAry1 ) {
          write += str + System.Environment.NewLine ;
        }

        write += System.Environment.NewLine ;
        foreach ( string str in beamAry2 ) {
          write += str + System.Environment.NewLine ;
        }

        write += System.Environment.NewLine ;
        foreach ( string str in paramAry ) {
          write += str + System.Environment.NewLine ;
        }

        write += System.Environment.NewLine ;
        foreach ( string str in mappingAry_LevelSort ) {
          write += str + System.Environment.NewLine ;
        }

        // 書き出し(ファイルが存在するときは上書き)
        System.IO.File.WriteAllText( saveFileName, write, enc ) ;
      }
    }

    /// ================================================================================
    /// <summary>設定ファイル書き出し - 上書き保存</summary>
    ///
    /// <history><p>2013/04/22 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public void OverWriteSettingValues( Collections.Generic.IList<string> commonAry, Collections.Generic.IList<string> columnAry, Collections.Generic.IList<string> beamAry1, Collections.Generic.IList<string> beamAry2, Collections.Generic.IList<string> paramAry )
    {
      string settingFileName = "" ;
      string settingFileDirectory = "" ;
      string levelSortOrder = "" ;

      GetString( ref settingFileName, ref settingFileDirectory, ref levelSortOrder ) ;

      if ( settingFileDirectory != "" && settingFileName != "" && settingFileDirectory != null && settingFileName != null && System.IO.File.Exists( settingFileDirectory + settingFileName ) == true ) {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        System.Text.Encoding enc = System.Text.Encoding.GetEncoding( "shift_jis" ) ;
        string[] strAry = System.IO.File.ReadAllLines( settingFileDirectory + settingFileName, enc ) ;

        Collections.Generic.IList<string> mappingAry_LevelSort = new Collections.Generic.List<string>() ;

        //int num = 0;
        //foreach (string str in strAry)
        //{
        //  if (num > 62)
        //  {
        //    mappingAry_LevelSort.Add(str);
        //  }

        //  num += 1;
        //}

        string write = "" ;

        foreach ( string str in commonAry ) {
          write += str + System.Environment.NewLine ;
        }

        write += System.Environment.NewLine ;
        foreach ( string str in columnAry ) {
          write += str + System.Environment.NewLine ;
        }

        write += System.Environment.NewLine ;
        foreach ( string str in beamAry1 ) {
          write += str + System.Environment.NewLine ;
        }

        write += System.Environment.NewLine ;
        foreach ( string str in beamAry2 ) {
          write += str + System.Environment.NewLine ;
        }

        write += System.Environment.NewLine ;
        foreach ( string str in paramAry ) {
          write += str + System.Environment.NewLine ;
        }
        //write += System.Environment.NewLine;
        //foreach (string str in mappingAry_LevelSort)
        //{
        //  write += str + System.Environment.NewLine;
        //}

        // 書き出し(ファイルが存在するときは上書き)
        System.IO.File.WriteAllText( settingFileDirectory + settingFileName, write, enc ) ;
      }
      else {
        WriteSettingValues( commonAry, columnAry, beamAry1, beamAry2, paramAry ) ;
      }
    }

    /// ================================================================================
    /// <summary>Excel確認</summary>
    ///
    /// <history>2014/09/01 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public bool IsExcelInComputer()
    {
      // 戻り値
      bool ret = false ;

      try {
        System.Type type = System.Type.GetTypeFromProgID( "Excel.Application" ) ;

        // Wordの場合
        //System.Type wordType = System.Type.GetTypeFromProgID("Word.Application");

        if ( type == null ) {
          ret = false ;
        }
        else if ( type != null ) {
          ret = true ;
        }
      }
      catch {
        return ret ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>文字作成</summary>
    ///
    /// <param name="view"      >ビュー</param>
    /// <param name="origin"    >原点</param>
    /// <param name="baseVec"   >文字方向</param>
    /// <param name="lineWidth" >幅</param>
    /// <param name="typeId"    >タイプID</param>
    /// <param name="text"      >文字</param>
    /// <param name="doc"       >ドキュメント</param>
    ///
    /// <history><p>2015/04/28 Created GSA,Inc. Ryo Kuroda</p>
    ///           <p>2015/06/25 Modified GSA, Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Revit.DB.TextNote CreateNewTextNote( Revit.DB.View view, Revit.DB.XYZ origin, Revit.DB.XYZ baseVec, double lineWidth, Revit.DB.ElementId typeId, string text, Revit.DB.Document doc )
    {
      Revit.DB.TextNote ret = null ;

      if ( text == "" ) {
        return ret ;
      }

      Revit.DB.XYZ p0 = new Revit.DB.XYZ( 0, 0, 0 ) ;
      Revit.DB.XYZ p1 = new Revit.DB.XYZ( 1, 0, 0 ) ;
      Revit.DB.XYZ p2 = baseVec ;

      // 回転角
      double dotProduct = _CmpGeometry.DotProduct2D( p0, p1, p2 ) ;
      double crossProduct = _CmpGeometry.CrossProduct2D( p0, p1, p2 ) ;
      double rotate = System.Math.Atan2( crossProduct, dotProduct ) ;

      // 各種設定
      Revit.DB.TextNoteOptions opt = new Revit.DB.TextNoteOptions() ;

      opt.HorizontalAlignment = Revit.DB.HorizontalTextAlignment.Center ;
      opt.KeepRotatedTextReadable = false ;
      opt.Rotation = rotate ;
      opt.TypeId = typeId ;

      // 作成
      trans.Start( "Location.Move" ) ;
      ret = Revit.DB.TextNote.Create( doc, view.Id, origin, text, opt ) ;
      trans.Commit() ;

      double txtMove = ret.Height * view.Scale / 2.0 ;

      trans.Start( "TextNote.Create" ) ;
      // 外形
      Revit.DB.BoundingBoxXYZ bbXYZ = ret.get_BoundingBox( view ) ;
      Revit.DB.XYZ max = bbXYZ.Max ;
      Revit.DB.XYZ min = bbXYZ.Min ;

      if ( rotate == 0.0 ) {
        // 移動量
        // 外形の縦方向の半分
        //          double dis = (origin.Y - min.Y) / 2;
        double dis = txtMove ;
        Revit.DB.XYZ move = new Revit.DB.XYZ( baseVec.Y * dis, baseVec.X * dis, baseVec.Z * dis ) ;

        // 移動
        ret.Location.Move( move ) ;
      }
      else if ( rotate != 0.0 ) {
        // 移動量
        //          double dis = (origin.X - max.X) / 2;
        double dis = -txtMove ;
        Revit.DB.XYZ move = new Revit.DB.XYZ( baseVec.Y * dis, baseVec.X * dis, baseVec.Z * dis ) ;

        // 移動
        ret.Location.Move( move ) ;
      }

      trans.Commit() ;
      return ret ;
    }

    /// ================================================================================
    /// <summary>文字作成</summary>
    ///
    /// <param name="view"      >ビュー</param>
    /// <param name="origin"    >原点</param>
    /// <param name="baseVec"   >文字方向</param>
    /// <param name="lineWidth" >幅</param>
    /// <param name="typeId"    >タイプID</param>
    /// <param name="text"      >文字</param>
    /// <param name="doc"       >ドキュメント</param>
    /// <param name="offset"    >オフセット</param>
    ///
    /// <history><p>2015/04/28 Created GSA,Inc. Ryo Kuroda</p>
    ///           <p>2015/06/25 Modified GSA, Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Revit.DB.TextNote CreateNewTextNote_Offset( Revit.DB.View view, Revit.DB.XYZ origin, Revit.DB.XYZ baseVec, double lineWidth, Revit.DB.ElementId typeId, string text, Revit.DB.Document doc, double offset )
    {
      Revit.DB.TextNote ret = null ;

      if ( text == "" ) {
        return ret ;
      }

      Revit.DB.XYZ p0 = new Revit.DB.XYZ( 0, 0, 0 ) ;
      Revit.DB.XYZ p1 = new Revit.DB.XYZ( 1, 0, 0 ) ;
      Revit.DB.XYZ p2 = baseVec ;

      // 回転角
      double dotProduct = _CmpGeometry.DotProduct2D( p0, p1, p2 ) ;
      double crossProduct = _CmpGeometry.CrossProduct2D( p0, p1, p2 ) ;
      double rotate = System.Math.Atan2( crossProduct, dotProduct ) ;

      // 各種設定
      Revit.DB.TextNoteOptions opt = new Revit.DB.TextNoteOptions() ;

      opt.HorizontalAlignment = Revit.DB.HorizontalTextAlignment.Center ;
      opt.KeepRotatedTextReadable = false ;
      opt.Rotation = rotate ;
      opt.TypeId = typeId ;

      // 作成
      trans.Start( "Location.Move" ) ;
      ret = Revit.DB.TextNote.Create( doc, view.Id, origin, text, opt ) ;
      trans.Commit() ;

      double txtMove = ret.Height * view.Scale / 2.0 ;

      // 外形
      Revit.DB.BoundingBoxXYZ bbXYZ = ret.get_BoundingBox( view ) ;
      Revit.DB.XYZ max = bbXYZ.Max ;
      Revit.DB.XYZ min = bbXYZ.Min ;

      trans.Start( "TextNote.Create" ) ;
      if ( rotate == 0.0 ) {
        // 移動量
        // 外形の縦方向の半分 + オフセット
        //          double dis = (origin.Y - min.Y) / 2 + offset;
        double dis = txtMove + offset ;
        Revit.DB.XYZ move = new Revit.DB.XYZ( baseVec.Y * dis, baseVec.X * dis, baseVec.Z * dis ) ;

        // 移動
        ret.Location.Move( move ) ;
      }
      else if ( rotate != 0.0 ) {
        // 移動量
        //  外形の横方向の半分 + オフセット
        //          double dis = (origin.X - max.X) / 2 + offset;
        double dis = -txtMove + offset ;
        Revit.DB.XYZ move = new Revit.DB.XYZ( baseVec.Y * dis, baseVec.X * dis, baseVec.Z * dis ) ;

        // 移動
        ret.Location.Move( move ) ;
      }

      trans.Commit() ;

      return ret ;
    }

    /// ================================================================================
    /// <summary>文字作成</summary>
    ///
    /// <param name="view"      >ビュー</param>
    /// <param name="origin"    >原点</param>
    /// <param name="baseVec"   >文字方向</param>
    /// <param name="lineWidth" >幅</param>
    /// <param name="typeId"    >タイプID</param>
    /// <param name="text"      >文字</param>
    /// <param name="doc"       >ドキュメント</param>
    ///
    /// <history><p>2015/04/28 Created GSA,Inc. Ryo Kuroda</p>
    ///           <p>2015/06/25 Modified GSA, Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Revit.DB.TextNote CreateNewTextNote_RebarSyukinOffset( Revit.DB.View view, Revit.DB.XYZ origin, Revit.DB.XYZ baseVec, double lineWidth, Revit.DB.ElementId typeId, string text, Revit.DB.Document doc )
    {
      Revit.DB.TextNote ret = null ;

      if ( text == "" ) {
        return ret ;
      }

      Revit.DB.XYZ p0 = new Revit.DB.XYZ( 0, 0, 0 ) ;
      Revit.DB.XYZ p1 = new Revit.DB.XYZ( 1, 0, 0 ) ;
      Revit.DB.XYZ p2 = baseVec ;

      // 回転角
      double dotProduct = _CmpGeometry.DotProduct2D( p0, p1, p2 ) ;
      double crossProduct = _CmpGeometry.CrossProduct2D( p0, p1, p2 ) ;
      double rotate = System.Math.Atan2( crossProduct, dotProduct ) ;

      // 各種設定
      Revit.DB.TextNoteOptions opt = new Revit.DB.TextNoteOptions() ;

      opt.HorizontalAlignment = Revit.DB.HorizontalTextAlignment.Center ;
      opt.KeepRotatedTextReadable = false ;
      opt.Rotation = rotate ;
      opt.TypeId = typeId ;

      // 作成
      // 右寄せで作ると原点が右端になる
      trans.Start( "CreateTextNote" ) ;
      ret = _CmpElements.CreateTextNote( view, origin, rotate, 0, Revit.DB.HorizontalTextAlignment.Center, typeId, text ) ;
      trans.Commit() ;

      double txtMove = ret.Height * view.Scale / 2.0 ;

      // 外形
      Revit.DB.BoundingBoxXYZ bbXYZ = ret.get_BoundingBox( view ) ;
      Revit.DB.XYZ max = bbXYZ.Max ;
      Revit.DB.XYZ min = bbXYZ.Min ;
      Revit.DB.XYZ mid = ( max + min ) / 2 ;

      // 移動量
      double x = ( origin.X - min.X ) / 2 + view.Scale / 304.8 ;
      //        double y = (origin.Y - min.Y) / 2;
      double y = txtMove ;
      Revit.DB.XYZ move = new Revit.DB.XYZ( x, y, 0 ) ;

      // 移動
      trans.Start( "Location.Move" ) ;
      ret.Location.Move( move ) ;

      // 後から右寄せに変更
      ret.HorizontalAlignment = Revit.DB.HorizontalTextAlignment.Right ;
      trans.Commit() ;

      return ret ;
    }

    /// ================================================================================
    /// <summary>枠オフセット</summary>
    ///
    /// <param name="dcAry"     >詳細線分</param>
    /// <param name="lineStyle" >線種</param>
    /// <param name="view"      >ビュー</param>
    /// <param name="tx"        >トランザクション</param>
    ///
    /// <history>2018/04/04 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public Collections.Generic.IList<Revit.DB.ElementId> FrameOffset( Collections.Generic.IList<Revit.DB.DetailCurve> dcAry, Revit.DB.GraphicsStyle lineStyle, Revit.DB.View view, Revit.DB.Transaction tx )
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.ElementId> ret = new Collections.Generic.List<Revit.DB.ElementId>() ;

      // 最大X、最小Y座標
      double maxX = double.MinValue ;
      double minY = double.MaxValue ;

      foreach ( Revit.DB.DetailCurve _dc in dcAry ) {
        Revit.DB.Curve curve = _dc.GeometryCurve ;

        Revit.DB.XYZ p0 = curve.GetEndPoint( 0 ) ;
        Revit.DB.XYZ p1 = curve.GetEndPoint( 1 ) ;

        if ( maxX < p0.X ) {
          maxX = p0.X ;
        }

        if ( maxX < p1.X ) {
          maxX = p1.X ;
        }

        if ( minY > p0.Y ) {
          minY = p0.Y ;
        }

        if ( minY > p1.Y ) {
          minY = p1.Y ;
        }
      }

      // Z座標
      double z = dcAry[ 0 ].GeometryCurve.GetEndPoint( 0 ).Z ;

      double offset = 20 / 304.8 ;

      // 1mmの線分作成

      // 左上
      Revit.DB.XYZ pA = new Revit.DB.XYZ( -offset, offset, z ) ;
      Revit.DB.XYZ pB = new Revit.DB.XYZ( -offset + 1 / 304.8, offset, z ) ;

      Revit.DB.Line line = Revit.DB.Line.CreateBound( pA, pB ) ;

      tx.Start( "オフセット" ) ;

      Revit.DB.DetailCurve dc = _CmpElements.RvtDBDoc.Create.NewDetailCurve( view, line ) ;
      dc.LineStyle = lineStyle ;

      tx.Commit() ;

      ret.Add( dc.Id ) ;

      // 右下
      pA = new Revit.DB.XYZ( maxX + offset, minY - offset, z ) ;
      pB = new Revit.DB.XYZ( maxX + offset - 1 / 304.8, minY - offset, z ) ;

      line = Revit.DB.Line.CreateBound( pA, pB ) ;

      tx.Start( "オフセット" ) ;

      dc = _CmpElements.RvtDBDoc.Create.NewDetailCurve( view, line ) ;
      dc.LineStyle = lineStyle ;

      tx.Commit() ;

      ret.Add( dc.Id ) ;

      return ret ;
    }

    /// ================================================================================
    /// <summary>イメージ設定 - ファミリ</summary>
    ///
    /// <history><p>2018/04/27 Created CST,Co.Ltd. Ryo Kuroda</p>
    ///           <p>2018/05/28 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
    /// ================================================================================
    public string SetImageAsFamily( SectionListRC.JExtComCompat.ProgressBarThread thread )
    {
      // 戻り値
      string ret = "" ;

      // ファイル名使用禁止文字の置換
      char[] invalidChars = System.IO.Path.GetInvalidFileNameChars() ;

      // ファミリごとに整列
      Collections.Generic.IDictionary<long, Collections.Generic.IList<Collections.Generic.IList<string>>> dicFamAry = new Collections.Generic.Dictionary<long, Collections.Generic.IList<Collections.Generic.IList<string>>>() ;

      foreach ( long id in DicTypeId_ImagePath.Keys ) {
                Revit.DB.ElementId typeId = new Revit.DB.ElementId( id ) ;
        
        
        Revit.DB.FamilySymbol famSym = _CmpElements.RvtDBDoc.GetElement( typeId ) as Revit.DB.FamilySymbol ;

        string path = DicTypeId_ImagePath[ id ] ;
        string name = famSym.Name ;

        Collections.Generic.IList<string> list = new Collections.Generic.List<string>() ;
        list.Add( path ) ;
        list.Add( name ) ;

        Revit.DB.Family fam = famSym.Family ;

        var famId = fam.Id.Value ;

        if ( dicFamAry.ContainsKey( famId ) ) {
          dicFamAry[ famId ].Add( list ) ;
        }
        else {
          Collections.Generic.IList<Collections.Generic.IList<string>> lists = new Collections.Generic.List<Collections.Generic.IList<string>>() ;
          lists.Add( list ) ;

          dicFamAry.Add( famId, lists ) ;
        }
      }

      int max = 0 ;
      foreach ( int famId in dicFamAry.Keys ) {
        foreach ( Collections.Generic.IList<string> list in dicFamAry[ famId ] ) {
          max += 1 ;
        }
      }

      int count = 0 ;

      thread.SetData( _CmpAttribute.ResourceText( "IDS_TXT_IMAGESET" ), max, count ) ;
      thread.Active() ;

      foreach ( long famId in dicFamAry.Keys ) {
                Revit.DB.Family fam = _CmpElements.RvtDBDoc.GetElement( new Revit.DB.ElementId( famId ) ) as Revit.DB.Family ;
        

        // ファミリ初期化
        Revit.DB.Document famDoc = _CmpElements.RvtDBDoc.EditFamily( fam ) ;
        Revit.DB.FamilyManager famMgr = famDoc.FamilyManager ;
        Revit.DB.FamilyTypeSet famTypes = famMgr.Types ;

        // 既存イメージ
        Revit.DB.FilteredElementCollector fecImage = new Revit.DB.FilteredElementCollector( famDoc ) ;
        fecImage.OfCategory( Revit.DB.BuiltInCategory.OST_RasterImages ) ;

        Collections.Generic.IList<Revit.DB.Element> images = fecImage.ToElements() ;

        Collections.Generic.IDictionary<string, long> dicImages = new Collections.Generic.Dictionary<string, long>() ;
        foreach ( Revit.DB.Element image in images ) {
          dicImages.Add( image.Name, image.Id.Value ) ;
        }

        // ファミリドキュメントトランザクション
        Revit.DB.Transaction txFam = new Revit.DB.Transaction( famDoc ) ;

        foreach ( Collections.Generic.IList<string> list in dicFamAry[ famId ] ) {
          count += 1 ;
          thread.SetData( _CmpAttribute.ResourceText( "IDS_TXT_IMAGESET" ), max, count ) ;
          thread.Active() ;

          string file = list[ 0 ] ;
          string name = list[ 1 ] ;

          try {
            // 既存同名イメージ削除
            if ( dicImages.ContainsKey( name + ".png" ) ) {
              var id = dicImages[ name + ".png" ] ;

                            Revit.DB.ElementId imageId = new Revit.DB.ElementId( id ) ;
              

              txFam.Start( "削除" ) ;
              famDoc.Delete( imageId ) ;
              txFam.Commit() ;
            }
          }
          catch {
            if ( txFam.GetStatus() != Autodesk.Revit.DB.TransactionStatus.Committed ) {
              txFam.RollBack() ;
            }

            ret = "既存同名イメージの削除に失敗しました" ;
            return ret ;
          }

          Revit.DB.ImageType imageType = null ;

          try {
            // イメージタイプ作成
            txFam.Start( "イメージタイプ" ) ;

            ImageTypeOptions options = new ImageTypeOptions( file, false, ImageTypeSource.Import ) ;
            imageType = Revit.DB.ImageType.Create( famDoc, options ) ;
            txFam.Commit() ;
          }
          catch ( Exception ex ) {
            if ( txFam.GetStatus() != Autodesk.Revit.DB.TransactionStatus.Committed ) {
              txFam.RollBack() ;
            }

            string mess = ex.Message ;

            ret = "イメージタイプの作成に失敗しました" ;
            return ret ;
          }

          try {
            foreach ( Revit.DB.FamilyType famType in famTypes ) {
              if ( famType.Name == name ) {
                txFam.Start( "変更" ) ;
                famMgr.CurrentType = famType ;
                txFam.Commit() ;

                Revit.DB.FamilyParameter famParam = famMgr.get_Parameter( Revit.DB.BuiltInParameter.ALL_MODEL_TYPE_IMAGE ) ;

                txFam.Start( "セット" ) ;
                famMgr.Set( famParam, imageType.Id ) ;
                txFam.Commit() ;

                break ;
              }
            }
          }
          catch {
            if ( txFam.GetStatus() != Autodesk.Revit.DB.TransactionStatus.Committed ) {
              txFam.RollBack() ;
            }

            ret = "イメージタイプの設定に失敗しました" ;
            return ret ;
          }
        }

        try {
          txFam.Start( "リロード" ) ;
          famDoc.LoadFamily( _CmpElements.RvtDBDoc, new LoadOptions() ) ;
          txFam.Commit() ;

          famDoc.Close( false ) ;
        }
        catch {
          if ( txFam.GetStatus() != Autodesk.Revit.DB.TransactionStatus.Committed ) {
            txFam.RollBack() ;
          }

          ret = "ファミリの再ロードに失敗しました" ;
          return ret ;
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>書き出しフォルダの削除</summary>
    ///
    /// <history>2018/05/28 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public string DeleteExportFolder( string exportFolderPath )
    {
      // 戻り値
      string ret = "" ;

      try {
        string exportFolder = exportFolderPath + "\\柱" ;
        if ( System.IO.Directory.Exists( exportFolder ) ) {
          // フォルダ内のファイル
          string[] files = System.IO.Directory.GetFiles( exportFolder ) ;
          foreach ( string file in files ) {
            System.IO.File.Delete( file ) ;
          }

          System.IO.Directory.Delete( exportFolder ) ;
        }

        exportFolder = exportFolderPath + "\\間柱" ;
        if ( System.IO.Directory.Exists( exportFolder ) ) {
          // フォルダ内のファイル
          string[] files = System.IO.Directory.GetFiles( exportFolder ) ;
          foreach ( string file in files ) {
            System.IO.File.Delete( file ) ;
          }

          System.IO.Directory.Delete( exportFolder ) ;
        }

        exportFolder = exportFolderPath + "\\大梁" ;
        if ( System.IO.Directory.Exists( exportFolder ) ) {
          // フォルダ内のファイル
          string[] files = System.IO.Directory.GetFiles( exportFolder ) ;
          foreach ( string file in files ) {
            System.IO.File.Delete( file ) ;
          }

          System.IO.Directory.Delete( exportFolder ) ;
        }

        exportFolder = exportFolderPath + "\\小梁" ;
        if ( System.IO.Directory.Exists( exportFolder ) ) {
          // フォルダ内のファイル
          string[] files = System.IO.Directory.GetFiles( exportFolder ) ;
          foreach ( string file in files ) {
            System.IO.File.Delete( file ) ;
          }

          System.IO.Directory.Delete( exportFolder ) ;
        }

        exportFolder = exportFolderPath + "\\片持ち大梁" ;
        if ( System.IO.Directory.Exists( exportFolder ) ) {
          // フォルダ内のファイル
          string[] files = System.IO.Directory.GetFiles( exportFolder ) ;
          foreach ( string file in files ) {
            System.IO.File.Delete( file ) ;
          }

          System.IO.Directory.Delete( exportFolder ) ;
        }

        exportFolder = exportFolderPath + "\\片持ち小梁" ;
        if ( System.IO.Directory.Exists( exportFolder ) ) {
          // フォルダ内のファイル
          string[] files = System.IO.Directory.GetFiles( exportFolder ) ;
          foreach ( string file in files ) {
            System.IO.File.Delete( file ) ;
          }

          System.IO.Directory.Delete( exportFolder ) ;
        }

        exportFolder = exportFolderPath + "\\基礎大梁" ;
        if ( System.IO.Directory.Exists( exportFolder ) ) {
          // フォルダ内のファイル
          string[] files = System.IO.Directory.GetFiles( exportFolder ) ;
          foreach ( string file in files ) {
            System.IO.File.Delete( file ) ;
          }

          System.IO.Directory.Delete( exportFolder ) ;
        }

        exportFolder = exportFolderPath + "\\基礎小梁" ;
        if ( System.IO.Directory.Exists( exportFolder ) ) {
          // フォルダ内のファイル
          string[] files = System.IO.Directory.GetFiles( exportFolder ) ;
          foreach ( string file in files ) {
            System.IO.File.Delete( file ) ;
          }

          System.IO.Directory.Delete( exportFolder ) ;
        }

        exportFolder = exportFolderPath + "\\片持ち基礎大梁" ;
        if ( System.IO.Directory.Exists( exportFolder ) ) {
          // フォルダ内のファイル
          string[] files = System.IO.Directory.GetFiles( exportFolder ) ;
          foreach ( string file in files ) {
            System.IO.File.Delete( file ) ;
          }

          System.IO.Directory.Delete( exportFolder ) ;
        }

        exportFolder = exportFolderPath + "\\片持ち基礎小梁" ;
        if ( System.IO.Directory.Exists( exportFolder ) ) {
          // フォルダ内のファイル
          string[] files = System.IO.Directory.GetFiles( exportFolder ) ;
          foreach ( string file in files ) {
            System.IO.File.Delete( file ) ;
          }

          System.IO.Directory.Delete( exportFolder ) ;
        }
      }
      catch {
        ret = "イメージファイルの削除に失敗しました" ;
      }

      return ret ;
    }

    #endregion

    // プロパティ

    #region Properties

    /// ================================================================================
    /// <summary>文字列取得 - 階記号ソート順序</summary>
    ///
    /// <history><p>2013/07/10 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public string GetStringLevelSortOrder
    {
      get { return entDtCmd.LevelSortOrdeer ; }
    }

    /// ================================================================================
    /// <summary>X2段筋配置判定</summary>
    /// <history>2013/05/22 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public bool IsX2ndRebar
    {
      get { return _X2ndRebarIs ; }
    }

    /// ================================================================================
    /// <summary>Y2段筋配置判定</summary>
    /// <history>2013/05/22 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public bool IsY2ndRebar
    {
      get { return _Y2ndRebarIs ; }
    }

    /// ================================================================================
    /// <summary>X2段筋記号直径</summary>
    /// <history>2013/05/22 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public double X2ndSymbolDiameter
    {
      get { return _X2ndRebarDistance ; }
    }

    /// ================================================================================
    /// <summary>Y2段筋記号直径</summary>
    /// <history>2013/05/22 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public double Y2ndSymbolDiameter
    {
      get { return _Y2ndRebarDistance ; }
    }

    /// ================================================================================
    /// <summary>主筋太径</summary>
    /// <history>2013/05/23 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public double SyukinDiameter
    {
      get { return _SyukinHutokei ; }
    }

    /// ================================================================================
    /// <summary>上端筋最下段</summary>
    /// <history>2013/05/23 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public Revit.DB.XYZ InnerTop
    {
      get { return _InnerTop ; }
    }

    /// ================================================================================
    /// <summary>下端筋最上段</summary>
    /// <history>2013/05/23 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public Revit.DB.XYZ InnerBottom
    {
      get { return _InnerBottom ; }
    }

    /// ================================================================================
    /// <summary>上端筋中間</summary>
    /// <history>2015/04/07 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public Revit.DB.XYZ RebarTop
    {
      get { return _RebarTop ; }
    }

    /// ================================================================================
    /// <summary>下端筋中間</summary>
    /// <history>2015/04/07 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public Revit.DB.XYZ RebarBtm
    {
      get { return _RebarBtm ; }
    }

    /// ================================================================================
    /// <summary>タイプIDとイメージパス</summary>
    /// <history>2018/04/02 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public Collections.Generic.IDictionary<long, string> DicTypeId_ImagePath
    {
      get { return _DicTypeId_ImagePath ; }
      set { _DicTypeId_ImagePath = value ; }
    }

    #endregion
  }

  /// <summary>符号名の並び替え</summary>
  public class HugoNameComparer : System.Collections.Generic.IComparer<string>
  {
    public static bool NumCheck = true ;

    private static string _NumRegex = @"^(.*?)([0-9]+).*?$" ;
    private static System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex( _NumRegex ) ;

    // xがyより小さいときはマイナス、大きいときはプラス
    // 同じときは0を返す
    public int Compare( string x, string y )
    {
      string a = x ;
      string b = y ;

      string a2 = a ;
      string b2 = b ;

      // 等しい
      if ( a == b ) {
        return 0 ;
      }

      // 数値部分
      int? ai = null ;
      int? bi = null ;

      if ( NumCheck ) {
        // 正規表現で切り出し
        System.Text.RegularExpressions.Match match = regex.Match( a ) ;

        if ( match.Success ) {
          a = match.Groups[ 1 ].Value ;
          ai = Convert.ToInt32( match.Groups[ 2 ].Value ) ;
        }

        match = regex.Match( b ) ;

        if ( match.Success ) {
          b = match.Groups[ 1 ].Value ;
          bi = Convert.ToInt32( match.Groups[ 2 ].Value ) ;
        }
      }

      // 文字の比較
      int t = string.Compare( a, b ) ;

      if ( NumCheck && t == 0 ) {
        if ( ai == null && bi != null ) {
          t = -1 ;
        }
        else if ( ai != null && bi == null ) {
          t = 1 ;
        }
        else if ( ai == null && bi == null ) {
          t = string.Compare( a2, b2 ) ;
        }
        else {
          t = (int)( ai - bi ) ;
          if ( t == 0 ) {
            t = string.Compare( a2, b2 ) ;
          }
        }
      }

      return t ;
    }
  }

  /// ================================================================================
  /// <summary>ファミリロードオプション</summary>
  /// ================================================================================
  public class LoadOptions : Revit.DB.IFamilyLoadOptions
  {
    public bool OnFamilyFound( bool familyInUse, out bool overwriteParameterValues )
    {
      overwriteParameterValues = true ;
      return true ;
    }

    public bool OnSharedFamilyFound( Revit.DB.Family sharedFamily, bool familyInUse, out Revit.DB.FamilySource source, out bool overwriteParameterValues )
    {
      source = Revit.DB.FamilySource.Family ;
      overwriteParameterValues = true ;
      return true ;
    }
  }
}
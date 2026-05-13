using System ;
using System.Collections.Generic ;
using System.Drawing ;
using System.Drawing.Drawing2D ;
using System.IO ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using System.Windows.Media.Imaging ;
using Autodesk ;
using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using Autodesk.Revit.DB.Events ;
using System.Diagnostics ;
using System.Runtime.Versioning ;


namespace MappingTable
{
  [Autodesk.Revit.Attributes.Transaction( Autodesk.Revit.Attributes.TransactionMode.Manual )]
  [Autodesk.Revit.Attributes.Regeneration( Autodesk.Revit.Attributes.RegenerationOption.Manual )]
  [Autodesk.Revit.Attributes.Journaling( Autodesk.Revit.Attributes.JournalingMode.NoCommandData )]
  public class RevitLNK : IExternalApplication
  {
    public Result OnShutdown( UIControlledApplication application )
    {
      return Result.Succeeded ;
    }

    public Result OnStartup( UIControlledApplication application )
    {
      const string TabName = "REXJ Standalone" ;
      const string rbnName = "Mapping Table" ;

      string assembly = this.GetType().Assembly.Location ;
      Commons.DLLFilePath = System.IO.Path.GetDirectoryName( assembly ) + "\\" ;
      RibbonPanel rbnPanel = null ;

      List<RibbonPanel> rbnList = new List<RibbonPanel>() ;
      try { rbnList = application.GetRibbonPanels( TabName ) ; } catch { }
      for ( int i = 0 ; i < rbnList.Count ; i++ ) {
        if ( rbnList[ i ].Name == rbnName ) {
          rbnPanel = rbnList[ i ] ;
          break ;
        }
      }

      if ( rbnPanel == null ) {
        try {
          rbnPanel = application.CreateRibbonPanel( TabName, rbnName ) ;
        }
        catch {
          application.CreateRibbonTab( TabName ) ;
          rbnPanel = application.CreateRibbonPanel( TabName, rbnName ) ;
        }
      }

      string[] btnName = new string[] {
        "Edit Mapping Table",
        "Edit Mapping Table (STB2.0)",
        "Base Family Path",
        "Edit Base Mapping Table",
        "Batch Add Parameters"
      } ;
      string[] clsName = new string[] { "Cmd_2", "Cmd_2a", "Cmd_3", "Cmd_4", "Cmd_5" } ;
      string[] tooltip = new string[] {
        "Edit the family mapping table (Excel)",
        "Edit the family mapping table for STB2.0 (Excel macro)",
        "Set base column family folder path",
        "Edit the base column mapping table (Excel)",
        "Batch add parameters to all mapped families"
      } ;
      string[] iconfile = new string[] { "Icon1.png", "Icon1.png", "Icon2.png", "Icon3.png", "Icon4.png" } ;

      string chm = Commons.HelpPath() ;
      ContextualHelp chmhelp = new ContextualHelp( ContextualHelpType.Url, chm ) ;

      SplitButtonData sbd = new SplitButtonData( Commons.SystemName, Commons.SystemName ) ;
      SplitButton splitBtn = rbnPanel.AddItem( sbd ) as SplitButton ;
      splitBtn.ToolTip = "Mapping Table Tools" ;
      splitBtn.SetContextualHelp( chmhelp ) ;

      string configDir = Path.GetDirectoryName( assembly ) + "\\" + Commons.Configuration ;
      string mainIconPath = configDir + "Icon1.png" ;
      if ( File.Exists( mainIconPath ) ) {
        splitBtn.LargeImage = new BitmapImage( new Uri( mainIconPath ) ) ;
      }

      for ( int i = 0 ; i < btnName.Length ; i++ ) {
        var pbd = new PushButtonData( "MT_" + clsName[ i ], btnName[ i ], assembly, "MappingTable." + clsName[ i ] ) ;
        pbd.ToolTip = tooltip[ i ] ;
        pbd.SetContextualHelp( chmhelp ) ;
        string iconPath = configDir + iconfile[ i ] ;
        if ( File.Exists( iconPath ) ) {
          pbd.LargeImage = new BitmapImage( new Uri( iconPath ) ) ;
        }
        splitBtn.AddPushButton( pbd ) ;
      }

      CopyAllFile() ;

      return Result.Succeeded ;
    }

    private static BitmapImage CreateIcon( string text, System.Drawing.Color bgColor, int size )
    {
      using ( var bmp = new Bitmap( size, size ) )
      using ( var g = Graphics.FromImage( bmp ) )
      {
        g.SmoothingMode = SmoothingMode.AntiAlias ;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit ;
        using ( var brush = new SolidBrush( bgColor ) )
        {
          g.FillRectangle( brush, 0, 0, size, size ) ;
        }
        float fontSize = size <= 16 ? 6f : 10f ;
        using ( var font = new System.Drawing.Font( "Segoe UI", fontSize, System.Drawing.FontStyle.Bold ) )
        using ( var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center } )
        {
          g.DrawString( text, font, System.Drawing.Brushes.White, new RectangleF( 0, 0, size, size ), sf ) ;
        }

        using ( var ms = new MemoryStream() )
        {
          bmp.Save( ms, System.Drawing.Imaging.ImageFormat.Png ) ;
          ms.Position = 0 ;
          var bmpImage = new BitmapImage() ;
          bmpImage.BeginInit() ;
          bmpImage.StreamSource = ms ;
          bmpImage.CacheOption = BitmapCacheOption.OnLoad ;
          bmpImage.EndInit() ;
          bmpImage.Freeze() ;
          return bmpImage ;
        }
      }
    }


    /// <summary>
    /// マッピングテーブルのExcelファイル更新チェック
    /// 最新のファイルがなく、古いバージョンがある場合にバックアップを取る。
    /// </summary>
    /// <param name="folderpath">\Documents\Autodesk REXJ\20XX</param>
    /// <param name="filename">ConvRFA(ConvBase)20XX.tbl</param>
    /// <param name="backup_filepath">[out]バックアップファイルパス</param>
    /// <returns></returns>
    private static bool CheckOldExcel( string folderpath, string filename, out string backup_filepath )
    {
      backup_filepath = "" ;

      bool isRFA = filename.Contains( "ConvRFA" ) ;
      string filetype = ( isRFA ? "ConvRFA" : "ConvBase" ) ;
      string recentFile = ( isRFA ? Commons.ConvRFA_xls : Commons.ConvBase_xls ) ;
      if ( File.Exists( folderpath + "\\" + recentFile ) ) {
        //最新があれば以前のファイルチェックは不要
        return false ;
      }


      int firstNo = ( isRFA ? Commons.ConvRFA_1stNo : Commons.ConvBase_1stNo ) ;
      string recentNo = ( isRFA ? Commons.ConvRFA_RecentNo : Commons.ConvBase_RecentNo ) ;

      int.TryParse( recentNo, out int n ) ;
      bool existOldExcel = false ;
      for ( int i = n - 1 ; n >= firstNo ; n-- ) {
        existOldExcel = File.Exists( folderpath + "\\" + filetype + Commons.RevitVersion + "_" + i.ToString() + ".xls" ) ;
        if ( existOldExcel ) break ;
      }

      //最新がなく、前のバージョンのファイルがあるとき
      if ( existOldExcel ) {
        //過去のtblのバックアップ
        string stCopyTo = Path.Combine( folderpath, filename ) ;

        if ( File.Exists( stCopyTo ) ) {
          int index = stCopyTo.IndexOf( filetype ) ;
          int bkupnum = 1 ;
          string bkupname = "" ;
          do {
            bkupname = stCopyTo.Insert( index, "bkup" + bkupnum.ToString( "00#" ) + "_" ) ;
            if ( ! File.Exists( bkupname ) ) {
              break ;
            }
            else {
              bkupnum++ ;
            }
          } while ( File.Exists( bkupname ) ) ;

          File.Copy( stCopyTo, bkupname, true ) ;

          backup_filepath = bkupname ;
        }

        //メッセージ表示
        return true ;
      }

      return false ;
    }

    /// <summary>
    /// ConvBase20XX.tbl のフォルダパスの移行
    /// </summary>
    /// <param name="oldfile">古いテーブルパス</param>
    /// <param name="newfile">新しいテーブルパス</param>
    private static void CopyBaseFamilyPath( string oldfile, string newfile )
    {
      //ファイルが存在しないなら何もしない
      if ( ! File.Exists( oldfile ) ) return ;
      if ( ! File.Exists( newfile ) ) return ;

      const string BASEPATH = "柱脚ファイルパス" ;

      string[] delimiter = new string[] { " : " } ; //文字を切り取る条件

      string[] olddata = File.ReadAllLines( oldfile, Encoding.GetEncoding( "Shift_JIS" ) ) ;
      int oldpathIndex = olddata.ToList().FindIndex( a => a.Trim().StartsWith( BASEPATH ) ) ;
      string oldpath1 = olddata.FirstOrDefault( a => a.Trim().StartsWith( BASEPATH ) ) ;
      string[] oldpath2 = oldpath1.Split( delimiter, StringSplitOptions.RemoveEmptyEntries ) ;
      string oldpath = ( oldpath2.Length >= 2 ? oldpath2[ 1 ] : "" ) ;

      //古いファイルにパスが設定されていないなら何もしない
      if ( ! Directory.Exists( oldpath ) ) return ;


      string[] newdata = File.ReadAllLines( newfile, Encoding.GetEncoding( "Shift_JIS" ) ) ;
      int newpathIndex = newdata.ToList().FindIndex( a => a.Trim().StartsWith( BASEPATH ) ) ;
      if ( newpathIndex > 0 ) {
        newdata[ newpathIndex ] = "柱脚ファイルパス : " + oldpath ;
        try {
          File.WriteAllLines( newfile, newdata, Encoding.GetEncoding( "Shift_JIS" ) ) ;
        }
        catch {
        }
      }
    }


    /// <summary>
    /// ロード時のファイルのコピー
    /// </summary>
    /// <param name="assembly"></param>
    private static void CopyAllFile()
    {
      string configpath = Commons.ConfigPath( "" ) ;
      string rexjpath = Commons.RexJPath( "", true ) ; //コピーは常にDocumentのみ。場所を変えるときはユーザーが手動でコピー

      //各ファイルの場所の設定
      if ( ! Directory.Exists( rexjpath ) ) {
        Directory.CreateDirectory( rexjpath ) ;
      }

      //エクセル更新メッセージの有無
      bool excelflg = false ; //true⇒メッセージを出す

      //ファイルのコピー
      foreach ( string stCopyFrom in Directory.GetFiles( configpath ) ) {
        //上書きする⇒true
        bool copyflg = false ;
        //ファイル名
        string filename = Path.GetFileName( stCopyFrom ) ;
        //コピー先
        string stCopyTo = Path.Combine( rexjpath, Path.GetFileName( stCopyFrom ) ) ;


        bool old_base_tbl = false ;
        string bkfile = "" ;

        switch ( filename ) {
          case Commons.ConvBase_tbl : //柱脚テーブルファイル
            if ( CheckOldExcel( rexjpath, Path.GetFileName( stCopyFrom ), out bkfile ) ) {
              //古いファイルのみ。
              copyflg = true ;
              old_base_tbl = true ;
            }
            else if ( ! File.Exists( stCopyTo ) ) {
              //無ければコピー
              copyflg = true ;
            }

            break ;

          case Commons.ConvRFA_tbl : //ファミリテーブルファイル⇒convRFAが無ければmydocuにコピー
            excelflg |= CheckOldExcel( rexjpath, Path.GetFileName( stCopyFrom ), out bkfile ) ;
            if ( ! File.Exists( stCopyTo ) ) {
              copyflg = true ;
            }

            break ;

          case Commons.ConvRFA_xls : //ファミリマッピング⇒ファイルが無ければmydocuにコピー
          case Commons.ConvBase_xls : //柱脚マッピング⇒ファイルが無ければmydocuにコピー
          case Commons.REXStructual : //共有パラメータファイル⇒ファイルが無ければmydocuにコピー
          case Commons.ConvRFA_STB2_xlsm : //STB2.0用マッピング⇒ファイルが無ければmydocuにコピー
            if ( ! File.Exists( stCopyTo ) ) {
              copyflg = true ;
            }

            break ;

          case Commons.REXStructual_org :
          case Commons.chmfile :
            //共有パラメータファイルのオリジナル
            //ヘルプ
            //⇒常にコピー
            copyflg = true ;
            break ;

          default :
            //その他アイコンなどは不要
            break ;
        }

        if ( copyflg ) {
          try {
            System.IO.File.Copy( stCopyFrom, stCopyTo, true ) ;

            if ( old_base_tbl ) {
              //古いConvBase.tblからフォルダパスを写す
              CopyBaseFamilyPath( bkfile, stCopyTo ) ;
            }
          }
          catch {
          }
        }
      }


      //エクセル編集
      if ( excelflg ) {
        DialogResult re = MessageBox.Show( "The mapping table Excel file has been updated.\r\n" + "Would you like to open the mapping table editor to migrate your current settings?\r\n" + "Since the mapping table Excel file has been updated, " + "please also update SS3 Link/STB Link/RC Section List/S Section List " + "if you are using them.\r\n\r\n" + "Note: Only users with family editing permissions should edit the mapping table.\r\n" + "Users without editing permissions should obtain the updated mapping table from authorized users.", Commons.SystemName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2 ) ;
        if ( re == DialogResult.Yes ) {
          if ( File.Exists( Commons.ConvRFA_xls ) ) {
            System.Diagnostics.Process.Start( Commons.ConvRFA_xls ) ;
          }
        }
      }
    }
  }

  [Autodesk.Revit.Attributes.Transaction( Autodesk.Revit.Attributes.TransactionMode.Manual )]
  [Autodesk.Revit.Attributes.Regeneration( Autodesk.Revit.Attributes.RegenerationOption.Manual )]
  [Autodesk.Revit.Attributes.Journaling( Autodesk.Revit.Attributes.JournalingMode.NoCommandData )]
  public class Cmd_2 : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      Commons.doc = commandData.Application.ActiveUIDocument.Document ;

      DialogResult result = MessageBox.Show( "Edit the mapping table.\r\nDo you have editing permissions for the mapping table?\r\n\r\nOnly users with editing permissions should edit the mapping table.", Commons.SystemName + " Edit Mapping Table", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2 ) ;


      if ( result == DialogResult.Yes ) {
        string mydocu = Commons.RexJPath( "" ) ;
        if ( ! File.Exists( mydocu + Commons.ConvRFA_xls ) ) {
          MessageBox.Show( "The mapping table Excel file was not found.\r\nPlease check that \"" + Commons.ConvRFA_xls + "\" exists at the following location:\r\n\r\n" + mydocu, Commons.SystemName + " Edit Mapping Table", MessageBoxButtons.OK, MessageBoxIcon.Warning ) ;
          return Result.Succeeded ;
        }
        else {
          string msg = "" ;
          if ( ! File.Exists( mydocu + Commons.ConvRFA_tbl ) ) msg += $"{Commons.ConvRFA_tbl}\r\n" ;
          if ( ! File.Exists( mydocu + Commons.REXStructual ) ) msg += $"{Commons.REXStructual}\r\n" ;
          if ( ! File.Exists( mydocu + Commons.REXStructual_org ) ) msg += $"{Commons.REXStructual_org}\r\n" ;

          if ( msg != "" ) {
            msg += "\r\nMapping table file location: " + mydocu ;
            MessageBox.Show( "The following files were not found:\r\n" + msg, Commons.SystemName + " Edit Mapping Table", MessageBoxButtons.OK, MessageBoxIcon.Warning ) ;
            return Result.Succeeded ;
          }

          var processStartInfo = new ProcessStartInfo {
            FileName = mydocu + Commons.ConvRFA_xls,
            UseShellExecute = true
          };
          
          Process.Start( processStartInfo ) ;
          
          //System.Diagnostics.Process.Start( mydocu + Commons.ConvRFA_xls ) ;
        }
      }

      return Result.Succeeded ;
    }
  }

  [Autodesk.Revit.Attributes.Transaction( Autodesk.Revit.Attributes.TransactionMode.Manual )]
  [Autodesk.Revit.Attributes.Regeneration( Autodesk.Revit.Attributes.RegenerationOption.Manual )]
  [Autodesk.Revit.Attributes.Journaling( Autodesk.Revit.Attributes.JournalingMode.NoCommandData )]
  public class Cmd_2a : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      Commons.doc = commandData.Application.ActiveUIDocument.Document ;

      DialogResult result = MessageBox.Show( "Edit the mapping table (STB2.0).\r\nDo you have editing permissions for the mapping table?\r\n\r\nOnly users with editing permissions should edit the mapping table.", Commons.SystemName + " Edit Mapping Table (STB2.0)", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2 ) ;


      if ( result == DialogResult.Yes ) {
        string mydocu = Commons.RexJPath( "" ) ;
        if ( ! File.Exists( mydocu + Commons.ConvRFA_STB2_xlsm ) ) {
          MessageBox.Show( "The mapping table Excel file was not found.\r\nPlease check that \"" + Commons.ConvRFA_STB2_xlsm + "\" exists at the following location:\r\n\r\n" + mydocu, Commons.SystemName + " Edit Mapping Table (STB2.0)", MessageBoxButtons.OK, MessageBoxIcon.Warning ) ;
          return Result.Succeeded ;
        }
        else {
          string msg = "" ;
          if ( ! File.Exists( mydocu + Commons.ConvRFA_tbl ) ) msg += $"{Commons.ConvRFA_tbl}\r\n" ;
          if ( ! File.Exists( mydocu + Commons.REXStructual ) ) msg += $"{Commons.REXStructual}\r\n" ;
          if ( ! File.Exists( mydocu + Commons.REXStructual_org ) ) msg += $"{Commons.REXStructual_org}\r\n" ;

          if ( msg != "" ) {
            msg += "\r\nMapping table file location: " + mydocu ;
            MessageBox.Show( "The following files were not found:\r\n" + msg, Commons.SystemName + " Edit Mapping Table (STB2.0)", MessageBoxButtons.OK, MessageBoxIcon.Warning ) ;
            return Result.Succeeded ;
          }

          var processStartInfo = new ProcessStartInfo {
            FileName =  mydocu + Commons.ConvRFA_STB2_xlsm,
            UseShellExecute = true
          };
          Process.Start( processStartInfo ) ;
          //System.Diagnostics.Process.Start( mydocu + Commons.ConvRFA_STB2_xlsm ) ;
        }
      }

      return Result.Succeeded ;
    }
  }

  [Autodesk.Revit.Attributes.Transaction( Autodesk.Revit.Attributes.TransactionMode.Manual )]
  [Autodesk.Revit.Attributes.Regeneration( Autodesk.Revit.Attributes.RegenerationOption.Manual )]
  [Autodesk.Revit.Attributes.Journaling( Autodesk.Revit.Attributes.JournalingMode.NoCommandData )]
  public class Cmd_3 : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      Commons.doc = commandData.Application.ActiveUIDocument.Document ;

      string mydocu = Commons.RexJPath( "" ) ;
      if ( ! File.Exists( mydocu + Commons.ConvBase_tbl ) ) {
        MessageBox.Show( "The base column mapping table was not found.\r\nPlease check that \"" + Commons.ConvBase_tbl + "\" exists at the following location:\r\n\r\n" + mydocu, Commons.SystemName + " Base Family Path", MessageBoxButtons.OK, MessageBoxIcon.Warning ) ;
        return Result.Succeeded ;
      }

      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
      
      string[] data = File.ReadAllLines( mydocu + Commons.ConvBase_tbl, Encoding.GetEncoding( "Shift_JIS" ) ) ;

      string[] delimiter = new string[] { " : " } ; //文字を切り取る条件

      int oldpathIndex = data.ToList().FindIndex( a => a.Trim().StartsWith( "柱脚ファイルパス" ) ) ;
      string oldpath1 = data.FirstOrDefault( a => a.Trim().StartsWith( "柱脚ファイルパス" ) ) ;
      string[] oldpath2 = oldpath1.Split( delimiter, StringSplitOptions.RemoveEmptyEntries ) ;
      string oldpath = ( oldpath2.Length >= 2 ? oldpath2[ 1 ] : "" ) ;

      List<string> table = data.Select( a => a.Split( delimiter, StringSplitOptions.RemoveEmptyEntries ) ).Where( a => a.Length > 2 ).Select( a => a[ 2 ] ).ToList() ;

      FolderBrowserDialog fbd = new FolderBrowserDialog { Description = "Select the folder where base column families are stored.\r\nCurrent base family path: " + oldpath, ShowNewFolderButton = false, SelectedPath = oldpath } ;

      bool endflg = false ;
      if ( fbd.ShowDialog() == DialogResult.OK ) {
        do {
          endflg = false ;
          for ( int i = 0 ; i < table.Count() ; i++ ) {
            if ( File.Exists( fbd.SelectedPath + table[ i ] ) ) {
              endflg = true ;
              break ;
            }
          }

          if ( endflg ) {
            if ( oldpathIndex > 0 ) {
              data[ oldpathIndex ] = "柱脚ファイルパス : " + fbd.SelectedPath ;
              try {
                File.WriteAllLines( mydocu + Commons.ConvBase_tbl, data, Encoding.GetEncoding( "Shift_JIS" ) ) ;
              }
              catch {
              }
            }

            MessageBox.Show( "Base family path has been set.\r\n\r\n" + fbd.SelectedPath, Commons.SystemName + " Base Family Path", MessageBoxButtons.OK, MessageBoxIcon.Information ) ;
            endflg = true ;
          }
          else {
            MessageBox.Show( "Base column families not found. Please verify the base family path is correct.\r\n\r\nSpecified path:\r\n" + fbd.SelectedPath, Commons.SystemName + " Base Family Path", MessageBoxButtons.OK, MessageBoxIcon.Warning ) ;

            fbd.Description = "Select the folder where base column families are stored.\r\nCurrent base family path: " + oldpath ;
            fbd.ShowNewFolderButton = false ;
            fbd.SelectedPath = oldpath ;
            if ( fbd.ShowDialog() == DialogResult.OK ) {
              endflg = false ;
            }
            else {
              endflg = true ;
            }
          }
        } while ( ! endflg ) ;
      }

      fbd.Dispose() ;

      return Result.Succeeded ;
    }
  }


  [Autodesk.Revit.Attributes.Transaction( Autodesk.Revit.Attributes.TransactionMode.Manual )]
  [Autodesk.Revit.Attributes.Regeneration( Autodesk.Revit.Attributes.RegenerationOption.Manual )]
  [Autodesk.Revit.Attributes.Journaling( Autodesk.Revit.Attributes.JournalingMode.NoCommandData )]
  public class Cmd_4 : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      Commons.doc = commandData.Application.ActiveUIDocument.Document ;

      DialogResult result = MessageBox.Show( "Edit the base column mapping table.\r\nDo you have editing permissions?\r\n\r\nOnly users with editing permissions should edit the base column mapping table.", Commons.SystemName + " Edit Base Mapping Table", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2 ) ;
      if ( result == DialogResult.Yes ) {
        string mydocu = Commons.RexJPath( "" ) ;
        if ( ! File.Exists( mydocu + Commons.ConvBase_xls ) ) {
          MessageBox.Show( "The base column mapping table Excel file was not found.\r\nPlease check that \"" + Commons.ConvBase_xls + "\" exists at the following location:\r\n\r\n" + mydocu, Commons.SystemName + " Edit Base Mapping Table", MessageBoxButtons.OK, MessageBoxIcon.Warning ) ;
        }
        else {
          string msg = "" ;
          if ( ! File.Exists( mydocu + Commons.ConvBase_tbl ) ) msg += $"{Commons.ConvBase_tbl}\r\n" ;

          if ( msg != "" ) {
            msg += "\r\nBase mapping table file location: " + mydocu ;
            MessageBox.Show( "The following files were not found:\r\n" + msg, Commons.SystemName + " Edit Base Mapping Table", MessageBoxButtons.OK, MessageBoxIcon.Warning ) ;
            return Result.Succeeded ;
          }

          var processStartInfo = new ProcessStartInfo {
            FileName =  mydocu + Commons.ConvBase_xls,
            UseShellExecute = true
          };
          Process.Start( processStartInfo ) ;
          // System.Diagnostics.Process.Start( mydocu + Commons.ConvBase_xls ) ;
        }
      }

      return Result.Succeeded ;
    }
  }


  [Autodesk.Revit.Attributes.Transaction( Autodesk.Revit.Attributes.TransactionMode.Manual )]
  [Autodesk.Revit.Attributes.Regeneration( Autodesk.Revit.Attributes.RegenerationOption.Manual )]
  [Autodesk.Revit.Attributes.Journaling( Autodesk.Revit.Attributes.JournalingMode.NoCommandData )]
  public class Cmd_5 : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      Commons.doc = commandData.Application.ActiveUIDocument.Document ;

      if ( Commons.doc.IsFamilyDocument == true ) {
        MessageBox.Show( "This command cannot be used in family editing mode.", Commons.SystemName + " Batch Add Parameters", MessageBoxButtons.OK, MessageBoxIcon.Warning ) ;
        return Result.Succeeded ;
      }


      DialogResult result = MessageBox.Show( "Add parameters to families specified in the mapping table.\r\nDo you have family editing permissions?\r\n\r\nOnly users with editing permissions should add parameters.", Commons.SystemName + " Batch Add Parameters", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2 ) ;

      if ( result == DialogResult.Yes ) {
        string mydocu = Commons.RexJPath( "" ) ;
        if ( ! File.Exists( mydocu + Commons.ConvRFA_tbl ) ) {
          MessageBox.Show( "The mapping table was not found.\r\nPlease check that \"" + Commons.ConvRFA_tbl + "\" exists at the following location:\r\n\r\n" + mydocu, Commons.SystemName + " Batch Add Parameters", MessageBoxButtons.OK, MessageBoxIcon.Warning ) ;
          return Result.Succeeded ;
        }


        Commons.SetSharedParametersFile() ;
        LogData.Data = new List<LogData.Log>() ;


        //プロジェクトに読み込まれているファミリ名を取得
        LoadFamily.LoadFfamily_fromProject() ;

        //マッピングテーブル内のファミリ名・パラメータを取得                
        if ( ! SetFamily.LoadTable() ) {
          MessageBox.Show( "An older version of the mapping table is in use. Please update to the latest version.", Commons.SystemName + " Batch Add Parameters", MessageBoxButtons.OK, MessageBoxIcon.Warning ) ;
          return Result.Succeeded ;
        }

        var f = new STBParaBuild() ;
        f.ShowDialog() ;
      }

      return Result.Succeeded ;
    }
  }

  [Autodesk.Revit.Attributes.Transaction( Autodesk.Revit.Attributes.TransactionMode.Manual )]
  [Autodesk.Revit.Attributes.Regeneration( Autodesk.Revit.Attributes.RegenerationOption.Manual )]
  [Autodesk.Revit.Attributes.Journaling( Autodesk.Revit.Attributes.JournalingMode.NoCommandData )]
  public class Cmd_6c : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      Commons.doc = commandData.Application.ActiveUIDocument.Document ;

      string buzai = "Structural Column" ;
      string title = Commons.SystemName + " " + buzai + " Add Parameters" ;
      DialogResult result = DialogResult.Yes ;

      if ( Commons.doc.IsFamilyDocument == false ) {
        result = MessageBox.Show( "Add parameters to " + buzai + " families.\r\nDo you have family editing permissions?\r\n\r\nOnly users with editing permissions should add parameters.", title, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2 ) ;
      }
      else {
#if REVIT2022 || REVIT2023
        var value = Commons.doc.OwnerFamily.FamilyCategoryId.IntegerValue ;
#else
                var value = Commons.doc.OwnerFamily.FamilyCategoryId.Value ;
#endif

        if ( value != (long)BuiltInCategory.OST_StructuralColumns ) {
          MessageBox.Show( "This command cannot be used for " + Commons.doc.OwnerFamily.FamilyCategory.Name + " family editing.", title, MessageBoxButtons.OK, MessageBoxIcon.Warning ) ;
          return Result.Succeeded ;
        }
      }

      if ( result == DialogResult.Yes ) {
        string mydocu = Commons.RexJPath( "" ) ;
        if ( ! File.Exists( mydocu + Commons.ConvRFA_tbl ) ) {
          MessageBox.Show( "The mapping table was not found.\r\nPlease check that \"" + Commons.ConvRFA_tbl + "\" exists at:\r\n\r\n" + mydocu, title, MessageBoxButtons.OK, MessageBoxIcon.Warning ) ;
          return Result.Succeeded ;
        }

        Commons.SetSharedParametersFile() ;
        LogData.Data = new List<LogData.Log>() ;

        if ( ! SetFamily.LoadTable() ) {
          MessageBox.Show( "An older version of the mapping table is in use. Please update.", title, MessageBoxButtons.OK, MessageBoxIcon.Warning ) ;
          return Result.Succeeded ;
        }

        AddParameterForm f = new AddParameterForm( BuiltInCategory.OST_StructuralColumns ) ;
        f.ShowDialog() ;
      }

      return Result.Succeeded ;
    }
  }

  [Autodesk.Revit.Attributes.Transaction( Autodesk.Revit.Attributes.TransactionMode.Manual )]
  [Autodesk.Revit.Attributes.Regeneration( Autodesk.Revit.Attributes.RegenerationOption.Manual )]
  [Autodesk.Revit.Attributes.Journaling( Autodesk.Revit.Attributes.JournalingMode.NoCommandData )]
  public class Cmd_6g : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      Commons.doc = commandData.Application.ActiveUIDocument.Document ;

      string buzai = "Structural Framing" ;
      string title = Commons.SystemName + " " + buzai + " Add Parameters" ;
      DialogResult result = DialogResult.Yes ;

      if ( Commons.doc.IsFamilyDocument == false ) {
        result = MessageBox.Show( "Add parameters to " + buzai + " families.\r\nDo you have family editing permissions?\r\n\r\nOnly users with editing permissions should add parameters.", title, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2 ) ;
      }
      else {
#if REVIT2022 || REVIT2023
        var value = Commons.doc.OwnerFamily.FamilyCategoryId.IntegerValue ;
#else
                var value = Commons.doc.OwnerFamily.FamilyCategoryId.Value ;
#endif
        if ( value != (long)BuiltInCategory.OST_StructuralFraming ) {
          MessageBox.Show( "This command cannot be used for " + Commons.doc.OwnerFamily.FamilyCategory.Name + " family editing.", title, MessageBoxButtons.OK, MessageBoxIcon.Warning ) ;
          return Result.Succeeded ;
        }
      }

      if ( result == DialogResult.Yes ) {
        string mydocu = Commons.RexJPath( "" ) ;
        if ( ! File.Exists( mydocu + Commons.ConvRFA_tbl ) ) {
          MessageBox.Show( "The mapping table was not found.\r\nPlease check that \"" + Commons.ConvRFA_tbl + "\" exists at:\r\n\r\n" + mydocu, title, MessageBoxButtons.OK, MessageBoxIcon.Warning ) ;
          return Result.Succeeded ;
        }

        Commons.SetSharedParametersFile() ;
        LogData.Data = new List<LogData.Log>() ;

        if ( ! SetFamily.LoadTable() ) {
          MessageBox.Show( "An older version of the mapping table is in use. Please update.", title, MessageBoxButtons.OK, MessageBoxIcon.Warning ) ;
          return Result.Succeeded ;
        }

        AddParameterForm f = new AddParameterForm( BuiltInCategory.OST_StructuralFraming ) ;
        f.ShowDialog() ;
      }

      return Result.Succeeded ;
    }
  }


  [Autodesk.Revit.Attributes.Transaction( Autodesk.Revit.Attributes.TransactionMode.Manual )]
  [Autodesk.Revit.Attributes.Regeneration( Autodesk.Revit.Attributes.RegenerationOption.Manual )]
  [Autodesk.Revit.Attributes.Journaling( Autodesk.Revit.Attributes.JournalingMode.NoCommandData )]
  public class Cmd_6s : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      Commons.doc = commandData.Application.ActiveUIDocument.Document ;

      string buzai = "Structural Floor" ;
      string title = Commons.SystemName + " " + buzai + " Add Parameters" ;

      if ( Commons.doc.IsFamilyDocument == true ) {
        MessageBox.Show( "This command cannot be used for " + Commons.doc.OwnerFamily.FamilyCategory.Name + " family editing.", title, MessageBoxButtons.OK, MessageBoxIcon.Warning ) ;
        return Result.Succeeded ;
      }

      DialogResult result = MessageBox.Show( "Add parameters to " + buzai + " families.\r\nDo you have family editing permissions?\r\n\r\nOnly users with editing permissions should add parameters.", title, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2 ) ;
      if ( result == DialogResult.Yes ) {
        string mydocu = Commons.RexJPath( "" ) ;
        if ( ! File.Exists( mydocu + Commons.ConvRFA_tbl ) ) {
          MessageBox.Show( "The mapping table was not found.\r\nPlease check that \"" + Commons.ConvRFA_tbl + "\" exists at:\r\n\r\n" + mydocu, title, MessageBoxButtons.OK, MessageBoxIcon.Warning ) ;
          return Result.Succeeded ;
        }

        Commons.SetSharedParametersFile() ;
        LogData.Data = new List<LogData.Log>() ;

        if ( ! SetFamily.LoadTable() ) {
          MessageBox.Show( "An older version of the mapping table is in use. Please update.", title, MessageBoxButtons.OK, MessageBoxIcon.Warning ) ;
          return Result.Succeeded ;
        }

        if ( ! File.Exists( Commons.RexJPath( Commons.REXStructual ) ) ) {
          string mes = "Shared parameters file not found." ;
          MessageBox.Show( mes + "\r\n\r\n" + Commons.RexJPath( Commons.REXStructual ), title, MessageBoxButtons.OK, MessageBoxIcon.Warning ) ;
          return Result.Succeeded ;
        }

        string title_ver = title + " " + Commons.GetVersion() ;

        string tuika = "Add parameters to structural floor families?" ;
        if ( MessageBox.Show( tuika, title_ver, MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes ) {
          try {
            FilteredElementCollector collector = new FilteredElementCollector( Commons.doc ) ;
            ElementFilter filter = new ElementCategoryFilter( BuiltInCategory.OST_Floors ) ;
            IList<Element> elms = collector.WherePasses( filter ).WhereElementIsElementType().ToElements() ;

            ProgressBarForm pform = new ProgressBarForm() ;
            Stopwatch stopw = new Stopwatch() ;
            stopw.Start() ;
            bool flg = false ;
            AddParameterForm.Pform_Show( pform, ref flg, title ) ;
            pform.Text = Commons.SystemName + " Adding Parameters" ;

            foreach ( Element el in elms ) {
              if ( el is FloorType symbol && symbol.IsFoundationSlab == false ) {
                Commons.ProgressBar_Show( pform, "Adding structural floor parameters" ) ;
                Commons.GaugePercent( symbol.FamilyName, (int)( (double)1 / (double)1 * 100 ) ) ;

                ParaSet.SetPara_Slab( "床", el, SetFamily.Slab ) ;
                break ;
              }
            }

            if ( this != null ) {
              do {
                Application.DoEvents() ;
              } while ( stopw.ElapsedMilliseconds <= 1000 ) ;

              ;
              stopw.Stop() ;
              Commons.GaugeClose() ;
              pform.Close() ;
            }

            string mes = "Parameters have been added to structural floor families." ;
            MessageBox.Show( mes, title_ver, MessageBoxButtons.OK, MessageBoxIcon.Information ) ;
          }
          catch {
            string mes = "Failed to add parameters to structural floor families." ;
            MessageBox.Show( mes, title_ver, MessageBoxButtons.OK, MessageBoxIcon.Warning ) ;
          }

          if ( LogData.Data.Count() != 0 ) {
            LogForm lf = new LogForm { Text = Commons.SystemName + " Floor Parameter Log " + Commons.GetVersion() } ;
            lf.ShowDialog() ;
          }
        }
      }

      return Result.Succeeded ;
    }
  }


  [Autodesk.Revit.Attributes.Transaction( Autodesk.Revit.Attributes.TransactionMode.Manual )]
  [Autodesk.Revit.Attributes.Regeneration( Autodesk.Revit.Attributes.RegenerationOption.Manual )]
  [Autodesk.Revit.Attributes.Journaling( Autodesk.Revit.Attributes.JournalingMode.NoCommandData )]
  public class Cmd_6w : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      Commons.doc = commandData.Application.ActiveUIDocument.Document ;

      string buzai = "Structural Wall" ;
      string title = Commons.SystemName + " " + buzai + " Add Parameters" ;

      if ( Commons.doc.IsFamilyDocument == true ) {
        MessageBox.Show( "This command cannot be used for " + Commons.doc.OwnerFamily.FamilyCategory.Name + " family editing.", title, MessageBoxButtons.OK, MessageBoxIcon.Warning ) ;
        return Result.Succeeded ;
      }

      DialogResult result = MessageBox.Show( "Add parameters to " + buzai + " families.\r\nDo you have family editing permissions?\r\n\r\nOnly users with editing permissions should add parameters.", title, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2 ) ;
      if ( result == DialogResult.Yes ) {
        string mydocu = Commons.RexJPath( "" ) ;
        if ( ! File.Exists( mydocu + Commons.ConvRFA_tbl ) ) {
          MessageBox.Show( "The mapping table was not found.\r\nPlease check that \"" + Commons.ConvRFA_tbl + "\" exists at:\r\n\r\n" + mydocu, title, MessageBoxButtons.OK, MessageBoxIcon.Warning ) ;
          return Result.Succeeded ;
        }

        Commons.SetSharedParametersFile() ;
        LogData.Data = new List<LogData.Log>() ;

        if ( ! SetFamily.LoadTable() ) {
          MessageBox.Show( "An older version of the mapping table is in use. Please update.", title, MessageBoxButtons.OK, MessageBoxIcon.Warning ) ;
          return Result.Succeeded ;
        }

        if ( ! File.Exists( Commons.RexJPath( Commons.REXStructual ) ) ) {
          string mes = "Shared parameters file not found." ;
          MessageBox.Show( mes + "\r\n\r\n" + Commons.RexJPath( Commons.REXStructual ), title, MessageBoxButtons.OK, MessageBoxIcon.Warning ) ;
          return Result.Succeeded ;
        }

        string title_ver = title + " " + Commons.GetVersion() ;

        string tuika = "Add parameters to structural wall families?" ;
        if ( MessageBox.Show( tuika, title_ver, MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes ) {
          try {
            FilteredElementCollector collector = new FilteredElementCollector( Commons.doc ) ;
            ElementFilter filter = new ElementCategoryFilter( BuiltInCategory.OST_Walls ) ;
            IList<Element> elms = collector.WherePasses( filter ).WhereElementIsElementType().ToElements() ;

            ProgressBarForm pform = new ProgressBarForm() ;
            Stopwatch stopw = new Stopwatch() ;
            stopw.Start() ;
            bool flg = false ;
            AddParameterForm.Pform_Show( pform, ref flg, title ) ;
            pform.Text = Commons.SystemName + " Adding Parameters" ;

            foreach ( Element el in elms ) {
              if ( el is WallType symbol && symbol.Kind == WallKind.Basic ) {
                Commons.ProgressBar_Show( pform, "Adding structural wall parameters" ) ;
                Commons.GaugePercent( symbol.FamilyName, (int)( (double)1 / (double)1 * 100 ) ) ;

                ParaSet.SetPara_Wall( "壁", el, SetFamily.Wall ) ;
                break ;
              }
            }

            if ( this != null ) {
              do {
                Application.DoEvents() ;
              } while ( stopw.ElapsedMilliseconds <= 1000 ) ;

              ;
              stopw.Stop() ;
              Commons.GaugeClose() ;
              pform.Close() ;
            }

            string mes = "Parameters have been added to structural wall families." ;
            MessageBox.Show( mes, title_ver, MessageBoxButtons.OK, MessageBoxIcon.Information ) ;
          }
          catch {
            string mes = "Failed to add parameters to structural wall families." ;
            MessageBox.Show( mes, title_ver, MessageBoxButtons.OK, MessageBoxIcon.Warning ) ;
          }

          if ( LogData.Data.Count() != 0 ) {
            LogForm lf = new LogForm { Text = Commons.SystemName + " Wall Parameter Log " + Commons.GetVersion() } ;
            lf.ShowDialog() ;
          }
        }
      }

      return Result.Succeeded ;
    }
  }


  [Autodesk.Revit.Attributes.Transaction( Autodesk.Revit.Attributes.TransactionMode.Manual )]
  [Autodesk.Revit.Attributes.Regeneration( Autodesk.Revit.Attributes.RegenerationOption.Manual )]
  [Autodesk.Revit.Attributes.Journaling( Autodesk.Revit.Attributes.JournalingMode.NoCommandData )]
  public class Cmd_6f : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      Commons.doc = commandData.Application.ActiveUIDocument.Document ;

      string buzai = "Structural Foundation" ;
      string title = Commons.SystemName + " " + buzai + " Add Parameters" ;
      DialogResult result = DialogResult.Yes ;

      if ( Commons.doc.IsFamilyDocument == false ) {
        result = MessageBox.Show( "Add parameters to " + buzai + " families.\r\nDo you have family editing permissions?\r\n\r\nOnly users with editing permissions should add parameters.", title, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2 ) ;
      }
      else {
#if REVIT2022 || REVIT2023
        var value = Commons.doc.OwnerFamily.FamilyCategoryId.IntegerValue ;
#else
                var value = Commons.doc.OwnerFamily.FamilyCategoryId.Value ;
#endif
        if ( value != (long)BuiltInCategory.OST_StructuralFoundation ) {
          MessageBox.Show( "This command cannot be used for " + Commons.doc.OwnerFamily.FamilyCategory.Name + " family editing.", title, MessageBoxButtons.OK, MessageBoxIcon.Warning ) ;
          return Result.Succeeded ;
        }
      }

      if ( result == DialogResult.Yes ) {
        string mydocu = Commons.RexJPath( "" ) ;
        if ( ! File.Exists( mydocu + Commons.ConvRFA_tbl ) ) {
          MessageBox.Show( "The mapping table was not found.\r\nPlease check that \"" + Commons.ConvRFA_tbl + "\" exists at:\r\n\r\n" + mydocu, title, MessageBoxButtons.OK, MessageBoxIcon.Warning ) ;
          return Result.Succeeded ;
        }

        Commons.SetSharedParametersFile() ;
        LogData.Data = new List<LogData.Log>() ;

        if ( ! SetFamily.LoadTable() ) {
          MessageBox.Show( "An older version of the mapping table is in use. Please update.", title, MessageBoxButtons.OK, MessageBoxIcon.Warning ) ;
          return Result.Succeeded ;
        }

        AddParameterForm f = new AddParameterForm( BuiltInCategory.OST_StructuralFoundation ) ;
        f.ShowDialog() ;
      }

      return Result.Succeeded ;
    }
  }
}
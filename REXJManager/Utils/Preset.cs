using System.Collections.Generic ;
using System ;
using System.IO ;
using System.Linq ;
using System.Text ;
using Autodesk.Revit.UI ;
using UIFramework ;
using IOException = Autodesk.Revit.Exceptions.IOException ;

namespace REXJManager
{
  public static class Preset
  {
    public static string Name { get ; private set ; }

    public static bool IsLt = true;
    
    /// <summary>
    /// プリセット名一覧を取得
    /// </summary>
    /// <returns></returns>
    public static List<string> Names()
    {
      try {
        var textFiles = Directory.GetFiles( PresetPath(), "*.txt" ) ;
        var names = textFiles.Select( Path.GetFileNameWithoutExtension ).ToList() ;
        return names ;
      }
      catch ( Exception e ) {
        Console.WriteLine( e ) ;
        return new List<string>() ;
      }
    }


    /// <summary>
    /// 設定ファイルから選択されたプリセットの名前を読み込む。
    /// </summary>
    public static void LoadConf()
    {
      if ( ! string.IsNullOrEmpty( Name ) ) return ;
      var filePath = $"{PresetPath()}\\{Resource.FILENAME_CONF}" ;
      if ( File.Exists( filePath ) ) {
        Name = File.ReadAllText( filePath ) ;
      }
      else {
        Name = Resource.FILENAME_EQUIPMENT ;
        SaveConf() ;
      }

      try {
        var textFiles = Directory.GetFiles( PresetPath(), "*.txt" ).ToList() ;
        var itemPath = $"{PresetPath()}\\{Name}.txt" ;
        if( !textFiles.Contains( itemPath )) Name = Resource.FILENAME_DESIGN ;
      }
      catch ( Exception e ) {
        Console.WriteLine( e ) ;
        TaskDialog.Show( $"{Resource.TAB_NAME} {Resource.TXT_CMD_SETTING}", $"{Resource.ERR_LOAD_PRESET_FAILURE}" ) ;
      }
      
    }

    /// <summary>
    /// 設定ファイルに選択されたプリセットの名前を保存する。
    /// </summary>
    public static void SaveConf()
    {
      if ( string.IsNullOrEmpty( Name ) ) return ;
      var filePath = $"{PresetPath()}\\{Resource.FILENAME_CONF}" ;
      try {
        File.WriteAllText( filePath, Name ) ;
      }
      catch ( Exception e ) {
        Console.WriteLine( e ) ;
      }
    }
    
    
    /// <summary>
    /// プリセットファイル名からパスの文字列を取得する
    /// </summary>
    /// <param name="name">拡張子なしファイル名</param>
    /// <returns></returns>
    public static List<string> LoadPathListFromPresetFile( string name )
    {
      var lines = new List<string>() ;
      try {
        var filePath = $"{PresetPath()}\\{name}.txt" ;
        using var reader = new StreamReader( filePath ) ;
        while ( reader.ReadLine() is { } line ) {
          lines.Add( line ) ;
        }

        NormalizeLegacyRibbonTabPrefixes( lines ) ;
      }
      catch ( Exception e ) {
        Console.WriteLine( e ) ;
        return lines ;
      }

      Name = name ;
      return lines ;
    }

    /// <summary>
    /// プリセットファイルが読み取り専用かどうかを返す
    /// </summary>
    /// <param name="name">拡張子なしファイル名</param>
    /// <returns></returns>
    public static bool IsReadOnly( string name )
    {
      var filePath = $"{PresetPath()}\\{name}.txt" ;
      try {
        var attr = File.GetAttributes( filePath ) ;
        return ( attr & FileAttributes.ReadOnly ) == FileAttributes.ReadOnly ;
      }
      catch ( Exception e ) {
        Console.WriteLine( e ) ;
        return false ;
      }
    }
    
    /// <summary>
    /// プリセット・ファイルのパスを返す
    /// </summary>
    /// <returns></returns>
    private static string PresetPath()
    {
      var dllFolderPath = Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location ) ;
      return $"{dllFolderPath}\\Presets" ;
    }

    /// <summary>
    /// 新規プリセットファイルを作成する
    /// </summary>
    /// <param name="name">拡張子なしファイル名</param>
    /// <returns></returns>
    public static string CreatePreset( string name )
    {
      var filePath = $"{PresetPath()}\\{name}.txt" ;
      var textFiles = Directory.GetFiles( PresetPath(), "*.txt" ).ToList() ;

      while ( textFiles.Contains( filePath ) ) {
        name = $"{name}_" ;
        filePath = $"{PresetPath()}\\{name}.txt" ;
      }
      
      var pathStrings = RevitRibbonControl.RibbonControl.Extract<string>() ;

      try {
        File.WriteAllText( filePath, pathStrings ) ;
      }
      catch ( Exception e ) {
        Console.WriteLine( e ) ;
        return null ;
      }

      Name = name ;
      return name ;
    }

    /// <summary>
    /// MyTreeNodeからプリセット用の文字列を生成する
    /// </summary>
    /// <param name="treeNode"></param>
    /// <returns></returns>
    public static string ToPresetString( this MyTreeNode treeNode )
    {
      var sb = new StringBuilder() ;
      SetPathToSb( treeNode ) ;

      return sb.ToString() ;

      void SetPathToSb( MyTreeNode node )
      {
        if (node.IsChecked) sb.Append( $"{node.Path}\r\n" ) ;
        if ( node.Children.Count == 0 ) return ;
        foreach ( var item in node.Children ) {
          SetPathToSb( item ) ;
        }
      }
    }

    /// <summary>
    /// プリセットファイルを保存する
    /// </summary>
    /// <param name="name">拡張子なしファイル名</param>
    /// <param name="body">ファイルの中身の文字列</param>
    public static void SavePreset( string name, string body )
    {
      if ( name == string.Empty ) return ;
      var filePath = $"{PresetPath()}\\{name}.txt" ;
      try {
        File.WriteAllText( filePath, body ) ;
      }
      catch ( Exception e ) {
        Console.WriteLine( e ) ;
      }
    }

    /// <summary>
    /// プリセットの削除
    /// </summary>
    /// <param name="name"></param>
    public static void Delete( string name )
    {
      var filePath = $"{PresetPath()}\\{name}.txt" ;
      try {
        File.Delete( filePath );
      }
      catch ( Exception e ) {
        Console.WriteLine( e ) ;
      }
    }
    
    /// <summary>
    /// プリセットの複製
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string Duplicate( string name )
    {
      var destPath = $"{PresetPath()}\\{name}.txt" ;
      var sourcePath = destPath ;
      var textFiles = Directory.GetFiles( PresetPath(), "*.txt" ).ToList() ;
      while ( textFiles.Contains( destPath ) ) {
        name = $"{name}_" ;
        destPath = $"{PresetPath()}\\{name}.txt" ;
      }
     
      try {
        File.Copy( sourcePath, destPath );
        File.SetAttributes( destPath, FileAttributes.Normal );
        Name = name ;
        return name ;
      }
      catch ( Exception e ) {
        Console.WriteLine( e ) ;
        return string.Empty ;
      }
    }

    
    /// <summary>
    /// 初期プリセットを読み取り専用にする。
    /// </summary>
    public static void SetSystemPresetsToReadOnly()
    {
      var presetPath = PresetPath() ;
      SetSystemPresetToReadOnly( Resource.FILENAME_DESIGN );
      SetSystemPresetToReadOnly( Resource.FILENAME_STRUCTURE );
      SetSystemPresetToReadOnly( Resource.FILENAME_EQUIPMENT );
      return ;

      void SetSystemPresetToReadOnly( string name )
      {
        var filePath = $"{presetPath}\\{name}.txt" ;
        try {
          File.SetAttributes( filePath, File.GetAttributes( filePath )| FileAttributes.ReadOnly );
        }
        catch ( IOException e ) {
          Console.WriteLine( e ) ;
        }
      }
      
    }

    private static readonly HashSet<string> StructuralPanelNames = new( StringComparer.OrdinalIgnoreCase )
    {
      "Structure Tag Filter", "RC Section List", "S Section List",
      "ST-Bridge Link", "ST-Bridge Link (New)", "Mapping Table", "Join Tools"
    } ;

    private static readonly HashSet<string> MepPanelNames = new( StringComparer.OrdinalIgnoreCase )
    {
      "Pipe Size Correction", "Pipe Sizing", "Duct Monitor", "Duct Resistance", "Ductulator",
      "Connection Tool", "MEP Edit", "Edit", "Arrow Mark Tool", "Arrow Tool",
      "Pipe/Duct Quantity Pickup", "Pipe/Duct Quantity",
      "Manhole Tool", "Flange",
      "Dimension", "Array && Tag", "Section Box", "Excel",
      "Filter", "Print", "Value Copy", "Views"
    } ;

    /// <summary>
    /// Regenerate the built-in system presets from the live ribbon so paths
    /// always match the actual button AutomationNames at runtime.
    /// Architecture = all non-structural, non-MEP panels.
    /// Structure    = Architecture panels + structural panels.
    /// MEP          = only MEP panels (empty until MEP tools are converted).
    /// </summary>
    public static void GenerateSystemPresets()
    {
      try {
        var allPaths = RevitRibbonControl.RibbonControl.Extract<string>() ;
        if ( string.IsNullOrEmpty( allPaths ) ) return ;

        var ribbon = Autodesk.Windows.ComponentManager.Ribbon ;
        var debugSb = new StringBuilder() ;
        debugSb.AppendLine( $"--- Extract<string>() result ({allPaths.Length} chars) ---" ) ;
        debugSb.AppendLine( allPaths ) ;
        debugSb.AppendLine( "--- Ribbon tab scan ---" ) ;
        foreach ( var tab in ribbon.Tabs ) {
          debugSb.AppendLine( $"Tab: Id=[{tab.Id}] Name=[{tab.Name}] AutoName=[{tab.AutomationName}] Panels={tab.Panels.Count}" ) ;
          if ( tab.Id == Resource.TAB_NAME || ( tab.Name != null && tab.Name.StartsWith( Resource.TAB_NAME, StringComparison.Ordinal ) ) ) {
            foreach ( var panel in tab.Panels ) {
              debugSb.AppendLine( $"  Panel: AutoName=[{panel.Source.AutomationName}] Visible={panel.IsVisible} Items={panel.Source.Items.Count}" ) ;
            }
          }
        }
        var debugPath = $"{PresetPath()}\\__debug_ribbon_paths.txt" ;
        File.WriteAllText( debugPath, debugSb.ToString() ) ;

        var lines = allPaths.Split( new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries ) ;
        var tabRoot = Resource.TAB_NAME ;

        var archLines = new List<string>() ;
        var structLines = new List<string>() ;
        var mepLines = new List<string>() ;

        string currentPanel = null ;
        var currentDiscipline = "arch" ;

        foreach ( var line in lines ) {
          if ( line == tabRoot ) {
            archLines.Add( line ) ;
            structLines.Add( line ) ;
            mepLines.Add( line ) ;
            continue ;
          }

          var afterRoot = line.StartsWith( tabRoot + "/", StringComparison.Ordinal )
            ? line.Substring( tabRoot.Length + 1 ) : null ;

          if ( afterRoot != null && ! afterRoot.Contains( "/" ) ) {
            currentPanel = afterRoot ;
            if ( StructuralPanelNames.Contains( currentPanel ) )
              currentDiscipline = "struct" ;
            else if ( MepPanelNames.Contains( currentPanel ) )
              currentDiscipline = "mep" ;
            else
              currentDiscipline = "arch" ;
          }

          switch ( currentDiscipline ) {
            case "arch" :
              archLines.Add( line ) ;
              structLines.Add( line ) ;
              break ;
            case "struct" :
              structLines.Add( line ) ;
              break ;
            case "mep" :
              mepLines.Add( line ) ;
              break ;
          }
        }

        var presetPath = PresetPath() ;
        WriteSystemPreset( presetPath, Resource.FILENAME_DESIGN, archLines ) ;
        WriteSystemPreset( presetPath, Resource.FILENAME_STRUCTURE, structLines ) ;
        WriteSystemPreset( presetPath, Resource.FILENAME_EQUIPMENT, mepLines ) ;
      }
      catch ( Exception e ) {
        Console.WriteLine( e ) ;
      }
    }

    private static void WriteSystemPreset( string presetDir, string name, List<string> lines )
    {
      var filePath = $"{presetDir}\\{name}.txt" ;
      try {
        if ( File.Exists( filePath ) ) {
          var attr = File.GetAttributes( filePath ) ;
          if ( ( attr & FileAttributes.ReadOnly ) == FileAttributes.ReadOnly )
            File.SetAttributes( filePath, attr & ~FileAttributes.ReadOnly ) ;
        }
        File.WriteAllText( filePath, string.Join( "\r\n", lines ) + "\r\n" ) ;
      }
      catch ( Exception e ) {
        Console.WriteLine( e ) ;
      }
    }

    /// <summary>
    /// LTのときに隠すコマンドかどうかパスから判断する
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool ShouldShow( this string path )
    {
      if ( ! IsLt ) return true ;

      const string LegacyRoot = "REXJ" ;
      var canonical = path ;
      if ( canonical == LegacyRoot || canonical.StartsWith( LegacyRoot + "/", StringComparison.Ordinal ) )
        canonical = Resource.TAB_NAME + canonical.Substring( LegacyRoot.Length ) ;

      return GetRibbonLtShowWhitelist().Contains( canonical ) ;
    }

    private static List<string> _ltShowPathsCached ;
    private static string _ltCachedRoot ;

    /// <remarks>Paths use the ribbon tab id from <c>Resource.TAB_NAME</c>.</remarks>
    private static List<string> GetRibbonLtShowWhitelist()
    {
      var root = Resource.TAB_NAME ;
      if ( _ltShowPathsCached != null && string.Equals( _ltCachedRoot, root, StringComparison.Ordinal ) )
        return _ltShowPathsCached ;

      _ltCachedRoot = root ;
      _ltShowPathsCached = new List<string>
      {
        root,
        $"{root}/建築",
        $"{root}/建築/法規チェック",
        $"{root}/建築/法規チェック/建築/部屋をエリアに変換",
        $"{root}/建築/法規チェック/建築/根拠式",
        $"{root}/建築/法規チェック/建築/法定面積",
        $"{root}/建築/法規チェック/建築/<RibbonSeparator>",
        $"{root}/建築/法規チェック/建築/平均地盤面算定",
        $"{root}/建築/法規チェック/建築/<RibbonSeparator>⁠",
        $"{root}/建築/法規チェック/建築/採光チェック",
        $"{root}/建築/法規チェック/建築/排煙チェック",
        $"{root}/建築/法規チェック/建築/換気チェック",
        $"{root}/建築/<RibbonSeparator>⁠⁠",
        $"{root}/建築/エクセル",
        $"{root}/建築/エクセル/建築/エクスポート",
        $"{root}/建築/エクセル/建築/集計表エクスポート",
        $"{root}/建築/エクセル/建築/インポート",
        $"{root}/建築/エクセル/建築/<RibbonSeparator>⁠⁠⁠",
        $"{root}/建築/エクセル/建築/Excel画像挿入",
        $"{root}/建築/建具",
        $"{root}/建築/建具/建築/建具姿図作成・更新",
        $"{root}/建築/建具/建築/建具姿図レイアウト",
        $"{root}/建築/床",
        $"{root}/建築/床/建築/自動床：意匠",
        $"{root}/建築/床/建築/自動床：構造",
        $"{root}/建築/床/建築/自動床：基礎",
        $"{root}/建築/床/建築/<RibbonSeparator>⁠⁠⁠⁠",
        $"{root}/建築/床/建築/梁範囲床配置",
        $"{root}/建築/寸法",
        $"{root}/建築/寸法/建築/通り芯寸法の作成",
        $"{root}/建築/寸法/建築/階高寸法の作成",
        $"{root}/建築/フィルタ",
        $"{root}/建築/フィルタ/建築/階層フィルタ",
        $"{root}/建築/結合調整"
      } ;

      return _ltShowPathsCached ;
    }

    /// <summary>Maps legacy preset lines that used the bundled REXJ tab id to the standalone tab name.</summary>
    private static void NormalizeLegacyRibbonTabPrefixes( List<string> lines )
    {
      const string LegacyRoot = "REXJ" ;
      var root = Resource.TAB_NAME ;
      for ( var i = 0 ; i < lines.Count ; i++ ) {
        var line = lines[ i ] ;
        if ( line == LegacyRoot ) lines[ i ] = root ;
        else if ( line.StartsWith( LegacyRoot + "/", StringComparison.Ordinal ) )
          lines[ i ] = root + line.Substring( LegacyRoot.Length ) ;
      }
    }
    
  }
}
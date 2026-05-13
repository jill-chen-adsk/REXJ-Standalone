using System ;
using System.IO ;
using System.Reflection ;
using System.Windows ;
using System.Windows.Media ;
using System.Windows.Media.Imaging ;
using Autodesk.Revit.UI ;
using Collections = System.Collections ;
using Revit = Autodesk.Revit ;
namespace SectionListRC.Components
{
  /// ================================================================================
  /// <summary>UI</summary>
  /// ================================================================================
  internal class UI : SectionListRC.JExtComCompat.RvtUI
  {
    // メンバ変数

    #region Member Variables

    /// <summary>属性</summary>
    private SectionListRC.Components.Attribute _CmpAttribute ;

    /// <summary>アセンブリフォルダ名</summary>
    private string _AssemblyFolderName ;

    /// <summary>アセンブリ名</summary>
    private string _AssemblyName ;

    #endregion Member Variables

    // コンストラクタ

    #region Constructor

    /// ================================================================================
    /// <summary>コンストラクタ</summary>
    ///
    /// <param name="cmpAttribute"  >属性</param>
    /// <param name="rvtUICtrlApp">Revit UIコントロールアプリケーション</param>
    ///
    /// <history>2013/02/01 Created  GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public UI( SectionListRC.Components.Attribute cmpAttribute, Revit.UI.UIControlledApplication rvtUICtrlApp ) : base( rvtUICtrlApp )
    {
      _CmpAttribute = cmpAttribute ;
      _AssemblyFolderName = _CmpAttribute.ExecuteFolder + "\\" ;
      _AssemblyName = _CmpAttribute.ExecuteFile ;
    }

    /// ================================================================================
    /// <summary>コンストラクタ</summary>
    ///
    /// <param name="rvtUIApp">Revit UIアプリケーション</param>
    ///
    /// <history>2013/02/01 Created  GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public UI( Revit.UI.UIApplication rvtUIApp ) : base( rvtUIApp )
    {
    }

    #endregion Constructor

    // メンバ関数

    #region Member Functions

    /// ================================================================================
    /// <summary>リボンパネル設定</summary>
    ///
    /// <history>2013/02/01 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public void SetRibbonPanel()
    {
      // 初期化
      string assembly = "" ;
      Revit.UI.PushButtonData pushBtnData = null ;
      Revit.UI.SplitButtonData splitBtnData = null ;
      Collections.Generic.IList<Revit.UI.RibbonItemData> itemDatas = new Collections.Generic.List<Revit.UI.RibbonItemData>() ;

      string tabName = _CmpAttribute.ResourceText( "IDS_BTN_TABNAME" ) ;
      try { base.CreateRibbonTab( tabName ) ; } catch { }

      // リボンパネル
      string panelName = _CmpAttribute.ResourceText( "IDS_BTN_PANELNAME" ) ;
      Revit.UI.RibbonPanel ribbonPanel = base.CreateRibbonPanel( tabName, panelName ) ;

      assembly = _AssemblyFolderName + _CmpAttribute.ResourceText( "IDS_SECTIONLIST_ASSEMBLYNAME" ) ;

      if ( System.IO.File.Exists( assembly ) == true ) {
        // F1ヘルプ
        Revit.UI.ContextualHelp contextHelp = null ;
        string contHelpPath = _AssemblyFolderName + "Resources" + "\\" + _CmpAttribute.ResourceText( "IDS_TXT_SECTIONLISTHELPHTM" ) ;
        if ( System.IO.File.Exists( contHelpPath ) == true ) {
          contextHelp = new Revit.UI.ContextualHelp( Revit.UI.ContextualHelpType.Url, contHelpPath ) ;
        }

        // RC断面リスト共通設定

        splitBtnData = CreateSplitButtonData( "IDS_BTN_SETTINGFILE_NAME", "IDS_BTN_SETTINGFILE_TEXT", "IDI_BTN_COMMONSETTING_S.png", "IDI_BTN_COMMONSETTING_L.png", "IDS_BTN_SETTINGFILE_TOOLTIP_S", "IDS_BTN_SETTINGFILE_TOOLTIP_L" ) ;

        Revit.UI.SplitButton splitBtn = ribbonPanel.AddItem( splitBtnData ) as Revit.UI.SplitButton ;
        splitBtn.IsSynchronizedWithCurrentItem = false ;
        if ( contextHelp != null ) splitBtn.SetContextualHelp( contextHelp ) ;

        // 共通設定
        pushBtnData = CreatePushButtonData( "IDS_BTN_COMMONSETTING_NAME", "IDS_BTN_COMMONSETTING_TEXT", "IDS_BTN_COMMONSETTING_CLASSNAME", "IDI_BTN_COMMONSETTING_S.png", "IDI_BTN_COMMONSETTING_L.png", "IDS_BTN_COMMONSETTING_TOOLTIP_S", "IDS_BTN_COMMONSETTING_TOOLTIP_L" ) ;
        if ( contextHelp != null ) pushBtnData.SetContextualHelp( contextHelp ) ;
        splitBtn.AddPushButton( pushBtnData ) ;

        // // 設定 - 柱リスト設定
        pushBtnData = CreatePushButtonData( "IDS_BTN_COLUMNLISTSETTING_NAME", "IDS_BTN_COLUMNLISTSETTING_TEXT", "IDS_BTN_COLUMNLISTSETTING_CLASSNAME", "IDI_BTN_COLUMNLISTSETTING_S.png", "IDI_BTN_COLUMNLISTSETTING_L.png", "IDS_BTN_COLUMNLISTSETTING_TOOLTIP_S", "IDS_BTN_COLUMNLISTSETTING_TOOLTIP_L" ) ;
        if ( contextHelp != null ) pushBtnData.SetContextualHelp( contextHelp ) ;
        splitBtn.AddPushButton( pushBtnData ) ;

        // -------

        // 設定 - 梁リスト設定

        pushBtnData = CreatePushButtonData( "IDS_BTN_BEAMLISTSETTING_NAME", "IDS_BTN_BEAMLISTSETTING_TEXT", "IDS_BTN_BEAMLISTSETTING_CLASSNAME", "IDI_BTN_BEAMLISTSETTING_S.png", "IDI_BTN_BEAMLISTSETTING_L.png", "IDS_BTN_BEAMLISTSETTING_TOOLTIP_S", "IDS_BTN_BEAMLISTSETTING_TOOLTIP_L" ) ;
        if ( contextHelp != null ) pushBtnData.SetContextualHelp( contextHelp ) ;
        splitBtn.AddPushButton( pushBtnData ) ;

        // ------
        // 階記号ソート

        pushBtnData = CreatePushButtonData( "IDS_BTN_LEVELSORTORDER_NAME", "IDS_BTN_LEVELSORTORDER_TEXT", "IDS_BTN_LEVELSORTORDER_CLASSNAME", "IDI_BTN_LEVELSORTORDER_S.png", "IDI_BTN_LEVELSORTORDER_L.png", "IDS_BTN_LEVELSORTORDER_TOOLTIP_S", "IDS_BTN_LEVELSORTORDER_TOOLTIP_L" ) ;
        if ( contextHelp != null ) pushBtnData.SetContextualHelp( contextHelp ) ;
        ribbonPanel.AddItem( pushBtnData ) ;

        ribbonPanel.AddSeparator() ;

        // -------
        // 実行 - 柱

        splitBtnData = CreateSplitButtonData( "IDS_BTN_COLUMNLIST_NAME", "IDS_BTN_COLUMNLIST_TEXT", "IDI_BTN_COLUMNLIST_S.png", "IDI_BTN_COLUMNLIST_L.png", "IDS_BTN_COLUMNLIST_TOOLTIP_S", "IDS_BTN_COLUMNLIST_TOOLTIP_L" ) ;

        splitBtn = ribbonPanel.AddItem( splitBtnData ) as Revit.UI.SplitButton ;
        splitBtn.IsSynchronizedWithCurrentItem = false ;
        if ( contextHelp != null ) splitBtn.SetContextualHelp( contextHelp ) ;


        // 実行 - 柱リスト作成

        pushBtnData = CreatePushButtonData( "IDS_BTN_COLUMNLIST_NAME", "IDS_BTN_COLUMNLIST_TEXT", "IDS_BTN_COLUMNLIST_CLASSNAME", "IDI_BTN_COLUMNLISTIMAGE_S.png", "IDI_BTN_COLUMNLISTIMAGE_L.png", "IDS_BTN_COLUMNLIST_TOOLTIP_S", "IDS_BTN_COLUMNLIST_TOOLTIP_L" ) ;

        if ( contextHelp != null ) pushBtnData.SetContextualHelp( contextHelp ) ;
        splitBtn.AddPushButton( pushBtnData ) ;


        // -------------
        // 実行 - 柱 個別画像書き出し
        pushBtnData = CreatePushButtonData( "IDS_BTN_EACHONECOLUMNLIST_NAME", "IDS_BTN_EACHONECOLUMNLIST_TEXT", "IDS_BTN_EACHONECOLUMNLIST_CLASSNAME", "IDI_BTN_EACHONECOLUMNLIST_S.png", "IDI_BTN_EACHONECOLUMNLIST_L.png", "IDS_BTN_EACHONECOLUMNLIST_TOOLTIP_S", "IDS_BTN_EACHONECOLUMNLIST_TOOLTIP_L" ) ;
        if ( contextHelp != null ) pushBtnData.SetContextualHelp( contextHelp ) ;
        splitBtn.AddPushButton( pushBtnData ) ;

        // -----
        // 実行 - 柱 イメージ設定
        pushBtnData = CreatePushButtonData( "IDS_BTN_COLUMNLISTIMAGE_NAME", "IDS_BTN_COLUMNLISTIMAGE_TEXT", "IDS_BTN_COLUMNLISTIMAGE_CLASSNAME", "IDI_BTN_COLUMNLISTIMAGE_S.png", "IDI_BTN_COLUMNLISTIMAGE_L.png", "IDS_BTN_COLUMNLISTIMAGE_TOOLTIP_S", "IDS_BTN_COLUMNLISTIMAGE_TOOLTIP_L" ) ;
        if ( contextHelp != null ) pushBtnData.SetContextualHelp( contextHelp ) ;
        splitBtn.AddPushButton( pushBtnData ) ;


        // -----
        // 実行 - 梁
        splitBtnData = CreateSplitButtonData( "IDS_BTN_BEAMLIST_NAME", "IDS_BTN_BEAMLIST_TEXT", "IDI_BTN_BEAMLIST_S.png", "IDI_BTN_BEAMLIST_L.png", "IDS_BTN_BEAMLIST_TOOLTIP_S", "IDS_BTN_BEAMLIST_TOOLTIP_L" ) ;

        splitBtn = ribbonPanel.AddItem( splitBtnData ) as Revit.UI.SplitButton ;
        splitBtn.IsSynchronizedWithCurrentItem = false ;
        if ( contextHelp != null ) splitBtn.SetContextualHelp( contextHelp ) ;


        // 実行 - 梁リスト作成
        pushBtnData = CreatePushButtonData( "IDS_BTN_BEAMLIST_NAME", "IDS_BTN_BEAMLIST_TEXT", "IDS_BTN_BEAMLIST_CLASSNAME", "IDI_BTN_BEAMLIST_S.png", "IDI_BTN_BEAMLIST_L.png", "IDS_BTN_BEAMLIST_TOOLTIP_S", "IDS_BTN_BEAMLIST_TOOLTIP_L" ) ;

        if ( contextHelp != null ) pushBtnData.SetContextualHelp( contextHelp ) ;
        splitBtn.AddPushButton( pushBtnData ) ;

        // 実行 - 梁 個別画像書き出し
        pushBtnData = CreatePushButtonData( "IDS_BTN_EACHONEBEAMLIST_NAME", "IDS_BTN_EACHONEBEAMLIST_TEXT", "IDS_BTN_EACHONEBEAMLIST_CLASSNAME", "IDI_BTN_EACHONEBEAMLIST_S.png", "IDI_BTN_EACHONEBEAMLIST_L.png", "IDS_BTN_EACHONEBEAMLIST_TOOLTIP_S", "IDS_BTN_EACHONEBEAMLIST_TOOLTIP_L" ) ;
        if ( contextHelp != null ) pushBtnData.SetContextualHelp( contextHelp ) ;
        splitBtn.AddPushButton( pushBtnData ) ;

        // 実行 - 梁 イメージ設定
        pushBtnData = CreatePushButtonData( "IDS_BTN_BEAMLISTIMAGE_NAME", "IDS_BTN_BEAMLISTIMAGE_TEXT", "IDS_BTN_BEAMLISTIMAGE_CLASSNAME", "IDI_BTN_BEAMLISTIMAGE_S.png", "IDI_BTN_BEAMLISTIMAGE_L.png", "IDS_BTN_BEAMLISTIMAGE_TOOLTIP_S", "IDS_BTN_BEAMLISTIMAGE_TOOLTIP_L" ) ;
        
        if (contextHelp != null)pushBtnData.SetContextualHelp(contextHelp);
        splitBtn.AddPushButton(pushBtnData);

        // Standalone version: enabled for all Revit editions
      }

      #endregion Member Functions
    }


    // private BitmapImage ResImageInPack( string path )
    // {
    //   var bitmapImage = new BitmapImage( new Uri( $@"pack://application:,,,/SectionListRC;component/{path}", UriKind.Absolute ) ) ;
    //   return bitmapImage ;
    // }
    private BitmapImage GetEmbeddedImage(string resourceName)
    {
      var assembly = Assembly.GetExecutingAssembly();
      using var stream = assembly.GetManifestResourceStream($"SectionListRC.Res.{resourceName}" );
      if (stream == null)
      {
        throw new Exception();
      }

      var img = System.Drawing.Image.FromStream( stream ) ;


      return ConvertToBitmapImage( img ) ;
    }
    private BitmapImage ConvertToBitmapImage(System.Drawing.Image img)
    {
      using var memoryStream = new MemoryStream();
      img.Save(memoryStream, img.RawFormat); // 画像をMemoryStreamに保存
      memoryStream.Seek(0, SeekOrigin.Begin); // ストリームの位置を先頭に戻す

      var bitmapImage = new BitmapImage();
      bitmapImage.BeginInit();
      bitmapImage.StreamSource = memoryStream;
      bitmapImage.CacheOption = BitmapCacheOption.OnLoad; // ストリームが閉じられた後も画像を保持
      bitmapImage.EndInit();
      bitmapImage.Freeze(); // スレッドセーフにするためにFreezeを呼ぶ

      return bitmapImage;
    }
    
    private SplitButtonData CreateSplitButtonData( string nameId, string textId, string imagePathS, string imagePathL, string toolTipId, string descId )
    {
      var splitBtnData = new SplitButtonData( _CmpAttribute.ResourceText( nameId ), _CmpAttribute.ResourceText( textId ) ) ;
      try {
        // splitBtnData.Image = ResImageInPack( imagePathS ) ;
        // splitBtnData.LargeImage = ResImageInPack( imagePathL ) ;
        // splitBtnData.ToolTip = _CmpAttribute.ResourceText( toolTipId ) ;
        // splitBtnData.LongDescription = _CmpAttribute.ResourceText( descId ) ;
        splitBtnData.Image = GetEmbeddedImage( imagePathS ) ;
        splitBtnData.LargeImage = GetEmbeddedImage( imagePathL ) ;
        splitBtnData.ToolTip = _CmpAttribute.ResourceText( toolTipId ) ;
        splitBtnData.LongDescription = _CmpAttribute.ResourceText( descId ) ;
      }
      catch ( Exception e ) {
        Console.WriteLine( e ) ;
      }

      return splitBtnData ;
    }

    private PushButtonData CreatePushButtonData( string nameId, string textId, string classNameId, string imagePathS, string imagePathL, string toolTipId, string descId )
    {
      var pushBtnData = new PushButtonData( _CmpAttribute.ResourceText( nameId ), _CmpAttribute.ResourceText( textId ), Assembly.GetExecutingAssembly().Location, _CmpAttribute.ResourceText( classNameId ) ) ;
      try {
        pushBtnData.Image = GetEmbeddedImage( imagePathS ) ;
        pushBtnData.LargeImage = GetEmbeddedImage( imagePathL ) ;
        pushBtnData.ToolTip = _CmpAttribute.ResourceText( toolTipId ) ;
        pushBtnData.LongDescription = _CmpAttribute.ResourceText( descId ) ;
      }
      catch ( Exception e ) {
        Console.WriteLine( e ) ;
      }

      return pushBtnData ;
    }
  }
}
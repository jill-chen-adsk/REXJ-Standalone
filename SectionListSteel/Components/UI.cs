using System;
using System.IO ;
using System.Reflection ;
using System.Windows.Media.Imaging ;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
namespace SectionListSteel.Components
{
    /// ================================================================================
    /// <summary>UI</summary>
    /// ================================================================================
    internal class UI : SectionListSteel.JExtComCompat.RvtUI
    {
        // メンバ変数

        #region Member Variables

        /// <summary>属性</summary>
        private SectionListSteel.Components.Attribute _CmpAttribute;

        /// <summary>アセンブリフォルダ名</summary>
        private string _AssemblyFolderName;

        /// <summary>アセンブリ名</summary>
        private string _AssemblyName;

        #endregion Member Variables

        // コンストラクタ

        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute">属性</param>
        /// <param name="rvtUICtrlApp">Revit UI コントロールアプリケーション</param>
        ///
        /// <history>2016/08/05 Created Ryo Kuroda</history>
        /// ================================================================================
        public
        UI(SectionListSteel.Components.Attribute cmpAttribute,
           Revit.UI.UIControlledApplication rvtUICtrlApp) :
          base(rvtUICtrlApp)
        {
            _CmpAttribute = cmpAttribute;

            _AssemblyFolderName = _CmpAttribute.ExecuteFolder + "\\";
            _AssemblyName = _CmpAttribute.ExecuteFile;
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>リボン設定</summary>
        ///
        /// <history>2016/08/05 Created Ryo Kuroda</history>
        /// ================================================================================
        public
        void SetRibbon()
        {
            // 初期化
            string assembly = "";
            Revit.UI.PushButtonData pushBtnData = null;
            Revit.UI.SplitButtonData splitBtnData = null;
            Collections.Generic.IList<Revit.UI.RibbonItemData> itemDatas = new Collections.Generic.List<Revit.UI.RibbonItemData>();

            // リボンタブ
            string tabName = _CmpAttribute.ResourceText("IDS_BTN_TABNAME");
            try { base.CreateRibbonTab(tabName); } catch { }

            // リボンパネル
            string panelName = _CmpAttribute.ResourceText("IDS_BTN_PANELNAME");
            Revit.UI.RibbonPanel ribbonPanel = base.CreateRibbonPanel(tabName, panelName);

            assembly = _AssemblyFolderName + _CmpAttribute.ResourceText("IDS_SECTIONLIST_ASSEMBLYNAME");

            if (System.IO.File.Exists(assembly))
            {
                // F1ヘルプ
                Revit.UI.ContextualHelp contextHelp = null;
                string contextHelpPath = _AssemblyFolderName +
                                  "Resources" + "\\" +
                                  _CmpAttribute.ResourceText("IDS_TXT_HELPHTM");

                if (System.IO.File.Exists(contextHelpPath) == true)
                {
                    contextHelp = new Revit.UI.ContextualHelp(Revit.UI.ContextualHelpType.Url, contextHelpPath);
                }
                else
                {
                    contextHelp = new Revit.UI.ContextualHelp(Revit.UI.ContextualHelpType.Url, "http://help.autodesk.com/view/RVT/2017/JPN/");
                }

                var img0 = GetEmbeddedImage( "_16x16.IDI_BTN_COMMONSETTING_S.png" ) ;
                
                // 共通設定
                pushBtnData = base.CreatePushButtonData(_CmpAttribute.ResourceText("IDS_BTN_COMMONSETTING_NAME"),
                                                        _CmpAttribute.ResourceText("IDS_BTN_COMMONSETTING_TEXT"),
                                                        GetEmbeddedImage( "_16x16.IDI_BTN_COMMONSETTING_S.png" ) ,
                                                        GetEmbeddedImage( "_32x32.IDI_BTN_COMMONSETTING_L.png" ) ,
                                                        _CmpAttribute.ResourceText("IDS_BTN_COMMONSETTING_TOOLTIP_S"),
                                                        _CmpAttribute.ResourceText("IDS_BTN_COMMONSETTING_TOOLTIP_L"),
                                                        null,
                                                        assembly,
                                                        _CmpAttribute.ResourceText("IDS_BTN_COMMONSETTING_CLASSNAME"),
                                                        "");

                if (contextHelp != null)
                    pushBtnData.SetContextualHelp(contextHelp);

                ribbonPanel.AddItem(pushBtnData);

                // 階記号ソート
                pushBtnData = base.CreatePushButtonData(_CmpAttribute.ResourceText("IDS_BTN_LEVELSORTORDER_NAME"),
                                                        _CmpAttribute.ResourceText("IDS_BTN_LEVELSORTORDER_TEXT"),
                                                        GetEmbeddedImage( "_16x16.IDI_BTN_LEVELSORTORDER_S.png" ) ,
                                                        GetEmbeddedImage( "_32x32.IDI_BTN_LEVELSORTORDER_L.png" ) ,
                                                        _CmpAttribute.ResourceText("IDS_BTN_LEVELSORTORDER_TOOLTIP_S"),
                                                        _CmpAttribute.ResourceText("IDS_BTN_LEVELSORTORDER_TOOLTIP_L"),
                                                        null,
                                                        assembly,
                                                        _CmpAttribute.ResourceText("IDS_BTN_LEVELSORTORDER_CLASSNAME"),
                                                        "");

                if (contextHelp != null)
                    pushBtnData.SetContextualHelp(contextHelp);

                ribbonPanel.AddItem(pushBtnData);
                ribbonPanel.AddSeparator();

                //Create sub items
                Column_SubItems(ribbonPanel, contextHelp, assembly);
            }

            //// OneBoxまたはStructure以外は使用不可 （スタンドアロンでは全製品で有効）
            // if (RvtUICtrlApp.ControlledApplication.Product != Revit.ApplicationServices.ProductType.Revit &&
            //     RvtUICtrlApp.ControlledApplication.Product != Autodesk.Revit.ApplicationServices.ProductType.Structure)
            // {
            //     ribbonPanel.Enabled = false;
            // }
        }

        private void Column_SubItems(Revit.UI.RibbonPanel ribbonPanel, Revit.UI.ContextualHelp contextHelp, string assembly)
        {
            var splitBtnData = base.CreatePulldownButtonData(_CmpAttribute.ResourceText("IDS_BTN_LIST_NAME"),
                                                          _CmpAttribute.ResourceText("IDS_BTN_LIST_TEXT"),
                                                          GetEmbeddedImage( "_16x16.IDI_BTN_LIST_S.png" ) ,
                                                          GetEmbeddedImage( "_32x32.IDI_BTN_LIST_L.png" ) ,
                                                          _CmpAttribute.ResourceText("IDS_BTN_LIST_TOOLTIP_S"),
                                                          _CmpAttribute.ResourceText("IDS_BTN_LIST_TOOLTIP_L"),
                                                          null
                                                          );

            Revit.UI.PulldownButton splitBtn = ribbonPanel.AddItem(splitBtnData) as Revit.UI.PulldownButton;
            if (contextHelp != null)
                splitBtn.SetContextualHelp(contextHelp);

            //All
            var pushBtnData = base.CreatePushButtonData(_CmpAttribute.ResourceText("IDS_BTN_ALL_NAME"),
                                                    _CmpAttribute.ResourceText("IDS_BTN_ALL_TEXT"),
                                                    GetEmbeddedImage( "_16x16.IDI_BTN_LIST_S.png" ) ,
                                                    GetEmbeddedImage( "_32x32.IDI_BTN_LIST_L.png" ) ,
                                                    _CmpAttribute.ResourceText("IDS_BTN_ALL_TOOLTIP_S"),
                                                    _CmpAttribute.ResourceText("IDS_BTN_ALL_TOOLTIP_L"),
                                                    null,
                                                    assembly,
                                                    _CmpAttribute.ResourceText("IDS_BTN_ALL_CLASSNAME"),
                                                    "");

            if (contextHelp != null)
                pushBtnData.SetContextualHelp(contextHelp);

            splitBtn.AddPushButton(pushBtnData);

            //Column
            pushBtnData = base.CreatePushButtonData(_CmpAttribute.ResourceText("IDS_BTN_COLUMN_TYPE_NAME"),
                                                    _CmpAttribute.ResourceText("IDS_BTN_COLUMN_TYPE_TEXT"),
                                                    GetEmbeddedImage( "_16x16.IDI_BTN_LISTCOLUMN_S.png" ) ,
                                                    GetEmbeddedImage( "_32x32.IDI_BTN_LISTCOLUMN_L.png" ) ,
                                                    _CmpAttribute.ResourceText("IDS_BTN_COLUMN_TYPE_TOOLTIP_S"),
                                                    _CmpAttribute.ResourceText("IDS_BTN_COLUMN_TYPE_TOOLTIP_L"),
                                                    null,
                                                    assembly,
                                                    _CmpAttribute.ResourceText("IDS_BTN_COLUMN_TYPE_CLASSNAME"),
                                                    "");

            if (contextHelp != null)
                pushBtnData.SetContextualHelp(contextHelp);

            splitBtn.AddPushButton(pushBtnData);

            //Post
            pushBtnData = base.CreatePushButtonData(_CmpAttribute.ResourceText("IDS_BTN_POST_TYPE_NAME"),
                                                    _CmpAttribute.ResourceText("IDS_BTN_POST_TYPE_TEXT"),
                                                    GetEmbeddedImage( "_16x16.IDI_BTN_LISTPOST_S.png" ) ,
                                                    GetEmbeddedImage( "_32x32.IDI_BTN_LISTPOST_L.png" ) ,
                                                    _CmpAttribute.ResourceText("IDS_BTN_POST_TYPE_TOOLTIP_S"),
                                                    _CmpAttribute.ResourceText("IDS_BTN_POST_TYPE_TOOLTIP_L"),
                                                    null,
                                                    assembly,
                                                    _CmpAttribute.ResourceText("IDS_BTN_POST_TYPE_CLASSNAME"),
                                                    "");

            if (contextHelp != null)
                pushBtnData.SetContextualHelp(contextHelp);

            splitBtn.AddPushButton(pushBtnData);

            //Girder
            pushBtnData = base.CreatePushButtonData(_CmpAttribute.ResourceText("IDS_BTN_GIRDER_TYPE_NAME"),
                                                    _CmpAttribute.ResourceText("IDS_BTN_GIRDER_TYPE_TEXT"),
                                                    GetEmbeddedImage( "_16x16.IDI_BTN_GIRDERLIST_S.png" ) ,
                                                    GetEmbeddedImage( "_32x32.IDI_BTN_GIRDERLIST_L.png" ) ,
                                                    _CmpAttribute.ResourceText("IDS_BTN_GIRDER_TYPE_TOOLTIP_S"),
                                                    _CmpAttribute.ResourceText("IDS_BTN_GIRDER_TYPE_TOOLTIP_L"),
                                                    null,
                                                    assembly,
                                                    _CmpAttribute.ResourceText("IDS_BTN_GIRDER_TYPE_CLASSNAME"),
                                                    "");

            if (contextHelp != null)
                pushBtnData.SetContextualHelp(contextHelp);

            splitBtn.AddPushButton(pushBtnData);

            //Beam
            pushBtnData = base.CreatePushButtonData(_CmpAttribute.ResourceText("IDS_BTN_BEAM_TYPE_NAME"),
                                                    _CmpAttribute.ResourceText("IDS_BTN_BEAM_TYPE_TEXT"),
                                                    GetEmbeddedImage( "_16x16.IDI_BTN_LISTBEAM_S.png" ) ,
                                                    GetEmbeddedImage( "_32x32.IDI_BTN_LISTBEAM_L.png" ) ,
                                                    _CmpAttribute.ResourceText("IDS_BTN_BEAM_TYPE_TOOLTIP_S"),
                                                    _CmpAttribute.ResourceText("IDS_BTN_BEAM_TYPE_TOOLTIP_L"),
                                                    null,
                                                    assembly,
                                                    _CmpAttribute.ResourceText("IDS_BTN_BEAM_TYPE_CLASSNAME"),
                                                    "");

            if (contextHelp != null)
                pushBtnData.SetContextualHelp(contextHelp);

            splitBtn.AddPushButton(pushBtnData);

            //Brace
            pushBtnData = base.CreatePushButtonData(_CmpAttribute.ResourceText("IDS_BTN_BRACE_TYPE_NAME"),
                                                    _CmpAttribute.ResourceText("IDS_BTN_BRACE_TYPE_TEXT"),
                                                    GetEmbeddedImage( "_16x16.IDI_BTN_BRACELIST_S.png" ) ,
                                                    GetEmbeddedImage( "_32x32.IDI_BTN_BRACELIST_L.png" ) ,
                                                    _CmpAttribute.ResourceText("IDS_BTN_BRACE_TYPE_TOOLTIP_S"),
                                                    _CmpAttribute.ResourceText("IDS_BTN_BRACE_TYPE_TOOLTIP_L"),
                                                    null,
                                                    assembly,
                                                    _CmpAttribute.ResourceText("IDS_BTN_BRACE_TYPE_CLASSNAME"),
                                                    "");

            if (contextHelp != null)
                pushBtnData.SetContextualHelp(contextHelp);

            splitBtn.AddPushButton(pushBtnData);
        }

        private BitmapImage ResImageInPack(string path)
        {
            return new BitmapImage( new Uri( $@"pack://application:,,,/SectionListSteel;component/{path}", UriKind.Absolute ) ) ;
        }
        private BitmapImage GetEmbeddedImage(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream($"SectionListSteel.Resources.{resourceName}" );
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
        
        #endregion Member Functions

        // プロパティ
    }
}
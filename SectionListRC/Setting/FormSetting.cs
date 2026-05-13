using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using System.Reflection;
using UTILS = SectionListRC.Utils;

namespace SectionListRC.Setting
{
    /// ================================================================================
    /// <summary>フォーム 共通設定</summary>
    /// ================================================================================
    public partial class FormSetting : Form
    {
        // メンバ変数
        #region Member Variables

        /// <summary>属性</summary>
        private SectionListRC.Components.Attribute _CmpAttribute;

        /// <summary>設定ファイル名</summary>
        private string _SettingFileName;

        /// <summary>フル設定ファイル名</summary>
        private string _FullName;

        /// <summary>設定ファイルディレクトリ</summary>
        private string _SettingFileDirectory;

        /// <summary>柱ビュー尺度のインデックス</summary>
        private int _ColumnListViewScaleIndex;

        /// <summary>梁ビュー尺度のインデックス</summary>
        private int _BeamListViewScaleIndex;

        /// <summary>柱リストビュー尺度</summary>
        private string _ColumnListViewScale;

        /// <summary>梁リストビュー尺度</summary>
        private string _BeamListViewScale;

        /// <summary>タイトルフォント</summary>
        private string _TitleFont;

        /// <summary>小項目フォント</summary>
        private string _ItemFont;

        /// <summary>寸法線タイプ</summary>
        private string _DimensionType;

        /// <summary>枠線種タイプ</summary>
        private string _FrameLineType;

        /// <summary>躯体線種タイプ</summary>
        private string _BodyLineType;

        /// <summary>幅止筋線種タイプ</summary>
        private string _SpacerLineType;

        /// <summary>階表示枠表示</summary>
        private int _LevelFrameShow;

        /// <summary>階表示枠幅</summary>
        private string _LevelFrameWidth;

        /// <summary>項目表示枠幅</summary>
        private string _ItemFrameWidth;

        /// <summary>符号表示枠高さ</summary>
        private string _SymbolFrameHeight;

        /// <summary>配筋枠高さ</summary>
        private string _ArrangementFrameHeight;

        /// <summary>項目表示枠幅2</summary>
        private string _ItemFrameWidth2;

        /// <summary>符号表示枠高さ2</summary>
        private string _SymbolFrameHeight2;

        /// <summary>配筋枠高さ2</summary>
        private string _ArrangementFrameHeight2;

        /// <summary>階表示枠タイトル</summary>
        private string _LevelFrameTitle;

        /// <summary>階表示枠接尾語</summary>
        private string _LevelFrameEndWord;

        /// <summary>符号表示枠タイトル</summary>
        private string _SymbolFrameTitle;

        /// <summary>文字タイプ</summary>
        private Collections.Generic.IList<Revit.DB.TextNoteType> _TxtNoteTypes;

        /// <summary>寸法タイプ</summary>
        private Collections.Generic.IList<Revit.DB.DimensionType> _DimTypes;

        /// <summary>線種タイプ</summary>
        private Collections.Generic.IList<Revit.DB.GraphicsStyle> _GraStyles;

        /// <summary>テーブルファイル名</summary>
        private string _TableFileName;

        /// <summary>フルテーブルファイル名</summary>
        private string _FullTableName;

        /// <summary>テーブルファイルディレクトリ</summary>
        private string _TableFileDirectory;

        /// <summary>テーブルの指定</summary>
        private string _PickTable;

        /// <summary>操作結果</summary>
        private int _Result;

        #endregion Member Variables

        // コンストラクタ
        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute">属性</param>
        ///
        /// <history>2013/02/04 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        public FormSetting(SectionListRC.Components.Attribute cmpAttribute,
                           string settingFileName,
                           string settingFileDirectory,
                           Collections.Generic.IList<Revit.DB.TextNoteType> txtNoteTypes,
                           Collections.Generic.IList<Revit.DB.DimensionType> dimTypes,
                           Collections.Generic.IList<Revit.DB.GraphicsStyle> graStyles)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            
            InitializeComponent();
            _CmpAttribute = cmpAttribute;
            _SettingFileName = settingFileName;
            _SettingFileDirectory = settingFileDirectory;
            _TxtNoteTypes = txtNoteTypes;
            _DimTypes = dimTypes;
            _GraStyles = graStyles;

            SetData();
            SetSettingValue();
        }

        #endregion Constructor

        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>データ設定</summary>
        ///
        /// <history>2013/02/04 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        private
        void SetData()
        {
            // フォント
            foreach (Revit.DB.TextNoteType tnp in _TxtNoteTypes) {
                this.cmbBoxTitleFont.Items.Add(tnp.Name);
                this.cmbBoxOtherFont.Items.Add(tnp.Name);
            }

            this.cmbBoxTitleFont.Sorted = true;
            this.cmbBoxOtherFont.Sorted = true;

            // スタイル変更
            this.cmbBoxTitleFont.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbBoxOtherFont.DropDownStyle = ComboBoxStyle.DropDownList;

            SetCmbBoxDropWidth(this.cmbBoxTitleFont);
            SetCmbBoxDropWidth(this.cmbBoxOtherFont);

            // 寸法
            foreach (Revit.DB.DimensionType dt in _DimTypes) {
                if (dt.StyleType == Revit.DB.DimensionStyleType.Linear && dt.Parameters.Size > 0 && dt.Name != dt.FamilyName) {
                    this.cmbBoxDimensionType.Items.Add(dt.Name);
                }
            }

            this.cmbBoxDimensionType.Sorted = true;

            this.cmbBoxDimensionType.DropDownStyle = ComboBoxStyle.DropDownList;
            SetCmbBoxDropWidth(this.cmbBoxDimensionType);

            // 線種タイプ
            foreach (Revit.DB.GraphicsStyle gs in _GraStyles) {
                this.cmbBoxLineTypeFrame.Items.Add(gs.Name);
                this.cmbBoxLineTypeBody.Items.Add(gs.Name);
                this.cmbBoxLineTypeStoper.Items.Add(gs.Name);
            }

            this.cmbBoxLineTypeFrame.Sorted = true;
            this.cmbBoxLineTypeBody.Sorted = true;
            this.cmbBoxLineTypeStoper.Sorted = true;

            this.cmbBoxLineTypeFrame.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbBoxLineTypeBody.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbBoxLineTypeStoper.DropDownStyle = ComboBoxStyle.DropDownList;

            SetCmbBoxDropWidth(this.cmbBoxLineTypeFrame);
            SetCmbBoxDropWidth(this.cmbBoxLineTypeBody);
            SetCmbBoxDropWidth(this.cmbBoxLineTypeStoper);

            // 右寄せ
            this.txtBoxLvlFrameWidth.TextAlign = HorizontalAlignment.Right;
            this.txtBoxItemFrameWidth.TextAlign = HorizontalAlignment.Right;
            this.txtBoxArrangementFrameHeight.TextAlign = HorizontalAlignment.Right;
            this.txtBoxSymbolFrameHeight.TextAlign = HorizontalAlignment.Right;
            this.txtBoxItemFrameWidth2.TextAlign = HorizontalAlignment.Right;
            this.txtBoxArrangementFrameHeight2.TextAlign = HorizontalAlignment.Right;
            this.txtBoxSymbolFrameHeight2.TextAlign = HorizontalAlignment.Right;

            // 文字数
            this.txtBoxLvlFrameWidth.MaxLength = 5;
            this.txtBoxItemFrameWidth.MaxLength = 5;
            this.txtBoxArrangementFrameHeight.MaxLength = 5;
            this.txtBoxSymbolFrameHeight.MaxLength = 5;
            this.txtBoxItemFrameWidth2.MaxLength = 5;
            this.txtBoxArrangementFrameHeight2.MaxLength = 5;
            this.txtBoxSymbolFrameHeight2.MaxLength = 5;
        }

        /// ================================================================================
        /// <summary>設定値設定</summary>
        ///
        /// <history><p>2013/04/11 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private
        void SetSettingValue()
        {
            // 設定ファイルから値を取得
            GetSettingValue();

            // 柱ビュー尺度
            bool isContain = false;

            string str = "";
            if (_ColumnListViewScale != "" && _ColumnListViewScale != null) {
                str = "1 : " + _ColumnListViewScale;
            }
            else {
                str = "1 : 30";
            }

            for (int i = 0; i < this.cmbBoxColumnListViewScale.Items.Count; i++) {
                if ((string)this.cmbBoxColumnListViewScale.Items[i] == str) {
                    this.cmbBoxColumnListViewScale.SelectedIndex = i;
                    isContain = true;
                    break;
                }
            }
            if (isContain == false) {
                this.cmbBoxColumnListViewScale.DropDownStyle = ComboBoxStyle.DropDown;

                if (this.cmbBoxColumnListViewScale.Items.Count == 13) {
                    this.cmbBoxColumnListViewScale.Items.Add("");
                }

                this.cmbBoxColumnListViewScale.Items[13] = str;
                this.cmbBoxColumnListViewScale.SelectedIndex = 13;
            }

            this.cmbBoxColumnListViewScale.DropDownStyle = ComboBoxStyle.DropDownList;
            SetCmbBoxDropWidth(this.cmbBoxColumnListViewScale);

            _ColumnListViewScaleIndex = this.cmbBoxColumnListViewScale.SelectedIndex;

            // 梁ビュー尺度
            isContain = false;

            if (_BeamListViewScale != "" && _BeamListViewScale != null) {
                str = "1 : " + _BeamListViewScale;
            }
            else {
                str = "1 : 30";
            }

            for (int i = 0; i < this.cmbBoxBeamListViewScale.Items.Count; i++) {
                if ((string)this.cmbBoxBeamListViewScale.Items[i] == str) {
                    this.cmbBoxBeamListViewScale.SelectedIndex = i;
                    isContain = true;
                    break;
                }
            }
            if (isContain == false) {
                this.cmbBoxBeamListViewScale.DropDownStyle = ComboBoxStyle.DropDown;

                if (this.cmbBoxBeamListViewScale.Items.Count == 13) {
                    this.cmbBoxBeamListViewScale.Items.Add("");
                }

                this.cmbBoxBeamListViewScale.Items[13] = str;
                this.cmbBoxBeamListViewScale.SelectedIndex = 13;
            }

            this.cmbBoxBeamListViewScale.DropDownStyle = ComboBoxStyle.DropDownList;
            SetCmbBoxDropWidth(this.cmbBoxBeamListViewScale);

            _BeamListViewScaleIndex = this.cmbBoxBeamListViewScale.SelectedIndex;

            // フォント
            for (int i = 0; i < this.cmbBoxTitleFont.Items.Count; i++) {
                if ((string)this.cmbBoxTitleFont.Items[i] == _TitleFont) {
                    this.cmbBoxTitleFont.SelectedIndex = i;
                }

                if ((string)this.cmbBoxOtherFont.Items[i] == _ItemFont) {
                    this.cmbBoxOtherFont.SelectedIndex = i;
                }
            }

            // 寸法
            for (int i = 0; i < this.cmbBoxDimensionType.Items.Count; i++) {
                if ((string)this.cmbBoxDimensionType.Items[i] == _DimensionType) {
                    this.cmbBoxDimensionType.SelectedIndex = i;
                }
            }

            // 線種タイプ
            for (int i = 0; i < this.cmbBoxLineTypeFrame.Items.Count; i++) {
                if ((string)this.cmbBoxLineTypeFrame.Items[i] == _FrameLineType) {
                    this.cmbBoxLineTypeFrame.SelectedIndex = i;
                }

                if ((string)this.cmbBoxLineTypeBody.Items[i] == _BodyLineType) {
                    this.cmbBoxLineTypeBody.SelectedIndex = i;
                }

                if ((string)this.cmbBoxLineTypeStoper.Items[i] == _SpacerLineType) {
                    this.cmbBoxLineTypeStoper.SelectedIndex = i;
                }
            }

            // 階表示枠表示
            if (_LevelFrameShow == 0) {
                this.rdoBtnLvlFrameShow.Checked = true;
            }
            else {
                this.rdoBtnLvlFrameShowInItemFrame.Checked = true;
            }

            // 枠幅高さ
            this.txtBoxLvlFrameWidth.Text = _LevelFrameWidth;
            this.txtBoxItemFrameWidth.Text = _ItemFrameWidth;
            this.txtBoxSymbolFrameHeight.Text = _SymbolFrameHeight;
            this.txtBoxArrangementFrameHeight.Text = _ArrangementFrameHeight;
            this.txtBoxItemFrameWidth2.Text = _ItemFrameWidth2;
            this.txtBoxSymbolFrameHeight2.Text = _SymbolFrameHeight2;
            this.txtBoxArrangementFrameHeight2.Text = _ArrangementFrameHeight2;

            this.txtBoxLvlFrameTitle.Text = _LevelFrameTitle;
            this.txtBoxLvlFrameEndWord.Text = _LevelFrameEndWord;
            this.txtBoxSymbolFrameTitle.Text = _SymbolFrameTitle;
        }

        /// ================================================================================
        /// <summary>文字設定</summary>
        ///
        /// <history><p>2013/02/04 Created GSA,Inc Ryo Kuroda</p>
        ///           <p>2017/02/16 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        private
        void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_COMMONSETTING") ;

            this.grpBoxSettingFile.Text = _CmpAttribute.ResourceText("IDS_TXT_SETTINGFILE");
            this.lblCurrentFile.Text = _CmpAttribute.ResourceText("IDS_TXT_CURRENTSETTINGFILE");

            // 設定ファイル
            this.lblCurrentFileName.Text = _SettingFileName;

            bool isSettingFileRight = true;
            string fullName = _SettingFileDirectory + _SettingFileName;

            if (this.lblCurrentFileName.Text == "" || System.IO.File.Exists(fullName) == false) {
                this.lblCurrentFileName.Text = "- " + _CmpAttribute.ResourceText("IDS_TXT_SETDEFAULT");
            }
            else {
                isSettingFileRight = IsSettingFileRight(fullName);
            }

            this.btnReadSettingFile.Text = _CmpAttribute.ResourceText("IDS_TXT_READ");

            // テーブル指定
            this.grpBoxSelectTable.Text = ""; // _CmpAttribute.ResourceText("IDS_TXT_TABLE");
            this.chkBoxPickTable.Text = _CmpAttribute.ResourceText("IDS_TXT_PICKTABLE");
            this.lblCurrentTable.Text = _CmpAttribute.ResourceText("IDS_TXT_CURRENTTABLEFILE");
            this.btnTableSelect.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECT");

      var version = _CmpAttribute.ResourceText( "IDS_TXT_REVITVERSION_2027" ) ;
            
            
            if (System.IO.File.Exists(_FullTableName)) {
                this.lblCurrentTableName.Text = _TableFileName;
            }
            else {
                // マイドキュメント
                string myDoc = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

                // 基準ファイル
                string defFile = myDoc + $"\\Autodesk REXJ\\{version}\\" + _CmpAttribute.ResourceText($"IDS_TXT_TABLEFILE_{version}");
                if (System.IO.File.Exists(defFile)) {
                    this.lblCurrentTableName.Text = _CmpAttribute.ResourceText($"IDS_TXT_TABLEFILE_{version}");
                }
                else {
                    this.lblCurrentTableName.Text = "- " + _CmpAttribute.ResourceText("IDS_TXT_USESHARETABLE");
                }
            }

            this.grpBoxCustomViewScale.Text = _CmpAttribute.ResourceText("IDS_TXT_VIEWSCALE");
            this.lblCustomViewScaleExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_VIEWSCALEEXPLAIN");
            this.lblColumnListViewScale.Text = _CmpAttribute.ResourceText("IDS_TXT_COLUMNVIEWLISTSCALE");
            this.lblBeamListViewScale.Text = _CmpAttribute.ResourceText("IDS_TXT_BEAMLISTVIEWSCALE");

            this.grpBoxFontType.Text = _CmpAttribute.ResourceText("IDS_TXT_FONTTYPE");
            this.lblFontTypeExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_FONTTYPEEXPLAIN");
            this.lblTitleFont.Text = _CmpAttribute.ResourceText("IDS_TXT_TITLE");
            this.lblOtherFont.Text = _CmpAttribute.ResourceText("IDS_TXT_OTHER");

            this.grpBoxDimensionType.Text = _CmpAttribute.ResourceText("IDS_TXT_DIMENSIONTYPE");
            this.lblDimensionTypeExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_DIMENSIONTYPEEXPLAIN");
            this.lblDimensionType.Text = _CmpAttribute.ResourceText("IDS_TXT_DIMENSIONTYPE");

            this.grpBoxLineType.Text = _CmpAttribute.ResourceText("IDS_TXT_LINETYPE");
            this.lblLineTypeExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_LINETYPEEXPLAIN");
            this.lblLineTypeFrame.Text = _CmpAttribute.ResourceText("IDS_TXT_FRAME");
            this.lblLineTypeBody.Text = _CmpAttribute.ResourceText("IDS_TXT_BODY");
            this.lblLineTypeStoper.Text = _CmpAttribute.ResourceText("IDS_TXT_STOPER");

            this.grpBoxFrame.Text = _CmpAttribute.ResourceText("IDS_TXT_SHOWFRAME");
            this.lbFrameExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_SHOWFRAMEEXPLAIN");

            this.rdoBtnLvlFrameShow.Text = _CmpAttribute.ResourceText("IDS_TXT_LEVELFRAMESHOW");
            this.rdoBtnLvlFrameShowInItemFrame.Text = _CmpAttribute.ResourceText("IDS_TXT_LEVELFRAMESHOWINITEMFRAME");
            this.lblLvlFrameWidthMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.lblLvlFrameTitle.Text = _CmpAttribute.ResourceText("IDS_TXT_LEVELFRAMETITLE");
            this.lblLvlFrameEndWord.Text = _CmpAttribute.ResourceText("IDS_TXT_LEVELFRAMEENDWORD");

            this.lblSymbolFrameTitle.Text = _CmpAttribute.ResourceText("IDS_TXT_SYMBOLFRAMETITLE");

            this.lblItemFrameWidthMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.lblSymbolFrameHeightMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.lblArrangementFrameHeightMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");

            this.lblItemFrameWidthMM2.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.lblSymbolFrameHeightMM2.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.lblArrangementFrameHeightMM2.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");

            this.btnToColumnListSettingFromSetting.Text = _CmpAttribute.ResourceText("IDS_TXT_COLUMNLISTSETTING");
            this.btnToBeamListSetting1FromSetting.Text = _CmpAttribute.ResourceText("IDS_TXT_BEAMLISTSETTING");
            this.btnOverWriteSave.Text = _CmpAttribute.ResourceText("IDS_TXT_OVERWRITESAVE");
            this.btnSaveAs.Text = _CmpAttribute.ResourceText("IDS_TXT_SAVEAS");
            this.btnEnd.Text = _CmpAttribute.ResourceText("IDS_TXT_END");

            if (isSettingFileRight == false) {
                System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_SETTINGFILEINCORRECT") + "\r\n" + _CmpAttribute.ResourceText("IDS_ERR_SELOTHERFILEORRESET"));
            }
        }

        /// ================================================================================
        /// <summary>設定値取得</summary>
        ///
        /// <history><p>2013/04/10 Created GSA,Inc Ryo Kuroda</p>
        ///           <p>2017/07/31 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        void GetSettingValue()
        {
            string fullName = _SettingFileDirectory + _SettingFileName;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            System.Text.Encoding enc = System.Text.Encoding.GetEncoding("shift_jis");

      var version = _CmpAttribute.ResourceText( "IDS_TXT_REVITVERSION_2027" ) ;
            
            if (System.IO.File.Exists(fullName)) {
                string[] _StrAry = System.IO.File.ReadAllLines(fullName, enc);

                //if (_StrAry.Length == 61 || _StrAry.Length == 62)
                {
                    _ColumnListViewScale = _StrAry[0];
                    _BeamListViewScale = _StrAry[1];
                    _TitleFont = _StrAry[2];
                    _ItemFont = _StrAry[3];
                    _DimensionType = _StrAry[4];
                    _FrameLineType = _StrAry[5];
                    _BodyLineType = _StrAry[6];
                    _SpacerLineType = _StrAry[7];
                    int.TryParse(_StrAry[8], out _LevelFrameShow);
                    _LevelFrameWidth = _StrAry[9];
                    _ItemFrameWidth = _StrAry[10];
                    _SymbolFrameHeight = _StrAry[11];
                    _ArrangementFrameHeight = _StrAry[12];
                    _ItemFrameWidth2 = _StrAry[13];
                    _SymbolFrameHeight2 = _StrAry[14];
                    _ArrangementFrameHeight2 = _StrAry[15];
                    _LevelFrameTitle = _StrAry[16];
                    _LevelFrameEndWord = _StrAry[17];
                    _SymbolFrameTitle = _StrAry[18];

                    _FullTableName = _StrAry[19];
                    _PickTable = _StrAry[20];

                    if (System.IO.File.Exists(_FullTableName)) {
                        _TableFileDirectory = _FullTableName.Substring(0, _FullTableName.LastIndexOf("\\"));
                        _TableFileName = _FullTableName.Substring(_FullTableName.LastIndexOf("\\") + 1);
                    }
                    else {
                        // マイドキュメント
                        string myDoc = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

                        string defFolder = myDoc + "\\" + _CmpAttribute.ResourceText("IDS_TXT_ADSKREXJ") + "\\" + version;
                        if (System.IO.Directory.Exists(defFolder)) {
                            _TableFileDirectory = defFolder;

                            string defFile = defFolder + "\\" + _CmpAttribute.ResourceText($"IDS_TXT_TABLEFILE_{version}");

                            if (System.IO.File.Exists(defFile)) {
                                _TableFileName = _CmpAttribute.ResourceText($"IDS_TXT_TABLEFILE_{version}");

                                _FullTableName = defFile;
                            }
                        }
                        else {
                            string defFolder_ADSK = myDoc + "\\" + _CmpAttribute.ResourceText("IDS_TXT_ADSKREXJ");
                            if (System.IO.Directory.Exists(defFolder_ADSK) == false) {
                                System.IO.Directory.CreateDirectory(defFolder_ADSK);
                            }

                            System.IO.Directory.CreateDirectory(defFolder);

                            // 実行フォルダ
                            string reservePath = _CmpAttribute.ExecuteFolder;

                            // テーブルファイル
                            string tableFile = _CmpAttribute.ResourceText($"IDS_TXT_TABLEFILE_{version}");
                            
                            // マッピングパラメータファイル
                            string mapParamFile = _CmpAttribute.ResourceText($"IDS_TXT_PARAMETERFILE_NAME_{version}");

                            // 共有パラメータファイル
                            string shareFile = _CmpAttribute.ResourceText("IDS_TXT_SHAREFILE");

                            // 共有パラメータファイル - オリジナル
                            string shareFileOrg = _CmpAttribute.ResourceText("IDS_TXT_SHAREFILE_ORG");

                            // テーブルコピー
                            if (System.IO.File.Exists(reservePath + "\\" + tableFile)) {
                                System.IO.File.Copy(reservePath + "\\" + tableFile,
                                                    defFolder + "\\" + tableFile);
                            }

                            // マッピングファイルコピー
                            if (System.IO.File.Exists(reservePath + "\\" + mapParamFile)) {
                                System.IO.File.Copy(reservePath + "\\" + mapParamFile,
                                                    defFolder + "\\" + mapParamFile);
                            }

                            // 共有パラメータファイルコピー
                            if (System.IO.File.Exists(reservePath + "\\" + shareFile)) {
                                System.IO.File.Copy(reservePath + "\\" + shareFile,
                                                    defFolder + "\\" + shareFile);
                            }

                            // 共有パラメータオリジナルファイルコピー
                            if (System.IO.File.Exists(reservePath + "\\" + shareFileOrg)) {
                                System.IO.File.Copy(reservePath + "\\" + shareFileOrg,
                                                    defFolder + "\\" + shareFileOrg);
                            }

                            _TableFileDirectory = defFolder;

                            string defFile = defFolder + "\\" + tableFile;

                            _TableFileName = _CmpAttribute.ResourceText($"IDS_TXT_TABLEFILE_{version}");

                            _FullTableName = defFile;
                        }
                    }

                    if (_PickTable == "0") {
                        this.chkBoxPickTable.Checked = false;
                    }
                    else if (_PickTable == "1") {
                        this.chkBoxPickTable.Checked = true;
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>コンボボックス - ドロップダウン幅設定</summary>
        ///
        /// <history><p>2013/02/13 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private
        void SetCmbBoxDropWidth(System.Windows.Forms.ComboBox cmbBox)
        {
            if (cmbBox.Items.Count > 0) {
                System.Drawing.Graphics graphics = this.CreateGraphics();

                float maxWidth = 0;

                // 最大幅取得
                foreach (object item in cmbBox.Items) {
                    maxWidth = System.Math.Max(maxWidth, graphics.MeasureString(item.ToString(), cmbBox.Font).Width);
                }

                // 余白
                maxWidth += 15;

                // 切り上げ、int型に変換
                int newWidth = (int)System.Math.Ceiling((decimal)maxWidth);

                // ドロップダウン幅の変更
                if (cmbBox.DropDownWidth < newWidth) {
                    cmbBox.DropDownWidth = newWidth;
                }
            }
        }

        /// ================================================================================
        /// <summary>文字列の小数値判定</summary>
        ///
        /// <history><p>2013/02/26 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private
        bool IsDoubleString(string strVal)
        {
            bool ret = false;

            double outDouble = 0;

            if (double.TryParse(strVal, out outDouble)) {
                if (outDouble != 0 && outDouble != 0.0 && outDouble <= 100) {
                    ret = true;
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>文字列の整数値判定</summary>
        ///
        /// <history><p>2013/02/26 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private
        bool IsIntString(string strVal, int mode)
        {
            bool ret = false;

            int outInt = 0;

            if (int.TryParse(strVal, out outInt)) {
                if (outInt > 10) {
                    ret = false;
                    return ret;
                }

                if (mode == 0) {
                    ret = true;
                }
                else {
                    if (outInt != 0) {
                        ret = true;
                    }
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>ボタン使用可否切り替え</summary>
        ///
        /// <history><p>2013/02/26 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private
        void BtnEnabledChange(bool enableBool)
        {
            this.btnOverWriteSave.Enabled = enableBool;
            this.btnToColumnListSettingFromSetting.Enabled = enableBool;
            this.btnToBeamListSetting1FromSetting.Enabled = enableBool;
        }

        /// ================================================================================
        /// <summary>コントロールの入力判定</summary>
        ///
        /// <param name="ctrl">テキストボックス、コンボボックス、ラジオボタン</param>
        ///
        /// <history><p>2013/02/27 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private
        bool InputJudge(System.Windows.Forms.Control ctrl)
        {
            bool ret = true;

            if (ctrl is System.Windows.Forms.TextBox) {
                System.Windows.Forms.TextBox txtBox = (System.Windows.Forms.TextBox)ctrl;

                if (txtBox.Text == "" || txtBox.Text == null) {
                    ret = false;
                }
            }
            else if (ctrl is System.Windows.Forms.ComboBox) {
                System.Windows.Forms.ComboBox cmbBox = (System.Windows.Forms.ComboBox)ctrl;

                if (cmbBox.SelectedItem == null) {
                    ret = false;
                }
            }
            else if (ctrl is System.Windows.Forms.RadioButton) {
                // 親をとって、親に含まれるラジオボタンを取得し、チェック
                System.Windows.Forms.RadioButton rdoBtn = (System.Windows.Forms.RadioButton)ctrl;

                System.Windows.Forms.Control.ControlCollection ctrlCollection = rdoBtn.Parent.Controls;

                bool check = false;

                foreach (System.Windows.Forms.Control cont in ctrlCollection) {
                    if (cont is System.Windows.Forms.RadioButton) {
                        System.Windows.Forms.RadioButton rb = (System.Windows.Forms.RadioButton)cont;
                        check = rb.Checked;

                        if (check == true) {
                            break;
                        }
                    }
                }

                if (check == false) {
                    ret = false;
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>コントロール内全コントロール</summary>
        ///
        /// <history><p>2013/02/27 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private
        System.Windows.Forms.Control[] GetCtrls(System.Windows.Forms.Control ctrl)
        {
            Collections.ArrayList ret = new Collections.ArrayList();

            foreach (System.Windows.Forms.Control c in ctrl.Controls) {
                ret.Add(c);
                ret.AddRange(GetCtrls(c));
            }

            return (System.Windows.Forms.Control[])ret.ToArray(typeof(System.Windows.Forms.Control));
        }

        /// ================================================================================
        /// <summary>未入力の入力</summary>
        ///
        /// <history><p>2013/05/16 Created GSA,Inc Ryo Kuroda</p>
        ///           <p>2015/04/27 Modified GSA, Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        private
        void SetUnWrite()
        {
            if (this.cmbBoxColumnListViewScale.SelectedItem == null) {
                if (this.cmbBoxColumnListViewScale.Items.Count > 0) {
                    this.cmbBoxColumnListViewScale.SelectedIndex = 0;
                }
                if (this.cmbBoxColumnListViewScale.Items.Count > 1) {
                    this.cmbBoxColumnListViewScale.SelectedIndex = 1;
                }
            }
            if (this.cmbBoxBeamListViewScale.SelectedItem == null) {
                if (this.cmbBoxBeamListViewScale.Items.Count > 0) {
                    this.cmbBoxBeamListViewScale.SelectedIndex = 0;
                }
                if (this.cmbBoxBeamListViewScale.Items.Count > 1) {
                    this.cmbBoxBeamListViewScale.SelectedIndex = 1;
                }
            }
            if (this.cmbBoxTitleFont.SelectedItem == null) {
                if (this.cmbBoxTitleFont.Items.Count > 0) {
                    this.cmbBoxTitleFont.SelectedIndex = 0;
                }
            }
            if (this.cmbBoxOtherFont.SelectedItem == null) {
                if (this.cmbBoxOtherFont.Items.Count > 0) {
                    this.cmbBoxOtherFont.SelectedIndex = 0;
                }
            }

            if (this.cmbBoxDimensionType.SelectedItem == null) {
                if (this.cmbBoxDimensionType.Items.Count > 0) {
                    this.cmbBoxDimensionType.SelectedIndex = 0;
                }
            }

            if (this.cmbBoxLineTypeFrame.SelectedItem == null) {
                if (this.cmbBoxLineTypeFrame.Items.Count > 0) {
                    this.cmbBoxLineTypeFrame.SelectedIndex = 0;
                }
            }
            if (this.cmbBoxLineTypeBody.SelectedItem == null) {
                if (this.cmbBoxLineTypeBody.Items.Count > 0) {
                    this.cmbBoxLineTypeBody.SelectedIndex = 0;
                }
            }
            if (this.cmbBoxLineTypeStoper.SelectedItem == null) {
                if (this.cmbBoxLineTypeStoper.Items.Count > 0) {
                    this.cmbBoxLineTypeStoper.SelectedIndex = 0;
                }
            }
            if (this.txtBoxLvlFrameWidth.Text == "" || this.txtBoxLvlFrameWidth.Text == null) {
                this.txtBoxLvlFrameWidth.Text = "12.5";
            }
            if (this.txtBoxLvlFrameTitle.Text == "" || this.txtBoxLvlFrameTitle.Text == null) {
                this.txtBoxLvlFrameTitle.Text = "Level";
            }
            //if (this.txtBoxLvlFrameEndWord.Text == null)
            //if (this.txtBoxLvlFrameEndWord.Text == "" || this.txtBoxLvlFrameEndWord.Text == null)
            //{
            //  this.txtBoxLvlFrameEndWord.Text = "F";
            //}

            if (this.txtBoxItemFrameWidth.Text == "" || this.txtBoxItemFrameWidth.Text == null) {
                this.txtBoxItemFrameWidth.Text = "12.5";
            }

            if (this.txtBoxArrangementFrameHeight.Text == "" || this.txtBoxArrangementFrameHeight.Text == null) {
                this.txtBoxArrangementFrameHeight.Text = "4.5";
            }

            if (this.txtBoxSymbolFrameTitle.Text == "" || this.txtBoxSymbolFrameTitle.Text == null) {
                this.txtBoxSymbolFrameTitle.Text = "Mark";
            }
            if (this.txtBoxSymbolFrameHeight.Text == "" || this.txtBoxSymbolFrameHeight.Text == null) {
                this.txtBoxSymbolFrameHeight.Text = "9";
            }

            if (this.txtBoxItemFrameWidth2.Text == "" || this.txtBoxItemFrameWidth2.Text == null) {
                this.txtBoxItemFrameWidth2.Text = "12.5";
            }

            if (this.txtBoxArrangementFrameHeight2.Text == "" || this.txtBoxArrangementFrameHeight2.Text == null) {
                this.txtBoxArrangementFrameHeight2.Text = "4.5";
            }

            if (this.txtBoxSymbolFrameHeight2.Text == "" || this.txtBoxSymbolFrameHeight2.Text == null) {
                this.txtBoxSymbolFrameHeight2.Text = "9";
            }

            if (this.rdoBtnLvlFrameShow.Checked == false && this.rdoBtnLvlFrameShowInItemFrame.Checked == false) {
                this.rdoBtnLvlFrameShow.Checked = true;
            }
        }

        /// ================================================================================
        /// <summary>設定ファイルの中身</summary>
        ///
        /// <history><p>2013/10/04 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private
        bool IsSettingFileRight(string fullName)
        {
            bool ret = false;

            if (System.IO.File.Exists(fullName)) {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                System.Text.Encoding enc = System.Text.Encoding.GetEncoding("shift_jis");

                string[] strs = System.IO.File.ReadAllLines(fullName, enc);

                if (strs.Length == 63 || strs.Length == 64) {
                    ret = true;
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>画像サイズ補正</summary>
        ///
        /// <history><p>2015/04/30 Created GSA,Inc Ryo Kuroda</p>
        ///           <p>2015/05/01 Modified GSA, Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        private
        void SetDPISizing()
        {
            // サイズ補正
            System.Drawing.Graphics gra = this.CreateGraphics();
            float dpiX = gra.DpiX;
            float dpiY = gra.DpiY;

            Bitmap bmp1 = Resources.Image.LevelFrameSetting_Show;
            Bitmap bmp2 = Resources.Image.LevelFrameSetting_InItemFrame;

            // 係数
            double coefficientX = dpiX / 96;
            double coefficientY = dpiY / 96;

            this.pictureBoxLvlFrame.SizeMode = PictureBoxSizeMode.AutoSize;

            Bitmap newBmp = new Bitmap((int)(bmp1.Width * coefficientX), (int)(bmp1.Height * coefficientY));
            this.pictureBoxLvlFrame.Image = newBmp;
            Graphics g = Graphics.FromImage(this.pictureBoxLvlFrame.Image);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.Clear(this.pictureBoxLvlFrame.BackColor);

            g.DrawImage(bmp1, 3, 3, (float)(bmp1.Width * coefficientX), (float)(bmp1.Height * coefficientY));
            this.pictureBoxLvlFrame.Refresh();

            this.pictureBoxLvlFrameInItemFrame.SizeMode = PictureBoxSizeMode.AutoSize;

            newBmp = new Bitmap((int)(bmp2.Width * coefficientX), (int)(bmp2.Height * coefficientY));
            this.pictureBoxLvlFrameInItemFrame.Image = newBmp;
            g = Graphics.FromImage(this.pictureBoxLvlFrameInItemFrame.Image);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.Clear(this.pictureBoxLvlFrameInItemFrame.BackColor);

            g.DrawImage(bmp2, 3, 3, (float)(bmp2.Width * coefficientX), (float)(bmp2.Height * coefficientY));
            this.pictureBoxLvlFrameInItemFrame.Refresh();
        }

        #endregion Member Functions

        // プロパティ
        #region Properties

        /// ================================================================================
        /// <summary>操作結果 - 設定</summary>
        /// ================================================================================
        public int SettingResult
        {
            get
            {
                return _Result;
            }
        }

        /// ================================================================================
        /// <summary>全項目の入力判定</summary>
        ///
        /// <history><p>2013/05 Created GSA, inc. Ryo Kuroda</p>
        ///           <p>2015/04/27 Modified GSA, Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        private bool AllInputJudge
        {
            get
            {
                bool ret = true;

                foreach (System.Windows.Forms.Control ctrl in GetCtrls(this)) {
                    // テキストボックス
                    if (ctrl is System.Windows.Forms.TextBox) {
                        System.Windows.Forms.TextBox txtBox = (System.Windows.Forms.TextBox)ctrl;

                        // 階表示枠接尾語
                        if (txtBox.Name == this.txtBoxLvlFrameEndWord.Name) {
                            continue;
                        }
                        else {
                            if (txtBox.Text == "" || txtBox.Text == null) {
                                ret = false;
                                break;
                            }
                        }
                    }
                    // コンボボックス
                    else if (ctrl is System.Windows.Forms.ComboBox) {
                        System.Windows.Forms.ComboBox cmbBox = (System.Windows.Forms.ComboBox)ctrl;

                        if (cmbBox.SelectedItem == null) {
                            ret = false;
                            break;
                        }
                    }
                    // ラジオボタン
                    else if (ctrl is System.Windows.Forms.RadioButton) {
                        // 親をとって、親に含まれるラジオボタンを取得し、チェック
                        System.Windows.Forms.RadioButton rdoBtn = (System.Windows.Forms.RadioButton)ctrl;

                        System.Windows.Forms.Control.ControlCollection ctrls = rdoBtn.Parent.Controls;

                        bool check = false;

                        foreach (System.Windows.Forms.Control cont in ctrls) {
                            if (cont is System.Windows.Forms.RadioButton) {
                                System.Windows.Forms.RadioButton rb = (System.Windows.Forms.RadioButton)cont;
                                check = rb.Checked;

                                if (check == true) {
                                    break;
                                }
                            }
                        }

                        if (check == false) {
                            ret = false;
                            break;
                        }
                    }
                }

                return ret;
            }
        }

        /// ================================================================================
        /// <summary>設定ファイル名</summary>
        /// ================================================================================
        public string SettingFileName
        {
            get
            {
                return _SettingFileName;
            }
        }

        /// ================================================================================
        /// <summary>設定ファイルディレクトリ</summary>
        /// ================================================================================
        public string SettingFileDirectory
        {
            get
            {
                return _SettingFileDirectory;
            }
        }

        /// ================================================================================
        /// <summary>設定値 - 共通設定</summary>
        /// ================================================================================
        public Collections.Generic.IList<string> SettingValues_Common
        {
            get
            {
                SetUnWrite();

                Collections.Generic.IList<string> ret = new Collections.Generic.List<string>();

                string columnScale = this.cmbBoxColumnListViewScale.SelectedItem.ToString();
                string beamScale = this.cmbBoxBeamListViewScale.SelectedItem.ToString();
                columnScale = columnScale.Substring(columnScale.IndexOf(":") + 2);
                beamScale = beamScale.Substring(beamScale.IndexOf(":") + 2);

                int lvlFrameShow = 0;
                if (this.rdoBtnLvlFrameShowInItemFrame.Checked == true) {
                    lvlFrameShow = 1;
                }

                ret.Add(columnScale);
                ret.Add(beamScale);
                ret.Add(this.cmbBoxTitleFont.SelectedItem.ToString());
                ret.Add(this.cmbBoxOtherFont.SelectedItem.ToString());
                ret.Add(this.cmbBoxDimensionType.SelectedItem.ToString());
                ret.Add(this.cmbBoxLineTypeFrame.SelectedItem.ToString());
                ret.Add(this.cmbBoxLineTypeBody.SelectedItem.ToString());
                ret.Add(this.cmbBoxLineTypeStoper.SelectedItem.ToString());
                ret.Add(lvlFrameShow.ToString());
                ret.Add(this.txtBoxLvlFrameWidth.Text);
                ret.Add(this.txtBoxItemFrameWidth.Text);
                ret.Add(this.txtBoxSymbolFrameHeight.Text);
                ret.Add(this.txtBoxArrangementFrameHeight.Text);
                ret.Add(this.txtBoxItemFrameWidth2.Text);
                ret.Add(this.txtBoxSymbolFrameHeight2.Text);
                ret.Add(this.txtBoxArrangementFrameHeight2.Text);
                ret.Add(this.txtBoxLvlFrameTitle.Text);
                ret.Add(this.txtBoxLvlFrameEndWord.Text);
                ret.Add(this.txtBoxSymbolFrameTitle.Text);
                ret.Add(_FullTableName);
                ret.Add(_PickTable);

                return ret;
            }
        }

        #endregion Properties

        // イベント
        #region Events

        #region ロード

        private void FormSetting_Load(object sender, EventArgs e)
        {
            SetText();
            SetSettingValue();

            if (_PickTable == "0") {
                this.grpBoxSelectTable.Enabled = false;
                this.lblCurrentTable.Enabled = false;
                this.lblCurrentTableName.Enabled = false;

                this.chkBoxPickTable.Enabled = true;
            }
            if (_PickTable == "1") {
                this.grpBoxSelectTable.Enabled = true;
                this.lblCurrentTable.Enabled = true;
                this.lblCurrentTableName.Enabled = true;

                this.chkBoxPickTable.Enabled = true;
            }

            this.cmbBoxColumnListViewScale.Select();

            SetUnWrite();

            BtnEnabledChange(AllInputJudge);

            SetDPISizing();
        }

        #endregion ロード

        #region 入力制限

        private void txtBoxLvlFrameWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.') {
                e.Handled = true;
            }
        }

        private void txtBoxItemFrameWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.') {
                e.Handled = true;
            }
        }

        private void txtBoxSymbolFrameHeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.') {
                e.Handled = true;
            }
        }

        private void txtBoxArrangementFrameHeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.') {
                e.Handled = true;
            }
        }

        private void txtBoxItemFrameWidth2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.') {
                e.Handled = true;
            }
        }

        private void txtBoxSymbolFrameHeight2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.') {
                e.Handled = true;
            }
        }

        private void txtBoxArrangementFrameHeight2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.') {
                e.Handled = true;
            }
        }

        #endregion 入力制限

        #region Changed イベント

        // 文字タイプ
        // タイトル
        private void cmbBoxTitleFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            BtnEnabledChange(AllInputJudge);
        }

        // その他
        private void cmbBoxOtherFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            BtnEnabledChange(AllInputJudge);
        }

        // 寸法タイプ
        private void cmbBoxDimensionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            BtnEnabledChange(AllInputJudge);
        }

        // 線種タイプ
        // 枠
        private void cmbBoxLineTypeFrame_SelectedIndexChanged(object sender, EventArgs e)
        {
            BtnEnabledChange(AllInputJudge);
        }

        // 躯体
        private void cmbBoxLineTypeBody_SelectedIndexChanged(object sender, EventArgs e)
        {
            BtnEnabledChange(AllInputJudge);
        }

        // 幅止筋
        private void cmbBoxLineTypeStoper_SelectedIndexChanged(object sender, EventArgs e)
        {
            BtnEnabledChange(AllInputJudge);
        }

        #region 階表示枠の表示方法

        // 表示ボタン
        private void rdoBtnLvlFrameShow_CheckedChanged(object sender, EventArgs e)
        {
            BtnEnabledChange(AllInputJudge);

            if (this.rdoBtnLvlFrameShow.Checked == true) {
                // 表示パネル
                System.Windows.Forms.Control.ControlCollection ctrlsLvlFrameShow = this.pnlLvlFrameShow.Controls;
                int ctrlCount = ctrlsLvlFrameShow.Count;

                for (int i = 0; i < ctrlCount; i++) {
                    System.Windows.Forms.Control ctrl = ctrlsLvlFrameShow[ctrlCount - (i + 1)];

                    if (ctrl is System.Windows.Forms.TextBox) {
                        System.Windows.Forms.TextBox txtBox = (System.Windows.Forms.TextBox)ctrl;
                        txtBox.Enabled = true;

                        if (!IsDoubleString(txtBox.Text)) {
                            this.errorProviderInvalid.SetError(txtBox, _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") +
                                                               "\r\n" +
                                                               _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLE"));
                            txtBox.Select();
                            txtBox.SelectAll();

                            BtnEnabledChange(false);
                        }
                        else {
                            this.errorProviderInvalid.SetError(txtBox, "");
                        }
                    }
                }

                // 項目表示枠に表示パネル
                System.Windows.Forms.Control.ControlCollection ctrlsInItemFrame = this.pnlLvlFrameShowInItemFrame.Controls;
                ctrlCount = ctrlsInItemFrame.Count;

                for (int i = 0; i < ctrlCount; i++) {
                    System.Windows.Forms.Control ctrl = ctrlsInItemFrame[ctrlCount - (i + 1)];

                    if (ctrl is System.Windows.Forms.TextBox) {
                        System.Windows.Forms.TextBox txtBox = (System.Windows.Forms.TextBox)ctrl;
                        this.errorProviderInvalid.SetError(txtBox, "");
                        txtBox.Enabled = false;
                    }
                }

                this.toolTipEnable.SetToolTip(this.pictureBoxLvlFrame, "");

                this.toolTipEnable.SetToolTip(this.pictureBoxLvlFrameInItemFrame,
                                              "「" +
                                              _CmpAttribute.ResourceText("IDS_TXT_LEVELFRAMESHOW") +
                                              "」" +
                                              _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUT"));
            }
        }

        // 項目表示枠に表示ボタン
        private void rdoBtnLvlFrameShowInItemFrame_CheckedChanged(object sender, EventArgs e)
        {
            BtnEnabledChange(AllInputJudge);

            if (this.rdoBtnLvlFrameShowInItemFrame.Checked == true) {
                // 表示パネル
                System.Windows.Forms.Control.ControlCollection ctrlsLvlFrameShow = this.pnlLvlFrameShow.Controls;
                int ctrlCount = ctrlsLvlFrameShow.Count;

                for (int i = 0; i < ctrlCount; i++) {
                    System.Windows.Forms.Control ctrl = ctrlsLvlFrameShow[ctrlCount - (i + 1)];

                    if (ctrl is System.Windows.Forms.TextBox) {
                        System.Windows.Forms.TextBox txtBox = (System.Windows.Forms.TextBox)ctrl;
                        this.errorProviderInvalid.SetError(txtBox, "");
                        txtBox.Enabled = false;
                    }
                }

                // 項目表示枠に表示パネル
                System.Windows.Forms.Control.ControlCollection ctrlsInItemFrame = this.pnlLvlFrameShowInItemFrame.Controls;
                ctrlCount = ctrlsInItemFrame.Count;

                for (int i = 0; i < ctrlCount; i++) {
                    System.Windows.Forms.Control ctrl = ctrlsInItemFrame[ctrlCount - (i + 1)];

                    if (ctrl is System.Windows.Forms.TextBox) {
                        System.Windows.Forms.TextBox txtBox = (System.Windows.Forms.TextBox)ctrl;
                        txtBox.Enabled = true;

                        if (!IsDoubleString(txtBox.Text)) {
                            this.errorProviderInvalid.SetError(txtBox, _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") +
                                                               "\r\n" +
                                                               _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLE"));
                            txtBox.Select();
                            txtBox.SelectAll();

                            BtnEnabledChange(false);
                        }
                        else {
                            this.errorProviderInvalid.SetError(txtBox, "");
                        }
                    }
                }

                this.toolTipEnable.SetToolTip(this.pictureBoxLvlFrame,
                                              "「" +
                                              _CmpAttribute.ResourceText("IDS_TXT_LEVELFRAMESHOWINITEMFRAME") +
                                              "」" +
                                              _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUT"));

                this.toolTipEnable.SetToolTip(this.pictureBoxLvlFrameInItemFrame, "");
            }
        }

        #endregion 階表示枠の表示方法

        // 階表示枠タイトル
        private void txtBoxLvlFrameTitle_TextChanged(object sender, EventArgs e)
        {
            if (AllInputJudge) {
                BtnEnabledChange(true);
            }
            else {
                BtnEnabledChange(false);
            }
        }

        // 階表示枠接尾語
        private void txtBoxLvlFrameEndWord_TextChanged(object sender, EventArgs e)
        {
            if (AllInputJudge) {
                BtnEnabledChange(true);
            }
            else {
                BtnEnabledChange(false);
            }
        }

        // 符号表示枠タイトル
        private void txtBoxSymbolFrameTitle_TextChanged(object sender, EventArgs e)
        {
            if (AllInputJudge) {
                BtnEnabledChange(true);
            }
            else {
                BtnEnabledChange(false);
            }
        }

        // 階表示枠幅
        private void txtBoxLvlFrameWidth_TextChanged(object sender, EventArgs e)
        {
            if (IsDoubleString(this.txtBoxLvlFrameWidth.Text)) {
                this.errorProviderInvalid.SetError(this.txtBoxLvlFrameWidth, "");

                if (AllInputJudge) {
                    BtnEnabledChange(true);
                }
            }
        }

        // 項目表示枠幅
        private void txtBoxItemFrameWidth_TextChanged(object sender, EventArgs e)
        {
            if (IsDoubleString(this.txtBoxItemFrameWidth.Text)) {
                this.errorProviderInvalid.SetError(this.txtBoxItemFrameWidth, "");

                if (AllInputJudge) {
                    BtnEnabledChange(true);
                }
            }
        }

        // 符号表示枠高さ
        private void txtBoxSymbolFrameHeight_TextChanged(object sender, EventArgs e)
        {
            if (IsDoubleString(this.txtBoxSymbolFrameHeight.Text)) {
                this.errorProviderInvalid.SetError(this.txtBoxSymbolFrameHeight, "");

                if (AllInputJudge) {
                    BtnEnabledChange(true);
                }
            }
        }

        // 配筋枠高さ
        private void txtBoxArrangementFrameHeight_TextChanged(object sender, EventArgs e)
        {
            this.errorProviderInvalid.SetError(this.txtBoxArrangementFrameHeight, "");

            if (IsDoubleString(this.txtBoxArrangementFrameHeight.Text)) {
                if (AllInputJudge) {
                    BtnEnabledChange(true);
                }
            }
        }

        // 項目表示枠幅2
        private void txtBoxItemFrameWidth2_TextChanged(object sender, EventArgs e)
        {
            this.errorProviderInvalid.SetError(this.txtBoxItemFrameWidth2, "");

            if (IsDoubleString(this.txtBoxItemFrameWidth2.Text)) {
                if (AllInputJudge) {
                    BtnEnabledChange(true);
                }
            }
        }

        // 符号表示枠高さ2
        private void txtBoxSymbolFrameHeight2_TextChanged(object sender, EventArgs e)
        {
            this.errorProviderInvalid.SetError(this.txtBoxSymbolFrameHeight2, "");

            if (IsDoubleString(this.txtBoxSymbolFrameHeight2.Text)) {
                if (AllInputJudge) {
                    BtnEnabledChange(true);
                }
            }
        }

        // 配筋枠高さ2
        private void txtBoxArrangementFrameHeight2_TextChanged(object sender, EventArgs e)
        {
            this.errorProviderInvalid.SetError(this.txtBoxArrangementFrameHeight2, "");

            if (IsDoubleString(this.txtBoxArrangementFrameHeight2.Text)) {
                if (AllInputJudge) {
                    BtnEnabledChange(true);
                }
            }
        }

        // テーブル指定許可切り替え
        private void chkBoxPickTable_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkBoxPickTable.Checked == true) {
                this.grpBoxSelectTable.Enabled = true;
                this.lblCurrentTable.Enabled = true;
                this.lblCurrentTableName.Enabled = true;

                this.chkBoxPickTable.Enabled = true;

                _PickTable = "1";
            }
            else {
                this.grpBoxSelectTable.Enabled = false;
                this.lblCurrentTable.Enabled = false;
                this.lblCurrentTableName.Enabled = false;

                this.chkBoxPickTable.Enabled = true;

                _PickTable = "0";
            }
        }

        #endregion Changed イベント

        #region 枠幅、高さのテキストボックスからコントロールが離れたとき

        // 階表示枠幅
        private void txtBoxLvlFrameWidth_Leave(object sender, EventArgs e)
        {
            if (!IsDoubleString(this.txtBoxLvlFrameWidth.Text) && this.rdoBtnLvlFrameShow.Checked) {
                this.errorProviderInvalid.SetError(this.txtBoxLvlFrameWidth, _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") +
                                                                             "\r\n" +
                                                                             _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLE"));
                this.txtBoxLvlFrameWidth.Select();
                this.txtBoxLvlFrameWidth.SelectAll();

                BtnEnabledChange(false);
            }
            else {
                this.errorProviderInvalid.SetError(this.txtBoxLvlFrameWidth, "");
            }
        }

        // 項目表示枠幅
        private void txtBoxItemFrameWidth_Leave(object sender, EventArgs e)
        {
            if (!IsDoubleString(this.txtBoxItemFrameWidth.Text) && this.rdoBtnLvlFrameShow.Checked) {
                this.errorProviderInvalid.SetError(this.txtBoxItemFrameWidth, _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") +
                                                                              "\r\n" +
                                                                              _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLE"));
                this.txtBoxItemFrameWidth.Select();
                this.txtBoxItemFrameWidth.SelectAll();

                BtnEnabledChange(false);
            }
            else {
                this.errorProviderInvalid.SetError(this.txtBoxItemFrameWidth, "");
            }
        }

        // 符号表示枠高さ
        private void txtBoxSymbolFrameHeight_Leave(object sender, EventArgs e)
        {
            if (!IsDoubleString(this.txtBoxSymbolFrameHeight.Text) && this.rdoBtnLvlFrameShow.Checked) {
                this.errorProviderInvalid.SetError(this.txtBoxSymbolFrameHeight, _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") +
                                                                                 "\r\n" +
                                                                                 _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLE"));
                this.txtBoxSymbolFrameHeight.Select();
                this.txtBoxSymbolFrameHeight.SelectAll();

                BtnEnabledChange(false);
            }
            else {
                this.errorProviderInvalid.SetError(this.txtBoxSymbolFrameHeight, "");
            }
        }

        // 配筋枠高さ
        private void txtBoxArrangementFrameHeight_Leave(object sender, EventArgs e)
        {
            if (!IsDoubleString(this.txtBoxArrangementFrameHeight.Text) && this.rdoBtnLvlFrameShow.Checked) {
                this.errorProviderInvalid.SetError(this.txtBoxArrangementFrameHeight, _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") +
                                                                                      "\r\n" +
                                                                                      _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLE"));
                this.txtBoxArrangementFrameHeight.Select();
                this.txtBoxArrangementFrameHeight.SelectAll();

                BtnEnabledChange(false);
            }
            else {
                this.errorProviderInvalid.SetError(this.txtBoxArrangementFrameHeight, "");
            }
        }

        // 項目表示枠幅2
        private void txtBoxItemFrameWidth2_Leave(object sender, EventArgs e)
        {
            if (!IsDoubleString(this.txtBoxItemFrameWidth2.Text) && this.rdoBtnLvlFrameShowInItemFrame.Checked) {
                this.errorProviderInvalid.SetError(this.txtBoxItemFrameWidth2, _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") +
                                                                               "\r\n" +
                                                                               _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLE"));
                this.txtBoxItemFrameWidth2.Select();
                this.txtBoxItemFrameWidth2.SelectAll();

                BtnEnabledChange(false);
            }
            else {
                this.errorProviderInvalid.SetError(this.txtBoxItemFrameWidth2, "");
            }
        }

        // 符号表示枠高さ2
        private void txtBoxSymbolFrameHeight2_Leave(object sender, EventArgs e)
        {
            if (!IsDoubleString(this.txtBoxSymbolFrameHeight2.Text) && this.rdoBtnLvlFrameShowInItemFrame.Checked) {
                this.errorProviderInvalid.SetError(this.txtBoxSymbolFrameHeight2, _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") +
                                                                                  "\r\n" +
                                                                                  _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLE"));
                this.txtBoxSymbolFrameHeight2.Select();
                this.txtBoxSymbolFrameHeight2.SelectAll();

                BtnEnabledChange(false);
            }
            else {
                this.errorProviderInvalid.SetError(this.txtBoxSymbolFrameHeight2, "");
            }
        }

        // 配筋枠高さ2
        private void txtBoxArrangementFrameHeight2_Leave(object sender, EventArgs e)
        {
            if (!IsDoubleString(this.txtBoxArrangementFrameHeight2.Text) && this.rdoBtnLvlFrameShowInItemFrame.Checked) {
                this.errorProviderInvalid.SetError(this.txtBoxArrangementFrameHeight2, _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") +
                                                                                       "\r\n" +
                                                                                       _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLE"));
                this.txtBoxArrangementFrameHeight2.Select();
                this.txtBoxArrangementFrameHeight2.SelectAll();

                BtnEnabledChange(false);
            }
            else {
                this.errorProviderInvalid.SetError(this.txtBoxArrangementFrameHeight2, "");
            }
        }

        #endregion 枠幅、高さのテキストボックスからコントロールが離れたとき

        #region ボタン

        // 設定ファイル読み込み
        private void btnReadSettingFile_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFile = new System.Windows.Forms.OpenFileDialog();
            openFile.Filter = "Text File (*.txt)|*.txt";
            openFile.Multiselect = false;
            openFile.Title = _CmpAttribute.ResourceText("IDS_TXT_READSETTINGFILE");
            if (_SettingFileDirectory != "") {
                openFile.InitialDirectory = _SettingFileDirectory;
            }

            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                _SettingFileName = openFile.SafeFileName;
                _FullName = openFile.FileName;
                _SettingFileDirectory = _FullName.Substring(0, _FullName.LastIndexOf(_SettingFileName));

                // 取得ファイルの内容確認
                bool isRight = false;
                bool isCancel = false;

                while (isRight == false && isCancel == false) {
                    // 正しいファイル
                    if (_SettingFileName != "SettingFlieInfo.txt") {
                        isRight = IsSettingFileRight(_FullName);
                    }

                    // 再選択
                    if (isRight == false) {
                        if (_SettingFileName == "SettingFlieInfo.txt") {
                            MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_SELOTHERFILE"));
                        }
                        else {
                            MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_SETTINGFILEINCORRECT") + "\r\n" + _CmpAttribute.ResourceText("IDS_ERR_SELOTHERFILEORRESET"));
                        }

                        if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                            _SettingFileName = openFile.SafeFileName;
                            _FullName = openFile.FileName;
                            _SettingFileDirectory = _FullName.Substring(0, _FullName.LastIndexOf(_SettingFileName));
                        }
                        else {
                            isCancel = true;
                        }
                    }
                }

                if (isRight == true) {
                    // 読み込み指定ファイル更新
                    this.DialogResult = System.Windows.Forms.DialogResult.Yes;
                    _Result = 5;

                    this.Close();

                    SetData();
                }
            }
        }

        // テーブル選択ボタン
        private void btnExportFolderSelect_Click(object sender, EventArgs e)
        {
            // テーブル選択
            System.Windows.Forms.OpenFileDialog opnFileDlg = new System.Windows.Forms.OpenFileDialog();
            opnFileDlg.Filter = "Table files (*.tbl)|*.tbl";
            opnFileDlg.Multiselect = false;
            opnFileDlg.Title = _CmpAttribute.ResourceText("IDS_TXT_TABLESELECT");

            // マイドキュメント
            string myDoc = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

            // 基準フォルダ
            if (System.IO.File.Exists(_FullTableName)) {
                opnFileDlg.InitialDirectory = _TableFileDirectory;
            }
            else {
                var version = _CmpAttribute.ResourceText( "IDS_TXT_REVITVERSION_2027" ) ;
                string defFolder = myDoc + "\\Autodesk REXJ\\" + version;
                if (System.IO.Directory.Exists(defFolder)) {
                    opnFileDlg.InitialDirectory = defFolder;
                }
            }

            if (opnFileDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                _FullTableName = opnFileDlg.FileName;
                _TableFileName = opnFileDlg.SafeFileName;
                _TableFileDirectory = _FullTableName.Substring(0, _FullTableName.LastIndexOf(_TableFileName));

                this.lblCurrentTableName.Text = _TableFileName;
            }
        }

        // 柱ビューリスト尺度
        private void cmbBoxCustomViewScale_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbBoxColumnListViewScale.SelectedIndex == 0) {
                FormCustomViewScale formCustom = new FormCustomViewScale(_CmpAttribute);
                formCustom.ShowDialog();

                if (formCustom.DialogResult == System.Windows.Forms.DialogResult.OK) {
                    this.cmbBoxColumnListViewScale.DropDownStyle = ComboBoxStyle.DropDown;

                    if (this.cmbBoxColumnListViewScale.Items.Count == 13) {
                        this.cmbBoxColumnListViewScale.Items.Add("");
                    }

                    this.cmbBoxColumnListViewScale.Items[13] = "1 : " + formCustom.CustomViewScale.ToString();
                    this.cmbBoxColumnListViewScale.SelectedIndex = 13;

                    this.cmbBoxColumnListViewScale.DropDownStyle = ComboBoxStyle.DropDownList;
                }
                else {
                    this.cmbBoxColumnListViewScale.SelectedIndex = _ColumnListViewScaleIndex;
                }
            }
            else {
                _ColumnListViewScaleIndex = this.cmbBoxColumnListViewScale.SelectedIndex;
            }

            BtnEnabledChange(AllInputJudge);
        }

        // 梁ビューリスト尺度
        private void cmbBoxBeamListViewScale_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbBoxBeamListViewScale.SelectedIndex == 0) {
                FormCustomViewScale formCustom = new FormCustomViewScale(_CmpAttribute);
                formCustom.ShowDialog();

                if (formCustom.DialogResult == System.Windows.Forms.DialogResult.OK) {
                    this.cmbBoxBeamListViewScale.DropDownStyle = ComboBoxStyle.DropDown;

                    if (this.cmbBoxBeamListViewScale.Items.Count == 13) {
                        this.cmbBoxBeamListViewScale.Items.Add("");
                    }

                    this.cmbBoxBeamListViewScale.Items[13] = "1 : " + formCustom.CustomViewScale.ToString();
                    this.cmbBoxBeamListViewScale.SelectedIndex = 13;

                    this.cmbBoxBeamListViewScale.DropDownStyle = ComboBoxStyle.DropDownList;
                }
                else {
                    this.cmbBoxBeamListViewScale.SelectedIndex = _BeamListViewScaleIndex;
                }
            }
            else {
                _BeamListViewScaleIndex = this.cmbBoxBeamListViewScale.SelectedIndex;
            }

            BtnEnabledChange(AllInputJudge);
        }

        // 柱リスト設定へ
        private void btnToColumnListSettingFromSetting_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            _Result = 1;

            this.Close();
        }

        // 梁リスト設定へ
        private void btnToBeamListSettingFromSetting_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            _Result = 2;

            this.Close();
        }

        // 上書き保存
        private void btnOverWriteSave_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            _Result = 6;

            this.Close();
        }

        // 名前を付けて保存
        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            _Result = 7;

            this.Close();
        }

        // 終了
        private void btnEnd_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;

            this.Close();
        }

        #endregion ボタン

        #endregion Events
    }
}
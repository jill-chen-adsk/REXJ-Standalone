using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using System.Reflection;

namespace SectionListSteel.Setting
{
    /// ================================================================================
    /// <summary>フォーム 共通設定</summary>
    /// ================================================================================
    public partial class FormCommonSetting : Form
    {
        // メンバ変数

        #region Member Variables

        /// <summary>属性</summary>
        private SectionListSteel.Components.Attribute _CmpAttribute;

        /// <summary>設定ファイル名</summary>
        private string _SettingFileName;

        /// <summary>フル設定ファイル名</summary>
        private string _FullName;

        /// <summary>設定ファイルディレクトリ</summary>
        private string _SettingFileDirectory;

        /// <summary>柱リストビュー尺度</summary>
        private string _ColumnListViewScale;

        /// <summary>梁リストビュー尺度</summary>
        private string _BeamListViewScale;

        /// <summary>タイトルフォント</summary>
        private string _TitleFont;

        /// <summary>鉄筋フォント</summary>
        private string _SteelFont;

        /// <summary>線種タイプ</summary>
        private string _LineType;

        /// <summary>タイトル表示</summary>
        private int _TitleShow;

        /// <summary>枠幅</summary>
        private string _FrameWidth;

        /// <summary>枠高さ</summary>
        private string _FrameHeight;

        /// <summary>枠幅2</summary>
        private string _FrameWidth2;

        /// <summary>枠高さ2</summary>
        private string _FrameHeight2;

        /// <summary>階表示枠タイトル</summary>
        private string _LvlFrameTitle;

        /// <summary>項目表示枠タイトル</summary>
        private string _SymbolFrameTitle;

        /// <summary>枠タイトル</summary>
        private string _FrameTitle;

        /// <summary>階表示枠接尾語</summary>
        private string _LvlFrameEndword;

        /// <summary>枠幅 二次部材</summary>
        private string _FrameWidthSub;

        /// <summary>枠高さ 二次部材</summary>
        private string _FrameHeightSub;

        /// <summary>二次部材タイトル</summary>
        private string _SubFrameTitle;

        /// <summary>柱ビュー尺度のインデックス</summary>
        private int _ColumnListViewScaleIndex;

        /// <summary>梁ビュー尺度のインデックス</summary>
        private int _BeamListViewScaleIndex;

        /// <summary>文字タイプ</summary>
        private Collections.Generic.IList<Revit.DB.TextNoteType> _TxtNoteTypes;

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

        /// <summary>全項目の入力判定</summary>
        private bool _InputJudge;

        /// <summary> Form all setting</summary>
        private FormAllSetting _FormAllSetting = null;

        #endregion Member Variables

        // コンストラクタ

        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute"        >属性</param>
        /// <param name="settingFileName"     >設定ファイル名</param>
        /// <param name="settingFileDirectory">設定ファイルディレクトリ</param>
        /// <param name="txtNoteTypes"        >文字タイプ</param>
        /// <param name="graStyles"           >線種タイプ</param>
        ///
        /// <history>2016/08/29 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        public FormCommonSetting(FormAllSetting parent, SectionListSteel.Components.Attribute cmpAttribute,
                                 string settingFileName,
                                 string settingFileDirectory,
                                 Collections.Generic.IList<Revit.DB.TextNoteType> txtNoteTypes,
                                 Collections.Generic.IList<Revit.DB.GraphicsStyle> graStyles)
        {
            InitializeComponent();
            _FormAllSetting = parent;

            _CmpAttribute = cmpAttribute;
            _SettingFileName = settingFileName;
            _SettingFileDirectory = settingFileDirectory;
            _TxtNoteTypes = txtNoteTypes;
            _GraStyles = graStyles;

            _PickTable = "0";

            SetData();
            SetSettingValue();
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>データ設定</summary>
        ///
        /// <history><p>2016/08/30 Created GSA,Inc Ryo Kuroda</p>
        ///           <p>2017/06/  Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        void SetData()
        {
            // フォント
            foreach (Revit.DB.TextNoteType tnp in _TxtNoteTypes)
            {
                this.cmbBoxTitleFont.Items.Add(tnp.Name);
                this.cmbBoxSteelFont.Items.Add(tnp.Name);
            }

            this.cmbBoxTitleFont.Sorted = true;
            this.cmbBoxSteelFont.Sorted = true;

            this.cmbBoxTitleFont.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbBoxSteelFont.DropDownStyle = ComboBoxStyle.DropDownList;

            SetCmbBoxDropWidth(this.cmbBoxTitleFont);
            SetCmbBoxDropWidth(this.cmbBoxSteelFont);

            // 線種
            foreach (Revit.DB.GraphicsStyle gs in _GraStyles)
            {
                this.cmbBoxLineTypeFrame.Items.Add(gs.Name);
            }

            this.cmbBoxLineTypeFrame.Sorted = true;

            this.cmbBoxLineTypeFrame.DropDownStyle = ComboBoxStyle.DropDownList;

            SetCmbBoxDropWidth(this.cmbBoxLineTypeFrame);

            // 右寄せ
            this.txtBoxFrameWidth.TextAlign = HorizontalAlignment.Right;
            this.txtBoxFrameHeight.TextAlign = HorizontalAlignment.Right;
            this.txtBoxFrameWidth2.TextAlign = HorizontalAlignment.Right;
            this.txtBoxFrameHeight2.TextAlign = HorizontalAlignment.Right;
            this.txtBoxSubWidth.TextAlign = HorizontalAlignment.Right;
            this.txtBoxSubHeight.TextAlign = HorizontalAlignment.Right;

            // 文字数
            this.txtBoxFrameWidth.MaxLength = 5;
            this.txtBoxFrameHeight.MaxLength = 5;
            this.txtBoxFrameWidth2.MaxLength = 5;
            this.txtBoxFrameHeight2.MaxLength = 5;
            this.txtBoxSubWidth.MaxLength = 5;
            this.txtBoxSubHeight.MaxLength = 5;
        }

        /// ================================================================================
        /// <summary>設定値設定</summary>
        ///
        /// <history>2016/08/30 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        public
        void SetSettingValue()
        {
            // 設定ファイルから設定値取得
            GetSettingValue();

            // 柱リストビュー尺度
            string str = "";
            if (_ColumnListViewScale != "" && _ColumnListViewScale != null)
            {
                str = "1 : " + _ColumnListViewScale;
            }
            else
            {
                str = "1 : 30";
            }

            bool isContain = false;

            for (int i = 0; i < this.cmbBoxColumnListViewScale.Items.Count; i++)
            {
                if ((string)this.cmbBoxColumnListViewScale.Items[i] == str)
                {
                    this.cmbBoxColumnListViewScale.SelectedIndex = i;
                    isContain = true;
                    break;
                }
            }
            if (isContain == false)
            {
                this.cmbBoxColumnListViewScale.DropDownStyle = ComboBoxStyle.DropDown;

                if (this.cmbBoxColumnListViewScale.Items.Count == 13)
                {
                    this.cmbBoxColumnListViewScale.Items.Add("");
                }

                this.cmbBoxColumnListViewScale.Items[13] = str;
                this.cmbBoxColumnListViewScale.SelectedIndex = 13;
            }

            this.cmbBoxColumnListViewScale.DropDownStyle = ComboBoxStyle.DropDownList;
            SetCmbBoxDropWidth(this.cmbBoxColumnListViewScale);

            _ColumnListViewScaleIndex = this.cmbBoxColumnListViewScale.SelectedIndex;

            // 梁リストビュー尺度
            str = "";
            if (_BeamListViewScale != "" && _BeamListViewScale != null)
            {
                str = "1 : " + _BeamListViewScale;
            }
            else
            {
                str = "1 : 30";
            }

            isContain = false;

            for (int i = 0; i < this.cmbBoxBeamListViewScale.Items.Count; i++)
            {
                if ((string)this.cmbBoxBeamListViewScale.Items[i] == str)
                {
                    this.cmbBoxBeamListViewScale.SelectedIndex = i;
                    isContain = true;
                    break;
                }
            }
            if (isContain == false)
            {
                this.cmbBoxBeamListViewScale.DropDownStyle = ComboBoxStyle.DropDown;

                if (this.cmbBoxBeamListViewScale.Items.Count == 13)
                {
                    this.cmbBoxBeamListViewScale.Items.Add("");
                }

                this.cmbBoxBeamListViewScale.Items[13] = str;
                this.cmbBoxBeamListViewScale.SelectedIndex = 13;
            }

            this.cmbBoxBeamListViewScale.DropDownStyle = ComboBoxStyle.DropDownList;
            SetCmbBoxDropWidth(this.cmbBoxBeamListViewScale);

            _BeamListViewScaleIndex = this.cmbBoxBeamListViewScale.SelectedIndex;

            // フォント
            for (int i = 0; i < this.cmbBoxTitleFont.Items.Count; i++)
            {
                if ((string)this.cmbBoxTitleFont.Items[i] == _TitleFont)
                {
                    this.cmbBoxTitleFont.SelectedIndex = i;
                }
                if ((string)this.cmbBoxSteelFont.Items[i] == _SteelFont)
                {
                    this.cmbBoxSteelFont.SelectedIndex = i;
                }
            }
            if (this.cmbBoxTitleFont.SelectedItem == null)
            {
                this.cmbBoxTitleFont.SelectedIndex = 0;
            }
            if (this.cmbBoxSteelFont.SelectedItem == null)
            {
                this.cmbBoxSteelFont.SelectedIndex = 0;
            }

            // 線種
            for (int i = 0; i < this.cmbBoxLineTypeFrame.Items.Count; i++)
            {
                if ((string)this.cmbBoxLineTypeFrame.Items[i] == _LineType)
                {
                    this.cmbBoxLineTypeFrame.SelectedIndex = i;
                }
            }
            if (this.cmbBoxLineTypeFrame.SelectedItem == null)
            {
                this.cmbBoxLineTypeFrame.SelectedIndex = 0;
            }

            // タイトル表示
            if (_TitleShow == 0)
            {
                this.rdoBtn2Title.Checked = true;
            }
            else
            {
                this.rdoBtn1Title.Checked = true;
            }

            // 枠サイズ
            this.txtBoxFrameWidth.Text = _FrameWidth;
            this.txtBoxFrameHeight.Text = _FrameHeight;
            this.txtBoxFrameWidth2.Text = _FrameWidth2;
            this.txtBoxFrameHeight2.Text = _FrameHeight2;
            this.txtBoxSubWidth.Text = _FrameWidthSub;
            this.txtBoxSubHeight.Text = _FrameHeightSub;

            // 文字
            this.txtBoxLvlFrameTitle.Text = _LvlFrameTitle;
            this.txtBoxSymbolFrameTitle.Text = _SymbolFrameTitle;
            this.txtBoxFrameTitle.Text = _FrameTitle;
            this.txtBoxLvlFrameEndWord.Text = _LvlFrameEndword;
            this.txtBoxSubFrameTitle.Text = _SubFrameTitle;

            // テーブル指定
            if (_PickTable == "0")
            {
                this.chkBoxPickTable.Checked = false;
            }
            else if (_PickTable == "1")
            {
                this.chkBoxPickTable.Checked = true;
            }

            this.lblCurrentTableName.Text = _TableFileName;
        }

        /// ================================================================================
        /// <summary>設定値取得</summary>
        ///
        /// <history><p>2016/08/30 Created GSA,Inc Ryo Kuroda</p>
        ///           <p>2017/07/31  Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        void GetSettingValue()
        {
            string fullName = _SettingFileDirectory + _SettingFileName;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            System.Text.Encoding enc = System.Text.Encoding.GetEncoding("shift_jis");

            if (System.IO.File.Exists(fullName))
            {
                string[] strAry = System.IO.File.ReadAllLines(fullName, enc);

                _ColumnListViewScale = strAry[0];
                _BeamListViewScale = strAry[1];
                _TitleFont = strAry[2];
                _SteelFont = strAry[3];
                _LineType = strAry[4];
                int.TryParse(strAry[5], out _TitleShow);
                _FrameWidth = strAry[6];
                _FrameHeight = strAry[7];
                _FrameWidth2 = strAry[8];
                _FrameHeight2 = strAry[9];
                _LvlFrameTitle = strAry[10];
                _SymbolFrameTitle = strAry[11];
                _FrameTitle = strAry[12];
                _LvlFrameEndword = strAry[13];
                _FrameWidthSub = strAry[14];
                _FrameHeightSub = strAry[15];
                _SubFrameTitle = strAry[16];

                _FullTableName = strAry[17];
                _PickTable = strAry[18];

                var version = _CmpAttribute.ResourceText("IDS_TXT_REVITVERSION_2027");

                
                
                if (System.IO.File.Exists(_FullTableName))
                {
                    _TableFileDirectory = _FullTableName.Substring(0, _FullTableName.LastIndexOf("\\"));
                    _TableFileName = _FullTableName.Substring(_FullTableName.LastIndexOf("\\") + 1);
                }
                else
                {
                    // マイドキュメント
                    string myDoc = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

                    string defFolder = myDoc + "\\" + _CmpAttribute.ResourceText("IDS_TXT_ADSKREXJ") + "\\" + version;
                    if (System.IO.Directory.Exists(defFolder))
                    {
                        _TableFileDirectory = defFolder;

                        string defFile = defFolder + "\\" + _CmpAttribute.ResourceText($"IDS_TXT_TABLEFILE_{version}");

                        if (System.IO.File.Exists(defFile))
                        {
                            _TableFileName = _CmpAttribute.ResourceText($"IDS_TXT_TABLEFILE_{version}");

                            _FullTableName = defFile;
                        }
                    }
                    else
                    {
                        string defFolder_ADSK = myDoc + "\\" + _CmpAttribute.ResourceText("IDS_TXT_ADSKREXJ");
                        if (System.IO.Directory.Exists(defFolder_ADSK) == false)
                        {
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
                        if (System.IO.File.Exists(reservePath + "\\" + tableFile))
                        {
                            System.IO.File.Copy(reservePath + "\\" + tableFile,
                                                defFolder + "\\" + tableFile);
                        }

                        // マッピングファイルコピー
                        if (System.IO.File.Exists(reservePath + "\\" + mapParamFile))
                        {
                            System.IO.File.Copy(reservePath + "\\" + mapParamFile,
                                                defFolder + "\\" + mapParamFile);
                        }

                        // 共有パラメータファイルコピー
                        if (System.IO.File.Exists(reservePath + "\\" + shareFile))
                        {
                            System.IO.File.Copy(reservePath + "\\" + shareFile,
                                                defFolder + "\\" + shareFile);
                        }

                        // 共有パラメータオリジナルファイルコピー
                        if (System.IO.File.Exists(reservePath + "\\" + shareFileOrg))
                        {
                            System.IO.File.Copy(reservePath + "\\" + shareFileOrg,
                                                defFolder + "\\" + shareFileOrg);
                        }

                        _TableFileDirectory = defFolder;

                        string defFile = defFolder + "\\" + tableFile;

                        _TableFileName = _CmpAttribute.ResourceText($"IDS_TXT_TABLEFILE_{version}");

                        _FullTableName = defFile;
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>文字設定</summary>
        ///
        /// <history><p>2016/08/29 Created GSA,Inc. Ryo Kuroda</p>
        ///           <p>2017/07/04  Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        void SetText()
        {
            // タイトル
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_COMMONSETTING");

            // ビュー尺度
            this.grpBoxCustomViewScale.Text = _CmpAttribute.ResourceText("IDS_TXT_VIEWSCALE");
            this.lblCustomViewScaleExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_VIEWSCALEEXPLAIN");
            this.lblColumnListViewScale.Text = _CmpAttribute.ResourceText("IDS_TXT_VIEWLISTSCALECOLUMN");
            this.lblBeamListViewScale.Text = _CmpAttribute.ResourceText("IDS_TXT_VIEWLISTSCALEBEAM");

            // 文字タイプ
            this.grpBoxFontType.Text = _CmpAttribute.ResourceText("IDS_TXT_FONTTYPE");
            this.lblFontTypeExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_FONTTYPEEXPLAIN");
            this.lblTitleFont.Text = _CmpAttribute.ResourceText("IDS_TXT_TITLE");
            this.lblSteelFont.Text = _CmpAttribute.ResourceText("IDS_TXT_STEELSIZE");

            // 線種タイプ
            this.grpBoxLineType.Text = _CmpAttribute.ResourceText("IDS_TXT_LINETYPE");
            this.lblLineTypeExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_LINETYPEEXPLAIN");
            this.lblLineTypeFrame.Text = _CmpAttribute.ResourceText("IDS_TXT_FRAMELINE");

            // 設定ファイル
            this.grpBoxSettingFile.Text = _CmpAttribute.ResourceText("IDS_TXT_SETTINGFILE");
            this.lblCurrentFile.Text = _CmpAttribute.ResourceText("IDS_TXT_CURRENTSETTINGFILE");
            this.lblCurrentFileName.Text = _SettingFileName;
            this.btnReadSettingFile.Text = _CmpAttribute.ResourceText("IDS_TXT_READ");

            bool isSettingFile = true;
            string fullName = _SettingFileDirectory + _SettingFileName;

            if (this.lblCurrentFileName.Text == "- " || System.IO.File.Exists(fullName) == false)
            {
                this.lblCurrentFileName.Text = "- " + _CmpAttribute.ResourceText("IDS_TXT_SETDEFAULT");
            }
            else
            {
                isSettingFile = IsSettingFileRight(fullName);
            }

            var version = _CmpAttribute.ResourceText("IDS_TXT_REVITVERSION_2027");
            
            // テーブルファイル
            this.grpBoxSelectTable.Text = "";
            this.chkBoxPickTable.Text = _CmpAttribute.ResourceText("IDS_TXT_PICKTABLE");
            this.lblCurrentTable.Text = _CmpAttribute.ResourceText("IDS_TXT_CURRENTTABLEFILE");
            this.btnTableSelect.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECT");

            if (System.IO.File.Exists(_FullTableName))
            {
                this.lblCurrentTableName.Text = _TableFileName;
            }
            else
            {
                // マイドキュメント
                string myDoc = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

                // 基準ファイル
                string defFile = myDoc + "\\" + _CmpAttribute.ResourceText("IDS_TXT_ADSKREXJ") + $"\\{version}\\" + _CmpAttribute.ResourceText($"IDS_TXT_TABLEFILE_{version}");
                if (System.IO.File.Exists(defFile))
                {
                    this.lblCurrentTableName.Text = _CmpAttribute.ResourceText($"IDS_TXT_TABLEFILE_{version}");
                }
                else
                {
                    this.lblCurrentTableName.Text = "- " + _CmpAttribute.ResourceText("IDS_TXT_USESHARETABLE");
                }
            }

            // 表示枠
            this.grpBoxFrame.Text = _CmpAttribute.ResourceText("IDS_TXT_SHOWFRAME");
            this.lblFrameExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_SHOWFRAMEEXPLAIN");
            this.rdoBtn2Title.Text = _CmpAttribute.ResourceText("IDS_TXT_2TITLE");
            this.rdoBtn1Title.Text = _CmpAttribute.ResourceText("IDS_TXT_1TITLE");
            this.lblFrameWidthMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.lblFrameHeightMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.lblFrameWidthMM2.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.lblFrameHeightMM2.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");

            this.lblLvlFrameTitle.Text = _CmpAttribute.ResourceText("IDS_TXT_LEVELFRAMETITLE");
            this.lblSymbolFrameTitle.Text = _CmpAttribute.ResourceText("IDS_TXT_SYMBOLFRAME");
            this.lblFrameTitle.Text = _CmpAttribute.ResourceText("IDS_TXT_FRAMETITLE");
            this.lblLvlFrameEndWord.Text = _CmpAttribute.ResourceText("IDS_TXT_LEVELFRAMEENDWORD");

            this.lblSubFrame.Text = _CmpAttribute.ResourceText("IDS_TXT_SUBITEMLIST");
            this.lblSubWidthMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.lblSubHeightMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.lblSubFrameTitle.Text = _CmpAttribute.ResourceText("IDS_TXT_FRAMETITLE");
        }

        /// ================================================================================
        /// <summary>設定ファイルの中身</summary>
        ///
        /// <history>2016/08/30 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        private
        bool IsSettingFileRight(string fullName)
        {
            bool ret = false;

            if (System.IO.File.Exists(fullName))
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                System.Text.Encoding enc = System.Text.Encoding.GetEncoding("shift_jis");

                string[] strs = System.IO.File.ReadAllLines(fullName, enc);

                if (strs.Length == 75 || strs.Length == 76)
                {
                    ret = true;
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>文字列の小数値判定</summary>
        ///
        /// <history><p>2016/08/30 Created GSA,Inc Ryo Kuroda</p>
        ///           <p>2016/09/07 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        private
        bool IsDoubleString(string strVal)
        {
            bool ret = false;

            double outDouble = 0;

            if (double.TryParse(strVal, out outDouble))
            {
                if (outDouble != 0 && outDouble != 0.0 && outDouble <= 100)
                {
                    ret = true;
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>文字列の整数値判定</summary>
        ///
        /// <history>2016/08/30 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        private
        bool IsIntString(string strVal, int mode)
        {
            bool ret = false;

            int outInt = 0;

            if (int.TryParse(strVal, out outInt))
            {
                if (outInt > 10)
                {
                    ret = false;
                    return ret;
                }

                if (mode == 0)
                {
                    ret = true;
                }
                else
                {
                    if (outInt != 0)
                    {
                        ret = true;
                    }
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>ボタン使用可否切り替え</summary>
        ///
        /// <history>2016/08/30 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        private
        void BtnEnabledChange(bool enableBool)
        {
            //this.btnOverWriteSave.Enabled = enableBool;
            //this.btnSaveAs.Enabled = enableBool;
        }

        /// ================================================================================
        /// <summary>コントロール内全コントロール</summary>
        ///
        /// <param name="ctrl">コントロール</param>
        ///
        /// <history>2016/08/30 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        private
        System.Windows.Forms.Control[] GetCtrls(System.Windows.Forms.Control ctrl)
        {
            Collections.ArrayList ret = new Collections.ArrayList();

            foreach (System.Windows.Forms.Control c in ctrl.Controls)
            {
                ret.Add(c);
                ret.AddRange(GetCtrls(c));
            }

            return (System.Windows.Forms.Control[])ret.ToArray(typeof(System.Windows.Forms.Control));
        }

        /// ================================================================================
        /// <summary>全項目の入力判定</summary>
        ///
        /// <history>2016/08/30 Created GSA, inc. Ryo Kuroda</history>
        /// ================================================================================
        private
        bool InputJudge()
        {
            bool ret = true;

            foreach (System.Windows.Forms.Control ctrl in GetCtrls(this))
            {
                // テキストボックス
                if (ctrl is System.Windows.Forms.TextBox)
                {
                    System.Windows.Forms.TextBox txtBox = (System.Windows.Forms.TextBox)ctrl;

                    // 階表示枠接尾語
                    if (txtBox.Name == this.txtBoxLvlFrameEndWord.Name)
                    {
                        continue;
                    }
                    else
                    {
                        if (txtBox.Text == "" || txtBox.Text == null)
                        {
                            ret = false;
                            break;
                        }
                    }
                }
                // コンボボックス
                else if (ctrl is System.Windows.Forms.ComboBox)
                {
                    System.Windows.Forms.ComboBox cmbBox = (System.Windows.Forms.ComboBox)ctrl;

                    if (cmbBox.SelectedItem == null)
                    {
                        ret = false;
                        break;
                    }
                }
                // ラジオボタン
                else if (ctrl is System.Windows.Forms.RadioButton)
                {
                    // 親をとって、親に含まれるラジオボタンを取得し、チェック
                    System.Windows.Forms.RadioButton rdoBtn = (System.Windows.Forms.RadioButton)ctrl;

                    System.Windows.Forms.Control.ControlCollection ctrls = rdoBtn.Parent.Controls;

                    bool check = false;

                    foreach (System.Windows.Forms.Control cont in ctrls)
                    {
                        if (cont is System.Windows.Forms.RadioButton)
                        {
                            System.Windows.Forms.RadioButton rb = (System.Windows.Forms.RadioButton)cont;
                            check = rb.Checked;

                            if (check == true)
                            {
                                break;
                            }
                        }
                    }

                    if (check == false)
                    {
                        ret = false;
                        break;
                    }
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>未入力の入力</summary>
        ///
        /// <history><p>2016/08/30 Created GSA,Inc Ryo Kuroda</p>
        ///           <p>2017/06/  Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        private
        void SetUnWrite()
        {
            // 柱リストビュー尺度
            if (this.cmbBoxColumnListViewScale.SelectedItem == null)
            {
                if (this.cmbBoxColumnListViewScale.Items.Count > 0)
                {
                    this.cmbBoxColumnListViewScale.SelectedIndex = 0;
                }
                if (this.cmbBoxColumnListViewScale.Items.Count > 1)
                {
                    this.cmbBoxColumnListViewScale.SelectedIndex = 1;
                }
            }
            // 梁リストビュー尺度
            if (this.cmbBoxBeamListViewScale.SelectedItem == null)
            {
                if (this.cmbBoxBeamListViewScale.Items.Count > 0)
                {
                    this.cmbBoxBeamListViewScale.SelectedIndex = 0;
                }
                if (this.cmbBoxBeamListViewScale.Items.Count > 1)
                {
                    this.cmbBoxBeamListViewScale.SelectedIndex = 1;
                }
            }

            // タイトルフォント
            if (this.cmbBoxTitleFont.SelectedItem == null)
            {
                if (this.cmbBoxTitleFont.Items.Count > 0)
                {
                    this.cmbBoxTitleFont.SelectedIndex = 0;
                }
                if (this.cmbBoxTitleFont.Items.Count > 1)
                {
                    this.cmbBoxTitleFont.SelectedIndex = 1;
                }
            }
            // 鉄筋フォント
            if (this.cmbBoxSteelFont.SelectedItem == null)
            {
                if (this.cmbBoxSteelFont.Items.Count > 0)
                {
                    this.cmbBoxSteelFont.SelectedIndex = 0;
                }
                if (this.cmbBoxSteelFont.Items.Count > 1)
                {
                    this.cmbBoxSteelFont.SelectedIndex = 1;
                }
            }

            // 線種
            if (this.cmbBoxLineTypeFrame.SelectedItem == null)
            {
                if (this.cmbBoxLineTypeFrame.Items.Count > 0)
                {
                    this.cmbBoxLineTypeFrame.SelectedIndex = 0;
                }
                if (this.cmbBoxLineTypeFrame.Items.Count > 1)
                {
                    this.cmbBoxLineTypeFrame.SelectedIndex = 1;
                }
            }

            // 枠サイズ
            if (this.txtBoxFrameWidth.Text == "" || this.txtBoxFrameWidth.Text == null)
            {
                this.txtBoxFrameWidth.Text = "12.5";
            }
            if (this.txtBoxFrameHeight.Text == "" || this.txtBoxFrameHeight.Text == null)
            {
                this.txtBoxFrameHeight.Text = "12.5";
            }
            if (this.txtBoxFrameWidth2.Text == "" || this.txtBoxFrameWidth2.Text == null)
            {
                this.txtBoxFrameWidth2.Text = "12.5";
            }
            if (this.txtBoxFrameHeight2.Text == "" || this.txtBoxFrameHeight2.Text == null)
            {
                this.txtBoxFrameHeight2.Text = "12.5";
            }
            if (this.txtBoxSubWidth.Text == "" || this.txtBoxSubWidth.Text == null)
            {
                this.txtBoxSubWidth.Text = "12.5";
            }
            if (this.txtBoxSubHeight.Text == "" || this.txtBoxSubHeight.Text == null)
            {
                this.txtBoxSubHeight.Text = "12.5";
            }

            // 文字
            if (this.txtBoxLvlFrameTitle.Text == "" || this.txtBoxLvlFrameTitle.Text == null)
            {
                this.txtBoxLvlFrameTitle.Text = "階";
            }
            if (this.txtBoxSymbolFrameTitle.Text == "" || this.txtBoxSymbolFrameTitle.Text == null)
            {
                this.txtBoxSymbolFrameTitle.Text = "符号";
            }
            if (this.txtBoxFrameTitle.Text == "" || this.txtBoxFrameTitle.Text == null)
            {
                this.txtBoxFrameTitle.Text = "階";
            }
            if (this.txtBoxSubFrameTitle.Text == "" || this.txtBoxSubFrameTitle.Text == null)
            {
                this.txtBoxSubFrameTitle.Text = "符号";
            }
        }

        /// ================================================================================
        /// <summary>コンボボックス - ドロップダウン幅設定</summary>
        ///
        /// <history>2016/08/30 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        private
        void SetCmbBoxDropWidth(System.Windows.Forms.ComboBox cmbBox)
        {
            if (cmbBox.Items.Count > 0)
            {
                System.Drawing.Graphics graphics = this.CreateGraphics();

                float maxWidth = 0;

                // 最大幅取得
                foreach (object item in cmbBox.Items)
                {
                    maxWidth = System.Math.Max(maxWidth, graphics.MeasureString(item.ToString(), cmbBox.Font).Width);
                }

                // 余白
                maxWidth += 15;

                // 切り上げ、int型に変換
                int newWidth = (int)System.Math.Ceiling((decimal)maxWidth);

                // ドロップダウン幅の変更
                if (cmbBox.DropDownWidth < newWidth)
                {
                    cmbBox.DropDownWidth = newWidth;
                }
            }
        }

        /// ================================================================================
        /// <summary>画像サイズ補正</summary>
        ///
        /// <history>2016/08/30 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        private
        void SetDPISizing()
        {
            // サイズ補正
            System.Drawing.Graphics gra = this.CreateGraphics();
            float dpiX = gra.DpiX;
            float dpiY = gra.DpiY;

            Bitmap bmp1 = Resources.Image.IDI_FORMIMAGE_2TITLE;
            Bitmap bmp2 = Resources.Image.IDI_FORMIMAGE_1TITLE;

            // 係数
            double coefficientX = dpiX / 96;
            double coefficientY = dpiY / 96;

            this.pictBox2Title.SizeMode = PictureBoxSizeMode.AutoSize;

            Bitmap newBmp = new Bitmap((int)(bmp1.Width * coefficientX), (int)(bmp1.Height * coefficientY));
            this.pictBox2Title.Image = newBmp;
            Graphics g = Graphics.FromImage(this.pictBox2Title.Image);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.Clear(this.pictBox2Title.BackColor);

            g.DrawImage(bmp1, 3, 3, (float)(bmp1.Width * coefficientX), (float)(bmp1.Height * coefficientY));
            this.pictBox2Title.Refresh();

            this.pictBox1Title.SizeMode = PictureBoxSizeMode.AutoSize;

            newBmp = new Bitmap((int)(bmp2.Width * coefficientX), (int)(bmp2.Height * coefficientY));
            this.pictBox1Title.Image = newBmp;
            g = Graphics.FromImage(this.pictBox1Title.Image);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.Clear(this.pictBox1Title.BackColor);

            g.DrawImage(bmp2, 3, 3, (float)(bmp2.Width * coefficientX), (float)(bmp2.Height * coefficientY));
            this.pictBox1Title.Refresh();
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        /// ================================================================================
        /// <summary>操作結果 - 設定</summary>
        /// ================================================================================
        public
        int SettingResult
        {
            get
            {
                return _Result;
            }
            set
            {
                _Result = value;
            }
        }

        /// ================================================================================
        /// <summary>全項目の入力判定</summary>
        ///
        /// <history>2016/08/30 Created GSA, inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        bool AllInputJudge
        {
            get
            {
                _InputJudge = InputJudge();

                return _InputJudge;
            }
        }

        /// ================================================================================
        /// <summary>設定ファイル名</summary>
        ///
        /// <history>2016/08/30 Created GSA, inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SettingFileName
        {
            get
            {
                return _SettingFileName;
            }
        }

        /// ================================================================================
        /// <summary>設定ファイルディレクトリ</summary>
        ///
        /// <history>2016/08/30 Created GSA, inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SettingFileDirectory
        {
            get
            {
                return _SettingFileDirectory;
            }
        }

        /// ================================================================================
        /// <summary>設定値 - 共通設定</summary>
        ///
        /// <history>2016/08/30 Created GSA, inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        Collections.Generic.IList<string> SettingValues_Common
        {
            get
            {
                Collections.Generic.IList<string> ret = new Collections.Generic.List<string>();

                string columnScale = this.cmbBoxColumnListViewScale.SelectedItem.ToString();
                string beamScale = this.cmbBoxBeamListViewScale.SelectedItem.ToString();
                columnScale = columnScale.Substring(columnScale.IndexOf(":") + 2);
                beamScale = beamScale.Substring(beamScale.IndexOf(":") + 2);

                int titleShow = 0;
                if (this.rdoBtn1Title.Checked)
                {
                    titleShow = 1;
                }

                ret.Add(columnScale);
                ret.Add(beamScale);
                ret.Add(this.cmbBoxTitleFont.SelectedItem.ToString());
                ret.Add(this.cmbBoxSteelFont.SelectedItem.ToString());
                ret.Add(this.cmbBoxLineTypeFrame.SelectedItem.ToString());
                ret.Add(titleShow.ToString());
                ret.Add(this.txtBoxFrameWidth.Text);
                ret.Add(this.txtBoxFrameHeight.Text);
                ret.Add(this.txtBoxFrameWidth2.Text);
                ret.Add(this.txtBoxFrameHeight2.Text);
                ret.Add(this.txtBoxLvlFrameTitle.Text);
                ret.Add(this.txtBoxSymbolFrameTitle.Text);
                ret.Add(this.txtBoxFrameTitle.Text);
                ret.Add(this.txtBoxLvlFrameEndWord.Text);
                ret.Add(this.txtBoxSubWidth.Text);
                ret.Add(this.txtBoxSubHeight.Text);
                ret.Add(this.txtBoxSubFrameTitle.Text);
                ret.Add(_FullTableName);
                ret.Add(_PickTable);

                return ret;
            }
        }

        #endregion Properties

        // イベント

        #region Events

        /// ================================================================================
        /// <summary>ロード</summary>
        /// ================================================================================
        private void FormCommonSetting_Load(object sender, EventArgs e)
        {
            ShowData();
        }

        public void ShowData()
        {
            SetText();
            SetSettingValue();

            if (_PickTable == "0")
            {
                this.btnTableSelect.Enabled = false;
                this.lblCurrentTable.Enabled = false;
                this.lblCurrentTableName.Enabled = false;
            }
            else if (_PickTable == "1")
            {
                this.btnTableSelect.Enabled = true;
                this.lblCurrentTable.Enabled = true;
                this.lblCurrentTableName.Enabled = true;
            }

            this.cmbBoxColumnListViewScale.Select();

            SetUnWrite();

            BtnEnabledChange(AllInputJudge);

            SetDPISizing();
        }

        /// ================================================================================
        /// <summary>設定ファイル読み込み</summary>
        /// ================================================================================
        private void btnReadSettingFile_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFile = new System.Windows.Forms.OpenFileDialog();
            openFile.Filter = "テキストファイル (*.txt)|*.txt";
            openFile.Multiselect = false;
            openFile.Title = _CmpAttribute.ResourceText("IDS_TXT_READSETTINGFILE");
            if (_SettingFileDirectory != "")
            {
                openFile.InitialDirectory = _SettingFileDirectory;
            }

            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _SettingFileName = openFile.SafeFileName;
                _FullName = openFile.FileName;
                _SettingFileDirectory = _FullName.Substring(0, _FullName.LastIndexOf(_SettingFileName));

                // 取得ファイルの内容確認
                bool isRight = false;
                bool isCancel = false;

                while (isRight == false && isCancel == false)
                {
                    // 正しいファイル
                    if (_SettingFileName != "SettingFlieInfo.txt")
                    {
                        isRight = IsSettingFileRight(_FullName);
                    }

                    // 再選択
                    if (isRight == false)
                    {
                        if (_SettingFileName == "SettingFlieInfo.txt")
                        {
                            System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_SELOTHERFILE"));
                        }
                        else
                        {
                            System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_SETTINGFILEINCORRECT") + "\r\n" + _CmpAttribute.ResourceText("IDS_ERR_SELOTHERFILEORRESET"));
                        }

                        if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            _SettingFileName = openFile.SafeFileName;
                            _FullName = openFile.FileName;
                            _SettingFileDirectory = _FullName.Substring(0, _FullName.LastIndexOf(_SettingFileName));
                        }
                        else
                        {
                            isCancel = true;
                        }
                    }
                }

                if (isRight == true)
                {
                    // 読み込み指定ファイル更新
                    // this.DialogResult = System.Windows.Forms.DialogResult.Yes;
                    _FormAllSetting.DialogResult = System.Windows.Forms.DialogResult.Yes;

                    _Result = 5;

                    _FormAllSetting.Close();
                    //this.Close();

                    SetData();
                }
            }
        }

        /// ================================================================================
        /// <summary>テーブルファイル選択</summary>
        /// ================================================================================
        private void chkBoxPickTable_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkBoxPickTable.Checked)
            {
                this.btnTableSelect.Enabled = true;
                this.lblCurrentTable.Enabled = true;
                this.lblCurrentTableName.Enabled = true;

                _PickTable = "1";
            }
            else
            {
                this.btnTableSelect.Enabled = false;
                this.lblCurrentTable.Enabled = false;
                this.lblCurrentTableName.Enabled = false;

                _PickTable = "0";
            }
        }

        /// ================================================================================
        /// <summary>テーブルファイル選択</summary>
        /// ================================================================================
        private void btnTableSelect_Click(object sender, EventArgs e)
        {
            var version = _CmpAttribute.ResourceText("IDS_TXT_REVITVERSION_2027");
            
            // テーブル選択
            System.Windows.Forms.OpenFileDialog opnFileDlg = new System.Windows.Forms.OpenFileDialog();
            opnFileDlg.Filter = "テーブルファイル (*.tbl)|*.tbl";
            opnFileDlg.Multiselect = false;
            opnFileDlg.Title = _CmpAttribute.ResourceText("IDS_TXT_TABLESELECT");

            // マイドキュメント
            string myDoc = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

            // 基準フォルダ
            if (System.IO.File.Exists(_FullTableName))
            {
                opnFileDlg.InitialDirectory = _TableFileDirectory;
            }
            else
            {
                string defFolder = myDoc + "\\" + _CmpAttribute.ResourceText("IDS_TXT_ADSKREXJ") + "\\" + version;
                if (System.IO.Directory.Exists(defFolder))
                {
                    opnFileDlg.InitialDirectory = defFolder;
                }
            }

            if (opnFileDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _FullTableName = opnFileDlg.FileName;
                _TableFileName = opnFileDlg.SafeFileName;
                _TableFileDirectory = _FullTableName.Substring(0, _FullTableName.LastIndexOf(_TableFileName));

                this.lblCurrentTableName.Text = _TableFileName;
            }
        }

        /// ================================================================================
        /// <summary>柱リストビュー尺度変更</summary>
        /// ================================================================================
        private void cmbBoxColumnListViewScale_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbBoxColumnListViewScale.SelectedIndex == 0)
            {
                FormCustomViewScale formCustom = new FormCustomViewScale(_CmpAttribute);
                formCustom.ShowDialog();

                if (formCustom.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.cmbBoxColumnListViewScale.DropDownStyle = ComboBoxStyle.DropDown;

                    if (this.cmbBoxColumnListViewScale.Items.Count == 13)
                    {
                        this.cmbBoxColumnListViewScale.Items.Add("");
                    }

                    this.cmbBoxColumnListViewScale.Items[13] = "1 : " + formCustom.CustomViewScale.ToString();
                    this.cmbBoxColumnListViewScale.SelectedIndex = 13;

                    this.cmbBoxColumnListViewScale.DropDownStyle = ComboBoxStyle.DropDownList;
                }
                else
                {
                    this.cmbBoxColumnListViewScale.SelectedIndex = _ColumnListViewScaleIndex;
                }
            }
            else
            {
                _ColumnListViewScaleIndex = this.cmbBoxColumnListViewScale.SelectedIndex;
            }

            BtnEnabledChange(AllInputJudge);
        }

        /// ================================================================================
        /// <summary>梁リストビュー尺度変更</summary>
        /// ================================================================================
        private void cmbBoxBeamListViewScale_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbBoxBeamListViewScale.SelectedIndex == 0)
            {
                FormCustomViewScale formCustom = new FormCustomViewScale(_CmpAttribute);
                formCustom.ShowDialog();

                if (formCustom.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.cmbBoxBeamListViewScale.DropDownStyle = ComboBoxStyle.DropDown;

                    if (this.cmbBoxBeamListViewScale.Items.Count == 13)
                    {
                        this.cmbBoxBeamListViewScale.Items.Add("");
                    }

                    this.cmbBoxBeamListViewScale.Items[13] = "1 : " + formCustom.CustomViewScale.ToString();
                    this.cmbBoxBeamListViewScale.SelectedIndex = 13;

                    this.cmbBoxBeamListViewScale.DropDownStyle = ComboBoxStyle.DropDownList;
                }
                else
                {
                    this.cmbBoxBeamListViewScale.SelectedIndex = _BeamListViewScaleIndex;
                }
            }
            else
            {
                _BeamListViewScaleIndex = this.cmbBoxBeamListViewScale.SelectedIndex;
            }

            BtnEnabledChange(AllInputJudge);
        }

        /// ================================================================================
        /// <summary>2タイトルを表示</summary>
        /// ================================================================================
        private void rdoBtn2Title_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdoBtn2Title.Checked)
            {
                this.txtBoxFrameWidth.Enabled = true;
                this.txtBoxFrameHeight.Enabled = true;

                this.txtBoxFrameWidth2.Enabled = false;
                this.txtBoxFrameHeight2.Enabled = false;

                this.txtBoxLvlFrameTitle.Enabled = true;
                this.txtBoxSymbolFrameTitle.Enabled = true;

                this.txtBoxFrameTitle.Enabled = false;
            }
        }

        /// ================================================================================
        /// <summary>1タイトルを表示</summary>
        /// ================================================================================
        private void rdoBtn1Title_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdoBtn1Title.Checked)
            {
                this.txtBoxFrameWidth.Enabled = false;
                this.txtBoxFrameHeight.Enabled = false;

                this.txtBoxFrameWidth2.Enabled = true;
                this.txtBoxFrameHeight2.Enabled = true;

                this.txtBoxLvlFrameTitle.Enabled = false;
                this.txtBoxSymbolFrameTitle.Enabled = false;

                this.txtBoxFrameTitle.Enabled = true;
            }
        }

        /// ================================================================================
        /// <summary>入力制限 - 枠幅</summary>
        /// ================================================================================
        private void txtBoxFrameWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        /// ================================================================================
        /// <summary>入力制限 - 枠高さ</summary>
        /// ================================================================================
        private void txtBoxFrameHeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        /// ================================================================================
        /// <summary>入力制限 - 枠幅2</summary>
        /// ================================================================================
        private void txtBoxFrameWidth2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        /// ================================================================================
        /// <summary>入力制限 - 枠高さ2</summary>
        /// ================================================================================
        private void txtBoxFrameHeight2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        /// ================================================================================
        /// <summary>入力制限 - 枠幅 二次部材</summary>
        /// ================================================================================
        private void txtBoxSubWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        /// ================================================================================
        /// <summary>入力制限 - 枠高さ 二次部材</summary>
        /// ================================================================================
        private void txtBoxSubHeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        /// ================================================================================
        /// <summary>コントロールが離れたとき - 枠幅</summary>
        /// ================================================================================
        private void txtBoxFrameWidth_Leave(object sender, EventArgs e)
        {
            if (IsDoubleString(this.txtBoxFrameWidth.Text) == false && this.rdoBtn2Title.Checked)
            {
                this.errorProviderInvalid.SetError(this.txtBoxFrameWidth,
                                                   _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") + "\r\n" + _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLEVALUE"));

                this.txtBoxFrameWidth.Select();
                this.txtBoxFrameWidth.SelectAll();

                BtnEnabledChange(false);
            }
            else
            {
                this.errorProviderInvalid.SetError(this.txtBoxFrameWidth, "");

                BtnEnabledChange(AllInputJudge);
            }
        }

        /// ================================================================================
        /// <summary>コントロールが離れたとき - 枠高さ</summary>
        /// ================================================================================
        private void txtBoxFrameHeight_Leave(object sender, EventArgs e)
        {
            if (IsDoubleString(this.txtBoxFrameHeight.Text) == false && this.rdoBtn2Title.Checked)
            {
                this.errorProviderInvalid.SetError(this.txtBoxFrameHeight,
                                                   _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") + "\r\n" + _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLEVALUE"));

                this.txtBoxFrameHeight.Select();
                this.txtBoxFrameHeight.SelectAll();

                BtnEnabledChange(false);
            }
            else
            {
                this.errorProviderInvalid.SetError(this.txtBoxFrameHeight, "");

                BtnEnabledChange(AllInputJudge);
            }
        }

        /// ================================================================================
        /// <summary>コントロールが離れたとき - 枠幅2</summary>
        /// ================================================================================
        private void txtBoxFrameWidth2_Leave(object sender, EventArgs e)
        {
            if (IsDoubleString(this.txtBoxFrameWidth2.Text) == false && this.rdoBtn1Title.Checked)
            {
                this.errorProviderInvalid.SetError(this.txtBoxFrameWidth2,
                                                   _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") + "\r\n" + _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLEVALUE"));

                this.txtBoxFrameWidth2.Select();
                this.txtBoxFrameWidth2.SelectAll();

                BtnEnabledChange(false);
            }
            else
            {
                this.errorProviderInvalid.SetError(this.txtBoxFrameWidth2, "");

                BtnEnabledChange(AllInputJudge);
            }
        }

        /// ================================================================================
        /// <summary>コントロールが離れたとき - 枠高さ2</summary>
        /// ================================================================================
        private void txtBoxFrameHeight2_Leave(object sender, EventArgs e)
        {
            if (IsDoubleString(this.txtBoxFrameHeight2.Text) == false && this.rdoBtn1Title.Checked)
            {
                this.errorProviderInvalid.SetError(this.txtBoxFrameHeight2,
                                                   _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") + "\r\n" + _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLEVALUE"));

                this.txtBoxFrameHeight2.Select();
                this.txtBoxFrameHeight2.SelectAll();

                BtnEnabledChange(false);
            }
            else
            {
                this.errorProviderInvalid.SetError(this.txtBoxFrameHeight2, "");

                BtnEnabledChange(AllInputJudge);
            }
        }

        /// ================================================================================
        /// <summary>コントロールが離れたとき - 枠幅 二次部材</summary>
        /// ================================================================================
        private void txtBoxSubWidth_Leave(object sender, EventArgs e)
        {
            if (IsDoubleString(this.txtBoxSubWidth.Text) == false)
            {
                this.errorProviderInvalid.SetError(this.txtBoxSubWidth,
                                                   _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") + "\r\n" + _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLEVALUE"));

                this.txtBoxSubWidth.Select();
                this.txtBoxSubWidth.SelectAll();

                BtnEnabledChange(false);
            }
            else
            {
                this.errorProviderInvalid.SetError(this.txtBoxSubWidth, "");

                BtnEnabledChange(AllInputJudge);
            }
        }

        /// ================================================================================
        /// <summary>コントロールが離れたとき - 枠高さ 二次部材</summary>
        /// ================================================================================
        private void txtBoxSubHeight_Leave(object sender, EventArgs e)
        {
            if (IsDoubleString(this.txtBoxSubHeight.Text) == false)
            {
                this.errorProviderInvalid.SetError(this.txtBoxSubHeight,
                                                   _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") + "\r\n" + _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLEVALUE"));

                this.txtBoxSubHeight.Select();
                this.txtBoxSubHeight.SelectAll();

                BtnEnabledChange(false);
            }
            else
            {
                this.errorProviderInvalid.SetError(this.txtBoxSubHeight, "");

                BtnEnabledChange(AllInputJudge);
            }
        }

        #endregion Events
    }
}
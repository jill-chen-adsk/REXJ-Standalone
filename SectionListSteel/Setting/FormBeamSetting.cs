using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using System.Reflection;
using System.Collections.Generic;
using SectionListSteel.Utils;

namespace SectionListSteel.Setting
{
    /// ================================================================================
    /// <summary>フォーム 梁リスト設定</summary>
    /// ================================================================================
    public partial class FormBeamSetting : Form
    {
        // メンバ変数

        #region Member Variables

        /// <summary>属性</summary>
        private SectionListSteel.Components.Attribute _CmpAttribute;

        /// <summary>設定ファイル名</summary>
        private string _SettingFileName;

        /// <summary>設定ファイルディレクトリ</summary>
        private string _SettingFileDirectory;

        /// <summary>断面位置枠幅</summary>
        private string _PosFrameWidth;

        /// <summary>鉄骨サイズ枠幅</summary>
        private string _SteelFrameWidth;

        /// <summary>鉄骨サイズ枠高さ</summary>
        private string _SteelFrameHeight;

        /// <summary>鋼材種表示</summary>
        private string _ShowSteel;

        /// <summary>全断面</summary>
        private string _SecZendanmen;

        /// <summary>中央部</summary>
        private string _SecChuohbu;

        /// <summary>端部</summary>
        private string _SecTanbu;

        /// <summary>始端</summary>
        private string _SecShitan;

        /// <summary>終端</summary>
        private string _SecSyutan;

        /// <summary>元端</summary>
        private string _SecMototan;

        /// <summary>先端</summary>
        private string _SecSentan;

        /// <summary>備考枠</summary>
        private string _ShowNote;

        /// <summary>リストの折り返し</summary>
        private string _NewLine;

        /// <summary>リストの折り返し列数</summary>
        private string _NewLineSpan;

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
        ///
        /// <history>2016/08/30 Created GSA,Inc. Ryo kuroda</history>
        /// ================================================================================
        public FormBeamSetting(FormAllSetting parent, SectionListSteel.Components.Attribute cmpAttribute,
                               string settingFileName,
                               string settingFileDirectory)
        {
            InitializeComponent();
            _FormAllSetting = parent;

            _CmpAttribute = cmpAttribute;
            _SettingFileName = settingFileName;
            _SettingFileDirectory = settingFileDirectory;

            SetData();
            SetSettingValue();
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>データ設定</summary>
        ///
        /// <history>2016/08/30 Created GSA,Inc. Ryo kuroda</history>
        /// ================================================================================
        private
        void SetData()
        {
            this.txtBoxBeamFramePosWidth.TextAlign = HorizontalAlignment.Right;
            this.txtBoxBeamFrameSteelWidth.TextAlign = HorizontalAlignment.Right;
            this.txtBoxBeamFrameSteelHeight.TextAlign = HorizontalAlignment.Right;

            this.txtBoxBeamFramePosWidth.MaxLength = 5;
            this.txtBoxBeamFrameSteelWidth.MaxLength = 5;
            this.txtBoxBeamFrameSteelHeight.MaxLength = 5;

            this.txtNoteHeight1.TextAlign = HorizontalAlignment.Right;
            this.txtNoteHeight2.TextAlign = HorizontalAlignment.Right;
            this.txtNoteHeight3.TextAlign = HorizontalAlignment.Right;

            this.txtNoteHeight1.MaxLength = 5;
            this.txtNoteHeight2.MaxLength = 5;
            this.txtNoteHeight3.MaxLength = 5;
            this.txtBoxNewLineSpan.MaxLength = 5;
        }

        /// ================================================================================
        /// <summary>文字設定</summary>
        ///
        /// <history><p>2016/08/30 Created GSA,Inc. Ryo kuroda</p>
        ///           <p>2017/07/04 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        private
        void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_BEAMLISTSETTING");

            this.grpBoxBeamFrame.Text = _CmpAttribute.ResourceText("IDS_TXT_SHOWFRAME");
            this.lblBeamFrameExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_SHOWFRAMEEXPLAIN");

            this.lblBeamFramePosWidthMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.lblBeamFrameSteelWidthMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.lblBeamFrameSteelHeightMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");

            this.grpBoxBeamMaterial.Text = _CmpAttribute.ResourceText("IDS_TXT_ZAISHITSU");
            this.lblBeamMaterialExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_ZAISHITSUEXPLAIN");
            this.chkBoxBeamSteel.Text = _CmpAttribute.ResourceText("IDS_TXT_SHOWSTEEL");

            this.grpBoxPositionFrameTitle.Text = _CmpAttribute.ResourceText("IDS_TXT_TITLE");
            this.lblPositionFrameTitleExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_POSITIONFRAMETITLEEXPLAIN");
            this.lblAllSection.Text = _CmpAttribute.ResourceText("IDS_TXT_ALLSECTION");
            this.lblCenterSection.Text = _CmpAttribute.ResourceText("IDS_TXT_CENTERSECTION");
            this.lblEndSection.Text = _CmpAttribute.ResourceText("IDS_TXT_ENDSECTION");
            this.lblItanSecction.Text = _CmpAttribute.ResourceText("IDS_TXT_ITANSECTION");
            this.lblJtanSection.Text = _CmpAttribute.ResourceText("IDS_TXT_JTANSECTION");
            this.lblCantiLeverBase.Text = _CmpAttribute.ResourceText("IDS_TXT_CANTIBASE");
            this.lblCantiLeverEnd.Text = _CmpAttribute.ResourceText("IDS_TXT_CANTIEND");

            this.grpBoxShowNote.Text = _CmpAttribute.ResourceText("IDS_TXT_NOTEFRAME");
            this.lblShowNoteExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_SHOWNOTEEXPLAIN");
            this.chkBoxShowNote.Text = _CmpAttribute.ResourceText("IDS_TXT_SHOWNOTE");

            this.grpBoxNewLine.Text = _CmpAttribute.ResourceText("IDS_TXT_NEWLINE");
            this.lblNewLineExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_NEWLINEEXPLAIN");
            this.chkBoxNewLine.Text = _CmpAttribute.ResourceText("IDS_TXT_NEWLINEGIRDERLIST");
            this.lblNewLineSpan.Text = _CmpAttribute.ResourceText("IDS_TXT_NEWLINESPAN");

            this.lblNotes.Text = _CmpAttribute.ResourceText("IDS_TXT_NOTES");

            this.lblLayer.Text = _CmpAttribute.ResourceText("IDS_TXT_LAYER");

            this.lblNoteName1.Text = _CmpAttribute.ResourceText("IDS_TXT_NOTE_1");
            this.lblNoteName2.Text = _CmpAttribute.ResourceText("IDS_TXT_NOTE_2");
            this.lblNoteName3.Text = _CmpAttribute.ResourceText("IDS_TXT_NOTE_3");

            this.lblNote1HeightMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.lblNote2HeightMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.lblNote3HeightMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
        }

        /// ================================================================================
        /// <summary>設定値設定</summary>
        ///
        /// <history><p>2016/08/30 Created GSA,Inc. Ryo kuroda</p>
        ///           <p>2017/06/21 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        void SetSettingValue()
        {
            GetSettingValue();

            this.txtBoxBeamFramePosWidth.Text = _PosFrameWidth;
            this.txtBoxBeamFrameSteelWidth.Text = _SteelFrameWidth;
            this.txtBoxBeamFrameSteelHeight.Text = _SteelFrameHeight;
            this.txtBoxAllSection.Text = _SecZendanmen;
            this.txtBoxCenterSection.Text = _SecChuohbu;
            this.txtBoxEndSection.Text = _SecTanbu;
            this.txtBoxItanSection.Text = _SecShitan;
            this.txtBoxJtanSection.Text = _SecSyutan;
            this.txtBoxCantiLeverBase.Text = _SecMototan;
            this.txtBoxCantiLeverEnd.Text = _SecSentan;

            if (_ShowSteel == "0")
            {
                this.chkBoxBeamSteel.Checked = false;
            }
            else if (_ShowSteel == "1")
            {
                this.chkBoxBeamSteel.Checked = true;
            }

            if (_ShowNote == "0")
            {
                this.chkBoxShowNote.Checked = false;
            }
            if (_ShowNote == "1")
            {
                this.chkBoxShowNote.Checked = true;
            }

            this.txtBoxNewLineSpan.Text = _NewLineSpan;

            if (_NewLine == "0")
            {
                this.chkBoxNewLine.Checked = false;
                this.txtBoxNewLineSpan.ReadOnly = true;
            }
            else if (_NewLine == "1")
            {
                this.chkBoxNewLine.Checked = true;
                this.txtBoxNewLineSpan.ReadOnly = false;
            }
        }

        /// ================================================================================
        /// <summary>設定値取得</summary>
        ///
        /// <history><p>2016/08/30 Created GSA,Inc Ryo Kuroda</p>
        ///           <p>2017/06/21 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
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

                _PosFrameWidth = strAry[41];
                _SteelFrameWidth = strAry[42];
                _SteelFrameHeight = strAry[43];
                _ShowSteel = strAry[44];
                _SecZendanmen = strAry[45];
                _SecChuohbu = strAry[46];
                _SecTanbu = strAry[47];
                _SecShitan = strAry[48];
                _SecSyutan = strAry[49];
                _SecMototan = strAry[50];
                _SecSentan = strAry[51];
                _ShowNote = strAry[52];

                var value = strAry[53].Trim();
                //double height = 0;
                if (string.IsNullOrEmpty(value))
                    txtNoteHeight1.Text = "9";
                else
                    txtNoteHeight1.Text = value;

                value = strAry[54].Trim();
                //height = 0;
                if (string.IsNullOrEmpty(value))
                    //height = 9;
                    txtNoteHeight2.Text = "9";
                else
                    txtNoteHeight2.Text = value;

                value = strAry[55].Trim();
                //height = 0;
                if (string.IsNullOrEmpty(value))
                    txtNoteHeight3.Text = "9";
                else
                    txtNoteHeight3.Text = value;

                string name = strAry[57].Trim();
                txtNoteName1.Text = name;

                name = strAry[58].Trim();
                txtNoteName2.Text = name;

                name = strAry[59].Trim();
                txtNoteName3.Text = name;

                int selectedIndex = 0;
                int.TryParse(strAry[56].Trim(), out selectedIndex);
                if (selectedIndex >= 0 && selectedIndex <= 3)
                    cboNoteCount.SelectedIndex = selectedIndex;
                else
                    cboNoteCount.SelectedIndex = 0;

                _NewLine = strAry[60];
                _NewLineSpan = strAry[61];
            }
        }

        /// ================================================================================
        /// <summary>設定ファイルの中身</summary>
        ///
        /// <history><p>2016/08/30 Created GSA,Inc. Ryo kuroda</p>
        ///           <p>2016/10/28 Modified CST,Co.Ltd Ryo Kuroda</p></history>
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
        private bool IsDoubleString(string strVal)
        {
            bool ret = false;

            double outDouble = 0;

            if (double.TryParse(strVal, out outDouble))
            {
                if (outDouble != 0 && outDouble != 0.0)
                {
                    ret = true;
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>ボタン使用可否切り替え</summary>
        ///
        /// <history>2016/08/30 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        private void BtnEnabledChange(bool enableBool)
        {
            //_FormAllSetting.BtnEnabledChange(enableBool);
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

                    if (txtBox.Enabled == false)
                    {
                        continue;
                    }

                    if (txtBox.Text == "" || txtBox.Text == null)
                    {
                        ret = false;
                        break;
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
        ///           <p>2017/06/21 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        private
        void SetUnWrite()
        {
            if (this.txtBoxBeamFramePosWidth.Text == "" || this.txtBoxBeamFramePosWidth.Text == null)
            {
                this.txtBoxBeamFramePosWidth.Text = "12";
            }

            if (this.txtBoxBeamFrameSteelWidth.Text == "" || this.txtBoxBeamFrameSteelWidth.Text == null)
            {
                this.txtBoxBeamFrameSteelWidth.Text = "30";
            }

            if (this.txtBoxBeamFrameSteelHeight.Text == "" || this.txtBoxBeamFrameSteelHeight.Text == null)
            {
                this.txtBoxBeamFrameSteelHeight.Text = "9";
            }

            if (this.txtBoxAllSection.Text == "" || this.txtBoxAllSection.Text == null)
            {
                this.txtBoxAllSection.Text = "全断";
            }

            if (this.txtBoxCenterSection.Text == "" || this.txtBoxCenterSection.Text == null)
            {
                this.txtBoxCenterSection.Text = "中央";
            }

            if (this.txtBoxEndSection.Text == "" || this.txtBoxEndSection.Text == null)
            {
                this.txtBoxEndSection.Text = "端部";
            }

            if (this.txtBoxItanSection.Text == "" || this.txtBoxItanSection.Text == null)
            {
                this.txtBoxItanSection.Text = "始端";
            }

            if (this.txtBoxJtanSection.Text == "" || this.txtBoxJtanSection.Text == null)
            {
                this.txtBoxJtanSection.Text = "終端";
            }

            if (this.txtBoxCantiLeverBase.Text == "" || this.txtBoxCantiLeverBase.Text == null)
            {
                this.txtBoxCantiLeverBase.Text = "元端";
            }

            if (this.txtBoxCantiLeverEnd.Text == "" || this.txtBoxCantiLeverEnd.Text == null)
            {
                this.txtBoxCantiLeverEnd.Text = "先端";
            }

            if (this.txtBoxNewLineSpan.Text == "" || this.txtBoxNewLineSpan.Text == null)
            {
                this.txtBoxNewLineSpan.Text = "5";
            }

            if (this.txtNoteName1.Text == "" || this.txtNoteName1.Text == null)
            {
                this.txtNoteName1.Text = "備考1";
            }

            if (this.txtNoteName2.Text == "" || this.txtNoteName2.Text == null)
            {
                this.txtNoteName2.Text = "備考2";
            }

            if (this.txtNoteName3.Text == "" || this.txtNoteName3.Text == null)
            {
                this.txtNoteName3.Text = "備考3";
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

            Bitmap bmp = Resources.Image.IDI_FORMIMAGE_BEAMFRAME;

            // 係数
            double coefficientX = dpiX / 96;
            double coefficientY = dpiY / 96;

            this.pictBoxBeam.SizeMode = PictureBoxSizeMode.AutoSize;

            Bitmap newBmp = new Bitmap((int)(bmp.Width * coefficientX), (int)(bmp.Height * coefficientY));
            this.pictBoxBeam.Image = newBmp;
            Graphics g = Graphics.FromImage(this.pictBoxBeam.Image);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.Clear(this.pictBoxBeam.BackColor);

            g.DrawImage(bmp, 3, 3, (float)(bmp.Width * coefficientX), (float)(bmp.Height * coefficientY));
            this.pictBoxBeam.Refresh();
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        // ================================================================================
        /// <summary>操作結果 - 設定</summary>
        /// ================================================================================
        public
        int SettingResult
        {
            get
            {
                return _Result;
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
        /// <summary>設定値 - 梁リスト設定</summary>
        ///
        /// <history><p>2016/08/30 Created GSA, inc. Ryo Kuroda</p>
        ///           <p>2017/06/21 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        Collections.Generic.IList<string> SettingValues_Beam
        {
            get
            {
                Collections.Generic.IList<string> ret = new Collections.Generic.List<string>();

                string showSteel = "";

                if (this.chkBoxBeamSteel.Checked)
                {
                    showSteel = "1";
                }
                else
                {
                    showSteel = "0";
                }

                string showNote = "";

                if (this.chkBoxShowNote.Checked)
                {
                    showNote = "1";
                }
                else
                {
                    showNote = "0";
                }

                string newLine = "";

                if (this.chkBoxNewLine.Checked)
                {
                    newLine = "1";
                }
                else
                {
                    newLine = "0";
                }

                ret.Add(this.txtBoxBeamFramePosWidth.Text);
                ret.Add(this.txtBoxBeamFrameSteelWidth.Text);
                ret.Add(this.txtBoxBeamFrameSteelHeight.Text);
                ret.Add(showSteel);
                ret.Add(this.txtBoxAllSection.Text);
                ret.Add(this.txtBoxCenterSection.Text);
                ret.Add(this.txtBoxEndSection.Text);
                ret.Add(this.txtBoxItanSection.Text);
                ret.Add(this.txtBoxJtanSection.Text);
                ret.Add(this.txtBoxCantiLeverBase.Text);
                ret.Add(this.txtBoxCantiLeverEnd.Text);
                ret.Add(showNote);
                ret.Add(txtNoteHeight1.Text);
                ret.Add(txtNoteHeight2.Text);
                ret.Add(txtNoteHeight3.Text);
                ret.Add(cboNoteCount.SelectedIndex.ToString());
                ret.Add(txtNoteName1.Text.Trim());
                ret.Add(txtNoteName2.Text.Trim());
                ret.Add(txtNoteName3.Text.Trim());

                ret.Add(newLine);
                ret.Add(this.txtBoxNewLineSpan.Text);

                return ret;
            }
        }

        #endregion Properties

        // イベント

        #region Events

        /// ================================================================================
        /// <summary>ロード</summary>
        /// ================================================================================
        private void FormBeamSetting_Load(object sender, EventArgs e)
        {
            ShowData();
        }

        public void ShowData()
        {
            SetText();
            SetSettingValue();

            this.txtBoxBeamFramePosWidth.Select();

            SetUnWrite();

            BtnEnabledChange(AllInputJudge);

            SetDPISizing();
        }

        /// ================================================================================
        /// <summary>入力制限 - 断面位置枠幅</summary>
        /// ================================================================================
        private void txtBoxBeamFramePosWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        /// ================================================================================
        /// <summary>入力制限 - 鉄骨サイズ枠枠幅</summary>
        /// ================================================================================
        private void txtBoxBeamFrameSteelWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        /// ================================================================================
        /// <summary>入力制限 - 鉄骨サイズ枠高さ</summary>
        /// ================================================================================
        private void txtBoxBeamFrameSteelHeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        /// ================================================================================
        /// <summary>コントロールが離れたとき - 断面位置枠幅</summary>
        /// ================================================================================
        private void txtBoxBeamFramePosWidth_Leave(object sender, EventArgs e)
        {
            if (IsDoubleString(this.txtBoxBeamFramePosWidth.Text) == false)
            {
                this.errorProviderInvalid.SetError(this.txtBoxBeamFramePosWidth,
                                                   _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") + "\r\n" + _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLE"));

                this.txtBoxBeamFramePosWidth.Select();
                this.txtBoxBeamFramePosWidth.SelectAll();

                BtnEnabledChange(false);
            }
            else
            {
                this.errorProviderInvalid.SetError(this.txtBoxBeamFramePosWidth, "");

                BtnEnabledChange(AllInputJudge);
            }
        }

        /// ================================================================================
        /// <summary>コントロールが離れたとき - 鉄骨サイズ枠幅</summary>
        /// ================================================================================
        private void txtBoxBeamFrameSteelWidth_Leave(object sender, EventArgs e)
        {
            if (IsDoubleString(this.txtBoxBeamFrameSteelWidth.Text) == false)
            {
                this.errorProviderInvalid.SetError(this.txtBoxBeamFrameSteelWidth,
                                                   _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") + "\r\n" + _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLE"));

                this.txtBoxBeamFrameSteelWidth.Select();
                this.txtBoxBeamFrameSteelWidth.SelectAll();

                BtnEnabledChange(false);
            }
            else
            {
                this.errorProviderInvalid.SetError(this.txtBoxBeamFrameSteelWidth, "");

                BtnEnabledChange(AllInputJudge);
            }
        }

        /// ================================================================================
        /// <summary>コントロールが離れたとき - 鉄骨サイズ枠高さ</summary>
        /// ================================================================================
        private void txtBoxBeamFrameSteelHeight_Leave(object sender, EventArgs e)
        {
            if (IsDoubleString(this.txtBoxBeamFrameSteelHeight.Text) == false)
            {
                this.errorProviderInvalid.SetError(this.txtBoxBeamFrameSteelHeight,
                                                   _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") + "\r\n" + _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLE"));

                this.txtBoxBeamFrameSteelHeight.Select();
                this.txtBoxBeamFrameSteelHeight.SelectAll();

                BtnEnabledChange(false);
            }
            else
            {
                this.errorProviderInvalid.SetError(this.txtBoxBeamFrameSteelHeight, "");

                BtnEnabledChange(AllInputJudge);
            }
        }

        /// ================================================================================
        /// <summary>ヘルプ</summary>
        /// ================================================================================
        private void pictBoxHelpBeamMaterial_Click(object sender, EventArgs e)
        {
            SectionListSteel.Setting.FormHelpView formHelp = new SectionListSteel.Setting.FormHelpView(_CmpAttribute, 1, this);
            formHelp.ShowDialog();
        }

        /// ================================================================================
        /// <summary>リストの折り返し</summary>
        /// ================================================================================
        private void chkBoxNewLine_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkBoxNewLine.Checked)
            {
                this.txtBoxNewLineSpan.ReadOnly = false;
            }
            else
            {
                this.txtBoxNewLineSpan.ReadOnly = true;
            }
        }

        /// ================================================================================
        /// <summary>入力制限 - 折り返しスパン</summary>
        /// ================================================================================
        private void txtBoxNewLineSpan_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        /// ================================================================================
        /// <summary>Selected index changed</summary>
        /// ================================================================================
        private void cboNoteCount_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtNoteName1.Enabled = false;
            txtNoteName2.Enabled = false;
            txtNoteName3.Enabled = false;

            txtNoteHeight1.Enabled = false;
            txtNoteHeight2.Enabled = false;
            txtNoteHeight3.Enabled = false;

            if (cboNoteCount.SelectedIndex >= 1)
            {
                txtNoteName1.Enabled = true;
                txtNoteHeight1.Enabled = true;
            }
            if (cboNoteCount.SelectedIndex >= 2)
            {
                txtNoteName2.Enabled = true;
                txtNoteHeight2.Enabled = true;
            }
            if (cboNoteCount.SelectedIndex == 3)
            {
                txtNoteName3.Enabled = true;
                txtNoteHeight3.Enabled = true;
            }

            BtnEnabledChange(AllInputJudge);
        }        

        /// ================================================================================
        /// <summary>Press textbox event</summary>
        /// ================================================================================
        private void txtNoteHeight1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        /// ================================================================================
        /// <summary>Press textbox event</summary>
        /// ================================================================================
        private void txtNoteHeight2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        /// ================================================================================
        /// <summary>Press textbox event</summary>
        /// ================================================================================
        private void txtNoteHeight3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        /// ================================================================================
        /// <summary>Leave textbox event</summary>
        /// ================================================================================
        private void txtNoteHeight1_Leave(object sender, EventArgs e)
        {
            if (IsDoubleString(this.txtNoteHeight1.Text) == false)
            {
                this.errorProviderInvalid.SetError(this.txtNoteHeight1,
                                                   _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") + "\r\n" + _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLE"));

                this.txtNoteHeight1.Select();
                this.txtNoteHeight1.SelectAll();

                BtnEnabledChange(false);
            }
            else
            {
                this.errorProviderInvalid.SetError(this.txtNoteHeight1, "");

                BtnEnabledChange(AllInputJudge);
            }
        }

        /// ================================================================================
        /// <summary>Leave textbox event</summary>
        /// ================================================================================
        private void txtNoteHeight2_Leave(object sender, EventArgs e)
        {
            if (IsDoubleString(this.txtNoteHeight2.Text) == false)
            {
                this.errorProviderInvalid.SetError(this.txtNoteHeight2,
                                                   _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") + "\r\n" + _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLE"));

                this.txtNoteHeight2.Select();
                this.txtNoteHeight2.SelectAll();

                BtnEnabledChange(false);
            }
            else
            {
                this.errorProviderInvalid.SetError(this.txtNoteHeight2, "");

                BtnEnabledChange(AllInputJudge);
            }
        }

        /// ================================================================================
        /// <summary>Leave textbox event</summary>
        /// ================================================================================
        private void txtNoteHeight3_Leave(object sender, EventArgs e)
        {
            if (IsDoubleString(this.txtNoteHeight3.Text) == false)
            {
                this.errorProviderInvalid.SetError(this.txtNoteHeight3,
                                                   _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") + "\r\n" + _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLE"));

                this.txtNoteHeight3.Select();
                this.txtNoteHeight3.SelectAll();

                BtnEnabledChange(false);
            }
            else
            {
                this.errorProviderInvalid.SetError(this.txtNoteHeight3, "");

                BtnEnabledChange(AllInputJudge);
            }
        }

        /// ================================================================================
        /// <summary>Text changed textbox event</summary>
        /// ================================================================================
        private void txtNoteName1_TextChanged(object sender, EventArgs e)
        {
            //BtnEnabledChange(AllInputJudge);
        }

        /// ================================================================================
        /// <summary>Text changed textbox event</summary>
        /// ================================================================================
        private void txtNoteName2_TextChanged(object sender, EventArgs e)
        {
            //BtnEnabledChange(AllInputJudge);
        }

        /// ================================================================================
        /// <summary>Text changed textbox event</summary>
        /// ================================================================================
        private void txtNoteName3_TextChanged(object sender, EventArgs e)
        {
            //BtnEnabledChange(AllInputJudge);
        }

        #endregion Events
    }
}
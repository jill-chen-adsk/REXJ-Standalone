using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using System.Reflection;
using SectionListSteel.Components;
using SectionListSteel.Utils;
using System.Collections.Generic;

namespace SectionListSteel.Setting
{
    /// ================================================================================
    /// <summary>フォーム 柱リスト設定</summary>
    /// ================================================================================
    public partial class FormColumnSetting : Form
    {
        // メンバ変数

        #region Member Variables

        /// <summary>属性</summary>
        private SectionListSteel.Components.Attribute _CmpAttribute;

        /// <summary>設定ファイル名</summary>
        private string _SettingFileName;

        /// <summary>設定ファイルディレクトリ</summary>
        private string _SettingFileDirectory;

        /// <summary>枠幅</summary>
        private string _FrameWidth;

        /// <summary>枠高さ</summary>
        private string _FrameHeight;

        /// <summary>鋼材種表示</summary>
        private string _ShowSteel;

        /// <summary>充填コンクリートFc表示</summary>
        private string _ShowConcrete;

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
        public FormColumnSetting(FormAllSetting parent, SectionListSteel.Components.Attribute cmpAttribute,
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
        public
        void SetData()
        {
            this.txtBoxColumnFrameWidth.TextAlign = HorizontalAlignment.Right;
            this.txtBoxColumnFrameHeight.TextAlign = HorizontalAlignment.Right;

            this.txtBoxColumnFrameWidth.MaxLength = 5;
            this.txtBoxColumnFrameHeight.MaxLength = 5;

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
        ///           <p>2017/07/04 Modified CST,Co.Ltd Ryo Kuroda</p></history>
        /// ================================================================================
        public
        void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_COLUMNLISTSETTING");

            this.grpBoxColumnFrame.Text = _CmpAttribute.ResourceText("IDS_TXT_SHOWFRAME");
            this.lblColumnFrameExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_SHOWFRAMEEXPLAIN2");

            this.lblColumnFrameWidthMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.lblColumnFrameHeightMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");

            this.grpBoxColumnMaterial.Text = _CmpAttribute.ResourceText("IDS_TXT_ZAISHITSU");
            this.lblColumnMaterialExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_ZAISHITSUEXPLAIN");
            this.chkBoxColumnSteel.Text = _CmpAttribute.ResourceText("IDS_TXT_SHOWSTEEL");
            this.chkBoxColumnConcreate.Text = _CmpAttribute.ResourceText("IDS_TXT_SHOWCONCRETE");

            this.grpBoxShowNote.Text = _CmpAttribute.ResourceText("IDS_TXT_NOTEFRAME");
            this.lblShowNoteExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_SHOWNOTEEXPLAIN");
            this.chkBoxShowNote.Text = _CmpAttribute.ResourceText("IDS_TXT_SHOWNOTE");

            this.grpBoxNewLine.Text = _CmpAttribute.ResourceText("IDS_TXT_NEWLINE");
            this.lblNewLineExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_NEWLINEEXPLAIN");
            this.chkBoxNewLine.Text = _CmpAttribute.ResourceText("IDS_TXT_NEWLINECOLUMNLIST");
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
        ///           <p>2017/06/21 Modified CST,Co.Ltd Ryo Kuroda</p></history>
        /// ================================================================================
        public
        void SetSettingValue()
        {
            // 設定ファイルから設定値取得
            GetSettingValue();

            this.txtBoxColumnFrameWidth.Text = _FrameWidth;
            this.txtBoxColumnFrameHeight.Text = _FrameHeight;

            if (_ShowSteel == "0")
            {
                this.chkBoxColumnSteel.Checked = false;
            }
            if (_ShowSteel == "1")
            {
                this.chkBoxColumnSteel.Checked = true; ;
            }

            if (_ShowConcrete == "0")
            {
                this.chkBoxColumnConcreate.Checked = false;
            }
            if (_ShowConcrete == "1")
            {
                this.chkBoxColumnConcreate.Checked = true;
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
        /// <history><p>2016/08/30 Created GSA,Inc. Ryo kuroda</p>
        ///           <p>2017/06/21 Modified CST,Co.Ltd Ryo Kuroda</p></history>
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

                _FrameWidth = strAry[20];
                _FrameHeight = strAry[21];
                _ShowSteel = strAry[22];
                _ShowConcrete = strAry[23];

                _ShowNote = strAry[24].Trim();

                var value = strAry[25].Trim();

                if (string.IsNullOrEmpty(value))
                    txtNoteHeight1.Text = "9";
                else
                    txtNoteHeight1.Text = value;

                value = strAry[26].Trim();
                if (string.IsNullOrEmpty(value))
                    txtNoteHeight2.Text = "9";
                else
                    txtNoteHeight2.Text = value;

                value = strAry[27].Trim();
                if (string.IsNullOrEmpty(value))
                    txtNoteHeight3.Text = "9";
                else
                    txtNoteHeight3.Text = value;

                string name = strAry[29].Trim();
                txtNoteName1.Text = name;

                name = strAry[30].Trim();
                txtNoteName2.Text = name;

                name = strAry[31].Trim();
                txtNoteName3.Text = name;

                int selectedIndex = 0;
                int.TryParse(strAry[28].Trim(), out selectedIndex);
                if (selectedIndex >= 0 && selectedIndex <= 3)
                    cboNoteCount.SelectedIndex = selectedIndex;
                else
                    cboNoteCount.SelectedIndex = 0;

                _NewLine = strAry[32];
                _NewLineSpan = strAry[33];
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
        /// <summary>コントロールの入力判定</summary>
        ///
        /// <param name="ctrl">テキストボックス、コンボボックス、ラジオボタン</param>
        ///
        /// <history>2016/08/30 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        private
        bool CtrlInputJudge(System.Windows.Forms.Control ctrl)
        {
            bool ret = true;

            if (ctrl is System.Windows.Forms.TextBox)
            {
                System.Windows.Forms.TextBox txtBox = (System.Windows.Forms.TextBox)ctrl;

                if (txtBox.Text == "" || txtBox.Text == null)
                {
                    ret = false;
                }
            }
            else if (ctrl is System.Windows.Forms.ComboBox)
            {
                System.Windows.Forms.ComboBox cmbBox = (System.Windows.Forms.ComboBox)ctrl;

                if (cmbBox.SelectedItem == null)
                {
                    ret = false;
                }
            }
            else if (ctrl is System.Windows.Forms.RadioButton)
            {
                // 親をとって、親に含まれるラジオボタンを取得し、チェック
                System.Windows.Forms.RadioButton rdoBtn = (System.Windows.Forms.RadioButton)ctrl;

                System.Windows.Forms.Control.ControlCollection ctrlCollection = rdoBtn.Parent.Controls;

                bool check = false;

                foreach (System.Windows.Forms.Control cont in ctrlCollection)
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
                }
            }

            return ret;
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
            if (this.txtBoxColumnFrameWidth.Text == "" || this.txtBoxColumnFrameWidth.Text == null)
            {
                this.txtBoxColumnFrameWidth.Text = "30";
            }

            if (this.txtBoxColumnFrameHeight.Text == "" || this.txtBoxColumnFrameHeight.Text == null)
            {
                this.txtBoxColumnFrameHeight.Text = "9";
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

            Bitmap bmp = Resources.Image.IDI_FORMIMAGE_COLUMNFRAME;

            // 係数
            double coefficientX = dpiX / 96;
            double coefficientY = dpiY / 96;

            this.pictBoxColumn.SizeMode = PictureBoxSizeMode.AutoSize;

            Bitmap newBmp = new Bitmap((int)(bmp.Width * coefficientX), (int)(bmp.Height * coefficientY));
            this.pictBoxColumn.Image = newBmp;
            Graphics g = Graphics.FromImage(this.pictBoxColumn.Image);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.Clear(this.pictBoxColumn.BackColor);

            g.DrawImage(bmp, 3, 3, (float)(bmp.Width * coefficientX), (float)(bmp.Height * coefficientY));
            this.pictBoxColumn.Refresh();
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
        /// <summary>設定値 - 柱リスト設定</summary>
        ///
        /// <history><p>2016/08/30 Created GSA, inc. Ryo Kuroda</p>
        ///           <p>2017/06/21 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        Collections.Generic.IList<string> SettingValues_Column
        {
            get
            {
                Collections.Generic.IList<string> ret = new Collections.Generic.List<string>();

                string showSteel = "";
                string showConcrete = "";

                if (this.chkBoxColumnSteel.Checked)
                {
                    showSteel = "1";
                }
                else
                {
                    showSteel = "0";
                }

                if (this.chkBoxColumnConcreate.Checked)
                {
                    showConcrete = "1";
                }
                else
                {
                    showConcrete = "0";
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

                ret.Add(this.txtBoxColumnFrameWidth.Text);
                ret.Add(this.txtBoxColumnFrameHeight.Text);
                ret.Add(showSteel);
                ret.Add(showConcrete);
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
        private void FormColumnSetting_Load(object sender, EventArgs e)
        {
            ShowData();
        }

        public void ShowData()
        {
            SetText();
            SetSettingValue();

            this.txtBoxColumnFrameWidth.Select();

            SetUnWrite();

            BtnEnabledChange(AllInputJudge);

            SetDPISizing();
        }

        /// ================================================================================
        /// <summary>入力制限 - 枠幅</summary>
        /// ================================================================================
        private void txtBoxColumnFrameWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        /// ================================================================================
        /// <summary>入力制限 - 枠高さ</summary>
        /// ================================================================================
        private void txtBoxColumnFrameHeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        /// ================================================================================
        /// <summary>コントロールが離れたとき - 枠幅</summary>
        /// ================================================================================
        private void txtBoxColumnFrameWidth_Leave(object sender, EventArgs e)
        {
            if (IsDoubleString(this.txtBoxColumnFrameWidth.Text) == false)
            {
                this.errorProviderInvalid.SetError(this.txtBoxColumnFrameWidth,
                                                   _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") + "\r\n" + _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLE"));

                this.txtBoxColumnFrameWidth.Select();
                this.txtBoxColumnFrameWidth.SelectAll();

                BtnEnabledChange(false);
            }
            else
            {
                this.errorProviderInvalid.SetError(this.txtBoxColumnFrameWidth, "");

                BtnEnabledChange(AllInputJudge);
            }
        }

        /// ================================================================================
        /// <summary>コントロールが離れたとき - 枠高さ</summary>
        /// ================================================================================
        private void txtBoxColumnFrameHeight_Leave(object sender, EventArgs e)
        {
            if (IsDoubleString(this.txtBoxColumnFrameHeight.Text) == false)
            {
                this.errorProviderInvalid.SetError(this.txtBoxColumnFrameHeight,
                                                   _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") + "\r\n" + _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLE"));

                this.txtBoxColumnFrameHeight.Select();
                this.txtBoxColumnFrameHeight.SelectAll();

                BtnEnabledChange(false);
            }
            else
            {
                this.errorProviderInvalid.SetError(this.txtBoxColumnFrameHeight, "");

                BtnEnabledChange(AllInputJudge);
            }
        }

        /// ================================================================================
        /// <summary>ヘルプ - 柱材種</summary>
        /// ================================================================================
        private void pictBoxHelpColumnMaterial_Click(object sender, EventArgs e)
        {
            FormHelpView formHelp = new SectionListSteel.Setting.FormHelpView(_CmpAttribute, 0, this);
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
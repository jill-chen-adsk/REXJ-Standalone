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
    /// <summary>フォーム 小梁リスト設定</summary>
    /// ================================================================================
    public partial class FormBeamSetting_Sub : Form
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

        /// <summary>備考枠幅</summary>
        private string _NoteFrameWidth;

        /// <summary>鉄骨サイズ枠高さ</summary>
        private string _SteelFrameHeight;

        /// <summary>鋼材種表示</summary>
        private string _ShowSteel;

        /// <summary>備考枠</summary>
        private string _ShowNote;

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
        /// <history>2017/06/22 Created CST,Co.Ltd. Ryo kuroda</history>
        /// ================================================================================
        public FormBeamSetting_Sub(FormAllSetting parent, SectionListSteel.Components.Attribute cmpAttribute,
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

        // ================================================================================
        /// <summary>データ設定</summary>
        ///
        /// <history>2017/06/22 Created CST,Co.Ltd. Ryo kuroda</history>
        /// ================================================================================
        private
        void SetData()
        {
            this.txtBoxBeamFramePosWidth.TextAlign = HorizontalAlignment.Right;
            this.txtBoxBeamFrameSteelWidth.TextAlign = HorizontalAlignment.Right;
            this.txtBoxBeamFrameNoteWidth.TextAlign = HorizontalAlignment.Right;
            this.txtBoxBeamFrameSteelHeight.TextAlign = HorizontalAlignment.Right;

            this.txtBoxBeamFramePosWidth.MaxLength = 5;
            this.txtBoxBeamFrameSteelWidth.MaxLength = 5;
            this.txtBoxBeamFrameNoteWidth.MaxLength = 5;
            this.txtBoxBeamFrameSteelHeight.MaxLength = 5;
        }

        /// ================================================================================
        /// <summary>文字設定</summary>
        ///
        /// <history><p>2017/06/22 Created CST,Co.Ltd. Ryo kuroda</p>
        ///           <p>2017/07/31 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        private
        void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_BEAMSUBSETTING");

            this.grpBoxBeamFrame.Text = _CmpAttribute.ResourceText("IDS_TXT_SHOWFRAME");
            this.lblBeamFrameExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_SHOWFRAMEEXPLAIN");

            this.lblBeamFramePosWidthMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.lblBeamFrameSteelWidthMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.lblBeamFrameNoteWidthMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.lblBeamFrameSteelHeightMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");

            this.grpBoxBeamMaterial.Text = _CmpAttribute.ResourceText("IDS_TXT_ZAISHITSU");
            this.lblBeamMaterialExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_ZAISHITSUEXPLAIN");
            this.chkBoxBeamSteel.Text = _CmpAttribute.ResourceText("IDS_TXT_SHOWSTEEL");

            this.grpBoxShowNote.Text = _CmpAttribute.ResourceText("IDS_TXT_NOTEFRAME");
            this.lblShowNoteExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_SHOWNOTEEXPLAIN2");
            this.chkBoxShowNote.Text = _CmpAttribute.ResourceText("IDS_TXT_SHOWNOTE");
        }

        /// ================================================================================
        /// <summary>設定値設定</summary>
        ///
        /// <history>2017/06/22 Created CST,Co.Ltd. Ryo kuroda</history>
        /// ================================================================================
        public
        void SetSettingValue()
        {
            GetSettingValue();

            this.txtBoxBeamFramePosWidth.Text = _PosFrameWidth;
            this.txtBoxBeamFrameSteelWidth.Text = _SteelFrameWidth;
            this.txtBoxBeamFrameNoteWidth.Text = _NoteFrameWidth;
            this.txtBoxBeamFrameSteelHeight.Text = _SteelFrameHeight;

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
        }

        /// ================================================================================
        /// <summary>設定値取得</summary>
        ///
        /// <history>2017/06/22 Created CST,Co.Ltd. Ryo kuroda</history>
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

                _PosFrameWidth = strAry[63];
                _SteelFrameWidth = strAry[64];
                _NoteFrameWidth = strAry[65];
                _SteelFrameHeight = strAry[66];
                _ShowSteel = strAry[67];
                _ShowNote = strAry[68];
            }
        }

        /// ================================================================================
        /// <summary>設定ファイルの中身</summary>
        ///
        /// <history>2017/06/22 Created CST,Co.Ltd. Ryo kuroda</history>
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

                if (strs.Length == 55 || strs.Length == 56)
                {
                    ret = true;
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>文字列の小数値判定</summary>
        ///
        /// <history>2017/06/22 Created CST,Co.Ltd. Ryo kuroda</history>
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
        /// <history>2017/06/22 Created CST,Co.Ltd. Ryo kuroda</history>
        /// ================================================================================
        private void BtnEnabledChange(bool enableBool)
        {
            // _FormAllSetting.BtnEnabledChange(enableBool);
        }

        /// ================================================================================
        /// <summary>コントロールの入力判定</summary>
        ///
        /// <param name="ctrl">テキストボックス、コンボボックス、ラジオボタン</param>
        ///
        /// <history>2017/06/22 Created CST,Co.Ltd. Ryo kuroda</history>
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
        /// <history>2017/06/22 Created CST,Co.Ltd. Ryo kuroda</history>
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
        /// <history>2017/06/22 Created CST,Co.Ltd. Ryo kuroda</history>
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
        /// <history>2017/06/22 Created CST,Co.Ltd. Ryo kuroda</history>
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

            if (this.txtBoxBeamFrameNoteWidth.Text == "" || this.txtBoxBeamFrameNoteWidth.Text == null)
            {
                this.txtBoxBeamFrameNoteWidth.Text = "20";
            }

            if (this.txtBoxBeamFrameSteelHeight.Text == "" || this.txtBoxBeamFrameSteelHeight.Text == null)
            {
                this.txtBoxBeamFrameSteelHeight.Text = "9";
            }
        }

        /// ================================================================================
        /// <summary>画像サイズ補正</summary>
        ///
        /// <history>2017/06/22 Created CST,Co.Ltd. Ryo kuroda</history>
        /// ================================================================================
        private
        void SetDPISizing()
        {
            // サイズ補正
            System.Drawing.Graphics gra = this.CreateGraphics();
            float dpiX = gra.DpiX;
            float dpiY = gra.DpiY;

            Bitmap bmp = Resources.Image.IDI_FORMIMAGE_BEAMFRAME_SUB;

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
        /// <history>2017/06/22 Created CST,Co.Ltd. Ryo kuroda</history>
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
        /// <summary>設定値 - 小梁リスト設定</summary>
        ///
        /// <history>2017/06/22 Created CST,Co.Ltd. Ryo kuroda</history>
        /// ================================================================================
        public
        Collections.Generic.IList<string> SettingValues_BeamSub
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

                ret.Add(this.txtBoxBeamFramePosWidth.Text);
                ret.Add(this.txtBoxBeamFrameSteelWidth.Text);
                ret.Add(this.txtBoxBeamFrameNoteWidth.Text);
                ret.Add(this.txtBoxBeamFrameSteelHeight.Text);
                ret.Add(showSteel);
                ret.Add(showNote);

                return ret;
            }
        }

        #endregion Properties

        // イベント

        #region Events

        /// ================================================================================
        /// <summary>ロード</summary>
        /// ================================================================================
        private void FormBeamSetting_Sub_Load(object sender, EventArgs e)
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
        /// <summary>入力制限 - 鉄骨サイズ枠幅</summary>
        /// ================================================================================
        private void txtBoxBeamFrameSteelWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        /// ================================================================================
        /// <summary>入力制限 - 備考枠幅</summary>
        /// ================================================================================
        private void txtBoxBeamFrameNoteWidth_KeyPress(object sender, KeyPressEventArgs e)
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
        /// <summary>コントロールが離れたとき - 備考枠幅</summary>
        /// ================================================================================
        private void txtBoxBeamFrameNoteWidth_Leave(object sender, EventArgs e)
        {
            if (IsDoubleString(this.txtBoxBeamFrameNoteWidth.Text) == false)
            {
                this.errorProviderInvalid.SetError(this.txtBoxBeamFrameNoteWidth,
                                                   _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") + "\r\n" + _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLE"));

                this.txtBoxBeamFrameNoteWidth.Select();
                this.txtBoxBeamFrameNoteWidth.SelectAll();

                BtnEnabledChange(false);
            }
            else
            {
                this.errorProviderInvalid.SetError(this.txtBoxBeamFrameNoteWidth, "");

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
            SectionListSteel.Setting.FormHelpView formHelp = new SectionListSteel.Setting.FormHelpView(_CmpAttribute, 3, this);
            formHelp.ShowDialog();
        }

        #endregion Events
    }
}
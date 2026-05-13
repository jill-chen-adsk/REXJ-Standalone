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
    /// <summary>フォーム 間柱リスト設定</summary>
    /// ================================================================================
    public partial class FormSubItemSetting_Post : Form
    {
        // メンバ変数

        #region Member Variables

        /// <summary>属性</summary>
        private SectionListSteel.Components.Attribute _CmpAttribute;

        /// <summary>設定ファイル名</summary>
        private string _SettingFileName;

        /// <summary>設定ファイルディレクトリ</summary>
        private string _SettingFileDirectory;

        /// <summary>枠幅 断面</summary>
        private string _FrameSecWidth;

        /// <summary>枠幅 備考</summary>
        private string _FrameNoteWidth;

        /// <summary>枠高さ</summary>
        private string _FrameHeight;

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
        public FormSubItemSetting_Post(FormAllSetting parent, SectionListSteel.Components.Attribute cmpAttribute,
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
        /// <history>2017/06/22 Created CST,Co.Ltd. Ryo kuroda</history>
        /// ================================================================================
        public
        void SetData()
        {
            this.txtBoxColumnFrameWidth.TextAlign = HorizontalAlignment.Right;
            this.txtBoxColumnFrameHeight.TextAlign = HorizontalAlignment.Right;
            this.txtBoxNoteWidth.TextAlign = HorizontalAlignment.Right;

            this.txtBoxColumnFrameWidth.MaxLength = 5;
            this.txtBoxColumnFrameHeight.MaxLength = 5;
            this.txtBoxNoteWidth.MaxLength = 5;
        }

        /// ================================================================================
        /// <summary>文字設定</summary>
        ///
        /// <history><p>2017/06/22 Created CST,Co.Ltd. Ryo kuroda</p>
        ///           <p>2017/07/31 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_COLUMNSUBSETTING");

            this.grpBoxColumnFrame.Text = _CmpAttribute.ResourceText("IDS_TXT_SHOWFRAME");
            this.lblColumnFrameExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_SHOWFRAMEEXPLAIN2");

            this.lblColumnFrameWidthMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.lblColumnFrameHeightMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.lblNoteWidthMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");

            this.grpBoxColumnMaterial.Text = _CmpAttribute.ResourceText("IDS_TXT_ZAISHITSU");
            this.lblColumnMaterialExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_ZAISHITSUEXPLAIN");
            this.chkBoxColumnSteel.Text = _CmpAttribute.ResourceText("IDS_TXT_SHOWSTEEL");

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
            // 設定ファイルから設定値取得
            GetSettingValue();

            this.txtBoxColumnFrameWidth.Text = _FrameSecWidth;
            this.txtBoxColumnFrameHeight.Text = _FrameHeight;
            this.txtBoxNoteWidth.Text = _FrameNoteWidth;

            if (_ShowSteel == "0")
            {
                this.chkBoxColumnSteel.Checked = false;
            }
            if (_ShowSteel == "1")
            {
                this.chkBoxColumnSteel.Checked = true; ;
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

                _FrameSecWidth = strAry[35];
                _FrameNoteWidth = strAry[36];
                _FrameHeight = strAry[37];
                _ShowSteel = strAry[38];
                _ShowNote = strAry[39];
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
            //_FormAllSetting.BtnEnabledChange(enableBool);
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
            if (this.txtBoxColumnFrameWidth.Text == "" || this.txtBoxColumnFrameWidth.Text == null)
            {
                this.txtBoxColumnFrameWidth.Text = "30";
            }

            if (this.txtBoxNoteWidth.Text == "" || this.txtBoxNoteWidth.Text == null)
            {
                this.txtBoxNoteWidth.Text = "20";
            }

            if (this.txtBoxColumnFrameHeight.Text == "" || this.txtBoxColumnFrameHeight.Text == null)
            {
                this.txtBoxColumnFrameHeight.Text = "9";
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

            Bitmap bmp = Resources.Image.IDI_FORMIMAGE_COLUMNFRAME_POST;

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
        /// <summary>設定値 - 間柱リスト設定</summary>
        ///
        /// <history>2017/06/22 Created CST,Co.Ltd. Ryo kuroda</history>
        /// ================================================================================
        public
        Collections.Generic.IList<string> SettingValues_SubItemPost
        {
            get
            {
                Collections.Generic.IList<string> ret = new Collections.Generic.List<string>();

                string showSteel = "";

                if (this.chkBoxColumnSteel.Checked)
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

                ret.Add(this.txtBoxColumnFrameWidth.Text);
                ret.Add(this.txtBoxNoteWidth.Text);
                ret.Add(this.txtBoxColumnFrameHeight.Text);
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
        private void FormSubItemSetting_Post_Load(object sender, EventArgs e)
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
        /// <summary>入力制限 - 枠幅 備考</summary>
        /// ================================================================================
        private void txtBoxNoteWidth_KeyPress(object sender, KeyPressEventArgs e)
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
        /// <summary>コントロールが離れたとき - 枠幅 備考</summary>
        /// ================================================================================
        private void txtBoxNoteWidth_Leave(object sender, EventArgs e)
        {
            if (IsDoubleString(this.txtBoxNoteWidth.Text) == false)
            {
                this.errorProviderInvalid.SetError(this.txtBoxNoteWidth,
                                                   _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") + "\r\n" + _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLE"));

                this.txtBoxNoteWidth.Select();
                this.txtBoxNoteWidth.SelectAll();

                BtnEnabledChange(false);
            }
            else
            {
                this.errorProviderInvalid.SetError(this.txtBoxNoteWidth, "");

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
            FormHelpView formHelp = new SectionListSteel.Setting.FormHelpView(_CmpAttribute, 2, this);
            formHelp.ShowDialog();
        }

        #endregion Events
    }
}
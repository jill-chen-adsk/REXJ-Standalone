using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using System.Reflection;
using UTILS = SectionListRC.Utils;

namespace SectionListRC.Setting
{
    public partial class FormBeamListSetting2 : Form
    {
        // メンバ変数

        #region Member Variables

        /// <summary>属性</summary>
        private SectionListRC.Components.Attribute _CmpAttribute;

        /// <summary>設定ファイル名</summary>
        private string _SettingFileName;

        /// <summary>設定ファイルディレクトリ</summary>
        private string _SettingFileDirectory;

        // 幅寸法線表示
        private int _WidthDimensionShow;

        // 高さ寸法線表示
        private int _HeightDimensionShow;

        // 主筋表示
        private int _RebarShow;

        // 肋筋枠表示
        private int _StirrupFrameShow;

        // 腹筋枠表示
        private int _WebFrameShow;

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
        /// <history>2013/02/12 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        public FormBeamListSetting2(SectionListRC.Components.Attribute cmpAttribute,
                                    string settingFileName,
                                    string settingFileDirectory)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            InitializeComponent();

            _CmpAttribute = cmpAttribute;

            _Result = 0;

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
        /// <history>2013/02/13 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        private void SetData()
        {
            this.cmbBoxDimensionWidth.Items.Add(_CmpAttribute.ResourceText("IDS_TXT_SHOWALL"));
            this.cmbBoxDimensionWidth.Items.Add(_CmpAttribute.ResourceText("IDS_TXT_ELLIPSISDIMENSION") + "(" + _CmpAttribute.ResourceText("IDS_TXT_DEFAULTLEFT") + ")");
            this.cmbBoxDimensionWidth.Items.Add(_CmpAttribute.ResourceText("IDS_TXT_ELLIPSISDIMENSION") + "(" + _CmpAttribute.ResourceText("IDS_TXT_DEFAULTCENTER") + ")");
            this.cmbBoxDimensionWidth.DropDownStyle = ComboBoxStyle.DropDownList;
            SetCmbBoxDropWidth(this.cmbBoxDimensionWidth);

            this.cmbBoxDimensionHeight.Items.Add(_CmpAttribute.ResourceText("IDS_TXT_SHOWALL"));
            this.cmbBoxDimensionHeight.Items.Add(_CmpAttribute.ResourceText("IDS_TXT_ELLIPSISDIMENSION"));
            this.cmbBoxDimensionHeight.DropDownStyle = ComboBoxStyle.DropDownList;
            SetCmbBoxDropWidth(this.cmbBoxDimensionHeight);

            this.cmbBoxBeamRebarShow.Items.Add(_CmpAttribute.ResourceText("IDS_TXT_NOSHOW"));
            this.cmbBoxBeamRebarShow.Items.Add(_CmpAttribute.ResourceText("IDS_TXT_EVERYSTEP"));
            this.cmbBoxBeamRebarShow.DropDownStyle = ComboBoxStyle.DropDownList;
            SetCmbBoxDropWidth(this.cmbBoxBeamRebarShow);

            this.cmbBoxStirrupFrameShow.Items.Add(_CmpAttribute.ResourceText("IDS_TXT_SHOWPACKAGE"));
            this.cmbBoxStirrupFrameShow.Items.Add(_CmpAttribute.ResourceText("IDS_TXT_SHOWSECTION"));
            this.cmbBoxStirrupFrameShow.DropDownStyle = ComboBoxStyle.DropDownList;
            SetCmbBoxDropWidth(this.cmbBoxStirrupFrameShow);

            this.cmbBoxWebReinforcementFrameShow.Items.Add(_CmpAttribute.ResourceText("IDS_TXT_SHOWPACKAGE"));
            this.cmbBoxWebReinforcementFrameShow.Items.Add(_CmpAttribute.ResourceText("IDS_TXT_SHOWSECTION"));
            this.cmbBoxWebReinforcementFrameShow.Items.Add(_CmpAttribute.ResourceText("IDS_TXT_NOSHOW"));
            this.cmbBoxWebReinforcementFrameShow.DropDownStyle = ComboBoxStyle.DropDownList;
            SetCmbBoxDropWidth(this.cmbBoxWebReinforcementFrameShow);
        }

        /// ================================================================================
        /// <summary>文字設定</summary>
        ///
        /// <history><p>2013/02/13 Created GSA,Inc Ryo Kuroda</p>
        ///           <p>2017/02/16 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        private void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_BEAMLISTSETTING2") ;

            this.grpBoxBeamDimensions.Text = _CmpAttribute.ResourceText("IDS_TXT_DIMENSIONS");
            this.lblDimensionsExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_DIMENSIONSEXPLAIN");
            this.lblDimensionWidth.Text = _CmpAttribute.ResourceText("IDS_TXT_DIMENSIONWIDTH");
            this.lblDimensionHeight.Text = _CmpAttribute.ResourceText("IDS_TXT_DIMENSIONHEIGHT");

            this.grpBoxStirrupFrameShow.Text = _CmpAttribute.ResourceText("IDS_TXT_STIRRUPFRAMESHOW");
            this.lblStirrupFrameShowExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_STIRRUPFRAMESHOWEXPLAIN");

            this.grpBoxWebReinforcementFrame.Text = _CmpAttribute.ResourceText("IDS_TXT_WEBREINFORCEMENTFRAME");
            this.lblWebReinforcementFrameExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_WEBREINFORCEMENTFRAMEEXPLAIN");

            this.grpBoxBeamRebar.Text = _CmpAttribute.ResourceText("IDS_TXT_MAINREBAR");
            this.lblBeamRebarExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_SECTIONFRAMEREBAREXPLAIN");

            this.btnToSettingFromBeamListSetting2.Text = _CmpAttribute.ResourceText("IDS_TXT_COMMONSETTING");
            this.btnToColumnListSettingFromBeamListSetting2.Text = _CmpAttribute.ResourceText("IDS_TXT_COLUMNLISTSETTING");
            this.btnToBeamListSetting1FromBeamListSetting2.Text = _CmpAttribute.ResourceText("IDS_TXT_BEAMLISTSETTING");
            this.btnOverWriteSave.Text = _CmpAttribute.ResourceText("IDS_TXT_OVERWRITESAVE");
            this.btnSaveAs.Text = _CmpAttribute.ResourceText("IDS_TXT_SAVEAS");
            this.btnEnd.Text = _CmpAttribute.ResourceText("IDS_TXT_END");

            this.toolTipHelp.SetToolTip(this.pictureBox1, _CmpAttribute.ResourceText("IDS_TXT_SHOWHELPVIEW"));
            this.toolTipHelp.SetToolTip(this.pictureBox2, _CmpAttribute.ResourceText("IDS_TXT_SHOWHELPVIEW"));
            this.toolTipHelp.SetToolTip(this.pictureBox3, _CmpAttribute.ResourceText("IDS_TXT_SHOWHELPVIEW"));
            this.toolTipHelp.SetToolTip(this.pictureBox4, _CmpAttribute.ResourceText("IDS_TXT_SHOWHELPVIEW"));
        }

        /// ================================================================================
        /// <summary>設定値設定</summary>
        ///
        /// <history><p>2013/04/11 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private void SetSettingValue()
        {
            GetSettingValue();

            this.cmbBoxDimensionWidth.SelectedIndex = _WidthDimensionShow;
            this.cmbBoxDimensionHeight.SelectedIndex = _HeightDimensionShow;
            this.cmbBoxBeamRebarShow.SelectedIndex = _RebarShow;
            this.cmbBoxStirrupFrameShow.SelectedIndex = _StirrupFrameShow;
            this.cmbBoxWebReinforcementFrameShow.SelectedIndex = _WebFrameShow;
        }

        /// ================================================================================
        /// <summary>設定値取得</summary>
        ///
        /// <history><p>2013/04/10 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        public void GetSettingValue()
        {
            string fullName = _SettingFileDirectory + _SettingFileName;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            System.Text.Encoding enc = System.Text.Encoding.GetEncoding("shift_jis");

            if (System.IO.File.Exists(fullName))
            {
                string[] strAry = System.IO.File.ReadAllLines(fullName, enc);

                //if (strAry.Length == 61 || strAry.Length == 62)
                {
                    int.TryParse(strAry[53], out _WidthDimensionShow);
                    int.TryParse(strAry[54], out _HeightDimensionShow);
                    int.TryParse(strAry[55], out _RebarShow);
                    int.TryParse(strAry[56], out _StirrupFrameShow);
                    int.TryParse(strAry[57], out _WebFrameShow);
                }
            }
        }

        /// ================================================================================
        /// <summary>コンボボックス - ドロップダウン幅設定</summary>
        ///
        /// <history><p>2013/02/13 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private void SetCmbBoxDropWidth(System.Windows.Forms.ComboBox cmbBox)
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
        /// <summary>ボタン使用可否切り替え</summary>
        ///
        /// <history><p>2013/02/27 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private void BtnEnabledChange(bool enableBool)
        {
            this.btnOverWriteSave.Enabled = enableBool;
            this.btnToSettingFromBeamListSetting2.Enabled = enableBool;
            this.btnToColumnListSettingFromBeamListSetting2.Enabled = enableBool;
            this.btnToBeamListSetting1FromBeamListSetting2.Enabled = enableBool;
        }

        /// ================================================================================
        /// <summary>コントロールの入力判定</summary>
        ///
        /// <param name="ctrl">テキストボックス、コンボボックス、ラジオボタン</param>
        ///
        /// <history><p>2013/02/27 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private bool InputJudge(System.Windows.Forms.Control ctrl)
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
        /// <history><p>2013/02/27 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private System.Windows.Forms.Control[] GetCtrls(System.Windows.Forms.Control ctrl)
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
        /// <summary>未入力の入力</summary>
        ///
        /// <history><p>2013/05/16 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private
        void SetUnWrite()
        {
            if (this.cmbBoxDimensionWidth.SelectedItem == null)
            {
                this.cmbBoxDimensionWidth.SelectedIndex = 0;
            }
            if (this.cmbBoxDimensionHeight.SelectedItem == null)
            {
                this.cmbBoxDimensionHeight.SelectedIndex = 0;
            }
            if (this.cmbBoxBeamRebarShow.SelectedItem == null)
            {
                this.cmbBoxBeamRebarShow.SelectedIndex = 0;
            }
            if (this.cmbBoxStirrupFrameShow.SelectedItem == null)
            {
                this.cmbBoxStirrupFrameShow.SelectedIndex = 0;
            }
            if (this.cmbBoxWebReinforcementFrameShow.SelectedItem == null)
            {
                this.cmbBoxWebReinforcementFrameShow.SelectedIndex = 0;
            }
        }

        /// ================================================================================
        /// <summary>画像サイズ補正</summary>
        ///
        /// <history><p>2015/04/30 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private
        void SetDPISizing()
        {
            // サイズ補正
            System.Drawing.Graphics gra = this.CreateGraphics();
            float dpiX = gra.DpiX;
            float dpiY = gra.DpiY;

            Bitmap bmp = Resources.Image.help_24x24;

            // 係数
            double coefficientX = dpiX / 96;
            double coefficientY = dpiY / 96;

            this.pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            Bitmap newBmp = new Bitmap((int)(bmp.Width * coefficientX), (int)(bmp.Height * coefficientY));
            this.pictureBox1.Image = newBmp;
            Graphics g = Graphics.FromImage(this.pictureBox1.Image);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.Clear(this.pictureBox1.BackColor);
            g.DrawImage(bmp, 0, 0, (float)(bmp.Width * coefficientX), (float)(bmp.Height * coefficientY));
            this.pictureBox1.Refresh();

            this.pictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;
            //newBmp = new Bitmap((int)(bmp.Width * coefficient), (int)(bmp.Height * coefficient));
            this.pictureBox2.Image = newBmp;
            g = Graphics.FromImage(this.pictureBox2.Image);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.Clear(this.pictureBox2.BackColor);
            g.DrawImage(bmp, 0, 0, (float)(bmp.Width * coefficientX), (float)(bmp.Height * coefficientY));
            this.pictureBox2.Refresh();

            this.pictureBox3.SizeMode = PictureBoxSizeMode.AutoSize;
            //newBmp = new Bitmap((int)(bmp.Width * coefficient), (int)(bmp.Height * coefficient));
            this.pictureBox3.Image = newBmp;
            g = Graphics.FromImage(this.pictureBox3.Image);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.Clear(this.pictureBox3.BackColor);
            g.DrawImage(bmp, 0, 0, (float)(bmp.Width * coefficientX), (float)(bmp.Height * coefficientY));
            this.pictureBox3.Refresh();

            this.pictureBox4.SizeMode = PictureBoxSizeMode.AutoSize;
            //newBmp = new Bitmap((int)(bmp.Width * coefficient), (int)(bmp.Height * coefficient));
            this.pictureBox4.Image = newBmp;
            g = Graphics.FromImage(this.pictureBox4.Image);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.Clear(this.pictureBox4.BackColor);
            g.DrawImage(bmp, 0, 0, (float)(bmp.Width * coefficientX), (float)(bmp.Height * coefficientY));
            this.pictureBox4.Refresh();
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        /// ================================================================================
        /// <summary>操作結果 - 梁リスト設定1</summary>
        /// ================================================================================
        public int BeamListSettingResult2
        {
            get
            {
                return _Result;
            }
        }

        /// ================================================================================
        /// <summary>全項目の入力判定</summary>
        /// ================================================================================
        private bool AllInputJudge
        {
            get
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
        }

        /// ================================================================================
        /// <summary>設定値 - 梁リスト設定2</summary>
        /// ================================================================================
        public Collections.Generic.IList<string> SettingValues_Beam2
        {
            get
            {
                SetUnWrite();

                Collections.Generic.IList<string> ret = new Collections.Generic.List<string>();

                ret.Add(this.cmbBoxDimensionWidth.SelectedIndex.ToString());
                ret.Add(this.cmbBoxDimensionHeight.SelectedIndex.ToString());
                ret.Add(this.cmbBoxBeamRebarShow.SelectedIndex.ToString());
                ret.Add(this.cmbBoxStirrupFrameShow.SelectedIndex.ToString());
                ret.Add(this.cmbBoxWebReinforcementFrameShow.SelectedIndex.ToString());

                return ret;
            }
        }

        #endregion Properties

        // イベント

        #region Events

        // ロード
        private void FormBeamListSetting2_Load(object sender, EventArgs e)
        {
            SetText();
            SetSettingValue();

            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(2, 2, 24, 24);

            //this.btnDimensionHelp.Region    = new System.Drawing.Region(path);
            //this.btnRebarHelp.Region        = new System.Drawing.Region(path);
            //this.btnStirrupHelp.Region      = new System.Drawing.Region(path);
            //this.btnWebReinforceHelp.Region = new System.Drawing.Region(path);

            this.cmbBoxDimensionWidth.Select();

            SetUnWrite();

            BtnEnabledChange(AllInputJudge);

            SetDPISizing();
        }

        #region Change イベント

        private void cmbBoxDimensionWidth_SelectedIndexChanged(object sender, EventArgs e)
        {
            BtnEnabledChange(AllInputJudge);
        }

        private void cmbBoxDimensionHeight_SelectedIndexChanged(object sender, EventArgs e)
        {
            BtnEnabledChange(AllInputJudge);
        }

        private void cmbBoxBeamRebarShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            BtnEnabledChange(AllInputJudge);
        }

        private void cmbBoxStirrupFrameShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            BtnEnabledChange(AllInputJudge);
        }

        private void cmbBoxWebReinforcementFrameShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            BtnEnabledChange(AllInputJudge);
        }

        #endregion Change イベント

        // 共通設定へ
        private void btnToSetteingFromBeamListSetting2_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            _Result = 0;

            this.Close();
        }

        // 柱リスト設定へ
        private void btnToColumnListSetteingFromBeamListSetting2_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            _Result = 1;

            this.Close();
        }

        // 梁リスト設定1へ
        private void btnToBeamListSetteing1FromBeamListSetting2_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            _Result = 2;

            this.Close();
        }

        // 上書き保存
        private void btnOverWriteSave_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            _Result = 12;

            this.Close();
        }

        // 名前を付けて保存
        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            _Result = 13;

            this.Close();
        }

        // キャンセルボタン
        private void btnCancal_BeamListSetting2_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        // ヒントビュー
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            SectionListRC.Setting.FormHintView formHint = new FormHintView(_CmpAttribute, 1, this);
            formHint.ShowDialog();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            SectionListRC.Setting.FormHintView formHint = new FormHintView(_CmpAttribute, 2, this);
            formHint.ShowDialog();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            SectionListRC.Setting.FormHintView formHint = new FormHintView(_CmpAttribute, 3, this);
            formHint.ShowDialog();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            SectionListRC.Setting.FormHintView formHint = new FormHintView(_CmpAttribute, 3, this);
            formHint.ShowDialog();
        }

        #endregion Events
    }
}
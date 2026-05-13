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
    public partial class FormHintView : Form
    {
        // メンバ変数
        #region Member Variables

        /// <summary>属性</summary>
        private SectionListRC.Components.Attribute _CmpAttribute;

        private int _Num;
        private System.Windows.Forms.Control _Ctrl;
        private int _GirderHint;

        #endregion Member Variables

        // コンストラクタ
        #region Constructor

        public FormHintView(SectionListRC.Components.Attribute cmpAttribute, int num, System.Windows.Forms.Control ctrl)
        {
            InitializeComponent();
            _CmpAttribute = cmpAttribute;
            _Num = num;
            _Ctrl = ctrl;
            _GirderHint = 0;

            SetData();

            switch (_Num) {
                case 0:
                    this.pictureBox1.Image = SectionListRC.Resources.Image.Hint_PositionFrame;
                    break;

                case 1:
                    this.pictureBox1.Image = SectionListRC.Resources.Image.Hint_Dimension;
                    break;

                case 2:
                    this.pictureBox1.Image = SectionListRC.Resources.Image.Hint_Rebar;
                    break;

                case 3:
                    this.pictureBox1.Image = SectionListRC.Resources.Image.Hint_StirrupFrame_WebReinforcementFrame;
                    break;

                case 4:
                    this.pictureBox1.Image = SectionListRC.Resources.Image.ParamMap_RectangleColumn;
                    break;

                case 5:
                    this.pictureBox1.Image = SectionListRC.Resources.Image.ParamMap_CircleColumn;
                    break;

                case 6:
                    this.pictureBox1.Image = SectionListRC.Resources.Image.ParamMap_Girder1;
                    break;

                case 7:
                    this.pictureBox1.Image = SectionListRC.Resources.Image.ParamMap_CantiGirder1;
                    break;

                default:
                    goto case 0;
            }

            SetDPISizing();
        }

        #endregion Constructor

        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>データ設定</summary>
        ///
        /// <history><p>2013/04/ Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private void SetData()
        {
        }

        /// ================================================================================
        /// <summary>文字設定</summary>
        ///
        /// <history><p>2013/04/ Created GSA,Inc Ryo Kuroda</p>
        ///           <p>2017/02/16 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        private void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_HELP") + _CmpAttribute.ResourceText("IDS_TXT_BUILDVERSION") ;
            this.btnParamMapGirderChange.Text = _CmpAttribute.ResourceText("IDS_TXT_NEXT");
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

            Bitmap bmp = null;

            switch (_Num) {
                case 0:
                    bmp = SectionListRC.Resources.Image.Hint_PositionFrame;
                    break;

                case 1:
                    bmp = SectionListRC.Resources.Image.Hint_Dimension;
                    break;

                case 2:
                    bmp = SectionListRC.Resources.Image.Hint_Rebar;
                    break;

                case 3:
                    bmp = SectionListRC.Resources.Image.Hint_StirrupFrame_WebReinforcementFrame;
                    break;

                case 4:
                    bmp = SectionListRC.Resources.Image.ParamMap_RectangleColumn;
                    break;

                case 5:
                    bmp = SectionListRC.Resources.Image.ParamMap_CircleColumn;
                    break;

                case 6:
                    bmp = SectionListRC.Resources.Image.ParamMap_Girder1;
                    break;

                case 7:
                    bmp = SectionListRC.Resources.Image.ParamMap_CantiGirder1;
                    break;

                default:
                    goto case 0;
            }

            // 係数
            double coefficientX = dpiX / 96;
            double coefficientY = dpiY / 96;

            this.pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;

            Bitmap newBmp = new Bitmap((int)(bmp.Width * coefficientX), (int)(bmp.Height * coefficientY));
            this.pictureBox1.Image = newBmp;
            Graphics g = Graphics.FromImage(this.pictureBox1.Image);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.Clear(this.pictureBox1.BackColor);

            g.DrawImage(bmp, 3, 3, (float)(bmp.Width * coefficientX), (float)(bmp.Height * coefficientY));
            this.pictureBox1.Refresh();
        }

        #endregion Member Functions

        // イベント
        #region Events

        // ロード
        private void FormHintView_Load(object sender, EventArgs e)
        {
            SetText();

            this.pictureBox1.Location = new System.Drawing.Point(13, 13);
            this.pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;

            this.btnParamMapGirderChange.Visible = false;
            this.btnParamMapGirderChange.Enabled = false;

            switch (_Num) {
                case 0:
                    this.pictureBox1.Image = SectionListRC.Resources.Image.Hint_PositionFrame;
                    break;

                case 1:
                    this.pictureBox1.Image = SectionListRC.Resources.Image.Hint_Dimension;
                    break;

                case 2:
                    this.pictureBox1.Image = SectionListRC.Resources.Image.Hint_Rebar;
                    break;

                case 3:
                    this.pictureBox1.Image = SectionListRC.Resources.Image.Hint_StirrupFrame_WebReinforcementFrame;
                    break;

                case 4:
                    this.pictureBox1.Image = SectionListRC.Resources.Image.ParamMap_RectangleColumn;
                    break;

                case 5:
                    this.pictureBox1.Image = SectionListRC.Resources.Image.ParamMap_CircleColumn;
                    break;

                case 6:
                    this.pictureBox1.Image = SectionListRC.Resources.Image.ParamMap_Girder1;

                    this.pictureBox1.Location = new System.Drawing.Point(13, 50);
                    this.btnParamMapGirderChange.Visible = true;
                    this.btnParamMapGirderChange.Enabled = true;

                    break;

                case 7:
                    this.pictureBox1.Image = SectionListRC.Resources.Image.ParamMap_CantiGirder1;

                    this.pictureBox1.Location = new System.Drawing.Point(13, 50);
                    this.btnParamMapGirderChange.Visible = true;
                    this.btnParamMapGirderChange.Enabled = true;

                    break;

                default:
                    goto case 0;
            }

            // フォームの枠の分を加算
            System.Drawing.Size frameSize = new System.Drawing.Size(16, 38);

            System.Drawing.Size plus = new System.Drawing.Size(26, 26);

            if (_Num == 6 || _Num == 7) {
                plus = new System.Drawing.Size(26, 63);
            }

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;

            this.Size = pictureBox1.Size + frameSize + plus;

            // 20140812
            //this.AutoSize = true;

            System.Drawing.Point parentLoc = _Ctrl.Location;
            System.Drawing.Size parentSize = _Ctrl.Size;
            System.Drawing.Point parentCenterLoc = new System.Drawing.Point(parentLoc.X + parentSize.Width / 2, parentLoc.Y + parentSize.Height / 2);

            System.Drawing.Size size = this.Size;

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new System.Drawing.Point(parentCenterLoc.X - size.Width / 2, parentCenterLoc.Y - size.Height / 2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;

            SetDPISizing();
        }

        // パラメータマッピング - 梁、片持ち梁
        private void btnParamMapGirderChange_Click(object sender, EventArgs e)
        {
            if (_Num == 6) {
                if (_GirderHint == 0) {
                    _GirderHint = 1;

                    this.pictureBox1.Image = SectionListRC.Resources.Image.ParamMap_Girder2;
                    System.Drawing.Size frameSize = new System.Drawing.Size(16, 38);

                    System.Drawing.Size plus = new System.Drawing.Size(26, 63);

                    this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;

                    this.Size = pictureBox1.Size + frameSize + plus;

                    System.Drawing.Point parentLoc = _Ctrl.Location;
                    System.Drawing.Size parentSize = _Ctrl.Size;
                    System.Drawing.Point parentCenterLoc = new System.Drawing.Point(parentLoc.X + parentSize.Width / 2, parentLoc.Y + parentSize.Height / 2);

                    System.Drawing.Size size = this.Size;

                    this.StartPosition = FormStartPosition.Manual;
                    this.Location = new System.Drawing.Point(parentCenterLoc.X - size.Width / 2, parentCenterLoc.Y - size.Height / 2);

                    this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;

                    this.btnParamMapGirderChange.Text = _CmpAttribute.ResourceText("IDS_TXT_PREV");
                }
                else if (_GirderHint == 1) {
                    _GirderHint = 0;

                    this.pictureBox1.Image = SectionListRC.Resources.Image.ParamMap_Girder1;
                    System.Drawing.Size frameSize = new System.Drawing.Size(16, 38);

                    System.Drawing.Size plus = new System.Drawing.Size(26, 63);

                    this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;

                    this.Size = pictureBox1.Size + frameSize + plus;

                    System.Drawing.Point parentLoc = _Ctrl.Location;
                    System.Drawing.Size parentSize = _Ctrl.Size;
                    System.Drawing.Point parentCenterLoc = new System.Drawing.Point(parentLoc.X + parentSize.Width / 2, parentLoc.Y + parentSize.Height / 2);

                    System.Drawing.Size size = this.Size;

                    this.StartPosition = FormStartPosition.Manual;
                    this.Location = new System.Drawing.Point(parentCenterLoc.X - size.Width / 2, parentCenterLoc.Y - size.Height / 2);

                    this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;

                    this.btnParamMapGirderChange.Text = _CmpAttribute.ResourceText("IDS_TXT_NEXT");
                }
            }

            if (_Num == 7) {
                if (_GirderHint == 0) {
                    _GirderHint = 1;

                    this.pictureBox1.Image = SectionListRC.Resources.Image.ParamMap_CantiGirder2;
                    System.Drawing.Size frameSize = new System.Drawing.Size(16, 38);

                    System.Drawing.Size plus = new System.Drawing.Size(26, 63);

                    this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;

                    this.Size = pictureBox1.Size + frameSize + plus;

                    System.Drawing.Point parentLoc = _Ctrl.Location;
                    System.Drawing.Size parentSize = _Ctrl.Size;
                    System.Drawing.Point parentCenterLoc = new System.Drawing.Point(parentLoc.X + parentSize.Width / 2, parentLoc.Y + parentSize.Height / 2);

                    System.Drawing.Size size = this.Size;

                    this.StartPosition = FormStartPosition.Manual;
                    this.Location = new System.Drawing.Point(parentCenterLoc.X - size.Width / 2, parentCenterLoc.Y - size.Height / 2);

                    this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;

                    this.btnParamMapGirderChange.Text = _CmpAttribute.ResourceText("IDS_TXT_PREV");
                }
                else if (_GirderHint == 1) {
                    _GirderHint = 0;

                    this.pictureBox1.Image = SectionListRC.Resources.Image.ParamMap_CantiGirder1;
                    System.Drawing.Size frameSize = new System.Drawing.Size(16, 38);

                    System.Drawing.Size plus = new System.Drawing.Size(26, 63);

                    this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;

                    this.Size = pictureBox1.Size + frameSize + plus;

                    System.Drawing.Point parentLoc = _Ctrl.Location;
                    System.Drawing.Size parentSize = _Ctrl.Size;
                    System.Drawing.Point parentCenterLoc = new System.Drawing.Point(parentLoc.X + parentSize.Width / 2, parentLoc.Y + parentSize.Height / 2);

                    System.Drawing.Size size = this.Size;

                    this.StartPosition = FormStartPosition.Manual;
                    this.Location = new System.Drawing.Point(parentCenterLoc.X - size.Width / 2, parentCenterLoc.Y - size.Height / 2);

                    this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;

                    this.btnParamMapGirderChange.Text = _CmpAttribute.ResourceText("IDS_TXT_NEXT");
                }
            }
        }

        #endregion Events
    }
}
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
    /// <summary>フォーム ヘルプ</summary>
    /// ================================================================================
    public partial class FormHelpView : Form
    {
        // メンバ変数

        #region Member Variables

        /// <summary>属性</summary>
        private SectionListSteel.Components.Attribute _CmpAttribute;

        /// <summary>番号</summary>
        private int _Num;

        /// <summary>親フォーム</summary>
        private System.Windows.Forms.Control _Parent;

        #endregion Member Variables

        // コンストラクタ

        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute">属性</param>
        /// <param name="num"         >番号</param>
        /// <param name="parent"      >親フォーム</param>
        ///
        /// <history>2016/08/30 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        FormHelpView(SectionListSteel.Components.Attribute cmpAttribute,
                     int num,
                     System.Windows.Forms.Control parent)
        {
            InitializeComponent();

            _CmpAttribute = cmpAttribute;
            _Num = num;
            _Parent = parent;

            SetDPISizing();
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>文字設定</summary>
        ///
        /// <history><p>2016/08/30 Created GSA,Inc Ryo Kuroda</p>
        ///           <p>2017/07/04 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        private void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_HELP") ;
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

            Bitmap bmp = null;

            switch (_Num)
            {
                case 0:
                    bmp = SectionListSteel.Resources.Image.IDI_FORMIMAGE_HELP_COLUMNMATERIAL;
                    break;

                case 1:
                    bmp = SectionListSteel.Resources.Image.IDI_FORMIMAGE_HELP_BEAMMATERIAL;
                    break;

                case 2:
                    bmp = SectionListSteel.Resources.Image.IDI_FORMIMAGE_HELP_COLUMNMATERIAL_POST;
                    break;

                case 3:
                    bmp = SectionListSteel.Resources.Image.IDI_FORMIMAGE_HELP_BEAMMATERIAL_SUB;
                    break;

                case 4:
                    bmp = SectionListSteel.Resources.Image.IDI_FORMIMAGE_HELP_BRACEMATERIAL;
                    break;

                default:
                    goto case 0;
            }

            // 係数
            double coefficientX = dpiX / 96;
            double coefficientY = dpiY / 96;

            this.pictBoxHelpView.SizeMode = PictureBoxSizeMode.AutoSize;

            Bitmap newBmp = new Bitmap((int)(bmp.Width * coefficientX), (int)(bmp.Height * coefficientY));
            this.pictBoxHelpView.Image = newBmp;
            Graphics g = Graphics.FromImage(this.pictBoxHelpView.Image);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.Clear(this.pictBoxHelpView.BackColor);

            g.DrawImage(bmp, 3, 3, (float)(bmp.Width * coefficientX), (float)(bmp.Height * coefficientY));
            this.pictBoxHelpView.Refresh();
        }

        #endregion Member Functions

        // プロパティ

        // イベント

        #region Events

        /// ================================================================================
        /// <summary>ロード</summary>
        /// ================================================================================
        private void FormHelpView_Load(object sender, EventArgs e)
        {
            SetText();

            this.pictBoxHelpView.Location = new System.Drawing.Point(13, 13);
            this.pictBoxHelpView.SizeMode = PictureBoxSizeMode.AutoSize;

            // フォームの枠幅を加算
            System.Drawing.Size frameSize = new System.Drawing.Size(16, 38);

            System.Drawing.Size plus = new System.Drawing.Size(26, 26);

            this.FormBorderStyle = FormBorderStyle.Sizable;

            this.Size = pictBoxHelpView.Size + frameSize + plus;

            System.Drawing.Point parentLoc = _Parent.Location;
            System.Drawing.Size parentSize = _Parent.Size;
            System.Drawing.Point parentCenterLoc = new System.Drawing.Point(parentLoc.X + parentSize.Width / 2, parentLoc.Y + parentSize.Height / 2);

            System.Drawing.Size size = this.Size;

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new System.Drawing.Point(parentCenterLoc.X - size.Width / 2, parentCenterLoc.Y - size.Height / 2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;

            SetDPISizing();
        }

        private void FormHelpView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        #endregion Events
    }
}
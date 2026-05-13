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
    /// <summary>フォーム カスタムビュー尺度</summary>
    /// ================================================================================
    public partial class FormCustomViewScale : Form
    {
        // メンバ変数

        #region Member Variables

        /// <summary>属性</summary>
        private SectionListSteel.Components.Attribute _CmpAttribute;

        /// <summary>ビュー尺度</summary>
        private int _ViewScale;

        #endregion Member Variables

        // コンストラクタ

        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute">属性</param>
        ///
        /// <history>2016/08/30 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        public FormCustomViewScale(SectionListSteel.Components.Attribute cmpAttribute)
        {
            InitializeComponent();

            _CmpAttribute = cmpAttribute;

            SetData();
        }

        #endregion Constructor

        //メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>データ設定</summary>
        ///
        /// <history>2016/08/30 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        private void SetData()
        {
            this.txtBoxCustomViewPlanScale.MaxLength = 6;
        }

        /// ================================================================================
        /// <summary>文字設定</summary>
        ///
        /// <history>2016/08/30 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        private void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_CUSTOMVIEWSCALE");
            this.lblCustomViewPlanScale.Text = _CmpAttribute.ResourceText("IDS_TXT_VIEWSCALEVALUE_2");

            this.btnOK.Text = _CmpAttribute.ResourceText("IDS_TXT_OK");
            this.btnCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        /// ================================================================================
        /// <summary>カスタムビュー尺度</summary>
        ///
        /// <history>2016/08/30 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        public int CustomViewScale
        {
            get
            {
                return _ViewScale;
            }
        }

        #endregion Properties

        // イベント

        #region Events

        /// ================================================================================
        /// <summary>ロード</summary>
        /// ================================================================================
        private void FormCustomViewScale_Load(object sender, EventArgs e)
        {
            SetText();
            this.txtBoxCustomViewPlanScale.Select();
        }

        /// ================================================================================
        /// <summary>入力制限</summary>
        /// ================================================================================
        private void txtBoxCustomViewPlanScale_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        /// ================================================================================
        /// <summary>Enterキー押下</summary>
        /// ================================================================================
        private void txtBoxCustomViewPlanScale_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.btnOK_Click(sender, e);
            }
        }

        /// ================================================================================
        /// <summary>OKボタン</summary>
        /// ================================================================================
        private void btnOK_Click(object sender, EventArgs e)
        {
            int viewScale = 0;

            if (int.TryParse(this.txtBoxCustomViewPlanScale.Text, out viewScale))
            {
                if (viewScale != 0)
                {
                    _ViewScale = viewScale;
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_VALUEISZERO"), _CmpAttribute.ResourceText("IDS_BTN_PANELNAME"));
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTINT"), _CmpAttribute.ResourceText("IDS_BTN_PANELNAME"));
            }
        }

        /// ================================================================================
        /// <summary>キャンセルボタン</summary>
        /// ================================================================================
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion Events
    }
}
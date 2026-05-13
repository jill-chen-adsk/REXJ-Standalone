using System;
using System.Text;
using System.Windows.Forms;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using System.Reflection;
using UTILS = SectionListRC.Utils;

namespace SectionListRC.Setting
{
    public partial class FormCustomViewScale : Form
    {
        // メンバ変数
        #region Member Variables

        /// <summary>属性</summary>
        private SectionListRC.Components.Attribute _CmpAttribute;

        private int _ViewScale;

        #endregion Member Variables

        // コンストラクタ
        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute">属性</param>
        ///
        /// <history>2013/04/09 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        public FormCustomViewScale(SectionListRC.Components.Attribute cmpAttribute)
        {
            InitializeComponent();

            _CmpAttribute = cmpAttribute;

            SetData();
        }

        #endregion Constructor

        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>データ設定</summary>
        ///
        /// <history>2013/04/09 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        private void SetData()
        {
            this.txtBoxCustomViewPlanScale.MaxLength = 6;
        }

        /// ================================================================================
        /// <summary>文字設定</summary>
        ///
        /// <history><p>2013/04/09 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_CUSTOMVIEWSCALE") ;
            this.lblCustomViewPlanScale.Text = _CmpAttribute.ResourceText("IDS_TXT_VIEWSCALEVALUE_2");

            this.btnOK.Text = _CmpAttribute.ResourceText("IDS_TXT_OK");
            this.btnCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");
        }

        #endregion Member Functions

        // プロパティ
        #region Properties

        /// ================================================================================
        /// <summary>カスタムビュー尺度</summary>
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

        // ロード
        private void FormCustomViewScale_Load(object sender, EventArgs e)
        {
            SetText();
            this.txtBoxCustomViewPlanScale.Select();
        }

        // 入力制限
        private void txtBoxCustomViewPlanScale_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b') {
                e.Handled = true;
            }
        }

        // Enterキー押下
        private void txtBoxCustomViewPlanScale_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) {
                this.btnOK_Click(sender, e);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            int viewScale = 0;

            if (int.TryParse(this.txtBoxCustomViewPlanScale.Text, out viewScale)) {
                if (viewScale != 0) {
                    _ViewScale = viewScale;
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }
                else {
                    System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_VALUEISZERO"));
                }
            }
            else {
                System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTINT"));
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion Events
    }
}
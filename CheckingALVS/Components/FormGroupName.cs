
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.CheckingALVS;
using System.Reflection;

namespace ADSK.JExtRAC.CheckingALVS.Components
{
    /// ================================================================================
    /// <summary>画面 グループ名</summary>
    /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
    /// ================================================================================
    public partial class FormGroupName : Form
    {
        // メンバ変数
        #region Memeber Variables

        /// <summary>属性</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>グループ名</summary>
        private string _GroupName;

        #endregion Memeber Variables

        // コンストラクタ
        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute"  >属性</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public FormGroupName(RvtExtApp.Components.Attribute cmpAttribute)
        {
            InitializeComponent();
            RevitFormTheme.Apply(this);

            _CmpAttribute = cmpAttribute;
        }

        #endregion Constructor

        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>フォームの文字設定</summary>
        ///
        /// <history><p>2011/09/04 Created GSA,Inc. Shinichi Ishii</p>
        ///           <p>2015/09/07 Modified GSA,Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        private
        void SetText()
        {
            // Control text
            // コントロール文字
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_GROUPNAME") + string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);

            this.lblGroupName.Text = _CmpAttribute.ResourceText("IDS_TXT_GROUPNAME");
            this.btnOK.Text = _CmpAttribute.ResourceText("IDS_TXT_OK");
            this.btnCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");
        }

        #endregion Member Functions

        // プロパティ
        #region Properties

        /// ================================================================================
        /// <summary>グループ名</summary>
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string GroupName
        {
            get
            {
                return _GroupName;
            }
        }

        #endregion Properties

        // イベント
        #region Events

        /// ================================================================================
        /// <summary>Handles the Load event of the  FormGroupName control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void FormGroupName_Load(object sender, EventArgs e)
        {
            SetText();
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnOK control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void btnOK_Click(object sender, EventArgs e)
        {
            _GroupName = this.txtGroupName.Text;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        #endregion Events
    }
}

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
    /// <summary>画面 グループ</summary>
    /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
    /// ================================================================================
    public partial class FormGroup : Form
    {
        // メンバ変数
        #region Memeber Variables

        /// <summary>属性</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>グループリスト</summary>
        private System.Windows.Forms.TreeNodeCollection _Groups;

        /// <summary>グループ</summary>
        private System.Windows.Forms.TreeNode _Group;

        #endregion Memeber Variables

        // コンストラクタ
        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute">属性</param>
        /// <param name="groups"      >グループリスト</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public FormGroup(RvtExtApp.Components.Attribute cmpAttribute,
                         System.Windows.Forms.TreeNodeCollection groups)
        {
            InitializeComponent();

            _CmpAttribute = cmpAttribute;
            _Groups = groups;
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
            // コントロール文字
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_GROUPNAME") + string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);

            this.lblGroup.Text = _CmpAttribute.ResourceText("IDS_TXT_GROUP");
            this.btnOK.Text = _CmpAttribute.ResourceText("IDS_TXT_OK");
            this.btnCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");
        }

        /// ================================================================================
        /// <summary>フォームのデータ設定</summary>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void SetData()
        {
            // グループ
            this.cboGroup.DataSource = _Groups;
            this.cboGroup.DisplayMember = "Text";
        }

        #endregion Member Functions

        // プロパティ
        #region Properties

        /// ================================================================================
        /// <summary>Group<p>グループ</p></summary>
        /// <value></value>
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        System.Windows.Forms.TreeNode Group
        {
            get
            {
                return _Group;
            }
        }

        #endregion Properties

        // イベント
        #region Events

        /// ================================================================================
        /// <summary>Handles the Load event of the  FormGroup control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void FormGroup_Load(object sender, EventArgs e)
        {
            SetText();
            SetData();
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnOK control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void btnOK_Click(object sender, EventArgs e)
        {
            _Group = this.cboGroup.SelectedItem as System.Windows.Forms.TreeNode;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        #endregion Events
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.AreaSchedule;
using System.Reflection;

namespace ADSK.JExtRAC.AreaSchedule.LegalArea
{
    /// ================================================================================
    /// <summary>画面 警告する部屋</summary>
    /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
    /// ================================================================================
    public partial class FormWarningRooms : Form
    {
        // メンバ変数
        #region Memeber Variables

        /// <summary>属性</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>警告する部屋のデータ</summary>
        private System.Data.DataTable _Data;

        #endregion Memeber Variables

        // コンストラクタ
        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute">属性</param>
        /// <param name="data"        >警告する部屋のデータ</param>
        ///
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public FormWarningRooms(RvtExtApp.Components.Attribute cmpAttribute,
                                System.Data.DataTable data)
        {
            InitializeComponent();
            _CmpAttribute = cmpAttribute;
            _Data = data;
        }

        #endregion Constructor

        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>フォームの文字設定</summary>
        ///
        /// <history><p>2011/08/02 Created GSA,Inc. Shinichi Ishii</p>
        ///           <p>2015/09/07 Modified GSA,Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        private
        void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_AREAMARGINERROR") + string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);

            this.dgvRooms.Columns[0].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_ROOMNAME");
            this.dgvRooms.Columns[1].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_ROOMNUMBER");
            this.dgvRooms.Columns[2].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_AREA_RVT");
            this.dgvRooms.Columns[3].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_AREA_LEGAL");
            this.btnOK.Text = "&" + _CmpAttribute.ResourceText("IDS_TXT_OK");
            this.btnCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL") + "(&C)";
        }

        /// ================================================================================
        /// <summary>フォームのデータ設定</summary>
        ///
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void SetData()
        {
            this.dgvRooms.AutoGenerateColumns = false;
            this.dgvRooms.DataSource = _Data;
            this.dgvRooms.Columns[0].DataPropertyName = _Data.Columns[0].ColumnName;
            this.dgvRooms.Columns[1].DataPropertyName = _Data.Columns[1].ColumnName;
            this.dgvRooms.Columns[2].DataPropertyName = _Data.Columns[2].ColumnName;
            this.dgvRooms.Columns[3].DataPropertyName = _Data.Columns[3].ColumnName;
        }

        #endregion Member Functions

        // イベント
        #region Events

        /// ================================================================================
        /// <summary>Handles the Load event of the FormWarningRooms control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/05/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void FormWarningRooms_Load(object sender, EventArgs e)
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
        /// <history>2009/05/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        #endregion Events
    }
}

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
    /// <summary>画面 有効寸法変更</summary>
    /// <history>2011/08/05 Created GSA,Inc. Shinichi Ishii</history>
    /// ================================================================================
    public partial class FormChangedUsableDim : Form
    {
        // メンバ変数
        #region Memeber Variables

        /// <summary>属性</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>データテーブル - 建具</summary>
        private RvtExtApp.Entities.DtWinDoor _EntDtWinDoor;

        /// <summary>変更データ</summary>
        private System.Data.DataTable _ChangeData;

        #endregion Memeber Variables

        // コンストラクタ
        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute">属性</param>
        /// <param name="entDtWinDoor">データテーブル - 建具</param>
        /// <param name="changeData"  >変更データ</param>
        ///
        /// <history>2011/08/05 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public FormChangedUsableDim(RvtExtApp.Components.Attribute cmpAttribute,
                                    RvtExtApp.Entities.DtWinDoor entDtWinDoor,
                                    System.Data.DataTable changeData)
        {
            InitializeComponent();
            RevitFormTheme.Apply(this);

            _CmpAttribute = cmpAttribute;
            _EntDtWinDoor = entDtWinDoor;
            _ChangeData = changeData;
        }

        #endregion Constructor

        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>フォームの文字設定</summary>
        ///
        /// <history><p>2011/08/05 Created GSA,Inc. Shinichi Ishii</p>
        ///           <p>2015/09/07 Modified GSA,Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        private
        void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_CHANGEUSABLEDIMPARTS") + string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);

            this.btnSave.Text = _CmpAttribute.ResourceText("IDS_TXT_SAVE");
            this.btnCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");

            // 列の文字位置
            System.Windows.Forms.DataGridViewContentAlignment alignMidRight = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            System.Windows.Forms.DataGridViewContentAlignment alignMidCenter = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            System.Windows.Forms.DataGridViewContentAlignment alignMidLeft = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;

            // DataGridView 列初期化
            int colsCount = 0;
            System.Windows.Forms.DataGridViewColumnCollection cols;
            System.Windows.Forms.DataGridViewColumn col;

            // 列数
            colsCount = _ChangeData.Columns.Count;
            if (colsCount == 0)
            {
                return;
            }
            cols = this.dgvChangedUsableDim.Columns;
            string colName = "";

            int comKind = _EntDtWinDoor.CommandKind;
            string header = "";

            // ID
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _ChangeData.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameID)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(this.dgvChangedUsableDim);
                    UtilForm.SetDataGridViewColumnProperty(col, 0, alignMidLeft, true, false, colName, "");
                    break;
                }
            }

            // Type ID
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _ChangeData.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameTypeID)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(this.dgvChangedUsableDim);
                    UtilForm.SetDataGridViewColumnProperty(col, 0, alignMidLeft, true, false, colName, "");
                    break;
                }
            }

            // 建具符号
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _ChangeData.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameSign)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(this.dgvChangedUsableDim);
                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidLeft, true, true, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_PARTSSIGN_N"));
                    break;
                }
            }

            // 変更前有効幅
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _ChangeData.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameUsableWidthCB)
                {
                    header = _CmpAttribute.ResourceText("IDS_TXT_USABLEWIDTHCB");
                    if (comKind == 1)
                    {
                        header = _CmpAttribute.ResourceText("IDS_TXT_SMOKEWINWIDTHCB");
                    }

                    col = UtilForm.SetDataGridViewTextBoxColumn(this.dgvChangedUsableDim);
                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, true, true, colName, header);
                    break;
                }
            }

            // 変更前有効幅
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _ChangeData.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameUsableWidthCA)
                {
                    header = _CmpAttribute.ResourceText("IDS_TXT_USABLEWIDTHCA");
                    if (comKind == 1)
                    {
                        header = _CmpAttribute.ResourceText("IDS_TXT_SMOKEWINWIDTHCA");
                    }

                    col = UtilForm.SetDataGridViewTextBoxColumn(this.dgvChangedUsableDim);
                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, true, true, colName, header);
                    break;
                }
            }

            // 変更前有効高さ
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _ChangeData.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameUsableHeightCB)
                {
                    header = _CmpAttribute.ResourceText("IDS_TXT_USABLEHEIGHTCB");
                    if (comKind == 1)
                    {
                        header = _CmpAttribute.ResourceText("IDS_TXT_SMOKEWINHEIGHTCB");
                    }

                    col = UtilForm.SetDataGridViewTextBoxColumn(this.dgvChangedUsableDim);
                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, true, true, colName, header);
                    break;
                }
            }

            // 変更後有効高さ
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _ChangeData.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameUsableHeightCA)
                {
                    header = _CmpAttribute.ResourceText("IDS_TXT_USABLEHEIGHTCA");
                    if (comKind == 1)
                    {
                        header = _CmpAttribute.ResourceText("IDS_TXT_SMOKEWINHEIGHTCA");
                    }

                    col = UtilForm.SetDataGridViewTextBoxColumn(this.dgvChangedUsableDim);
                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, true, true, colName, header);

                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    break;
                }
            }

            // ヘッダー
            this.dgvChangedUsableDim.ColumnHeadersHeight = 50;
            this.dgvChangedUsableDim.ColumnHeadersDefaultCellStyle.Alignment = alignMidCenter;
            this.dgvChangedUsableDim.AutoGenerateColumns = false;
        }

        /// ================================================================================
        /// <summary>フォームのデータ設定</summary>
        ///
        /// <history>2011/08/05 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void SetData()
        {
            // データグリッドビュー-建具
            this.dgvChangedUsableDim.DataSource = _ChangeData;
        }

        #endregion Member Functions

        // イベント
        #region Events

        /// ================================================================================
        /// <summary>Handles the Load event of the  FormChangedUsableDim control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2010/06/28 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void FormChangedUsableDim_Load(object sender, EventArgs e)
        {
            SetText();
            SetData();
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnSave control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2010/06/28 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void btnSave_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        #endregion Events
    }
}
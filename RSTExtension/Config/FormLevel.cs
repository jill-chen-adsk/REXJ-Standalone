using System;
using System.Windows.Forms;
using JExtComCompat;

using RvtExtApp = RSTExtension;

namespace RSTExtension.Config
{
    /// ================================================================================
    /// <summary>画面 レベル</summary>
    /// <history>2011/11/26 Created GSA,Inc. Shinichi Ishii</history>
    /// ================================================================================
    public partial class FormLevel : Form
    {
        // メンバ変数
        #region Memeber Variables

        /// <summary>属性</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>データテーブル - レベル</summary>
        private RvtExtApp.Entities.DtLevel _EntDtLevel;

        #endregion Memeber Variables

        // コンストラクタ
        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute">属性</param>
        /// <param name="entDtLevel"  >データテーブル - レベル</param>
        ///
        /// <history>2011/11/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public FormLevel(RvtExtApp.Components.Attribute cmpAttribute,
                         RvtExtApp.Entities.DtLevel entDtLevel)
        {
            InitializeComponent();

            _CmpAttribute = cmpAttribute;
            _EntDtLevel = entDtLevel;

            SetText();
            SetData();
        }

        #endregion Constructor

        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>DataGridView - 列定義</summary>
        ///
        /// <param name="dataGridView">DataGridView</param>
        ///
        /// <history>2011/11/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void SetDataGridViewCol(System.Windows.Forms.DataGridView dataGridView)
        {
            // 列の文字位置
            System.Windows.Forms.DataGridViewContentAlignment alignMidRight = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            System.Windows.Forms.DataGridViewContentAlignment alignMidCenter = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            System.Windows.Forms.DataGridViewContentAlignment alignMidLeft = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;

            // DataGridView 列初期化
            int colsCount = 0;
            System.Windows.Forms.DataGridViewColumnCollection cols;
            System.Windows.Forms.DataGridViewColumn col;

            // 列数
            colsCount = _EntDtLevel.Data.Columns.Count;
            if (colsCount == 0)
            {
                return;
            }
            cols = dataGridView.Columns;
            string colName = "";

            // ID
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtLevel.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtLevel.ColNameID)
                {
                    col = JExtComCompat.UtilForm.SetDataGridViewTextBoxColumn(dataGridView);
                    JExtComCompat.UtilForm.SetDataGridViewColumnProperty(col, 0, alignMidRight, true, false, colName, "");
                    break;
                }
            }

            // 名称
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtLevel.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtLevel.ColNameName)
                {
                    col = JExtComCompat.UtilForm.SetDataGridViewTextBoxColumn(dataGridView);
                    JExtComCompat.UtilForm.SetDataGridViewColumnProperty(col, 150, alignMidLeft, true, true, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_NAME"));
                    break;
                }
            }

            // 高さ
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtLevel.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtLevel.ColNameHeight)
                {
                    col = JExtComCompat.UtilForm.SetDataGridViewTextBoxColumn(dataGridView);
                    JExtComCompat.UtilForm.SetDataGridViewColumnProperty(col, 80, alignMidRight, true, true, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_HEIGHT"));
                    break;
                }
            }

            // スラブレベル
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtLevel.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtLevel.ColNameSlabLevel)
                {
                    col = JExtComCompat.UtilForm.SetDataGridViewCheckBoxColumn(dataGridView);
                    JExtComCompat.UtilForm.SetDataGridViewColumnProperty(col, 80, alignMidCenter, false, true, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_SLABLEVEL"));

                    col.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
                    break;
                }
            }

            // 列ヘッダー
            dataGridView.ColumnHeadersDefaultCellStyle.Alignment = alignMidCenter;
            dataGridView.AutoGenerateColumns = false;

            // ソート禁止
            foreach (System.Windows.Forms.DataGridViewColumn dgvCol in dataGridView.Columns)
            {
                dgvCol.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        /// ================================================================================
        /// <summary>フォームの文字設定</summary>
        ///
        /// <history>2011/11/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_REGISTLEVEL");

            this.btnOK.Text = _CmpAttribute.ResourceText("IDS_TXT_OK");
            this.btnCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");

            var iconStream = typeof(FormConfig).Assembly.GetManifestResourceStream("RSTExtension.Resources.Images.IDI_SUBS_ICON.ico");
            if (iconStream != null) this.Icon = new System.Drawing.Icon(iconStream);

            // DataGridView列定義
            SetDataGridViewCol(this.dgvLevel);
        }

        /// ================================================================================
        /// <summary>フォームのデータ設定</summary>
        ///
        /// <history>2011/09/22 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void SetData()
        {
            // レベル
            this.dgvLevel.DataSource = _EntDtLevel.Data;
        }

        /// ================================================================================
        /// <summary>フォームのデータ取得</summary>
        ///
        /// <history>2011/09/22 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void GetData()
        {
        }

        #endregion Member Functions

        // プロパティ

        // イベント
        #region Events

        /// ================================================================================
        /// <summary>Handles the Click event of the btnOK control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/11/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void btnOK_Click(object sender, EventArgs e)
        {
            // データ取得
            GetData();

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        #endregion Events
    }
}

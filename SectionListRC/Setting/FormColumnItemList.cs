using SectionListRC.Components;
using SectionListRC.Utils;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SectionListRC.Setting
{
    public partial class FormColumnItemList : System.Windows.Forms.Form
    {
        // メンバ変数

        #region Member Variables

        /// <summary>属性</summary>
        private Components.Attribute _CmpAttribute;

        private List<string> _columnHugoAry = null;
        private List<string> _enHugoAry = null;

        private List<string> _levelList = null;
        private Parameters _cmpParameters = null;
        private DataTable _kakuData = null;
        private DataTable _enData = null;

        public List<string> _SelectedLevels = new List<string>();
        public List<string> _SelectedColumns = new List<string>();

        private EnumType _EnumType = EnumType.Invalid;

        private List<string> _StringSetting = null;

        //0:
        private int _iType = 0;

        #endregion Member Variables

        // コンストラクタ

        #region Constructor

        public FormColumnItemList(Components.Attribute cmpAttribute, Parameters cmpParameters, List<string> setting, EnumType enumType,
            List<string> columnHugoAry, List<string> enHugoAry, DataTable kakuData, DataTable enData, List<string> levelList, int iType)
        {
            InitializeComponent();
            _CmpAttribute = cmpAttribute;
            _cmpParameters = cmpParameters;

            _StringSetting = setting;

            _EnumType = enumType;

            _columnHugoAry = columnHugoAry;
            _enHugoAry = enHugoAry;

            _kakuData = kakuData;
            _enData = enData;

            _levelList = levelList;

            ValidateColumns();
            ValidateLevels();

            dgrItems.AllowUserToAddRows = false;
            dgrItems.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgrItems.RowHeadersVisible = false;

            _iType = iType;
        }

        #endregion Constructor

        #region Properties

        /// ================================================================================
        /// <summary>Get setting value </summary>
        /// ================================================================================
        public List<string> GetSettingValue
        {
            get
            {
                List<string> list = new List<string>();

                if (_SelectedColumns.Count != 0 && _SelectedLevels.Count != 0)
                {
                    list.Add(_SelectedLevels.Last());
                    list.Add(_SelectedLevels.First());
                    list.Add(_SelectedColumns.First());
                    list.Add(_SelectedColumns.Last());
                }
                return list;
            }
        }

        #endregion Properties

        #region Member Functions

        /// ================================================================================
        /// <summary>Get selected cells </summary>
        /// ================================================================================
        public bool GetData()
        {
            if (dgrItems.SelectedCells.Count == 0)
                return false;

            _SelectedLevels.Clear();
            _SelectedColumns.Clear();

            int minRow = int.MaxValue;
            int minCol = int.MaxValue;

            int maxRow = int.MinValue;
            int maxCol = int.MinValue;
            foreach (DataGridViewCell cell in dgrItems.SelectedCells)
            {
                if (minRow > cell.RowIndex)
                    minRow = cell.RowIndex;

                if (maxRow < cell.RowIndex)
                    maxRow = cell.RowIndex;

                if (minCol > cell.ColumnIndex)
                    minCol = cell.ColumnIndex;

                if (maxCol < cell.ColumnIndex)
                    maxCol = cell.ColumnIndex;
            }

            if (minCol == 0)
                minCol = 1;

            for (int row = minRow; row <= maxRow; row++)
            {
                for (int col = minCol; col <= maxCol; col++)
                {
                    var cell = dgrItems.Rows[row].Cells[col];

                    if (_SelectedColumns.Contains(cell.OwningColumn.Name) == false)
                        _SelectedColumns.Add(cell.OwningColumn.Name);

                    if (_SelectedLevels.Contains(cell.OwningRow.Tag as string) == false)
                        _SelectedLevels.Add(cell.OwningRow.Tag as string);
                }
            }

            if (_SelectedLevels.Count == 0 || _SelectedColumns.Count == 0)
                return false;

            return true;
        }

        /// ================================================================================
        /// <summary>Set text for control </summary>
        /// ================================================================================
        private void SetData()
        {
            if(_iType == 0)
                this.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECT_COL_ITEMS");
            else if(_iType == 1)
                this.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECT_EACHONECOL_ITEMS");
            else if(_iType == 2)
                this.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECT_COL_IMAGE_ITEMS");

            this.lblText.Text = _CmpAttribute.ResourceText("IDS_TXT_COLUMN_LIST") + " : " +
                (_EnumType == EnumType.Column ? _CmpAttribute.ResourceText("IDS_TXT_COLUMNTYPE") : _CmpAttribute.ResourceText("IDS_TXT_POSTTYPE"));

            this.btnOK.Text = _CmpAttribute.ResourceText("IDS_TXT_OK");
            this.btnCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");
        }

        /// <summary>Validate column</summary>
        private void ValidateColumns()
        {
            List<string> display = new List<string>();

            foreach (string name in _columnHugoAry)
            {
                bool flag = false;

                foreach (string level in _levelList)
                {
                    if (IsAvaiable(_kakuData, level, name) == true || IsAvaiable(_enData, level, name) == true)
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                    display.Add(name);
            }

            _columnHugoAry = display;
            display = new List<string>();
            foreach (string name in _enHugoAry)
            {
                bool flag = false;
                foreach (string level in _levelList)
                {
                    if (IsAvaiable(_kakuData, level, name) == true || IsAvaiable(_enData, level, name) == true)
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                    display.Add(name);
            }

            _enHugoAry = display;
        }

        /// <summary>Validate level</summary>
        private void ValidateLevels()
        {
            List<string> display = new List<string>();
            foreach (string level in _levelList)
            {
                bool flag = false;
                foreach (string name in _columnHugoAry)
                {
                    if (IsAvaiable(_kakuData, level, name) == true || IsAvaiable(_enData, level, name) == true)
                    {
                        flag = true;
                        break;
                    }
                }

                foreach (string name in _enHugoAry)
                {
                    if (IsAvaiable(_kakuData, level, name) == true || IsAvaiable(_enData, level, name) == true)
                    {
                        flag = true;
                        break;
                    }
                }

                if (flag)
                    display.Add(level);
            }

            _levelList = display;
        }

        /// ================================================================================
        /// <summary>Form loaded event </summary>
        /// ================================================================================
        private void FormColumnItemList_Load(object sender, EventArgs e)
        {
            SetData();

            dgrItems.Columns.Clear();
            dgrItems.Rows.Clear();

            int colIndex = dgrItems.Columns.Add("", "");
            dgrItems.Columns[colIndex].ReadOnly = true;
            dgrItems.Columns[colIndex].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgrItems.Columns[colIndex].DefaultCellStyle.SelectionBackColor = System.Drawing.Color.White;
            dgrItems.Columns[colIndex].DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;


            foreach (string name in _columnHugoAry)
            {
                int index = dgrItems.Columns.Add(name, name);
                dgrItems.Columns[index].ReadOnly = true;
                dgrItems.Columns[index].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            foreach (string name in _enHugoAry)
            {
                dgrItems.Columns.Add(name, name);
            }

            foreach (string levelNameOriginal in _levelList)
            {
                string level = levelNameOriginal + _cmpParameters.LevelFrameEndWord;
                int rowIndex = dgrItems.Rows.Add(level);

                var row = dgrItems.Rows[rowIndex];
                row.Tag = levelNameOriginal;

                DisplayCell(row, levelNameOriginal, _columnHugoAry);
                DisplayCell(row, levelNameOriginal, _enHugoAry);
            }

            SetCurrentCells();
        }

        /// ================================================================================
        /// <summary>Set current selected cells</summary>
        /// ================================================================================
        private void SetCurrentCells()
        {
            dgrItems.CurrentCell = null;
            if (_StringSetting != null && _StringSetting.Count != 0 && dgrItems.Columns.Count != 0 && dgrItems.Rows.Count != 0)
            {
                var all_cols = _columnHugoAry;
                all_cols.AddRange(_enHugoAry);

                //if (splits.Contains(_CmpAttribute.ResourceText("IDS_TXT_PART_ENG")) && splits.Count > 4)
                if (_StringSetting.Count == 9)
                {
                    var max_sign = _StringSetting[_StringSetting.Count - 1];
                    var min_sign = _StringSetting[_StringSetting.Count - 2];
                    var highestFL = _StringSetting[_StringSetting.Count - 3];
                    var lowsestFL = _StringSetting[_StringSetting.Count - 4];

                    if (all_cols.Contains(max_sign) == false || all_cols.Contains(min_sign) == false ||
                        _levelList.Contains(highestFL) == false || _levelList.Contains(lowsestFL) == false)
                        return;

                    int hIndex = _levelList.IndexOf(highestFL);
                    int lIndex = _levelList.IndexOf(lowsestFL);

                    int maxIndex = all_cols.IndexOf(max_sign);
                    int minIndex = all_cols.IndexOf(min_sign);

                    if (hIndex == -1 || lIndex == -1 || maxIndex == -1 || minIndex == -1)
                        return;

                    for (int row = hIndex; row <= lIndex; row++)
                    {
                        for (int col = minIndex; col <= maxIndex; col++)
                        {
                            if (col + 1 < dgrItems.Columns.Count && row < dgrItems.Rows.Count)
                                dgrItems[col + 1, row].Selected = true;
                        }
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>Display cell value </summary>
        /// <param name="row">Current row</param>
        /// <param name="level">Level</param>
        /// <param name="columnAry">Column list</param>
        /// ================================================================================
        private void DisplayCell(DataGridViewRow row, string level, List<string> columnAry)
        {
            foreach (string colName in columnAry)
            {
                if (dgrItems.Columns.Contains(colName) == false)
                    continue;

                DataGridViewColumn column = dgrItems.Columns[colName];

                var value = string.Format("{0}{1}", level, colName);

                var cell = row.Cells[column.Index];
                cell.Value = value;

                if (IsAvaiable(_kakuData, level, colName) == false && IsAvaiable(_enData, level, colName) == false)
                {
                    cell.Style.BackColor = System.Drawing.Color.FromArgb(210, 210, 210);
                }
            }
        }

        /// ================================================================================
        /// <summary>Check cell is visible or not</summary>
        /// <param name="data">DataTable</param>
        /// <param name="level">Level</param>
        /// <param name="hugoName">Column name</param>
        /// <returns>True or False</returns>
        /// ================================================================================
        private bool IsAvaiable(DataTable data, string level, string hugoName)
        {
            try
            {
                for (int k = 0; k < data.Rows.Count; ++k)
                {
                    string currenthugo = (string)data.Rows[k][_cmpParameters.RST_HasiraHugo_Kaku];
                    string currentlevel = (string)data.Rows[k][_cmpParameters.LevelFrameTitle];

                    if (currenthugo == hugoName && currentlevel == level)
                    {
                        return true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                string mess = ex.Message;
            }

            return false;
        }

        /// ================================================================================
        /// <summary>Button clicked event </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        /// ================================================================================
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (GetData() == false)
            {
                MessageBox.Show("Please select the range for creating the column list.");
                return;
            }

            DialogResult = DialogResult.OK;
        }

        /// ================================================================================
        /// <summary>Button clicked event </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        /// ================================================================================
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        #endregion Member Functions
    }
}
using System.Windows.Forms;

namespace ADSK.JExtRAC.CheckingALVS.Utils
{
    /// <summary>Replaces DnfCom.UtilForm DataGridView helpers.</summary>
    public static class UtilForm
    {
        public static DataGridViewTextBoxColumn SetDataGridViewTextBoxColumn(DataGridView dgv)
        {
            var col = new DataGridViewTextBoxColumn();
            dgv.Columns.Add(col);
            return col;
        }

        public static DataGridViewComboBoxColumn SetDataGridViewComboBoxColumn(DataGridView dgv, System.Data.DataTable dataSource,
            string displayMember, string valueMember)
        {
            var col = new DataGridViewComboBoxColumn();
            col.DataSource = dataSource;
            col.DisplayMember = displayMember;
            col.ValueMember = valueMember;
            dgv.Columns.Add(col);
            return col;
        }

        public static DataGridViewCheckBoxColumn SetDataGridViewCheckBoxColumn(DataGridView dgv)
        {
            var col = new DataGridViewCheckBoxColumn();
            dgv.Columns.Add(col);
            return col;
        }

        public static void SetDataGridViewColumnProperty(DataGridViewColumn col,
            int widthOrZero,
            DataGridViewContentAlignment alignment,
            bool readOnly,
            bool visible,
            string dataPropertyName,
            string headerTextOrEmpty)
        {
            col.DefaultCellStyle.Alignment = alignment;
            col.ReadOnly = readOnly;
            col.Visible = visible;
            col.DataPropertyName = dataPropertyName ?? "";
            col.Name = dataPropertyName ?? "";
            if (!string.IsNullOrEmpty(headerTextOrEmpty))
                col.HeaderText = headerTextOrEmpty;
            if (widthOrZero > 0)
                col.Width = widthOrZero;
            col.SortMode = DataGridViewColumnSortMode.NotSortable;
        }
    }
}

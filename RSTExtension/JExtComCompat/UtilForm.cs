using System.Windows.Forms;

namespace JExtComCompat
{
    public static class UtilForm
    {
        public static DataGridViewColumn SetDataGridViewTextBoxColumn(DataGridView dataGridView)
        {
            var c = new DataGridViewTextBoxColumn();
            dataGridView.Columns.Add(c);
            return c;
        }

        public static DataGridViewColumn SetDataGridViewCheckBoxColumn(DataGridView dataGridView)
        {
            var c = new DataGridViewCheckBoxColumn();
            dataGridView.Columns.Add(c);
            return c;
        }

        public static void SetDataGridViewColumnProperty(
            DataGridViewColumn column,
            int width,
            DataGridViewContentAlignment alignment,
            bool readOnly,
            bool visible,
            string name,
            string headerText)
        {
            if (width > 0)
            {
                column.Width = width;
            }
            column.DefaultCellStyle.Alignment = alignment;
            column.ReadOnly = readOnly;
            column.Visible = visible;
            column.Name = name;
            column.HeaderText = headerText ?? "";
        }
    }
}

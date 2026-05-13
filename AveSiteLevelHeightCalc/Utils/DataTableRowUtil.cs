using System;
using System.Data;

namespace ADSK.JExtRAC.AveSiteLevelHeightCalc.Utils
{
    public static class DataTableRowUtil
    {
        public static bool UpDnDataTableRow(ref DataTable table, int selectIndex, bool upFlag)
        {
            if (table?.Rows == null || selectIndex < 0 || selectIndex >= table.Rows.Count)
                return false;

            int to = upFlag ? selectIndex - 1 : selectIndex + 1;
            if (to < 0 || to >= table.Rows.Count)
                return false;

            object[] tmpA = table.Rows[selectIndex].ItemArray;
            object[] tmpB = table.Rows[to].ItemArray;

            DataRow rowA = table.Rows[selectIndex];
            DataRow rowB = table.Rows[to];
            for (int i = 0; i < table.Columns.Count; i++)
            {
                object xa = tmpA[i];
                object xb = tmpB[i];
                rowA[i] = xb;
                rowB[i] = xa;
            }

            table.AcceptChanges();
            return true;
        }

        public static object MoveListDataTableRow(ref DataTable tableSrc, int index, ref DataTable tableDel)
        {
            if (tableSrc == null || index < 0 || index >= tableSrc.Rows.Count)
                return null;
            if (tableDel == null)
                tableDel = tableSrc.Clone();

            DataRow row = tableSrc.Rows[index];
            tableDel.ImportRow(row);
            tableSrc.Rows.RemoveAt(index);
            tableSrc.AcceptChanges();
            tableDel.AcceptChanges();
            return tableDel.Rows[tableDel.Rows.Count - 1];
        }
    }
}

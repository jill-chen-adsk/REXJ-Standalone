using System.Data;

namespace ADSK.JExtRAC.FittingSchedule.Components
{
    public static class UtilData
    {
        public static object MoveListDataTableRow(ref DataTable source, int selectIndex, ref DataTable target)
        {
            if (selectIndex < 0 || selectIndex >= source.DefaultView.Count) return null;
            var sourceRow = source.DefaultView[selectIndex].Row;
            var newRow = target.NewRow();
            newRow.ItemArray = sourceRow.ItemArray;
            target.Rows.Add(newRow);
            object retVal = sourceRow[0];
            sourceRow.Delete();
            source.AcceptChanges();
            return retVal;
        }

        public static object UpDnDataTableRow(ref DataTable table, int selectIndex, bool upFlag)
        {
            if (table == null || table.Rows.Count < 2) return null;
            int targetIndex = upFlag ? selectIndex - 1 : selectIndex + 1;
            if (targetIndex < 0 || targetIndex >= table.Rows.Count) return null;

            var tempRow = table.NewRow();
            tempRow.ItemArray = table.Rows[selectIndex].ItemArray;
            table.Rows.RemoveAt(selectIndex);
            table.Rows.InsertAt(tempRow, targetIndex);
            return tempRow[0];
        }

        public static object AddDataTableRow(ref DataTable table, int insertIndex, int id, string name)
        {
            var newRow = table.NewRow();
            newRow[0] = id;
            newRow[1] = name;
            if (insertIndex >= 0 && insertIndex < table.Rows.Count)
                table.Rows.InsertAt(newRow, insertIndex);
            else
                table.Rows.Add(newRow);
            return id;
        }
    }
}

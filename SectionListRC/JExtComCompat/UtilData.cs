using System.Data;

namespace SectionListRC.JExtComCompat
{
    public static class UtilData
    {
        public static string GetValueTableData(DataTable table, string keyCol, string keyVal, string valueCol)
        {
            if (table == null || table.Rows.Count == 0)
            {
                return "";
            }
            foreach (DataRow row in table.Rows)
            {
                if (row[keyCol]?.ToString() == keyVal)
                {
                    return row[valueCol]?.ToString() ?? "";
                }
            }
            return "";
        }
    }
}

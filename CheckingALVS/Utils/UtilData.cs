using System;
using System.Data;

namespace ADSK.JExtRAC.CheckingALVS.Utils
{
    /// <summary>Replaces DnfCom.UtilData table lookups.</summary>
    public static class UtilData
    {
        public static string GetValueTableData(DataTable table, string keyColumn, string keyValue, string valueColumn)
        {
            if (table == null || table.Rows.Count == 0 || string.IsNullOrEmpty(keyColumn))
                return null;
            foreach (DataRow row in table.Rows)
            {
                try
                {
                    if ((row[keyColumn]?.ToString() ?? "") != (keyValue ?? "")) continue;
                    if (table.Columns.Contains(valueColumn))
                        return row[valueColumn]?.ToString();
                }
                catch { }
            }
            return null;
        }

        /// <summary>Row index into table; returns Name column (use district list).</summary>
        public static string GetValueTableData(DataTable table, int rowIndex, int unusedPlaceholder)
        {
            if (table == null || rowIndex < 0 || rowIndex >= table.Rows.Count) return null;
            if (!table.Columns.Contains("Name")) return null;
            return table.Rows[rowIndex]["Name"]?.ToString();
        }

        public static string GetValueTableData(DataTable table, string keyColumn, string keyValue, string keyColumn2, string keyValue2, string valueColumn)
        {
            if (table == null || table.Rows.Count == 0) return "";
            foreach (DataRow row in table.Rows)
            {
                try
                {
                    if ((row[keyColumn]?.ToString() ?? "") != (keyValue ?? "")) continue;
                    if ((row[keyColumn2]?.ToString() ?? "") != (keyValue2 ?? "")) continue;
                    return row[valueColumn]?.ToString() ?? "";
                }
                catch { }
            }
            return "";
        }
    }
}

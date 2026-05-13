using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml.Linq;

namespace ADSK.JExtRAC.CheckingALVS.Utils
{
    /// <summary>Replaces DnfCom.UtilIO.GetXMLFile for Coefficient.xml.</summary>
    public static class UtilXml
    {
        public static DataTable GetXMLFile(string filePath, string sectionName, string itemElement,
            IList<string> itemNames, IList<Type> itemTypes)
        {
            var table = new DataTable();
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return table;
            for (int i = 0; i < itemNames.Count && i < itemTypes.Count; i++)
                table.Columns.Add(itemNames[i], itemTypes[i]);

            try
            {
                var doc = XDocument.Load(filePath);
                var root = doc.Root;
                if (root == null) return table;
                var section = root.Element(sectionName);
                if (section == null) return table;

                foreach (var item in section.Elements(itemElement))
                {
                    DataRow row = table.NewRow();
                    for (int i = 0; i < itemNames.Count && i < itemTypes.Count; i++)
                    {
                        string col = itemNames[i];
                        var el = item.Element(col);
                        string text = el?.Value?.Trim() ?? "";
                        try
                        {
                            if (itemTypes[i] == typeof(double))
                                row[col] = string.IsNullOrEmpty(text) ? 0.0 : Convert.ToDouble(text, System.Globalization.CultureInfo.InvariantCulture);
                            else if (itemTypes[i] == typeof(int))
                                row[col] = string.IsNullOrEmpty(text) ? 0 : Convert.ToInt32(text, System.Globalization.CultureInfo.InvariantCulture);
                            else
                                row[col] = text;
                        }
                        catch
                        {
                            row[col] = text;
                        }
                    }
                    table.Rows.Add(row);
                }
            }
            catch { }

            return table;
        }
    }
}

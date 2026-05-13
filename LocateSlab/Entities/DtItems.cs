using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Xml;
namespace ADSK.JExtRAC.LocateSlab.Entities
{
    public class DtItems
    {
        private readonly string _fileItems;
        private DataTable _default;

        public DtItems(Components.Attribute cmpAttribute)
        {
            string folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fileName = cmpAttribute.ResourceText("IDS_FILE_ITEMS");
            string path = Path.Combine(folder, fileName);
            _fileItems = File.Exists(path) ? path : null;
        }

        public DataTable Default
        {
            get
            {
                if (_default == null && _fileItems != null)
                {
                    _default = new DataTable();
                    _default.Columns.Add("Name", typeof(string));
                    _default.Columns.Add("Value", typeof(string));

                    try
                    {
                        var doc = new XmlDocument();
                        doc.Load(_fileItems);
                        var items = doc.SelectNodes("//Default/Item");
                        if (items != null)
                        {
                            foreach (XmlNode item in items)
                            {
                                var nameNode = item.SelectSingleNode("Name");
                                var valueNode = item.SelectSingleNode("Value");
                                if (nameNode != null && valueNode != null)
                                {
                                    var row = _default.NewRow();
                                    row["Name"] = nameNode.InnerText;
                                    row["Value"] = valueNode.InnerText;
                                    _default.Rows.Add(row);
                                }
                            }
                        }
                    }
                    catch { }
                }
                return _default;
            }
        }

        public double ToleranceInter
        {
            get
            {
                if (Default == null) return 0;
                foreach (DataRow row in Default.Rows)
                {
                    if (row["Name"]?.ToString() == "ToleranceInter")
                    {
                        if (double.TryParse(row["Value"]?.ToString(), out double val))
                            return val;
                    }
                }
                return 0;
            }
        }
    }
}

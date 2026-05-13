using System.Data;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using RvtExtApp = ADSK.JExtRAC.AutomaticFloor;

namespace ADSK.JExtRAC.AutomaticFloor.Entities
{
    public class DtItems
    {
        private RvtExtApp.Components.Attribute _CmpAttribute;
        private string _FileItems;
        private DataTable _Default;

        public DtItems(RvtExtApp.Components.Attribute cmpAttribute)
        {
            _CmpAttribute = cmpAttribute;
            string itemsFoldr = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _FileItems = Path.Combine(itemsFoldr, _CmpAttribute.ResourceText("IDS_FILE_ITEMS"));
            if (!File.Exists(_FileItems))
                _FileItems = null;
        }

        public DataTable Default
        {
            get
            {
                if (_Default == null && _FileItems != null)
                {
                    _Default = new DataTable();
                    _Default.Columns.Add("Name", typeof(string));
                    _Default.Columns.Add("Value", typeof(string));

                    try
                    {
                        XDocument xdoc = XDocument.Load(_FileItems);
                        var defaultElement = xdoc.Root?.Element("Default");
                        if (defaultElement != null)
                        {
                            foreach (var item in defaultElement.Elements("Item"))
                            {
                                var row = _Default.NewRow();
                                row["Name"] = item.Element("Name")?.Value ?? "";
                                row["Value"] = item.Element("Value")?.Value ?? "";
                                _Default.Rows.Add(row);
                            }
                        }
                    }
                    catch { }
                }
                return _Default;
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

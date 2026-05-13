using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace ADSK.JExtRAC.ExportExcel.Entities
{
    public class CategoryItem : ObjectItem
    {
        public bool _IsChecked = false;

        private ElementId _ElementId = ElementId.InvalidElementId;

        public List<ParameterData> _Parameters = new List<ParameterData>();

        public ElementId ElementId
        {
            get { return this._ElementId; }
        }

        public CategoryItem(string name, ElementId elementId) : base(name)
        {
            this._ElementId = elementId;
        }
    }
}

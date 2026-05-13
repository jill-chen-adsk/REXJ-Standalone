using System.Collections.Generic;
using Autodesk.Revit.DB;
using RvtExtApp = ADSK.JExtRAC.AutomaticFloor;

namespace ADSK.JExtRAC.AutomaticFloor.Entities
{
    public class DtSlabType : DtBase
    {
        private SpSlabType _EntSpSlabType;
        private System.Data.DataTable _Data;
        private Element _WorkElem;

        public DtSlabType(RvtExtApp.Components.Attribute cmpAttribute,
                          RvtExtApp.Components.Elements cmpElements,
                          RvtExtApp.Components.Geometry cmpGeometry,
                          RvtExtApp.Components.Parameters cmpParameters,
                          RvtExtApp.Components.Settings cmpSettings) :
               base(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings)
        {
            _EntSpSlabType = new SpSlabType(cmpAttribute, cmpParameters, cmpSettings);
            if (!_EntSpSlabType.DefSuccess)
            {
                string strCategory = cmpAttribute.ResourceText("IDS_TXT_CATEGORY");
                string strParam = cmpAttribute.ResourceText("IDS_TXT_PARAMETER");
                base.ErrMsg = cmpAttribute.ResourceText("IDS_ERR_PARAMDEF") + "\n" +
                              strCategory + " = " + _EntSpSlabType.DefCatName + "\n" +
                              "    " + strParam + "[" + _EntSpSlabType.ErrDefName + "]";
                _Data = null;
                _WorkElem = null;
            }
        }

        private void DefDataFormat(ref System.Data.DataTable data)
        {
            data.Columns.Add(base.ColNameID, typeof(int));
            data.Columns.Add(base.ColNameName, typeof(string));
        }

        public System.Data.DataRow GetData(Element elem)
        {
            if (_Data == null)
            {
                _Data = new System.Data.DataTable();
                DefDataFormat(ref _Data);
            }
            System.Data.DataRow row = _Data.NewRow();
            if (elem != null)
            {
                _EntSpSlabType.CurrentElem = elem;
                row[base.ColNameID] = elem.Id.ToString();
                row[base.ColNameName] = _EntSpSlabType.FamilyTypeName;
            }
            return row;
        }

        public void GetData(IList<Element> elems)
        {
            if (_Data == null)
            {
                _Data = new System.Data.DataTable();
                DefDataFormat(ref _Data);
            }
            if (elems != null)
            {
                foreach (Element elem in elems)
                {
                    System.Data.DataRow row = GetData(elem);
                    if (row != null) _Data.Rows.Add(row);
                }
            }
            _Data.DefaultView.Sort = base.ColNameName + " ASC";
        }

        public void GetWorkElem(int elemID)
        {
            _WorkElem = base.CmpElements.GetElementDoc(elemID);
        }

        public System.Data.DataTable Data => _Data;
        public Element WorkElem => _WorkElem;
    }
}

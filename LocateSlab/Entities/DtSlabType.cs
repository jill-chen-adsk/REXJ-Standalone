using System.Collections.Generic;
using System.Data;
using Autodesk.Revit.DB;

namespace ADSK.JExtRAC.LocateSlab.Entities
{
    public class DtSlabType : DtBase
    {
        private readonly SpSlabType _entSpSlabType;
        private DataTable _data;
        private Element _workElem;

        public DtSlabType(Components.Attribute cmpAttribute, Components.Elements cmpElements,
            Components.Geometry cmpGeometry, Components.Parameters cmpParameters, Components.Settings cmpSettings)
            : base(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings)
        {
            _entSpSlabType = new SpSlabType(cmpAttribute, cmpParameters, cmpSettings);
            if (!_entSpSlabType.DefSuccess)
            {
                string strCategory = cmpAttribute.ResourceText("IDS_TXT_CATEGORY");
                string strParam = cmpAttribute.ResourceText("IDS_TXT_PARAMETER");
                ErrMsg = cmpAttribute.ResourceText("IDS_ERR_PARAMDEF") + "\n" +
                         strCategory + " = " + _entSpSlabType.DefCatName + "\n" +
                         "    " + strParam + "[" + _entSpSlabType.ErrDefName + "]";
                _data = null;
                _workElem = null;
            }
        }

        private void DefDataFormat(ref DataTable data)
        {
            data.Columns.Add(ColNameID, typeof(int));
            data.Columns.Add(ColNameName, typeof(string));
        }

        public DataRow GetData(Element elem)
        {
            if (_data == null)
            {
                _data = new DataTable();
                DefDataFormat(ref _data);
            }

            var row = _data.NewRow();
            if (elem != null)
            {
                _entSpSlabType.CurrentElem = elem;
                row[ColNameID] = elem.Id.ToString();
                row[ColNameName] = _entSpSlabType.FamilyTypeName;
            }
            return row;
        }

        public void GetData(IList<Element> elems)
        {
            if (_data == null)
            {
                _data = new DataTable();
                DefDataFormat(ref _data);
            }

            if (elems != null)
            {
                foreach (var elem in elems)
                {
                    var row = GetData(elem);
                    if (row != null) _data.Rows.Add(row);
                }
            }
            _data.DefaultView.Sort = ColNameName + " ASC";
        }

        public void GetWorkElem(int elemID)
        {
            _workElem = CmpElements.GetElementDoc(elemID);
        }

        public DataTable Data => _data;
        public Element WorkElem => _workElem;
    }
}

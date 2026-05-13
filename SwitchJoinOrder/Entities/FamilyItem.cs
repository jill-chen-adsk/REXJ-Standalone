using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace ADSK.JExtRAC.SwitchJoinOrder.Entities
{
    public class FamilyItem
    {
        public string _nameFami;
        public string _nameCate;
        public List<ElementId> _listElementIdOfFamily;
        public int _indexFami;
        public int _indexCate;

        public void AddElementToList(Element ele)
        {
            if (_listElementIdOfFamily == null)
                _listElementIdOfFamily = new List<ElementId>();
            _listElementIdOfFamily.Add(ele.Id);
        }

        public override string ToString()
        {
            return this._nameFami;
        }
    }
}

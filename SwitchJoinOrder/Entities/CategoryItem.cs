using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace ADSK.JExtRAC.SwitchJoinOrder.Entities
{
    public class CategoryItem
    {
        public string _name;
        public List<ElementId> _listElementId;
        public int index;
        public List<FamilyItem> _listFamilyItem;
        public bool _isJoinFami = false;

        public void AddElementToList(Element ele)
        {
            if (_listElementId == null)
                _listElementId = new List<ElementId>();
            _listElementId.Add(ele.Id);
        }

        public bool ContainsFml(string name)
        {
            if (_listFamilyItem == null)
                return false;
            foreach (FamilyItem fami in _listFamilyItem)
                if (fami._nameFami == name)
                    return true;
            return false;
        }

        public FamilyItem FindFamilyByName(string name)
        {
            FamilyItem retFami = null;
            if (string.IsNullOrEmpty(name))
                return retFami;
            foreach (var s in _listFamilyItem)
            {
                if (s._nameFami == name)
                {
                    retFami = s;
                    break;
                }
            }
            return retFami;
        }

        public void SwapFml(int indexFist, int indexSecond)
        {
            if (indexFist >= _listFamilyItem.Count || indexSecond >= _listFamilyItem.Count || indexFist < 0 || indexSecond < 0)
                return;
            _listFamilyItem[indexFist]._indexFami = indexSecond;
            _listFamilyItem[indexSecond]._indexFami = indexFist;
            Swap(_listFamilyItem, indexFist, indexSecond);
        }

        private void Swap<T>(List<T> list, int index1, int index2)
        {
            T temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }

        public override string ToString()
        {
            return this._name;
        }
    }
}

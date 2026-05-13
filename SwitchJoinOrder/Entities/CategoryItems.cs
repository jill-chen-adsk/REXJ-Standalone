using System.Collections.Generic;

namespace ADSK.JExtRAC.SwitchJoinOrder.Entities
{
    public class CategoryItems
    {
        public List<CategoryItem> _categoryShow;

        public void SwapCtg(int indexFist, int indexSecond)
        {
            if (indexFist >= _categoryShow.Count || indexSecond >= _categoryShow.Count || indexFist < 0 || indexSecond < 0)
                return;
            _categoryShow[indexFist].index = indexSecond;
            _categoryShow[indexSecond].index = indexFist;
            Swap(_categoryShow, indexFist, indexSecond);
        }

        public bool ClearCtg()
        {
            if (_categoryShow != null && _categoryShow.Count != 0)
            {
                _categoryShow.Clear();
                return true;
            }
            else
                return false;
        }

        public CategoryItem Remove(int index)
        {
            CategoryItem retCls = null;
            if (index < 0)
                return retCls;
            int i = 0;
            foreach (var s in _categoryShow)
            {
                if (s.index == index)
                    retCls = s;
                if (index < s.index)
                {
                    s.index = index + i;
                    i++;
                }
            }
            _categoryShow.RemoveAt(index);
            return retCls;
        }

        public CategoryItem FindByName(string name)
        {
            CategoryItem retCls = null;
            if (string.IsNullOrEmpty(name))
                return retCls;
            foreach (var s in _categoryShow)
            {
                if (s._name == name)
                {
                    retCls = s;
                    break;
                }
            }
            return retCls;
        }

        public bool Contains(string name)
        {
            if (_categoryShow == null)
                return false;
            foreach (CategoryItem cata in _categoryShow)
                if (cata._name == name)
                    return true;
            return false;
        }

        public int Find(string name)
        {
            if (_categoryShow == null)
                return -1;
            foreach (CategoryItem cata in _categoryShow)
                if (cata._name == name)
                    return cata.index;
            return -1;
        }

        public bool Add(CategoryItem clsCtg)
        {
            if (_categoryShow.Contains(clsCtg) || clsCtg == null)
                return false;
            clsCtg.index = _categoryShow.Count;
            _categoryShow.Add(clsCtg);
            return true;
        }

        private void Swap<T>(List<T> list, int index1, int index2)
        {
            T temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }

        public int CountEle()
        {
            int retCount = 0;
            for (int i = 0; i < _categoryShow.Count - 1; i++)
            {
                CategoryItem cata = _categoryShow[i];
                retCount += cata._listElementId.Count;
                if (cata._isJoinFami)
                {
                    retCount += cata._listFamilyItem.Count;
                }
            }
            return retCount;
        }
    }
}

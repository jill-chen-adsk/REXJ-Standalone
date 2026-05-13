using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace ADSK.ViewExtension.ViewDuplicate.Sorter
{
    public class CmpLevel : IComparer<Level>
    {
        public int Compare(Level x, Level y)
        {
            if (ReferenceEquals(x, y))
                return 0;
            if (x == null)
                return -1;
            if (y == null)
                return 1;
            return x.Elevation.CompareTo(y.Elevation);
        }
    }
}

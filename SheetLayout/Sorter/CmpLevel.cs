using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace ADSK.ViewExtension.SheetLayout.Sorter
{
    public class CmpLevel : IComparer<Level>
    {
        public int Compare(Level x, Level y)
        {
            if (x == null && y == null)
                return 0;
            if (x == null)
                return -1;
            if (y == null)
                return 1;
            if (x.Elevation > y.Elevation)
                return 1;
            if (x.Elevation < y.Elevation)
                return -1;
            return 0;
        }
    }
}

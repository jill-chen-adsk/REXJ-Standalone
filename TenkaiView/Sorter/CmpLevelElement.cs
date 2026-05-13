using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace ADSK.ViewExtension.TenkaiView.Sorter
{
    public class CmpLevelElement : IComparer<Level>
    {
        public int Compare(Level x, Level y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0;
                return -1;
            }
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

using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace ADSK.ViewExtension.SheetLayout.Sorter
{
    public class CmpViewGenLevel : IComparer<View>
    {
        public int Compare(View x, View y)
        {
            if (x == null && y == null)
                return 0;
            if (x == null)
                return -1;
            if (y == null)
                return 1;

            Level genLvX = null;
            Level genLvY = null;
            try { genLvX = x.GenLevel; } catch { }
            try { genLvY = y.GenLevel; } catch { }

            if (genLvX == null && genLvY == null)
                return 0;
            if (genLvX == null)
                return -1;
            if (genLvY == null)
                return 1;

            if (genLvX.Elevation > genLvY.Elevation)
                return 1;
            if (genLvX.Elevation < genLvY.Elevation)
                return -1;
            return 0;
        }
    }
}

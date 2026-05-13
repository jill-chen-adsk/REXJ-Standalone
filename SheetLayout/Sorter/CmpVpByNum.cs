using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace ADSK.ViewExtension.SheetLayout.Sorter
{
    public class CmpVpByNum : IComparer<Viewport>
    {
        public int Compare(Viewport x, Viewport y)
        {
            if (x == null && y == null)
                return 0;
            if (x == null)
                return -1;
            if (y == null)
                return 1;

            Parameter prmX = x.get_Parameter(BuiltInParameter.VIEWPORT_DETAIL_NUMBER);
            Parameter prmY = y.get_Parameter(BuiltInParameter.VIEWPORT_DETAIL_NUMBER);
            string valX = prmX.AsString();
            string valY = prmY.AsString();

            return string.CompareOrdinal(valX, valY);
        }
    }
}

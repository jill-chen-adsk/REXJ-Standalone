using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace ADSK.ViewExtension.TenkaiView.Sorter
{
    public class CmpLevel : IComparer<Level>
    {
        private static readonly CmpLevelElement Inner = new CmpLevelElement();

        public int Compare(Level x, Level y) => Inner.Compare(x, y);
    }
}

using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SectionListRC.Utils
{
    public class ColumnRangeItem
    {
        public EnumType _EnumType = EnumType.Invalid;
        public List<FamilySymbol> _familySymbols = new List<FamilySymbol>();
        public List<string> _columnHugoAry = new List<string>();
        public List<string> _enHugoAry = new List<string>();

        public List<string> _levels = new List<string>();

        public System.Data.DataTable _kakuData = null;
        public System.Data.DataTable _enData = null;

        public ColumnRangeItem(System.Data.DataTable kakuData, System.Data.DataTable enData, List<string> levels)
        {
            _kakuData = kakuData;
            _enData = enData;

            _levels = levels;
        }
    }
}
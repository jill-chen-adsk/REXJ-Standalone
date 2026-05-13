using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SectionListRC.Utils
{
    public class BeamRangeItem
    {
        public EnumType _EnumType = EnumType.Invalid;
        public List<FamilySymbol> _familySymbols = new List<FamilySymbol>();

        public List<string> _levels = new List<string>();

        public BeamRangeItem(EnumType type, List<FamilySymbol> symbols)
        {
            _EnumType = type;
            _familySymbols = symbols;
        }
    }
}
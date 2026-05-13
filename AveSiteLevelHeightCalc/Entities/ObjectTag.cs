using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADSK.JExtRAC.AveSiteLevelHeightCalc.Entities
{
    public class ObjectTag
    {
        /// <summary>Circle of tag</summary>
        public FamilyInstance CircleTag { get; set; }

        /// <summary>Current tag</summary>
        public IndependentTag Tag { get; set; }
    }
}
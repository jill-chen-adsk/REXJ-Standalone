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

        /// <summary>Point number assigned during creation (used when family params are unavailable).</summary>
        public int Number { get; set; }

        /// <summary>Sampled elevation in internal units (used when family params are unavailable).</summary>
        public double Level { get; set; }

        /// <summary>True when <see cref="Number"/> and <see cref="Level"/> were set by the create command.</summary>
        public bool HasStoredValues { get; set; }
    }
}
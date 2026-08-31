using ADSK.JExtRAC.PrintRegion.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADSK.JExtRAC.PrintRegion.Entities
{
    public class EntitiesData
    {
        /// <summary>Revit UIApplication</summary>
        public UIApplication _rvtUIApp;

        /// <summary>Print manager class</summary>
        public PrintMgr _pMgr = null;

        /// <summary>Region print min</summary>
        public XYZ _pointPickMin = null;

        /// <summary>Region print max</summary>
        public XYZ _pointPickMax = null;

        /// <summary>Current view scale</summary>
        public int _viewScale = -1;

        /// <summary>Revit handle</summary>
        public IntPtr _revitHandle;

        /// <summary>Form main</summary>
        public PrintFrmWPF _printFrm = null;

        /// <summary>View duplicate</summary>
        public View _viewDuplicate = null;

        /// <summary>View current</summary>
        public View _viewCurrent = null;
    }
}

using Revit = Autodesk.Revit;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace ADSK.JExtRAC.AutoLayoutTag.Utils
{
    /// ================================================================================
    /// <summary>This class for SelectionElementFilter</summary>
    /// ================================================================================
    public class SelectionElementFilter : Revit.UI.Selection.ISelectionFilter
    {
        //Member variable

        #region Member variable

        /// <summary>List category</summary>
        public List<BuiltInCategory> ListCategoryDefault = new List<BuiltInCategory>()
        {
            BuiltInCategory.OST_Doors,
            BuiltInCategory.OST_Windows,
            BuiltInCategory.OST_Furniture,
            BuiltInCategory.OST_Floors,
            BuiltInCategory.OST_Walls,
            BuiltInCategory.OST_Ceilings,
            BuiltInCategory.OST_Columns,
            BuiltInCategory.OST_GenericModel,
            BuiltInCategory.OST_StructuralColumns,
            BuiltInCategory.OST_StructuralFraming,
            BuiltInCategory.OST_StructuralFoundation,
            BuiltInCategory.OST_DuctCurves,
            BuiltInCategory.OST_DuctFitting,
            BuiltInCategory.OST_FlexDuctCurves,
            BuiltInCategory.OST_DuctAccessory,
            BuiltInCategory.OST_DuctTerminal,
            BuiltInCategory.OST_PipeCurves,
            BuiltInCategory.OST_PipeFitting,
            BuiltInCategory.OST_PipeAccessory,
            BuiltInCategory.OST_MechanicalEquipment,
            BuiltInCategory.OST_LightingFixtures,
            BuiltInCategory.OST_LightingDevices,
            BuiltInCategory.OST_SpecialityEquipment,
            BuiltInCategory.OST_PlumbingFixtures,
            BuiltInCategory.OST_ElectricalFixtures,
            BuiltInCategory.OST_ElectricalEquipment,
            BuiltInCategory.OST_Conduit,
            BuiltInCategory.OST_ConduitFitting,
            BuiltInCategory.OST_CableTray,
            BuiltInCategory.OST_CableTrayFitting
        };

        #endregion Member variable

        //Member function

        #region Member function

        public bool AllowElement(Revit.DB.Element element)
        {
            if (element == null || element.Category == null)
                return false;

            if (ListCategoryDefault.Any(x => ((int)x).ToString() == element.Category.Id.ToString()))
                return true;

            return false;
        }

        public bool AllowReference(Revit.DB.Reference reference, Revit.DB.XYZ position)
        {
            return true;
        }

        #endregion Member function
    }
}
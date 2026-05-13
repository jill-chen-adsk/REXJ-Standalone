using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ADSK.JExtRAC.AutomaticFloor.Utils;

namespace ADSK.JExtRAC.AutomaticFloor.Commands
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class CmdCreateStructuralFloor : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            FloorCreator creator = new FloorCreator();
            return creator.CreateFloor(commandData, eFloorType.Struct);
        }
    }
}

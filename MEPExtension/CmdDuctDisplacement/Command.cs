#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

#endregion

namespace CmdDuctDisplacement
{
    using CmdDuctDisplacement.Logic;
    using Application = Autodesk.Revit.ApplicationServices.Application;
    using Constant;




    /// <summary>
    /// Command ? partial duct elevation adjustment along a picked segment (two picks).
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ModDuctLevelPartiallyCommand : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            CmdDuctDisplacement _cmdDuctDisplacemnet = new CmdDuctDisplacement(commandData);

            return _cmdDuctDisplacemnet.Main(DuctDisplacementDefine.Frow.TwoPick);
        }
    }

    /// <summary>
    /// Command ? same workflow with three picks (general model avoidance target).
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ModDuctLevelPartiallyCommand_3Pick : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            CmdDuctDisplacement _cmdDuctDisplacemnet = new CmdDuctDisplacement(commandData);

            return _cmdDuctDisplacemnet.Main(DuctDisplacementDefine.Frow.ThreePick_GeneralModel);
        }
    }

    /// <summary>
    /// Command ? three-pick workflow with linked-model avoidance target selection.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ModDuctLevelPartiallyCommand_3Pick_LinkdModel : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            CmdDuctDisplacement _cmdDuctDisplacemnet = new CmdDuctDisplacement(commandData);

            return _cmdDuctDisplacemnet.Main(DuctDisplacementDefine.Frow.ThreePick_LinkdModel);
        }
    }



}

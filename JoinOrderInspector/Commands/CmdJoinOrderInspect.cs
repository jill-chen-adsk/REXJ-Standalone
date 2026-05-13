using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ADSK.JExtRAC.JoinOrderInspector.Utils;
using System;

namespace ADSK.JExtRAC.JoinOrderInspector.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CmdJoinOrderInspect : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            try
            {
                CultureHelper.InitializeCulture();

                UIApplication uiapp = commandData.Application;
                var window = new UI.JoinOrderInspectWindow(uiapp);
                window.Show();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}

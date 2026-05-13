using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MepManholeTool.Views;
using Revit = Autodesk.Revit;

namespace MepManholeTool.Tools
{
    [Revit.Attributes.TransactionAttribute(Revit.Attributes.TransactionMode.Manual)]
    [JournalingAttribute(JournalingMode.NoCommandData)]
    public class MepModelLineParameterCommand : IExternalCommand, IExternalCommandAvailability
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var uiDocument = commandData.Application.ActiveUIDocument;
                ParameterMappingView view = new ParameterMappingView(uiDocument);
                view.ShowDialog();
                
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                if (ex is Revit.Exceptions.OperationCanceledException)
                {
                    return Result.Succeeded;
                }

                TaskDialog.Show("Error", ex.Message);
                return Result.Failed;
            }
        }

        public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
        {
            return true;
        }
    }
}
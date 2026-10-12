using System;
using Autodesk.Revit.DB;

namespace ADSK.JExtRAC.SwitchJoinOrder.Utils
{
    internal static class JoinFailures
    {
        // Revit identifies the failure type by this GUID; keep it stable across releases.
        public static readonly FailureDefinitionId JoinRefused =
            new FailureDefinitionId(new Guid("665213da-2410-4d52-89b6-e8656e0551a6"));

        // Revit only allows failure definitions to be created during application startup.
        public static void Register(string description)
        {
            try
            {
                FailureDefinition.CreateFailureDefinition(JoinRefused, FailureSeverity.Warning, description);
            }
            catch (Exception)
            {
            }
        }

        public static void Post(Document doc, ElementId firstId, ElementId secondId)
        {
            var failure = new FailureMessage(JoinRefused);
            failure.SetFailingElements(new[] { firstId, secondId });
            doc.PostFailure(failure);
        }
    }
}

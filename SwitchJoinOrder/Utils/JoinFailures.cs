using System;
using System.Diagnostics;
using Autodesk.Revit.DB;
using RevitApplication = Autodesk.Revit.ApplicationServices.Application;

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
            if (IsRegistered())
                return;

            try
            {
                FailureDefinition.CreateFailureDefinition(JoinRefused, FailureSeverity.Warning, description);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SwitchJoinOrder: failed to register join failure definition. " + ex);
            }
        }

        // Returns false when the warning could not be posted, so the caller can report it another way.
        // Must not throw: a reporting failure must not roll back the joins already made.
        public static bool Post(Document doc, ElementId firstId, ElementId secondId)
        {
            if (!IsRegistered())
                return false;

            try
            {
                var failure = new FailureMessage(JoinRefused);
                failure.SetFailingElements(new[] { firstId, secondId });
                doc.PostFailure(failure);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SwitchJoinOrder: failed to post join failure. " + ex);
                return false;
            }
        }

        private static bool IsRegistered()
        {
            try
            {
                return RevitApplication.GetFailureDefinitionRegistry()?.FindFailureDefinition(JoinRefused) != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

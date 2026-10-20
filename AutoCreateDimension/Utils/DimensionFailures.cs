using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.DB;
using RevitApplication = Autodesk.Revit.ApplicationServices.Application;

namespace ADSK.JExtRAC.AutoCreateDimension.Utils
{
    internal static class DimensionFailures
    {
        // Revit identifies the failure type by this GUID; keep it stable across releases.
        public static readonly FailureDefinitionId DimensionNotCreated =
            new FailureDefinitionId(new Guid("3f6c2b8e-9a41-4d7e-b5c2-7e1d4a9f0c63"));

        // Revit only allows failure definitions to be created during application startup.
        public static void Register(string description)
        {
            if (IsRegistered())
                return;

            try
            {
                FailureDefinition.CreateFailureDefinition(DimensionNotCreated, FailureSeverity.Warning, description);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("AutoCreateDimension: failed to register failure definition. " + ex);
            }
        }

        // Returns false when the warning could not be shown, so the caller can report it another way.
        // Uses its own transaction: cancelling Revit's warning dialog must not roll back dimensions already created.
        public static bool Report(Document doc, ICollection<ElementId> failingIds, string transactionName)
        {
            if (failingIds.Count == 0 || !IsRegistered())
                return false;

            try
            {
                using (Transaction tr = new Transaction(doc, transactionName))
                {
                    tr.Start();
                    var failure = new FailureMessage(DimensionNotCreated);
                    failure.SetFailingElements(failingIds);
                    doc.PostFailure(failure);
                    tr.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("AutoCreateDimension: failed to post failure. " + ex);
                return false;
            }
        }

        private static bool IsRegistered()
        {
            try
            {
                return RevitApplication.GetFailureDefinitionRegistry()?.FindFailureDefinition(DimensionNotCreated) != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

#region Namespaces
using System;
using System.Text;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Events;
using System.Linq;
using Autodesk.Revit.DB.Mechanical;
using Resources = CmdDuctSlopeChecker.Properties.Resources;
using System.IO;
using System.Windows.Media.Imaging;
#endregion

namespace CmdDuctSlopeChecker
{
    /// <summary>
    /// Duct slope monitoring command.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CmdDuctSlopeChecker : IExternalCommand
    {
        /// <summary>
        /// External command entry point for duct slope monitoring.
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiApp = commandData.Application;

            // Toggle monitoring flag.
            CmdDuctSlopeCheckerMain.isCheck = !CmdDuctSlopeCheckerMain.isCheck;
            if (!CmdDuctSlopeCheckerMain.isCheck) {
                CmdDuctSlopeCheckerMain.radioButtonGroup.Current = CmdDuctSlopeCheckerMain.tbOff;

                // Unregister failure-processing handler.
                uiApp.Application.FailuresProcessing -= CmdDuctSlopeCheckerMain.CheckWarnings;

                // Unregister updater.
                DuctSlopeWarnUpdater ductUpdater = new DuctSlopeWarnUpdater(uiApp.ActiveAddInId);
                UpdaterRegistry.UnregisterUpdater(ductUpdater.GetUpdaterId());

                // Update status file.
                UpdateStatus("0");
            }
            else {
                CmdDuctSlopeCheckerMain.radioButtonGroup.Current = CmdDuctSlopeCheckerMain.tbOn;

                // Register failure-processing handler.
                uiApp.Application.FailuresProcessing += new EventHandler<FailuresProcessingEventArgs>(CmdDuctSlopeCheckerMain.CheckWarnings);

                // Register updater and triggers.
                DuctSlopeWarnUpdater ductUpdater = new DuctSlopeWarnUpdater(uiApp.ActiveAddInId);
                UpdaterRegistry.RegisterUpdater(ductUpdater);
                ElementClassFilter filter = new ElementClassFilter(typeof(Duct));
                UpdaterRegistry.AddTrigger(ductUpdater.GetUpdaterId(), filter, Element.GetChangeTypeElementAddition());
                UpdaterRegistry.AddTrigger(ductUpdater.GetUpdaterId(), filter, Element.GetChangeTypeElementDeletion());
                UpdaterRegistry.AddTrigger(ductUpdater.GetUpdaterId(), filter, Element.GetChangeTypeAny());

                FailureDefinitionId failId = new FailureDefinitionId(new Guid(Resources.ID_FAILUREDEFINITION));
                ductUpdater.FailureId = failId;

                // Update status file.
                UpdateStatus("1");
            }

            return Result.Succeeded;
        }

        public static void AddEvent() {

        }

        /// <summary>
        /// Writes the status flag to the add-in status file.
        /// </summary>
        /// <param name="status"></param>
        private void UpdateStatus(string status)
        {
            var version = "2027";
            var commandName = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var roaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var paths = new string[] { roaming, "Autodesk", "Revit", "Addins", version, commandName };
            var commandFolder = Path.Combine(paths);
            var statusFile = Path.Combine(commandFolder, "Status.txt");

            if (!Directory.Exists(commandFolder)) {
                Directory.CreateDirectory(commandFolder);
            }


            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            // Write to file (Shift_JIS for compatibility with existing status reader).
            using (var sw = new StreamWriter(statusFile, false, System.Text.Encoding.GetEncoding("shift_jis"))) {
                sw.Write(status);
            }
        }
    }

    /// <summary>
    /// Dynamic model updater for duct slope checks.
    /// </summary>
    public class DuctSlopeWarnUpdater : IUpdater
    {
        static AddInId m_appId;
        UpdaterId m_updaterId;
        FailureDefinitionId m_failureId = null;

        /// <summary>
        /// Creates the updater for the given add-in.
        /// </summary>
        /// <param name="id">Add-in id.</param>
        public DuctSlopeWarnUpdater(AddInId id)
        {
            m_appId = id;
            m_updaterId = new UpdaterId(m_appId, new Guid(Resources.ID_FAILUREDEFINITION));
        }

        /// <summary>
        /// Runs slope checks for added or modified ducts.
        /// </summary>
        /// <param name="data">Updater data.</param>
        public void Execute(UpdaterData data)
        {
            // No work when monitoring is off.
            if (!CmdDuctSlopeCheckerMain.isCheck) {
                return;
            }

            // Consider only added/modified elements.
            Document doc = data.GetDocument();
            Autodesk.Revit.ApplicationServices.Application app = doc.Application;
            var list = data.GetAddedElementIds().ToList();
            list.AddRange(data.GetModifiedElementIds());

            // Check each element.
            foreach (ElementId id in list) {
                // Slope parameter
                var duct = doc.GetElement(id) as Duct;
                Autodesk.Revit.DB.Parameter p = duct.get_Parameter(BuiltInParameter.RBS_DUCT_SLOPE);

                // Only when slope parameter exists.
                if (p != null) {
                    // Warn when slope angle is between 0 and 15 degrees.
                    if (p.AsDouble() != 0 && Math.Round(Math.Atan(p.AsDouble()) * (180 / Math.PI), 2) < 15) {
                        // Task dialog
                        TaskDialog td = new TaskDialog(Resources.DLGTITLE_DUCT_SLOPED);

                        td.Id = Resources.ID_TSKDIALOG;
                        td.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
                        td.Title = Resources.DLGTITLE_DUCT_SLOPED;
                        td.TitleAutoPrefix = false;
                        td.AllowCancellation = false;
                        td.MainInstruction = Resources.MSG_DUCT_SLOPING;
                        td.MainContent = Resources.DLG_MAINCONTENT;

                        td.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, Resources.MSG_CONTINUE);
                        td.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, Resources.MSG_CANCEL);

                        // Show task dialog.
                        TaskDialogResult tdRes = td.Show();

                        // Cancel: post Revit failure to force visibility.
                        if (tdRes == TaskDialogResult.CommandLink2) {
                            // Post failure so Revit surfaces the warning.
                            FailureMessage failMessage = new FailureMessage(FailureId);
                            failMessage.SetFailingElement(id);
                            doc.PostFailure(failMessage);
                        }
                    }
                }
            }
        }

        public FailureDefinitionId FailureId
        {
            get { return m_failureId; }
            set { m_failureId = value; }
        }

        public string GetAdditionalInformation()
        {
            return Resources.MSG_ADDITIONALINFOMATION;
        }

        public ChangePriority GetChangePriority()
        {
            return ChangePriority.MEPSystems;
        }

        public UpdaterId GetUpdaterId()
        {
            return m_updaterId;
        }

        public string GetUpdaterName()
        {
            return Resources.DLGTITLE_DUCT_SLOPED;
        }
    }
}

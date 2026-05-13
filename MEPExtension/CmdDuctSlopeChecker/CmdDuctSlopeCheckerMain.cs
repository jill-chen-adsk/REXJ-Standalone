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
    /// Duct slope monitoring ribbon application entry.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CmdDuctSlopeCheckerMain : IExternalApplication
    {
        private UIControlledApplication _uicApp;
        public static ToggleButton tbOn;
        public static ToggleButton tbOff;
        public static RadioButtonGroup radioButtonGroup;

        // Monitoring flag (default off).
        public static bool isCheck = false;

        /// <summary>
        /// OnStartup application callback.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public Result OnStartup(UIControlledApplication a)
        {
            _uicApp = a;

            FailureDefinitionId failId = new FailureDefinitionId(new Guid(Resources.ID_FAILUREDEFINITION));
            FailureDefinition failDefError = FailureDefinition.CreateFailureDefinition(failId, FailureSeverity.Error, Resources.MSG_DUCT_SLOPING);

            addRibbon();

            return Result.Succeeded;
        }

        /// <summary>
        /// OnShutdown application callback.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }

        /// <summary>
        /// Creates the ribbon controls for this command.
        /// </summary>
        private void addRibbon()
        {
            try {
                _uicApp.CreateRibbonTab(Resources.TITLE_RIBBON_TAB);
            }
            catch (Autodesk.Revit.Exceptions.ArgumentException) {
                // Tab already exists; ignore.
            }

            var existingPanels = _uicApp.GetRibbonPanels(Resources.TITLE_RIBBON_TAB);
            var panel = existingPanels.Find(p => p.Name == Resources.TITLE_RIBBON_PANEL);
            if (panel == null)
                panel = _uicApp.CreateRibbonPanel(Resources.TITLE_RIBBON_TAB, Resources.TITLE_RIBBON_PANEL);

            // Toggle group (shows one button; second is invisible for radio behavior).
            RadioButtonGroupData radioData = new RadioButtonGroupData("DuctSlopeCheckerRadioGroup");
            CmdDuctSlopeCheckerMain.radioButtonGroup = panel.AddItem(radioData) as RadioButtonGroup;

            // Visible toggle button
            ToggleButtonData tb1 = new ToggleButtonData("DuctSlopeChecker",
                                                        Resources.TITLE_TOGGLE_BTN1 + "\r\n" + Resources.TITLE_TOGGLE_BTN2,
                                                        System.Reflection.Assembly.GetExecutingAssembly().Location,
                                                        "CmdDuctSlopeChecker.CmdDuctSlopeChecker");
            // Hidden sibling for radio group pairing
            ToggleButtonData tb2 = new ToggleButtonData("toggleButton2",
                                                        Resources.TITLE_TOGGLE_BTN1 + "\r\n" + Resources.TITLE_TOGGLE_BTN2,
                                                        System.Reflection.Assembly.GetExecutingAssembly().Location,
                                                        "CmdDuctSlopeChecker.CmdDuctSlopeChecker");

            // Icons
            var cmdPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //var iconPath = Path.Combine(cmdPath, "Resources");
            
            var tb1Large = SafeLoadPackImage( "CmdDuctSlopeCheckerLarge.png" ) ;
            var tb1Small = SafeLoadPackImage( "CmdDuctSlopeChecker.png" ) ;
            if ( tb1Large != null ) tb1.LargeImage = tb1Large ;
            if ( tb1Small != null ) tb1.Image = tb1Small ;
            
            // Tooltip
            tb1.ToolTip = Resources.MSG_TOOLTIP;

            var tb2Large = SafeLoadPackImage( "CmdDuctSlopeCheckerLarge.png" ) ;
            var tb2Small = SafeLoadPackImage( "CmdDuctSlopeChecker.png" ) ;
            if ( tb2Large != null ) tb2.LargeImage = tb2Large ;
            if ( tb2Small != null ) tb2.Image = tb2Small ;

            // Tooltip
            tb2.ToolTip = Resources.MSG_TOOLTIP;

            CmdDuctSlopeCheckerMain.tbOn = CmdDuctSlopeCheckerMain.radioButtonGroup.AddItem(tb1);
            CmdDuctSlopeCheckerMain.tbOff = CmdDuctSlopeCheckerMain.radioButtonGroup.AddItem(tb2);

            // F1 contextual help
            var helpPath = Path.Combine(cmdPath, "Resources");
            ContextualHelp contHelp = null;
            string contHelpPath = Path.Combine(helpPath, Resources.FILE_HELP);
            if (System.IO.File.Exists(contHelpPath) == true) {
                contHelp = new ContextualHelp(ContextualHelpType.Url, contHelpPath);
            }

            if (contHelp != null) {
                CmdDuctSlopeCheckerMain.tbOn.SetContextualHelp(contHelp);
                CmdDuctSlopeCheckerMain.tbOff.SetContextualHelp(contHelp);
                CmdDuctSlopeCheckerMain.radioButtonGroup.SetContextualHelp(contHelp);                
            }

            // Restore persisted monitoring state.
            if (!GetStatus()) {
                CmdDuctSlopeCheckerMain.radioButtonGroup.Current = CmdDuctSlopeCheckerMain.tbOff;
                isCheck = false;
            }
            else {
                CmdDuctSlopeCheckerMain.radioButtonGroup.Current = CmdDuctSlopeCheckerMain.tbOn;
                isCheck = true;

                // Register failure-processing handler.
                _uicApp.ControlledApplication.FailuresProcessing += new EventHandler<FailuresProcessingEventArgs>(CheckWarnings);

                // Register updater.
                DuctSlopeWarnUpdater ductUpdater = new DuctSlopeWarnUpdater(_uicApp.ActiveAddInId);
                UpdaterRegistry.RegisterUpdater(ductUpdater);
                ElementClassFilter filter = new ElementClassFilter(typeof(Duct));
                UpdaterRegistry.AddTrigger(ductUpdater.GetUpdaterId(), filter, Element.GetChangeTypeElementAddition());
                UpdaterRegistry.AddTrigger(ductUpdater.GetUpdaterId(), filter, Element.GetChangeTypeElementDeletion());
                UpdaterRegistry.AddTrigger(ductUpdater.GetUpdaterId(), filter, Element.GetChangeTypeAny());

                FailureDefinitionId failId = new FailureDefinitionId(new Guid(Resources.ID_FAILUREDEFINITION));
                ductUpdater.FailureId = failId;

            }

            tbOff.Visible = false;
        }

        private static BitmapImage SafeLoadPackImage( string iconName )
        {
            try {
                string dir = System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location ) ;
                string path = System.IO.Path.Combine( dir, "Icons", iconName ) ;
                if ( !System.IO.File.Exists( path ) ) return null ;
                var bmp = new BitmapImage() ;
                bmp.BeginInit() ;
                bmp.CacheOption = BitmapCacheOption.OnLoad ;
                bmp.UriSource = new Uri( path, UriKind.Absolute ) ;
                bmp.EndInit() ;
                bmp.Freeze() ;
                return bmp ;
            }
            catch {
                return null ;
            }
        }

        /// <summary>
        /// Reads monitoring state from the add-in status file.
        /// </summary>
        private bool GetStatus()
        {
            var version = "2027";

            var commandName = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var roaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var paths = new string[] { roaming, "Autodesk", "Revit", "Addins", version, commandName };
            var commandFolder = Path.Combine(paths);
            var statusFile = Path.Combine(commandFolder, "Status.txt");

            if (!Directory.Exists(commandFolder)) {
                return false;
            }

            if (!File.Exists(statusFile)) {
                return false;
            }

            bool status = false;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (var sr = new System.IO.StreamReader(statusFile, System.Text.Encoding.GetEncoding("shift_jis"))) {
                // First line holds the persisted flag ("1" = on).
                string s = sr.ReadLine();
                status = (s.Trim() == "1");
            }

            return status;
        }

        /// <summary>
        /// Suppresses the custom failure bubble so the workflow can proceed when the user continues.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void CheckWarnings(object sender, FailuresProcessingEventArgs e)
        {
            FailuresAccessor fa = e.GetFailuresAccessor();
            IList<FailureMessageAccessor> failList = new List<FailureMessageAccessor>();
            failList = fa.GetFailureMessages(); // Inside event handler, get all warnings
            foreach (FailureMessageAccessor failure in failList) {
                // check FailureDefinitionIds against ones that you want to dismiss, 
                FailureDefinitionId failID = failure.GetFailureDefinitionId();
                // prevent Revit from showing Unenclosed room warnings
                if (failID == new FailureDefinitionId(new Guid(Resources.ID_FAILUREDEFINITION))) {
                    e.SetProcessingResult(FailureProcessingResult.ProceedWithCommit);
                }
            }
        }
    }
}

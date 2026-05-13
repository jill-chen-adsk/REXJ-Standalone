#region Namespaces

using System;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using System.Windows.Media.Imaging;
using System.Reflection;
using System.IO;
using Resources = CmdFlexDuctConvert.Properties.Resource;

#endregion Namespaces

namespace CmdFlexDuctConvert
{
    public class App : IExternalApplication
    {
        private string tabName = "REXJ Standalone";
        private string panelName = "Edit";

        public Result OnStartup(UIControlledApplication app)
        {
            try
            {
                app.CreateRibbonTab(tabName);
            }
            catch (Autodesk.Revit.Exceptions.ApplicationException)
            {
            }

            AddPanel(app);
            //AddMenu(app);
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication app)
        {
            return Result.Succeeded;
        }

        private static string ExecuteingAssemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

        // ???{???p?l??????
        private void AddPanel(UIControlledApplication app)
        {
            // ???{???p?l?????
            var panels = app.GetRibbonPanels(tabName);
            RibbonPanel ribbonPanel = panels.Find(p => p.Name == panelName);
            if (ribbonPanel == null)
                ribbonPanel = app.CreateRibbonPanel(tabName, panelName);

            // ?v?b?V???{?^?????????A???{???p?l??????
            PushButtonData buttonData = new PushButtonData("Flexible Duct Convert",
                "Flexible\nDuct Convert", ExecuteingAssemblyPath, "CmdFlexDuctConvert.Command");
            buttonData.AvailabilityClassName = "CmdFlexDuctConvert.Availability0";

            PushButton pushButton = ribbonPanel.AddItem(buttonData) as PushButton;

            // ?c?[???`?b?v
            pushButton.ToolTip = Resources.MSG_TOOLTIP;

            // F1?w???v
            var cmdPath = Path.GetDirectoryName(ExecuteingAssemblyPath);
            var helpPath = Path.Combine(cmdPath, "Resources");
            ContextualHelp contHelp = null;
            string contHelpPath = Path.Combine(helpPath, Resources.FILE_HELP);
            if (System.IO.File.Exists(contHelpPath) == true)
            {
                contHelp = new ContextualHelp(ContextualHelpType.Url, contHelpPath);
            }
            if (contHelp != null)
            {
                pushButton.SetContextualHelp(contHelp);
            }

            var largeImg = SafeLoadPackImage( "CmdFlexDuctConvertLarge.png" ) ;
            var smallImg = SafeLoadPackImage( "CmdFlexDuctConvert.png" ) ;
            if ( largeImg != null ) pushButton.LargeImage = largeImg ;
            if ( smallImg != null ) pushButton.Image = smallImg ;

        }

        //???j???[????
        private void AddMenu(UIControlledApplication app)
        {
            app.CreateRibbonPanel(tabName);
            RibbonPanel projectPanel = app.CreateRibbonPanel(tabName, panelName);

            #region ?t???L?V?u???_?N?g???j???[

            {
                //???{???{?^???????
                PulldownButtonData dataFlex = new PulldownButtonData("Options1", "Flexible Duct Convert");
                RibbonItem itemFlex = projectPanel.AddItem(dataFlex);
                PulldownButton optionsBtnMEP = itemFlex as PulldownButton;
                var optLarge = SafeLoadPackImage( "CmdFlexDuctConvertLarge.png" ) ;
                var optSmall = SafeLoadPackImage( "CmdFlexDuctConvert.png" ) ;
                if ( optLarge != null ) optionsBtnMEP.LargeImage = optLarge ;
                if ( optSmall != null ) optionsBtnMEP.Image = optSmall ;

                #region ???j???[?c???[

                //
                {
                    var button = new PushButtonData("Flexible Duct Convert", "Flexible Duct Convert", ExecuteingAssemblyPath, "CmdFlexDuctConvert.Command");
                    button.AvailabilityClassName = "CmdFlexDuctConvert.Availability0";
                    //optionsBtnMEP.AddPushButton(button);

                    // ?c?[???`?b?v
                    button.ToolTip = Resources.MSG_TOOLTIP;
                    optionsBtnMEP.ToolTip = Resources.MSG_TOOLTIP;

                    // F1?w???v
                    var cmdPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    var helpPath = Path.Combine(cmdPath, "Resources");
                    ContextualHelp contHelp = null;
                    string contHelpPath = Path.Combine(helpPath, Resources.FILE_HELP);
                    if (System.IO.File.Exists(contHelpPath) == true)
                    {
                        contHelp = new ContextualHelp(ContextualHelpType.Url, contHelpPath);
                    }
                    if (contHelp != null)
                    {
                        button.SetContextualHelp(contHelp);
                        optionsBtnMEP.SetContextualHelp(contHelp);
                    }

                    optionsBtnMEP.AddPushButton(button);
                }

                #endregion ???j???[?c???[
            }

            #endregion ?t???L?V?u???_?N?g???j???[
        }

        private static BitmapSource GetEmbeddedImage(string name)
        {
            try
            {
                Assembly a = Assembly.GetExecutingAssembly();
                Stream s = a.GetManifestResourceStream(name);
                return BitmapFrame.Create(s);
            }
            catch
            {
                return null;
            }
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
    }

    // http://thebuildingcoder.typepad.com/blog/2011/02/enable-ribbon-items-in-zero-document-state.html
    // Enable Ribbon Items in Zero Document State
    public class Availability0
        : IExternalCommandAvailability
    {
        // ?R?????g:????????i?x??????????Revit?????????????j
        //     This callback will be called by Revit's user interface any time there is a contextual
        //     change. Therefore, the callback must be fast and is not permitted to modify the
        //     active document and be blocking in any way.
        public bool IsCommandAvailable(
            UIApplication a,
            CategorySet b)
        {
            if (a.ActiveUIDocument == null)
            {
                return false;
            }
            else
            {
                UIDocument uidoc = a.ActiveUIDocument;
                Document doc = uidoc.Document;

                if (doc.IsFamilyDocument)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
using System;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.EnhancedSectionBox
{
    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            string tabName = "REXJ Standalone";
            try { application.CreateRibbonTab(tabName); } catch { }

            string panelName = "Section Box";
            RibbonPanel panel = null;
            foreach (var p in application.GetRibbonPanels(tabName))
            {
                if (p.Name == panelName) { panel = p; break; }
            }
            if (panel == null)
                panel = application.CreateRibbonPanel(tabName, panelName);

            var pulldownData = new PulldownButtonData("SectionBoxPulldown", "Section Box");
            pulldownData.LargeImage = LoadPng("IDI_BTN_ENHANCEDSECTIONBOX_L.png", 32);
            pulldownData.Image = LoadPng("IDI_BTN_ENHANCEDSECTIONBOX_S.png", 16);
            var pulldown = panel.AddItem(pulldownData) as PulldownButton;

            string assemblyPath = Assembly.GetExecutingAssembly().Location;

            var btnAdjust = new PushButtonData(
                "CmdSectionBoxAdjustment",
                "Section Box Adjustment",
                assemblyPath,
                "ADSK.JExtRAC.EnhancedSectionBox.Commands.CmdSectionBoxAdjustment");
            btnAdjust.LargeImage = LoadPng("IDI_BTN_ADJUSTSECTIONBOX_L.png", 32);
            btnAdjust.Image = LoadPng("IDI_BTN_ADJUSTSECTIONBOX_S.png", 16);
            pulldown.AddPushButton(btnAdjust);

            var btnBoxView = new PushButtonData(
                "CmdBoxViewN",
                "Box View",
                assemblyPath,
                "ADSK.JExtRAC.EnhancedSectionBox.Commands.CmdBoxViewN");
            btnBoxView.LargeImage = LoadPng("IDI_BTN_ENHANCEDSECTIONBOX_L.png", 32);
            btnBoxView.Image = LoadPng("IDI_BTN_ENHANCEDSECTIONBOX_S.png", 16);
            pulldown.AddPushButton(btnBoxView);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application) => Result.Succeeded;

        private static BitmapImage LoadPng(string fileName, int size = 0)
        {
            try
            {
                string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string path = Path.Combine(dir, "Resources", "Images", fileName);
                if (!File.Exists(path)) return null;
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(path);
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                if (size > 0)
                {
                    bmp.DecodePixelWidth = size;
                    bmp.DecodePixelHeight = size;
                }
                bmp.EndInit();
                bmp.Freeze();
                return bmp;
            }
            catch { return null; }
        }
    }
}

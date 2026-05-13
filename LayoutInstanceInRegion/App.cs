using System;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using ADSK.JExtRAC.LayoutInstanceInRegion.Utils;

namespace ADSK.JExtRAC.LayoutInstanceInRegion
{
    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            CultureHelper.InitializeCulture();

            string tabName = "REXJ Standalone";
            try { application.CreateRibbonTab(tabName); } catch { }

            string panelName = "Array && Tag";
            RibbonPanel panel = null;
            foreach (var p in application.GetRibbonPanels(tabName))
            {
                if (p.Name == panelName) { panel = p; break; }
            }
            if (panel == null)
                panel = application.CreateRibbonPanel(tabName, panelName);

            var pulldown = FindPulldown(panel, "ArrayTagPulldown");
            if (pulldown == null)
            {
                var pulldownData = new PulldownButtonData("ArrayTagPulldown", "Array && Tag");
                pulldownData.LargeImage = LoadPng("IDI_BTN_AUTOMATIC_TAG_L.png", 32);
                pulldownData.Image = LoadPng("IDI_BTN_AUTOMATIC_TAG_S.png", 16);
                pulldown = panel.AddItem(pulldownData) as PulldownButton;
            }

            pulldown.AddSeparator();

            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            var btnData = new PushButtonData(
                "cmdLayoutInstance",
                "Instance Array Layout",
                assemblyPath,
                "ADSK.JExtRAC.LayoutInstanceInRegion.Commands.CmdLayoutInstance");
            btnData.LargeImage = LoadPng("IDI_BTN_LAYOUTINSTANCE_L.png", 32);
            btnData.Image = LoadPng("IDI_BTN_LAYOUTINSTANCE_S.png", 16);
            btnData.ToolTip = "Place family instances in an array pattern within a specified region";
            pulldown.AddPushButton(btnData);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application) => Result.Succeeded;

        private static PulldownButton FindPulldown(RibbonPanel panel, string name)
        {
            foreach (var item in panel.GetItems())
            {
                if (item is PulldownButton pb && pb.Name == name)
                    return pb;
            }
            return null;
        }

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

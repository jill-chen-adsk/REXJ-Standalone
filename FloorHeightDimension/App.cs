using System;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using ADSK.JExtRAC.FloorHeightDimension.Utils;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.FloorHeightDimension
{
    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            CultureHelper.InitializeCulture();

            string tabName = "REXJ Standalone";
            try { application.CreateRibbonTab(tabName); }
            catch { }

            string panelName = "Dimension";
            RibbonPanel panel = null;
            foreach (var p in application.GetRibbonPanels(tabName))
            {
                if (p.Name == panelName) { panel = p; break; }
            }
            if (panel == null)
                panel = application.CreateRibbonPanel(tabName, panelName);

            var pulldown = FindPulldown(panel, "DimensionPulldown");
            if (pulldown == null)
            {
                var pulldownData = new PulldownButtonData("DimensionPulldown", "Dimension");
                pulldownData.LargeImage = LoadPng("IDI_BTN_DIMENSION_L.png", 32);
                pulldownData.Image = LoadPng("IDI_BTN_DIMENSION_S.png", 16);
                pulldown = panel.AddItem(pulldownData) as PulldownButton;
            }

            pulldown.AddSeparator();

            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            var btn = new PushButtonData(
                "CmdFloorHeightDimension",
                "Level Height Dimension",
                assemblyPath,
                "ADSK.JExtRAC.FloorHeightDimension.Commands.CmdFloorHeightDimension");
            btn.LargeImage = LoadPng("IDI_BTN_FLOORHEIGHTDIMENSION_CONFIG_L.png", 32);
            btn.Image = LoadPng("IDI_BTN_FLOORHEIGHTDIMENSION_CONFIG_S.png", 16);
            pulldown.AddPushButton(btn);

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

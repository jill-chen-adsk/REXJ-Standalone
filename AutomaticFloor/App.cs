using System;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using ADSK.JExtRAC.AutomaticFloor.Utils;

namespace ADSK.JExtRAC.AutomaticFloor
{
    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            CultureHelper.InitializeCulture();

            string tabName = "REXJ Standalone";
            try { application.CreateRibbonTab(tabName); } catch { }

            string panelName = "Auto Floor";
            RibbonPanel panel = null;
            foreach (var p in application.GetRibbonPanels(tabName))
            {
                if (p.Name == panelName) { panel = p; break; }
            }
            if (panel == null)
                panel = application.CreateRibbonPanel(tabName, panelName);

            var pulldown = FindPulldown(panel, "AutoFloorPulldown");
            if (pulldown == null)
            {
                var pulldownData = new PulldownButtonData("AutoFloorPulldown", "Auto Floor");
                pulldownData.LargeImage = LoadPng("IDI_BTN_FLOOR_L.png", 32);
                pulldownData.Image = LoadPng("IDI_BTN_FLOOR_S.png", 16);
                pulldown = panel.AddItem(pulldownData) as PulldownButton;
            }

            string assemblyPath = Assembly.GetExecutingAssembly().Location;

            var btnArch = new PushButtonData(
                "CmdCreateArchitectureFloor",
                "Architecture Floor",
                assemblyPath,
                "ADSK.JExtRAC.AutomaticFloor.Commands.CmdCreateArchitectureFloor");
            btnArch.LargeImage = LoadPng("IDI_BTN_AUTOMATIC_FLOOR_DESIGN_L.png", 32);
            btnArch.Image = LoadPng("IDI_BTN_AUTOMATIC_FLOOR_DESIGN_S.png", 16);
            pulldown.AddPushButton(btnArch);

            var btnStruct = new PushButtonData(
                "CmdCreateStructuralFloor",
                "Structural Floor",
                assemblyPath,
                "ADSK.JExtRAC.AutomaticFloor.Commands.CmdCreateStructuralFloor");
            btnStruct.LargeImage = LoadPng("IDI_BTN_AUTOMATIC_FLOOR_STRUCTURE_L.png", 32);
            btnStruct.Image = LoadPng("IDI_BTN_AUTOMATIC_FLOOR_STRUCTURE_S.png", 16);
            pulldown.AddPushButton(btnStruct);

            var btnSlab = new PushButtonData(
                "CmdFoundationSlab",
                "Foundation Slab",
                assemblyPath,
                "ADSK.JExtRAC.AutomaticFloor.Commands.CmdFoundationSlab");
            btnSlab.LargeImage = LoadPng("IDI_BTN_AUTOMATIC_FLOOR_BASIC_L.png", 32);
            btnSlab.Image = LoadPng("IDI_BTN_AUTOMATIC_FLOOR_BASIC_S.png", 16);
            pulldown.AddPushButton(btnSlab);

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

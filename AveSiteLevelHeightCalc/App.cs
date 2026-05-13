using System;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using ADSK.JExtRAC.AveSiteLevelHeightCalc.Utils;

namespace ADSK.JExtRAC.AveSiteLevelHeightCalc
{
    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            CultureHelper.InitializeCulture();

            string tabName = "REXJ Standalone";
            try { application.CreateRibbonTab(tabName); } catch { }

            string panelName = "Code Check";
            RibbonPanel panel = null;
            foreach (var p in application.GetRibbonPanels(tabName))
            {
                if (p.Name == panelName) { panel = p; break; }
            }
            if (panel == null)
                panel = application.CreateRibbonPanel(tabName, panelName);

            var pulldown = FindPulldown(panel, "CodeCheckPulldown");
            if (pulldown == null)
            {
                var pulldownData = new PulldownButtonData("CodeCheckPulldown", "Code Check");
                pulldownData.LargeImage = LoadPng("IDI_BTN_CHECKLAW_L.png", 32);
                pulldownData.Image = LoadPng("IDI_BTN_CHECKLAW_S.png", 16);
                pulldown = panel.AddItem(pulldownData) as PulldownButton;
            }

            if (pulldown == null)
                return Result.Failed;

            string assemblyPath = Assembly.GetExecutingAssembly().Location;

            if (FindPushButton(pulldown, "CmdAvgSiteLevelHeightCalc") == null)
            {
                pulldown.AddSeparator();

                var btn = new PushButtonData(
                    "CmdAvgSiteLevelHeightCalc",
                    "Avg Site Level Calc",
                    assemblyPath,
                    "ADSK.JExtRAC.AveSiteLevelHeightCalc.Create.CmdCreate");
                btn.LargeImage = LoadPng("IDI_BTN_AVESITELEVELHEIGHTCALC_CREATE_L.png", 32);
                btn.Image = LoadPng("IDI_BTN_AVESITELEVELHEIGHTCALC_CREATE_S.png", 16);
                pulldown.AddPushButton(btn);
            }

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication _) => Result.Succeeded;

        private static PulldownButton FindPulldown(RibbonPanel panel, string name)
        {
            foreach (RibbonItem item in panel.GetItems())
            {
                if (item is PulldownButton pb && pb.Name == name)
                    return pb;
            }
            return null;
        }

        private static PushButton FindPushButton(PulldownButton pulldown, string name)
        {
            foreach (RibbonItem item in pulldown.GetItems())
            {
                if (item is PushButton pb && pb.Name == name)
                    return pb;
            }
            return null;
        }

        private static BitmapImage LoadPng(string fileName, int size = 0)
        {
            try
            {
                string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string path = Path.Combine(dir ?? "", "Resources", "Images", fileName);
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

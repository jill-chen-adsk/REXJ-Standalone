using System;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using ADSK.JExtRAC.CheckingALVS.Utils;

namespace ADSK.JExtRAC.CheckingALVS
{
    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            CultureHelper.InitializeCulture();

            string tabName = "REXJ Standalone";
            try { application.CreateRibbonTab(tabName); }
            catch { }

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
                var pb = new PulldownButtonData("CodeCheckPulldown", "Code Check");
                pb.LargeImage = LoadPng("IDI_BTN_CHECKLAW_L.png", 32);
                pb.Image = LoadPng("IDI_BTN_CHECKLAW_S.png", 16);
                pulldown = panel.AddItem(pb) as PulldownButton;
            }

            string asm = Assembly.GetExecutingAssembly().Location;

            pulldown.AddSeparator();

            var cmd1 = new PushButtonData(
                nameof(NaturalLighting.CmdNaturalLighting),
                "Daylight Check",
                asm,
                "ADSK.JExtRAC.CheckingALVS.NaturalLighting.CmdNaturalLighting");
            cmd1.LargeImage = LoadPng("IDI_BTN_CHECKINGALVS_NATURALLIGNTING_L.png", 32);
            cmd1.Image = LoadPng("IDI_BTN_CHECKINGALVS_NATURALLIGNTING_S.png", 16);
            pulldown.AddPushButton(cmd1);

            var cmd2 = new PushButtonData(
                nameof(SmokeExhaustion.CmdSmokeExhaustion),
                "Smoke Exhaust Check",
                asm,
                "ADSK.JExtRAC.CheckingALVS.SmokeExhaustion.CmdSmokeExhaustion");
            cmd2.LargeImage = LoadPng("IDI_BTN_CHECKINGALVS_SMOKEEXHAUSTION_L.png", 32);
            cmd2.Image = LoadPng("IDI_BTN_CHECKINGALVS_SMOKEEXHAUSTION_S.png", 16);
            pulldown.AddPushButton(cmd2);

            var cmd3 = new PushButtonData(
                nameof(AirVentilation.CmdAirVentilation),
                "Ventilation Check",
                asm,
                "ADSK.JExtRAC.CheckingALVS.AirVentilation.CmdAirVentilation");
            cmd3.LargeImage = LoadPng("IDI_BTN_CHECKINGALVS_AIRVENTILATION_L.png", 32);
            cmd3.Image = LoadPng("IDI_BTN_CHECKINGALVS_AIRVENTILATION_S.png", 16);
            pulldown.AddPushButton(cmd3);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application) => Result.Succeeded;

        private static PulldownButton FindPulldown(RibbonPanel panel, string itemName)
        {
            foreach (RibbonItem ri in panel.GetItems())
                if (ri is PulldownButton pb && pb.Name == itemName)
                    return pb;
            return null;
        }

        private static BitmapImage LoadPng(string fileName, int size)
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

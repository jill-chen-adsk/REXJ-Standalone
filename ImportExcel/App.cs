using System;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using ADSK.JExtRAC.ImportExcel.Utils;

namespace ADSK.JExtRAC.ImportExcel
{
    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            CultureHelper.InitializeCulture();

            string tabName = "REXJ Standalone";
            try { application.CreateRibbonTab(tabName); } catch { }

            string panelName = "Excel";
            RibbonPanel panel = null;
            foreach (var p in application.GetRibbonPanels(tabName))
            {
                if (p.Name == panelName) { panel = p; break; }
            }
            if (panel == null)
                panel = application.CreateRibbonPanel(tabName, panelName);

            var pulldown = FindPulldown(panel, "ExcelPulldown");
            if (pulldown == null)
            {
                var pulldownData = new PulldownButtonData("ExcelPulldown", "Excel");
                pulldownData.LargeImage = LoadPng("IDI_BTN_EXCEL_L.png", 32);
                pulldownData.Image = LoadPng("IDI_BTN_EXCEL_S.png", 16);
                pulldown = panel.AddItem(pulldownData) as PulldownButton;
            }

            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            var btnImport = new PushButtonData(
                "CmdImportExcel",
                "Import Excel",
                assemblyPath,
                "ADSK.JExtRAC.ImportExcel.Commands.CmdImportExcel");
            btnImport.LargeImage = LoadPng("IDI_BTN_IMPORT_EXCEL_L.png", 32);
            btnImport.Image = LoadPng("IDI_BTN_IMPORT_EXCEL_S.png", 16);

            pulldown.AddPushButton(btnImport);

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

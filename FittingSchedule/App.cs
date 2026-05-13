using System;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.FittingSchedule
{
    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            CultureHelper.ApplySavedCulture();

            string tabName = "REXJ Standalone";
            try { application.CreateRibbonTab(tabName); } catch { }

            var panel = application.CreateRibbonPanel(tabName, "Window/Door View");
            string assemblyPath = Assembly.GetExecutingAssembly().Location;

            var pulldownData = new PulldownButtonData("WinDoorViewPulldown", "Window/Door\nView");
            pulldownData.LargeImage = LoadPng("IDI_BTN_FITTINGSCHEDULE_L.png", 32);
            pulldownData.Image = LoadPng("IDI_BTN_FITTINGSCHEDULE_S.png", 16);
            var pulldown = panel.AddItem(pulldownData) as PulldownButton;

            var btnCreate = new PushButtonData(
                "CmdFittingCreate",
                "Create (Doors/Windows) View",
                assemblyPath,
                "ADSK.JExtRAC.FittingSchedule.CreateAndEdit.CmdCreateAndEdit");
            btnCreate.LargeImage = LoadPng("IDI_BTN_FITTINGSCHEDULE_CREATEANDEDIT_L.png", 32);
            btnCreate.Image = LoadPng("IDI_BTN_FITTINGSCHEDULE_CREATEANDEDIT_S.png", 16);
            pulldown.AddPushButton(btnCreate);

            var btnLayout = new PushButtonData(
                "CmdFittingLayout",
                "Layout (Doors/Windows) Views in Sheet",
                assemblyPath,
                "ADSK.JExtRAC.FittingSchedule.Layout.CmdLayout");
            btnLayout.LargeImage = LoadPng("IDI_BTN_FITTINGSCHEDULE_LAYOUT_L.png", 32);
            btnLayout.Image = LoadPng("IDI_BTN_FITTINGSCHEDULE_LAYOUT_S.png", 16);
            pulldown.AddPushButton(btnLayout);

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

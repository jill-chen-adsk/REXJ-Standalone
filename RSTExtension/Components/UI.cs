using System;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using JExtComCompat;

namespace RSTExtension.Components
{
    public class UI : RvtUI
    {
        private readonly Attribute _attribute;

        public UI(Attribute attribute, UIControlledApplication application) : base(application)
        {
            _attribute = attribute;
        }

        public void SetRibbonPanel()
        {
            string tabName = _attribute.ResourceText("IDS_BTN_TABNAME");
            string panelName = _attribute.ResourceText("IDS_BTN_PANELNAME");

            try
            {
                CreateRibbonTab(tabName);
            }
            catch
            {
                // Tab may already exist.
            }

            RibbonPanel panel = CreateRibbonPanel(tabName, panelName);

            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            string helpPath = Path.Combine(Path.GetDirectoryName(assemblyPath) ?? "", "Resources", _attribute.ResourceText("IDS_TXT_RSTHELPHTM"));
            ContextualHelp? help = null;
            if (File.Exists(helpPath))
            {
                help = new ContextualHelp(ContextualHelpType.ChmFile, helpPath);
            }

            var pulldownData = new PulldownButtonData("RSTStructuralExtPulldown", _attribute.ResourceText("IDS_PUL_STRUCTURAL_EXT"));
            pulldownData.Image = LoadPng("IDI_BTN_CORRECTFRAMINGPLAN_CONFIG_S.png", 16);
            pulldownData.LargeImage = LoadPng("IDI_BTN_CORRECTFRAMINGPLAN_CONFIG_L.png", 32);

            var pulldown = panel.AddItem(pulldownData) as PulldownButton;
            if (pulldown == null)
            {
                return;
            }

            var cfp = new PushButtonData(
                "CmdCorrectFramingPlan",
                "Correct Framing Plan",
                assemblyPath,
                "RSTExtension.Config.CmdCorrectFramingPlan");
            cfp.ToolTip = _attribute.ResourceText("IDS_BTN_CORRECTFRAMINGPLAN_TT");
            cfp.LargeImage = LoadPng("IDI_BTN_CORRECTFRAMINGPLAN_CONFIG_L.png", 32);
            cfp.Image = LoadPng("IDI_BTN_CORRECTFRAMINGPLAN_CONFIG_S.png", 16);
            if (help != null)
            {
                cfp.SetContextualHelp(help);
            }
            pulldown.AddPushButton(cfp);

            var esm = new PushButtonData(
                "CmdExclusionSpecialMention",
                "Exclusion Special Mention",
                assemblyPath,
                "RSTExtension.Config.CmdExclusionSpecialMention");
            esm.ToolTip = _attribute.ResourceText("IDS_BTN_ESM_TT");
            esm.LargeImage = LoadPng("IDI_BTN_EXCLUSIONSPECIALMENTION_CONFIG_L.png", 32);
            esm.Image = LoadPng("IDI_BTN_EXCLUSIONSPECIALMENTION_CONFIG_S.png", 16);
            if (help != null)
            {
                esm.SetContextualHelp(help);
            }
            pulldown.AddPushButton(esm);
        }

        private static BitmapImage? LoadPng(string fileName, int size)
        {
            try
            {
                string? dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string path = Path.Combine(dir ?? "", "Resources", "Images", fileName);
                if (!File.Exists(path))
                {
                    return null;
                }

                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(path, UriKind.Absolute);
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
            catch
            {
                return null;
            }
        }
    }
}

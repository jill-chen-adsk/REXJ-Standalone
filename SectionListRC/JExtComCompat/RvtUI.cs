using System.Collections.Generic;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;

namespace SectionListRC.JExtComCompat
{
    public class RvtUI
    {
        private readonly UIControlledApplication? _ctrl;
        private readonly UIApplication? _ui;

        public RvtUI(UIControlledApplication application)
        {
            _ctrl = application;
        }

        public RvtUI(UIApplication application)
        {
            _ui = application;
        }

        public IList<RibbonPanel> GetRibbonPanel(string tabName)
        {
            if (_ctrl != null)
            {
                return _ctrl.GetRibbonPanels(tabName);
            }
            return _ui!.GetRibbonPanels(tabName);
        }

        public void CreateRibbonTab(string tabName)
        {
            _ctrl?.CreateRibbonTab(tabName);
        }

        public RibbonPanel CreateRibbonPanel(string tabName, string panelName)
        {
            if (_ctrl != null)
            {
                return _ctrl.CreateRibbonPanel(tabName, panelName);
            }
            return _ui!.CreateRibbonPanel(tabName, panelName);
        }

        public PushButtonData CreatePushButtonData(
            string name,
            string text,
            BitmapImage? imgS,
            BitmapImage? imgL,
            string tooltip,
            string longDesc,
            object? comboItems,
            string assembly,
            string className,
            string availClassName)
        {
            _ = comboItems;
            _ = availClassName;
            var data = new PushButtonData(name, text, assembly, className)
            {
                ToolTip = tooltip,
                LongDescription = longDesc,
                Image = imgS,
                LargeImage = imgL,
            };
            return data;
        }

        public void SetStackItems(RibbonPanel panel, IList<RibbonItemData> itemDatas, int mode)
        {
            if (itemDatas == null || itemDatas.Count == 0)
            {
                return;
            }

            if (mode == 1 && itemDatas.Count == 1)
            {
                panel.AddItem(itemDatas[0]);
                return;
            }

            if (itemDatas.Count == 2)
            {
                panel.AddStackedItems(itemDatas[0], itemDatas[1]);
                return;
            }

            for (int i = 0; i < itemDatas.Count; i += 3)
            {
                int remain = itemDatas.Count - i;
                if (remain >= 3)
                {
                    panel.AddStackedItems(itemDatas[i], itemDatas[i + 1], itemDatas[i + 2]);
                }
                else if (remain == 2)
                {
                    panel.AddStackedItems(itemDatas[i], itemDatas[i + 1]);
                }
                else
                {
                    panel.AddItem(itemDatas[i]);
                }
            }
        }
    }
}

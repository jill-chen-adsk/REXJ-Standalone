using System.Drawing;
using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using Autodesk.Revit.UI;

namespace MepManholeTool.Utils
{
    public static class RevitUIUtilis
    {
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        
        /// <summary>
        /// Revitボタンにアイコンをセットします。
        /// </summary>
        /// <param name="button">ボタン。</param>
        /// <param name="bitmap">ビットマップ。</param>
        public static bool TrySetBitmapResourceToButton(Autodesk.Revit.UI.RibbonButton button, Bitmap bitmap)
        {
            if (button == null || bitmap == null) return false;

            IntPtr hBitmap = bitmap.GetHbitmap();

            try
            {
                button.LargeImage = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                DeleteObject(hBitmap);
            }

            return true;
        }
        
        #region リボンメニュー
        /// <summary>
        /// 同じ名前のタブがなければ作成します。
        /// </summary>
        /// <param name="application">UIControlledApplication。</param>
        /// <param name="tabTitle">追加するタブ。</param>
        public static void CreateRibbonTab(UIControlledApplication application, string tabTitle)
        {
            if (HasSameTabTitle(tabTitle))
                return;

            try
            {
                application.CreateRibbonTab(tabTitle);
            }
            catch
            {
                // Tab may already exist (e.g. another add-in or race with HasSameTabTitle).
            }
        }
        
        /// <summary>リボンに同じタブが存在するか確認します。</summary>
        /// <param name="tabTitle">タブタイトル。</param>
        /// <returns>成功した場合はtrue、その他の場合はfalse。</returns>
        public static bool HasSameTabTitle(string tabTitle)
        {
            foreach (var tab in Autodesk.Windows.ComponentManager.Ribbon.Tabs)
                if (tab.Title == tabTitle) return true;

            return false;
        }
        #endregion
        
        #region パネル
        /// <summary>
        /// タブ内に同じ名前のパネルがなければ作成します。あればそのパネルを返却します。
        /// </summary>
        /// <param name="application"></param>
        /// <param name="tabTitle">タブタイトル。</param>
        /// <param name="panelTitle">配置するパネル名。</param>
        public static Autodesk.Revit.UI.RibbonPanel CreateRibbonPanel(UIControlledApplication application, string tabTitle, string panelTitle)
        {
            var panels = application.GetRibbonPanels(tabTitle);
            foreach (var panel in panels)
                // 同じ名前のパネルがあればそれを返却。
                if (panel.Name == panelTitle) return panel;
            // 同じ名前のパネルがない場合、パネル作成。
            return application.CreateRibbonPanel(tabTitle, panelTitle);
        }
        #endregion
    }
}
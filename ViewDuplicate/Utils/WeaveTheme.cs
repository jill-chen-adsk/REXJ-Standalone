using System;
using System.Windows;
using Autodesk.Revit.UI;

namespace ADSK.ViewExtension.ViewDuplicate.Utils
{
    public static class WeaveTheme
    {
        public static bool IsDarkTheme =>
            UIThemeManager.CurrentTheme == UITheme.Dark;

        public static void Apply(Window window)
        {
            if (window == null)
                return;

            ApplyPalette(window.Resources);

            window.SetResourceReference(Window.ForegroundProperty, "Weave.Brush.Text.Primary");

            // Chrome windows keep a transparent background so their rounded outer border shows.
            if (window is IWeaveChromeWindow chrome)
                WeaveWindowChrome.ApplyTheme(chrome);
            else
                window.SetResourceReference(Window.BackgroundProperty, "Weave.Brush.Surface");
        }

        public static void Apply(
            Window window,
            IWeaveChromeWindow chrome,
            string title,
            System.Action onClose = null,
            bool showCloseButton = true)
        {
            Apply(window);
            WeaveWindowChrome.Initialize(window, chrome, title, onClose, showCloseButton);
        }

        static readonly Uri DarkPaletteUri =
            new Uri("/ADSK.ViewExtension.ViewDuplicate;component/Resources/WeaveDialogResources.Dark.xaml", UriKind.Relative);

        // Light theme uses the defaults in WeaveDialogResources.xaml; dark theme merges its overrides last so they win.
        static void ApplyPalette(ResourceDictionary resources)
        {
            if (IsDarkTheme)
                resources.MergedDictionaries.Add(new ResourceDictionary { Source = DarkPaletteUri });
        }
    }
}

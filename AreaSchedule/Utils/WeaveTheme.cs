using System;
using System.Windows;
using System.Windows.Media;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.AreaSchedule.Utils
{
    /// <summary>
    /// Applies Revit UI theme (light/dark) to Weave WPF dialogs.
    /// </summary>
    public static class WeaveTheme
    {
        public static bool IsDarkTheme =>
            UIThemeManager.CurrentTheme == UITheme.Dark;

        public static SolidColorBrush BrushFromHex(string hex)
        {
            var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
            brush.Freeze();
            return brush;
        }

        public static void Apply(Window window)
        {
            if (window == null)
                return;

            ApplyPalette(window.Resources);

            window.SetResourceReference(Window.BackgroundProperty, "Weave.Brush.Surface");
            window.SetResourceReference(Window.ForegroundProperty, "Weave.Brush.Text.Primary");

            if (window is IWeaveChromeWindow chrome)
                WeaveWindowChrome.ApplyTheme(chrome);
        }

        public static void Apply(
            Window window,
            IWeaveChromeWindow chrome,
            string title,
            Action onClose = null,
            bool showCloseButton = true)
        {
            Apply(window);
            WeaveWindowChrome.Initialize(window, chrome, title, onClose, showCloseButton);
        }

        static void ApplyPalette(ResourceDictionary resources)
        {
            if (IsDarkTheme)
            {
                SetPair(resources, "#E0E8F0", "#C0CCD8", "#3A4F63", "#263545", "#2E3F50", "#34495F");
            }
            else
            {
                SetPair(resources, "#3C3C3C", "#6B6B6B", "#D9D9D9", "#FFFFFF", "#F5F5F5", "#FFFFFF");
            }
        }

        static void SetPair(
            ResourceDictionary resources,
            string textPrimary,
            string textSecondary,
            string border,
            string surface,
            string surfaceSubtle,
            string inputSurface)
        {
            SetColorBrush(resources, "Weave.Color.Text.Primary", "Weave.Brush.Text.Primary", textPrimary);
            SetColorBrush(resources, "Weave.Color.Text.Secondary", "Weave.Brush.Text.Secondary", textSecondary);
            SetColorBrush(resources, "Weave.Color.Border", "Weave.Brush.Border", border);
            SetColorBrush(resources, "Weave.Color.Surface", "Weave.Brush.Surface", surface);
            SetColorBrush(resources, "Weave.Color.Surface.Subtle", "Weave.Brush.Surface.Subtle", surfaceSubtle);
            SetColorBrush(resources, "Weave.Color.Input", "Weave.Brush.Input", inputSurface);
        }

        static void SetColorBrush(ResourceDictionary resources, string colorKey, string brushKey, string hex)
        {
            var color = (Color)ColorConverter.ConvertFromString(hex);
            resources[colorKey] = color;

            var brush = new SolidColorBrush(color);
            brush.Freeze();
            resources[brushKey] = brush;
        }
    }
}

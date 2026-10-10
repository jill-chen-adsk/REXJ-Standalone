using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ADSK.JExtRAC.CheckingALVS.Utils
{
    public interface IWeaveChromeWindow
    {
        System.Windows.Controls.Border ChromeOuterBorder { get; }
        System.Windows.Controls.Grid ChromeTitleBar { get; }
        System.Windows.Controls.Border ChromeDivider { get; }
        System.Windows.Controls.TextBlock ChromeTitleText { get; }
        System.Windows.Controls.Button ChromeCloseButton { get; }
    }

    public static class WeaveWindowChrome
    {
        public static void Initialize(
            Window window,
            IWeaveChromeWindow chrome,
            string title,
            Action onClose = null,
            bool showCloseButton = true)
        {
            if (window == null || chrome == null)
                return;

            SetTitle(window, chrome, title);

            chrome.ChromeTitleBar.MouseLeftButtonDown += (_, e) =>
            {
                if (e.ButtonState == System.Windows.Input.MouseButtonState.Pressed)
                    window.DragMove();
            };

            if (chrome.ChromeCloseButton != null)
            {
                chrome.ChromeCloseButton.Visibility = showCloseButton
                    ? Visibility.Visible
                    : Visibility.Collapsed;

                if (showCloseButton)
                {
                    chrome.ChromeCloseButton.Click += (_, __) =>
                    {
                        if (onClose != null)
                            onClose();
                        else
                            window.Close();
                    };
                }
            }

            ApplyTheme(chrome);
        }

        public static void SetTitle(Window window, IWeaveChromeWindow chrome, string title)
        {
            string safeTitle = title ?? string.Empty;
            window.Title = safeTitle;
            if (chrome?.ChromeTitleText != null)
                chrome.ChromeTitleText.Text = safeTitle;
        }

        public static void ApplyTheme(IWeaveChromeWindow chrome)
        {
            if (chrome == null)
                return;

            SetBrush(chrome.ChromeOuterBorder, System.Windows.Controls.Border.BackgroundProperty, "Weave.Brush.Surface");
            SetBrush(chrome.ChromeTitleBar, System.Windows.Controls.Panel.BackgroundProperty, "Weave.Brush.Surface.Subtle");
            SetBrush(chrome.ChromeDivider, System.Windows.Controls.Border.BackgroundProperty, "Weave.Brush.Border");
            SetBrush(chrome.ChromeTitleText, System.Windows.Controls.TextBlock.ForegroundProperty, "Weave.Brush.Text.Primary");
            SetBrush(chrome.ChromeCloseButton, System.Windows.Controls.Control.ForegroundProperty, "Weave.Brush.Text.Secondary");
        }

        static void SetBrush(FrameworkElement element, DependencyProperty property, string resourceKey)
        {
            element?.SetResourceReference(property, resourceKey);
        }
    }
}

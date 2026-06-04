using System;
using System.Windows;
using System.Windows.Media;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.FittingSchedule.Components
{
    public partial class ProgressWindow : Window
    {
        private readonly bool _isDarkTheme;
        private int _maximum = 100;
        private int _value = 0;

        public ProgressWindow()
        {
            InitializeComponent();

            _isDarkTheme = UIThemeManager.CurrentTheme == UITheme.Dark;
            ApplyThemeColors();
        }

        private void ApplyThemeColors()
        {
            if (_isDarkTheme)
            {
                // Weave Dark Blue Level 1 surface
                outerBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#454f61"));
                titleText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f0f0f0"));
                statusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#b0b8c4"));
                // Track: subtle darker background
                progressTrack.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2e3440"));
                // Fill: Weave blue
                progressFill.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0696d7"));
            }
            else
            {
                // Weave Light Gray Level 1 surface
                outerBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));
                titleText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1e1e1e"));
                statusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5c5c5c"));
                // Track: light gray
                progressTrack.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e0e0e0"));
                // Fill: Weave blue
                progressFill.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0696d7"));
            }
        }

        public void SetStatus(string text)
        {
            statusText.Text = text;
            _value = 0;
            UpdateProgressBar();
        }

        public void SetMaximum(int max, int value)
        {
            _maximum = Math.Max(max, 1);
            _value = Math.Min(value, _maximum);
            UpdateProgressBar();
        }

        public void SetValue(int value)
        {
            if (value <= _maximum)
                _value = value;
            UpdateProgressBar();
        }

        private void UpdateProgressBar()
        {
            if (_maximum <= 0) return;

            double fraction = (double)_value / _maximum;
            double trackWidth = progressTrack.ActualWidth;

            if (trackWidth <= 0)
                trackWidth = 432; // fallback: 480 - 24*2 padding

            progressFill.Width = Math.Max(0, trackWidth * fraction);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateProgressBar();
        }
    }
}

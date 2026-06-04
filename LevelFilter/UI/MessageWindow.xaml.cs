using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.LevelFilter.UI
{
    public partial class MessageWindow : Window
    {
        private readonly bool _isDarkTheme;

        public MessageWindow(string title, string message)
        {
            InitializeComponent();
            _isDarkTheme = UIThemeManager.CurrentTheme == UITheme.Dark;
            titleText.Text = title;
            messageText.Text = message;
            ApplyThemeColors();
        }

        private static SolidColorBrush Brush(string hex)
        {
            var b = new SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString(hex));
            b.Freeze();
            return b;
        }

        private void ApplyThemeColors()
        {
            var accent = Brush("#0696d7");
            if (_isDarkTheme)
            {
                outerBorder.Background = Brush("#263545");
                titleBarGrid.Background = Brush("#263545");
                dividerBorder.Background = Brush("#3a4f63");
                titleText.Foreground = Brush("#e0e8f0");
                messageText.Foreground = Brush("#c0ccd8");
                btnClose.Foreground = Brush("#8fa4b8");
                footerBorder.Background = Brushes.Transparent;
            }
            else
            {
                outerBorder.Background = Brush("#ffffff");
                titleBarGrid.Background = Brush("#f5f5f5");
                dividerBorder.Background = Brush("#e0e0e0");
                titleText.Foreground = Brush("#1e1e1e");
                messageText.Foreground = Brush("#3c3c3c");
                btnClose.Foreground = Brush("#5c5c5c");
                footerBorder.Background = Brushes.Transparent;
            }
            btnOk.Foreground = Brushes.White;
            btnOk.Background = accent;
        }

        protected override void OnContentRendered(System.EventArgs e)
        {
            base.OnContentRendered(e);
            var accent = Brush("#0696d7");
            btnOk.ApplyTemplate();
            if (VisualTreeHelper.GetChildrenCount(btnOk) > 0)
            {
                var border = VisualTreeHelper.GetChild(btnOk, 0) as System.Windows.Controls.Border;
                if (border != null) border.Background = accent;
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
        private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
        private void BtnOk_Click(object sender, RoutedEventArgs e) => Close();

        public static void Show(string title, string message)
        {
            var window = new MessageWindow(title, message);
            window.ShowDialog();
        }
    }
}

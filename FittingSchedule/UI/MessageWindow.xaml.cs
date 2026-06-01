using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.FittingSchedule.UI
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

        private void ApplyThemeColors()
        {
            if (_isDarkTheme)
            {
                outerBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#454f61"));
                titleBarGrid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3b4453"));
                dividerBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2e3440"));
                titleText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f0f0f0"));
                messageText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#d0d4da"));
                btnClose.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#b0b8c4"));
                footerBorder.Background = Brushes.Transparent;

                var okBg = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0696d7"));
                btnOk.Background = okBg;
                btnOk.Foreground = Brushes.White;

                var okTemplate = btnOk.Template;
                btnOk.ApplyTemplate();
                if (VisualTreeHelper.GetChildrenCount(btnOk) > 0)
                {
                    var border = VisualTreeHelper.GetChild(btnOk, 0) as System.Windows.Controls.Border;
                    if (border != null) border.Background = okBg;
                }
            }
            else
            {
                outerBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));
                titleBarGrid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f5f5f5"));
                dividerBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e0e0e0"));
                titleText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1e1e1e"));
                messageText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3c3c3c"));
                btnClose.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5c5c5c"));
                footerBorder.Background = Brushes.Transparent;

                var okBg = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0696d7"));
                btnOk.Background = okBg;
                btnOk.Foreground = Brushes.White;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SetOkButtonBackground();
        }

        protected override void OnContentRendered(System.EventArgs e)
        {
            base.OnContentRendered(e);
            SetOkButtonBackground();
        }

        private void SetOkButtonBackground()
        {
            var okBg = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0696d7"));
            btnOk.ApplyTemplate();
            if (VisualTreeHelper.GetChildrenCount(btnOk) > 0)
            {
                var border = VisualTreeHelper.GetChild(btnOk, 0) as System.Windows.Controls.Border;
                if (border != null) border.Background = okBg;
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public static void Show(string title, string message)
        {
            var window = new MessageWindow(title, message);
            window.ShowDialog();
        }
    }
}

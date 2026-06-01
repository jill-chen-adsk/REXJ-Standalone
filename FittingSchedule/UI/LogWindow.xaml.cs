using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Web.WebView2.Core;
using Autodesk.Revit.UI;

using RvtExtApp = ADSK.JExtRAC.FittingSchedule;

namespace ADSK.JExtRAC.FittingSchedule.UI
{
    public partial class LogWindow : Window
    {
        private readonly RvtExtApp.Components.Attribute _cmpAttribute;
        private readonly StringBuilder _strLog;
        private readonly bool _isDarkTheme;

        public LogWindow(RvtExtApp.Components.Attribute cmpAttribute, StringBuilder strLog)
        {
            InitializeComponent();
            _cmpAttribute = cmpAttribute;
            _strLog = strLog;

            _isDarkTheme = UIThemeManager.CurrentTheme == UITheme.Dark;
            ApplyThemeColors();

            Loaded += OnWindowLoaded;
        }

        private void ApplyThemeColors()
        {
            if (_isDarkTheme)
            {
                outerBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#263545"));
                titleBarGrid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#263545"));
                titleText.Foreground = Brushes.White;
                dividerBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3a4f63"));
                btnClose.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8fa4b8"));
            }
            else
            {
                outerBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f0f0f0"));
                titleBarGrid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f0f0f0"));
                titleText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1a1a1a"));
                dividerBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#d0d0d0"));
                btnClose.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#555555"));
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            var userDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ADSK", "JExtRAC", "FittingSchedule", "WebView2");

            var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder);
            await webView.EnsureCoreWebView2Async(env);
            webView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;

            string html = LoadEmbeddedHtml();
            webView.NavigateToString(html);

            webView.CoreWebView2.NavigationCompleted += async (s, args) =>
            {
                if (args.IsSuccess)
                {
                    await SendLogDataAsync();
                }
            };
        }

        private string LoadEmbeddedHtml()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "ADSK.JExtRAC.FittingSchedule.UI.LogDialog.html";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private async Task SendLogDataAsync()
        {
            var theme = _isDarkTheme ? "dark" : "light";
            var logText = JsonSerializer.Serialize(_strLog.ToString());

            var script = $"window.initLog({{ theme: \"{theme}\", logText: {logText} }})";
            await webView.CoreWebView2.ExecuteScriptAsync(script);
        }

        private void OnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string json = e.WebMessageAsJson;
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                string action = root.GetProperty("action").GetString();

                if (action == "save")
                {
                    var dlg = new Microsoft.Win32.SaveFileDialog
                    {
                        Title = "Export log",
                        DefaultExt = "txt",
                        Filter = "Text Documents (*.txt)|*.txt|All files (*.*)|*.*",
                        FilterIndex = 1
                    };
                    if (dlg.ShowDialog() == true)
                    {
                        File.WriteAllText(dlg.FileName, _strLog.ToString(), Encoding.UTF8);
                    }
                    return;
                }
                else if (action == "close")
                {
                    this.Close();
                }
            }
            catch
            {
                this.Close();
            }
        }
    }
}

using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Web.WebView2.Core;
using Autodesk.Revit.UI;

using RvtExtApp = ADSK.JExtRAC.FittingSchedule;

namespace ADSK.JExtRAC.FittingSchedule.Layout
{
    public partial class LayoutWindow : Window
    {
        private readonly RvtExtApp.Components.Attribute _cmpAttribute;
        private readonly RvtExtApp.Entities.DtViewSheet _entDtViewSheet;
        private readonly RvtExtApp.Entities.DtCmd _entDtCmd;
        private readonly bool _isDarkTheme;

        public LayoutWindow(
            RvtExtApp.Components.Attribute cmpAttribute,
            RvtExtApp.Entities.DtViewSheet entDtViewSheet,
            RvtExtApp.Entities.DtCmd entDtCmd)
        {
            InitializeComponent();
            _cmpAttribute = cmpAttribute;
            _entDtViewSheet = entDtViewSheet;
            _entDtCmd = entDtCmd;

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
            DialogResult = false;
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
                    await SendInitialDataAsync();
                }
            };
        }

        private string LoadEmbeddedHtml()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "ADSK.JExtRAC.FittingSchedule.Layout.LayoutDialog.html";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private async Task SendInitialDataAsync()
        {
            var existTable = _entDtViewSheet.DataViewExist;
            var targetTable = _entDtViewSheet.DataViewTarget;

            var viewsExist = new object[existTable.DefaultView.Count];
            for (int i = 0; i < existTable.DefaultView.Count; i++)
            {
                var row = existTable.DefaultView[i];
                viewsExist[i] = new
                {
                    id = Convert.ToInt32(row["ID"]),
                    name = row["NAME"]?.ToString()
                };
            }

            var viewsTarget = new object[targetTable.Rows.Count];
            for (int i = 0; i < targetTable.Rows.Count; i++)
            {
                var row = targetTable.Rows[i];
                viewsTarget[i] = new
                {
                    id = Convert.ToInt32(row["ID"]),
                    name = row["NAME"]?.ToString()
                };
            }

            var initData = JsonSerializer.Serialize(new
            {
                theme = _isDarkTheme ? "dark" : "light",
                viewsExist,
                viewsTarget,
                currentSettings = new
                {
                    viewTypeOpt = _entDtViewSheet.ViewTypeOpt,
                    blankTop = _entDtViewSheet.BlankTop,
                    blankBottom = _entDtViewSheet.BlankBottom,
                    blankLeft = _entDtViewSheet.BlankLeft,
                    blankRight = _entDtViewSheet.BlankRight
                }
            });

            await webView.CoreWebView2.ExecuteScriptAsync($"window.initSettings({initData})");
        }

        private void OnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string json = e.WebMessageAsJson;
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                string action = root.GetProperty("action").GetString();

                if (action == "ok")
                {
                    int viewTypeOpt = root.GetProperty("viewTypeOpt").GetInt32();
                    int blankTop = root.GetProperty("blankTop").GetInt32();
                    int blankBottom = root.GetProperty("blankBottom").GetInt32();
                    int blankLeft = root.GetProperty("blankLeft").GetInt32();
                    int blankRight = root.GetProperty("blankRight").GetInt32();

                    _entDtViewSheet.ViewTypeOpt = viewTypeOpt;
                    _entDtViewSheet.BlankTop = blankTop;
                    _entDtViewSheet.BlankBottom = blankBottom;
                    _entDtViewSheet.BlankLeft = blankLeft;
                    _entDtViewSheet.BlankRight = blankRight;

                    _entDtViewSheet.DataViewTarget.Rows.Clear();
                    var targetViews = root.GetProperty("targetViews");
                    foreach (var item in targetViews.EnumerateArray())
                    {
                        var row = _entDtViewSheet.DataViewTarget.NewRow();
                        row["ID"] = item.GetProperty("id").GetInt32();
                        row["NAME"] = item.GetProperty("name").GetString();
                        _entDtViewSheet.DataViewTarget.Rows.Add(row);
                    }

                    _entDtCmd.Data[0] = viewTypeOpt.ToString();
                    _entDtCmd.Data[1] = blankTop.ToString();
                    _entDtCmd.Data[2] = blankBottom.ToString();
                    _entDtCmd.Data[3] = blankLeft.ToString();
                    _entDtCmd.Data[4] = blankRight.ToString();

                    DialogResult = true;
                }
                else if (action == "cancel")
                {
                    DialogResult = false;
                }
            }
            catch
            {
                DialogResult = false;
            }
        }
    }
}

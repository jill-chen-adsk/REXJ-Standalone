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

namespace ADSK.JExtRAC.FittingSchedule.CreateAndEdit
{
    public partial class CreateViewWindow : Window
    {
        private readonly RvtExtApp.Components.Attribute _cmpAttribute;
        private readonly RvtExtApp.Entities.DtView _entDtView;
        private readonly RvtExtApp.Entities.DtWinDoorType _entDtWinDoorType;
        private readonly RvtExtApp.Entities.DtCmd _entDtCmd;
        private readonly bool _isDarkTheme;

        public CreateViewWindow(
            RvtExtApp.Components.Attribute cmpAttribute,
            RvtExtApp.Entities.DtView entDtView,
            RvtExtApp.Entities.DtWinDoorType entDtWinDoorType,
            RvtExtApp.Entities.DtCmd entDtCmd)
        {
            InitializeComponent();
            _cmpAttribute = cmpAttribute;
            _entDtView = entDtView;
            _entDtWinDoorType = entDtWinDoorType;
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
            var resourceName = "ADSK.JExtRAC.FittingSchedule.CreateAndEdit.CreateViewDialog.html";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private async Task SendInitialDataAsync()
        {
            var doorTagTable = _entDtWinDoorType.DataDoorTags;
            var windowTagTable = _entDtWinDoorType.DataWindowTags;

            var doorTags = DataTableToJson(doorTagTable,
                doorTagTable.Columns[0].ColumnName,
                doorTagTable.Columns[1].ColumnName);
            var windowTags = DataTableToJson(windowTagTable,
                windowTagTable.Columns[0].ColumnName,
                windowTagTable.Columns[1].ColumnName);
            var scales = DataTableToJson(_entDtView.DataScale, "Value", "Name");
            var detailLevels = DataTableToJson(_entDtView.DetailLevel, "Value", "Name");

            var initData = JsonSerializer.Serialize(new
            {
                theme = _isDarkTheme ? "dark" : "light",
                doorTags,
                windowTags,
                scales,
                detailLevels,
                currentSettings = new
                {
                    doorTagId = _entDtWinDoorType.IdDoorTag,
                    windowTagId = _entDtWinDoorType.IdWindowTag,
                    scale = _entDtView.ViewScaleDefault,
                    detailLevel = _entDtView.ViewDetailLevel,
                    duplicateHandling = _entDtView.DuplicateViewOpt,
                    scaleCustom = _entDtView.ViewScaleCustom
                }
            });

            await webView.CoreWebView2.ExecuteScriptAsync($"window.initSettings({initData})");
        }

        private static object[] DataTableToJson(DataTable table, string valueColumn, string nameColumn)
        {
            if (table is null || table.Rows.Count == 0)
                return Array.Empty<object>();

            var rows = new object[table.Rows.Count];
            for (int i = 0; i < table.Rows.Count; i++)
            {
                var row = table.Rows[i];
                rows[i] = new
                {
                    value = row[valueColumn],
                    name = row[nameColumn]?.ToString()
                };
            }
            return rows;
        }

        private void OnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string json = e.WebMessageAsJson;
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                string action = root.GetProperty("action").GetString();

                if (action == "create")
                {
                    int doorTagId = root.GetProperty("doorTagId").GetInt32();
                    int windowTagId = root.GetProperty("windowTagId").GetInt32();
                    int scale = root.GetProperty("scale").GetInt32();
                    int detailLevel = root.GetProperty("detailLevel").GetInt32();
                    int duplicateHandling = root.GetProperty("duplicateHandling").GetInt32();
                    int scaleCustom = root.GetProperty("scaleCustom").GetInt32();

                    // -999 is the marker for "user selected custom scale entry"
                    if (scale == -999)
                    {
                        scale = 0;
                    }

                    _entDtWinDoorType.IdDoorTag = doorTagId;
                    _entDtWinDoorType.IdWindowTag = windowTagId;
                    _entDtView.ViewScaleDefault = scale;
                    _entDtView.ViewDetailLevel = detailLevel;
                    _entDtView.DuplicateViewOpt = duplicateHandling;
                    _entDtView.ViewScaleCustom = scaleCustom;

                    _entDtCmd.Data[0] = doorTagId.ToString();
                    _entDtCmd.Data[1] = windowTagId.ToString();
                    _entDtCmd.Data[2] = duplicateHandling.ToString();
                    _entDtCmd.Data[3] = scale.ToString();
                    _entDtCmd.Data[4] = scaleCustom.ToString();
                    _entDtCmd.Data[5] = detailLevel.ToString();

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

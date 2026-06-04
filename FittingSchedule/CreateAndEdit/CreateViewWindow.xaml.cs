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

        private static double? _savedLeft;
        private static double? _savedTop;
        private static readonly string _posFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ADSK", "JExtRAC", "FittingSchedule", "createview_pos.txt");

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
            RestorePosition();

            Loaded += OnWindowLoaded;
            Closing += OnWindowClosing;
        }

        private void RestorePosition()
        {
            if (_savedLeft.HasValue && _savedTop.HasValue &&
                IsOnScreen(_savedLeft.Value, _savedTop.Value))
            {
                Left = _savedLeft.Value;
                Top = _savedTop.Value;
                WindowStartupLocation = WindowStartupLocation.Manual;
                return;
            }

            try
            {
                if (File.Exists(_posFile))
                {
                    var parts = File.ReadAllText(_posFile).Split(',');
                    if (parts.Length == 2 &&
                        double.TryParse(parts[0], out double l) &&
                        double.TryParse(parts[1], out double t) &&
                        IsOnScreen(l, t))
                    {
                        Left = l;
                        Top = t;
                        _savedLeft = l;
                        _savedTop = t;
                        WindowStartupLocation = WindowStartupLocation.Manual;
                        return;
                    }
                }
            }
            catch { }

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private static bool IsOnScreen(double left, double top)
        {
            var virtualWidth = SystemParameters.VirtualScreenWidth;
            var virtualHeight = SystemParameters.VirtualScreenHeight;
            var virtualLeft = SystemParameters.VirtualScreenLeft;
            var virtualTop = SystemParameters.VirtualScreenTop;

            return left >= virtualLeft - 100 &&
                   top >= virtualTop - 100 &&
                   left < virtualLeft + virtualWidth &&
                   top < virtualTop + virtualHeight;
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _savedLeft = Left;
            _savedTop = Top;
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_posFile));
                File.WriteAllText(_posFile, Left + "," + Top);
            }
            catch { }
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
            Activate();

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
                isImperial = _entDtView.IsImperial,
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
                },
                labels = new
                {
                    doorTag = _cmpAttribute.ResourceText("IDS_TXT_DOORTAG"),
                    windowTag = _cmpAttribute.ResourceText("IDS_TXT_WINDOWTAG"),
                    scale = _cmpAttribute.ResourceText("IDS_TXT_SCALE"),
                    detailLevel = _cmpAttribute.ResourceText("IDS_TXT_DETAILLEVEL"),
                    handlingDuplicate = _cmpAttribute.ResourceText("IDS_TXT_HANDLINGDUPLICATEVIEW"),
                    deleteOldView = _cmpAttribute.ResourceText("IDS_TXT_DELOLDVIEW"),
                    keepExisting = _cmpAttribute.ResourceText("IDS_TXT_NOTUPDATE"),
                    renameOldView = _cmpAttribute.ResourceText("IDS_TXT_CHANGEOLDVIEW"),
                    customScale = _cmpAttribute.ResourceText("IDS_TXT_CUSTOM"),
                    scalePrefix = _cmpAttribute.ResourceText("IDS_TXT_COLON1"),
                    cancel = _cmpAttribute.ResourceText("IDS_TXT_CANCEL"),
                    createView = _cmpAttribute.ResourceText("IDS_TXT_CREATEVIEWPARTS"),
                    ok = _cmpAttribute.ResourceText("IDS_TXT_OK")
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

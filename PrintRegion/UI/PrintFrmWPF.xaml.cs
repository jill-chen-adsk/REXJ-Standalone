using ADSK.JExtRAC.PrintRegion.Commands;
using ADSK.JExtRAC.PrintRegion.Components;
using ADSK.JExtRAC.PrintRegion.Request;
using ADSK.JExtRAC.PrintRegion.Utils;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using RvtExtApp = ADSK.JExtRAC.PrintRegion;

namespace ADSK.JExtRAC.PrintRegion.UI
{
    public partial class PrintFrmWPF : Window, IWeaveChromeWindow
    {
        private readonly Elements _CmpElements;
        private readonly RvtExtApp.Components.Attribute _CmpAttribute;
        private readonly PrintMgr m_printMgr;
        private bool m_isShowing;
        private bool _isOkOrCancel;
        private bool _isImperial;

        public PrintFrmWPF(
            RvtExtApp.Components.Attribute cmpAttribute,
            Elements cmpElements,
            PrintMgr pMgr)
        {
            InitializeComponent();

            _CmpAttribute = cmpAttribute;
            _CmpElements = cmpElements;
            m_printMgr = pMgr;

            WeaveTheme.Apply(this, this, cmpAttribute.ResourceText("IDS_TXT_PRINT_REGION"), CancelDialog);

            cbScale.LostFocus += CbScale_LostFocus;
            cbScale.PreviewTextInput += CbScale_PreviewTextInput;
            DataObject.AddPastingHandler(cbScale, CbScale_Pasting);

            Loaded += PrintFrmWPF_Loaded;
            Closed += PrintFrmWPF_Closed;

            cbPrintName.SelectionChanged += CbPrintName_SelectionChanged;
            cbScale.SelectionChanged += CbScale_SelectionChanged;
            btOK.Click += BtOK_Click;
            btPropetives.Click += BtPropetives_Click;
            btPreview.Click += BtPreview_Click;
            btCancel.Click += BtCancel_Click;
        }

        public Border ChromeOuterBorder => chromeOuterBorder;
        public Grid ChromeTitleBar => chromeTitleBar;
        public Border ChromeDivider => chromeDivider;
        public TextBlock ChromeTitleText => chromeTitleText;
        public Button ChromeCloseButton => chromeCloseButton;

        private void PrintFrmWPF_Loaded(object sender, RoutedEventArgs e)
        {
            m_isShowing = true;
            SetText();
            SetIcon();
            SetData();
            m_isShowing = false;
        }

        private void PrintFrmWPF_Closed(object sender, EventArgs e)
        {
            if (_isOkOrCancel)
                return;

            RequestHandler.Execute(RequestId.CANCEL);
        }

        private void SetText()
        {
            WeaveWindowChrome.SetTitle(this, this, _CmpAttribute.ResourceText("IDS_TXT_PRINT_REGION"));
            lblPrintName.Text = _CmpAttribute.ResourceText("IDS_TXT_PRINTER_NAME");
            btPropetives.Content = _CmpAttribute.ResourceText("IDS_TXT_PROPERTIES");
            lblScale.Text = _CmpAttribute.ResourceText("IDS_TXT_PRINT_SCALE");
            btPreview.Content = _CmpAttribute.ResourceText("IDS_TXT_PREVIEW");
            btOK.Content = _CmpAttribute.ResourceText("IDS_TXT_OK");
            btCancel.Content = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");
        }

        private void SetIcon()
        {
            if (_CmpAttribute.ResourceImage("IDI_SUBS_ICON") is not Icon icon)
                return;

            Icon = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        private void SetData()
        {
            try
            {
                PrinterSettings.StringCollection printers = PrinterSettings.InstalledPrinters;
                string[] printerNames = new string[printers.Count];
                printers.CopyTo(printerNames, 0);

                cbPrintName.ItemsSource = printerNames.OrderBy(name => name, StringComparer.CurrentCultureIgnoreCase).ToArray();

                int printerIndex = cbPrintName.Items.IndexOf(m_printMgr.PrinterName);
                if (printerIndex >= 0)
                    cbPrintName.SelectedIndex = printerIndex;
            }
            catch (Exception)
            {
            }

            DataTable dataScale = _CmpElements.GetDataScale();
            _isImperial = ViewScaleHelper.IsImperial(_CmpElements.RvtDBDoc);
            int index = 0;
            bool found = false;
            for (int i = 0; i < dataScale.Rows.Count; i++)
            {
                DataRow row = dataScale.Rows[i];
                if ((int)row["Value"] == CmdPrint._entData._viewScale)
                {
                    found = true;
                    index = i;
                    break;
                }
            }

            cbScale.ItemsSource = dataScale.DefaultView;
            cbScale.DisplayMemberPath = "Name";
            cbScale.SelectedValuePath = "Value";

            if (found)
            {
                cbScale.SelectedIndex = index;
            }
            else
            {
                DataRow row = dataScale.NewRow();
                row["Name"] = ViewScaleHelper.FormatScaleDisplay(
                    _CmpElements.RvtDBDoc,
                    CmdPrint._entData._viewDuplicate,
                    CmdPrint._entData._viewScale,
                    _isImperial);
                row["Value"] = CmdPrint._entData._viewDuplicate.Scale;
                dataScale.Rows.InsertAt(row, 0);
                cbScale.SelectedIndex = 0;
            }
        }

        private bool GetDataAndCheckError()
        {
            ClearScaleError();

            if (cbScale.SelectedValue != null)
            {
                CmdPrint._entData._viewScale = (int)cbScale.SelectedValue;
                return false;
            }

            string text = cbScale.Text?.Trim() ?? string.Empty;
            if (ViewScaleHelper.TryParseScale(text, _isImperial, out int scale))
            {
                CmdPrint._entData._viewScale = scale;
                return false;
            }

            ShowScaleError(_CmpAttribute.ResourceText(
                _isImperial ? "IDS_ERROR_FORMAT_SCALE_IMPERIAL" : "IDS_ERROR_FORMAT_SCALE"));
            return true;
        }

        private void ShowScaleError(string message)
        {
            scaleErrorText.Text = message;
            scaleErrorText.Visibility = Visibility.Visible;
        }

        private void ClearScaleError()
        {
            scaleErrorText.Text = string.Empty;
            scaleErrorText.Visibility = Visibility.Collapsed;
        }

        private void CbPrintName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbPrintName.SelectedItem is string printerName)
                m_printMgr.PrinterName = printerName;
        }

        private void BtOK_Click(object sender, RoutedEventArgs e)
        {
            if (GetDataAndCheckError())
                return;

            _isOkOrCancel = true;
            RequestHandler.Execute(RequestId.OK);
        }

        private void BtPropetives_Click(object sender, RoutedEventArgs e)
        {
            RequestHandler.Execute(RequestId.CHANGESETUP);
        }

        private void CbScale_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Text))
                return;

            if (!ViewScaleHelper.AllowsScaleInputChar(e.Text[0], cbScale.Text, _isImperial))
                e.Handled = true;
        }

        private void CbScale_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string pasteText = e.DataObject.GetData(typeof(string)) as string ?? string.Empty;
                if (pasteText.Any(ch => !ViewScaleHelper.AllowsScaleInputChar(ch, cbScale.Text, _isImperial)))
                    e.CancelCommand();
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void CbScale_LostFocus(object sender, RoutedEventArgs e)
        {
            GetDataAndCheckError();
        }

        private void CbScale_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!m_isShowing)
                GetDataAndCheckError();
        }

        private void BtPreview_Click(object sender, RoutedEventArgs e)
        {
            if (GetDataAndCheckError())
                return;

            RequestHandler.Execute(RequestId.PREVIEW);
        }

        private void BtCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelDialog();
        }

        private void CancelDialog()
        {
            _isOkOrCancel = true;
            RequestHandler.Execute(RequestId.CANCEL);
            Close();
        }
    }
}

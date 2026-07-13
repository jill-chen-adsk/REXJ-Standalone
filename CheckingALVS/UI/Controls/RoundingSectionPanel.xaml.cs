using System;
using System.Windows;
using System.Windows.Controls;

namespace ADSK.JExtRAC.CheckingALVS.UI.Controls
{
    public partial class RoundingSectionPanel : System.Windows.Controls.UserControl
    {
        string _radioGroupName = Guid.NewGuid().ToString("N");

        public RoundingSectionPanel()
        {
            InitializeComponent();
            defaultButton.Click += (_, __) => DefaultClick?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler DefaultClick;

        public int DefaultInitFlag { get; set; }

        public void ConfigureRadioGroup(string groupName)
        {
            _radioGroupName = groupName ?? _radioGroupName;
            cutRadio.GroupName = _radioGroupName;
            closeRadio.GroupName = _radioGroupName;
            roundingRadio.GroupName = _radioGroupName;
        }

        public void SetSectionTitle(string title) => sectionTitleText.Text = title ?? string.Empty;

        public void SetDecimalLabel(string label) => decimalLabelText.Text = label ?? string.Empty;

        public void SetOrderLabel(string label) => orderLabelText.Text = label ?? string.Empty;

        public void SetCutText(string text) => cutRadio.Content = text ?? string.Empty;

        public void SetCloseText(string text) => closeRadio.Content = text ?? string.Empty;

        public void SetRoundingText(string text) => roundingRadio.Content = text ?? string.Empty;

        public void SetDefaultButtonText(string text)
        {
            defaultButton.Content = text ?? "Default";
        }

        public string DecimalText
        {
            get => decimalTextBox.Text;
            set => decimalTextBox.Text = value ?? string.Empty;
        }

        public int RoundingOption
        {
            get
            {
                if (cutRadio.IsChecked == true)
                    return 0;
                if (closeRadio.IsChecked == true)
                    return 1;
                return 2;
            }
            set
            {
                switch (value)
                {
                    case 0:
                        cutRadio.IsChecked = true;
                        break;
                    case 1:
                        closeRadio.IsChecked = true;
                        break;
                    default:
                        roundingRadio.IsChecked = true;
                        break;
                }
            }
        }

        public void SetError(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                errorTextBlock.Text = string.Empty;
                errorTextBlock.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                errorTextBlock.Text = message;
                errorTextBlock.Visibility = System.Windows.Visibility.Visible;
            }
        }

        public string ValidateNumeric(Func<string, string> validator)
        {
            string message = validator?.Invoke(DecimalText.Trim()) ?? string.Empty;
            SetError(message);
            return message;
        }

        void DecimalTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            LostFocusValidation?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler LostFocusValidation;
    }
}

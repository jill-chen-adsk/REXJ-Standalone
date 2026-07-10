using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace ADSK.JExtRAC.AreaSchedule.Utils
{
    public partial class WeaveProgressWindow : Window, IWeaveChromeWindow
    {
        private IntPtr _ownerHandle = IntPtr.Zero;

        public WeaveProgressWindow()
        {
            InitializeComponent();
            WeaveTheme.Apply(this, this, Title, showCloseButton: false);
        }

        public Border ChromeOuterBorder => chromeOuterBorder;
        public Grid ChromeTitleBar => chromeTitleBar;
        public Border ChromeDivider => chromeDivider;
        public TextBlock ChromeTitleText => chromeTitleText;
        public Button ChromeCloseButton => null;

        public void SetOwnerHandle(IntPtr ownerHandle)
        {
            _ownerHandle = ownerHandle;
            if (_ownerHandle != IntPtr.Zero)
                new WindowInteropHelper(this) { Owner = _ownerHandle };
        }

        public void SetCommandTitle(string title)
        {
            WeaveWindowChrome.SetTitle(this, this, title);
        }

        public void SetMessage(string message)
        {
            MessageText.Text = message ?? string.Empty;
        }

        public void SetProgress(int maximum, int value)
        {
            int safeMaximum = Math.Max(maximum, 1);
            int safeValue = Math.Clamp(value, 0, safeMaximum);

            OperationProgress.Maximum = safeMaximum;
            OperationProgress.Value = safeValue;

            int percent = safeValue * 100 / safeMaximum;
            StatusText.Text = $"{safeValue} / {safeMaximum} ({percent}%)";
        }
    }
}

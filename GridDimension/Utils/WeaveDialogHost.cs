using System;
using System.Windows;
using System.Windows.Interop;

namespace ADSK.JExtRAC.GridDimension.Utils
{
    public static class WeaveDialogHost
    {
        public static IntPtr RevitWindowHandle =>
            System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

        public static void SetOwner(Window window, IntPtr ownerHandle)
        {
            if (window == null || ownerHandle == IntPtr.Zero)
                return;

            new WindowInteropHelper(window) { Owner = ownerHandle };
        }

        public static bool? ShowDialog(Window dialog, IntPtr ownerHandle = default)
        {
            if (dialog == null)
                return false;

            WeaveTheme.Apply(dialog);
            SetOwner(dialog, ownerHandle);

            return dialog.ShowDialog();
        }

        public static void ShowMessage(IntPtr ownerHandle, string message, string title, string okText = "OK")
        {
            var dialog = new WeaveMessageDialog(message, title, okText);
            SetOwner(dialog, ownerHandle);
            dialog.ShowDialog();
        }
    }
}

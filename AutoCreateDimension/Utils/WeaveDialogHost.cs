using System;
using System.Windows;
using System.Windows.Interop;

namespace ADSK.JExtRAC.AutoCreateDimension.Utils
{
    public static class WeaveDialogHost
    {
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

            if (ownerHandle == IntPtr.Zero)
                dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            SetOwner(dialog, ownerHandle);

            return dialog.ShowDialog();
        }

        public static void ShowMessage(IntPtr ownerHandle, string message, string title, string okText = null)
        {
            var dialog = new WeaveMessageDialog(message, title, okText);
            if (ownerHandle == IntPtr.Zero)
                dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            SetOwner(dialog, ownerHandle);
            dialog.ShowDialog();
        }
    }
}

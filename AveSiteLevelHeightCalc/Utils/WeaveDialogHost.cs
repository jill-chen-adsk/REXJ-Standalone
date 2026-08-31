using System;
using System.Windows;
using System.Windows.Interop;

namespace ADSK.JExtRAC.AveSiteLevelHeightCalc.Utils
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

            WeaveTheme.Apply(dialog);
            if (ownerHandle != IntPtr.Zero)
                SetOwner(dialog, ownerHandle);

            return dialog.ShowDialog();
        }
    }
}

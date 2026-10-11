using System;
using System.Windows;
using System.Windows.Interop;

namespace ADSK.JExtRAC.JoinOrderInspector.Utils
{
    public static class WeaveDialogHost
    {
        public static void SetOwner(Window window, IntPtr ownerHandle)
        {
            if (window == null || ownerHandle == IntPtr.Zero)
                return;

            new WindowInteropHelper(window) { Owner = ownerHandle };
        }

        public static void Show(Window window, IntPtr ownerHandle = default)
        {
            if (window == null)
                return;

            WeaveTheme.Apply(window);
            SetOwner(window, ownerHandle);
            window.Show();
        }
    }
}

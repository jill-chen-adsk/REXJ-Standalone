using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace ADSK.JExtRAC.ParameterFilter.Utils
{
    sealed class WinFormsWindowWrapper : System.Windows.Forms.IWin32Window
    {
        public WinFormsWindowWrapper(IntPtr handle) => Handle = handle;

        public IntPtr Handle { get; }
    }

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
            if (ownerHandle != IntPtr.Zero)
                SetOwner(dialog, ownerHandle);

            return dialog.ShowDialog();
        }

        public static bool? ShowDialog(Window dialog, Window ownerWindow, IntPtr fallbackOwnerHandle = default)
        {
            if (dialog == null)
                return false;

            WeaveTheme.Apply(dialog);
            if (ownerWindow != null)
            {
                dialog.Owner = ownerWindow;
                dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else if (fallbackOwnerHandle != IntPtr.Zero)
                SetOwner(dialog, fallbackOwnerHandle);

            return dialog.ShowDialog();
        }

        public static DialogResult ShowWinFormsDialog(Form form, Window owner = null)
        {
            if (form == null)
                return DialogResult.Cancel;

            if (owner != null)
            {
                IntPtr ownerHandle = new WindowInteropHelper(owner).Handle;
                return ShowWinFormsDialog(form, ownerHandle);
            }

            return form.ShowDialog();
        }

        public static DialogResult ShowWinFormsDialog(Form form, IntPtr ownerHandle)
        {
            if (form == null)
                return DialogResult.Cancel;

            if (ownerHandle != IntPtr.Zero)
                return form.ShowDialog(new WinFormsWindowWrapper(ownerHandle));

            return form.ShowDialog();
        }

        public static void ShowMessage(IntPtr ownerHandle, string message, string title, string okText = "OK")
        {
            var dialog = new WeaveMessageDialog(message, title, okText);
            SetOwner(dialog, ownerHandle);
            dialog.ShowDialog();
        }
    }
}

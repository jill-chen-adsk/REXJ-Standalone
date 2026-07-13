using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace ADSK.JExtRAC.CheckingALVS.Utils
{
    sealed class WinFormsWindowWrapper : System.Windows.Forms.IWin32Window
    {
        public WinFormsWindowWrapper(IntPtr handle) => Handle = handle;

        public IntPtr Handle { get; }
    }

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

        public static DialogResult ShowWinFormsDialog(Form form, Window owner = null)
        {
            if (form == null)
                return DialogResult.Cancel;

            if (owner != null)
            {
                IntPtr ownerHandle = new WindowInteropHelper(owner).Handle;
                if (ownerHandle != IntPtr.Zero)
                    return form.ShowDialog(new WinFormsWindowWrapper(ownerHandle));
            }

            return form.ShowDialog();
        }
    }
}

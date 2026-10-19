using ADSK.JExtRAC.AutoCreateDimension.Utils;
using System;

namespace ADSK.JExtRAC.AutoCreateDimension.Common
{
    /// ================================================================================
    /// <summary>Weave-compliant message dialogs</summary>
    /// ================================================================================
    public static class ComDialog
    {
        public static void ShowDialog(IntPtr ownerHandle, string title, string message, string okText = null)
        {
            WeaveDialogHost.ShowMessage(ownerHandle, message, title, okText);
        }
    }
}

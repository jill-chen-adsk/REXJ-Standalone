using System;
using System.Windows.Forms;

namespace ADSK.JExtRAC.PrintRegion.Request
{
    internal class JtWindowHandle : IWin32Window
    {
        private IntPtr _hwnd;

        /// <summary>
        /// Set parten of form is revit, when revit minimum in taskbar this form will minimum too
        /// </summary>
        /// <param name="h"></param>
        public JtWindowHandle(IntPtr h)
        {
            System.Diagnostics.Debug.Assert(IntPtr.Zero != h,
              "expected non-null window handle");

            _hwnd = h;
        }

        public IntPtr Handle
        {
            get
            {
                return _hwnd;
            }
        }
    }
}

using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace ADSK.ViewExtension.ViewDuplicate.Utils
{
    /// <summary>
    /// Positions dialogs over their (possibly native) owner window, optionally restoring the
    /// last saved position. WPF's CenterOwner is unreliable for SizeToContent windows owned
    /// by a native HWND such as Revit's main window.
    /// </summary>
    public static class WindowPositionMemory
    {
        private const string KeyLeft = "WindowLeft";
        private const string KeyTop = "WindowTop";
        private const uint MonitorDefaultToNull = 0;

        [StructLayout(LayoutKind.Sequential)]
        private struct NativeRect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NativePoint
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out NativeRect rect);

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromPoint(NativePoint pt, uint flags);

        public static void Attach(Window window, string settingsSection)
        {
            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.Loaded += (s, e) => Restore(window, settingsSection);
            window.Closing += (s, e) => Save(window, settingsSection);
        }

        /// <summary>Centers the window on its owner once its size is known, without remembering the position.</summary>
        public static void CenterOnOwnerWhenLoaded(Window window)
        {
            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.Loaded += (s, e) => CenterOnOwner(window);
        }

        private static void Restore(Window window, string section)
        {
            if (TryGetSaved(section, out double left, out double top) && IsTitleBarOnScreen(window, left, top))
            {
                window.Left = left;
                window.Top = top;
                return;
            }

            CenterOnOwner(window);
        }

        private static void Save(Window window, string section)
        {
            if (window.WindowState != WindowState.Normal || double.IsNaN(window.Left) || double.IsNaN(window.Top))
                return;

            DialogUtil.SaveSetting(section, KeyLeft, window.Left.ToString(CultureInfo.InvariantCulture));
            DialogUtil.SaveSetting(section, KeyTop, window.Top.ToString(CultureInfo.InvariantCulture));
        }

        private static bool TryGetSaved(string section, out double left, out double top)
        {
            top = 0;
            return double.TryParse(DialogUtil.GetSetting(section, KeyLeft, string.Empty), NumberStyles.Float, CultureInfo.InvariantCulture, out left)
                && double.TryParse(DialogUtil.GetSetting(section, KeyTop, string.Empty), NumberStyles.Float, CultureInfo.InvariantCulture, out top);
        }

        private static bool IsTitleBarOnScreen(Window window, double left, double top)
        {
            Matrix toDevice = GetTransformToDevice(window);
            Point probe = toDevice.Transform(new Point(left + Math.Min(60, window.ActualWidth / 2), top + 16));
            var pt = new NativePoint { X = (int)probe.X, Y = (int)probe.Y };
            return MonitorFromPoint(pt, MonitorDefaultToNull) != IntPtr.Zero;
        }

        public static void CenterOnOwner(Window window)
        {
            Rect ownerRect;
            if (window.Owner != null)
            {
                ownerRect = new Rect(window.Owner.Left, window.Owner.Top, window.Owner.ActualWidth, window.Owner.ActualHeight);
            }
            else
            {
                IntPtr ownerHandle = new WindowInteropHelper(window).Owner;
                if (ownerHandle != IntPtr.Zero && GetWindowRect(ownerHandle, out NativeRect native))
                {
                    Matrix fromDevice = GetTransformToDevice(window);
                    fromDevice.Invert();
                    Point topLeft = fromDevice.Transform(new Point(native.Left, native.Top));
                    Point bottomRight = fromDevice.Transform(new Point(native.Right, native.Bottom));
                    ownerRect = new Rect(topLeft, bottomRight);
                }
                else
                {
                    ownerRect = SystemParameters.WorkArea;
                }
            }

            window.Left = ownerRect.Left + (ownerRect.Width - window.ActualWidth) / 2;
            window.Top = ownerRect.Top + (ownerRect.Height - window.ActualHeight) / 2;
        }

        private static Matrix GetTransformToDevice(Window window) =>
            PresentationSource.FromVisual(window)?.CompositionTarget?.TransformToDevice ?? Matrix.Identity;
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using RvtExtApp = ADSK.JExtRAC.ExportSchedule;

namespace ADSK.JExtRAC.ExportSchedule.UserControls
{
    /// <summary>
    /// Custom SaveFileDialog
    /// </summary>
    public partial class CustomSaveFileDialog : UserControl
    {
        // メンバ変数

        #region Member Variables

        private const uint WINEVENT_OUTOFCONTEXT = 0;

        // Save As dialog handle
        public IntPtr _HDlg;

        // Event hook
        private IntPtr _HHook;

        // App. specific user control handle
        private IntPtr _HCtrl;

        // App. specific user control
        private UserControl _Ctrl;

        //Static variable containing the instance object
        private static CustomSaveFileDialog _CustomSaveFileDialog;

        public static bool _SetFileName = false;

        public SaveFileDialog _SaveFileDialog
        { get { return saveFileDialog; } }

        //属性
        private static RvtExtApp.Components.Attribute _CmpAttribute = null;

        #endregion Member Variables

        // コンストラクタ

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctrl">The User Control to be displayed in the file dialog</param>
        public CustomSaveFileDialog(RvtExtApp.Components.Attribute cmpAttribute, UserControl ctrl)
        {
            InitializeComponent();
            _CmpAttribute = cmpAttribute;

            _CustomSaveFileDialog = this;
            this._Ctrl = ctrl;
            _HCtrl = ctrl.Handle;

            //Setup Hook; for simplicity, hook all possible events from the current process
            _HHook = SetWinEventHook(1, 0x7fffffff, IntPtr.Zero,
                    procDelegate, (uint)Process.GetCurrentProcess().Id, 0, WINEVENT_OUTOFCONTEXT);
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// <summary>
        /// Window event delegate
        /// </summary>
        private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType,
    IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        /// <summary>
        /// Sets an event hook function for a range of events.
        /// </summary>
        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr
           hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess,
           uint idThread, uint dwFlags);

        /// <summary>
        /// Changes the position and dimensions of the specified window.
        /// For a top-level window, the position and dimensions are relative to the upper-left corner of the screen.
        /// </summary>
        [DllImport("user32.dll")]
        private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int w, int h, bool repaint);

        /// <summary>
        /// The RECT structure defines the coordinates of the upper-left and lower-right corners of a rectangle.
        /// </summary>
        private struct RECT
        { public int Left; public int Top; public int Right; public int Bottom; }

        /// <summary>
        /// Retrieves the coordinates of a window's client area.
        /// </summary>
        [DllImport("user32.dll")]
        private static extern bool GetClientRect(IntPtr hWnd, out RECT rc);

        /// <summary>
        /// Changes the position and dimensions of the specified window.
        /// For a top-level window, the position and dimensions are relative to the upper-left corner of the screen.
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        /// <summary>
        /// Changes the parent window of the specified child window.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hwndChild, IntPtr hwndNewParent);

        /// <summary>
        /// Retrieves a handle to the specified window's parent or owner.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetParent(IntPtr hWnd);

        //Event hook delegate
        private static WinEventDelegate procDelegate = new WinEventDelegate(WinEventProc);

        // Hook function
        private static void WinEventProc(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            CustomSaveFileDialog csfdg = _CustomSaveFileDialog;

            if (csfdg._HDlg == IntPtr.Zero)
                csfdg._HDlg = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "#32770", _CmpAttribute.ResourceText("IDS_SAVE_EXCEL"));

            if (hwnd == csfdg._HDlg)
            {
                IntPtr hParent = GetParent(csfdg._HCtrl);

                //this is done only once
                if (!(hParent == csfdg._HDlg))
                {
                    //Bind the user control to the Common Dialog
                    SetParent(csfdg._HCtrl, csfdg._HDlg);
                }

                RECT cliRect;
                GetClientRect(csfdg._HDlg, out cliRect);

                //Position the button in the file dialog
                MoveWindow(csfdg._HCtrl, cliRect.Left + 120, cliRect.Bottom - 55, 500, 60, true);
            }
        }

        #endregion Member Functions
    }
}

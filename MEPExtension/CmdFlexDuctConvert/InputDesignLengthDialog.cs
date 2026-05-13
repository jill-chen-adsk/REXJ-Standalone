#region Namespaces
using System.Collections.Generic;
using System.Linq;
using System;
using System.Windows;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion

namespace CmdFlexDuctConvert
{
    /// <summary>
    /// 設計長さ入力ダイアログ
    /// </summary>
    public class InputDesignLengthDialog
    {
        private UIApplication uiApp { get; set; }
        private static InputDesignLengthDialog instance = new InputDesignLengthDialog();
        private FormDesignLength _dialogBox = null;
        public FormDesignLength dialogBox { get { return _dialogBox; } }
        public int designLength { get { return _dialogBox.designLength; } }
        public bool isVisible { get { return _dialogBox.IsVisible; } }

        /// <summary>
        /// </summary>
        public static InputDesignLengthDialog GetInstance()
        {
            return instance;
        }

        /// <summary>
        /// </summary>
        private InputDesignLengthDialog()
        {
        }

        /// <summary>
        /// </summary>
        public InputDesignLengthDialog(UIApplication uiapp)
        {
            uiApp = uiapp;
            _dialogBox = new FormDesignLength(uiApp);
        }

        /// <summary>
        /// </summary>
        public T FindWindowOfType<T>() where T : Window
        {
            return System.Windows.Application.Current.Windows.OfType<T>().FirstOrDefault();
        }

        static IntPtr getRevitWindowHandleIntPtr()
        {
            Process process = Process.GetCurrentProcess();
            IntPtr h = process.MainWindowHandle;
            return h;
        }

        /// <summary>
        /// アクティブUIViewの取得
        /// </summary>
        public static UIView getActiveUiView(UIDocument uidoc)
        {
            Document doc = uidoc.Document;
            View view = doc.ActiveView;
            IList<UIView> uiviews = uidoc.GetOpenUIViews();
            UIView uiview = null;

            foreach (UIView uv in uiviews)
            {
                if (uv.ViewId.Equals(view.Id))
                {
                    uiview = uv;
                    break;
                }
            }
            return uiview;
        }

        /// <summary>
        /// </summary>
        public void dlgShow()
        {
            if (!dlgVisible())
            {
                UIDocument uidoc = uiApp.ActiveUIDocument;
                var helper = new System.Windows.Interop.WindowInteropHelper(_dialogBox);
                helper.Owner = getRevitWindowHandleIntPtr();
                _dialogBox.Show();
            }
        }

        /// <summary>
        /// </summary>
        public void dlgHide()
        {
            if (dlgVisible())
            {
                _dialogBox.Hide();
            }
        }

        /// <summary>
        /// </summary>
        public void dlgClose()
        {
            if (_dialogBox != null)
            {
                _dialogBox.Close();
            }
        }

        /// <summary>
        /// </summary>
        public bool dlgVisible()
        {
            if (_dialogBox != null)
            {
                return _dialogBox.IsVisible;
            }
            return false;
        }

        /// <summary>
        /// </summary>
        private void dlgMoveToLeftTop(/*UIDocument uidoc*/)
        {
            if (_dialogBox != null)
            {
                UIDocument uidoc = uiApp.ActiveUIDocument;
                UIView uiview = getActiveUiView(uidoc);
                Rectangle rect = uiview.GetWindowRectangle();
                _dialogBox.Top = rect.Top;
                _dialogBox.Left = rect.Left;
            }
        }

        /// <summary>
        /// </summary>
        public Result Show(UIApplication uiapp)
        {
            uiApp = uiapp;
            if (_dialogBox == null)
            {
                _dialogBox = new FormDesignLength(uiApp);
                dlgShow();
                dlgMoveToLeftTop();
                _dialogBox.Activate();
            }
            else
            {
                if (!_dialogBox.IsVisible)
                {
                    dlgShow();
                    _dialogBox.Activate();
                }
            }

            return Result.Failed;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace ADSK.JExtRAC.ExportSchedule.UserControls
{
    /// <summary>
    /// Create custom user control to add to the File dialog
    /// </summary>
    public partial class MyUserControl : UserControl
    {
        public CustomSaveFileDialog _CustomSaveFileDialog = null;

        private string _DateTimeFormat = "yyyyMMddHHmmss";

        private Components.Attribute _cmpAttribute;

        // コンストラクタ

        #region Constructor

        public MyUserControl(Components.Attribute cmpAttribute)
        {
            _cmpAttribute = cmpAttribute;
            InitializeComponent();
            SetLocalizedText();
        }

        private void SetLocalizedText()
        {
            if (_cmpAttribute == null) return;
            chbAddDateTime.Text = _cmpAttribute.ResourceText("IDS_CHK_ADD_DATETIME");
            chbItemized.Text = _cmpAttribute.ResourceText("IDS_CHK_ITEMIZED");
            radForImport.Text = _cmpAttribute.ResourceText("IDS_RAD_FOR_IMPORT");
            radForSchedule.Text = _cmpAttribute.ResourceText("IDS_RAD_FOR_SCHEDULE");
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// <summary>
        /// Itemized
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Itemized
        {
            get
            {
                return chbItemized.Checked;
            }
            set
            {
                chbItemized.Checked = value;
            }
        }

        /// <summary>
        /// Export to excel file for import or display purposes
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ForImport
        {
            get
            {
                return radForImport.Checked;
            }
            set
            {
                radForImport.Checked = value;
            }
        }

        #endregion Member Functions

        public delegate bool Win32Callback(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.Dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr parentHandle, Win32Callback callback, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetClassName(IntPtr hWnd, System.Text.StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetWindowText(IntPtr hwnd, String lpString);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        /// <summary> Enum window</summary>
        /// <param name="handle">Handle</param>
        /// <param name="pointer">Pointer</param>
        /// <returns>True of False</returns>
        /// <history>2021/12/06 Created Applied Technology</history>
        /// ================================================================================
        private static bool EnumWindow(IntPtr handle, IntPtr pointer)
        {
            GCHandle gch = GCHandle.FromIntPtr(pointer);
            List<IntPtr> list = gch.Target as List<IntPtr>;
            if (list == null)
                throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
            list.Add(handle);

            Control c = Control.FromHandle(handle);

            if (c != null)
            {
            }

            return true;
        }

        /// <summary>Get child windows</summary>
        /// <param name="parent">Handle</param>
        /// <returns>List of child windows</returns>
        /// <history>2021/12/06 Created Applied Technology</history>
        /// ================================================================================
        public static List<IntPtr> GetChildWindows(IntPtr parent)
        {
            List<IntPtr> result = new List<IntPtr>();
            GCHandle listHandle = GCHandle.Alloc(result);
            try
            {
                Win32Callback childProc = new Win32Callback(EnumWindow);
                EnumChildWindows(parent, childProc, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }
            return result;
        }

        /// <summary>Get window class </summary>
        /// <param name="hwnd">Handle</param>
        /// <returns>Class name of handle</returns>
        /// <history>2021/12/06 Created Applied Technology</history>
        /// ================================================================================
        public static string GetWinClass(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
                return null;
            StringBuilder classname = new StringBuilder(100);
            IntPtr result = GetClassName(hwnd, classname, classname.Capacity);
            if (result != IntPtr.Zero)
                return classname.ToString();
            return null;
        }

        /// <summary>Get all childs window with class name</summary>
        /// <param name="hwnd">Handle</param>
        /// <param name="childClassName">Class name</param>
        /// <returns>List of child windows with class name</returns>
        /// <history>2021/12/06 Created Applied Technology</history>
        /// ================================================================================
        public static IEnumerable<IntPtr> EnumAllWindows(IntPtr hwnd, string childClassName)
        {
            List<IntPtr> children = GetChildWindows(hwnd);
            if (children == null)
                yield break;
            foreach (IntPtr child in children)
            {
                if (GetWinClass(child) == childClassName)
                    yield return child;
                foreach (var childchild in EnumAllWindows(child, childClassName))
                    yield return childchild;
            }
        }

        /// <summary>Event check changed </summary>
        /// <param name="sender">Checkbox</param>
        /// <param name="e">Event args</param>
        /// <history>2021/12/06 Created Applied Technology</history>
        /// ================================================================================
        private void chbAddDateTime_CheckedChanged(object sender, EventArgs e)
        {
            if (_CustomSaveFileDialog._SaveFileDialog == null)
                return;

            //Find all comboboxs
            var childs = EnumAllWindows(_CustomSaveFileDialog._HDlg, "ComboBox");
            if (childs.ToList().Count == 0)
                return;

            //First element is combobox FileName on SaveFileDialog
            foreach (IntPtr handle in childs)
            {
                try
                {
                    string fielName = string.Empty;

                    //Get curent file name
                    int chars = 256;
                    StringBuilder buff = new StringBuilder(chars);
                    if (GetWindowText(handle, buff, chars) > 0)
                    {
                        fielName = buff.ToString();
                    }
                    string extension = Path.GetExtension(fielName);
                    if (extension.Length != 0)
                    {
                        fielName = fielName.Replace(extension, "");
                    }
                    if (chbAddDateTime.Checked)
                    {
                        //Add date time
                        fielName += GetCurrentDateTime();
                    }
                    else
                    {
                        //Remove date time
                        int find_ = fielName.LastIndexOf('_');
                        if (find_ != -1)
                        {
                            string right = fielName.Substring(find_);
                            if (right.Length - 1 > _DateTimeFormat.Length)
                                return;

                            var regex = new System.Text.RegularExpressions.Regex("_[0-9]{4}(0[1-9]|1[0-2])(0[1-9]|[12][0-9]|3[01])([01][0-9]|2[0-3])[0-5][0-9][0-5][0-9]");
                            // Validate
                            bool IsMatch = regex.IsMatch(fielName);
                            if (IsMatch)
                            {
                                fielName = fielName.Substring(0, find_);
                            }
                        }
                    }

                    if (extension.Length != 0)
                    {
                        fielName += extension;
                    }
                    SetWindowText(handle, fielName);

                    break;
                }
                catch (Exception ex)
                {
                    string mess = ex.Message;
                }
            }
        }

        public string GetCurrentDateTime()
        {
            string value = string.Empty;
            try
            {
                DateTime dt = DateTime.Now;

                value = "_" + dt.ToString(_DateTimeFormat);
            }
            catch (Exception Ex)
            {
                string mess = Ex.Message;
            }

            return value;
        }
    }
}

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.CheckingALVS.Utils
{
    /// <summary>
    /// Applies Revit UI theme (light/dark) to WinForms dialogs.
    /// </summary>
    public static class RevitFormTheme
    {
        static readonly Color DarkBack = Color.FromArgb(38, 53, 69);
        static readonly Color DarkSurface = Color.FromArgb(46, 63, 80);
        static readonly Color DarkBorder = Color.FromArgb(58, 79, 99);
        static readonly Color DarkText = Color.FromArgb(224, 232, 240);
        static readonly Color Accent = Color.FromArgb(6, 150, 215);
        const int DwmUseImmersiveDarkMode = 20;

        [DllImport("dwmapi.dll")]
        static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        public static bool IsDarkTheme =>
            UIThemeManager.CurrentTheme == UITheme.Dark;

        public static void Apply(Form form)
        {
            if (form == null)
                return;

            ApplyTheme(form);
            ApplyNativeTitleBar(form);
            form.Load += (_, __) =>
            {
                ApplyTheme(form);
                ApplyNativeTitleBar(form);
            };
            form.HandleCreated += (_, __) => ApplyNativeTitleBar(form);
        }

        static void ApplyNativeTitleBar(Form form)
        {
            if (form == null || !form.IsHandleCreated)
                return;

            int useDark = IsDarkTheme ? 1 : 0;
            DwmSetWindowAttribute(form.Handle, DwmUseImmersiveDarkMode, ref useDark, sizeof(int));
        }

        static void ApplyTheme(Form form)
        {
            if (!IsDarkTheme)
                return;

            form.BackColor = DarkBack;
            form.ForeColor = DarkText;
            ApplyControls(form.Controls);
        }

        static void ApplyControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                switch (control)
                {
                    case GroupBox groupBox:
                        groupBox.BackColor = DarkBack;
                        groupBox.ForeColor = DarkText;
                        break;

                    case Label label:
                        label.BackColor = Color.Transparent;
                        label.ForeColor = DarkText;
                        break;

                    case System.Windows.Forms.TextBox textBox:
                        textBox.BackColor = DarkSurface;
                        textBox.ForeColor = DarkText;
                        textBox.BorderStyle = BorderStyle.FixedSingle;
                        break;

                    case System.Windows.Forms.ComboBox comboBox:
                        comboBox.BackColor = DarkSurface;
                        comboBox.ForeColor = DarkText;
                        comboBox.FlatStyle = FlatStyle.Flat;
                        break;

                    case RadioButton radioButton:
                        radioButton.BackColor = DarkBack;
                        radioButton.ForeColor = DarkText;
                        break;

                    case CheckBox checkBox:
                        checkBox.BackColor = DarkBack;
                        checkBox.ForeColor = DarkText;
                        break;

                    case Button button:
                        button.FlatStyle = FlatStyle.Flat;
                        button.FlatAppearance.BorderColor = DarkBorder;
                        button.BackColor = DarkSurface;
                        button.ForeColor = DarkText;
                        if (button.DialogResult == DialogResult.OK)
                        {
                            button.BackColor = Accent;
                            button.ForeColor = Color.White;
                            button.FlatAppearance.BorderColor = Accent;
                        }
                        break;

                    case DataGridView dataGridView:
                        ApplyDataGridView(dataGridView);
                        break;

                    case TreeView treeView:
                        treeView.BackColor = DarkSurface;
                        treeView.ForeColor = DarkText;
                        treeView.BorderStyle = BorderStyle.FixedSingle;
                        break;

                    case TabControl tabControl:
                        tabControl.BackColor = DarkBack;
                        tabControl.ForeColor = DarkText;
                        break;

                    case TabPage tabPage:
                        tabPage.BackColor = DarkBack;
                        tabPage.ForeColor = DarkText;
                        break;

                    case Panel panel:
                        panel.BackColor = DarkBack;
                        panel.ForeColor = DarkText;
                        break;

                    default:
                        control.BackColor = DarkBack;
                        control.ForeColor = DarkText;
                        break;
                }

                if (control.HasChildren)
                    ApplyControls(control.Controls);
            }
        }

        static void ApplyDataGridView(DataGridView dataGridView)
        {
            dataGridView.BackgroundColor = DarkSurface;
            dataGridView.GridColor = DarkBorder;
            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.DefaultCellStyle.BackColor = DarkSurface;
            dataGridView.DefaultCellStyle.ForeColor = DarkText;
            dataGridView.DefaultCellStyle.SelectionBackColor = Accent;
            dataGridView.DefaultCellStyle.SelectionForeColor = Color.White;
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = DarkBack;
            dataGridView.AlternatingRowsDefaultCellStyle.ForeColor = DarkText;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = DarkBack;
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = DarkText;
            dataGridView.RowHeadersDefaultCellStyle.BackColor = DarkBack;
            dataGridView.RowHeadersDefaultCellStyle.ForeColor = DarkText;
        }
    }
}

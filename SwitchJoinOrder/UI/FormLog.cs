using System;
using System.Reflection;
using System.Windows.Forms;
using RvtExtApp = ADSK.JExtRAC.SwitchJoinOrder;

namespace ADSK.JExtRAC.SwitchJoinOrder.UI
{
    public partial class FormLog : Form
    {
        private RvtExtApp.Components.Attribute _cmpAttribute;
        private System.Text.StringBuilder _strLog;

        public FormLog(RvtExtApp.Components.Attribute cmpAttribute, System.Text.StringBuilder strLog)
        {
            InitializeComponent();
            _cmpAttribute = cmpAttribute;
            _strLog = strLog;
            this.TopMost = true;
            SetText();
            SetData();
        }

        private void SetText()
        {
            this.Text = _cmpAttribute.ResourceText("IDS_TXT_LOG") + string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);
            this.Icon = _cmpAttribute.ResourceImage("IDI_SUBS_ICON") as System.Drawing.Icon;
            this.btnSave.Text = _cmpAttribute.ResourceText("IDS_TXT_SAVELOG");
            this.btnClose.Text = _cmpAttribute.ResourceText("IDS_TXT_CLOSELOG");
        }

        private void SetData() { RtxtLog.Text = _strLog.ToString(); }

        private bool SaveLog()
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Export log";
                saveFileDialog.DefaultExt = "txt";
                saveFileDialog.Filter = "Text Documents (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    RtxtLog.SaveFile(saveFileDialog.FileName, RichTextBoxStreamType.UnicodePlainText);
                    return true;
                }
                else return false;
            }
            catch (Exception ex)
            {
                Autodesk.Revit.UI.TaskDialog.Show(_cmpAttribute.ResourceText("IDS_TXT_ERROR"), ex.ToString());
                return false;
            }
        }

        private void btnClose_Click(object sender, EventArgs e) { this.Close(); }
        private void btnSave_Click(object sender, EventArgs e) { if (SaveLog()) this.Close(); }
    }
}

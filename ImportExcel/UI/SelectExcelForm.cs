using ADSK.JExtRAC.ImportExcel.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using RvtExtApp = ADSK.JExtRAC.ImportExcel;

namespace ADSK.JExtRAC.ImportExcel.UI
{
    public partial class SelectExcelForm : Form
    {
        private RvtExtApp.Components.Attribute _CmpAttribute = null;
        private List<ExcelInfo> _ExcelInfos = null;

        public SelectExcelForm(List<ExcelInfo> excelList, RvtExtApp.Components.Attribute cmpAttribute)
        {
            _CmpAttribute = cmpAttribute;
            _ExcelInfos = excelList;

            InitializeComponent();
            SetLocalizedText();

            this.Text += string.Format(" [Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);
        }

        private void SetLocalizedText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_FORM_TITLE");
            btnCancel.Text = _CmpAttribute.ResourceText("IDS_BTN_CANCEL");
        }

        private void DisplayData()
        {
            tvExcel.Nodes.Clear();

            foreach (ExcelInfo excel in _ExcelInfos)
            {
                var excelNode = tvExcel.Nodes.Add(excel._Workbook.Name);
                excelNode.Tag = excel;

                foreach (Excel.Worksheet sheet in excel._Worksheets)
                {
                    var sheetNode = excelNode.Nodes.Add(sheet.Name);
                    sheetNode.Tag = sheet;
                }
            }
            tvExcel.ExpandAll();
        }

        public Excel.Worksheet SheetSelected
        {
            get
            {
                if (tvExcel.SelectedNode == null)
                    return null;

                var workSheet = tvExcel.SelectedNode.Tag as Excel.Worksheet;
                return workSheet;
            }
        }

        private void SelectExcelForm_Load(object sender, EventArgs e)
        {
            DisplayData();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (SheetSelected == null)
            {
                MessageBox.Show(_CmpAttribute.ResourceText("IDS_SELECT_WORKSHEET_EXCEL"), _CmpAttribute.ResourceText("IDS_TXT_INFORMATION"));
                return;
            }

            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void tvExcel_DoubleClick(object sender, EventArgs e)
        {
            if (SheetSelected != null)
                DialogResult = DialogResult.OK;
        }
    }
}

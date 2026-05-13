using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.FloorHeightDimension;

namespace ADSK.JExtRAC.FloorHeightDimension.UI
{
    public partial class FormSelectView : Form
    {
        private RvtExtApp.Components.Attribute _CmpAttribute;

        private RvtExtApp.Entities.DtCmd _EntDtCmd;

        private List<Autodesk.Revit.DB.ViewSection> _ViewList;

        private Revit.DB.ViewSection _ViewSection;

        private int _LastIndex;

        public FormSelectView(RvtExtApp.Components.Attribute cmpAttribute,
                        RvtExtApp.Entities.DtCmd entDtCmd, List<Autodesk.Revit.DB.ViewSection> viewList, Revit.DB.ViewSection viewSection)
        {
            InitializeComponent();
            _CmpAttribute = cmpAttribute;

            _EntDtCmd = entDtCmd;

            _ViewList = viewList;

            _ViewSection = viewSection;

            SetText();
            SetData();
        }

        private void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECTVIEWS") + string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);

            this.btnOk.Text = _CmpAttribute.ResourceText("IDS_TXT_OK");
            this.btnCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");
            this.cbkSelecAll.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECTALLVIEW");

            this.Icon = _CmpAttribute.ResourceImage("IDI_SUBS_ICON") as System.Drawing.Icon;
        }

        private void SetData()
        {
            if (_ViewList == null || _ViewList.Count == 0)
                return;

            int index = 0;
            foreach (Revit.DB.ViewSection view in _ViewList)
            {
                this.cklView.Items.Add(new RvtExtApp.Entities.ViewItem()
                {
                    Tag = view,
                    Text = view.Title
                });

                if (view.Id == _ViewSection.Id)
                {
                    cklView.SetItemChecked(index, true);
                }
                index++;
            }

            if (_ViewList.Count == 1)
                cbkSelecAll.CheckState = CheckState.Checked;
            else
                cbkSelecAll.CheckState = CheckState.Indeterminate;
        }

        private void GetData()
        {
            _ViewList.Clear();

            foreach (RvtExtApp.Entities.ViewItem item in this.cklView.CheckedItems)
            {
                if (item.Tag.Id == _ViewSection.Id)
                    _ViewList.Insert(0, item.Tag as Revit.DB.ViewSection);
                else
                    _ViewList.Add(item.Tag as Revit.DB.ViewSection);
            }
            if (_ViewList.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_TXT_NOVIEWSELECT"), _CmpAttribute.ResourceText("IDS_TXT_ERROR"), System.Windows.Forms.MessageBoxButtons.OK);
                this.DialogResult = DialogResult.None;
            }
            else
                this.DialogResult = DialogResult.OK;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            GetData();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void cklView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Control.ModifierKeys == Keys.Shift)
            {
                int lastindex = _LastIndex;
                int currentindex = cklView.SelectedIndex;
                int upper = Math.Max(lastindex, currentindex);
                int lower = Math.Min(lastindex, currentindex);
                bool isChecked = cklView.GetItemChecked(currentindex);
                for (int i = lower; i <= upper; i++)
                {
                    cklView.SetItemCheckState(i, isChecked == true ? CheckState.Checked : CheckState.Unchecked);
                }
            }
            _LastIndex = cklView.SelectedIndex;

            int countItem = cklView.Items.Count;
            int checkedItem = cklView.CheckedItems.Count;
            if (checkedItem != 0 && checkedItem != countItem)
                cbkSelecAll.CheckState = CheckState.Indeterminate;
            else
            {
                if (checkedItem == countItem)
                    cbkSelecAll.CheckState = CheckState.Checked;
                else
                    cbkSelecAll.CheckState = CheckState.Unchecked;
            }
        }

        private void cbkSelecAll_CheckedChanged(object sender, EventArgs e)
        {
            if (cbkSelecAll.CheckState != CheckState.Indeterminate)
            {
                for (int i = 0; i < cklView.Items.Count; i++)
                {
                    cklView.SetItemCheckState(i, cbkSelecAll.Checked == true ? CheckState.Checked : CheckState.Unchecked);
                }
            }
        }
    }
}

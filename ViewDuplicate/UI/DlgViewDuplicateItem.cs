using ADSK.ViewExtension.ViewDuplicate.DialogItem;
using R = ADSK.ViewExtension.ViewDuplicate.Resources;
using static ADSK.ViewExtension.ViewDuplicate.Utils.DialogUtil;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using RevitView = Autodesk.Revit.DB.View;

namespace ADSK.ViewExtension.ViewDuplicate.UI
{
    public partial class DlgViewDuplicateItem : System.Windows.Forms.Form
    {
        private Document _dbDoc;
        private readonly ViewType _curViewType;
        private readonly List<RevitView> _lstViewTemplate;

        private string _preSuf;
        private string _prefix;
        private ItmViewFamilyType _viewFamilyTypeItem;
        private ItmView _viewTemplateItem;

        public DlgViewDuplicateItem(ComboBox cmbBox, ViewType currentViewType, List<RevitView> viewTemplates)
        {
            InitializeComponent();

            OK_Button.Click += OK_Button_Click;
            Cancel_Button.Click += Cancel_Button_Click;
            CbxViewFamilyType.SelectedIndexChanged += CbxViewFamilyType_SelectedIndexChanged;
            CbxViewTemplate.SelectedIndexChanged += CbxViewTemplate_SelectedIndexChanged;

            Text = R.Text.CMD_VIEWDUPLICATE;

            ViewFamily useViewFamily = ViewFamily.Invalid;
            foreach (ItmViewFamilyType itm in cmbBox.Items)
            {
                if (itm.ViewfamilyType == null)
                    continue;
                if (_dbDoc == null)
                    _dbDoc = itm.ViewfamilyType.Document;
                useViewFamily = itm.ViewfamilyType.ViewFamily;
                break;
            }

            if (_dbDoc == null && viewTemplates != null && viewTemplates.Count > 0)
                _dbDoc = viewTemplates[0].Document;

            var vfCollector = new FilteredElementCollector(_dbDoc);
            vfCollector.OfClass(typeof(ViewFamilyType));
            var lstUseVft = vfCollector.Cast<ViewFamilyType>().Where(vf1 => vf1.ViewFamily == useViewFamily).ToList();

            foreach (ViewFamilyType vft1 in lstUseVft)
            {
                var itm = new ItmViewFamilyType(vft1);
                CbxViewFamilyType.Items.Add(itm);
            }

            _curViewType = currentViewType;
            _lstViewTemplate = viewTemplates;

            SetLastValue(CbxViewFamilyType, Name);
            SetLastValue(RbnPrefix, Name, true);
            SetLastValue(RbnSuffix, Name, false);
        }

        public string PreSuf
        {
            get => _preSuf;
            set => _preSuf = value;
        }

        public string Prefix
        {
            get => _prefix;
            set => _prefix = value;
        }

        public ItmViewFamilyType ViewFamilyTypeItem
        {
            get => _viewFamilyTypeItem;
            set => _viewFamilyTypeItem = value;
        }

        public ItmView ViewTemplateItem
        {
            get => _viewTemplateItem;
            set => _viewTemplateItem = value;
        }

        private void OK_Button_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TbxAddFor.Text))
            {
                MessageBox.Show(string.Format(R.Text.ERR_FIELD_EMPTY, gpAddFor.Text), Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _prefix = TbxAddFor.Text;
            if (RbnPrefix.Checked)
                _preSuf = R.Text.TXT_PREFIX;
            else
                _preSuf = R.Text.TXT_SUFFIX;

            _viewFamilyTypeItem = (ItmViewFamilyType)CbxViewFamilyType.SelectedItem;
            _viewTemplateItem = (ItmView)CbxViewTemplate.SelectedItem;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void CbxViewFamilyType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var curVftItem = (ItmViewFamilyType)CbxViewFamilyType.SelectedItem;
            ElementId defViewTemplateId = curVftItem.AppliedViewTemplateId;

            if (defViewTemplateId.Equals(ElementId.InvalidElementId))
            {
                CbxViewTemplate.Enabled = true;
                CbxViewTemplate.Items.Clear();

                foreach (RevitView vtemp in _lstViewTemplate)
                {
                    var vtmp = new ItmView(vtemp);
                    if (vtmp.ViewType == _curViewType)
                        CbxViewTemplate.Items.Add(vtmp);
                }
                if (CbxViewTemplate.Items.Count > 0)
                    CbxViewTemplate.SelectedIndex = 0;
            }
            else
            {
                CbxViewTemplate.Items.Clear();
                var defVt = (RevitView)_dbDoc.GetElement(defViewTemplateId);
                var vtItem = new ItmView(defVt);
                CbxViewTemplate.Items.Add(vtItem);
                CbxViewTemplate.SelectedIndex = 0;
                CbxViewTemplate.Enabled = false;
            }
        }

        private void CbxViewTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            var vtItem = CbxViewTemplate.SelectedItem as ItmView;
            TbxAddFor.Text = vtItem?.ToString() ?? string.Empty;
        }
    }
}

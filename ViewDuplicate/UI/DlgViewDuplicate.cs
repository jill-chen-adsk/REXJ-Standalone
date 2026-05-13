using ADSK.ViewExtension.ViewDuplicate.DialogItem;
using R = ADSK.ViewExtension.ViewDuplicate.Resources;
using static ADSK.ViewExtension.ViewDuplicate.Utils.DialogUtil;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using RevitView = Autodesk.Revit.DB.View;

namespace ADSK.ViewExtension.ViewDuplicate.UI
{
    public partial class DlgViewDuplicate : System.Windows.Forms.Form
    {
        private readonly UIDocument _uiDoc;
        private readonly Document _dbDoc;
        private List<RevitView> _allViews;
        private List<ElementId> _allViewIds;
        private List<RevitView> _allViewTemplate;
        private ViewType _currentViewType;

        public DlgViewDuplicate(ExternalCommandData cmdData)
        {
            InitializeComponent();

            OK_Button.Click += OK_Button_Click;
            Cancel_Button.Click += Cancel_Button_Click;
            CbxDicipline.SelectedIndexChanged += CbxDicipline_SelectedIndexChanged;
            CbxViewType.SelectedIndexChanged += CbxViewType_SelectedIndexChanged;
            CbxViewFamilyType.SelectedIndexChanged += CbxViewFamilyType_SelectedIndexChanged;
            BtnAdd.Click += BtnAdd_Click;
            BtnDel.Click += BtnDel_Click;

            Text = R.Text.CMD_VIEWDUPLICATE;
            _uiDoc = cmdData.Application.ActiveUIDocument;
            _dbDoc = _uiDoc.Document;

            CbxDupMode.Items.Clear();
            CbxDupMode.Items.Add(R.Text.DUP_MODE_DUPLICATE);
            CbxDupMode.Items.Add(R.Text.DUP_MODE_WITH_DETAILING);

            var vCollector = new FilteredElementCollector(_dbDoc);
            vCollector.OfClass(typeof(RevitView));
            var q1 = vCollector.Cast<RevitView>().Where(v1 =>
                v1.CanViewBeDuplicated(ViewDuplicateOption.Duplicate) &&
                v1.CanViewBeDuplicated(ViewDuplicateOption.WithDetailing) &&
                v1.HasViewDiscipline());

            _allViews = q1.ToList();
            _allViewIds = new List<ElementId>();

            var lstVdec = new List<ViewDiscipline>();
            var lstVtype = new List<ViewType>();
            var lstFtype = new List<ElementId>();

            foreach (RevitView v1 in _allViews)
            {
                if (v1.IsTemplate)
                    continue;

                _allViewIds.Add(v1.Id);
                ViewDiscipline vdplin = v1.Discipline;
                if (!lstVdec.Contains(vdplin))
                    lstVdec.Add(vdplin);
                ViewType vtype = v1.ViewType;
                if (!lstVtype.Contains(vtype))
                    lstVtype.Add(vtype);
                ElementId vftype = v1.GetTypeId();
                if (!lstFtype.Contains(vftype))
                    lstFtype.Add(vftype);
            }

            var vtCollector = new FilteredElementCollector(_dbDoc);
            vtCollector.OfClass(typeof(RevitView));
            _allViewTemplate = vtCollector.Cast<RevitView>().Where(vt1 => vt1.IsTemplate).ToList();

            foreach (ViewDiscipline vd in lstVdec)
            {
                var vdItem = new ItmViewDiscipline(vd);
                CbxDicipline.Items.Add(vdItem);
            }

            SetLastValue(CbxDicipline, Name);
            SetLastValue(CbxDupMode, Name);
        }

        private void OK_Button_Click(object sender, EventArgs e)
        {
            SaveLastValue(CbxDicipline, Name);
            SaveLastValue(CbxDupMode, Name);

            if (LbxViews.SelectedIndices.Count == 0)
            {
                MessageBox.Show(R.Text.ERR_SELECTTARGETVIEW, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (dgViews.Rows.Count == 0)
            {
                MessageBox.Show(R.Text.ERR_INPUTCOPYRULE, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var lstErrMsg = new List<string>();
            for (int i = 0; i < LbxViews.SelectedItems.Count; i++)
            {
                var viewItem = (ItmView)LbxViews.SelectedItems[i];
                RevitView targetView = viewItem.View;
                var targetVft = (ViewFamilyType)_dbDoc.GetElement(targetView.GetTypeId());

                for (int iRow = 0; iRow < dgViews.RowCount; iRow++)
                {
                    DataGridViewRow dgRow = dgViews.Rows[iRow];
                    object prefixVal = dgRow.Cells[DgcolPrefix.Name].Value;
                    if (prefixVal == null || string.IsNullOrEmpty(prefixVal.ToString()))
                    {
                        lstErrMsg.Add(string.Format(R.Text.ERR_FIELD_EMPTY, DgcolPrefix.HeaderText));
                    }

                    var vftItem = (ItmViewFamilyType)dgRow.Cells[DgcolViewFamilyType.Name].Value;

                    if (targetVft.ViewFamily != vftItem.ViewfamilyType.ViewFamily)
                    {
                        lstErrMsg.Add(string.Format(R.Text.ERR_CANNOTCOPY, targetView.Name, vftItem.ToString()));
                        continue;
                    }
                }
            }

            if (lstErrMsg.Count > 0)
            {
                string strMsg = R.Text.TXT_CANNOTCOPYVIEW;
                foreach (string msg1 in lstErrMsg)
                {
                    strMsg += Environment.NewLine;
                    strMsg += msg1;
                }
                MessageBox.Show(strMsg, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ViewDuplicateOption dupMode = ViewDuplicateOption.Duplicate;
            if (CbxDupMode.SelectedIndex == 0)
                dupMode = ViewDuplicateOption.Duplicate;
            else
                dupMode = ViewDuplicateOption.WithDetailing;

            for (int i = 0; i < LbxViews.SelectedItems.Count; i++)
            {
                var viewItem = (ItmView)LbxViews.SelectedItems[i];
                RevitView targetView = viewItem.View;
                var targetVft = (ViewFamilyType)_dbDoc.GetElement(targetView.GetTypeId());

                for (int iRow = 0; iRow < dgViews.RowCount; iRow++)
                {
                    DataGridViewRow dgRow = dgViews.Rows[iRow];
                    string strPrsuf = dgRow.Cells[DgcolPSfix.Name].Value.ToString();
                    string strSubtext = dgRow.Cells[DgcolPrefix.Name].Value.ToString();
                    var itmViewTemplate = (ItmView)dgRow.Cells[DgcolViewTemplate.Name].Value;
                    var vftItem = (ItmViewFamilyType)dgRow.Cells[DgcolViewFamilyType.Name].Value;

                    try
                    {
                        ElementId newViewid = targetView.Duplicate(dupMode);
                        var newView = (RevitView)_dbDoc.GetElement(newViewid);

                        string tmpName = targetView.Name;
                        if (strPrsuf == R.Text.TXT_PREFIX)
                            tmpName = strSubtext + tmpName;
                        else
                            tmpName = tmpName + strSubtext;
                        string newName = NewSafeName(tmpName, vftItem.ViewfamilyType.Id);
                        newView.Name = newName;

                        newView.ChangeTypeId(vftItem.ViewfamilyType.Id);

                        ViewFamilyType vft1 = vftItem.ViewfamilyType;
                        Parameter prmAsn = vft1.get_Parameter(BuiltInParameter.ASSIGN_TEMPLATE_ON_VIEW_CREATION);
                        int intAsn = 0;
                        if (prmAsn != null)
                            intAsn = prmAsn.AsInteger();
                        if (intAsn == 0)
                            newView.ViewTemplateId = itmViewTemplate.View.Id;
                    }
                    catch
                    {
                    }
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void CbxDicipline_SelectedIndexChanged(object sender, EventArgs e)
        {
            CbxViewType.Items.Clear();

            var itmDc = (ItmViewDiscipline)CbxDicipline.SelectedItem;
            ViewDiscipline vd = itmDc.Discipline;

            var collector = new FilteredElementCollector(_dbDoc, _allViewIds);
            collector.OfClass(typeof(RevitView));
            var lstView = collector.Cast<RevitView>().Where(v1 => v1.Discipline == vd).ToList();

            var lstVt = new List<ViewType>();
            foreach (RevitView v1 in lstView)
            {
                if (!lstVt.Contains(v1.ViewType))
                    lstVt.Add(v1.ViewType);
            }

            foreach (ViewType vt in lstVt)
            {
                var vtItem = new ItmViewType(vt);
                CbxViewType.Items.Add(vtItem);
            }

            SetLastValue(CbxViewType, Name);
        }

        private void CbxViewType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var viewTypeItem = (ItmViewType)CbxViewType.SelectedItem;
            _currentViewType = viewTypeItem.ViewType;

            var disciplineItem = (ItmViewDiscipline)CbxDicipline.SelectedItem;
            ViewDiscipline curDiscipline = disciplineItem.Discipline;

            var collector = new FilteredElementCollector(_dbDoc, _allViewIds);
            collector.OfClass(typeof(RevitView));
            var lstViews = collector.Cast<RevitView>()
                .Where(v1 => v1.Discipline == curDiscipline && v1.ViewType == _currentViewType)
                .ToList();

            var lstDubVftId = new List<ElementId>();
            CbxViewFamilyType.Items.Clear();
            CbxViewFamilyType.Items.Add(new ItmViewFamilyType());
            foreach (RevitView v1 in lstViews)
            {
                var vft = (ViewFamilyType)_dbDoc.GetElement(v1.GetTypeId());
                if (!lstDubVftId.Contains(vft.Id))
                {
                    lstDubVftId.Add(vft.Id);
                    var vftItem = new ItmViewFamilyType(vft);
                    CbxViewFamilyType.Items.Add(vftItem);
                }
            }

            SetLastValue(CbxViewFamilyType, Name);
        }

        private void CbxViewFamilyType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var viewTypeItem = (ItmViewType)CbxViewType.SelectedItem;
            ViewType curViewType = viewTypeItem.ViewType;

            var disciplineItem = (ItmViewDiscipline)CbxDicipline.SelectedItem;
            ViewDiscipline curDiscipline = disciplineItem.Discipline;

            var vftItem = (ItmViewFamilyType)CbxViewFamilyType.SelectedItem;
            ViewFamilyType curVft = vftItem.ViewfamilyType;

            var collector = new FilteredElementCollector(_dbDoc, _allViewIds);
            collector.OfClass(typeof(RevitView));
            List<RevitView> lstViews;
            if (curVft == null)
            {
                lstViews = collector.Cast<RevitView>()
                    .Where(v1 => v1.ViewType == curViewType && v1.Discipline == curDiscipline)
                    .ToList();
            }
            else
            {
                lstViews = collector.Cast<RevitView>()
                    .Where(v1 => v1.GetTypeId().Equals(curVft.Id) && v1.ViewType == curViewType && v1.Discipline == curDiscipline)
                    .ToList();
            }

            LbxViews.Items.Clear();
            foreach (RevitView v1 in lstViews)
            {
                var viewItem = new ItmView(v1);
                LbxViews.Items.Add(viewItem);
            }

            SetLastValue(LbxViews, Name);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var dlg1 = new DlgViewDuplicateItem(CbxViewFamilyType, _currentViewType, _allViewTemplate))
            {
                if (dlg1.ShowDialog() == DialogResult.OK)
                {
                    int iRow = dgViews.Rows.Add();
                    DataGridViewRow dgRow = dgViews.Rows[iRow];
                    dgRow.Cells[DgcolPSfix.Name].Value = dlg1.PreSuf;
                    dgRow.Cells[DgcolPrefix.Name].Value = dlg1.Prefix;
                    dgRow.Cells[DgcolViewTemplate.Name].Value = dlg1.ViewTemplateItem;
                    dgRow.Cells[DgcolViewFamilyType.Name].Value = dlg1.ViewFamilyTypeItem;
                }
            }
        }

        private void BtnDel_Click(object sender, EventArgs e)
        {
            if (dgViews.SelectedRows.Count == 0)
            {
                MessageBox.Show(R.Text.ERR_SELECTROW, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (DataGridViewRow selectedRow in dgViews.SelectedRows)
                dgViews.Rows.RemoveAt(selectedRow.Index);
        }

        private string NewSafeName(string newName, ElementId vfTypeId)
        {
            var collector = new FilteredElementCollector(_dbDoc);
            collector.OfClass(typeof(RevitView));
            var lstAllViews = collector.Cast<RevitView>().Where(v1 => v1.GetTypeId().Equals(vfTypeId)).ToList();

            var lstAllViewName = new List<string>();
            foreach (RevitView av1 in lstAllViews)
                lstAllViewName.Add(av1.Name.ToUpperInvariant());

            if (!lstAllViewName.Contains(newName.ToUpperInvariant()))
                return newName;

            int i = 0;
            string newNameSuf = newName;
            while (true)
            {
                i += 1;
                        newNameSuf = string.Concat(newName, R.Text.TXT_LEFTBRACE, i, R.Text.TXT_RIGHTBRACE);
                if (!lstAllViewName.Contains(newNameSuf.ToUpperInvariant()))
                    break;
            }
            return newNameSuf;
        }
    }
}

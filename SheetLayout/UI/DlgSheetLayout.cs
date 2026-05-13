using ADSK.ViewExtension.SheetLayout.DialogItem;
using ADSK.ViewExtension.SheetLayout.Sorter;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitView = Autodesk.Revit.DB.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using R = ADSK.ViewExtension.SheetLayout.Resources;

namespace ADSK.ViewExtension.SheetLayout
{
    public partial class DlgSheetLayout : System.Windows.Forms.Form
    {
        private readonly UIDocument m_uiDoc;
        private readonly Document m_dbDoc;
        private readonly ViewSheet m_curSheet;
        private readonly List<Viewport> m_refVps = new List<Viewport>();
        private List<ScheduleSheetInstance> m_refScheduleInstances;
        private readonly List<Viewport> m_refLegVps = new List<Viewport>();
        private List<RevitView> m_AllViews;
        private List<ElementId> m_AllViewIds;
        private readonly List<ElementId> m_SelectedViewIds = new List<ElementId>();
        private ViewType m_currentViewType;

        public DlgSheetLayout(ExternalCommandData cmdData)
        {
            InitializeComponent();

            Text = R.Text.CMD_SHEETLAYOUT;
            m_uiDoc = cmdData.Application.ActiveUIDocument;
            m_dbDoc = m_uiDoc.Document;

            if (m_dbDoc.ActiveView.ViewType != ViewType.DrawingSheet)
                throw new InvalidOperationException(R.Text.ERR_NOTVIEWSHEET);

            m_curSheet = (ViewSheet)m_dbDoc.ActiveView;

            var lstVpIds = m_curSheet.GetAllViewports().ToList();

            var shinstCollector = new FilteredElementCollector(m_dbDoc, m_curSheet.Id);
            shinstCollector.OfClass(typeof(ScheduleSheetInstance));
            var schq1 = shinstCollector.Cast<ScheduleSheetInstance>().Where(sch1 => !sch1.IsTitleblockRevisionSchedule);
            m_refScheduleInstances = schq1.ToList();

            if (lstVpIds.Count == 0 && m_refScheduleInstances.Count == 0)
                throw new InvalidOperationException(R.Text.ERR_NOVIEWPORTSCHEDULE);

            if (lstVpIds.Count == 0)
                GrpViewports.Enabled = false;

            if (m_refScheduleInstances.Count == 0)
                GrpSchedule.Enabled = false;

            foreach (ElementId vpid in lstVpIds)
            {
                var vp1 = (Viewport)m_dbDoc.GetElement(vpid);
                m_refVps.Add(vp1);
            }

            var sorter = new CmpVpByNum();
            m_refVps.Sort(sorter);
            m_refVps.Reverse();

            for (int i = m_refVps.Count - 1; i >= 0; i--)
            {
                Viewport refVp = m_refVps[i];
                ElementId refViewId = refVp.ViewId;
                var orgView = (RevitView)m_dbDoc.GetElement(refViewId);
                if (orgView.ViewType == ViewType.Legend)
                {
                    m_refVps.RemoveAt(i);
                    m_refLegVps.Add(refVp);
                }
            }

            if (m_refLegVps.Count > 1)
                m_refVps.Reverse();

            if (m_refLegVps.Count > 0)
                ChkAddSameLeg.Visible = true;
            else
            {
                ChkAddSameLeg.Checked = false;
                ChkAddSameLeg.Visible = false;
            }

            for (int i = 0; i < m_refVps.Count; i++)
            {
                Viewport vp1 = m_refVps[i];
                var itmVp = new ItmViewPort(vp1);
                LbxViewports.Items.Add(itmVp);
            }

            LbxViewports.Sorted = true;
            LbxViewports.Sorted = false;

            var lstNgViewTypes = new List<ViewType>
            {
                ViewType.DrawingSheet,
                ViewType.Internal,
                ViewType.Legend,
                ViewType.ProjectBrowser,
                ViewType.SystemBrowser,
                ViewType.Undefined
            };

            var vCollector = new FilteredElementCollector(m_dbDoc);
            vCollector.OfClass(typeof(RevitView));
            var q1 = vCollector.Cast<RevitView>().Where(v1 =>
                !lstNgViewTypes.Contains(v1.ViewType) &&
                Viewport.CanAddViewToSheet(m_dbDoc, m_curSheet.Id, v1.Id) &&
                v1.HasViewDiscipline());

            m_AllViews = q1.ToList();
            m_AllViewIds = new List<ElementId>();

            var lstVdec = new List<ViewDiscipline>();
            var lstVtype = new List<ViewType>();
            var lstFtype = new List<ElementId>();

            foreach (RevitView v1 in m_AllViews)
            {
                if (!v1.IsTemplate)
                {
                    m_AllViewIds.Add(v1.Id);
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
            }

            foreach (ViewDiscipline vd in lstVdec)
            {
                var vdItem = new ItmViewDiscipline(vd);
                CbxDicipline.Items.Add(vdItem);
            }

            ADSK.ViewExtension.SheetLayout.Utils.DialogUtil.SetLastValue(CbxDicipline, Name);

            foreach (ScheduleSheetInstance schInst in m_refScheduleInstances)
                LbxScheduleInstances.Items.Add(new ItmSchedule(schInst));

            var schCollector = new FilteredElementCollector(m_dbDoc);
            schCollector.OfClass(typeof(ViewSchedule));
            var schq2 = schCollector.Cast<ViewSchedule>().Where(sch2 =>
                !sch2.IsTitleblockRevisionSchedule &&
                !sch2.IsTemplate &&
                !sch2.IsInternalKeynoteSchedule);

            List<ViewSchedule> lstSchedule = schq2.ToList();
            foreach (ViewSchedule schView in lstSchedule)
                LbxSchedules.Items.Add(new ItmSchedule(schView));

            LbxSchedules.Sorted = true;

            WireEvents();
        }

        private void WireEvents()
        {
            OK_Button.Click += OK_Button_Click;
            Cancel_Button.Click += Cancel_Button_Click;
            CbxDicipline.SelectedIndexChanged += CbxDicipline_SelectedIndexChanged;
            CbxViewType.SelectedIndexChanged += CbxViewType_SelectedIndexChanged;
            CbxViewFamilyType.SelectedIndexChanged += CbxViewFamilyType_SelectedIndexChanged;
            BtnAdd.Click += BtnAdd_Click;
            BtnRmv.Click += BtnRmv_Click;
            BtnUp2.Click += BtnUp2_Click;
            BtnDn2.Click += BtnDn2_Click;
            BtnUP.Click += BtnUP_Click;
            BtnDN.Click += BtnDN_Click;
            LbxViews.DoubleClick += LbxViews_DoubleClick;
            LvViewOnSheet.DoubleClick += LvViewOnSheet_DoubleClick;
            BtnAdd2.Click += BtnAdd2_Click;
            BtmRmv.Click += BtmRmv_Click;
            BtnUP3.Click += BtnUP3_Click;
            BtnDN3.Click += BtnDN3_Click;
            LbxSchedules.DoubleClick += LbxSchedules_DoubleClick;
            LvScheduleinstanceOnSheet.DoubleClick += LvScheduleinstanceOnSheet_DoubleClick;
        }

        private void OK_Button_Click(object sender, EventArgs e)
        {
            if (LvViewOnSheet.Items.Count == 0 && LvScheduleinstanceOnSheet.Items.Count == 0)
            {
                MessageBox.Show(R.Text.ERR_SELECTVIEW, Text, MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }

            var tbCollector = new FilteredElementCollector(m_dbDoc, m_curSheet.Id);
            tbCollector.OfCategory(BuiltInCategory.OST_TitleBlocks).OfClass(typeof(FamilyInstance));
            List<FamilyInstance> lstTbs = tbCollector.Cast<FamilyInstance>().ToList();
            ElementId tbBlkId = ElementId.InvalidElementId;
            if (lstTbs.Count > 0)
                tbBlkId = lstTbs[0].Symbol.Id;

            int iSheetNum = -1;
            int iViewPortNum = -1;
            ViewSheet targetSheet = null;
            var lstCreatedSheetIds = new List<ElementId>();

            for (int i = 0; i < LvViewOnSheet.Items.Count; i++)
            {
                ListViewItem itmView = LvViewOnSheet.Items[i];
                ElementId viewId = new ElementId(long.Parse(itmView.SubItems[0].Text));
                int curSheetNum = int.Parse(itmView.SubItems[1].Text);
                iViewPortNum = int.Parse(itmView.SubItems[2].Text);

                if (curSheetNum != iSheetNum)
                {
                    targetSheet = ViewSheet.Create(m_dbDoc, tbBlkId);
                    iSheetNum = curSheetNum;
                }

                string strSheetTitle = m_curSheet.Name;
                targetSheet.Name = strSheetTitle;

                var vpItem = (ItmViewPort)LbxViewports.Items[iViewPortNum - 1];
                Viewport targetViewPort = vpItem.ViewPort;
                Viewport newViewPort = Viewport.Create(m_dbDoc, targetSheet.Id, viewId, targetViewPort.GetBoxCenter());
                newViewPort.ChangeTypeId(targetViewPort.GetTypeId());

                Parameter prmTargetRotation = targetViewPort.get_Parameter(BuiltInParameter.VIEWPORT_ATTR_ORIENTATION_ON_SHEET);
                Parameter prmNewRotation = newViewPort.get_Parameter(BuiltInParameter.VIEWPORT_ATTR_ORIENTATION_ON_SHEET);
                prmNewRotation.Set(prmTargetRotation.AsInteger());

                if (!lstCreatedSheetIds.Contains(targetSheet.Id))
                    lstCreatedSheetIds.Add(targetSheet.Id);
            }

            iSheetNum = -1;
            iViewPortNum = -1;
            targetSheet = null;
            for (int i = 0; i < LvScheduleinstanceOnSheet.Items.Count; i++)
            {
                ListViewItem itmView = LvScheduleinstanceOnSheet.Items[i];
                ElementId viewId = new ElementId(long.Parse(itmView.SubItems[0].Text));
                int curSheetNum = int.Parse(itmView.SubItems[1].Text);
                iViewPortNum = int.Parse(itmView.SubItems[2].Text);

                if (curSheetNum != iSheetNum)
                {
                    targetSheet = null;
                    if (curSheetNum - 1 < lstCreatedSheetIds.Count)
                        targetSheet = (ViewSheet)m_dbDoc.GetElement(lstCreatedSheetIds[curSheetNum - 1]);
                    else
                        targetSheet = ViewSheet.Create(m_dbDoc, tbBlkId);

                    iSheetNum = curSheetNum;
                }

                XYZ schPos = m_refScheduleInstances[iViewPortNum - 1].Point;
                ScheduleSheetInstance.Create(m_dbDoc, targetSheet.Id, viewId, schPos);

                if (!lstCreatedSheetIds.Contains(targetSheet.Id))
                    lstCreatedSheetIds.Add(targetSheet.Id);
            }

            if (ChkAddSameLeg.Checked && lstCreatedSheetIds.Count > 0 && m_refLegVps.Count > 0)
            {
                foreach (ElementId tgSheetId in lstCreatedSheetIds)
                {
                    var tgSheet = (ViewSheet)m_dbDoc.GetElement(tgSheetId);
                    foreach (Viewport legvpt in m_refLegVps)
                        Viewport.Create(m_dbDoc, tgSheet.Id, legvpt.ViewId, legvpt.GetBoxCenter());
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

            var collector = new FilteredElementCollector(m_dbDoc, m_AllViewIds);
            collector.OfClass(typeof(RevitView));
            var q1 = collector.Cast<RevitView>().Where(v1 => v1.Discipline == vd);
            List<RevitView> lstView = q1.ToList();

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

            ADSK.ViewExtension.SheetLayout.Utils.DialogUtil.SetLastValue(CbxViewType, Name);
        }

        private void CbxViewType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ViewTypeAction();
            ADSK.ViewExtension.SheetLayout.Utils.DialogUtil.SaveLastValue(CbxViewType, Name);
        }

        private void ViewTypeAction()
        {
            var ViewTypeItem = (ItmViewType)CbxViewType.SelectedItem;
            m_currentViewType = ViewTypeItem.ViewType;

            var DisciplineItem = (ItmViewDiscipline)CbxDicipline.SelectedItem;
            ViewDiscipline curDiscipline = DisciplineItem.Discipline;

            var collector = new FilteredElementCollector(m_dbDoc, m_AllViewIds);
            collector.OfClass(typeof(RevitView));
            var q1 = collector.Cast<RevitView>().Where(v1 => v1.Discipline == curDiscipline && v1.ViewType == m_currentViewType);
            List<RevitView> lstViews = q1.ToList();

            var lstDubVftId = new List<ElementId>();
            CbxViewFamilyType.Items.Clear();
            CbxViewFamilyType.Items.Add(new ItmViewFamilyType());
            foreach (RevitView v1 in lstViews)
            {
                var vft = (ViewFamilyType)m_dbDoc.GetElement(v1.GetTypeId());
                if (!lstDubVftId.Contains(vft.Id))
                {
                    lstDubVftId.Add(vft.Id);
                    var VftItem = new ItmViewFamilyType(vft);
                    CbxViewFamilyType.Items.Add(VftItem);
                }
            }

            ADSK.ViewExtension.SheetLayout.Utils.DialogUtil.SetLastValue(CbxViewFamilyType, Name);
        }

        private void CbxViewFamilyType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ViewFamilyTypeAction();
            ADSK.ViewExtension.SheetLayout.Utils.DialogUtil.SaveLastValue(CbxViewFamilyType, Name);
        }

        private void ViewFamilyTypeAction()
        {
            var ViewTypeItem = (ItmViewType)CbxViewType.SelectedItem;
            ViewType curViewType = ViewTypeItem.ViewType;

            var DisciplineItem = (ItmViewDiscipline)CbxDicipline.SelectedItem;
            ViewDiscipline curDiscipline = DisciplineItem.Discipline;

            var VftItem = (ItmViewFamilyType)CbxViewFamilyType.SelectedItem;
            ViewFamilyType curVft = VftItem.ViewfamilyType;

            var collector = new FilteredElementCollector(m_dbDoc, m_AllViewIds);
            collector.OfClass(typeof(RevitView));
            List<RevitView> lstViews;
            if (curVft == null)
            {
                var q1 = collector.Cast<RevitView>().Where(v1 => v1.ViewType == curViewType && v1.Discipline == curDiscipline);
                lstViews = q1.ToList();
            }
            else
            {
                var q1 = collector.Cast<RevitView>().Where(v1 => v1.GetTypeId() == curVft.Id && v1.ViewType == curViewType);
                lstViews = q1.ToList();
            }

            if (m_currentViewType == ViewType.AreaPlan ||
                m_currentViewType == ViewType.CeilingPlan ||
                m_currentViewType == ViewType.EngineeringPlan ||
                m_currentViewType == ViewType.FloorPlan)
            {
                var sorterLv = new CmpViewGenLevel();
                lstViews.Sort(sorterLv);
            }

            LbxViews.Items.Clear();
            foreach (RevitView v1 in lstViews)
            {
                var viewItem = new ItmView(v1);
                LbxViews.Items.Add(viewItem);
            }

            if (m_currentViewType == ViewType.AreaPlan ||
                m_currentViewType == ViewType.CeilingPlan ||
                m_currentViewType == ViewType.EngineeringPlan ||
                m_currentViewType == ViewType.FloorPlan)
            {
                LbxViews.Sorted = false;
            }
            else
            {
                LbxViews.Sorted = true;
                LbxViews.Sorted = false;
            }

            ADSK.ViewExtension.SheetLayout.Utils.DialogUtil.SetLastValue(LbxViews, Name);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (LbxViews.SelectedItems.Count == 0)
                return;

            foreach (ItmView itmView in LbxViews.SelectedItems)
            {
                ListViewItem lvItem = LvViewOnSheet.Items.Add(itmView.View.Id.ToString());
                lvItem.SubItems.Add(string.Empty);
                lvItem.SubItems.Add(string.Empty);
                lvItem.SubItems.Add(itmView.View.Name);

                ElementId moveId = itmView.View.Id;
                m_AllViewIds.Remove(moveId);
                m_SelectedViewIds.Add(moveId);
            }

            Renumber();
            ViewFamilyTypeAction();
        }

        private void Renumber()
        {
            int eachVpNum = LbxViewports.Items.Count;
            int intViewNum = 0;

            for (int i = 0; i < LvViewOnSheet.Items.Count; i++)
            {
                ListViewItem lvItem0 = LvViewOnSheet.Items[i];
                int iSheetNum = Math.DivRem(i, eachVpNum, out _) + 1;
                lvItem0.SubItems[1].Text = iSheetNum.ToString();

                Math.DivRem(i + 1, eachVpNum, out intViewNum);
                if (intViewNum == 0)
                    intViewNum = eachVpNum;
                lvItem0.SubItems[2].Text = intViewNum.ToString();
            }
        }

        private void BtnRmv_Click(object sender, EventArgs e)
        {
            if (LvViewOnSheet.SelectedIndices.Count == 0)
                return;

            for (int idx = LvViewOnSheet.SelectedIndices.Count - 1; idx >= 0; idx--)
            {
                int idx0 = LvViewOnSheet.SelectedIndices[idx];
                ListViewItem lvItem = LvViewOnSheet.Items[idx0];
                LvViewOnSheet.Items.RemoveAt(idx0);
                ElementId idSelected = new ElementId(long.Parse(lvItem.SubItems[0].Text));
                m_SelectedViewIds.Remove(idSelected);
                m_AllViewIds.Add(idSelected);
            }

            Renumber();
            ViewFamilyTypeAction();
            Renumber();
        }

        private void BtnUp2_Click(object sender, EventArgs e)
        {
            var lstSelIdxs = new List<int>();
            for (int i = 0; i < LvViewOnSheet.SelectedIndices.Count; i++)
            {
                int idx = LvViewOnSheet.SelectedIndices[i];
                if (idx == 0)
                    return;

                ListViewItem ivItem = LvViewOnSheet.Items[idx];
                LvViewOnSheet.Items.RemoveAt(idx);
                LvViewOnSheet.Items.Insert(idx - 1, ivItem);
                lstSelIdxs.Add(idx - 1);
            }

            Renumber();

            for (int i = 0; i < LvViewOnSheet.Items.Count; i++)
                LvViewOnSheet.Items[i].Selected = lstSelIdxs.Contains(i);
        }

        private void BtnDn2_Click(object sender, EventArgs e)
        {
            var lstSelIdxs = new List<int>();

            for (int i = LvViewOnSheet.SelectedIndices.Count - 1; i >= 0; i--)
            {
                int idx = LvViewOnSheet.SelectedIndices[i];
                if (idx == LvViewOnSheet.Items.Count - 1)
                    return;

                ListViewItem ivItem = LvViewOnSheet.Items[idx];
                LvViewOnSheet.Items.RemoveAt(idx);
                LvViewOnSheet.Items.Insert(idx + 1, ivItem);
                lstSelIdxs.Add(idx + 1);
            }

            Renumber();

            for (int i = 0; i < LvViewOnSheet.Items.Count; i++)
                LvViewOnSheet.Items[i].Selected = lstSelIdxs.Contains(i);
        }

        private void BtnUP_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < LbxViewports.SelectedIndices.Count; i++)
            {
                int idx = LbxViewports.SelectedIndices[i];
                if (idx == 0)
                    return;

                var ivItem = (ItmViewPort)LbxViewports.Items[idx];
                LbxViewports.Items.RemoveAt(idx);
                LbxViewports.Items.Insert(idx - 1, ivItem);
                LbxViewports.SetSelected(idx - 1, true);
            }
        }

        private void BtnDN_Click(object sender, EventArgs e)
        {
            for (int i = LbxViewports.SelectedIndices.Count - 1; i >= 0; i--)
            {
                int idx = LbxViewports.SelectedIndices[i];
                if (idx == LbxViewports.Items.Count - 1)
                    return;

                var ivItem = (ItmViewPort)LbxViewports.Items[idx];
                LbxViewports.Items.RemoveAt(idx);
                LbxViewports.Items.Insert(idx + 1, ivItem);
                LbxViewports.SetSelected(idx + 1, true);
            }
        }

        private void LbxViews_DoubleClick(object sender, EventArgs e)
        {
            if (LbxViews.SelectedItems.Count == 0)
                return;

            foreach (ItmView itmView in LbxViews.SelectedItems)
            {
                ListViewItem lvItem = LvViewOnSheet.Items.Add(itmView.View.Id.ToString());
                lvItem.SubItems.Add(string.Empty);
                lvItem.SubItems.Add(string.Empty);
                lvItem.SubItems.Add(itmView.View.Name);

                ElementId moveId = itmView.View.Id;
                m_AllViewIds.Remove(moveId);
                m_SelectedViewIds.Add(moveId);
            }

            Renumber();
            ViewFamilyTypeAction();
        }

        private void LvViewOnSheet_DoubleClick(object sender, EventArgs e)
        {
            if (LvViewOnSheet.SelectedIndices.Count == 0)
                return;

            for (int idx = LvViewOnSheet.SelectedIndices.Count - 1; idx >= 0; idx--)
            {
                int idx0 = LvViewOnSheet.SelectedIndices[idx];
                ListViewItem lvItem = LvViewOnSheet.Items[idx0];
                LvViewOnSheet.Items.RemoveAt(idx0);
                ElementId idSelected = new ElementId(long.Parse(lvItem.SubItems[0].Text));
                m_SelectedViewIds.Remove(idSelected);
                m_AllViewIds.Add(idSelected);
            }

            Renumber();
            ViewFamilyTypeAction();
            Renumber();
        }

        private void BtnAdd2_Click(object sender, EventArgs e)
        {
            if (LbxSchedules.SelectedItems.Count == 0)
                return;

            foreach (ItmSchedule itmView in LbxSchedules.SelectedItems)
            {
                ListViewItem lvItem = LvScheduleinstanceOnSheet.Items.Add(itmView.Id.ToString());
                lvItem.SubItems.Add(string.Empty);
                lvItem.SubItems.Add(string.Empty);
                lvItem.SubItems.Add(itmView.Name);
            }

            RenumberSchedule();
        }

        private void RenumberSchedule()
        {
            int eachVpNum = LbxScheduleInstances.Items.Count;
            int intViewNum = 0;

            for (int i = 0; i < LvScheduleinstanceOnSheet.Items.Count; i++)
            {
                ListViewItem lvItem0 = LvScheduleinstanceOnSheet.Items[i];
                int iSheetNum = Math.DivRem(i, eachVpNum, out _) + 1;
                lvItem0.SubItems[1].Text = iSheetNum.ToString();

                Math.DivRem(i + 1, eachVpNum, out intViewNum);
                if (intViewNum == 0)
                    intViewNum = eachVpNum;
                lvItem0.SubItems[2].Text = intViewNum.ToString();
            }
        }

        private void BtmRmv_Click(object sender, EventArgs e)
        {
            if (LvScheduleinstanceOnSheet.SelectedIndices.Count == 0)
                return;

            for (int idx = LvScheduleinstanceOnSheet.SelectedIndices.Count - 1; idx >= 0; idx--)
            {
                int idx0 = LvScheduleinstanceOnSheet.SelectedIndices[idx];
                LvScheduleinstanceOnSheet.Items.RemoveAt(idx0);
            }

            RenumberSchedule();
        }

        private void BtnUP3_Click(object sender, EventArgs e)
        {
            var lstSelIdxs = new List<int>();
            for (int i = 0; i < LvScheduleinstanceOnSheet.SelectedIndices.Count; i++)
            {
                int idx = LvScheduleinstanceOnSheet.SelectedIndices[i];
                if (idx == 0)
                    return;

                ListViewItem ivItem = LvScheduleinstanceOnSheet.Items[idx];
                LvScheduleinstanceOnSheet.Items.RemoveAt(idx);
                LvScheduleinstanceOnSheet.Items.Insert(idx - 1, ivItem);
                lstSelIdxs.Add(idx - 1);
            }

            RenumberSchedule();

            for (int i = 0; i < LvScheduleinstanceOnSheet.Items.Count; i++)
                LvScheduleinstanceOnSheet.Items[i].Selected = lstSelIdxs.Contains(i);
        }

        private void BtnDN3_Click(object sender, EventArgs e)
        {
            var lstSelIdxs = new List<int>();

            for (int i = LvScheduleinstanceOnSheet.SelectedIndices.Count - 1; i >= 0; i--)
            {
                int idx = LvScheduleinstanceOnSheet.SelectedIndices[i];
                if (idx == LvScheduleinstanceOnSheet.Items.Count - 1)
                    return;

                ListViewItem ivItem = LvScheduleinstanceOnSheet.Items[idx];
                LvScheduleinstanceOnSheet.Items.RemoveAt(idx);
                LvScheduleinstanceOnSheet.Items.Insert(idx + 1, ivItem);
                lstSelIdxs.Add(idx + 1);
            }

            RenumberSchedule();

            for (int i = 0; i < LvScheduleinstanceOnSheet.Items.Count; i++)
                LvScheduleinstanceOnSheet.Items[i].Selected = lstSelIdxs.Contains(i);
        }

        private void LbxSchedules_DoubleClick(object sender, EventArgs e)
        {
            if (LbxSchedules.SelectedItems.Count == 0)
                return;

            foreach (ItmSchedule itmView in LbxSchedules.SelectedItems)
            {
                ListViewItem lvItem = LvScheduleinstanceOnSheet.Items.Add(itmView.Id.ToString());
                lvItem.SubItems.Add(string.Empty);
                lvItem.SubItems.Add(string.Empty);
                lvItem.SubItems.Add(itmView.Name);
            }

            RenumberSchedule();
        }

        private void LvScheduleinstanceOnSheet_DoubleClick(object sender, EventArgs e)
        {
            if (LvScheduleinstanceOnSheet.SelectedIndices.Count == 0)
                return;

            for (int idx = LvScheduleinstanceOnSheet.SelectedIndices.Count - 1; idx >= 0; idx--)
            {
                int idx0 = LvScheduleinstanceOnSheet.SelectedIndices[idx];
                LvScheduleinstanceOnSheet.Items.RemoveAt(idx0);
            }

            RenumberSchedule();
        }
    }
}

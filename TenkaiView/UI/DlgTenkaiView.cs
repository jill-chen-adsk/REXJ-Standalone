using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using ADSK.ViewExtension.TenkaiView.DialogItem;
using ADSK.ViewExtension.TenkaiView.Utils;
using R = ADSK.ViewExtension.TenkaiView.Resources;

namespace ADSK.ViewExtension.TenkaiView.UI
{
    public partial class DlgTenkaiView : System.Windows.Forms.Form
    {
        private readonly UIDocument m_uiDoc;
        private readonly Document m_dbDoc;
        private readonly Autodesk.Revit.DB.View m_View;
        private List<ElementId> m_RoomIds;
        private int m_Scale = 100;

        private CreateTenkaiJoken m_TenkaiCondition;
        public CreateTenkaiJoken TenkaiKondition => m_TenkaiCondition;

        public List<ElementId> RoomIds => m_RoomIds;

        public DlgTenkaiView(ExternalCommandData cmdData)
        {
            InitializeComponent();

            this.Text = R.Text.CMD_TENKAIVIEW;
            m_uiDoc = cmdData.Application.ActiveUIDocument;
            m_dbDoc = m_uiDoc.Document;
            m_View = m_dbDoc.ActiveView;

            if (m_View.ViewType != ViewType.FloorPlan)
                throw new InvalidOperationException(R.Text.ERR_NOTFLOORPLAN);

            ICollection<ElementId> lstSelIds = m_uiDoc.Selection.GetElementIds();
            RoomFilter roomFilt = new RoomFilter();
            List<ElementId> lstRoomIds = new List<ElementId>();
            if (lstSelIds.Count > 0)
            {
                FilteredElementCollector collector1 = new FilteredElementCollector(m_dbDoc, lstSelIds);
                collector1.WherePasses(roomFilt);
                lstRoomIds = collector1.ToElementIds().ToList();
            }
            if (lstRoomIds.Count == 0)
            {
                FilteredElementCollector collector2 = new FilteredElementCollector(m_dbDoc, m_View.Id);
                collector2.WherePasses(roomFilt);
                lstRoomIds = collector2.ToElementIds().ToList();
            }

            if (lstRoomIds.Count == 0)
                throw new InvalidOperationException(R.Text.ERR_CANNOTSELECTROOM);

            foreach (ElementId rmid in lstRoomIds)
            {
                Room rmtemp = m_dbDoc.GetElement(rmid) as Room;
                ItmRoom itmRoom = new ItmRoom(rmtemp);
                chbxRooms.Items.Add(itmRoom);
            }
            chbxRooms.Sorted = true;

            FilteredElementCollector vftCollector = new FilteredElementCollector(m_dbDoc);
            vftCollector.OfClass(typeof(ViewFamilyType));
            List<ViewFamilyType> lstVftElvs = vftCollector.Cast<ViewFamilyType>()
                .Where(vft => vft.ViewFamily == ViewFamily.Elevation)
                .ToList();

            foreach (ViewFamilyType vftTemp in lstVftElvs)
            {
                ItmViewFamily iViewType = new ItmViewFamily(vftTemp);
                cbxViewType.Items.Add(iViewType);
            }

            FilteredElementCollector dCollector = new FilteredElementCollector(m_dbDoc);
            dCollector.OfClass(typeof(DimensionType));
            List<DimensionType> lstLinerDts = dCollector.Cast<DimensionType>()
                .Where(dt => dt.StyleType == DimensionStyleType.Linear)
                .ToList();

            foreach (DimensionType dTp in lstLinerDts)
            {
                if (dTp.Parameters.Size > 20)
                {
                    ItmDimStyle iDim = new ItmDimStyle(dTp);
                    cbxDimGrid.Items.Add(iDim);
                    cbxDimLevel.Items.Add(iDim);
                }
            }

            ADSK.ViewExtension.TenkaiView.Utils.DialogUtil.SetLastValue(cbxViewType, Name);
            ADSK.ViewExtension.TenkaiView.Utils.DialogUtil.SetLastValue(rbnTrimVol, Name, true);
            ADSK.ViewExtension.TenkaiView.Utils.DialogUtil.SetLastValue(rbnTrimLevel, Name, false);
            ADSK.ViewExtension.TenkaiView.Utils.DialogUtil.SetLastValue(tbxLR, Name, "0");
            ADSK.ViewExtension.TenkaiView.Utils.DialogUtil.SetLastValue(tbxTB, Name, "0");
            ADSK.ViewExtension.TenkaiView.Utils.DialogUtil.SetLastValue(cbxDimGrid, Name);
            ADSK.ViewExtension.TenkaiView.Utils.DialogUtil.SetLastValue(cbxDimLevel, Name);
            ADSK.ViewExtension.TenkaiView.Utils.DialogUtil.SetLastValue(numUPDScale, Name, 100);

            OK_Button.Click += OK_Button_Click;
            Cancel_Button.Click += Cancel_Button_Click;
            btnSelAll.Click += btnSelAll_Click;
            Button1.Click += Button1_Click;
        }

        private void OK_Button_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(tbxLR.Text, out _))
            {
                MessageBox.Show(string.Concat(lblLR.Text, R.Text.ERR_NOTNUMBER), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!double.TryParse(tbxTB.Text, out _))
            {
                MessageBox.Show(string.Concat(lblTB.Text, R.Text.ERR_NOTNUMBER), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (chbxRooms.CheckedIndices.Count == 0)
            {
                MessageBox.Show(R.Text.ERR_NOSELECTROOM, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!(cbxViewType.SelectedItem is ItmViewFamily iVFT))
            {
                MessageBox.Show(R.Text.ERR_NOSELECTVIEWFAMILY, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            m_TenkaiCondition = new CreateTenkaiJoken();

            m_Scale = (int)numUPDScale.Value;

            m_TenkaiCondition.ViewTypeID = iVFT.VfType.Id;
            m_TenkaiCondition.ExtendedRightLeft = double.Parse(tbxLR.Text);
            m_TenkaiCondition.ExtendTopBottom = double.Parse(tbxTB.Text);

            if (rbnTrimVol.Checked)
                m_TenkaiCondition.TrimBase = CreateTenkaiJoken.TrimingBase.RoomVolume;
            else
                m_TenkaiCondition.TrimBase = CreateTenkaiJoken.TrimingBase.BetweenLevel;

            ItmDimStyle itmDimGrid = cbxDimGrid.SelectedItem as ItmDimStyle;
            ItmDimStyle itmDimLevel = cbxDimLevel.SelectedItem as ItmDimStyle;
            if (itmDimGrid != null)
                m_TenkaiCondition.DimTypeTorishinID = itmDimGrid.Id;
            if (itmDimLevel != null)
                m_TenkaiCondition.DimLevelID = itmDimLevel.Id;

            m_TenkaiCondition.DimTypeCHID = ElementId.InvalidElementId;
            m_TenkaiCondition.DimCHText = R.Text.TXT_PREFIX;
            m_TenkaiCondition.ViewScale = m_Scale;

            m_RoomIds = new List<ElementId>();
            foreach (ItmRoom itmRm in chbxRooms.CheckedItems)
                m_RoomIds.Add(itmRm.RoomId);

            ADSK.ViewExtension.TenkaiView.Utils.DialogUtil.SaveLastValue(cbxViewType, Name);
            ADSK.ViewExtension.TenkaiView.Utils.DialogUtil.SaveLastValue(rbnTrimVol, Name);
            ADSK.ViewExtension.TenkaiView.Utils.DialogUtil.SaveLastValue(rbnTrimLevel, Name);
            ADSK.ViewExtension.TenkaiView.Utils.DialogUtil.SaveLastValue(tbxLR, Name);
            ADSK.ViewExtension.TenkaiView.Utils.DialogUtil.SaveLastValue(tbxTB, Name);
            ADSK.ViewExtension.TenkaiView.Utils.DialogUtil.SaveLastValue(cbxDimGrid, Name);
            ADSK.ViewExtension.TenkaiView.Utils.DialogUtil.SaveLastValue(cbxDimLevel, Name);
            ADSK.ViewExtension.TenkaiView.Utils.DialogUtil.SaveLastValue(numUPDScale, Name);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnSelAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chbxRooms.Items.Count; i++)
                chbxRooms.SetItemChecked(i, true);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chbxRooms.Items.Count; i++)
                chbxRooms.SetItemChecked(i, false);
        }
    }
}

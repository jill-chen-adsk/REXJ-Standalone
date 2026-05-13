using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ADSK.ViewExtension.TenkaiView.Utils;
using R = ADSK.ViewExtension.TenkaiView.Resources;

namespace ADSK.ViewExtension.TenkaiView.UI
{
    public partial class DlgCreateTenkaiProcess : System.Windows.Forms.Form
    {
        private bool m_isCanceled;
        private bool m_isRunning;

        private readonly Document m_dbDoc;
        private readonly UIDocument m_uiDoc;
        private readonly Autodesk.Revit.DB.View m_curView;
        private readonly List<ElementId> m_RoomIdList;
        private readonly CreateTenkaiJoken m_TenkaiJoken;

        public DlgCreateTenkaiProcess(ExternalCommandData cmdData, List<ElementId> roomIdList, CreateTenkaiJoken iTenkaiJoken)
        {
            InitializeComponent();

            m_uiDoc = cmdData.Application.ActiveUIDocument;
            m_dbDoc = m_uiDoc.Document;
            m_curView = m_dbDoc.ActiveView;
            m_RoomIdList = roomIdList;
            m_TenkaiJoken = iTenkaiJoken;
            this.Text = R.Text.CMD_TENKAIVIEW;

            ProgressBar1.Minimum = 0;
            ProgressBar1.Maximum = m_RoomIdList.Count;
            ProgressBar1.Value = 0;
            lblMax.Text = string.Concat(m_RoomIdList.Count, R.Text.TXT_SLASH, m_RoomIdList.Count);

            btnStart.Click += btnStart_Click;
            btnStop.Click += btnStop_Click;
            btnCancel.Click += btnCancel_Click;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            m_isCanceled = true;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            lblInformation.Text = R.Text.LBL_INFO;
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            btnCancel.Enabled = false;
            m_isRunning = true;

            for (int i = 0; i < m_RoomIdList.Count; i++)
            {
                ElementId rmId = m_RoomIdList[i];

                lblMax.Text = string.Concat(i + 1, R.Text.TXT_SLASH, m_RoomIdList.Count);
                ProgressBar1.Value = i + 1;
                ProgressBar1.Refresh();
                lblMax.Refresh();
                lblInformation.Refresh();

                Application.DoEvents();
                if (m_isCanceled)
                {
                    MessageBox.Show(R.Text.INFO_OPERATIONCANCEL, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                }

                try
                {
                    RoomElevation.CreateElevation3(rmId, m_dbDoc, m_uiDoc, m_curView.Id, m_TenkaiJoken);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    m_isCanceled = true;
                }
                if (m_isCanceled)
                    break;
            }

            m_isRunning = false;

            if (!m_isCanceled)
                this.DialogResult = DialogResult.OK;
            else
                this.DialogResult = DialogResult.Cancel;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (m_isRunning)
            {
                m_isCanceled = true;
                return;
            }
            this.DialogResult = DialogResult.Cancel;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (m_isRunning)
            {
                m_isCanceled = true;
                e.Cancel = true;
                return;
            }
            base.OnFormClosing(e);
        }
    }
}

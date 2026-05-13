using ADSK.JExtRAC.ValueCopy.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RvtExtApp = ADSK.JExtRAC.ValueCopy;

namespace ADSK.JExtRAC.ValueCopy.UI
{
    /// ================================================================================
    /// <summary>FormReport</summary>
    ///
    /// <history>2022/01/10 Created Applied Technology</history>
    /// ================================================================================
    public partial class FormReport : Form
    {
        //Member Variables

        #region Member Variables

        /// <summary>Element</summary>
        private RvtExtApp.Components.Elements _CmpElements;

        /// <summary>Attributes</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>List ObjectReportCopy </summary>
        private List<ObjectReportCopy> _ObjectReports = null;

        /// <summary>StringBuilder</summary>
        private StringBuilder _ErrorMess;

        #endregion Member Variables

        // Constructor

        #region Constructor

        /// ================================================================================
        /// <summary>Constructor</summary>
        ///
        /// <param name="cmpElements">Elements</param>
        /// <param name="cmpAttribute">Attribute</param>
        /// <param name="ObjectReports">List ObjectReportCopy</param>
        /// <param name="errorMess">StringBuilder</param>
        ///
        /// <history>2022/01/10 Created Applied Technology</history>
        /// ================================================================================

        public FormReport(RvtExtApp.Components.Elements cmpElements, RvtExtApp.Components.Attribute cmpAttribute, List<ObjectReportCopy> ObjectReports, StringBuilder errorMess)
        {
            InitializeComponent();

            _CmpElements = cmpElements;
            _CmpAttribute = cmpAttribute;
            _ObjectReports = ObjectReports;
            _ErrorMess = errorMess;
        }

        #endregion Constructor

        // Member Functions

        #region Member Functions

        /// ================================================================================
        /// <summary>Form character setting</summary>
        ///
        /// <history>2022/01/10 Created Applied Technology</history>
        /// ================================================================================
        private void InitText()
        {
            // Set icon
            this.Icon = Resources.Image.IDI_SUBS_ICON;

            this.Text = _CmpAttribute.ResourceText("IDS_TXT_REPORTFORM");
            btOK.Text = _CmpAttribute.ResourceText("IDS_TXT_OK");
            btShowLog.Text = _CmpAttribute.ResourceText("IDS_TXT_ERRORDISPLAY");

            // Set header DataGridview
            dgvLog_ElementId.HeaderText = _CmpAttribute.ResourceText("IDS_TXT_INSTANCEIDFORM");
            dgvLog_FamilyName.HeaderText = _CmpAttribute.ResourceText("IDS_TXT_FAMILYSTRINGFORM");
            dgvLog_TypeName.HeaderText = _CmpAttribute.ResourceText("IDS_TXT_TYPESTRINGFORM");
            dgvLog_ParameterName.HeaderText = _CmpAttribute.ResourceText("IDS_TXT_PARAMETERNAMEFORM");
            dgvLog_IconStatus.HeaderText = _CmpAttribute.ResourceText("IDS_TXT_ICONSTATUSFORM");
            dgvLog_Status.HeaderText = _CmpAttribute.ResourceText("IDS_TXT_STATUSFORM");

            if (_ErrorMess.Length == 0)
                btShowLog.Enabled = false;
            else
                btShowLog.Enabled = true;
        }

        /// ================================================================================
        /// <summary>Form data setting</summary>
        ///
        /// <history>2022/01/10 Created Applied Technology</history>
        /// ================================================================================
        private void InitData()
        {
            if (_ObjectReports == null)
                return;

            string okMess = _CmpAttribute.ResourceText("IDS_TXT_STATUSOKSTRINGFORM");
            string failMess = _CmpAttribute.ResourceText("IDS_TXT_STATUSNOTOKSTRINGFORM");

            // Sort element
            _ObjectReports = _ObjectReports.OrderBy(x => x.FamilyNameElement).ThenBy(x => x.TypeNameElement).ThenBy(x => x.ElementCurrent.Id.IntegerValue).ToList();

            foreach (var objReport in _ObjectReports)
            {
                objReport.ObjectParameterData = objReport.ObjectParameterData.OrderBy(x => x.NameParameter).ToList();

                foreach (var objParameter in objReport.ObjectParameterData)
                {
                    // Copy = false
                    if (objParameter.IsCopy == false)
                        continue;

                    // Get string report
                    string strReport = GetStrFormStatusReport(objParameter.StatusCopyParameter);

                    // Get string X or O
                    var messCurrent = GetStatusOKOrNot(objParameter.StatusCopyParameter, okMess, failMess);

                    if (objReport.ElementCurrent.IsValidObject == false)
                        continue;

                    dgvLogData.Rows.Add(objReport.ElementCurrent.Id.ToString(), objReport.FamilyNameElement, objReport.TypeNameElement, objParameter.NameParameter, messCurrent, strReport);
                }
            }
        }

        /// ================================================================================
        /// <summary>Get Status OK Or Not</summary>
        ///
        /// <param name="statusCopy">StatusCopy</param>
        /// <param name="okMess">string message</param>
        /// <param name="failMess">string fail message</param>
        /// <returns></returns>
        ///
        /// <history>2022/01/10 Created Applied Technology</history>
        /// ================================================================================
        private string GetStatusOKOrNot(StatusCopy statusCopy, string okMess, string failMess)
        {
            if (statusCopy == StatusCopy.CS_Success)
                return okMess;
            else
                return failMess;
        }

        /// ================================================================================
        /// <summary>Get String Form Status Report</summary>
        ///
        /// <param name="statusCopy">StatusCopy</param>
        /// <returns></returns>
        ///
        /// <history>2022/01/10 Created Applied Technology</history>
        /// ================================================================================
        private string GetStrFormStatusReport(StatusCopy statusCopy)
        {
            switch (statusCopy)
            {
                case StatusCopy.CS_Success:
                    return _CmpAttribute.ResourceText("IDS_LOG_SUSSCESS");

                case StatusCopy.CS_CanFindParameter:
                    return _CmpAttribute.ResourceText("IDS_LOG_CANOTFINDPARAMETER");

                case StatusCopy.CS_OutOfRange:
                    return _CmpAttribute.ResourceText("IDS_LOG_OUTOFRANGE");

                case StatusCopy.CS_ReadOnlyOrRecipe:
                    return _CmpAttribute.ResourceText("IDS_LOG_READONLYORRECIPE");

                case StatusCopy.CS_CantCopy:
                    return _CmpAttribute.ResourceText("IDS_LOG_CANNOTCOPY");
            }

            return string.Empty;
        }

        /// ================================================================================
        /// <summary>Is The Same Cell Value</summary>
        ///
        /// <param name="row">index row</param>
        /// <returns></returns>
        ///
        /// <history>2022/01/10 Created Applied Technology</history>
        /// ================================================================================
        private bool IsTheSameCellValue(int row)
        {
            // Get data cell current and cell previous
            DataGridViewCell cell1 = dgvLogData[0, row];
            DataGridViewCell cell2 = dgvLogData[1, row];
            DataGridViewCell cell3 = dgvLogData[2, row];

            DataGridViewCell cell11 = dgvLogData[0, row - 1];
            DataGridViewCell cell21 = dgvLogData[1, row - 1];
            DataGridViewCell cell31 = dgvLogData[2, row - 1];

            // Don't have any value
            if (cell1.Value == null || cell2.Value == null || cell3.Value == null ||
                cell11.Value == null || cell21.Value == null || cell31.Value == null)
                return false;

            return (cell1.Value.ToString() == cell11.Value.ToString() &&
                    cell2.Value.ToString() == cell21.Value.ToString() &&
                    cell3.Value.ToString() == cell31.Value.ToString());
        }

        #endregion Member Functions

        // Events

        #region Events

        /// ================================================================================
        /// <summary>Handles the Load event of the FormReport control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2022/01/11 Created Applied Technology</history>
        /// ================================================================================
        private void FormReport_Load(object sender, EventArgs e)
        {
            // Init text data
            InitText();

            // Init report data
            InitData();
        }

        /// ================================================================================
        /// <summary>Handles the Cell Painting event of the dgvLogData control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="DataGridViewCellPaintingEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2022/01/11 Created Applied Technology</history>
        /// ================================================================================
        private void dgvLogData_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // Only col parameterId need to merge
            if (e.RowIndex < 0 || (e.ColumnIndex != 0 && e.ColumnIndex != 1 && e.ColumnIndex != 2))
                return;

            // Set border
            e.AdvancedBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;

            if (e.RowIndex < 1)
                return;

            if (IsTheSameCellValue(e.RowIndex))
                e.AdvancedBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
            else
                e.AdvancedBorderStyle.Top = dgvLogData.AdvancedCellBorderStyle.Top;
        }

        /// ================================================================================
        /// <summary>Handles the Cell Formatting event of the dgvLogData control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="DataGridViewCellFormattingEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2022/01/11 Created Applied Technology</history>
        /// ================================================================================
        private void dgvLogData_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex == 0 || (e.ColumnIndex != 0 && e.ColumnIndex != 1 && e.ColumnIndex != 2))
                return;

            // Set formating
            if (IsTheSameCellValue(e.RowIndex))
            {
                e.Value = string.Empty;
                e.FormattingApplied = true;
            }
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btShowLog control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2022/01/11 Created Applied Technology</history>
        /// ================================================================================
        private void btShowLog_Click(object sender, EventArgs e)
        {
            FormLog frm = new FormLog(_CmpAttribute, _ErrorMess);
            frm.ShowDialog();
        }

        #endregion Events
    }
}
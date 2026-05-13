using ADSK.JExtRAC.ValueCopy.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using RvtExtApp = ADSK.JExtRAC.ValueCopy;

namespace ADSK.JExtRAC.ValueCopy.UI
{
    /// ================================================================================
    /// <summary>FormReportWPF</summary>
    ///
    /// <history>2024/03/21 Created</history>
    /// ================================================================================
    public partial class FormReportWPF : Window
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
        /// <history>2024/03/21 Created</history>
        /// ================================================================================
        public FormReportWPF(RvtExtApp.Components.Elements cmpElements, RvtExtApp.Components.Attribute cmpAttribute, List<ObjectReportCopy> ObjectReports, StringBuilder errorMess)
        {
            InitializeComponent();
            btShowLog.Click += btShowLog_Click;

            _CmpElements = cmpElements;
            _CmpAttribute = cmpAttribute;
            _ObjectReports = ObjectReports;
            _ErrorMess = errorMess;

            InitText();
            InitData();
        }

        #endregion Constructor

        // Member Functions

        #region Member Functions

        /// ================================================================================
        /// <summary>Form character setting</summary>
        ///
        /// <history>2024/03/21 Created</history>
        /// ================================================================================
        private void InitText()
        {
            this.Title = _CmpAttribute.ResourceText("IDS_TXT_REPORTFORM");
            btOK.Content = _CmpAttribute.ResourceText("IDS_TXT_OK");
            btShowLog.Content = _CmpAttribute.ResourceText("IDS_TXT_ERRORDISPLAY");

            // Set header DataGrid
            dgvLog_ElementId.Header = _CmpAttribute.ResourceText("IDS_TXT_INSTANCEIDFORM");
            dgvLog_FamilyName.Header = _CmpAttribute.ResourceText("IDS_TXT_FAMILYSTRINGFORM");
            dgvLog_TypeName.Header = _CmpAttribute.ResourceText("IDS_TXT_TYPESTRINGFORM");
            dgvLog_ParameterName.Header = _CmpAttribute.ResourceText("IDS_TXT_PARAMETERNAMEFORM");
            dgvLog_IconStatus.Header = _CmpAttribute.ResourceText("IDS_TXT_ICONSTATUSFORM");
            dgvLog_Status.Header = _CmpAttribute.ResourceText("IDS_TXT_STATUSFORM");

            btShowLog.IsEnabled = _ErrorMess.Length > 0;
        }

        /// ================================================================================
        /// <summary>Form data setting</summary>
        ///
        /// <history>2024/03/21 Created</history>
        /// ================================================================================
        private void InitData()
        {
            if (_ObjectReports == null)
                return;

            string okMess = _CmpAttribute.ResourceText("IDS_TXT_STATUSOKSTRINGFORM");
            string failMess = _CmpAttribute.ResourceText("IDS_TXT_STATUSNOTOKSTRINGFORM");

            // Sort element
            _ObjectReports = _ObjectReports.OrderBy(x => x.FamilyNameElement).ThenBy(x => x.TypeNameElement).ThenBy(x => x.ElementCurrent.Id.ToString()).ToList();

            var items = new List<dynamic>();
            string previousElementId = null;
            string previousFamilyName = null;
            string previousTypeName = null;

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

                    var elementId = objReport.ElementCurrent.Id.ToString();
                    var familyName = objReport.FamilyNameElement;
                    var typeName = objReport.TypeNameElement;

                    // Check if we should show the values
                    if (elementId == previousElementId && 
                        familyName == previousFamilyName && 
                        typeName == previousTypeName)
                    {
                        elementId = "";
                        familyName = "";
                        typeName = "";
                    }

                    items.Add(new
                    {
                        ElementId = elementId,
                        FamilyName = familyName,
                        TypeName = typeName,
                        ParameterName = objParameter.NameParameter,
                        IconStatus = messCurrent,
                        Status = strReport
                    });

                    previousElementId = objReport.ElementCurrent.Id.ToString();
                    previousFamilyName = objReport.FamilyNameElement;
                    previousTypeName = objReport.TypeNameElement;
                }
            }

            dgvLogData.ItemsSource = items;
        }

        /// ================================================================================
        /// <summary>Get Status OK Or Not</summary>
        ///
        /// <param name="statusCopy">StatusCopy</param>
        /// <param name="okMess">string message</param>
        /// <param name="failMess">string fail message</param>
        /// <returns></returns>
        ///
        /// <history>2024/03/21 Created</history>
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
        /// <history>2024/03/21 Created</history>
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

        #endregion Member Functions

        // Events

        #region Events

        /// ================================================================================
        /// <summary>Handles the Click event of the btShowLog control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2024/03/21 Created</history>
        /// ================================================================================
        private void btShowLog_Click(object sender, RoutedEventArgs e)
        {
            var frm = new FormLogWPF(_CmpAttribute, _ErrorMess);
            frm.ShowDialog();
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btOK control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2024/03/21 Created</history>
        /// ================================================================================
        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        #endregion Events
    }
} 
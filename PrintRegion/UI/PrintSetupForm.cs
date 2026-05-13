using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using RvtExtApp = ADSK.JExtRAC.PrintRegion;
using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;

namespace ADSK.JExtRAC.PrintRegion.UI
{
    /// ================================================================================
    /// <summary>PrintSetupForm</summary>
    ///
    /// <history><p>2022/01/17 Created Applied Technology</p></history>
    /// ================================================================================
    public partial class PrintSetupForm : System.Windows.Forms.Form
    {
        //Member Variables

        #region Member Variables

        /// <summary>PrintMgr</summary>
        private PrintMgr m_printMgr;

        /// <summary>PrintSTP</summary>
        private PrintSTP m_printSetup;

        /// <summary>RvtExtApp.Components.Attribute</summary>
        private ExternalCommandData m_commandData;

        /// <summary>RvtExtApp.Components.Attribute</summary>
        private bool m_stopUpdateFlag;

        /// <summary>RvtExtApp.Components.Attribute</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        #endregion Member Variables

        //Consructor

        #region Constructor

        /// ================================================================================
        /// <summary>Set Text</summary>
        ///
        /// <param name="printSetup">PrintSTP</param>
        /// <param name="cmpAttribute">RvtExtApp.Components.Attribute</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public PrintSetupForm(PrintMgr printMgr, PrintSTP printSetup, RvtExtApp.Components.Attribute cmpAttribute)
        {
            InitializeComponent();
            m_printSetup = printSetup;
            _CmpAttribute = cmpAttribute;
            m_printMgr = printMgr;
            m_commandData = printMgr.m_commandData;
        }

        #endregion Constructor

        //Member functions

        #region Member functions

        /// ================================================================================
        /// <summary>Set Text</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_PRINT_SETUP");
            this.Icon = _CmpAttribute.ResourceImage("IDI_SUBS_ICON") as System.Drawing.Icon;

            this.lblPrinter.Text = _CmpAttribute.ResourceText("IDS_TXT_PRINTER_NAME") + " :";
            this.lblPrinterName.Text = m_printSetup.PrinterName;
            this.lblName.Text = _CmpAttribute.ResourceText("IDS_TXT_NAME") + "(N):";

            // Group box paper
            this.grbPaper.Text = _CmpAttribute.ResourceText("IDS_TXT_PAPER");
            this.lblSize.Text = _CmpAttribute.ResourceText("IDS_TXT_SIZE") + "(I):";
            this.lblSource.Text = _CmpAttribute.ResourceText("IDS_TXT_SOURCE") + "(O):";

            // Group box orientation
            this.grbOrientation.Text = _CmpAttribute.ResourceText("IDS_TXT_ORIENTATION");
            this.rdbPortrait.Text = _CmpAttribute.ResourceText("IDS_TXT_PORTRAIT") + "(&P)";
            this.rdbLandscape.Text = _CmpAttribute.ResourceText("IDS_TXT_LANDSCAPE") + "(&L)";

            //Group box paper placement
            this.grbPaperPlacement.Text = _CmpAttribute.ResourceText("IDS_TXT_PAPER_PLACEMENT");
            this.rdbCenter.Text = _CmpAttribute.ResourceText("IDS_TXT_CENTER") + "(&C)";
            this.rdbOffsetFromConer.Text = _CmpAttribute.ResourceText("IDS_TXT_OFFSET_FROM_CONER") + "(&M)" + ":";

            // Group box hidden line views
            this.grbHiddenLineViews.Text = _CmpAttribute.ResourceText("IDS_TXT_HIDDEN_LINES_VIEWS");
            this.lblRemoveLlinesUsing.Text = _CmpAttribute.ResourceText("IDS_TXT_REMOVE_LINES_USING");
            this.rdbVectorProcessing.Text = _CmpAttribute.ResourceText("IDS_TXT_VECTOR_PROCESSING") + "(&E)";
            this.rdbRasterProcessing.Text = _CmpAttribute.ResourceText("IDS_TXT_RASTER_PROCESSING") + "(&G)";

            // Group box zoom
            this.grbZoom.Text = _CmpAttribute.ResourceText("IDS_TXT_ZOOM");
            this.rdbFitToPage.Text = _CmpAttribute.ResourceText("IDS_TXT_FIT_TO_PAGE") + "(&F)";
            this.rdbZoom.Text = _CmpAttribute.ResourceText("IDS_TXT_ZOOM") + "(&Z)" + ":";
            this.lblSizeZoom.Text = _CmpAttribute.ResourceText("IDS_TXT_SIZE") + "(%)";

            // Group box appearance
            this.grbAppearance.Text = _CmpAttribute.ResourceText("IDS_TXT_APPEARANCE");
            this.lblRasterQuality.Text = _CmpAttribute.ResourceText("IDS_TXT_RASTER_QUALITY") + "(Q):";
            this.lblColors.Text = _CmpAttribute.ResourceText("IDS_TXT_COLORS") + "(R):";

            // Group box options

            this.grbOptions.Text = _CmpAttribute.ResourceText("IDS_TXT_OPTION");
            this.ckbViewLinksInBlue.Text = _CmpAttribute.ResourceText("IDS_TXT_VIEW_LINKS_IN_BLUE") + "(&Y)"; ;
            this.ckbHideRefWorkPlanes.Text = _CmpAttribute.ResourceText("IDS_TXT_HIDE_REF_WORK_PLANES") + "(&W)"; ;
            this.ckbHideUnreferencedViewTags.Text = _CmpAttribute.ResourceText("IDS_TXT_HIDE_UNREFERENCED_VIEW_TAGS") + "(&U)"; ;
            this.ckbHideScopeBoxed.Text = _CmpAttribute.ResourceText("IDS_TXT_HIDE_SCOPE_BOXED") + "(&X)"; ;
            this.ckbHideCropBoundaries.Text = _CmpAttribute.ResourceText("IDS_TXT_HIDE_CROP_BOUNDARIES") + "(&B)"; ;
            this.ckbRegionEdgesMaskCoincidentLines.Text = _CmpAttribute.ResourceText("IDS_TXT_REGION_EDGES_MASK_COINCIDENT_LINES") + "(&K)"; ;
            this.ckbReplaceHafttoneWithThinLines.Text = _CmpAttribute.ResourceText("IDS_TXT_REPLACE_HALFTONE_WITH_THIN_LINES") + "(&H)"; ;

            this.btSave.Text = _CmpAttribute.ResourceText("IDS_TXT_SAVE") + "(&S)";
            this.btSaveAs.Text = _CmpAttribute.ResourceText("IDS_TXT_SAVE_AS") + "(&V)";
            this.btRevert.Text = _CmpAttribute.ResourceText("IDS_TXT_REVERT") + "(&T)";
            this.btRename.Text = _CmpAttribute.ResourceText("IDS_TXT_RENAME") + "(&A)";
            this.btDelete.Text = _CmpAttribute.ResourceText("IDS_TXT_DELETE") + "(&D)";

            this.btOK.Text = _CmpAttribute.ResourceText("IDS_TXT_OK");
            this.btCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");
        }

        /// ================================================================================
        /// <summary>Set Data</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void SetData()
        {
            printSetupsComboBox.DataSource = m_printSetup.PrintSettingNames;
            printSetupsComboBox.SelectedItem = m_printSetup.SettingName;
            this.printSetupsComboBox.SelectedValueChanged += new System.EventHandler(this.printSetupsComboBox_SelectedValueChanged);

            btRename.Enabled = btDelete.Enabled = !IsInSection();

            var paperSize = m_printSetup.PaperSizes;
            var listname = paperSize.OrderBy(x => x).ToList();
            this.cboSizePaper.DataSource = listname;
            this.cboSizePaper.SelectedItem = m_printSetup.PaperSize;
            this.cboSizePaper.SelectedValueChanged += new System.EventHandler(this.cboSizePaper_SelectedValueChanged);

            cboSourcePaper.DataSource = m_printSetup.PaperSources;
            cboSourcePaper.SelectedItem = m_printSetup.PaperSource;
            this.cboSourcePaper.SelectedValueChanged += new System.EventHandler(this.cboSourcePaper_SelectedValueChanged);

            if (m_printSetup.PageOrientation == PageOrientationType.Landscape)
            {
                rdbLandscape.Checked = true;

                this.picBoxOrientation.Image = _CmpAttribute.ResourceImage("IDI_PIC_LANDSCAPE") as System.Drawing.Image;
            }
            else
            {
                rdbPortrait.Checked = true;

                this.picBoxOrientation.Image = _CmpAttribute.ResourceImage("IDI_PIC_PORTAIT") as System.Drawing.Image;
            }
            this.rdbLandscape.CheckedChanged += new System.EventHandler(this.rdbLandscape_CheckedChanged);
            this.rdbPortrait.CheckedChanged += new System.EventHandler(this.rdbPortrait_CheckedChanged);

            cboMarginType.DataSource = m_printSetup.MarginTypes;

            this.rdbOffsetFromConer.CheckedChanged += new System.EventHandler(this.rdbOffsetFromConer_CheckedChanged);
            this.rdbCenter.CheckedChanged += new System.EventHandler(this.rdbCenter_CheckedChanged);
            this.txtUserDefinedMarginY.TextChanged += new System.EventHandler(this.txtUserDefinedMarginY_TextChanged);
            this.txtUserDefinedMarginX.TextChanged += new System.EventHandler(this.txtUserDefinedMarginX_TextChanged);

            SetValueForMarginTypeCombobox(m_printSetup.SelectedMarginType);

            this.cboMarginType.SelectedValueChanged += new System.EventHandler(this.cboMarginType_SelectedValueChanged);

            if (m_printSetup.PaperPlacement == PaperPlacementType.Center)
            {
                rdbCenter.Checked = true;
                rdbOffsetFromConer.Checked = false;
            }
            else
            {
                rdbOffsetFromConer.Checked = true;
                rdbCenter.Checked = false;
            }

            if (m_printSetup.HiddenLineViews == HiddenLineViewsType.RasterProcessing)
            {
                rdbRasterProcessing.Checked = true;
            }
            else
            {
                rdbVectorProcessing.Checked = true;
            }
            this.rdbRasterProcessing.CheckedChanged += new System.EventHandler(this.rdbRasterProcessing_CheckedChanged);
            this.rdbVectorProcessing.CheckedChanged += new System.EventHandler(this.rdbVectorProcessing_CheckedChanged);

            ShowHideZoomSize(false);
            if (m_printSetup.ZoomType == ZoomType.Zoom)
            {
                rdbZoom.Checked = true;
                zoomPercentNumericUpDown.Value = m_printSetup.Zoom;
                ShowHideZoomSize(true);
            }
            else
            {
                rdbFitToPage.Checked = true;
            }

            this.rdbZoom.CheckedChanged += new System.EventHandler(this.rdbZoom_CheckedChanged);
            this.rdbFitToPage.CheckedChanged += new System.EventHandler(this.rdbFitToPage_CheckedChanged);

            cboRasterQuality.DataSource = m_printSetup.RasterQualities;
            SetValueForRasterQualityCombobox(m_printSetup.RasterQuality);
            this.cboRasterQuality.SelectedValueChanged += new System.EventHandler(this.cboRasterQuality_SelectedValueChanged);

            cboColors.DataSource = m_printSetup.Colors;
            SetValueForColorsCombobox(m_printSetup.Color);

            this.cboColors.SelectedValueChanged += new System.EventHandler(this.cboColors_SelectedValueChanged);

            ckbViewLinksInBlue.Checked = m_printSetup.ViewLinksinBlue;
            this.ckbViewLinksInBlue.CheckedChanged += new System.EventHandler(this.ckbViewLinksInBlue_CheckedChanged);

            ckbHideScopeBoxed.Checked = m_printSetup.HideScopeBoxes;
            this.ckbHideScopeBoxed.CheckedChanged += new System.EventHandler(this.ckbHideScopeBoxed_CheckedChanged);

            ckbHideRefWorkPlanes.Checked = m_printSetup.HideReforWorkPlanes;
            this.ckbHideRefWorkPlanes.CheckedChanged += new System.EventHandler(this.ckbHideRefWorkPlanes_CheckedChanged);

            ckbHideCropBoundaries.Checked = m_printSetup.HideCropBoundaries;
            this.ckbHideCropBoundaries.CheckedChanged += new System.EventHandler(this.ckbHideCropBoundaries_CheckedChanged);

            ckbHideUnreferencedViewTags.Checked = m_printSetup.HideUnreferencedViewTags;
            this.ckbHideUnreferencedViewTags.CheckedChanged += new System.EventHandler(this.ckbHideUnreferencedViewTags_CheckedChanged);

            ckbRegionEdgesMaskCoincidentLines.Checked = m_printSetup.RegionEdgesMaskCoincidentLines;
            this.ckbRegionEdgesMaskCoincidentLines.CheckedChanged += new System.EventHandler(this.ckbRegionEdgesMaskCoincidentLines_CheckedChanged);

            ckbReplaceHafttoneWithThinLines.Checked = m_printSetup.ReplaceHaftoneWithThinLines;
            this.ckbReplaceHafttoneWithThinLines.CheckedChanged += new System.EventHandler(this.ckbReplaceHafttoneWithThinLines_CheckedChanged);

            btRevert.Enabled = false;
            btSave.Enabled = false;
        }

        #endregion Member functions

        // Event

        #region Event

        /// ================================================================================
        /// <summary>Load form</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void PrintSetupForm_Load(object sender, EventArgs e)
        {
            SetText();
            SetData();
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btSave control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void btSave_Click(object sender, EventArgs e)
        {
            m_printSetup.Save();
            btSave.Enabled = false;
        }

        /// ================================================================================
        /// <summary>Handles the selected value changed event of the printSetupsComboBox control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void printSetupsComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (m_stopUpdateFlag)
                return;

            btRename.Enabled = btDelete.Enabled = !IsInSection();

            m_printSetup.SettingName = printSetupsComboBox.SelectedItem as string;

            cboSizePaper.SelectedItem = m_printSetup.PaperSize;
            cboSourcePaper.SelectedItem = m_printSetup.PaperSource;

            // Page Orientation
            if (m_printSetup.PageOrientation == PageOrientationType.Landscape)
                rdbLandscape.Checked = true;
            else
                rdbPortrait.Checked = true;

            //Paper Placement
            if (m_printSetup.PaperPlacement == PaperPlacementType.Center)
                rdbCenter.Checked = true;
            else
                rdbOffsetFromConer.Checked = true;

            if (m_printSetup.VerifyMarginType(cboMarginType))
            {
                SetValueForMarginTypeCombobox(m_printSetup.SelectedMarginType);
            }

            // HiddenLineViewsType
            if (m_printSetup.HiddenLineViews == HiddenLineViewsType.RasterProcessing)
                rdbRasterProcessing.Checked = true;
            else
                rdbVectorProcessing.Checked = true;

            //ZoomType
            if (m_printSetup.ZoomType == ZoomType.Zoom)
            {
                rdbZoom.Checked = true;
                zoomPercentNumericUpDown.Value = m_printSetup.Zoom;
                this.zoomPercentNumericUpDown.Visible = true;
                this.lblSizeZoom.Visible = true;
            }
            else
            {
                rdbFitToPage.Checked = true;
                m_printSetup.ZoomType = ZoomType.Zoom;
                zoomPercentNumericUpDown.Value = m_printSetup.Zoom;
                m_printSetup.ZoomType = ZoomType.FitToPage;
            }

            // Group box appearance
            SetValueForRasterQualityCombobox(m_printSetup.RasterQuality);

            SetValueForColorsCombobox(m_printSetup.Color);

            ckbViewLinksInBlue.Checked = m_printSetup.ViewLinksinBlue;
            ckbHideScopeBoxed.Checked = m_printSetup.HideScopeBoxes;
            ckbHideRefWorkPlanes.Checked = m_printSetup.HideReforWorkPlanes;
            ckbHideCropBoundaries.Checked = m_printSetup.HideCropBoundaries;
            ckbHideUnreferencedViewTags.Checked = m_printSetup.HideUnreferencedViewTags;
            ckbRegionEdgesMaskCoincidentLines.Checked = m_printSetup.RegionEdgesMaskCoincidentLines;
            ckbReplaceHafttoneWithThinLines.Checked = m_printSetup.ReplaceHaftoneWithThinLines;

            btRevert.Enabled = false;
            btSave.Enabled = false;

            ShowHideZoomSize(rdbZoom.Checked);
        }

        /// ================================================================================
        /// <summary>Handles the SelectedValueChanged event of the paperSizeComboBox control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void cboSizePaper_SelectedValueChanged(object sender, EventArgs e)
        {
            m_printSetup.PaperSize = cboSizePaper.SelectedItem as string;

            if (!IsInSection())
            {
                if (!btRevert.Enabled)
                    btRevert.Enabled = true;

                if (!btSave.Enabled)
                    btSave.Enabled = true;
            }
        }

        /// ================================================================================
        /// <summary>Handles the SelectedValueChanged event of the paperSourceComboBox control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void cboSourcePaper_SelectedValueChanged(object sender, EventArgs e)
        {
            m_printSetup.PaperSource = cboSourcePaper.SelectedItem as string;
            if (!IsInSection())
            {
                if (!btRevert.Enabled)
                    btRevert.Enabled = true;

                if (!btSave.Enabled)
                    btSave.Enabled = true;
            }
        }

        /// ================================================================================
        /// <summary>Handles the checked changed event of the rdbPortrait control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void rdbPortrait_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbPortrait.Checked)
            {
                m_printSetup.PageOrientation = PageOrientationType.Portrait;

                this.picBoxOrientation.Image = _CmpAttribute.ResourceImage("IDI_PIC_PORTAIT") as System.Drawing.Image;
            }
            if (!IsInSection())
            {
                if (!btRevert.Enabled)
                    btRevert.Enabled = true;

                if (!btSave.Enabled)
                    btSave.Enabled = true;
            }
        }

        /// ================================================================================
        /// <summary>Handles the checked changed event of the rdbLandscape control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void rdbLandscape_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbLandscape.Checked)
            {
                m_printSetup.PageOrientation = PageOrientationType.Landscape;

                this.picBoxOrientation.Image = _CmpAttribute.ResourceImage("IDI_PIC_LANDSCAPE") as System.Drawing.Image;
            }
            if (!IsInSection())
            {
                if (!btRevert.Enabled)
                    btRevert.Enabled = true;

                if (!btSave.Enabled)
                    btSave.Enabled = true;
            }
        }

        /// ================================================================================
        /// <summary>Handles the checked changed event of the rdbCenter control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void rdbCenter_CheckedChanged(object sender, EventArgs e)
        {
            if (!rdbCenter.Checked)
                return;

            m_printSetup.PaperPlacement = PaperPlacementType.Center;

            m_printSetup.VerifyMarginType(cboMarginType);

            this.txtUserDefinedMarginX.Enabled = false;
            this.txtUserDefinedMarginY.Enabled = false;

            if (!IsInSection())
            {
                if (!btRevert.Enabled)
                    btRevert.Enabled = true;

                if (!btSave.Enabled)
                    btSave.Enabled = true;
            }
        }

        /// ================================================================================
        /// <summary>Handles the checked changed event of the rdbOffsetFromConer control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void rdbOffsetFromConer_CheckedChanged(object sender, EventArgs e)
        {
            if (!rdbOffsetFromConer.Checked)
            {
                m_stopUpdateFlag = true;
                SetValueForMarginTypeCombobox(MarginType.PrinterLimit);
                m_printSetup.SelectedMarginType = MarginType.PrinterLimit;

                DisplayOffset(ref txtUserDefinedMarginX, UnitUtils.ConvertFromInternalUnits(0, UnitTypeId.Millimeters));
                DisplayOffset(ref txtUserDefinedMarginY, UnitUtils.ConvertFromInternalUnits(0, UnitTypeId.Millimeters));
                m_stopUpdateFlag = false;
                return;
            }

            m_printSetup.PaperPlacement = PaperPlacementType.LowerLeft;

            m_printSetup.VerifyMarginType(cboMarginType);

            System.Collections.ObjectModel.Collection<System.Windows.Forms.Control> controlsToEnableOrNot =
                new System.Collections.ObjectModel.Collection<System.Windows.Forms.Control>();
            controlsToEnableOrNot.Add(txtUserDefinedMarginX);
            controlsToEnableOrNot.Add(txtUserDefinedMarginY);
            if (m_printSetup.VerifyUserDefinedMargin(controlsToEnableOrNot))
            {
                DisplayOffset(ref txtUserDefinedMarginX, UnitUtils.ConvertFromInternalUnits(m_printSetup.OriginOffsetX, UnitTypeId.Millimeters));
                DisplayOffset(ref txtUserDefinedMarginY, UnitUtils.ConvertFromInternalUnits(m_printSetup.OriginOffsetY, UnitTypeId.Millimeters));
            }

            if (!IsInSection())
            {
                if (!btRevert.Enabled)
                    btRevert.Enabled = true;

                if (!btSave.Enabled)
                    btSave.Enabled = true;
            }
        }

        /// ================================================================================
        /// <summary>Handles the SelectedValueChanged event of the marginTypeComboBox control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void cboMarginType_SelectedValueChanged(object sender, EventArgs e)
        {
            if (m_stopUpdateFlag)
                return;

            //m_printSetup.SelectedMarginType = (MarginType)cboMarginType.SelectedItem;
            if (cboMarginType.SelectedIndex == 0)
                m_printSetup.SelectedMarginType = MarginType.NoMargin;
            else if (cboMarginType.SelectedIndex == 1)
                m_printSetup.SelectedMarginType = MarginType.PrinterLimit;
            else if (cboMarginType.SelectedIndex == 2)
                m_printSetup.SelectedMarginType = MarginType.UserDefined;

            System.Collections.ObjectModel.Collection<System.Windows.Forms.Control> controlsToEnableOrNot =
                new System.Collections.ObjectModel.Collection<System.Windows.Forms.Control>();
            controlsToEnableOrNot.Add(txtUserDefinedMarginX);
            controlsToEnableOrNot.Add(txtUserDefinedMarginY);
            if (m_printSetup.VerifyUserDefinedMargin(controlsToEnableOrNot))
            {
                DisplayOffset(ref txtUserDefinedMarginX, UnitUtils.ConvertFromInternalUnits(m_printSetup.OriginOffsetX, UnitTypeId.Millimeters));
                DisplayOffset(ref txtUserDefinedMarginY, UnitUtils.ConvertFromInternalUnits(m_printSetup.OriginOffsetY, UnitTypeId.Millimeters));
            }

            if (!btRevert.Enabled)
                btRevert.Enabled = true;

            if (!btSave.Enabled)
                btSave.Enabled = true;
        }

        /// ================================================================================
        /// <summary>Handles the checked changed event of the rdbVectorProcessing control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void rdbVectorProcessing_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbVectorProcessing.Checked)
            {
                m_printSetup.HiddenLineViews = HiddenLineViewsType.VectorProcessing;

                if (!IsInSection())
                {
                    if (!btRevert.Enabled)
                        btRevert.Enabled = true;

                    if (!btSave.Enabled)
                        btSave.Enabled = true;
                }
            }
        }

        /// ================================================================================
        /// <summary>Handles the checked changed event of the rdbRasterProcessing control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void rdbRasterProcessing_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbRasterProcessing.Checked)
            {
                m_printSetup.HiddenLineViews = HiddenLineViewsType.RasterProcessing;

                if (!IsInSection())
                {
                    if (!btRevert.Enabled)
                        btRevert.Enabled = true;

                    if (!btSave.Enabled)
                        btSave.Enabled = true;
                }
            }
        }

        /// ================================================================================
        /// <summary>Handles the checked changed event of the rdbFitToPage control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void rdbFitToPage_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbFitToPage.Checked)
            {
                m_printSetup.ZoomType = ZoomType.FitToPage;
                rdbCenter.Checked = true;

                if (!IsInSection())
                {
                    if (!btRevert.Enabled)
                        btRevert.Enabled = true;

                    if (!btSave.Enabled)
                        btSave.Enabled = true;
                }
            }

            ShowHideZoomSize(rdbZoom.Checked);
        }

        /// ================================================================================
        /// <summary>Handles the checked changed event of the rdbZoom control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void rdbZoom_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbZoom.Checked)
            {
                m_printSetup.ZoomType = ZoomType.Zoom;
                rdbOffsetFromConer.Checked = true;
                m_printSetup.Zoom = (int)zoomPercentNumericUpDown.Value;
                if (!IsInSection())
                {
                    if (!btRevert.Enabled)
                        btRevert.Enabled = true;

                    if (!btSave.Enabled)
                        btSave.Enabled = true;
                }
            }

            ShowHideZoomSize(rdbZoom.Checked);
        }

        /// ================================================================================
        /// <summary>Handles the value changed event of the zoomPercentNumericUpDown control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void zoomPercentNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (rdbZoom.Checked)
            {
                m_printSetup.Zoom = (int)zoomPercentNumericUpDown.Value;

                if (!IsInSection())
                {
                    if (!btRevert.Enabled)
                        btRevert.Enabled = true;

                    if (!btSave.Enabled)
                        btSave.Enabled = true;
                }
            }
        }

        /// ================================================================================
        /// <summary>Handles the selectedValueChanged event of the rasterQualityComboBox control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void cboRasterQuality_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cboRasterQuality.SelectedIndex == 0)
                m_printSetup.RasterQuality = RasterQualityType.Low;
            else if (cboRasterQuality.SelectedIndex == 1)
                m_printSetup.RasterQuality = RasterQualityType.Medium;
            else if (cboRasterQuality.SelectedIndex == 2)
                m_printSetup.RasterQuality = RasterQualityType.High;
            else if (cboRasterQuality.SelectedIndex == 3)
                m_printSetup.RasterQuality = RasterQualityType.Presentation;

            if (!IsInSection())
            {
                if (!btRevert.Enabled)
                    btRevert.Enabled = true;

                if (!btSave.Enabled)
                    btSave.Enabled = true;
            }
        }

        /// ================================================================================
        /// <summary>Handles the selectedValueChanged event of the colorsComboBox control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void cboColors_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cboColors.SelectedIndex == 0)
                m_printSetup.Color = ColorDepthType.BlackLine;
            else if (cboColors.SelectedIndex == 1)
                m_printSetup.Color = ColorDepthType.GrayScale;
            else if (cboColors.SelectedIndex == 2)
                m_printSetup.Color = ColorDepthType.Color;

            if (!IsInSection())
            {
                if (!btRevert.Enabled)
                    btRevert.Enabled = true;

                if (!btSave.Enabled)
                    btSave.Enabled = true;
            }
        }

        /// ================================================================================
        /// <summary>Handles the checked changed event of the ckbViewLinksInBlue control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void ckbViewLinksInBlue_CheckedChanged(object sender, EventArgs e)
        {
            m_printSetup.ViewLinksinBlue = ckbViewLinksInBlue.Checked;

            if (!IsInSection())
            {
                if (!btRevert.Enabled)
                    btRevert.Enabled = true;

                if (!btSave.Enabled)
                    btSave.Enabled = true;
            }
        }

        /// ================================================================================
        /// <summary>Handles the checked changed event of the ckbHideScopeBoxed control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void ckbHideScopeBoxed_CheckedChanged(object sender, EventArgs e)
        {
            m_printSetup.HideScopeBoxes = ckbHideScopeBoxed.Checked;

            if (!IsInSection())
            {
                if (!btRevert.Enabled)
                    btRevert.Enabled = true;

                if (!btSave.Enabled)
                    btSave.Enabled = true;
            }
        }

        /// ================================================================================
        /// <summary>Handles the checked changed event of the ckbHideRefWorkPlanes control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void ckbHideRefWorkPlanes_CheckedChanged(object sender, EventArgs e)
        {
            m_printSetup.HideReforWorkPlanes = ckbHideRefWorkPlanes.Checked;

            if (!IsInSection())
            {
                if (!btRevert.Enabled)
                    btRevert.Enabled = true;

                if (!btSave.Enabled)
                    btSave.Enabled = true;
            }
        }

        /// ================================================================================
        /// <summary>Handles the checked changed event of the ckbHideCropBoundaries control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void ckbHideCropBoundaries_CheckedChanged(object sender, EventArgs e)
        {
            m_printSetup.HideCropBoundaries = ckbHideCropBoundaries.Checked;

            if (!IsInSection())
            {
                if (!btRevert.Enabled)
                    btRevert.Enabled = true;

                if (!btSave.Enabled)
                    btSave.Enabled = true;
            }
        }

        /// ================================================================================
        /// <summary>Handles the checked changed event of the ckbHideUnreferencedViewTags control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void ckbHideUnreferencedViewTags_CheckedChanged(object sender, EventArgs e)
        {
            m_printSetup.HideUnreferencedViewTags = ckbHideUnreferencedViewTags.Checked;
            if (!IsInSection())
            {
                if (!btRevert.Enabled)
                    btRevert.Enabled = true;

                if (!btSave.Enabled)
                    btSave.Enabled = true;
            }
        }

        /// ================================================================================
        /// <summary>Handles the checked changed event of the ckbRegionEdgesMaskCoincidentLines control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void ckbRegionEdgesMaskCoincidentLines_CheckedChanged(object sender, EventArgs e)
        {
            m_printSetup.RegionEdgesMaskCoincidentLines = ckbRegionEdgesMaskCoincidentLines.Checked;

            if (!IsInSection())
            {
                if (!btRevert.Enabled)
                    btRevert.Enabled = true;

                if (!btSave.Enabled)
                    btSave.Enabled = true;
            }
        }

        /// ================================================================================
        /// <summary>Handles the checked changed event of the ckbReplaceHafttoneWithThinLines control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void ckbReplaceHafttoneWithThinLines_CheckedChanged(object sender, EventArgs e)
        {
            m_printSetup.ReplaceHaftoneWithThinLines = ckbReplaceHafttoneWithThinLines.Checked;

            if (!IsInSection())
            {
                if (!btRevert.Enabled)
                    btRevert.Enabled = true;

                if (!btSave.Enabled)
                    btSave.Enabled = true;
            }
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnOK control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void txtUserDefinedMarginX_TextChanged(object sender, EventArgs e)
        {
            if (m_stopUpdateFlag)
                return;

            double doubleValue;
            if (!double.TryParse(txtUserDefinedMarginX.Text, out doubleValue))
            {
                PrintMgr.MyMessageBox(_CmpAttribute.ResourceText("IDS_TXT_INVALID_INPUT"));
                return;
            }
            //m_printSetup.OriginOffsetX = doubleValue / INCHES_IN_FEET;
            m_printSetup.OriginOffsetX = UnitUtils.ConvertToInternalUnits(doubleValue, UnitTypeId.Millimeters);

            if (!IsInSection())
            {
                if (!btRevert.Enabled)
                    btRevert.Enabled = true;

                if (!btSave.Enabled)
                    btSave.Enabled = true;
            }
        }

        /// ================================================================================
        /// <summary>Handles the text changed event of the btnOK control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void txtUserDefinedMarginY_TextChanged(object sender, EventArgs e)
        {
            if (m_stopUpdateFlag)
                return;

            double doubleValue;
            if (!double.TryParse(txtUserDefinedMarginY.Text, out doubleValue))
            {
                PrintMgr.MyMessageBox(_CmpAttribute.ResourceText("IDS_TXT_INVALID_INPUT"));
                return;
            }

            m_printSetup.OriginOffsetY = UnitUtils.ConvertToInternalUnits(doubleValue, UnitTypeId.Millimeters);

            if (!IsInSection())
            {
                if (!btRevert.Enabled)
                    btRevert.Enabled = true;

                if (!btSave.Enabled)
                    btSave.Enabled = true;
            }
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btSaveAs control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void btSaveAs_Click(object sender, EventArgs e)
        {
            using (SaveAsForm dlg = new SaveAsForm(m_printSetup, _CmpAttribute))
            {
                dlg.ShowDialog();
            }

            m_stopUpdateFlag = true;
            printSetupsComboBox.DataSource = m_printSetup.PrintSettingNames;
            printSetupsComboBox.Update();
            m_stopUpdateFlag = false;

            printSetupsComboBox.SelectedItem = m_printSetup.SettingName;
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btRename control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void btRename_Click(object sender, EventArgs e)
        {
            using (ReNameForm dlg = new ReNameForm(m_printSetup, _CmpAttribute))
            {
                dlg.ShowDialog();
            }

            m_stopUpdateFlag = true;
            printSetupsComboBox.DataSource = m_printSetup.PrintSettingNames;
            printSetupsComboBox.Update();
            m_stopUpdateFlag = false;

            printSetupsComboBox.SelectedItem = m_printSetup.SettingName;
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btRevert control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void btRevert_Click(object sender, EventArgs e)
        {
            m_printSetup.Revert();

            printSetupsComboBox_SelectedValueChanged(null, null);
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btDelete control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void btDelete_Click(object sender, EventArgs e)
        {
            try
            {
                bool isSuceff = m_printSetup.Delete();

                if (isSuceff)
                {
                    m_stopUpdateFlag = true;
                    printSetupsComboBox.DataSource = m_printSetup.PrintSettingNames;
                    printSetupsComboBox.Update();
                    m_stopUpdateFlag = false;

                    if (printSetupsComboBox.Items.Count > 0)
                    {
                        for (int i = 0; i < printSetupsComboBox.Items.Count; i++)
                        {
                            if ((printSetupsComboBox.Items[i] as string).Equals("<In-Session>") || (printSetupsComboBox.Items[i] as string).Equals(ConstData.InSessionName))
                            {
                                printSetupsComboBox.SelectedIndex = i;
                                printSetupsComboBox_SelectedValueChanged(null, null);
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var mess = ex.Message;
            }
        }

        /// ================================================================================
        /// <summary>Display offset value on TextBox</summary>
        /// <param name="textBox">Textbox</param>
        /// <param name="value">Value</param>
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void DisplayOffset(ref System.Windows.Forms.TextBox textBox, double value)
        {
            string str = string.Format("{0:0.0000}", value);

            textBox.Text = str;
        }

        /// ================================================================================
        /// <summary> Show or hide zoom size controls</summary>
        /// <param name="isShow">Show or hide</param>
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void ShowHideZoomSize(bool isShow)
        {
            zoomPercentNumericUpDown.Visible = isShow;
            lblSizeZoom.Visible = isShow;
        }

        /// ================================================================================
        /// <summary>Check current setup combobox is In Section</summary>
        /// <returns>True or False</returns>
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private bool IsInSection()
        {
            bool flag = false;
            if ((printSetupsComboBox.SelectedItem as string).Equals("<In-Session>") || (printSetupsComboBox.SelectedItem as string).Equals(ConstData.InSessionName))
                flag = true;

            return flag;
        }

        /// ================================================================================
        /// <summary>Set value for MarginType Combobox</summary>
        /// <param name="type"></param>
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void SetValueForMarginTypeCombobox(MarginType type)
        {
            if (cboMarginType.Items.Count == 0)
                cboMarginType.DataSource = m_printSetup.MarginTypes;

            if (type == MarginType.NoMargin)
                cboMarginType.SelectedIndex = 0;
            else if (type == MarginType.PrinterLimit)
                cboMarginType.SelectedIndex = 1;
            else if (type == MarginType.UserDefined)
                cboMarginType.SelectedIndex = 2;
        }

        /// ================================================================================
        /// <summary> Set value for RasterQuality Combobox</summary>
        /// <param name="type"></param>
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void SetValueForRasterQualityCombobox(RasterQualityType type)
        {
            if (cboMarginType.Items.Count == 0)
                cboRasterQuality.DataSource = m_printSetup.RasterQualities;

            if (type == RasterQualityType.Low)
                cboRasterQuality.SelectedIndex = 0;
            else if (type == RasterQualityType.Medium)
                cboRasterQuality.SelectedIndex = 1;
            else if (type == RasterQualityType.High)
                cboRasterQuality.SelectedIndex = 2;
            else if (type == RasterQualityType.Presentation)
                cboRasterQuality.SelectedIndex = 3;
        }

        /// ================================================================================
        /// <summary> Set value for colors combobox</summary>
        /// <param name="type">Color type</param>
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void SetValueForColorsCombobox(ColorDepthType type)
        {
            if (cboColors.Items.Count == 0)
                cboColors.DataSource = m_printSetup.Colors;

            if (type == ColorDepthType.BlackLine)
                cboColors.SelectedIndex = 0;
            else if (type == ColorDepthType.GrayScale)
                cboColors.SelectedIndex = 1;
            else if (type == ColorDepthType.Color)
                cboColors.SelectedIndex = 2;
        }

        /// ================================================================================
        /// <summary> Keydown event</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void PrintSetupForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Q)
                cboRasterQuality.Focus();

            if (e.KeyCode == Keys.R)
                cboColors.Focus();

            if (e.KeyCode == Keys.N)
                printSetupsComboBox.Focus();
        }

        #endregion Event
    }
}
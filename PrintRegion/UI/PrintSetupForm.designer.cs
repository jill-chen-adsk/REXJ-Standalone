
namespace ADSK.JExtRAC.PrintRegion.UI
{
    partial class PrintSetupForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblPrinter = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.lblPrinterName = new System.Windows.Forms.Label();
            this.printSetupsComboBox = new System.Windows.Forms.ComboBox();
            this.grbPaper = new System.Windows.Forms.GroupBox();
            this.lblSource = new System.Windows.Forms.Label();
            this.lblSize = new System.Windows.Forms.Label();
            this.cboSourcePaper = new System.Windows.Forms.ComboBox();
            this.cboSizePaper = new System.Windows.Forms.ComboBox();
            this.grbOrientation = new System.Windows.Forms.GroupBox();
            this.picBoxOrientation = new System.Windows.Forms.PictureBox();
            this.rdbLandscape = new System.Windows.Forms.RadioButton();
            this.rdbPortrait = new System.Windows.Forms.RadioButton();
            this.grbPaperPlacement = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtUserDefinedMarginY = new System.Windows.Forms.TextBox();
            this.txtUserDefinedMarginX = new System.Windows.Forms.TextBox();
            this.cboMarginType = new System.Windows.Forms.ComboBox();
            this.rdbOffsetFromConer = new System.Windows.Forms.RadioButton();
            this.rdbCenter = new System.Windows.Forms.RadioButton();
            this.grbHiddenLineViews = new System.Windows.Forms.GroupBox();
            this.rdbRasterProcessing = new System.Windows.Forms.RadioButton();
            this.rdbVectorProcessing = new System.Windows.Forms.RadioButton();
            this.lblRemoveLlinesUsing = new System.Windows.Forms.Label();
            this.grbZoom = new System.Windows.Forms.GroupBox();
            this.zoomPercentNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.lblSizeZoom = new System.Windows.Forms.Label();
            this.rdbZoom = new System.Windows.Forms.RadioButton();
            this.rdbFitToPage = new System.Windows.Forms.RadioButton();
            this.grbAppearance = new System.Windows.Forms.GroupBox();
            this.lblColors = new System.Windows.Forms.Label();
            this.cboColors = new System.Windows.Forms.ComboBox();
            this.cboRasterQuality = new System.Windows.Forms.ComboBox();
            this.lblRasterQuality = new System.Windows.Forms.Label();
            this.grbOptions = new System.Windows.Forms.GroupBox();
            this.ckbReplaceHafttoneWithThinLines = new System.Windows.Forms.CheckBox();
            this.ckbRegionEdgesMaskCoincidentLines = new System.Windows.Forms.CheckBox();
            this.ckbHideCropBoundaries = new System.Windows.Forms.CheckBox();
            this.ckbHideScopeBoxed = new System.Windows.Forms.CheckBox();
            this.ckbHideUnreferencedViewTags = new System.Windows.Forms.CheckBox();
            this.ckbHideRefWorkPlanes = new System.Windows.Forms.CheckBox();
            this.ckbViewLinksInBlue = new System.Windows.Forms.CheckBox();
            this.btOK = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.btSave = new System.Windows.Forms.Button();
            this.btSaveAs = new System.Windows.Forms.Button();
            this.btRevert = new System.Windows.Forms.Button();
            this.btRename = new System.Windows.Forms.Button();
            this.btDelete = new System.Windows.Forms.Button();
            this.grbPaper.SuspendLayout();
            this.grbOrientation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxOrientation)).BeginInit();
            this.grbPaperPlacement.SuspendLayout();
            this.grbHiddenLineViews.SuspendLayout();
            this.grbZoom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zoomPercentNumericUpDown)).BeginInit();
            this.grbAppearance.SuspendLayout();
            this.grbOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblPrinter
            // 
            this.lblPrinter.AutoSize = true;
            this.lblPrinter.Location = new System.Drawing.Point(6, 9);
            this.lblPrinter.Name = "lblPrinter";
            this.lblPrinter.Size = new System.Drawing.Size(50, 13);
            this.lblPrinter.TabIndex = 0;
            this.lblPrinter.Text = "lblPrinter:";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(6, 35);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(45, 13);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "lblName";
            // 
            // lblPrinterName
            // 
            this.lblPrinterName.AutoSize = true;
            this.lblPrinterName.Location = new System.Drawing.Point(73, 9);
            this.lblPrinterName.Name = "lblPrinterName";
            this.lblPrinterName.Size = new System.Drawing.Size(75, 13);
            this.lblPrinterName.TabIndex = 0;
            this.lblPrinterName.Text = "lblPrinterName";
            // 
            // printSetupsComboBox
            // 
            this.printSetupsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.printSetupsComboBox.FormattingEnabled = true;
            this.printSetupsComboBox.Location = new System.Drawing.Point(76, 32);
            this.printSetupsComboBox.Name = "printSetupsComboBox";
            this.printSetupsComboBox.Size = new System.Drawing.Size(390, 21);
            this.printSetupsComboBox.TabIndex = 1;
            // 
            // grbPaper
            // 
            this.grbPaper.Controls.Add(this.lblSource);
            this.grbPaper.Controls.Add(this.lblSize);
            this.grbPaper.Controls.Add(this.cboSourcePaper);
            this.grbPaper.Controls.Add(this.cboSizePaper);
            this.grbPaper.Location = new System.Drawing.Point(9, 59);
            this.grbPaper.Name = "grbPaper";
            this.grbPaper.Size = new System.Drawing.Size(251, 86);
            this.grbPaper.TabIndex = 2;
            this.grbPaper.TabStop = false;
            this.grbPaper.Text = "grbPaper";
            // 
            // lblSource
            // 
            this.lblSource.AutoSize = true;
            this.lblSource.Location = new System.Drawing.Point(5, 55);
            this.lblSource.Name = "lblSource";
            this.lblSource.Size = new System.Drawing.Size(51, 13);
            this.lblSource.TabIndex = 3;
            this.lblSource.Text = "lblSource";
            // 
            // lblSize
            // 
            this.lblSize.AutoSize = true;
            this.lblSize.Location = new System.Drawing.Point(5, 27);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(37, 13);
            this.lblSize.TabIndex = 2;
            this.lblSize.Text = "lblSize";
            // 
            // cboSourcePaper
            // 
            this.cboSourcePaper.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSourcePaper.FormattingEnabled = true;
            this.cboSourcePaper.Location = new System.Drawing.Point(67, 50);
            this.cboSourcePaper.Name = "cboSourcePaper";
            this.cboSourcePaper.Size = new System.Drawing.Size(178, 21);
            this.cboSourcePaper.TabIndex = 3;
            // 
            // cboSizePaper
            // 
            this.cboSizePaper.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSizePaper.FormattingEnabled = true;
            this.cboSizePaper.Location = new System.Drawing.Point(67, 20);
            this.cboSizePaper.Name = "cboSizePaper";
            this.cboSizePaper.Size = new System.Drawing.Size(178, 21);
            this.cboSizePaper.TabIndex = 2;
            // 
            // grbOrientation
            // 
            this.grbOrientation.Controls.Add(this.picBoxOrientation);
            this.grbOrientation.Controls.Add(this.rdbLandscape);
            this.grbOrientation.Controls.Add(this.rdbPortrait);
            this.grbOrientation.Location = new System.Drawing.Point(266, 59);
            this.grbOrientation.Name = "grbOrientation";
            this.grbOrientation.Size = new System.Drawing.Size(200, 86);
            this.grbOrientation.TabIndex = 2;
            this.grbOrientation.TabStop = false;
            this.grbOrientation.Text = "grbOrientation";
            // 
            // picBoxOrientation
            // 
            this.picBoxOrientation.Location = new System.Drawing.Point(38, 26);
            this.picBoxOrientation.Name = "picBoxOrientation";
            this.picBoxOrientation.Size = new System.Drawing.Size(40, 40);
            this.picBoxOrientation.TabIndex = 1;
            this.picBoxOrientation.TabStop = false;
            // 
            // rdbLandscape
            // 
            this.rdbLandscape.AutoSize = true;
            this.rdbLandscape.Location = new System.Drawing.Point(102, 51);
            this.rdbLandscape.Name = "rdbLandscape";
            this.rdbLandscape.Size = new System.Drawing.Size(93, 17);
            this.rdbLandscape.TabIndex = 5;
            this.rdbLandscape.TabStop = true;
            this.rdbLandscape.Text = "rdbLandscape";
            this.rdbLandscape.UseVisualStyleBackColor = true;
            // 
            // rdbPortrait
            // 
            this.rdbPortrait.AutoSize = true;
            this.rdbPortrait.Location = new System.Drawing.Point(102, 23);
            this.rdbPortrait.Name = "rdbPortrait";
            this.rdbPortrait.Size = new System.Drawing.Size(73, 17);
            this.rdbPortrait.TabIndex = 4;
            this.rdbPortrait.TabStop = true;
            this.rdbPortrait.Text = "rdbPortrait";
            this.rdbPortrait.UseVisualStyleBackColor = true;
            // 
            // grbPaperPlacement
            // 
            this.grbPaperPlacement.Controls.Add(this.label6);
            this.grbPaperPlacement.Controls.Add(this.label5);
            this.grbPaperPlacement.Controls.Add(this.txtUserDefinedMarginY);
            this.grbPaperPlacement.Controls.Add(this.txtUserDefinedMarginX);
            this.grbPaperPlacement.Controls.Add(this.cboMarginType);
            this.grbPaperPlacement.Controls.Add(this.rdbOffsetFromConer);
            this.grbPaperPlacement.Controls.Add(this.rdbCenter);
            this.grbPaperPlacement.Location = new System.Drawing.Point(9, 151);
            this.grbPaperPlacement.Name = "grbPaperPlacement";
            this.grbPaperPlacement.Size = new System.Drawing.Size(251, 103);
            this.grbPaperPlacement.TabIndex = 2;
            this.grbPaperPlacement.TabStop = false;
            this.grbPaperPlacement.Text = "grbPaperPlacement";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(230, 71);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(18, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "=y";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(163, 71);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(18, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "=x";
            // 
            // txtUserDefinedMarginY
            // 
            this.txtUserDefinedMarginY.Location = new System.Drawing.Point(182, 68);
            this.txtUserDefinedMarginY.Name = "txtUserDefinedMarginY";
            this.txtUserDefinedMarginY.Size = new System.Drawing.Size(45, 20);
            this.txtUserDefinedMarginY.TabIndex = 10;
            this.txtUserDefinedMarginY.Text = "0.0000";
            // 
            // txtUserDefinedMarginX
            // 
            this.txtUserDefinedMarginX.Location = new System.Drawing.Point(118, 68);
            this.txtUserDefinedMarginX.Name = "txtUserDefinedMarginX";
            this.txtUserDefinedMarginX.Size = new System.Drawing.Size(45, 20);
            this.txtUserDefinedMarginX.TabIndex = 9;
            this.txtUserDefinedMarginX.Text = "0.0000";
            // 
            // cboMarginType
            // 
            this.cboMarginType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMarginType.FormattingEnabled = true;
            this.cboMarginType.Location = new System.Drawing.Point(118, 41);
            this.cboMarginType.Name = "cboMarginType";
            this.cboMarginType.Size = new System.Drawing.Size(127, 21);
            this.cboMarginType.TabIndex = 8;
            // 
            // rdbOffsetFromConer
            // 
            this.rdbOffsetFromConer.AutoSize = true;
            this.rdbOffsetFromConer.Location = new System.Drawing.Point(6, 42);
            this.rdbOffsetFromConer.Name = "rdbOffsetFromConer";
            this.rdbOffsetFromConer.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.rdbOffsetFromConer.Size = new System.Drawing.Size(122, 17);
            this.rdbOffsetFromConer.TabIndex = 7;
            this.rdbOffsetFromConer.TabStop = true;
            this.rdbOffsetFromConer.Text = "rdbOffsetFromConer:";
            this.rdbOffsetFromConer.UseVisualStyleBackColor = true;
            // 
            // rdbCenter
            // 
            this.rdbCenter.AutoSize = true;
            this.rdbCenter.Location = new System.Drawing.Point(6, 19);
            this.rdbCenter.Name = "rdbCenter";
            this.rdbCenter.Size = new System.Drawing.Size(71, 17);
            this.rdbCenter.TabIndex = 6;
            this.rdbCenter.TabStop = true;
            this.rdbCenter.Text = "rdbCenter";
            this.rdbCenter.UseVisualStyleBackColor = true;
            // 
            // grbHiddenLineViews
            // 
            this.grbHiddenLineViews.Controls.Add(this.rdbRasterProcessing);
            this.grbHiddenLineViews.Controls.Add(this.rdbVectorProcessing);
            this.grbHiddenLineViews.Controls.Add(this.lblRemoveLlinesUsing);
            this.grbHiddenLineViews.Location = new System.Drawing.Point(266, 151);
            this.grbHiddenLineViews.Name = "grbHiddenLineViews";
            this.grbHiddenLineViews.Size = new System.Drawing.Size(200, 102);
            this.grbHiddenLineViews.TabIndex = 2;
            this.grbHiddenLineViews.TabStop = false;
            this.grbHiddenLineViews.Text = "grbHiddenLineViews";
            // 
            // rdbRasterProcessing
            // 
            this.rdbRasterProcessing.AutoSize = true;
            this.rdbRasterProcessing.Location = new System.Drawing.Point(7, 68);
            this.rdbRasterProcessing.Name = "rdbRasterProcessing";
            this.rdbRasterProcessing.Size = new System.Drawing.Size(123, 17);
            this.rdbRasterProcessing.TabIndex = 12;
            this.rdbRasterProcessing.TabStop = true;
            this.rdbRasterProcessing.Text = "rdbRasterProcessing";
            this.rdbRasterProcessing.UseVisualStyleBackColor = true;
            // 
            // rdbVectorProcessing
            // 
            this.rdbVectorProcessing.AutoSize = true;
            this.rdbVectorProcessing.Location = new System.Drawing.Point(7, 42);
            this.rdbVectorProcessing.Name = "rdbVectorProcessing";
            this.rdbVectorProcessing.Size = new System.Drawing.Size(123, 17);
            this.rdbVectorProcessing.TabIndex = 11;
            this.rdbVectorProcessing.TabStop = true;
            this.rdbVectorProcessing.Text = "rdbVectorProcessing";
            this.rdbVectorProcessing.UseVisualStyleBackColor = true;
            // 
            // lblRemoveLlinesUsing
            // 
            this.lblRemoveLlinesUsing.AutoSize = true;
            this.lblRemoveLlinesUsing.Location = new System.Drawing.Point(5, 21);
            this.lblRemoveLlinesUsing.Name = "lblRemoveLlinesUsing";
            this.lblRemoveLlinesUsing.Size = new System.Drawing.Size(114, 13);
            this.lblRemoveLlinesUsing.TabIndex = 0;
            this.lblRemoveLlinesUsing.Text = "lblRemoveLlinesUsing:";
            // 
            // grbZoom
            // 
            this.grbZoom.Controls.Add(this.zoomPercentNumericUpDown);
            this.grbZoom.Controls.Add(this.lblSizeZoom);
            this.grbZoom.Controls.Add(this.rdbZoom);
            this.grbZoom.Controls.Add(this.rdbFitToPage);
            this.grbZoom.Location = new System.Drawing.Point(9, 259);
            this.grbZoom.Name = "grbZoom";
            this.grbZoom.Size = new System.Drawing.Size(251, 125);
            this.grbZoom.TabIndex = 2;
            this.grbZoom.TabStop = false;
            this.grbZoom.Text = "grbZoom";
            // 
            // zoomPercentNumericUpDown
            // 
            this.zoomPercentNumericUpDown.Location = new System.Drawing.Point(120, 42);
            this.zoomPercentNumericUpDown.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
            this.zoomPercentNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.zoomPercentNumericUpDown.Name = "zoomPercentNumericUpDown";
            this.zoomPercentNumericUpDown.Size = new System.Drawing.Size(46, 20);
            this.zoomPercentNumericUpDown.TabIndex = 15;
            this.zoomPercentNumericUpDown.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.zoomPercentNumericUpDown.ValueChanged += new System.EventHandler(this.zoomPercentNumericUpDown_ValueChanged);
            // 
            // lblSizeZoom
            // 
            this.lblSizeZoom.AutoSize = true;
            this.lblSizeZoom.Location = new System.Drawing.Point(172, 44);
            this.lblSizeZoom.Name = "lblSizeZoom";
            this.lblSizeZoom.Size = new System.Drawing.Size(37, 13);
            this.lblSizeZoom.TabIndex = 2;
            this.lblSizeZoom.Text = "lblSize";
            // 
            // rdbZoom
            // 
            this.rdbZoom.AutoSize = true;
            this.rdbZoom.Location = new System.Drawing.Point(6, 42);
            this.rdbZoom.Name = "rdbZoom";
            this.rdbZoom.Size = new System.Drawing.Size(67, 17);
            this.rdbZoom.TabIndex = 14;
            this.rdbZoom.TabStop = true;
            this.rdbZoom.Text = "rdbZoom";
            this.rdbZoom.UseVisualStyleBackColor = true;
            // 
            // rdbFitToPage
            // 
            this.rdbFitToPage.AutoSize = true;
            this.rdbFitToPage.Location = new System.Drawing.Point(6, 19);
            this.rdbFitToPage.Name = "rdbFitToPage";
            this.rdbFitToPage.Size = new System.Drawing.Size(89, 17);
            this.rdbFitToPage.TabIndex = 13;
            this.rdbFitToPage.TabStop = true;
            this.rdbFitToPage.Text = "rdbFitToPage";
            this.rdbFitToPage.UseVisualStyleBackColor = true;
            // 
            // grbAppearance
            // 
            this.grbAppearance.Controls.Add(this.lblColors);
            this.grbAppearance.Controls.Add(this.cboColors);
            this.grbAppearance.Controls.Add(this.cboRasterQuality);
            this.grbAppearance.Controls.Add(this.lblRasterQuality);
            this.grbAppearance.Location = new System.Drawing.Point(266, 259);
            this.grbAppearance.Name = "grbAppearance";
            this.grbAppearance.Size = new System.Drawing.Size(200, 125);
            this.grbAppearance.TabIndex = 2;
            this.grbAppearance.TabStop = false;
            this.grbAppearance.Text = "grbAppearance";
            // 
            // lblColors
            // 
            this.lblColors.AutoSize = true;
            this.lblColors.Location = new System.Drawing.Point(4, 71);
            this.lblColors.Name = "lblColors";
            this.lblColors.Size = new System.Drawing.Size(46, 13);
            this.lblColors.TabIndex = 2;
            this.lblColors.Text = "lblColors";
            // 
            // cboColors
            // 
            this.cboColors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboColors.FormattingEnabled = true;
            this.cboColors.Location = new System.Drawing.Point(6, 98);
            this.cboColors.Name = "cboColors";
            this.cboColors.Size = new System.Drawing.Size(121, 21);
            this.cboColors.TabIndex = 17;
            // 
            // cboRasterQuality
            // 
            this.cboRasterQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRasterQuality.FormattingEnabled = true;
            this.cboRasterQuality.Location = new System.Drawing.Point(6, 38);
            this.cboRasterQuality.Name = "cboRasterQuality";
            this.cboRasterQuality.Size = new System.Drawing.Size(121, 21);
            this.cboRasterQuality.TabIndex = 16;
            // 
            // lblRasterQuality
            // 
            this.lblRasterQuality.AutoSize = true;
            this.lblRasterQuality.Location = new System.Drawing.Point(5, 16);
            this.lblRasterQuality.Name = "lblRasterQuality";
            this.lblRasterQuality.Size = new System.Drawing.Size(80, 13);
            this.lblRasterQuality.TabIndex = 0;
            this.lblRasterQuality.Text = "lblRasterQuality";
            // 
            // grbOptions
            // 
            this.grbOptions.Controls.Add(this.ckbReplaceHafttoneWithThinLines);
            this.grbOptions.Controls.Add(this.ckbRegionEdgesMaskCoincidentLines);
            this.grbOptions.Controls.Add(this.ckbHideCropBoundaries);
            this.grbOptions.Controls.Add(this.ckbHideScopeBoxed);
            this.grbOptions.Controls.Add(this.ckbHideUnreferencedViewTags);
            this.grbOptions.Controls.Add(this.ckbHideRefWorkPlanes);
            this.grbOptions.Controls.Add(this.ckbViewLinksInBlue);
            this.grbOptions.Location = new System.Drawing.Point(9, 390);
            this.grbOptions.Name = "grbOptions";
            this.grbOptions.Size = new System.Drawing.Size(457, 115);
            this.grbOptions.TabIndex = 2;
            this.grbOptions.TabStop = false;
            this.grbOptions.Text = "grbOptions";
            // 
            // ckbReplaceHafttoneWithThinLines
            // 
            this.ckbReplaceHafttoneWithThinLines.AutoSize = true;
            this.ckbReplaceHafttoneWithThinLines.Location = new System.Drawing.Point(264, 65);
            this.ckbReplaceHafttoneWithThinLines.Name = "ckbReplaceHafttoneWithThinLines";
            this.ckbReplaceHafttoneWithThinLines.Size = new System.Drawing.Size(193, 17);
            this.ckbReplaceHafttoneWithThinLines.TabIndex = 24;
            this.ckbReplaceHafttoneWithThinLines.Text = "ckbReplaceHafttoneWithThinLines";
            this.ckbReplaceHafttoneWithThinLines.UseVisualStyleBackColor = true;
            // 
            // ckbRegionEdgesMaskCoincidentLines
            // 
            this.ckbRegionEdgesMaskCoincidentLines.AutoSize = true;
            this.ckbRegionEdgesMaskCoincidentLines.Location = new System.Drawing.Point(6, 88);
            this.ckbRegionEdgesMaskCoincidentLines.Name = "ckbRegionEdgesMaskCoincidentLines";
            this.ckbRegionEdgesMaskCoincidentLines.Size = new System.Drawing.Size(209, 17);
            this.ckbRegionEdgesMaskCoincidentLines.TabIndex = 21;
            this.ckbRegionEdgesMaskCoincidentLines.Text = "ckbRegionEdgesMaskCoincidentLines";
            this.ckbRegionEdgesMaskCoincidentLines.UseVisualStyleBackColor = true;
            // 
            // ckbHideCropBoundaries
            // 
            this.ckbHideCropBoundaries.AutoSize = true;
            this.ckbHideCropBoundaries.Location = new System.Drawing.Point(264, 42);
            this.ckbHideCropBoundaries.Name = "ckbHideCropBoundaries";
            this.ckbHideCropBoundaries.Size = new System.Drawing.Size(141, 17);
            this.ckbHideCropBoundaries.TabIndex = 23;
            this.ckbHideCropBoundaries.Text = "ckbHideCropBoundaries";
            this.ckbHideCropBoundaries.UseVisualStyleBackColor = true;
            // 
            // ckbHideScopeBoxed
            // 
            this.ckbHideScopeBoxed.AutoSize = true;
            this.ckbHideScopeBoxed.Location = new System.Drawing.Point(264, 19);
            this.ckbHideScopeBoxed.Name = "ckbHideScopeBoxed";
            this.ckbHideScopeBoxed.Size = new System.Drawing.Size(127, 17);
            this.ckbHideScopeBoxed.TabIndex = 22;
            this.ckbHideScopeBoxed.Text = "ckbHideScopeBoxed";
            this.ckbHideScopeBoxed.UseVisualStyleBackColor = true;
            // 
            // ckbHideUnreferencedViewTags
            // 
            this.ckbHideUnreferencedViewTags.AutoSize = true;
            this.ckbHideUnreferencedViewTags.Location = new System.Drawing.Point(6, 65);
            this.ckbHideUnreferencedViewTags.Name = "ckbHideUnreferencedViewTags";
            this.ckbHideUnreferencedViewTags.Size = new System.Drawing.Size(178, 17);
            this.ckbHideUnreferencedViewTags.TabIndex = 20;
            this.ckbHideUnreferencedViewTags.Text = "ckbHideUnreferencedViewTags";
            this.ckbHideUnreferencedViewTags.UseVisualStyleBackColor = true;
            // 
            // ckbHideRefWorkPlanes
            // 
            this.ckbHideRefWorkPlanes.AutoSize = true;
            this.ckbHideRefWorkPlanes.Location = new System.Drawing.Point(6, 42);
            this.ckbHideRefWorkPlanes.Name = "ckbHideRefWorkPlanes";
            this.ckbHideRefWorkPlanes.Size = new System.Drawing.Size(141, 17);
            this.ckbHideRefWorkPlanes.TabIndex = 19;
            this.ckbHideRefWorkPlanes.Text = "ckbHideRefWorkPlanes";
            this.ckbHideRefWorkPlanes.UseVisualStyleBackColor = true;
            // 
            // ckbViewLinksInBlue
            // 
            this.ckbViewLinksInBlue.AutoSize = true;
            this.ckbViewLinksInBlue.Location = new System.Drawing.Point(6, 19);
            this.ckbViewLinksInBlue.Name = "ckbViewLinksInBlue";
            this.ckbViewLinksInBlue.Size = new System.Drawing.Size(122, 17);
            this.ckbViewLinksInBlue.TabIndex = 18;
            this.ckbViewLinksInBlue.Text = "ckbViewLinksInBlue";
            this.ckbViewLinksInBlue.UseVisualStyleBackColor = true;
            // 
            // btOK
            // 
            this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btOK.Location = new System.Drawing.Point(442, 511);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 23);
            this.btOK.TabIndex = 30;
            this.btOK.Text = "btOK";
            this.btOK.UseVisualStyleBackColor = true;
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(523, 511);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 31;
            this.btCancel.Text = "btCancel";
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // btSave
            // 
            this.btSave.Location = new System.Drawing.Point(472, 31);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(128, 23);
            this.btSave.TabIndex = 25;
            this.btSave.Text = "btSave";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // btSaveAs
            // 
            this.btSaveAs.Location = new System.Drawing.Point(472, 60);
            this.btSaveAs.Name = "btSaveAs";
            this.btSaveAs.Size = new System.Drawing.Size(128, 23);
            this.btSaveAs.TabIndex = 26;
            this.btSaveAs.Text = "btSaveAs";
            this.btSaveAs.UseVisualStyleBackColor = true;
            this.btSaveAs.Click += new System.EventHandler(this.btSaveAs_Click);
            // 
            // btRevert
            // 
            this.btRevert.Enabled = false;
            this.btRevert.Location = new System.Drawing.Point(472, 89);
            this.btRevert.Name = "btRevert";
            this.btRevert.Size = new System.Drawing.Size(128, 23);
            this.btRevert.TabIndex = 27;
            this.btRevert.Text = "btRevert";
            this.btRevert.UseVisualStyleBackColor = true;
            this.btRevert.Click += new System.EventHandler(this.btRevert_Click);
            // 
            // btRename
            // 
            this.btRename.Location = new System.Drawing.Point(472, 118);
            this.btRename.Name = "btRename";
            this.btRename.Size = new System.Drawing.Size(128, 23);
            this.btRename.TabIndex = 28;
            this.btRename.Text = "btRename";
            this.btRename.UseVisualStyleBackColor = true;
            this.btRename.Click += new System.EventHandler(this.btRename_Click);
            // 
            // btDelete
            // 
            this.btDelete.Location = new System.Drawing.Point(472, 147);
            this.btDelete.Name = "btDelete";
            this.btDelete.Size = new System.Drawing.Size(128, 23);
            this.btDelete.TabIndex = 29;
            this.btDelete.Text = "btDelete";
            this.btDelete.UseVisualStyleBackColor = true;
            this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
            // 
            // PrintSetupForm
            // 
            this.AcceptButton = this.btOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btCancel;
            this.ClientSize = new System.Drawing.Size(606, 546);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btDelete);
            this.Controls.Add(this.btRename);
            this.Controls.Add(this.btRevert);
            this.Controls.Add(this.btSaveAs);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.grbOrientation);
            this.Controls.Add(this.grbAppearance);
            this.Controls.Add(this.grbOptions);
            this.Controls.Add(this.grbZoom);
            this.Controls.Add(this.grbHiddenLineViews);
            this.Controls.Add(this.grbPaperPlacement);
            this.Controls.Add(this.grbPaper);
            this.Controls.Add(this.printSetupsComboBox);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblPrinterName);
            this.Controls.Add(this.lblPrinter);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PrintSetupForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Print Setup";
            this.Load += new System.EventHandler(this.PrintSetupForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PrintSetupForm_KeyDown);
            this.grbPaper.ResumeLayout(false);
            this.grbPaper.PerformLayout();
            this.grbOrientation.ResumeLayout(false);
            this.grbOrientation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxOrientation)).EndInit();
            this.grbPaperPlacement.ResumeLayout(false);
            this.grbPaperPlacement.PerformLayout();
            this.grbHiddenLineViews.ResumeLayout(false);
            this.grbHiddenLineViews.PerformLayout();
            this.grbZoom.ResumeLayout(false);
            this.grbZoom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zoomPercentNumericUpDown)).EndInit();
            this.grbAppearance.ResumeLayout(false);
            this.grbAppearance.PerformLayout();
            this.grbOptions.ResumeLayout(false);
            this.grbOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPrinter;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblPrinterName;
        private System.Windows.Forms.ComboBox printSetupsComboBox;
        private System.Windows.Forms.GroupBox grbPaper;
        private System.Windows.Forms.GroupBox grbOrientation;
        private System.Windows.Forms.GroupBox grbPaperPlacement;
        private System.Windows.Forms.GroupBox grbHiddenLineViews;
        private System.Windows.Forms.GroupBox grbZoom;
        private System.Windows.Forms.GroupBox grbAppearance;
        private System.Windows.Forms.GroupBox grbOptions;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Button btSaveAs;
        private System.Windows.Forms.Button btRevert;
        private System.Windows.Forms.Button btRename;
        private System.Windows.Forms.Button btDelete;
        private System.Windows.Forms.ComboBox cboSourcePaper;
        private System.Windows.Forms.ComboBox cboSizePaper;
        private System.Windows.Forms.RadioButton rdbLandscape;
        private System.Windows.Forms.RadioButton rdbPortrait;
        private System.Windows.Forms.RadioButton rdbCenter;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtUserDefinedMarginY;
        private System.Windows.Forms.TextBox txtUserDefinedMarginX;
        private System.Windows.Forms.ComboBox cboMarginType;
        private System.Windows.Forms.RadioButton rdbOffsetFromConer;
        private System.Windows.Forms.RadioButton rdbRasterProcessing;
        private System.Windows.Forms.RadioButton rdbVectorProcessing;
        private System.Windows.Forms.Label lblRemoveLlinesUsing;
        private System.Windows.Forms.RadioButton rdbZoom;
        private System.Windows.Forms.RadioButton rdbFitToPage;
        private System.Windows.Forms.Label lblSizeZoom;
        private System.Windows.Forms.Label lblColors;
        private System.Windows.Forms.ComboBox cboColors;
        private System.Windows.Forms.ComboBox cboRasterQuality;
        private System.Windows.Forms.Label lblRasterQuality;
        private System.Windows.Forms.CheckBox ckbHideCropBoundaries;
        private System.Windows.Forms.CheckBox ckbHideScopeBoxed;
        private System.Windows.Forms.CheckBox ckbHideUnreferencedViewTags;
        private System.Windows.Forms.CheckBox ckbHideRefWorkPlanes;
        private System.Windows.Forms.CheckBox ckbViewLinksInBlue;
        private System.Windows.Forms.NumericUpDown zoomPercentNumericUpDown;
        private System.Windows.Forms.CheckBox ckbReplaceHafttoneWithThinLines;
        private System.Windows.Forms.CheckBox ckbRegionEdgesMaskCoincidentLines;
        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.Label lblSource;
        private System.Windows.Forms.PictureBox picBoxOrientation;
    }
}
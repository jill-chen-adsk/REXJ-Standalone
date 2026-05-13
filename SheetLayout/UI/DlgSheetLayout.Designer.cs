namespace ADSK.ViewExtension.SheetLayout
{
    partial class DlgSheetLayout
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components != null)
                    components.Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        private void InitializeComponent()
        {
            this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.OK_Button = new System.Windows.Forms.Button();
            this.Cancel_Button = new System.Windows.Forms.Button();
            this.BtnUP = new System.Windows.Forms.Button();
            this.BtnDN = new System.Windows.Forms.Button();
            this.LbxViews = new System.Windows.Forms.ListBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.CbxViewFamilyType = new System.Windows.Forms.ComboBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.CbxViewType = new System.Windows.Forms.ComboBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.CbxDicipline = new System.Windows.Forms.ComboBox();
            this.BtnAdd = new System.Windows.Forms.Button();
            this.BtnRmv = new System.Windows.Forms.Button();
            this.BtnUp2 = new System.Windows.Forms.Button();
            this.BtnDn2 = new System.Windows.Forms.Button();
            this.ChkAddSameLeg = new System.Windows.Forms.CheckBox();
            this.GrpViewports = new System.Windows.Forms.GroupBox();
            this.Label10 = new System.Windows.Forms.Label();
            this.LvViewOnSheet = new System.Windows.Forms.ListView();
            this.ColViewID = new System.Windows.Forms.ColumnHeader();
            this.ColViewPage = new System.Windows.Forms.ColumnHeader();
            this.ColViewNo = new System.Windows.Forms.ColumnHeader();
            this.ColViewName = new System.Windows.Forms.ColumnHeader();
            this.Label9 = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.LbxViewports = new System.Windows.Forms.ListBox();
            this.GrpSchedule = new System.Windows.Forms.GroupBox();
            this.LblSchNewSheet = new System.Windows.Forms.Label();
            this.BtnDN3 = new System.Windows.Forms.Button();
            this.BtmRmv = new System.Windows.Forms.Button();
            this.BtnUP3 = new System.Windows.Forms.Button();
            this.BtnAdd2 = new System.Windows.Forms.Button();
            this.LvScheduleinstanceOnSheet = new System.Windows.Forms.ListView();
            this.ColumnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.ColumnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.ColumnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.ColumnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.LblScheduleInfo = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.LbxSchedules = new System.Windows.Forms.ListBox();
            this.LbxScheduleInstances = new System.Windows.Forms.ListBox();
            this.Label7 = new System.Windows.Forms.Label();
            this.TableLayoutPanel1.SuspendLayout();
            this.GrpViewports.SuspendLayout();
            this.GrpSchedule.SuspendLayout();
            this.SuspendLayout();
            //
            // TableLayoutPanel1
            //
            this.TableLayoutPanel1.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.TableLayoutPanel1.ColumnCount = 2;
            this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanel1.Controls.Add(this.OK_Button, 0, 0);
            this.TableLayoutPanel1.Controls.Add(this.Cancel_Button, 1, 0);
            this.TableLayoutPanel1.Location = new System.Drawing.Point(575, 536);
            this.TableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TableLayoutPanel1.Name = "TableLayoutPanel1";
            this.TableLayoutPanel1.RowCount = 1;
            this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanel1.Size = new System.Drawing.Size(146, 26);
            this.TableLayoutPanel1.TabIndex = 0;
            //
            // OK_Button
            //
            this.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.OK_Button.Location = new System.Drawing.Point(3, 2);
            this.OK_Button.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.OK_Button.Name = "OK_Button";
            this.OK_Button.Size = new System.Drawing.Size(67, 22);
            this.OK_Button.TabIndex = 0;
            this.OK_Button.Text = global::ADSK.ViewExtension.SheetLayout.Resources.Text.TXT_OK;
            //
            // Cancel_Button
            //
            this.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel_Button.Location = new System.Drawing.Point(76, 2);
            this.Cancel_Button.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Cancel_Button.Name = "Cancel_Button";
            this.Cancel_Button.Size = new System.Drawing.Size(67, 22);
            this.Cancel_Button.TabIndex = 1;
            this.Cancel_Button.Text = global::ADSK.ViewExtension.SheetLayout.Resources.Text.TXT_CANCEL;
            //
            // BtnUP
            //
            this.BtnUP.Location = new System.Drawing.Point(2, 294);
            this.BtnUP.Margin = new System.Windows.Forms.Padding(2);
            this.BtnUP.Name = "BtnUP";
            this.BtnUP.Size = new System.Drawing.Size(45, 18);
            this.BtnUP.TabIndex = 2;
            this.BtnUP.Text = "▲";
            this.BtnUP.UseVisualStyleBackColor = true;
            //
            // BtnDN
            //
            this.BtnDN.Location = new System.Drawing.Point(51, 294);
            this.BtnDN.Margin = new System.Windows.Forms.Padding(2);
            this.BtnDN.Name = "BtnDN";
            this.BtnDN.Size = new System.Drawing.Size(45, 18);
            this.BtnDN.TabIndex = 3;
            this.BtnDN.Text = "▼";
            this.BtnDN.UseVisualStyleBackColor = true;
            //
            // LbxViews
            //
            this.LbxViews.FormattingEnabled = true;
            this.LbxViews.ItemHeight = 12;
            this.LbxViews.Location = new System.Drawing.Point(173, 130);
            this.LbxViews.Name = "LbxViews";
            this.LbxViews.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.LbxViews.Size = new System.Drawing.Size(237, 160);
            this.LbxViews.TabIndex = 11;
            //
            // Label3
            //
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(172, 102);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(60, 12);
            this.Label3.TabIndex = 9;
            this.Label3.Text = global::ADSK.ViewExtension.SheetLayout.Resources.Text.TXT_LABEL_VIEW_FAMILY_TYPE;
            //
            // CbxViewFamilyType
            //
            this.CbxViewFamilyType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CbxViewFamilyType.FormattingEnabled = true;
            this.CbxViewFamilyType.Location = new System.Drawing.Point(256, 98);
            this.CbxViewFamilyType.Name = "CbxViewFamilyType";
            this.CbxViewFamilyType.Size = new System.Drawing.Size(154, 20);
            this.CbxViewFamilyType.TabIndex = 10;
            //
            // Label2
            //
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(172, 76);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(57, 12);
            this.Label2.TabIndex = 7;
            this.Label2.Text = global::ADSK.ViewExtension.SheetLayout.Resources.Text.TXT_LABEL_VIEW_CATEGORY;
            //
            // CbxViewType
            //
            this.CbxViewType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CbxViewType.FormattingEnabled = true;
            this.CbxViewType.Location = new System.Drawing.Point(256, 72);
            this.CbxViewType.Name = "CbxViewType";
            this.CbxViewType.Size = new System.Drawing.Size(154, 20);
            this.CbxViewType.TabIndex = 8;
            //
            // Label4
            //
            this.Label4.AutoSize = true;
            this.Label4.Location = new System.Drawing.Point(172, 50);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(53, 12);
            this.Label4.TabIndex = 5;
            this.Label4.Text = global::ADSK.ViewExtension.SheetLayout.Resources.Text.TXT_LABEL_DISCIPLINE;
            //
            // CbxDicipline
            //
            this.CbxDicipline.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CbxDicipline.FormattingEnabled = true;
            this.CbxDicipline.Location = new System.Drawing.Point(256, 46);
            this.CbxDicipline.Name = "CbxDicipline";
            this.CbxDicipline.Size = new System.Drawing.Size(154, 20);
            this.CbxDicipline.TabIndex = 6;
            //
            // BtnAdd
            //
            this.BtnAdd.Location = new System.Drawing.Point(420, 182);
            this.BtnAdd.Margin = new System.Windows.Forms.Padding(2);
            this.BtnAdd.Name = "BtnAdd";
            this.BtnAdd.Size = new System.Drawing.Size(29, 18);
            this.BtnAdd.TabIndex = 12;
            this.BtnAdd.Text = ">";
            this.BtnAdd.UseVisualStyleBackColor = true;
            //
            // BtnRmv
            //
            this.BtnRmv.Location = new System.Drawing.Point(420, 204);
            this.BtnRmv.Margin = new System.Windows.Forms.Padding(2);
            this.BtnRmv.Name = "BtnRmv";
            this.BtnRmv.Size = new System.Drawing.Size(29, 18);
            this.BtnRmv.TabIndex = 13;
            this.BtnRmv.Text = "<";
            this.BtnRmv.UseVisualStyleBackColor = true;
            //
            // BtnUp2
            //
            this.BtnUp2.Location = new System.Drawing.Point(462, 294);
            this.BtnUp2.Margin = new System.Windows.Forms.Padding(2);
            this.BtnUp2.Name = "BtnUp2";
            this.BtnUp2.Size = new System.Drawing.Size(45, 18);
            this.BtnUp2.TabIndex = 16;
            this.BtnUp2.Text = "▲";
            this.BtnUp2.UseVisualStyleBackColor = true;
            //
            // BtnDn2
            //
            this.BtnDn2.Location = new System.Drawing.Point(512, 294);
            this.BtnDn2.Margin = new System.Windows.Forms.Padding(2);
            this.BtnDn2.Name = "BtnDn2";
            this.BtnDn2.Size = new System.Drawing.Size(45, 18);
            this.BtnDn2.TabIndex = 17;
            this.BtnDn2.Text = "▼";
            this.BtnDn2.UseVisualStyleBackColor = true;
            //
            // ChkAddSameLeg
            //
            this.ChkAddSameLeg.AutoSize = true;
            this.ChkAddSameLeg.Location = new System.Drawing.Point(22, 542);
            this.ChkAddSameLeg.Margin = new System.Windows.Forms.Padding(2);
            this.ChkAddSameLeg.Name = "ChkAddSameLeg";
            this.ChkAddSameLeg.Size = new System.Drawing.Size(121, 16);
            this.ChkAddSameLeg.TabIndex = 18;
            this.ChkAddSameLeg.Text = global::ADSK.ViewExtension.SheetLayout.Resources.Text.TXT_CHK_SAME_LEGEND;
            this.ChkAddSameLeg.UseVisualStyleBackColor = true;
            //
            // GrpViewports
            //
            this.GrpViewports.Controls.Add(this.Label10);
            this.GrpViewports.Controls.Add(this.BtnUp2);
            this.GrpViewports.Controls.Add(this.LvViewOnSheet);
            this.GrpViewports.Controls.Add(this.BtnDn2);
            this.GrpViewports.Controls.Add(this.CbxDicipline);
            this.GrpViewports.Controls.Add(this.Label9);
            this.GrpViewports.Controls.Add(this.Label2);
            this.GrpViewports.Controls.Add(this.BtnRmv);
            this.GrpViewports.Controls.Add(this.Label1);
            this.GrpViewports.Controls.Add(this.CbxViewType);
            this.GrpViewports.Controls.Add(this.BtnAdd);
            this.GrpViewports.Controls.Add(this.LbxViewports);
            this.GrpViewports.Controls.Add(this.Label4);
            this.GrpViewports.Controls.Add(this.BtnUP);
            this.GrpViewports.Controls.Add(this.CbxViewFamilyType);
            this.GrpViewports.Controls.Add(this.Label3);
            this.GrpViewports.Controls.Add(this.BtnDN);
            this.GrpViewports.Controls.Add(this.LbxViews);
            this.GrpViewports.Location = new System.Drawing.Point(16, 11);
            this.GrpViewports.Margin = new System.Windows.Forms.Padding(2);
            this.GrpViewports.Name = "GrpViewports";
            this.GrpViewports.Padding = new System.Windows.Forms.Padding(2);
            this.GrpViewports.Size = new System.Drawing.Size(708, 326);
            this.GrpViewports.TabIndex = 1;
            this.GrpViewports.TabStop = false;
            this.GrpViewports.Text = global::ADSK.ViewExtension.SheetLayout.Resources.Text.TXT_GRP_VIEWPORTS;
            //
            // Label10
            //
            this.Label10.AutoSize = true;
            this.Label10.Location = new System.Drawing.Point(462, 23);
            this.Label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(118, 12);
            this.Label10.TabIndex = 14;
            this.Label10.Text = global::ADSK.ViewExtension.SheetLayout.Resources.Text.TXT_LABEL_ORDER_NEW_SHEET;
            //
            // LvViewOnSheet
            //
            this.LvViewOnSheet.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { this.ColViewID, this.ColViewPage, this.ColViewNo, this.ColViewName });
            this.LvViewOnSheet.FullRowSelect = true;
            this.LvViewOnSheet.GridLines = true;
            this.LvViewOnSheet.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.LvViewOnSheet.HideSelection = false;
            this.LvViewOnSheet.Location = new System.Drawing.Point(462, 46);
            this.LvViewOnSheet.Margin = new System.Windows.Forms.Padding(2);
            this.LvViewOnSheet.Name = "LvViewOnSheet";
            this.LvViewOnSheet.Size = new System.Drawing.Size(234, 244);
            this.LvViewOnSheet.TabIndex = 15;
            this.LvViewOnSheet.UseCompatibleStateImageBehavior = false;
            this.LvViewOnSheet.View = System.Windows.Forms.View.Details;
            //
            // ColViewID
            //
            this.ColViewID.DisplayIndex = 3;
            this.ColViewID.Text = global::ADSK.ViewExtension.SheetLayout.Resources.Text.TXT_COL_ID;
            //
            // ColViewPage
            //
            this.ColViewPage.DisplayIndex = 0;
            this.ColViewPage.Text = global::ADSK.ViewExtension.SheetLayout.Resources.Text.TXT_COL_SHEET;
            this.ColViewPage.Width = 40;
            //
            // ColViewNo
            //
            this.ColViewNo.DisplayIndex = 1;
            this.ColViewNo.Text = global::ADSK.ViewExtension.SheetLayout.Resources.Text.TXT_COL_NUMBER;
            this.ColViewNo.Width = 40;
            //
            // ColViewName
            //
            this.ColViewName.DisplayIndex = 2;
            this.ColViewName.Text = global::ADSK.ViewExtension.SheetLayout.Resources.Text.TXT_COL_NAME;
            this.ColViewName.Width = 200;
            //
            // Label9
            //
            this.Label9.AutoSize = true;
            this.Label9.Location = new System.Drawing.Point(172, 23);
            this.Label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(107, 12);
            this.Label9.TabIndex = 4;
            this.Label9.Text = global::ADSK.ViewExtension.SheetLayout.Resources.Text.TXT_LABEL_SELECT_VIEWS;
            //
            // Label1
            //
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(4, 23);
            this.Label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(109, 12);
            this.Label1.TabIndex = 0;
            this.Label1.Text = global::ADSK.ViewExtension.SheetLayout.Resources.Text.TXT_LABEL_VIEWPORTS_ON_SHEET;
            //
            // LbxViewports
            //
            this.LbxViewports.FormattingEnabled = true;
            this.LbxViewports.ItemHeight = 12;
            this.LbxViewports.Location = new System.Drawing.Point(4, 46);
            this.LbxViewports.Margin = new System.Windows.Forms.Padding(2);
            this.LbxViewports.Name = "LbxViewports";
            this.LbxViewports.Size = new System.Drawing.Size(147, 244);
            this.LbxViewports.TabIndex = 1;
            //
            // GrpSchedule
            //
            this.GrpSchedule.Controls.Add(this.LblSchNewSheet);
            this.GrpSchedule.Controls.Add(this.BtnDN3);
            this.GrpSchedule.Controls.Add(this.BtmRmv);
            this.GrpSchedule.Controls.Add(this.BtnUP3);
            this.GrpSchedule.Controls.Add(this.BtnAdd2);
            this.GrpSchedule.Controls.Add(this.LvScheduleinstanceOnSheet);
            this.GrpSchedule.Controls.Add(this.LblScheduleInfo);
            this.GrpSchedule.Controls.Add(this.Label6);
            this.GrpSchedule.Controls.Add(this.Label5);
            this.GrpSchedule.Controls.Add(this.LbxSchedules);
            this.GrpSchedule.Controls.Add(this.LbxScheduleInstances);
            this.GrpSchedule.Location = new System.Drawing.Point(12, 342);
            this.GrpSchedule.Name = "GrpSchedule";
            this.GrpSchedule.Size = new System.Drawing.Size(709, 184);
            this.GrpSchedule.TabIndex = 2;
            this.GrpSchedule.TabStop = false;
            this.GrpSchedule.Text = global::ADSK.ViewExtension.SheetLayout.Resources.Text.TXT_GRP_SCHEDULES;
            //
            // LblSchNewSheet
            //
            this.LblSchNewSheet.AutoSize = true;
            this.LblSchNewSheet.Location = new System.Drawing.Point(456, 22);
            this.LblSchNewSheet.Name = "LblSchNewSheet";
            this.LblSchNewSheet.Size = new System.Drawing.Size(118, 12);
            this.LblSchNewSheet.TabIndex = 7;
            this.LblSchNewSheet.Text = global::ADSK.ViewExtension.SheetLayout.Resources.Text.TXT_LABEL_ORDER_NEW_SHEET;
            //
            // BtnDN3
            //
            this.BtnDN3.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.BtnDN3.Location = new System.Drawing.Point(512, 161);
            this.BtnDN3.Margin = new System.Windows.Forms.Padding(2);
            this.BtnDN3.Name = "BtnDN3";
            this.BtnDN3.Size = new System.Drawing.Size(45, 18);
            this.BtnDN3.TabIndex = 10;
            this.BtnDN3.Text = "▼";
            this.BtnDN3.UseVisualStyleBackColor = true;
            //
            // BtmRmv
            //
            this.BtmRmv.Location = new System.Drawing.Point(420, 96);
            this.BtmRmv.Margin = new System.Windows.Forms.Padding(2);
            this.BtmRmv.Name = "BtmRmv";
            this.BtmRmv.Size = new System.Drawing.Size(29, 18);
            this.BtmRmv.TabIndex = 6;
            this.BtmRmv.Text = "<";
            this.BtmRmv.UseVisualStyleBackColor = true;
            //
            // BtnUP3
            //
            this.BtnUP3.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.BtnUP3.Location = new System.Drawing.Point(462, 161);
            this.BtnUP3.Margin = new System.Windows.Forms.Padding(2);
            this.BtnUP3.Name = "BtnUP3";
            this.BtnUP3.Size = new System.Drawing.Size(45, 18);
            this.BtnUP3.TabIndex = 9;
            this.BtnUP3.Text = "▲";
            this.BtnUP3.UseVisualStyleBackColor = true;
            //
            // BtnAdd2
            //
            this.BtnAdd2.Location = new System.Drawing.Point(420, 74);
            this.BtnAdd2.Margin = new System.Windows.Forms.Padding(2);
            this.BtnAdd2.Name = "BtnAdd2";
            this.BtnAdd2.Size = new System.Drawing.Size(29, 18);
            this.BtnAdd2.TabIndex = 5;
            this.BtnAdd2.Text = ">";
            this.BtnAdd2.UseVisualStyleBackColor = true;
            //
            // LvScheduleinstanceOnSheet
            //
            this.LvScheduleinstanceOnSheet.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { this.ColumnHeader1, this.ColumnHeader2, this.ColumnHeader3, this.ColumnHeader4 });
            this.LvScheduleinstanceOnSheet.FullRowSelect = true;
            this.LvScheduleinstanceOnSheet.GridLines = true;
            this.LvScheduleinstanceOnSheet.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.LvScheduleinstanceOnSheet.HideSelection = false;
            this.LvScheduleinstanceOnSheet.Location = new System.Drawing.Point(462, 43);
            this.LvScheduleinstanceOnSheet.Margin = new System.Windows.Forms.Padding(2);
            this.LvScheduleinstanceOnSheet.Name = "LvScheduleinstanceOnSheet";
            this.LvScheduleinstanceOnSheet.Size = new System.Drawing.Size(234, 112);
            this.LvScheduleinstanceOnSheet.TabIndex = 8;
            this.LvScheduleinstanceOnSheet.UseCompatibleStateImageBehavior = false;
            this.LvScheduleinstanceOnSheet.View = System.Windows.Forms.View.Details;
            //
            // ColumnHeader1
            //
            this.ColumnHeader1.DisplayIndex = 3;
            this.ColumnHeader1.Text = global::ADSK.ViewExtension.SheetLayout.Resources.Text.TXT_COL_ID;
            //
            // ColumnHeader2
            //
            this.ColumnHeader2.DisplayIndex = 0;
            this.ColumnHeader2.Text = global::ADSK.ViewExtension.SheetLayout.Resources.Text.TXT_COL_SHEET;
            this.ColumnHeader2.Width = 40;
            //
            // ColumnHeader3
            //
            this.ColumnHeader3.DisplayIndex = 1;
            this.ColumnHeader3.Text = global::ADSK.ViewExtension.SheetLayout.Resources.Text.TXT_COL_NUMBER;
            this.ColumnHeader3.Width = 40;
            //
            // ColumnHeader4
            //
            this.ColumnHeader4.DisplayIndex = 2;
            this.ColumnHeader4.Text = global::ADSK.ViewExtension.SheetLayout.Resources.Text.TXT_COL_NAME;
            this.ColumnHeader4.Width = 200;
            //
            // LblScheduleInfo
            //
            this.LblScheduleInfo.AutoSize = true;
            this.LblScheduleInfo.Location = new System.Drawing.Point(172, 164);
            this.LblScheduleInfo.Name = "LblScheduleInfo";
            this.LblScheduleInfo.Size = new System.Drawing.Size(185, 12);
            this.LblScheduleInfo.TabIndex = 4;
            this.LblScheduleInfo.Text = global::ADSK.ViewExtension.SheetLayout.Resources.Text.TXT_SCHEDULE_INFO;
            //
            // Label6
            //
            this.Label6.AutoSize = true;
            this.Label6.Location = new System.Drawing.Point(172, 22);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(84, 12);
            this.Label6.TabIndex = 2;
            this.Label6.Text = global::ADSK.ViewExtension.SheetLayout.Resources.Text.TXT_LABEL_SCHEDULES_TO_PLACE;
            //
            // Label5
            //
            this.Label5.AutoSize = true;
            this.Label5.Location = new System.Drawing.Point(4, 22);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(91, 12);
            this.Label5.TabIndex = 0;
            this.Label5.Text = global::ADSK.ViewExtension.SheetLayout.Resources.Text.TXT_LABEL_SCHEDULES_ON_SHEET;
            //
            // LbxSchedules
            //
            this.LbxSchedules.FormattingEnabled = true;
            this.LbxSchedules.ItemHeight = 12;
            this.LbxSchedules.Location = new System.Drawing.Point(173, 43);
            this.LbxSchedules.Margin = new System.Windows.Forms.Padding(2);
            this.LbxSchedules.Name = "LbxSchedules";
            this.LbxSchedules.Size = new System.Drawing.Size(237, 112);
            this.LbxSchedules.TabIndex = 3;
            //
            // LbxScheduleInstances
            //
            this.LbxScheduleInstances.FormattingEnabled = true;
            this.LbxScheduleInstances.ItemHeight = 12;
            this.LbxScheduleInstances.Location = new System.Drawing.Point(4, 43);
            this.LbxScheduleInstances.Margin = new System.Windows.Forms.Padding(2);
            this.LbxScheduleInstances.Name = "LbxScheduleInstances";
            this.LbxScheduleInstances.Size = new System.Drawing.Size(146, 112);
            this.LbxScheduleInstances.TabIndex = 1;
            //
            // Label7
            //
            this.Label7.AutoSize = true;
            this.Label7.Location = new System.Drawing.Point(407, 543);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(139, 12);
            this.Label7.TabIndex = 19;
            this.Label7.Text = global::ADSK.ViewExtension.SheetLayout.Resources.Text.TXT_CREDIT;
            this.Label7.Visible = false;
            //
            // DlgSheetLayout
            //
            this.AcceptButton = this.OK_Button;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.Cancel_Button;
            this.ClientSize = new System.Drawing.Size(733, 574);
            this.Controls.Add(this.Label7);
            this.Controls.Add(this.GrpSchedule);
            this.Controls.Add(this.ChkAddSameLeg);
            this.Controls.Add(this.GrpViewports);
            this.Controls.Add(this.TableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgSheetLayout";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DlgSheetLayout";
            this.TableLayoutPanel1.ResumeLayout(false);
            this.GrpViewports.ResumeLayout(false);
            this.GrpViewports.PerformLayout();
            this.GrpSchedule.ResumeLayout(false);
            this.GrpSchedule.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
        private System.Windows.Forms.Button OK_Button;
        private System.Windows.Forms.Button Cancel_Button;
        private System.Windows.Forms.Button BtnUP;
        private System.Windows.Forms.Button BtnDN;
        private System.Windows.Forms.ListBox LbxViews;
        private System.Windows.Forms.Label Label3;
        private System.Windows.Forms.ComboBox CbxViewFamilyType;
        private System.Windows.Forms.Label Label2;
        private System.Windows.Forms.ComboBox CbxViewType;
        private System.Windows.Forms.Label Label4;
        private System.Windows.Forms.ComboBox CbxDicipline;
        private System.Windows.Forms.Button BtnAdd;
        private System.Windows.Forms.Button BtnRmv;
        private System.Windows.Forms.Button BtnUp2;
        private System.Windows.Forms.Button BtnDn2;
        private System.Windows.Forms.CheckBox ChkAddSameLeg;
        private System.Windows.Forms.GroupBox GrpViewports;
        private System.Windows.Forms.Label Label1;
        private System.Windows.Forms.ListBox LbxViewports;
        private System.Windows.Forms.ListView LvViewOnSheet;
        private System.Windows.Forms.ColumnHeader ColViewID;
        private System.Windows.Forms.ColumnHeader ColViewPage;
        private System.Windows.Forms.ColumnHeader ColViewNo;
        private System.Windows.Forms.ColumnHeader ColViewName;
        private System.Windows.Forms.GroupBox GrpSchedule;
        private System.Windows.Forms.ListBox LbxScheduleInstances;
        private System.Windows.Forms.ListBox LbxSchedules;
        private System.Windows.Forms.Button BtmRmv;
        private System.Windows.Forms.Button BtnAdd2;
        private System.Windows.Forms.ListView LvScheduleinstanceOnSheet;
        private System.Windows.Forms.ColumnHeader ColumnHeader1;
        private System.Windows.Forms.ColumnHeader ColumnHeader2;
        private System.Windows.Forms.ColumnHeader ColumnHeader3;
        private System.Windows.Forms.ColumnHeader ColumnHeader4;
        private System.Windows.Forms.Button BtnUP3;
        private System.Windows.Forms.Button BtnDN3;
        private System.Windows.Forms.Label Label10;
        private System.Windows.Forms.Label Label9;
        private System.Windows.Forms.Label LblSchNewSheet;
        private System.Windows.Forms.Label LblScheduleInfo;
        private System.Windows.Forms.Label Label6;
        private System.Windows.Forms.Label Label5;
        private System.Windows.Forms.Label Label7;
    }
}

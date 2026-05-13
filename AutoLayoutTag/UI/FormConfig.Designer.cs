using System.Windows.Forms;

namespace ADSK.JExtRAC.AutoLayoutTag.UI
{
  partial class FormConfig
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSetTag = new System.Windows.Forms.Button();
            this.tabAutomaticTag = new System.Windows.Forms.TabControl();
            this.tabSettingCondition = new System.Windows.Forms.TabPage();
            this.gpbHandlePresetTag = new System.Windows.Forms.GroupBox();
            this.rdbOderMore = new System.Windows.Forms.RadioButton();
            this.rdbReset = new System.Windows.Forms.RadioButton();
            this.rdbOnlyNewTag = new System.Windows.Forms.RadioButton();
            this.gpbAreaPremises = new System.Windows.Forms.GroupBox();
            this.btnSetArea = new System.Windows.Forms.Button();
            this.rdbAutoJudgment = new System.Windows.Forms.RadioButton();
            this.rdbSetByHand = new System.Windows.Forms.RadioButton();
            this.gpbPosittionTag = new System.Windows.Forms.GroupBox();
            this.cbkLeftRight = new System.Windows.Forms.CheckBox();
            this.cbkTopBottom = new System.Windows.Forms.CheckBox();
            this.gpbObject = new System.Windows.Forms.GroupBox();
            this.lblSelectionNumber = new System.Windows.Forms.Label();
            this.dgvCategory = new System.Windows.Forms.DataGridView();
            this.cbkCategory = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.nameCategory = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnSelectObject = new System.Windows.Forms.Button();
            this.rdbAllCategory = new System.Windows.Forms.RadioButton();
            this.rdbSelectObject = new System.Windows.Forms.RadioButton();
            this.tabSettingTag = new System.Windows.Forms.TabPage();
            this.ScrollBarVertical = new System.Windows.Forms.VScrollBar();
            this.dgvSaveSetting = new System.Windows.Forms.DataGridView();
            this.columnCategory = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnTag = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.lblValue = new System.Windows.Forms.Label();
            this.btnSaveSetting = new System.Windows.Forms.Button();
            this.lblViewTemplate = new System.Windows.Forms.Label();
            this.dataGridViewCheckBoxColumn1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.gpbLearderLine = new System.Windows.Forms.GroupBox();
            this.rdbHasLeader = new System.Windows.Forms.RadioButton();
            this.rdbNoLeader = new System.Windows.Forms.RadioButton();
            this.tabAutomaticTag.SuspendLayout();
            this.tabSettingCondition.SuspendLayout();
            this.gpbHandlePresetTag.SuspendLayout();
            this.gpbAreaPremises.SuspendLayout();
            this.gpbPosittionTag.SuspendLayout();
            this.gpbObject.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCategory)).BeginInit();
            this.tabSettingTag.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSaveSetting)).BeginInit();
            this.gpbLearderLine.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(327, 627);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 25);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSetTag
            // 
            this.btnSetTag.Location = new System.Drawing.Point(246, 627);
            this.btnSetTag.Name = "btnSetTag";
            this.btnSetTag.Size = new System.Drawing.Size(75, 25);
            this.btnSetTag.TabIndex = 5;
            this.btnSetTag.Text = "btnSetTag";
            this.btnSetTag.UseVisualStyleBackColor = true;
            this.btnSetTag.Click += new System.EventHandler(this.btnSetTag_Click);
            // 
            // tabAutomaticTag
            // 
            this.tabAutomaticTag.Controls.Add(this.tabSettingCondition);
            this.tabAutomaticTag.Controls.Add(this.tabSettingTag);
            this.tabAutomaticTag.Location = new System.Drawing.Point(6, 12);
            this.tabAutomaticTag.Name = "tabAutomaticTag";
            this.tabAutomaticTag.SelectedIndex = 0;
            this.tabAutomaticTag.Size = new System.Drawing.Size(408, 609);
            this.tabAutomaticTag.TabIndex = 7;
            // 
            // tabSettingCondition
            // 
            this.tabSettingCondition.BackColor = System.Drawing.Color.Transparent;
            this.tabSettingCondition.Controls.Add(this.gpbLearderLine);
            this.tabSettingCondition.Controls.Add(this.gpbHandlePresetTag);
            this.tabSettingCondition.Controls.Add(this.gpbAreaPremises);
            this.tabSettingCondition.Controls.Add(this.gpbPosittionTag);
            this.tabSettingCondition.Controls.Add(this.gpbObject);
            this.tabSettingCondition.Location = new System.Drawing.Point(4, 22);
            this.tabSettingCondition.Name = "tabSettingCondition";
            this.tabSettingCondition.Padding = new System.Windows.Forms.Padding(3);
            this.tabSettingCondition.Size = new System.Drawing.Size(400, 583);
            this.tabSettingCondition.TabIndex = 0;
            this.tabSettingCondition.Text = "tabSettingCondition";
            // 
            // gpbHandlePresetTag
            // 
            this.gpbHandlePresetTag.Controls.Add(this.rdbOderMore);
            this.gpbHandlePresetTag.Controls.Add(this.rdbReset);
            this.gpbHandlePresetTag.Controls.Add(this.rdbOnlyNewTag);
            this.gpbHandlePresetTag.Location = new System.Drawing.Point(7, 491);
            this.gpbHandlePresetTag.Name = "gpbHandlePresetTag";
            this.gpbHandlePresetTag.Size = new System.Drawing.Size(388, 80);
            this.gpbHandlePresetTag.TabIndex = 3;
            this.gpbHandlePresetTag.TabStop = false;
            this.gpbHandlePresetTag.Text = "gpbHandlePresetTag";
            // 
            // rdbOderMore
            // 
            this.rdbOderMore.AutoSize = true;
            this.rdbOderMore.Location = new System.Drawing.Point(292, 36);
            this.rdbOderMore.Name = "rdbOderMore";
            this.rdbOderMore.Size = new System.Drawing.Size(87, 17);
            this.rdbOderMore.TabIndex = 4;
            this.rdbOderMore.TabStop = true;
            this.rdbOderMore.Text = "rdbOderMore";
            this.rdbOderMore.UseVisualStyleBackColor = true;
            // 
            // rdbReset
            // 
            this.rdbReset.AutoSize = true;
            this.rdbReset.Location = new System.Drawing.Point(158, 36);
            this.rdbReset.Name = "rdbReset";
            this.rdbReset.Size = new System.Drawing.Size(68, 17);
            this.rdbReset.TabIndex = 3;
            this.rdbReset.TabStop = true;
            this.rdbReset.Text = "rdbReset";
            this.rdbReset.UseVisualStyleBackColor = true;
            // 
            // rdbOnlyNewTag
            // 
            this.rdbOnlyNewTag.AutoSize = true;
            this.rdbOnlyNewTag.Location = new System.Drawing.Point(18, 36);
            this.rdbOnlyNewTag.Name = "rdbOnlyNewTag";
            this.rdbOnlyNewTag.Size = new System.Drawing.Size(102, 17);
            this.rdbOnlyNewTag.TabIndex = 2;
            this.rdbOnlyNewTag.TabStop = true;
            this.rdbOnlyNewTag.Text = "rdbOnlyNewTag";
            this.rdbOnlyNewTag.UseVisualStyleBackColor = true;
            // 
            // gpbAreaPremises
            // 
            this.gpbAreaPremises.Controls.Add(this.btnSetArea);
            this.gpbAreaPremises.Controls.Add(this.rdbAutoJudgment);
            this.gpbAreaPremises.Controls.Add(this.rdbSetByHand);
            this.gpbAreaPremises.Location = new System.Drawing.Point(7, 405);
            this.gpbAreaPremises.Name = "gpbAreaPremises";
            this.gpbAreaPremises.Size = new System.Drawing.Size(388, 80);
            this.gpbAreaPremises.TabIndex = 2;
            this.gpbAreaPremises.TabStop = false;
            this.gpbAreaPremises.Text = "gpbAreaPremises";
            // 
            // btnSetArea
            // 
            this.btnSetArea.Location = new System.Drawing.Point(292, 35);
            this.btnSetArea.Name = "btnSetArea";
            this.btnSetArea.Size = new System.Drawing.Size(90, 23);
            this.btnSetArea.TabIndex = 3;
            this.btnSetArea.Text = "btnSetArea";
            this.btnSetArea.UseVisualStyleBackColor = true;
            this.btnSetArea.Click += new System.EventHandler(this.btnSetArea_Click);
            // 
            // rdbAutoJudgment
            // 
            this.rdbAutoJudgment.AutoSize = true;
            this.rdbAutoJudgment.Location = new System.Drawing.Point(18, 36);
            this.rdbAutoJudgment.Name = "rdbAutoJudgment";
            this.rdbAutoJudgment.Size = new System.Drawing.Size(108, 17);
            this.rdbAutoJudgment.TabIndex = 1;
            this.rdbAutoJudgment.TabStop = true;
            this.rdbAutoJudgment.Text = "rdbAutoJudgment";
            this.rdbAutoJudgment.UseVisualStyleBackColor = true;
            this.rdbAutoJudgment.Click += new System.EventHandler(this.rdbAutoJudgment_Click);
            // 
            // rdbSetByHand
            // 
            this.rdbSetByHand.AutoSize = true;
            this.rdbSetByHand.Location = new System.Drawing.Point(158, 36);
            this.rdbSetByHand.Name = "rdbSetByHand";
            this.rdbSetByHand.Size = new System.Drawing.Size(94, 17);
            this.rdbSetByHand.TabIndex = 0;
            this.rdbSetByHand.TabStop = true;
            this.rdbSetByHand.Text = "rdbSetByHand";
            this.rdbSetByHand.UseVisualStyleBackColor = true;
            this.rdbSetByHand.Click += new System.EventHandler(this.rdbSetByHand_Click);
            // 
            // gpbPosittionTag
            // 
            this.gpbPosittionTag.Controls.Add(this.cbkLeftRight);
            this.gpbPosittionTag.Controls.Add(this.cbkTopBottom);
            this.gpbPosittionTag.Location = new System.Drawing.Point(7, 233);
            this.gpbPosittionTag.Name = "gpbPosittionTag";
            this.gpbPosittionTag.Size = new System.Drawing.Size(388, 80);
            this.gpbPosittionTag.TabIndex = 1;
            this.gpbPosittionTag.TabStop = false;
            this.gpbPosittionTag.Text = "gpbPosittionTag";
            // 
            // cbkLeftRight
            // 
            this.cbkLeftRight.AutoSize = true;
            this.cbkLeftRight.Location = new System.Drawing.Point(18, 36);
            this.cbkLeftRight.Name = "cbkLeftRight";
            this.cbkLeftRight.Size = new System.Drawing.Size(87, 17);
            this.cbkLeftRight.TabIndex = 1;
            this.cbkLeftRight.Text = "cbkLeftRight";
            this.cbkLeftRight.UseVisualStyleBackColor = true;
            // 
            // cbkTopBottom
            // 
            this.cbkTopBottom.AutoSize = true;
            this.cbkTopBottom.Location = new System.Drawing.Point(158, 36);
            this.cbkTopBottom.Name = "cbkTopBottom";
            this.cbkTopBottom.Size = new System.Drawing.Size(96, 17);
            this.cbkTopBottom.TabIndex = 0;
            this.cbkTopBottom.Text = "cbkTopBottom";
            this.cbkTopBottom.UseVisualStyleBackColor = true;
            // 
            // gpbObject
            // 
            this.gpbObject.Controls.Add(this.lblSelectionNumber);
            this.gpbObject.Controls.Add(this.dgvCategory);
            this.gpbObject.Controls.Add(this.btnSelectObject);
            this.gpbObject.Controls.Add(this.rdbAllCategory);
            this.gpbObject.Controls.Add(this.rdbSelectObject);
            this.gpbObject.Location = new System.Drawing.Point(6, 6);
            this.gpbObject.Name = "gpbObject";
            this.gpbObject.Size = new System.Drawing.Size(388, 221);
            this.gpbObject.TabIndex = 0;
            this.gpbObject.TabStop = false;
            this.gpbObject.Text = "gpbObject";
            // 
            // lblSelectionNumber
            // 
            this.lblSelectionNumber.AutoSize = true;
            this.lblSelectionNumber.Location = new System.Drawing.Point(158, 24);
            this.lblSelectionNumber.Name = "lblSelectionNumber";
            this.lblSelectionNumber.Size = new System.Drawing.Size(98, 13);
            this.lblSelectionNumber.TabIndex = 5;
            this.lblSelectionNumber.Text = "lblSelectionNumber";
            this.lblSelectionNumber.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dgvCategory
            // 
            this.dgvCategory.AllowUserToAddRows = false;
            this.dgvCategory.AllowUserToDeleteRows = false;
            this.dgvCategory.AllowUserToResizeColumns = false;
            this.dgvCategory.AllowUserToResizeRows = false;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.dgvCategory.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvCategory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvCategory.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvCategory.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvCategory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCategory.ColumnHeadersVisible = false;
            this.dgvCategory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cbkCategory,
            this.nameCategory});
            this.dgvCategory.Location = new System.Drawing.Point(39, 79);
            this.dgvCategory.MultiSelect = false;
            this.dgvCategory.Name = "dgvCategory";
            this.dgvCategory.RowHeadersVisible = false;
            this.dgvCategory.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvCategory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCategory.Size = new System.Drawing.Size(343, 134);
            this.dgvCategory.TabIndex = 4;
            this.dgvCategory.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCategory_CellClick);
            // 
            // cbkCategory
            // 
            this.cbkCategory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbkCategory.HeaderText = "";
            this.cbkCategory.Name = "cbkCategory";
            this.cbkCategory.Width = 28;
            // 
            // nameCategory
            // 
            this.nameCategory.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.nameCategory.DefaultCellStyle = dataGridViewCellStyle8;
            this.nameCategory.HeaderText = "";
            this.nameCategory.Name = "nameCategory";
            this.nameCategory.ReadOnly = true;
            // 
            // btnSelectObject
            // 
            this.btnSelectObject.Location = new System.Drawing.Point(292, 19);
            this.btnSelectObject.Name = "btnSelectObject";
            this.btnSelectObject.Size = new System.Drawing.Size(90, 23);
            this.btnSelectObject.TabIndex = 2;
            this.btnSelectObject.Text = "btnSelectObject";
            this.btnSelectObject.UseVisualStyleBackColor = true;
            this.btnSelectObject.Click += new System.EventHandler(this.btnSelectObject_Click);
            // 
            // rdbAllCategory
            // 
            this.rdbAllCategory.AutoSize = true;
            this.rdbAllCategory.Location = new System.Drawing.Point(18, 56);
            this.rdbAllCategory.Name = "rdbAllCategory";
            this.rdbAllCategory.Size = new System.Drawing.Size(93, 17);
            this.rdbAllCategory.TabIndex = 1;
            this.rdbAllCategory.TabStop = true;
            this.rdbAllCategory.Text = "rdbAllCategory";
            this.rdbAllCategory.UseVisualStyleBackColor = true;
            this.rdbAllCategory.Click += new System.EventHandler(this.rdbAllCategory_Click);
            // 
            // rdbSelectObject
            // 
            this.rdbSelectObject.AutoSize = true;
            this.rdbSelectObject.Location = new System.Drawing.Point(18, 23);
            this.rdbSelectObject.Name = "rdbSelectObject";
            this.rdbSelectObject.Size = new System.Drawing.Size(101, 17);
            this.rdbSelectObject.TabIndex = 0;
            this.rdbSelectObject.TabStop = true;
            this.rdbSelectObject.Text = "rdbSelectObject";
            this.rdbSelectObject.UseVisualStyleBackColor = true;
            this.rdbSelectObject.Click += new System.EventHandler(this.rdbSelectObject_Click);
            // 
            // tabSettingTag
            // 
            this.tabSettingTag.BackColor = System.Drawing.SystemColors.Control;
            this.tabSettingTag.Controls.Add(this.ScrollBarVertical);
            this.tabSettingTag.Controls.Add(this.dgvSaveSetting);
            this.tabSettingTag.Controls.Add(this.lblValue);
            this.tabSettingTag.Controls.Add(this.btnSaveSetting);
            this.tabSettingTag.Controls.Add(this.lblViewTemplate);
            this.tabSettingTag.Location = new System.Drawing.Point(4, 22);
            this.tabSettingTag.Name = "tabSettingTag";
            this.tabSettingTag.Padding = new System.Windows.Forms.Padding(3);
            this.tabSettingTag.Size = new System.Drawing.Size(400, 506);
            this.tabSettingTag.TabIndex = 1;
            this.tabSettingTag.Text = "tabSettingTag";
            // 
            // ScrollBarVertical
            // 
            this.ScrollBarVertical.Location = new System.Drawing.Point(378, 6);
            this.ScrollBarVertical.Maximum = 35;
            this.ScrollBarVertical.Name = "ScrollBarVertical";
            this.ScrollBarVertical.Size = new System.Drawing.Size(16, 173);
            this.ScrollBarVertical.TabIndex = 5;
            this.ScrollBarVertical.Scroll += new System.Windows.Forms.ScrollEventHandler(this.ScrollBarVertical_Scroll);
            // 
            // dgvSaveSetting
            // 
            this.dgvSaveSetting.AllowUserToAddRows = false;
            this.dgvSaveSetting.AllowUserToDeleteRows = false;
            this.dgvSaveSetting.AllowUserToResizeRows = false;
            this.dgvSaveSetting.BackgroundColor = System.Drawing.Color.White;
            this.dgvSaveSetting.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSaveSetting.ColumnHeadersVisible = false;
            this.dgvSaveSetting.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnCategory,
            this.columnTag});
            this.dgvSaveSetting.EnableHeadersVisualStyles = false;
            this.dgvSaveSetting.Location = new System.Drawing.Point(6, 6);
            this.dgvSaveSetting.Name = "dgvSaveSetting";
            this.dgvSaveSetting.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvSaveSetting.RowHeadersVisible = false;
            this.dgvSaveSetting.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.dgvSaveSetting.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSaveSetting.Size = new System.Drawing.Size(369, 173);
            this.dgvSaveSetting.TabIndex = 4;
            this.dgvSaveSetting.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSaveSetting_CellClick);
            this.dgvSaveSetting.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvSaveSetting_DataError);
            this.dgvSaveSetting.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dgvSaveSetting_Scroll);
            this.dgvSaveSetting.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.dgvSaveSetting_MouseWheel);
            // 
            // columnCategory
            // 
            this.columnCategory.HeaderText = "";
            this.columnCategory.Name = "columnCategory";
            this.columnCategory.ReadOnly = true;
            this.columnCategory.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.columnCategory.Width = 125;
            // 
            // columnTag
            // 
            this.columnTag.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.columnTag.HeaderText = "";
            this.columnTag.Name = "columnTag";
            // 
            // lblValue
            // 
            this.lblValue.AutoSize = true;
            this.lblValue.Location = new System.Drawing.Point(105, 190);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(44, 13);
            this.lblValue.TabIndex = 3;
            this.lblValue.Text = "lblValue";
            // 
            // btnSaveSetting
            // 
            this.btnSaveSetting.Location = new System.Drawing.Point(283, 185);
            this.btnSaveSetting.Name = "btnSaveSetting";
            this.btnSaveSetting.Size = new System.Drawing.Size(75, 23);
            this.btnSaveSetting.TabIndex = 2;
            this.btnSaveSetting.Text = "btnSaveSetting";
            this.btnSaveSetting.UseVisualStyleBackColor = true;
            this.btnSaveSetting.Click += new System.EventHandler(this.btnSaveSetting_Click);
            // 
            // lblViewTemplate
            // 
            this.lblViewTemplate.AutoSize = true;
            this.lblViewTemplate.Location = new System.Drawing.Point(6, 190);
            this.lblViewTemplate.Name = "lblViewTemplate";
            this.lblViewTemplate.Size = new System.Drawing.Size(93, 13);
            this.lblViewTemplate.TabIndex = 1;
            this.lblViewTemplate.Text = "lblViewTemplate : ";
            // 
            // dataGridViewCheckBoxColumn1
            // 
            this.dataGridViewCheckBoxColumn1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dataGridViewCheckBoxColumn1.HeaderText = "";
            this.dataGridViewCheckBoxColumn1.Name = "dataGridViewCheckBoxColumn1";
            this.dataGridViewCheckBoxColumn1.Width = 28;
            // 
            // gpbLearderLine
            // 
            this.gpbLearderLine.Controls.Add(this.rdbHasLeader);
            this.gpbLearderLine.Controls.Add(this.rdbNoLeader);
            this.gpbLearderLine.Location = new System.Drawing.Point(6, 319);
            this.gpbLearderLine.Name = "gpbLearderLine";
            this.gpbLearderLine.Size = new System.Drawing.Size(388, 80);
            this.gpbLearderLine.TabIndex = 4;
            this.gpbLearderLine.TabStop = false;
            this.gpbLearderLine.Text = "gpbLearderLine";
            // 
            // rdbHasLeader
            // 
            this.rdbHasLeader.AutoSize = true;
            this.rdbHasLeader.Location = new System.Drawing.Point(18, 36);
            this.rdbHasLeader.Name = "rdbHasLeader";
            this.rdbHasLeader.Size = new System.Drawing.Size(92, 17);
            this.rdbHasLeader.TabIndex = 1;
            this.rdbHasLeader.TabStop = true;
            this.rdbHasLeader.Text = "rdbHasLeader";
            this.rdbHasLeader.UseVisualStyleBackColor = true;
            // 
            // rdbNoLeader
            // 
            this.rdbNoLeader.AutoSize = true;
            this.rdbNoLeader.Location = new System.Drawing.Point(158, 36);
            this.rdbNoLeader.Name = "rdbNoLeader";
            this.rdbNoLeader.Size = new System.Drawing.Size(87, 17);
            this.rdbNoLeader.TabIndex = 0;
            this.rdbNoLeader.TabStop = true;
            this.rdbNoLeader.Text = "rdbNoLeader";
            this.rdbNoLeader.UseVisualStyleBackColor = true;
            // 
            // FormConfig
            // 
            this.AcceptButton = this.btnSetTag;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(417, 658);
            this.Controls.Add(this.tabAutomaticTag);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSetTag);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AutomaticTag";
            this.tabAutomaticTag.ResumeLayout(false);
            this.tabSettingCondition.ResumeLayout(false);
            this.gpbHandlePresetTag.ResumeLayout(false);
            this.gpbHandlePresetTag.PerformLayout();
            this.gpbAreaPremises.ResumeLayout(false);
            this.gpbAreaPremises.PerformLayout();
            this.gpbPosittionTag.ResumeLayout(false);
            this.gpbPosittionTag.PerformLayout();
            this.gpbObject.ResumeLayout(false);
            this.gpbObject.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCategory)).EndInit();
            this.tabSettingTag.ResumeLayout(false);
            this.tabSettingTag.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSaveSetting)).EndInit();
            this.gpbLearderLine.ResumeLayout(false);
            this.gpbLearderLine.PerformLayout();
            this.ResumeLayout(false);

    }

    #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSetTag;
        private System.Windows.Forms.TabControl tabAutomaticTag;
        private System.Windows.Forms.TabPage tabSettingCondition;
        private System.Windows.Forms.TabPage tabSettingTag;
        private System.Windows.Forms.GroupBox gpbHandlePresetTag;
        private System.Windows.Forms.RadioButton rdbOderMore;
        private System.Windows.Forms.RadioButton rdbReset;
        private System.Windows.Forms.RadioButton rdbOnlyNewTag;
        private System.Windows.Forms.GroupBox gpbAreaPremises;
        private System.Windows.Forms.RadioButton rdbAutoJudgment;
        private System.Windows.Forms.RadioButton rdbSetByHand;
        private System.Windows.Forms.GroupBox gpbPosittionTag;
        private System.Windows.Forms.CheckBox cbkLeftRight;
        private System.Windows.Forms.CheckBox cbkTopBottom;
        private System.Windows.Forms.GroupBox gpbObject;
        private System.Windows.Forms.Button btnSelectObject;
        private System.Windows.Forms.RadioButton rdbAllCategory;
        private System.Windows.Forms.RadioButton rdbSelectObject;
        private System.Windows.Forms.Button btnSetArea;
        private System.Windows.Forms.Button btnSaveSetting;
        private System.Windows.Forms.Label lblViewTemplate;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.DataGridView dgvSaveSetting;
        private DataGridView dgvCategory;
        private DataGridViewTextBoxColumn columnCategory;
        private DataGridViewComboBoxColumn columnTag;
        public Label lblSelectionNumber;
        private DataGridViewCheckBoxColumn cbkCategory;
        private DataGridViewTextBoxColumn nameCategory;
        private VScrollBar ScrollBarVertical;
        private DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn1;
        private GroupBox gpbLearderLine;
        private RadioButton rdbHasLeader;
        private RadioButton rdbNoLeader;
    }
}
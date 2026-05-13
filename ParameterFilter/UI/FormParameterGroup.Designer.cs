namespace ADSK.JExtRAC.ParameterFilter.UI
{
    partial class FormParameterGroup
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
            this.gboxSettingParameter = new System.Windows.Forms.GroupBox();
            this.btnUnCheck = new System.Windows.Forms.Button();
            this.btnCheckAll = new System.Windows.Forms.Button();
            this.dgvSetting = new System.Windows.Forms.DataGridView();
            this.dgvCbk1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dgvGroupName1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvCbk2 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dgvGroupName2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gboxSettingParameter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSetting)).BeginInit();
            this.SuspendLayout();
            // 
            // gboxSettingParameter
            // 
            this.gboxSettingParameter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gboxSettingParameter.Controls.Add(this.btnUnCheck);
            this.gboxSettingParameter.Controls.Add(this.btnCheckAll);
            this.gboxSettingParameter.Controls.Add(this.dgvSetting);
            this.gboxSettingParameter.Location = new System.Drawing.Point(4, 4);
            this.gboxSettingParameter.Name = "gboxSettingParameter";
            this.gboxSettingParameter.Size = new System.Drawing.Size(506, 573);
            this.gboxSettingParameter.TabIndex = 0;
            this.gboxSettingParameter.TabStop = false;
            this.gboxSettingParameter.Text = "gboxSettingParameter";
            // 
            // btnUnCheck
            // 
            this.btnUnCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUnCheck.Location = new System.Drawing.Point(397, 539);
            this.btnUnCheck.Name = "btnUnCheck";
            this.btnUnCheck.Size = new System.Drawing.Size(99, 23);
            this.btnUnCheck.TabIndex = 1;
            this.btnUnCheck.Text = "btnUnCheck";
            this.btnUnCheck.UseVisualStyleBackColor = true;
            this.btnUnCheck.Click += new System.EventHandler(this.btnUnCheck_Click);
            // 
            // btnCheckAll
            // 
            this.btnCheckAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCheckAll.Location = new System.Drawing.Point(293, 539);
            this.btnCheckAll.Name = "btnCheckAll";
            this.btnCheckAll.Size = new System.Drawing.Size(99, 23);
            this.btnCheckAll.TabIndex = 1;
            this.btnCheckAll.Text = "btnCheckAll";
            this.btnCheckAll.UseVisualStyleBackColor = true;
            this.btnCheckAll.Click += new System.EventHandler(this.btnCheckAll_Click);
            // 
            // dgvSetting
            // 
            this.dgvSetting.AllowUserToAddRows = false;
            this.dgvSetting.AllowUserToDeleteRows = false;
            this.dgvSetting.AllowUserToResizeColumns = false;
            this.dgvSetting.AllowUserToResizeRows = false;
            this.dgvSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvSetting.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dgvSetting.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvSetting.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSetting.ColumnHeadersVisible = false;
            this.dgvSetting.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvCbk1,
            this.dgvGroupName1,
            this.dgvCbk2,
            this.dgvGroupName2});
            this.dgvSetting.Location = new System.Drawing.Point(6, 16);
            this.dgvSetting.MultiSelect = false;
            this.dgvSetting.Name = "dgvSetting";
            this.dgvSetting.RowHeadersVisible = false;
            this.dgvSetting.Size = new System.Drawing.Size(494, 517);
            this.dgvSetting.TabIndex = 0;
            this.dgvSetting.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSetting_CellClick);
            this.dgvSetting.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgvSetting_CellPainting);
            // 
            // dgvCbk1
            // 
            this.dgvCbk1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgvCbk1.HeaderText = "dgvCbk1";
            this.dgvCbk1.Name = "dgvCbk1";
            this.dgvCbk1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvCbk1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.dgvCbk1.Width = 40;
            // 
            // dgvGroupName1
            // 
            this.dgvGroupName1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvGroupName1.HeaderText = "dgvGroupName1";
            this.dgvGroupName1.Name = "dgvGroupName1";
            this.dgvGroupName1.ReadOnly = true;
            this.dgvGroupName1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // dgvCbk2
            // 
            this.dgvCbk2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgvCbk2.HeaderText = "dgvCbk2";
            this.dgvCbk2.Name = "dgvCbk2";
            this.dgvCbk2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvCbk2.Width = 40;
            // 
            // dgvGroupName2
            // 
            this.dgvGroupName2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvGroupName2.HeaderText = "dgvGroupName2";
            this.dgvGroupName2.Name = "dgvGroupName2";
            this.dgvGroupName2.ReadOnly = true;
            this.dgvGroupName2.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnApply.Location = new System.Drawing.Point(299, 583);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(99, 23);
            this.btnApply.TabIndex = 1;
            this.btnApply.Text = "btnApply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(403, 583);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(99, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // FormParameterGroup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 621);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.gboxSettingParameter);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MinimumSize = new System.Drawing.Size(530, 660);
            this.Name = "FormParameterGroup";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormParameterGroup";
            this.Load += new System.EventHandler(this.FormParameterGroup_Load);
            this.gboxSettingParameter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSetting)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gboxSettingParameter;
        private System.Windows.Forms.Button btnUnCheck;
        private System.Windows.Forms.Button btnCheckAll;
        private System.Windows.Forms.DataGridView dgvSetting;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dgvCbk1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvGroupName1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dgvCbk2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvGroupName2;
    }
}
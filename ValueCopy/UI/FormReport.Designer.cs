namespace ADSK.JExtRAC.ValueCopy.UI
{
    partial class FormReport
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvLogData = new System.Windows.Forms.DataGridView();
            this.dgvLog_ElementId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvLog_FamilyName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvLog_TypeName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvLog_ParameterName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvLog_IconStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvLog_Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btOK = new System.Windows.Forms.Button();
            this.btShowLog = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLogData)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvLogData
            // 
            this.dgvLogData.AllowUserToAddRows = false;
            this.dgvLogData.AllowUserToDeleteRows = false;
            this.dgvLogData.AllowUserToResizeColumns = false;
            this.dgvLogData.AllowUserToResizeRows = false;
            this.dgvLogData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvLogData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvLogData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvLog_ElementId,
            this.dgvLog_FamilyName,
            this.dgvLog_TypeName,
            this.dgvLog_ParameterName,
            this.dgvLog_IconStatus,
            this.dgvLog_Status});
            this.dgvLogData.Location = new System.Drawing.Point(8, 8);
            this.dgvLogData.Name = "dgvLogData";
            this.dgvLogData.ReadOnly = true;
            this.dgvLogData.RowHeadersVisible = false;
            this.dgvLogData.Size = new System.Drawing.Size(807, 312);
            this.dgvLogData.TabIndex = 0;
            this.dgvLogData.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvLogData_CellFormatting);
            this.dgvLogData.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgvLogData_CellPainting);
            // 
            // dgvLog_ElementId
            // 
            this.dgvLog_ElementId.HeaderText = "dgvLog_ElementId";
            this.dgvLog_ElementId.Name = "dgvLog_ElementId";
            this.dgvLog_ElementId.ReadOnly = true;
            this.dgvLog_ElementId.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvLog_FamilyName
            // 
            this.dgvLog_FamilyName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvLog_FamilyName.FillWeight = 25F;
            this.dgvLog_FamilyName.HeaderText = "dgvLog_FamilyName";
            this.dgvLog_FamilyName.Name = "dgvLog_FamilyName";
            this.dgvLog_FamilyName.ReadOnly = true;
            this.dgvLog_FamilyName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvLog_TypeName
            // 
            this.dgvLog_TypeName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvLog_TypeName.FillWeight = 25F;
            this.dgvLog_TypeName.HeaderText = "dgvLog_TypeName";
            this.dgvLog_TypeName.Name = "dgvLog_TypeName";
            this.dgvLog_TypeName.ReadOnly = true;
            this.dgvLog_TypeName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvLog_ParameterName
            // 
            this.dgvLog_ParameterName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvLog_ParameterName.FillWeight = 50F;
            this.dgvLog_ParameterName.HeaderText = "dgvLog_ParameterName";
            this.dgvLog_ParameterName.Name = "dgvLog_ParameterName";
            this.dgvLog_ParameterName.ReadOnly = true;
            this.dgvLog_ParameterName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvLog_IconStatus
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dgvLog_IconStatus.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvLog_IconStatus.HeaderText = "dgvLog_IconStatus";
            this.dgvLog_IconStatus.Name = "dgvLog_IconStatus";
            this.dgvLog_IconStatus.ReadOnly = true;
            this.dgvLog_IconStatus.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dgvLog_IconStatus.Width = 40;
            // 
            // dgvLog_Status
            // 
            this.dgvLog_Status.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvLog_Status.HeaderText = "dgvLog_Status";
            this.dgvLog_Status.Name = "dgvLog_Status";
            this.dgvLog_Status.ReadOnly = true;
            this.dgvLog_Status.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // btOK
            // 
            this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btOK.Location = new System.Drawing.Point(742, 331);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 23);
            this.btOK.TabIndex = 1;
            this.btOK.Text = "btOK";
            this.btOK.UseVisualStyleBackColor = true;
            // 
            // btShowLog
            // 
            this.btShowLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btShowLog.Location = new System.Drawing.Point(8, 328);
            this.btShowLog.Name = "btShowLog";
            this.btShowLog.Size = new System.Drawing.Size(88, 23);
            this.btShowLog.TabIndex = 2;
            this.btShowLog.Text = "btShowLog";
            this.btShowLog.UseVisualStyleBackColor = true;
            this.btShowLog.Click += new System.EventHandler(this.btShowLog_Click);
            // 
            // FormReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(826, 364);
            this.Controls.Add(this.btShowLog);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.dgvLogData);
            this.MinimumSize = new System.Drawing.Size(700, 400);
            this.Name = "FormReport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormReport";
            this.Load += new System.EventHandler(this.FormReport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLogData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvLogData;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.Button btShowLog;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvLog_ElementId;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvLog_FamilyName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvLog_TypeName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvLog_ParameterName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvLog_IconStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvLog_Status;
    }
}
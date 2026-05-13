namespace STBLink
{
    partial class LogForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle() ;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( LogForm ) ) ;
            this.chkInfo = new System.Windows.Forms.CheckBox() ;
            this.chkWarning = new System.Windows.Forms.CheckBox() ;
            this.chkError = new System.Windows.Forms.CheckBox() ;
            this.dgvLog = new System.Windows.Forms.DataGridView() ;
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn() ;
            this.Column2 = new System.Windows.Forms.DataGridViewImageColumn() ;
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn() ;
            this.btnClose = new System.Windows.Forms.Button() ;
            this.btnSave = new System.Windows.Forms.Button() ;
            this.chkSaveOption = new System.Windows.Forms.CheckBox() ;
            ( (System.ComponentModel.ISupportInitialize)( this.dgvLog ) ).BeginInit() ;
            this.SuspendLayout() ;
            // 
            // chkInfo
            // 
            this.chkInfo.Appearance = System.Windows.Forms.Appearance.Button ;
            this.chkInfo.Checked = true ;
            this.chkInfo.CheckState = System.Windows.Forms.CheckState.Checked ;
            this.chkInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat ;
            this.chkInfo.Location = new System.Drawing.Point( 10, 10 ) ;
            this.chkInfo.Margin = new System.Windows.Forms.Padding( 0, 0, 1, 0 ) ;
            this.chkInfo.Name = "chkInfo" ;
            this.chkInfo.Size = new System.Drawing.Size( 100, 30 ) ;
            this.chkInfo.TabIndex = 0 ;
            this.chkInfo.Text = "Information" ;
            this.chkInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter ;
            this.chkInfo.UseVisualStyleBackColor = true ;
            this.chkInfo.CheckedChanged += new System.EventHandler( this.ChkInfo_CheckedChanged ) ;
            // 
            // chkWarning
            // 
            this.chkWarning.Appearance = System.Windows.Forms.Appearance.Button ;
            this.chkWarning.Checked = true ;
            this.chkWarning.CheckState = System.Windows.Forms.CheckState.Checked ;
            this.chkWarning.FlatStyle = System.Windows.Forms.FlatStyle.Flat ;
            this.chkWarning.Location = new System.Drawing.Point( 111, 10 ) ;
            this.chkWarning.Margin = new System.Windows.Forms.Padding( 0, 0, 1, 0 ) ;
            this.chkWarning.Name = "chkWarning" ;
            this.chkWarning.Size = new System.Drawing.Size( 100, 30 ) ;
            this.chkWarning.TabIndex = 1 ;
            this.chkWarning.Text = "Warning" ;
            this.chkWarning.TextAlign = System.Drawing.ContentAlignment.MiddleCenter ;
            this.chkWarning.UseVisualStyleBackColor = true ;
            this.chkWarning.CheckedChanged += new System.EventHandler( this.ChkInfo_CheckedChanged ) ;
            // 
            // chkError
            // 
            this.chkError.Appearance = System.Windows.Forms.Appearance.Button ;
            this.chkError.Checked = true ;
            this.chkError.CheckState = System.Windows.Forms.CheckState.Checked ;
            this.chkError.FlatStyle = System.Windows.Forms.FlatStyle.Flat ;
            this.chkError.Location = new System.Drawing.Point( 212, 10 ) ;
            this.chkError.Margin = new System.Windows.Forms.Padding( 0, 0, 1, 0 ) ;
            this.chkError.Name = "chkError" ;
            this.chkError.Size = new System.Drawing.Size( 100, 30 ) ;
            this.chkError.TabIndex = 2 ;
            this.chkError.Text = "Error" ;
            this.chkError.TextAlign = System.Drawing.ContentAlignment.MiddleCenter ;
            this.chkError.UseVisualStyleBackColor = true ;
            this.chkError.CheckedChanged += new System.EventHandler( this.ChkInfo_CheckedChanged ) ;
            // 
            // dgvLog
            // 
            this.dgvLog.AllowUserToAddRows = false ;
            this.dgvLog.AllowUserToDeleteRows = false ;
            this.dgvLog.AllowUserToResizeColumns = false ;
            this.dgvLog.AllowUserToResizeRows = false ;
            this.dgvLog.Anchor = System.Windows.Forms.AnchorStyles.None ;
            this.dgvLog.BackgroundColor = System.Drawing.Color.White ;
            this.dgvLog.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D ;
            this.dgvLog.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None ;
            this.dgvLog.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable ;
            this.dgvLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize ;
            this.dgvLog.ColumnHeadersVisible = false ;
            this.dgvLog.Columns.AddRange( new System.Windows.Forms.DataGridViewColumn[] { this.Column1, this.Column2, this.Column3 } ) ;
            this.dgvLog.GridColor = System.Drawing.SystemColors.WindowFrame ;
            this.dgvLog.Location = new System.Drawing.Point( 12, 45 ) ;
            this.dgvLog.MultiSelect = false ;
            this.dgvLog.Name = "dgvLog" ;
            this.dgvLog.ReadOnly = true ;
            this.dgvLog.RowHeadersVisible = false ;
            this.dgvLog.RowTemplate.ReadOnly = true ;
            this.dgvLog.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False ;
            this.dgvLog.ShowCellErrors = false ;
            this.dgvLog.ShowCellToolTips = false ;
            this.dgvLog.ShowEditingIcon = false ;
            this.dgvLog.ShowRowErrors = false ;
            this.dgvLog.Size = new System.Drawing.Size( 560, 371 ) ;
            this.dgvLog.TabIndex = 3 ;
            this.dgvLog.TabStop = false ;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Column1" ;
            this.Column1.Name = "Column1" ;
            this.Column1.ReadOnly = true ;
            this.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.False ;
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable ;
            this.Column1.Visible = false ;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Column2" ;
            this.Column2.Name = "Column2" ;
            this.Column2.ReadOnly = true ;
            this.Column2.Resizable = System.Windows.Forms.DataGridViewTriState.False ;
            this.Column2.Width = 25 ;
            // 
            // Column3
            // 
            dataGridViewCellStyle1.Font = new System.Drawing.Font( "Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) ) ;
            this.Column3.DefaultCellStyle = dataGridViewCellStyle1 ;
            this.Column3.HeaderText = "Column3" ;
            this.Column3.Name = "Column3" ;
            this.Column3.ReadOnly = true ;
            this.Column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable ;
            this.Column3.Width = 200 ;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.None ;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel ;
            this.btnClose.Location = new System.Drawing.Point( 497, 427 ) ;
            this.btnClose.Name = "btnClose" ;
            this.btnClose.Size = new System.Drawing.Size( 75, 23 ) ;
            this.btnClose.TabIndex = 4 ;
            this.btnClose.Text = "Close" ;
            this.btnClose.UseVisualStyleBackColor = true ;
            this.btnClose.Click += new System.EventHandler( this.BtnClose_Click ) ;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.None ;
            this.btnSave.Location = new System.Drawing.Point( 12, 427 ) ;
            this.btnSave.Name = "btnSave" ;
            this.btnSave.Size = new System.Drawing.Size( 75, 23 ) ;
            this.btnSave.TabIndex = 5 ;
            this.btnSave.Text = "Save" ;
            this.btnSave.UseVisualStyleBackColor = true ;
            this.btnSave.Click += new System.EventHandler( this.BtnSave_Click ) ;
            // 
            // chkSaveOption
            // 
            this.chkSaveOption.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) ) ;
            this.chkSaveOption.AutoSize = true ;
            this.chkSaveOption.Checked = true ;
            this.chkSaveOption.CheckState = System.Windows.Forms.CheckState.Checked ;
            this.chkSaveOption.Location = new System.Drawing.Point( 90, 431 ) ;
            this.chkSaveOption.Name = "chkSaveOption" ;
            this.chkSaveOption.Size = new System.Drawing.Size( 140, 16 ) ;
            this.chkSaveOption.TabIndex = 6 ;
            this.chkSaveOption.Text = "Include hidden rows when saving" ;
            this.chkSaveOption.UseVisualStyleBackColor = true ;
            this.chkSaveOption.Visible = false ;
            // 
            // LogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 12F ) ;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font ;
            this.CancelButton = this.btnClose ;
            this.ClientSize = new System.Drawing.Size( 584, 462 ) ;
            this.Controls.Add( this.chkSaveOption ) ;
            this.Controls.Add( this.btnSave ) ;
            this.Controls.Add( this.btnClose ) ;
            this.Controls.Add( this.dgvLog ) ;
            this.Controls.Add( this.chkError ) ;
            this.Controls.Add( this.chkWarning ) ;
            this.Controls.Add( this.chkInfo ) ;
            // this.Icon = ( (System.Drawing.Icon)( resources.GetObject( "$this.Icon" ) ) ) ;
            this.MinimizeBox = false ;
            this.MinimumSize = new System.Drawing.Size( 400, 400 ) ;
            this.Name = "LogForm" ;
            this.ShowIcon = false ;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen ;
            this.Text = "Conversion log" ;
            this.Activated += new System.EventHandler( this.LogForm_Activated ) ;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler( this.LogForm_FormClosed ) ;
            this.Load += new System.EventHandler( this.LogForm_Load ) ;
            this.ResizeEnd += new System.EventHandler( this.LogForm_ResizeEnd ) ;
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler( this.LogForm_HelpRequested ) ;
            ( (System.ComponentModel.ISupportInitialize)( this.dgvLog ) ).EndInit() ;
            this.ResumeLayout( false ) ;
            this.PerformLayout() ;
        }

        #endregion

        private System.Windows.Forms.CheckBox chkInfo;
        private System.Windows.Forms.CheckBox chkWarning;
        private System.Windows.Forms.CheckBox chkError;
        private System.Windows.Forms.DataGridView dgvLog;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox chkSaveOption;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewImageColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
    }
}
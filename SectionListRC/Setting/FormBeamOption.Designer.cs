namespace SectionListRC.Setting
{
    partial class FormBeamOption
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
            this.components = new System.ComponentModel.Container();
            this.grpBoxSelectType = new System.Windows.Forms.GroupBox();
            this.dgvItems = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grpBoxRange = new System.Windows.Forms.GroupBox();
            this.lblmm2 = new System.Windows.Forms.Label();
            this.lblmm1 = new System.Windows.Forms.Label();
            this.txtMaxWidth = new System.Windows.Forms.TextBox();
            this.txtMaxLength = new System.Windows.Forms.TextBox();
            this.lblMaxWidth = new System.Windows.Forms.Label();
            this.lblMaxHeight = new System.Windows.Forms.Label();
            this.chkRange = new System.Windows.Forms.CheckBox();
            this.rdoSelection = new System.Windows.Forms.RadioButton();
            this.rdoAll = new System.Windows.Forms.RadioButton();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.errorProviderInvalid = new System.Windows.Forms.ErrorProvider(this.components);
            this.grpBoxSelectType.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            this.grpBoxRange.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderInvalid)).BeginInit();
            this.SuspendLayout();
            // 
            // grpBoxSelectType
            // 
            this.grpBoxSelectType.Controls.Add(this.dgvItems);
            this.grpBoxSelectType.Location = new System.Drawing.Point(9, 9);
            this.grpBoxSelectType.Name = "grpBoxSelectType";
            this.grpBoxSelectType.Size = new System.Drawing.Size(302, 220);
            this.grpBoxSelectType.TabIndex = 0;
            this.grpBoxSelectType.TabStop = false;
            this.grpBoxSelectType.Text = "grpBoxSelectType";
            // 
            // dgvItems
            // 
            this.dgvItems.AllowUserToAddRows = false;
            this.dgvItems.AllowUserToResizeColumns = false;
            this.dgvItems.AllowUserToResizeRows = false;
            this.dgvItems.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvItems.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dgvItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.dgvItems.Location = new System.Drawing.Point(7, 18);
            this.dgvItems.Name = "dgvItems";
            this.dgvItems.RowHeadersWidth = 20;
            this.dgvItems.Size = new System.Drawing.Size(286, 190);
            this.dgvItems.TabIndex = 6;
            this.dgvItems.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgvItems_CellPainting);
            this.dgvItems.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvItems_CellValueChanged);
            this.dgvItems.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvItems_CurrentCellDirtyStateChanged);
            this.dgvItems.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dgvItems_RowPostPaint);
            // 
            // Column1
            // 
            this.Column1.FillWeight = 50.76143F;
            this.Column1.HeaderText = "";
            this.Column1.Name = "Column1";
            // 
            // Column2
            // 
            this.Column2.FillWeight = 149.2386F;
            this.Column2.HeaderText = "Target Type";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // grpBoxRange
            // 
            this.grpBoxRange.Controls.Add(this.lblmm2);
            this.grpBoxRange.Controls.Add(this.lblmm1);
            this.grpBoxRange.Controls.Add(this.txtMaxWidth);
            this.grpBoxRange.Controls.Add(this.txtMaxLength);
            this.grpBoxRange.Controls.Add(this.lblMaxWidth);
            this.grpBoxRange.Controls.Add(this.lblMaxHeight);
            this.grpBoxRange.Controls.Add(this.chkRange);
            this.grpBoxRange.Controls.Add(this.rdoSelection);
            this.grpBoxRange.Controls.Add(this.rdoAll);
            this.grpBoxRange.Location = new System.Drawing.Point(9, 235);
            this.grpBoxRange.Name = "grpBoxRange";
            this.grpBoxRange.Size = new System.Drawing.Size(302, 128);
            this.grpBoxRange.TabIndex = 1;
            this.grpBoxRange.TabStop = false;
            this.grpBoxRange.Text = "grpBoxRange";
            // 
            // lblmm2
            // 
            this.lblmm2.AutoSize = true;
            this.lblmm2.Location = new System.Drawing.Point(260, 87);
            this.lblmm2.Name = "lblmm2";
            this.lblmm2.Size = new System.Drawing.Size(41, 12);
            this.lblmm2.TabIndex = 8;
            this.lblmm2.Text = "lblmm2";
            // 
            // lblmm1
            // 
            this.lblmm1.AutoSize = true;
            this.lblmm1.Location = new System.Drawing.Point(260, 63);
            this.lblmm1.Name = "lblmm1";
            this.lblmm1.Size = new System.Drawing.Size(41, 12);
            this.lblmm1.TabIndex = 7;
            this.lblmm1.Text = "lblmm1";
            // 
            // txtMaxWidth
            // 
            this.txtMaxWidth.Location = new System.Drawing.Point(154, 84);
            this.txtMaxWidth.Name = "txtMaxWidth";
            this.txtMaxWidth.Size = new System.Drawing.Size(100, 19);
            this.txtMaxWidth.TabIndex = 6;
            this.txtMaxWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtMaxWidth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMaxWidth_KeyPress);
            this.txtMaxWidth.Leave += new System.EventHandler(this.txtMaxWidth_Leave);
            // 
            // txtMaxLength
            // 
            this.txtMaxLength.Location = new System.Drawing.Point(154, 60);
            this.txtMaxLength.Name = "txtMaxLength";
            this.txtMaxLength.Size = new System.Drawing.Size(100, 19);
            this.txtMaxLength.TabIndex = 5;
            this.txtMaxLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtMaxLength.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMaxLength_KeyPress);
            this.txtMaxLength.Leave += new System.EventHandler(this.txtMaxLength_Leave);
            // 
            // lblMaxWidth
            // 
            this.lblMaxWidth.AutoSize = true;
            this.lblMaxWidth.Location = new System.Drawing.Point(76, 84);
            this.lblMaxWidth.Name = "lblMaxWidth";
            this.lblMaxWidth.Size = new System.Drawing.Size(66, 12);
            this.lblMaxWidth.TabIndex = 4;
            this.lblMaxWidth.Text = "lblMaxWidth";
            // 
            // lblMaxHeight
            // 
            this.lblMaxHeight.AutoSize = true;
            this.lblMaxHeight.Location = new System.Drawing.Point(76, 63);
            this.lblMaxHeight.Name = "lblMaxHeight";
            this.lblMaxHeight.Size = new System.Drawing.Size(71, 12);
            this.lblMaxHeight.TabIndex = 3;
            this.lblMaxHeight.Text = "lblMaxHeight";
            // 
            // chkRange
            // 
            this.chkRange.AutoSize = true;
            this.chkRange.Location = new System.Drawing.Point(50, 40);
            this.chkRange.Name = "chkRange";
            this.chkRange.Size = new System.Drawing.Size(74, 16);
            this.chkRange.TabIndex = 2;
            this.chkRange.Text = "chkRange";
            this.chkRange.UseVisualStyleBackColor = true;
            this.chkRange.CheckedChanged += new System.EventHandler(this.chkRange_CheckedChanged);
            // 
            // rdoSelection
            // 
            this.rdoSelection.AutoSize = true;
            this.rdoSelection.Location = new System.Drawing.Point(28, 106);
            this.rdoSelection.Name = "rdoSelection";
            this.rdoSelection.Size = new System.Drawing.Size(86, 16);
            this.rdoSelection.TabIndex = 1;
            this.rdoSelection.Text = "rdoSelection";
            this.rdoSelection.UseVisualStyleBackColor = true;
            this.rdoSelection.CheckedChanged += new System.EventHandler(this.rdoSelection_CheckedChanged);
            // 
            // rdoAll
            // 
            this.rdoAll.AutoSize = true;
            this.rdoAll.Checked = true;
            this.rdoAll.Location = new System.Drawing.Point(28, 18);
            this.rdoAll.Name = "rdoAll";
            this.rdoAll.Size = new System.Drawing.Size(53, 16);
            this.rdoAll.TabIndex = 0;
            this.rdoAll.TabStop = true;
            this.rdoAll.Text = "rdoAll";
            this.rdoAll.UseVisualStyleBackColor = true;
            this.rdoAll.CheckedChanged += new System.EventHandler(this.rdoAll_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(236, 368);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 21);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(152, 368);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 21);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // errorProviderInvalid
            // 
            this.errorProviderInvalid.ContainerControl = this;
            // 
            // FormBeamOption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(319, 396);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.grpBoxRange);
            this.Controls.Add(this.grpBoxSelectType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormBeamOption";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Beam List - Generation Target Selection";
            this.Load += new System.EventHandler(this.FormBeamOption_Load);
            this.grpBoxSelectType.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            this.grpBoxRange.ResumeLayout(false);
            this.grpBoxRange.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderInvalid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpBoxSelectType;
        private System.Windows.Forms.GroupBox grpBoxRange;
        private System.Windows.Forms.RadioButton rdoSelection;
        private System.Windows.Forms.RadioButton rdoAll;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblmm2;
        private System.Windows.Forms.Label lblmm1;
        private System.Windows.Forms.TextBox txtMaxWidth;
        private System.Windows.Forms.TextBox txtMaxLength;
        private System.Windows.Forms.Label lblMaxWidth;
        private System.Windows.Forms.Label lblMaxHeight;
        private System.Windows.Forms.CheckBox chkRange;
        private System.Windows.Forms.DataGridView dgvItems;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.ErrorProvider errorProviderInvalid;
    }
}
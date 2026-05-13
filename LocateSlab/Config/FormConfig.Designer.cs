namespace ADSK.JExtRAC.LocateSlab.Config
{
    partial class FormConfig
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblSlabType = new System.Windows.Forms.Label();
            this.lblHeightOffset = new System.Windows.Forms.Label();
            this.lblHeightOffsetUnit = new System.Windows.Forms.Label();
            this.cboSlabType = new System.Windows.Forms.ComboBox();
            this.txtHeightOffset = new System.Windows.Forms.TextBox();
            this.lblDirectionAngle = new System.Windows.Forms.Label();
            this.cboDirectionAngle = new System.Windows.Forms.ComboBox();
            this.lblDegree = new System.Windows.Forms.Label();
            this.errPvd = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errPvd)).BeginInit();
            this.SuspendLayout();
            //
            // btnCancel
            //
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(337, 98);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 25);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            //
            // btnOK
            //
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.btnOK.Location = new System.Drawing.Point(256, 98);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 25);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            //
            // lblSlabType
            //
            this.lblSlabType.AutoSize = true;
            this.lblSlabType.Location = new System.Drawing.Point(12, 16);
            this.lblSlabType.Name = "lblSlabType";
            this.lblSlabType.Size = new System.Drawing.Size(62, 13);
            this.lblSlabType.TabIndex = 0;
            this.lblSlabType.Text = "lblSlabType";
            //
            // lblHeightOffset
            //
            this.lblHeightOffset.AutoSize = true;
            this.lblHeightOffset.Location = new System.Drawing.Point(12, 44);
            this.lblHeightOffset.Name = "lblHeightOffset";
            this.lblHeightOffset.Size = new System.Drawing.Size(76, 13);
            this.lblHeightOffset.TabIndex = 2;
            this.lblHeightOffset.Text = "lblHeightOffset";
            //
            // lblHeightOffsetUnit
            //
            this.lblHeightOffsetUnit.AutoSize = true;
            this.lblHeightOffsetUnit.Location = new System.Drawing.Point(213, 44);
            this.lblHeightOffsetUnit.Name = "lblHeightOffsetUnit";
            this.lblHeightOffsetUnit.Size = new System.Drawing.Size(95, 13);
            this.lblHeightOffsetUnit.TabIndex = 4;
            this.lblHeightOffsetUnit.Text = "lblHeightOffsetUnit";
            //
            // cboSlabType
            //
            this.cboSlabType.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.cboSlabType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSlabType.FormattingEnabled = true;
            this.cboSlabType.Location = new System.Drawing.Point(111, 13);
            this.cboSlabType.Name = "cboSlabType";
            this.cboSlabType.Size = new System.Drawing.Size(300, 21);
            this.cboSlabType.TabIndex = 1;
            //
            // txtHeightOffset
            //
            this.txtHeightOffset.Location = new System.Drawing.Point(111, 41);
            this.txtHeightOffset.Name = "txtHeightOffset";
            this.txtHeightOffset.Size = new System.Drawing.Size(100, 20);
            this.txtHeightOffset.TabIndex = 3;
            this.txtHeightOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            //
            // lblDirectionAngle
            //
            this.lblDirectionAngle.AutoSize = true;
            this.lblDirectionAngle.Location = new System.Drawing.Point(12, 74);
            this.lblDirectionAngle.Name = "lblDirectionAngle";
            this.lblDirectionAngle.Size = new System.Drawing.Size(86, 13);
            this.lblDirectionAngle.TabIndex = 7;
            this.lblDirectionAngle.Text = "lblDirectionAngle";
            //
            // cboDirectionAngle
            //
            this.cboDirectionAngle.FormattingEnabled = true;
            this.cboDirectionAngle.Location = new System.Drawing.Point(111, 71);
            this.cboDirectionAngle.Name = "cboDirectionAngle";
            this.cboDirectionAngle.Size = new System.Drawing.Size(100, 21);
            this.cboDirectionAngle.TabIndex = 8;
            this.cboDirectionAngle.Validated += new System.EventHandler(this.cboDirectionAngle_Validated);
            //
            // lblDegree
            //
            this.lblDegree.AutoSize = true;
            this.lblDegree.Location = new System.Drawing.Point(213, 71);
            this.lblDegree.Name = "lblDegree";
            this.lblDegree.Size = new System.Drawing.Size(52, 13);
            this.lblDegree.TabIndex = 9;
            this.lblDegree.Text = "lblDegree";
            //
            // errPvd
            //
            this.errPvd.ContainerControl = this;
            //
            // FormConfig
            //
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(432, 135);
            this.Controls.Add(this.lblDegree);
            this.Controls.Add(this.cboDirectionAngle);
            this.Controls.Add(this.lblDirectionAngle);
            this.Controls.Add(this.txtHeightOffset);
            this.Controls.Add(this.cboSlabType);
            this.Controls.Add(this.lblHeightOffsetUnit);
            this.Controls.Add(this.lblHeightOffset);
            this.Controls.Add(this.lblSlabType);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FormConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormConfig";
            ((System.ComponentModel.ISupportInitialize)(this.errPvd)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblSlabType;
        private System.Windows.Forms.Label lblHeightOffset;
        private System.Windows.Forms.Label lblHeightOffsetUnit;
        private System.Windows.Forms.ComboBox cboSlabType;
        private System.Windows.Forms.TextBox txtHeightOffset;
        private System.Windows.Forms.Label lblDirectionAngle;
        private System.Windows.Forms.ComboBox cboDirectionAngle;
        private System.Windows.Forms.Label lblDegree;
        private System.Windows.Forms.ErrorProvider errPvd;
    }
}

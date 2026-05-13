namespace ADSK.JExtRAC.AutomaticFloor.Config
{
  partial class FormConfig
  {
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
        components.Dispose();
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
            this.lblLock = new System.Windows.Forms.Label();
            this.chbLock = new System.Windows.Forms.CheckBox();
            this.cboDirectionAngle = new System.Windows.Forms.ComboBox();
            this.lblDirectionAngle = new System.Windows.Forms.Label();
            this.lblDegree = new System.Windows.Forms.Label();
            this.errPvd = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errPvd)).BeginInit();
            this.SuspendLayout();
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(336, 130);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 25);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnOK.Location = new System.Drawing.Point(255, 130);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 25);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            this.lblSlabType.AutoSize = true;
            this.lblSlabType.Location = new System.Drawing.Point(12, 16);
            this.lblSlabType.Name = "lblSlabType";
            this.lblSlabType.Size = new System.Drawing.Size(62, 13);
            this.lblSlabType.TabIndex = 0;
            this.lblSlabType.Text = "Slab Type";
            this.lblHeightOffset.AutoSize = true;
            this.lblHeightOffset.Location = new System.Drawing.Point(12, 44);
            this.lblHeightOffset.Name = "lblHeightOffset";
            this.lblHeightOffset.Size = new System.Drawing.Size(76, 13);
            this.lblHeightOffset.TabIndex = 2;
            this.lblHeightOffset.Text = "Height Offset";
            this.lblHeightOffsetUnit.AutoSize = true;
            this.lblHeightOffsetUnit.Location = new System.Drawing.Point(213, 44);
            this.lblHeightOffsetUnit.Name = "lblHeightOffsetUnit";
            this.lblHeightOffsetUnit.Size = new System.Drawing.Size(23, 13);
            this.lblHeightOffsetUnit.TabIndex = 4;
            this.lblHeightOffsetUnit.Text = "mm";
            this.cboSlabType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSlabType.FormattingEnabled = true;
            this.cboSlabType.Location = new System.Drawing.Point(111, 13);
            this.cboSlabType.Name = "cboSlabType";
            this.cboSlabType.Size = new System.Drawing.Size(300, 21);
            this.cboSlabType.TabIndex = 1;
            this.txtHeightOffset.Location = new System.Drawing.Point(111, 41);
            this.txtHeightOffset.Name = "txtHeightOffset";
            this.txtHeightOffset.Size = new System.Drawing.Size(100, 20);
            this.txtHeightOffset.TabIndex = 3;
            this.txtHeightOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtHeightOffset.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtHeightOffset_KeyPress);
            this.lblLock.AutoSize = true;
            this.lblLock.Location = new System.Drawing.Point(12, 74);
            this.lblLock.Name = "lblLock";
            this.lblLock.Size = new System.Drawing.Size(32, 13);
            this.lblLock.TabIndex = 17;
            this.lblLock.Text = "Lock";
            this.chbLock.AutoSize = true;
            this.chbLock.Location = new System.Drawing.Point(111, 74);
            this.chbLock.Name = "chbLock";
            this.chbLock.Size = new System.Drawing.Size(15, 14);
            this.chbLock.TabIndex = 16;
            this.chbLock.UseVisualStyleBackColor = true;
            this.cboDirectionAngle.FormattingEnabled = true;
            this.cboDirectionAngle.Location = new System.Drawing.Point(111, 94);
            this.cboDirectionAngle.Name = "cboDirectionAngle";
            this.cboDirectionAngle.Size = new System.Drawing.Size(100, 21);
            this.cboDirectionAngle.TabIndex = 19;
            this.cboDirectionAngle.Validated += new System.EventHandler(this.cboDirectionAngle_Validated);
            this.lblDirectionAngle.AutoSize = true;
            this.lblDirectionAngle.Location = new System.Drawing.Point(12, 97);
            this.lblDirectionAngle.Name = "lblDirectionAngle";
            this.lblDirectionAngle.Size = new System.Drawing.Size(78, 13);
            this.lblDirectionAngle.TabIndex = 18;
            this.lblDirectionAngle.Text = "Span Direction";
            this.lblDegree.AutoSize = true;
            this.lblDegree.Location = new System.Drawing.Point(213, 94);
            this.lblDegree.Name = "lblDegree";
            this.lblDegree.Size = new System.Drawing.Size(11, 13);
            this.lblDegree.TabIndex = 20;
            this.lblDegree.Text = "°";
            this.errPvd.ContainerControl = this;
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(432, 167);
            this.Controls.Add(this.lblDegree);
            this.Controls.Add(this.cboDirectionAngle);
            this.Controls.Add(this.lblDirectionAngle);
            this.Controls.Add(this.lblLock);
            this.Controls.Add(this.chbLock);
            this.Controls.Add(this.txtHeightOffset);
            this.Controls.Add(this.cboSlabType);
            this.Controls.Add(this.lblHeightOffsetUnit);
            this.Controls.Add(this.lblHeightOffset);
            this.Controls.Add(this.lblSlabType);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Auto Floor Creation";
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
    private System.Windows.Forms.Label lblLock;
    private System.Windows.Forms.CheckBox chbLock;
    private System.Windows.Forms.ComboBox cboDirectionAngle;
    private System.Windows.Forms.Label lblDirectionAngle;
    private System.Windows.Forms.Label lblDegree;
    private System.Windows.Forms.ErrorProvider errPvd;
  }
}

namespace ADSK.JExtRAC.CheckingALVS.Components
{
  partial class FormGroup
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
      this.lblGroup = new System.Windows.Forms.Label();
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnOK = new System.Windows.Forms.Button();
      this.cboGroup = new System.Windows.Forms.ComboBox();
      this.SuspendLayout();
      // 
      // lblGroup
      // 
      this.lblGroup.AutoSize = true;
      this.lblGroup.Location = new System.Drawing.Point(12, 15);
      this.lblGroup.Name = "lblGroup";
      this.lblGroup.Size = new System.Drawing.Size(47, 12);
      this.lblGroup.TabIndex = 6;
      this.lblGroup.Text = "lblGroup";
      this.lblGroup.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // btnCancel
      // 
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Location = new System.Drawing.Point(174, 38);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(80, 23);
      this.btnCancel.TabIndex = 5;
      this.btnCancel.Text = "btnCancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      // 
      // btnOK
      // 
      this.btnOK.Location = new System.Drawing.Point(54, 38);
      this.btnOK.Name = "btnOK";
      this.btnOK.Size = new System.Drawing.Size(80, 23);
      this.btnOK.TabIndex = 4;
      this.btnOK.Text = "btnOK";
      this.btnOK.UseVisualStyleBackColor = true;
      this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
      // 
      // cboGroup
      // 
      this.cboGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cboGroup.FormattingEnabled = true;
      this.cboGroup.Location = new System.Drawing.Point(54, 12);
      this.cboGroup.Name = "cboGroup";
      this.cboGroup.Size = new System.Drawing.Size(200, 20);
      this.cboGroup.TabIndex = 7;
      // 
      // FormGroup
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(279, 71);
      this.Controls.Add(this.cboGroup);
      this.Controls.Add(this.lblGroup);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.btnOK);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "FormGroup";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "FormGroup";
      this.Load += new System.EventHandler(this.FormGroup_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lblGroup;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.ComboBox cboGroup;

  }
}
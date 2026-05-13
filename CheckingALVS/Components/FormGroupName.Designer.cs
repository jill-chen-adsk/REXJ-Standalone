namespace ADSK.JExtRAC.CheckingALVS.Components
{
  partial class FormGroupName
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
      this.btnOK = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.txtGroupName = new System.Windows.Forms.TextBox();
      this.lblGroupName = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // btnOK
      // 
      this.btnOK.Location = new System.Drawing.Point(14, 31);
      this.btnOK.Name = "btnOK";
      this.btnOK.Size = new System.Drawing.Size(80, 23);
      this.btnOK.TabIndex = 1;
      this.btnOK.Text = "btnOK";
      this.btnOK.UseVisualStyleBackColor = true;
      this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Location = new System.Drawing.Point(113, 31);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(80, 23);
      this.btnCancel.TabIndex = 2;
      this.btnCancel.Text = "btnCancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      // 
      // txtGroupName
      // 
      this.txtGroupName.Location = new System.Drawing.Point(74, 6);
      this.txtGroupName.Name = "txtGroupName";
      this.txtGroupName.Size = new System.Drawing.Size(119, 19);
      this.txtGroupName.TabIndex = 0;
      // 
      // lblGroupName
      // 
      this.lblGroupName.AutoSize = true;
      this.lblGroupName.Location = new System.Drawing.Point(12, 9);
      this.lblGroupName.Name = "lblGroupName";
      this.lblGroupName.Size = new System.Drawing.Size(76, 12);
      this.lblGroupName.TabIndex = 3;
      this.lblGroupName.Text = "lblGroupName";
      this.lblGroupName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // FormGroupName
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(206, 64);
      this.Controls.Add(this.txtGroupName);
      this.Controls.Add(this.lblGroupName);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.btnOK);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "FormGroupName";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "FormGroupName";
      this.Load += new System.EventHandler(this.FormGroupName_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.TextBox txtGroupName;
    private System.Windows.Forms.Label lblGroupName;
  }
}
namespace ADSK.JExtRAC.FittingSchedule.CreateAndEdit
{
  partial class FormScaleCustom
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
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnOK = new System.Windows.Forms.Button();
      this.lblScale = new System.Windows.Forms.Label();
      this.lblScaleOrder = new System.Windows.Forms.Label();
      this.txtScale = new System.Windows.Forms.TextBox();
      this.errPvd = new System.Windows.Forms.ErrorProvider(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.errPvd)).BeginInit();
      this.SuspendLayout();
      // 
      // btnCancel
      // 
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Location = new System.Drawing.Point(98, 46);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(80, 23);
      this.btnCancel.TabIndex = 7;
      this.btnCancel.Text = "btnCancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      // 
      // btnOK
      // 
      this.btnOK.Location = new System.Drawing.Point(12, 46);
      this.btnOK.Name = "btnOK";
      this.btnOK.Size = new System.Drawing.Size(80, 23);
      this.btnOK.TabIndex = 6;
      this.btnOK.Text = "btnOK";
      this.btnOK.UseVisualStyleBackColor = true;
      this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
      // 
      // lblScale
      // 
      this.lblScale.AutoSize = true;
      this.lblScale.Location = new System.Drawing.Point(12, 15);
      this.lblScale.Name = "lblScale";
      this.lblScale.Size = new System.Drawing.Size(52, 15);
      this.lblScale.TabIndex = 8;
      this.lblScale.Text = "lblScale";
      this.lblScale.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // lblScaleOrder
      // 
      this.lblScaleOrder.AutoSize = true;
      this.lblScaleOrder.Location = new System.Drawing.Point(52, 15);
      this.lblScaleOrder.Name = "lblScaleOrder";
      this.lblScaleOrder.Size = new System.Drawing.Size(84, 15);
      this.lblScaleOrder.TabIndex = 9;
      this.lblScaleOrder.Text = "lblScaleOrder";
      this.lblScaleOrder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // txtScale
      // 
      this.txtScale.Location = new System.Drawing.Point(65, 12);
      this.txtScale.MaxLength = 4;
      this.txtScale.Name = "txtScale";
      this.txtScale.Size = new System.Drawing.Size(67, 23);
      this.txtScale.TabIndex = 11;
      this.txtScale.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.txtScale.Validated += new System.EventHandler(this.txtScale_Validated);
      // 
      // errPvd
      // 
      this.errPvd.ContainerControl = this;
      // 
      // FormScaleCustom
      // 
      this.AcceptButton = this.btnOK;
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(191, 82);
      this.Controls.Add(this.txtScale);
      this.Controls.Add(this.lblScaleOrder);
      this.Controls.Add(this.lblScale);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.btnOK);
      this.Font = new System.Drawing.Font("Segoe UI", 9F);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "FormScaleCustom";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "FormScaleCustom";
      this.Load += new System.EventHandler(this.FormScaleCustom_Load);
      ((System.ComponentModel.ISupportInitialize)(this.errPvd)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.Label lblScale;
    private System.Windows.Forms.Label lblScaleOrder;
    private System.Windows.Forms.TextBox txtScale;
    private System.Windows.Forms.ErrorProvider errPvd;
  }
}

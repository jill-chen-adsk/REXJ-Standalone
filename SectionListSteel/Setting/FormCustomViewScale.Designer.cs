namespace SectionListSteel.Setting
{
  partial class FormCustomViewScale
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
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnOK = new System.Windows.Forms.Button();
      this.txtBoxCustomViewPlanScale = new System.Windows.Forms.TextBox();
      this.lblCustomViewPlanScale = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // btnCancel
      // 
      this.btnCancel.Location = new System.Drawing.Point(106, 46);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(75, 23);
      this.btnCancel.TabIndex = 7;
      this.btnCancel.Text = "btnCancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // btnOK
      // 
      this.btnOK.Location = new System.Drawing.Point(22, 46);
      this.btnOK.Name = "btnOK";
      this.btnOK.Size = new System.Drawing.Size(75, 23);
      this.btnOK.TabIndex = 6;
      this.btnOK.Text = "btnOK";
      this.btnOK.UseVisualStyleBackColor = true;
      this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
      // 
      // txtBoxCustomViewPlanScale
      // 
      this.txtBoxCustomViewPlanScale.Location = new System.Drawing.Point(79, 11);
      this.txtBoxCustomViewPlanScale.Name = "txtBoxCustomViewPlanScale";
      this.txtBoxCustomViewPlanScale.Size = new System.Drawing.Size(100, 19);
      this.txtBoxCustomViewPlanScale.TabIndex = 5;
      this.txtBoxCustomViewPlanScale.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBoxCustomViewPlanScale_KeyDown);
      this.txtBoxCustomViewPlanScale.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBoxCustomViewPlanScale_KeyPress);
      // 
      // lblCustomViewPlanScale
      // 
      this.lblCustomViewPlanScale.AutoSize = true;
      this.lblCustomViewPlanScale.Location = new System.Drawing.Point(15, 14);
      this.lblCustomViewPlanScale.Name = "lblCustomViewPlanScale";
      this.lblCustomViewPlanScale.Size = new System.Drawing.Size(131, 12);
      this.lblCustomViewPlanScale.TabIndex = 4;
      this.lblCustomViewPlanScale.Text = "lblCustomViewPlanScale";
      // 
      // FormCustomViewScale
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
      this.AutoScroll = true;
      this.ClientSize = new System.Drawing.Size(197, 81);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.btnOK);
      this.Controls.Add(this.txtBoxCustomViewPlanScale);
      this.Controls.Add(this.lblCustomViewPlanScale);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "FormCustomViewScale";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "FormCustomViewScale";
      this.Load += new System.EventHandler(this.FormCustomViewScale_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.TextBox txtBoxCustomViewPlanScale;
    private System.Windows.Forms.Label lblCustomViewPlanScale;
  }
}
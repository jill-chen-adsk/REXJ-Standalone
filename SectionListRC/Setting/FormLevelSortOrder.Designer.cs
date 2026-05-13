namespace SectionListRC.Setting
{
  partial class FormLevelSortOrder
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
      this.listBoxAllLevel = new System.Windows.Forms.ListBox();
      this.lblAllLevel = new System.Windows.Forms.Label();
      this.btnOK = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnUp = new System.Windows.Forms.Button();
      this.btnDown = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // listBoxAllLevel
      // 
      this.listBoxAllLevel.FormattingEnabled = true;
      this.listBoxAllLevel.ItemHeight = 12;
      this.listBoxAllLevel.Location = new System.Drawing.Point(12, 50);
      this.listBoxAllLevel.Name = "listBoxAllLevel";
      this.listBoxAllLevel.Size = new System.Drawing.Size(150, 196);
      this.listBoxAllLevel.TabIndex = 0;
      // 
      // lblAllLevel
      // 
      this.lblAllLevel.AutoSize = true;
      this.lblAllLevel.Location = new System.Drawing.Point(21, 22);
      this.lblAllLevel.Name = "lblAllLevel";
      this.lblAllLevel.Size = new System.Drawing.Size(58, 12);
      this.lblAllLevel.TabIndex = 1;
      this.lblAllLevel.Text = "lblAllLevel";
      // 
      // btnOK
      // 
      this.btnOK.Location = new System.Drawing.Point(181, 170);
      this.btnOK.Name = "btnOK";
      this.btnOK.Size = new System.Drawing.Size(75, 23);
      this.btnOK.TabIndex = 2;
      this.btnOK.Text = "btnOK";
      this.btnOK.UseVisualStyleBackColor = true;
      this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.Location = new System.Drawing.Point(181, 220);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(75, 23);
      this.btnCancel.TabIndex = 3;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // btnUp
      // 
      this.btnUp.Location = new System.Drawing.Point(181, 50);
      this.btnUp.Name = "btnUp";
      this.btnUp.Size = new System.Drawing.Size(75, 23);
      this.btnUp.TabIndex = 4;
      this.btnUp.Text = "btnUp";
      this.btnUp.UseVisualStyleBackColor = true;
      this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
      // 
      // btnDown
      // 
      this.btnDown.Location = new System.Drawing.Point(181, 100);
      this.btnDown.Name = "btnDown";
      this.btnDown.Size = new System.Drawing.Size(75, 23);
      this.btnDown.TabIndex = 5;
      this.btnDown.Text = "btnDown";
      this.btnDown.UseVisualStyleBackColor = true;
      this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
      // 
      // FormLevelSortOrder
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
      this.AutoScroll = true;
      this.ClientSize = new System.Drawing.Size(274, 261);
      this.Controls.Add(this.btnDown);
      this.Controls.Add(this.btnUp);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.btnOK);
      this.Controls.Add(this.lblAllLevel);
      this.Controls.Add(this.listBoxAllLevel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "FormLevelSortOrder";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "FormLevelSortOrder";
      this.Load += new System.EventHandler(this.FormLevelSortOrder_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ListBox listBoxAllLevel;
    private System.Windows.Forms.Label lblAllLevel;
    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnUp;
    private System.Windows.Forms.Button btnDown;
  }
}
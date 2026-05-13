namespace SectionListSteel.Setting
{
  partial class FormHelpView
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
            this.pnlHelpView = new System.Windows.Forms.Panel();
            this.pictBoxHelpView = new System.Windows.Forms.PictureBox();
            this.pnlHelpView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictBoxHelpView)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlHelpView
            // 
            this.pnlHelpView.AutoSize = true;
            this.pnlHelpView.Controls.Add(this.pictBoxHelpView);
            this.pnlHelpView.Location = new System.Drawing.Point(0, 0);
            this.pnlHelpView.Name = "pnlHelpView";
            this.pnlHelpView.Size = new System.Drawing.Size(100, 108);
            this.pnlHelpView.TabIndex = 0;
            // 
            // pictBoxHelpView
            // 
            this.pictBoxHelpView.Location = new System.Drawing.Point(3, 3);
            this.pictBoxHelpView.Name = "pictBoxHelpView";
            this.pictBoxHelpView.Size = new System.Drawing.Size(80, 80);
            this.pictBoxHelpView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictBoxHelpView.TabIndex = 0;
            this.pictBoxHelpView.TabStop = false;
            // 
            // FormHelpView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(184, 174);
            this.Controls.Add(this.pnlHelpView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormHelpView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FormHelpView";
            this.Load += new System.EventHandler(this.FormHelpView_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormHelpView_KeyDown);
            this.pnlHelpView.ResumeLayout(false);
            this.pnlHelpView.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictBoxHelpView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel pnlHelpView;
    private System.Windows.Forms.PictureBox pictBoxHelpView;
  }
}
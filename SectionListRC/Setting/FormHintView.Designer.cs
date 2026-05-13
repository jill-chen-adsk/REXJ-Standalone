namespace SectionListRC.Setting
{
  partial class FormHintView
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
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.btnParamMapGirderChange = new System.Windows.Forms.Button();
      this.panel1 = new System.Windows.Forms.Panel();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // pictureBox1
      // 
      this.pictureBox1.Location = new System.Drawing.Point(3, 3);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(117, 95);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      // 
      // btnParamMapGirderChange
      // 
      this.btnParamMapGirderChange.Location = new System.Drawing.Point(3, 3);
      this.btnParamMapGirderChange.Name = "btnParamMapGirderChange";
      this.btnParamMapGirderChange.Size = new System.Drawing.Size(75, 23);
      this.btnParamMapGirderChange.TabIndex = 1;
      this.btnParamMapGirderChange.Text = "btnParamMapGirderChange";
      this.btnParamMapGirderChange.UseVisualStyleBackColor = true;
      this.btnParamMapGirderChange.Click += new System.EventHandler(this.btnParamMapGirderChange_Click);
      // 
      // panel1
      // 
      this.panel1.AutoSize = true;
      this.panel1.Controls.Add(this.pictureBox1);
      this.panel1.Location = new System.Drawing.Point(0, 0);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(140, 101);
      this.panel1.TabIndex = 2;
      // 
      // FormHintView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
      this.AutoScroll = true;
      this.AutoSize = true;
      this.ClientSize = new System.Drawing.Size(149, 109);
      this.Controls.Add(this.panel1);
      this.Controls.Add(this.btnParamMapGirderChange);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "FormHintView";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "FormHintView";
      this.Load += new System.EventHandler(this.FormHintView_Load);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Button btnParamMapGirderChange;
    private System.Windows.Forms.Panel panel1;
  }
}
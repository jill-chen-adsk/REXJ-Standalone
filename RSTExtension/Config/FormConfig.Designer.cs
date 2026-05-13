namespace RSTExtension.Config
{
  partial class FormConfig
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
            this.lblLevelTitle = new System.Windows.Forms.Label();
            this.cboLevelWork = new System.Windows.Forms.ComboBox();
            this.lblLevelBase = new System.Windows.Forms.Label();
            this.lblLevelWork = new System.Windows.Forms.Label();
            this.lblLevelExplan = new System.Windows.Forms.Label();
            this.btnLevelRegist = new System.Windows.Forms.Button();
            this.cboLevelBase = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.btnOK.Location = new System.Drawing.Point(224, 141);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 25);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(305, 141);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 25);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // lblLevelTitle
            // 
            this.lblLevelTitle.AutoSize = true;
            this.lblLevelTitle.Location = new System.Drawing.Point(10, 10);
            this.lblLevelTitle.Name = "lblLevelTitle";
            this.lblLevelTitle.Size = new System.Drawing.Size(63, 13);
            this.lblLevelTitle.TabIndex = 0;
            this.lblLevelTitle.Text = "lblLevelTitle";
            // 
            // cboLevelWork
            // 
            this.cboLevelWork.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.cboLevelWork.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLevelWork.FormattingEnabled = true;
            this.cboLevelWork.Location = new System.Drawing.Point(12, 38);
            this.cboLevelWork.Name = "cboLevelWork";
            this.cboLevelWork.Size = new System.Drawing.Size(184, 21);
            this.cboLevelWork.TabIndex = 1;
            // 
            // lblLevelBase
            // 
            this.lblLevelBase.AutoSize = true;
            this.lblLevelBase.Location = new System.Drawing.Point(202, 69);
            this.lblLevelBase.Name = "lblLevelBase";
            this.lblLevelBase.Size = new System.Drawing.Size(67, 13);
            this.lblLevelBase.TabIndex = 2;
            this.lblLevelBase.Text = "lblLevelBase";
            // 
            // lblLevelWork
            // 
            this.lblLevelWork.AutoSize = true;
            this.lblLevelWork.Location = new System.Drawing.Point(202, 41);
            this.lblLevelWork.Name = "lblLevelWork";
            this.lblLevelWork.Size = new System.Drawing.Size(69, 13);
            this.lblLevelWork.TabIndex = 4;
            this.lblLevelWork.Text = "lblLevelWork";
            // 
            // lblLevelExplan
            // 
            this.lblLevelExplan.AutoSize = true;
            this.lblLevelExplan.Location = new System.Drawing.Point(10, 102);
            this.lblLevelExplan.Name = "lblLevelExplan";
            this.lblLevelExplan.Size = new System.Drawing.Size(75, 13);
            this.lblLevelExplan.TabIndex = 5;
            this.lblLevelExplan.Text = "lblLevelExplan";
            // 
            // btnLevelRegist
            // 
            this.btnLevelRegist.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.btnLevelRegist.Location = new System.Drawing.Point(12, 141);
            this.btnLevelRegist.Name = "btnLevelRegist";
            this.btnLevelRegist.Size = new System.Drawing.Size(100, 25);
            this.btnLevelRegist.TabIndex = 6;
            this.btnLevelRegist.Text = "btnLevelRegist";
            this.btnLevelRegist.UseVisualStyleBackColor = true;
            this.btnLevelRegist.Click += new System.EventHandler(this.btnLevelRegist_Click);
            // 
            // cboLevelBase
            // 
            this.cboLevelBase.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.cboLevelBase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLevelBase.FormattingEnabled = true;
            this.cboLevelBase.Location = new System.Drawing.Point(12, 64);
            this.cboLevelBase.Name = "cboLevelBase";
            this.cboLevelBase.Size = new System.Drawing.Size(184, 21);
            this.cboLevelBase.TabIndex = 1;
            // 
            // FormConfig
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(394, 181);
            this.Controls.Add(this.btnLevelRegist);
            this.Controls.Add(this.lblLevelExplan);
            this.Controls.Add(this.lblLevelWork);
            this.Controls.Add(this.lblLevelBase);
            this.Controls.Add(this.cboLevelBase);
            this.Controls.Add(this.cboLevelWork);
            this.Controls.Add(this.lblLevelTitle);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MinimumSize = new System.Drawing.Size(410, 220);
            this.Name = "FormConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormConfig";
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Label lblLevelTitle;
    private System.Windows.Forms.ComboBox cboLevelWork;
    private System.Windows.Forms.Label lblLevelBase;
    private System.Windows.Forms.Label lblLevelWork;
    private System.Windows.Forms.Label lblLevelExplan;
    private System.Windows.Forms.Button btnLevelRegist;
        private System.Windows.Forms.ComboBox cboLevelBase;
    }
}

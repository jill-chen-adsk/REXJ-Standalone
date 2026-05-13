using System.Drawing;

namespace ADSK.JExtRAC.AreaSchedule.GroundsExpression
{
  partial class FormCalcArea
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
            this.gpbLengthDecimal = new System.Windows.Forms.GroupBox();
            this.gpbLengthUnit = new System.Windows.Forms.GroupBox();
            this.rdbLengthM = new System.Windows.Forms.RadioButton();
            this.rdbLengthMM = new System.Windows.Forms.RadioButton();
            this.btnLengthDefault = new System.Windows.Forms.Button();
            this.lblLengthUnit = new System.Windows.Forms.Label();
            this.gpbLengthRounding = new System.Windows.Forms.GroupBox();
            this.rdbLengthRounding = new System.Windows.Forms.RadioButton();
            this.rdbLengthClose = new System.Windows.Forms.RadioButton();
            this.rdbLengthCut = new System.Windows.Forms.RadioButton();
            this.lblLengthOrder = new System.Windows.Forms.Label();
            this.txtLengthDecimal = new System.Windows.Forms.TextBox();
            this.lblLengthDecimal = new System.Windows.Forms.Label();
            this.gpbAreaDecimal = new System.Windows.Forms.GroupBox();
            this.btnAreaDefault = new System.Windows.Forms.Button();
            this.gpbAreaRounding = new System.Windows.Forms.GroupBox();
            this.rdbAreaRounding = new System.Windows.Forms.RadioButton();
            this.rdbAreaClose = new System.Windows.Forms.RadioButton();
            this.rdbAreaCut = new System.Windows.Forms.RadioButton();
            this.lblAreaOrder = new System.Windows.Forms.Label();
            this.txtAreaDecimal = new System.Windows.Forms.TextBox();
            this.lblAreaDecimal = new System.Windows.Forms.Label();
            this.gpbPi = new System.Windows.Forms.GroupBox();
            this.cboPi = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.errPvd = new System.Windows.Forms.ErrorProvider(this.components);
            this.gpbLengthDecimal.SuspendLayout();
            this.gpbLengthUnit.SuspendLayout();
            this.gpbLengthRounding.SuspendLayout();
            this.gpbAreaDecimal.SuspendLayout();
            this.gpbAreaRounding.SuspendLayout();
            this.gpbPi.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errPvd)).BeginInit();
            this.SuspendLayout();
            // 
            // gpbLengthDecimal
            // 
            this.gpbLengthDecimal.Controls.Add(this.gpbLengthUnit);
            this.gpbLengthDecimal.Controls.Add(this.btnLengthDefault);
            this.gpbLengthDecimal.Controls.Add(this.lblLengthUnit);
            this.gpbLengthDecimal.Controls.Add(this.gpbLengthRounding);
            this.gpbLengthDecimal.Controls.Add(this.lblLengthOrder);
            this.gpbLengthDecimal.Controls.Add(this.txtLengthDecimal);
            this.gpbLengthDecimal.Controls.Add(this.lblLengthDecimal);
            this.gpbLengthDecimal.Location = new System.Drawing.Point(12, 12);
            this.gpbLengthDecimal.Name = "gpbLengthDecimal";
            this.gpbLengthDecimal.Size = new System.Drawing.Size(345, 130);
            this.gpbLengthDecimal.TabIndex = 0;
            this.gpbLengthDecimal.TabStop = false;
            this.gpbLengthDecimal.Text = "gpbLengthDecimal";
            // 
            // gpbLengthUnit
            // 
            this.gpbLengthUnit.Controls.Add(this.rdbLengthM);
            this.gpbLengthUnit.Controls.Add(this.rdbLengthMM);
            this.gpbLengthUnit.Location = new System.Drawing.Point(34, 14);
            this.gpbLengthUnit.Name = "gpbLengthUnit";
            this.gpbLengthUnit.Size = new System.Drawing.Size(80, 29);
            this.gpbLengthUnit.TabIndex = 0;
            this.gpbLengthUnit.TabStop = false;
            // 
            // rdbLengthM
            // 
            this.rdbLengthM.AutoSize = true;
            this.rdbLengthM.Location = new System.Drawing.Point(46, 9);
            this.rdbLengthM.Name = "rdbLengthM";
            this.rdbLengthM.Size = new System.Drawing.Size(82, 16);
            this.rdbLengthM.TabIndex = 1;
            this.rdbLengthM.TabStop = true;
            this.rdbLengthM.Text = "rdbLengthM";
            this.rdbLengthM.UseVisualStyleBackColor = true;
            this.rdbLengthM.Click += new System.EventHandler(this.rdbDisplayM_Click);
            // 
            // rdbLengthMM
            // 
            this.rdbLengthMM.AutoSize = true;
            this.rdbLengthMM.Location = new System.Drawing.Point(6, 9);
            this.rdbLengthMM.Name = "rdbLengthMM";
            this.rdbLengthMM.Size = new System.Drawing.Size(91, 16);
            this.rdbLengthMM.TabIndex = 0;
            this.rdbLengthMM.TabStop = true;
            this.rdbLengthMM.Text = "rdbLengthMM";
            this.rdbLengthMM.UseVisualStyleBackColor = true;
            this.rdbLengthMM.Click += new System.EventHandler(this.rdbDisplayMM_Click);
            // 
            // btnLengthDefault
            // 
            this.btnLengthDefault.Location = new System.Drawing.Point(6, 100);
            this.btnLengthDefault.Name = "btnLengthDefault";
            this.btnLengthDefault.Size = new System.Drawing.Size(75, 23);
            this.btnLengthDefault.TabIndex = 2;
            this.btnLengthDefault.Text = "btnLengthDefault";
            this.btnLengthDefault.UseVisualStyleBackColor = true;
            this.btnLengthDefault.Click += new System.EventHandler(this.btnLengthDefault_Click);
            // 
            // lblLengthUnit
            // 
            this.lblLengthUnit.AutoSize = true;
            this.lblLengthUnit.Location = new System.Drawing.Point(5, 25);
            this.lblLengthUnit.Name = "lblLengthUnit";
            this.lblLengthUnit.Size = new System.Drawing.Size(72, 12);
            this.lblLengthUnit.TabIndex = 4;
            this.lblLengthUnit.Text = "lblLengthUnit";
            this.lblLengthUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gpbLengthRounding
            // 
            this.gpbLengthRounding.Controls.Add(this.rdbLengthRounding);
            this.gpbLengthRounding.Controls.Add(this.rdbLengthClose);
            this.gpbLengthRounding.Controls.Add(this.rdbLengthCut);
            this.gpbLengthRounding.Location = new System.Drawing.Point(180, 32);
            this.gpbLengthRounding.Name = "gpbLengthRounding";
            this.gpbLengthRounding.Size = new System.Drawing.Size(159, 91);
            this.gpbLengthRounding.TabIndex = 2;
            this.gpbLengthRounding.TabStop = false;
            this.gpbLengthRounding.Text = "gpbLengthRounding";
            // 
            // rdbLengthRounding
            // 
            this.rdbLengthRounding.AutoSize = true;
            this.rdbLengthRounding.Location = new System.Drawing.Point(6, 62);
            this.rdbLengthRounding.Name = "rdbLengthRounding";
            this.rdbLengthRounding.Size = new System.Drawing.Size(120, 16);
            this.rdbLengthRounding.TabIndex = 2;
            this.rdbLengthRounding.TabStop = true;
            this.rdbLengthRounding.Text = "rdbLengthRounding";
            this.rdbLengthRounding.UseVisualStyleBackColor = true;
            // 
            // rdbLengthClose
            // 
            this.rdbLengthClose.AutoSize = true;
            this.rdbLengthClose.Location = new System.Drawing.Point(6, 40);
            this.rdbLengthClose.Name = "rdbLengthClose";
            this.rdbLengthClose.Size = new System.Drawing.Size(102, 16);
            this.rdbLengthClose.TabIndex = 1;
            this.rdbLengthClose.TabStop = true;
            this.rdbLengthClose.Text = "rdbLengthClose";
            this.rdbLengthClose.UseVisualStyleBackColor = true;
            // 
            // rdbLengthCut
            // 
            this.rdbLengthCut.AutoSize = true;
            this.rdbLengthCut.Location = new System.Drawing.Point(6, 18);
            this.rdbLengthCut.Name = "rdbLengthCut";
            this.rdbLengthCut.Size = new System.Drawing.Size(91, 16);
            this.rdbLengthCut.TabIndex = 0;
            this.rdbLengthCut.TabStop = true;
            this.rdbLengthCut.Text = "rdbLengthCut";
            this.rdbLengthCut.UseVisualStyleBackColor = true;
            // 
            // lblLengthOrder
            // 
            this.lblLengthOrder.AutoSize = true;
            this.lblLengthOrder.Location = new System.Drawing.Point(152, 52);
            this.lblLengthOrder.Name = "lblLengthOrder";
            this.lblLengthOrder.Size = new System.Drawing.Size(79, 12);
            this.lblLengthOrder.TabIndex = 2;
            this.lblLengthOrder.Text = "lblLengthOrder";
            this.lblLengthOrder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtLengthDecimal
            // 
            this.txtLengthDecimal.Location = new System.Drawing.Point(84, 49);
            this.txtLengthDecimal.MaxLength = 1;
            this.txtLengthDecimal.Name = "txtLengthDecimal";
            this.txtLengthDecimal.Size = new System.Drawing.Size(67, 19);
            this.txtLengthDecimal.TabIndex = 1;
            this.txtLengthDecimal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtLengthDecimal.Validated += new System.EventHandler(this.txtLengthDecimal_Validated);
            // 
            // lblLengthDecimal
            // 
            this.lblLengthDecimal.AutoSize = true;
            this.lblLengthDecimal.Location = new System.Drawing.Point(6, 52);
            this.lblLengthDecimal.Name = "lblLengthDecimal";
            this.lblLengthDecimal.Size = new System.Drawing.Size(92, 12);
            this.lblLengthDecimal.TabIndex = 0;
            this.lblLengthDecimal.Text = "lblLengthDecimal";
            this.lblLengthDecimal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gpbAreaDecimal
            // 
            this.gpbAreaDecimal.Controls.Add(this.btnAreaDefault);
            this.gpbAreaDecimal.Controls.Add(this.gpbAreaRounding);
            this.gpbAreaDecimal.Controls.Add(this.lblAreaOrder);
            this.gpbAreaDecimal.Controls.Add(this.txtAreaDecimal);
            this.gpbAreaDecimal.Controls.Add(this.lblAreaDecimal);
            this.gpbAreaDecimal.Location = new System.Drawing.Point(11, 148);
            this.gpbAreaDecimal.Name = "gpbAreaDecimal";
            this.gpbAreaDecimal.Size = new System.Drawing.Size(346, 107);
            this.gpbAreaDecimal.TabIndex = 1;
            this.gpbAreaDecimal.TabStop = false;
            this.gpbAreaDecimal.Text = "gpbAreaDecimal";
            // 
            // btnAreaDefault
            // 
            this.btnAreaDefault.Location = new System.Drawing.Point(6, 77);
            this.btnAreaDefault.Name = "btnAreaDefault";
            this.btnAreaDefault.Size = new System.Drawing.Size(75, 23);
            this.btnAreaDefault.TabIndex = 2;
            this.btnAreaDefault.Text = "btnAreaDefault";
            this.btnAreaDefault.UseVisualStyleBackColor = true;
            this.btnAreaDefault.Click += new System.EventHandler(this.btnAreaDefault_Click);
            // 
            // gpbAreaRounding
            // 
            this.gpbAreaRounding.Controls.Add(this.rdbAreaRounding);
            this.gpbAreaRounding.Controls.Add(this.rdbAreaClose);
            this.gpbAreaRounding.Controls.Add(this.rdbAreaCut);
            this.gpbAreaRounding.Location = new System.Drawing.Point(180, 9);
            this.gpbAreaRounding.Name = "gpbAreaRounding";
            this.gpbAreaRounding.Size = new System.Drawing.Size(160, 91);
            this.gpbAreaRounding.TabIndex = 1;
            this.gpbAreaRounding.TabStop = false;
            this.gpbAreaRounding.Text = "gpbAreaRounding";
            // 
            // rdbAreaRounding
            // 
            this.rdbAreaRounding.AutoSize = true;
            this.rdbAreaRounding.Location = new System.Drawing.Point(6, 62);
            this.rdbAreaRounding.Name = "rdbAreaRounding";
            this.rdbAreaRounding.Size = new System.Drawing.Size(110, 16);
            this.rdbAreaRounding.TabIndex = 2;
            this.rdbAreaRounding.TabStop = true;
            this.rdbAreaRounding.Text = "rdbAreaRounding";
            this.rdbAreaRounding.UseVisualStyleBackColor = true;
            // 
            // rdbAreaClose
            // 
            this.rdbAreaClose.AutoSize = true;
            this.rdbAreaClose.Location = new System.Drawing.Point(6, 40);
            this.rdbAreaClose.Name = "rdbAreaClose";
            this.rdbAreaClose.Size = new System.Drawing.Size(92, 16);
            this.rdbAreaClose.TabIndex = 1;
            this.rdbAreaClose.TabStop = true;
            this.rdbAreaClose.Text = "rdbAreaClose";
            this.rdbAreaClose.UseVisualStyleBackColor = true;
            // 
            // rdbAreaCut
            // 
            this.rdbAreaCut.AutoSize = true;
            this.rdbAreaCut.Location = new System.Drawing.Point(6, 18);
            this.rdbAreaCut.Name = "rdbAreaCut";
            this.rdbAreaCut.Size = new System.Drawing.Size(81, 16);
            this.rdbAreaCut.TabIndex = 0;
            this.rdbAreaCut.TabStop = true;
            this.rdbAreaCut.Text = "rdbAreaCut";
            this.rdbAreaCut.UseVisualStyleBackColor = true;
            // 
            // lblAreaOrder
            // 
            this.lblAreaOrder.AutoSize = true;
            this.lblAreaOrder.Location = new System.Drawing.Point(152, 29);
            this.lblAreaOrder.Name = "lblAreaOrder";
            this.lblAreaOrder.Size = new System.Drawing.Size(69, 12);
            this.lblAreaOrder.TabIndex = 2;
            this.lblAreaOrder.Text = "lblAreaOrder";
            this.lblAreaOrder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtAreaDecimal
            // 
            this.txtAreaDecimal.Location = new System.Drawing.Point(84, 26);
            this.txtAreaDecimal.MaxLength = 1;
            this.txtAreaDecimal.Name = "txtAreaDecimal";
            this.txtAreaDecimal.Size = new System.Drawing.Size(67, 19);
            this.txtAreaDecimal.TabIndex = 0;
            this.txtAreaDecimal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtAreaDecimal.Validated += new System.EventHandler(this.txtAreaDecimal_Validated);
            // 
            // lblAreaDecimal
            // 
            this.lblAreaDecimal.AutoSize = true;
            this.lblAreaDecimal.Location = new System.Drawing.Point(6, 29);
            this.lblAreaDecimal.Name = "lblAreaDecimal";
            this.lblAreaDecimal.Size = new System.Drawing.Size(82, 12);
            this.lblAreaDecimal.TabIndex = 0;
            this.lblAreaDecimal.Text = "lblAreaDecimal";
            this.lblAreaDecimal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gpbPi
            // 
            this.gpbPi.Controls.Add(this.cboPi);
            this.gpbPi.Location = new System.Drawing.Point(11, 261);
            this.gpbPi.Name = "gpbPi";
            this.gpbPi.Size = new System.Drawing.Size(346, 37);
            this.gpbPi.TabIndex = 2;
            this.gpbPi.TabStop = false;
            this.gpbPi.Text = "gpbPi";
            // 
            // cboPi
            // 
            this.cboPi.FormattingEnabled = true;
            this.cboPi.Location = new System.Drawing.Point(100, 12);
            this.cboPi.Name = "cboPi";
            this.cboPi.Size = new System.Drawing.Size(83, 20);
            this.cboPi.TabIndex = 0;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(69, 304);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(80, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(224, 304);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // errPvd
            // 
            this.errPvd.ContainerControl = this;
            // 
            // FormCalcArea
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(369, 332);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.gpbPi);
            this.Controls.Add(this.gpbAreaDecimal);
            this.Controls.Add(this.gpbLengthDecimal);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormCalcArea";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormCalcArea";
            this.Load += new System.EventHandler(this.FormCalcArea_Load);
            this.gpbLengthDecimal.ResumeLayout(false);
            this.gpbLengthDecimal.PerformLayout();
            this.gpbLengthUnit.ResumeLayout(false);
            this.gpbLengthUnit.PerformLayout();
            this.gpbLengthRounding.ResumeLayout(false);
            this.gpbLengthRounding.PerformLayout();
            this.gpbAreaDecimal.ResumeLayout(false);
            this.gpbAreaDecimal.PerformLayout();
            this.gpbAreaRounding.ResumeLayout(false);
            this.gpbAreaRounding.PerformLayout();
            this.gpbPi.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errPvd)).EndInit();
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox gpbLengthDecimal;
    private System.Windows.Forms.GroupBox gpbLengthRounding;
    private System.Windows.Forms.RadioButton rdbLengthCut;
    private System.Windows.Forms.Label lblLengthOrder;
    private System.Windows.Forms.TextBox txtLengthDecimal;
    private System.Windows.Forms.Label lblLengthDecimal;
    private System.Windows.Forms.Button btnLengthDefault;
    private System.Windows.Forms.RadioButton rdbLengthRounding;
    private System.Windows.Forms.RadioButton rdbLengthClose;
    private System.Windows.Forms.GroupBox gpbAreaDecimal;
    private System.Windows.Forms.Button btnAreaDefault;
    private System.Windows.Forms.GroupBox gpbAreaRounding;
    private System.Windows.Forms.RadioButton rdbAreaRounding;
    private System.Windows.Forms.RadioButton rdbAreaClose;
    private System.Windows.Forms.RadioButton rdbAreaCut;
    private System.Windows.Forms.Label lblAreaOrder;
    private System.Windows.Forms.TextBox txtAreaDecimal;
    private System.Windows.Forms.Label lblAreaDecimal;
    private System.Windows.Forms.GroupBox gpbPi;
    private System.Windows.Forms.ComboBox cboPi;
    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.ErrorProvider errPvd;
    private System.Windows.Forms.GroupBox gpbLengthUnit;
    private System.Windows.Forms.RadioButton rdbLengthMM;
    private System.Windows.Forms.RadioButton rdbLengthM;
    private System.Windows.Forms.Label lblLengthUnit;
  }
}
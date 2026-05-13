namespace ADSK.JExtRAC.AveSiteLevelHeightCalc.Create
{
  partial class FormCalcDraw
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.txtVertical = new System.Windows.Forms.TextBox();
            this.txtHorizontal = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lbl1Slash = new System.Windows.Forms.Label();
            this.gpbCalcPoint = new System.Windows.Forms.GroupBox();
            this.btnUpdateNumber = new System.Windows.Forms.Button();
            this.btnDel = new System.Windows.Forms.Button();
            this.btnDn = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.dgvCalcPoint = new System.Windows.Forms.DataGridView();
            this.gpbCalcDraw = new System.Windows.Forms.GroupBox();
            this.lblVertical = new System.Windows.Forms.Label();
            this.txtScale = new System.Windows.Forms.TextBox();
            this.lblRate = new System.Windows.Forms.Label();
            this.lblScale = new System.Windows.Forms.Label();
            this.lblHorizontal = new System.Windows.Forms.Label();
            this.errPvd = new System.Windows.Forms.ErrorProvider(this.components);
            this.btnCreate = new System.Windows.Forms.Button();
            this.txtBMHeight = new System.Windows.Forms.TextBox();
            this.lblBMHeight = new System.Windows.Forms.Label();
            this.btnUpdateLevel = new System.Windows.Forms.Button();
            this.txtBMHeightOld = new System.Windows.Forms.TextBox();
            this.gpbAreaDecimal = new System.Windows.Forms.GroupBox();
            this.gpbAreaRounding = new System.Windows.Forms.GroupBox();
            this.rdbAreaRounding = new System.Windows.Forms.RadioButton();
            this.rdbAreaClose = new System.Windows.Forms.RadioButton();
            this.rdbAreaCut = new System.Windows.Forms.RadioButton();
            this.gpbLengthUnit = new System.Windows.Forms.GroupBox();
            this.rdbLengthM = new System.Windows.Forms.RadioButton();
            this.rdbLengthMM = new System.Windows.Forms.RadioButton();
            this.lblAreaOrder = new System.Windows.Forms.Label();
            this.txtAreaDecimal = new System.Windows.Forms.TextBox();
            this.lblAreaDecimal = new System.Windows.Forms.Label();
            this.gpbCalcPoint.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCalcPoint)).BeginInit();
            this.gpbCalcDraw.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errPvd)).BeginInit();
            this.gpbAreaDecimal.SuspendLayout();
            this.gpbAreaRounding.SuspendLayout();
            this.gpbLengthUnit.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtVertical
            // 
            this.txtVertical.Location = new System.Drawing.Point(190, 46);
            this.txtVertical.Name = "txtVertical";
            this.txtVertical.Size = new System.Drawing.Size(50, 19);
            this.txtVertical.TabIndex = 10;
            this.txtVertical.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtVertical.Validated += new System.EventHandler(this.txtVertical_Validated);
            // 
            // txtHorizontal
            // 
            this.txtHorizontal.Location = new System.Drawing.Point(85, 46);
            this.txtHorizontal.Name = "txtHorizontal";
            this.txtHorizontal.Size = new System.Drawing.Size(50, 19);
            this.txtHorizontal.TabIndex = 9;
            this.txtHorizontal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtHorizontal.Validated += new System.EventHandler(this.txtHorizontal_Validated);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(221, 531);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 23);
            this.btnCancel.TabIndex = 22;
            this.btnCancel.Text = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(135, 531);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(80, 23);
            this.btnClose.TabIndex = 21;
            this.btnClose.Text = "btnClose";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lbl1Slash
            // 
            this.lbl1Slash.Location = new System.Drawing.Point(15, 19);
            this.lbl1Slash.Name = "lbl1Slash";
            this.lbl1Slash.Size = new System.Drawing.Size(69, 23);
            this.lbl1Slash.TabIndex = 7;
            this.lbl1Slash.Text = "lbl1Slash";
            this.lbl1Slash.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // gpbCalcPoint
            // 
            this.gpbCalcPoint.Controls.Add(this.btnUpdateNumber);
            this.gpbCalcPoint.Controls.Add(this.btnDel);
            this.gpbCalcPoint.Controls.Add(this.btnDn);
            this.gpbCalcPoint.Controls.Add(this.btnUp);
            this.gpbCalcPoint.Controls.Add(this.dgvCalcPoint);
            this.gpbCalcPoint.Location = new System.Drawing.Point(12, 12);
            this.gpbCalcPoint.Name = "gpbCalcPoint";
            this.gpbCalcPoint.Size = new System.Drawing.Size(289, 277);
            this.gpbCalcPoint.TabIndex = 0;
            this.gpbCalcPoint.TabStop = false;
            this.gpbCalcPoint.Text = "gpbCalcPoint";
            // 
            // btnUpdateNumber
            // 
            this.btnUpdateNumber.Location = new System.Drawing.Point(242, 236);
            this.btnUpdateNumber.Name = "btnUpdateNumber";
            this.btnUpdateNumber.Size = new System.Drawing.Size(40, 35);
            this.btnUpdateNumber.TabIndex = 4;
            this.btnUpdateNumber.Text = "btnUpdateNumber";
            this.btnUpdateNumber.UseVisualStyleBackColor = true;
            this.btnUpdateNumber.Click += new System.EventHandler(this.btnUpdateNumber_Click);
            // 
            // btnDel
            // 
            this.btnDel.Location = new System.Drawing.Point(242, 118);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(40, 35);
            this.btnDel.TabIndex = 3;
            this.btnDel.Text = "btnDel";
            this.btnDel.UseVisualStyleBackColor = true;
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // btnDn
            // 
            this.btnDn.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnDn.Location = new System.Drawing.Point(242, 59);
            this.btnDn.Name = "btnDn";
            this.btnDn.Size = new System.Drawing.Size(40, 35);
            this.btnDn.TabIndex = 2;
            this.btnDn.Text = "btnDn";
            this.btnDn.UseVisualStyleBackColor = true;
            this.btnDn.Click += new System.EventHandler(this.btnDn_Click);
            // 
            // btnUp
            // 
            this.btnUp.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnUp.Location = new System.Drawing.Point(242, 18);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(40, 35);
            this.btnUp.TabIndex = 1;
            this.btnUp.Text = "btnUp";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // dgvCalcPoint
            // 
            this.dgvCalcPoint.AllowUserToAddRows = false;
            this.dgvCalcPoint.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCalcPoint.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvCalcPoint.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvCalcPoint.Location = new System.Drawing.Point(6, 18);
            this.dgvCalcPoint.MultiSelect = false;
            this.dgvCalcPoint.Name = "dgvCalcPoint";
            this.dgvCalcPoint.RowHeadersVisible = false;
            this.dgvCalcPoint.RowTemplate.Height = 21;
            this.dgvCalcPoint.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvCalcPoint.Size = new System.Drawing.Size(230, 253);
            this.dgvCalcPoint.TabIndex = 0;
            this.dgvCalcPoint.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCalcPoint_CellValidated);
            // 
            // gpbCalcDraw
            // 
            this.gpbCalcDraw.Controls.Add(this.txtVertical);
            this.gpbCalcDraw.Controls.Add(this.txtHorizontal);
            this.gpbCalcDraw.Controls.Add(this.lblVertical);
            this.gpbCalcDraw.Controls.Add(this.txtScale);
            this.gpbCalcDraw.Controls.Add(this.lblRate);
            this.gpbCalcDraw.Controls.Add(this.lblScale);
            this.gpbCalcDraw.Controls.Add(this.lbl1Slash);
            this.gpbCalcDraw.Controls.Add(this.lblHorizontal);
            this.gpbCalcDraw.Location = new System.Drawing.Point(12, 320);
            this.gpbCalcDraw.Name = "gpbCalcDraw";
            this.gpbCalcDraw.Size = new System.Drawing.Size(289, 72);
            this.gpbCalcDraw.TabIndex = 7;
            this.gpbCalcDraw.TabStop = false;
            this.gpbCalcDraw.Text = "gpbCalcDraw";
            // 
            // lblVertical
            // 
            this.lblVertical.Location = new System.Drawing.Point(90, 44);
            this.lblVertical.Name = "lblVertical";
            this.lblVertical.Size = new System.Drawing.Size(100, 23);
            this.lblVertical.TabIndex = 4;
            this.lblVertical.Text = "lblVertical";
            this.lblVertical.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtScale
            // 
            this.txtScale.Location = new System.Drawing.Point(85, 21);
            this.txtScale.Name = "txtScale";
            this.txtScale.Size = new System.Drawing.Size(50, 19);
            this.txtScale.TabIndex = 8;
            this.txtScale.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtScale.Validated += new System.EventHandler(this.txtScale_Validated);
            // 
            // lblRate
            // 
            this.lblRate.AutoSize = true;
            this.lblRate.Location = new System.Drawing.Point(6, 49);
            this.lblRate.Name = "lblRate";
            this.lblRate.Size = new System.Drawing.Size(41, 12);
            this.lblRate.TabIndex = 1;
            this.lblRate.Text = "lblRate";
            this.lblRate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblScale
            // 
            this.lblScale.AutoSize = true;
            this.lblScale.Location = new System.Drawing.Point(6, 24);
            this.lblScale.Name = "lblScale";
            this.lblScale.Size = new System.Drawing.Size(45, 12);
            this.lblScale.TabIndex = 0;
            this.lblScale.Text = "lblScale";
            this.lblScale.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblHorizontal
            // 
            this.lblHorizontal.Location = new System.Drawing.Point(15, 44);
            this.lblHorizontal.Name = "lblHorizontal";
            this.lblHorizontal.Size = new System.Drawing.Size(69, 23);
            this.lblHorizontal.TabIndex = 3;
            this.lblHorizontal.Text = "lblHorizontal";
            this.lblHorizontal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // errPvd
            // 
            this.errPvd.ContainerControl = this;
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(54, 531);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(75, 23);
            this.btnCreate.TabIndex = 20;
            this.btnCreate.Text = "btnCreate";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // txtBMHeight
            // 
            this.txtBMHeight.Location = new System.Drawing.Point(70, 295);
            this.txtBMHeight.Name = "txtBMHeight";
            this.txtBMHeight.Size = new System.Drawing.Size(50, 19);
            this.txtBMHeight.TabIndex = 5;
            this.txtBMHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtBMHeight.Validated += new System.EventHandler(this.txtBMHeight_Validated);
            // 
            // lblBMHeight
            // 
            this.lblBMHeight.AutoSize = true;
            this.lblBMHeight.Location = new System.Drawing.Point(16, 298);
            this.lblBMHeight.Name = "lblBMHeight";
            this.lblBMHeight.Size = new System.Drawing.Size(67, 12);
            this.lblBMHeight.TabIndex = 39;
            this.lblBMHeight.Text = "lblBMHeight";
            // 
            // btnUpdateLevel
            // 
            this.btnUpdateLevel.Location = new System.Drawing.Point(126, 293);
            this.btnUpdateLevel.Name = "btnUpdateLevel";
            this.btnUpdateLevel.Size = new System.Drawing.Size(70, 23);
            this.btnUpdateLevel.TabIndex = 6;
            this.btnUpdateLevel.Text = "btnUpdateLevel";
            this.btnUpdateLevel.UseVisualStyleBackColor = true;
            this.btnUpdateLevel.Click += new System.EventHandler(this.btnUpdateLevel_Click);
            // 
            // txtBMHeightOld
            // 
            this.txtBMHeightOld.Location = new System.Drawing.Point(202, 295);
            this.txtBMHeightOld.Name = "txtBMHeightOld";
            this.txtBMHeightOld.Size = new System.Drawing.Size(50, 19);
            this.txtBMHeightOld.TabIndex = 40;
            this.txtBMHeightOld.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtBMHeightOld.Visible = false;
            // 
            // gpbAreaDecimal
            // 
            this.gpbAreaDecimal.Controls.Add(this.gpbAreaRounding);
            this.gpbAreaDecimal.Controls.Add(this.gpbLengthUnit);
            this.gpbAreaDecimal.Controls.Add(this.lblAreaOrder);
            this.gpbAreaDecimal.Controls.Add(this.txtAreaDecimal);
            this.gpbAreaDecimal.Controls.Add(this.lblAreaDecimal);
            this.gpbAreaDecimal.Location = new System.Drawing.Point(12, 398);
            this.gpbAreaDecimal.Name = "gpbAreaDecimal";
            this.gpbAreaDecimal.Size = new System.Drawing.Size(289, 127);
            this.gpbAreaDecimal.TabIndex = 11;
            this.gpbAreaDecimal.TabStop = false;
            this.gpbAreaDecimal.Text = "gpbAreaDecimal";
            // 
            // gpbAreaRounding
            // 
            this.gpbAreaRounding.Controls.Add(this.rdbAreaRounding);
            this.gpbAreaRounding.Controls.Add(this.rdbAreaClose);
            this.gpbAreaRounding.Controls.Add(this.rdbAreaCut);
            this.gpbAreaRounding.Location = new System.Drawing.Point(193, 27);
            this.gpbAreaRounding.Name = "gpbAreaRounding";
            this.gpbAreaRounding.Size = new System.Drawing.Size(89, 91);
            this.gpbAreaRounding.TabIndex = 16;
            this.gpbAreaRounding.TabStop = false;
            this.gpbAreaRounding.Text = "gpbAreaRounding";
            // 
            // rdbAreaRounding
            // 
            this.rdbAreaRounding.AutoSize = true;
            this.rdbAreaRounding.Location = new System.Drawing.Point(6, 62);
            this.rdbAreaRounding.Name = "rdbAreaRounding";
            this.rdbAreaRounding.Size = new System.Drawing.Size(110, 16);
            this.rdbAreaRounding.TabIndex = 19;
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
            this.rdbAreaClose.TabIndex = 18;
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
            this.rdbAreaCut.TabIndex = 17;
            this.rdbAreaCut.TabStop = true;
            this.rdbAreaCut.Text = "rdbAreaCut";
            this.rdbAreaCut.UseVisualStyleBackColor = true;
            // 
            // gpbLengthUnit
            // 
            this.gpbLengthUnit.Controls.Add(this.rdbLengthM);
            this.gpbLengthUnit.Controls.Add(this.rdbLengthMM);
            this.gpbLengthUnit.Location = new System.Drawing.Point(6, 18);
            this.gpbLengthUnit.Name = "gpbLengthUnit";
            this.gpbLengthUnit.Size = new System.Drawing.Size(80, 45);
            this.gpbLengthUnit.TabIndex = 12;
            this.gpbLengthUnit.TabStop = false;
            this.gpbLengthUnit.Text = "gpbLengthUnit";
            // 
            // rdbLengthM
            // 
            this.rdbLengthM.AutoSize = true;
            this.rdbLengthM.Location = new System.Drawing.Point(46, 18);
            this.rdbLengthM.Name = "rdbLengthM";
            this.rdbLengthM.Size = new System.Drawing.Size(82, 16);
            this.rdbLengthM.TabIndex = 14;
            this.rdbLengthM.TabStop = true;
            this.rdbLengthM.Text = "rdbLengthM";
            this.rdbLengthM.UseVisualStyleBackColor = true;
            // 
            // rdbLengthMM
            // 
            this.rdbLengthMM.AutoSize = true;
            this.rdbLengthMM.Location = new System.Drawing.Point(6, 18);
            this.rdbLengthMM.Name = "rdbLengthMM";
            this.rdbLengthMM.Size = new System.Drawing.Size(91, 16);
            this.rdbLengthMM.TabIndex = 13;
            this.rdbLengthMM.TabStop = true;
            this.rdbLengthMM.Text = "rdbLengthMM";
            this.rdbLengthMM.UseVisualStyleBackColor = true;
            // 
            // lblAreaOrder
            // 
            this.lblAreaOrder.AutoSize = true;
            this.lblAreaOrder.Location = new System.Drawing.Point(156, 72);
            this.lblAreaOrder.Name = "lblAreaOrder";
            this.lblAreaOrder.Size = new System.Drawing.Size(69, 12);
            this.lblAreaOrder.TabIndex = 2;
            this.lblAreaOrder.Text = "lblAreaOrder";
            this.lblAreaOrder.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtAreaDecimal
            // 
            this.txtAreaDecimal.Location = new System.Drawing.Point(87, 70);
            this.txtAreaDecimal.MaxLength = 1;
            this.txtAreaDecimal.Name = "txtAreaDecimal";
            this.txtAreaDecimal.Size = new System.Drawing.Size(67, 19);
            this.txtAreaDecimal.TabIndex = 15;
            this.txtAreaDecimal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtAreaDecimal.Validated += new System.EventHandler(this.txtAreaDecimal_Validated);
            // 
            // lblAreaDecimal
            // 
            this.lblAreaDecimal.AutoSize = true;
            this.lblAreaDecimal.Location = new System.Drawing.Point(6, 72);
            this.lblAreaDecimal.Name = "lblAreaDecimal";
            this.lblAreaDecimal.Size = new System.Drawing.Size(82, 12);
            this.lblAreaDecimal.TabIndex = 0;
            this.lblAreaDecimal.Text = "lblAreaDecimal";
            this.lblAreaDecimal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FormCalcDraw
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(313, 561);
            this.Controls.Add(this.gpbAreaDecimal);
            this.Controls.Add(this.txtBMHeightOld);
            this.Controls.Add(this.btnUpdateLevel);
            this.Controls.Add(this.txtBMHeight);
            this.Controls.Add(this.lblBMHeight);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.gpbCalcPoint);
            this.Controls.Add(this.gpbCalcDraw);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormCalcDraw";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormCalcDraw";
            this.Load += new System.EventHandler(this.FormCalcDraw_Load);
            this.gpbCalcPoint.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCalcPoint)).EndInit();
            this.gpbCalcDraw.ResumeLayout(false);
            this.gpbCalcDraw.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errPvd)).EndInit();
            this.gpbAreaDecimal.ResumeLayout(false);
            this.gpbAreaDecimal.PerformLayout();
            this.gpbAreaRounding.ResumeLayout(false);
            this.gpbAreaRounding.PerformLayout();
            this.gpbLengthUnit.ResumeLayout(false);
            this.gpbLengthUnit.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox txtVertical;
    private System.Windows.Forms.TextBox txtHorizontal;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Label lbl1Slash;
    private System.Windows.Forms.GroupBox gpbCalcPoint;
    private System.Windows.Forms.Button btnDel;
    private System.Windows.Forms.Button btnDn;
    private System.Windows.Forms.Button btnUp;
    private System.Windows.Forms.DataGridView dgvCalcPoint;
    private System.Windows.Forms.GroupBox gpbCalcDraw;
    private System.Windows.Forms.Label lblVertical;
    private System.Windows.Forms.Label lblHorizontal;
    private System.Windows.Forms.TextBox txtScale;
    private System.Windows.Forms.Label lblRate;
    private System.Windows.Forms.Label lblScale;
    private System.Windows.Forms.ErrorProvider errPvd;
    private System.Windows.Forms.Button btnUpdateNumber;
    private System.Windows.Forms.Button btnCreate;
    private System.Windows.Forms.TextBox txtBMHeight;
    private System.Windows.Forms.Label lblBMHeight;
    private System.Windows.Forms.Button btnUpdateLevel;
    private System.Windows.Forms.TextBox txtBMHeightOld;
    private System.Windows.Forms.GroupBox gpbAreaDecimal;
    private System.Windows.Forms.GroupBox gpbLengthUnit;
    private System.Windows.Forms.RadioButton rdbLengthM;
    private System.Windows.Forms.RadioButton rdbLengthMM;
    private System.Windows.Forms.GroupBox gpbAreaRounding;
    private System.Windows.Forms.RadioButton rdbAreaRounding;
    private System.Windows.Forms.RadioButton rdbAreaClose;
    private System.Windows.Forms.RadioButton rdbAreaCut;
    private System.Windows.Forms.Label lblAreaOrder;
    private System.Windows.Forms.TextBox txtAreaDecimal;
    private System.Windows.Forms.Label lblAreaDecimal;
  }
}
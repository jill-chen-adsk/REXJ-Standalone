namespace ADSK.JExtRAC.FittingSchedule.Layout
{
  partial class FormLayoutPartsDrawing
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
            this.lblSolidPicture = new System.Windows.Forms.Label();
            this.lstSolidPicture = new System.Windows.Forms.ListBox();
            this.lblPlacementSolidPicture = new System.Windows.Forms.Label();
            this.lstPlacementSolidPicture = new System.Windows.Forms.ListBox();
            this.rdbWindow = new System.Windows.Forms.RadioButton();
            this.rdbDoor = new System.Windows.Forms.RadioButton();
            this.rdbBoth = new System.Windows.Forms.RadioButton();
            this.btnMove = new System.Windows.Forms.Button();
            this.btnDel = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDn = new System.Windows.Forms.Button();
            this.lblBlank = new System.Windows.Forms.Label();
            this.gpbBlank = new System.Windows.Forms.GroupBox();
            this.lblTop = new System.Windows.Forms.Label();
            this.lblBottom = new System.Windows.Forms.Label();
            this.lblLeft = new System.Windows.Forms.Label();
            this.lblRight = new System.Windows.Forms.Label();
            this.txtTop = new System.Windows.Forms.TextBox();
            this.txtBottom = new System.Windows.Forms.TextBox();
            this.txtLeft = new System.Windows.Forms.TextBox();
            this.txtRight = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.errPvd = new System.Windows.Forms.ErrorProvider(this.components);
            this.btnNewLine = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.errPvd)).BeginInit();
            this.SuspendLayout();
            // 
            // lblSolidPicture
            // 
            this.lblSolidPicture.AutoSize = true;
            this.lblSolidPicture.Location = new System.Drawing.Point(12, 9);
            this.lblSolidPicture.Name = "lblSolidPicture";
            this.lblSolidPicture.Size = new System.Drawing.Size(90, 15);
            this.lblSolidPicture.TabIndex = 0;
            this.lblSolidPicture.Text = "lblSolidPicture";
            // 
            // lstSolidPicture
            // 
            this.lstSolidPicture.FormattingEnabled = true;
            this.lstSolidPicture.ItemHeight = 15;
            this.lstSolidPicture.Location = new System.Drawing.Point(12, 24);
            this.lstSolidPicture.Name = "lstSolidPicture";
            this.lstSolidPicture.Size = new System.Drawing.Size(222, 229);
            this.lstSolidPicture.TabIndex = 1;
            // 
            // lblPlacementSolidPicture
            // 
            this.lblPlacementSolidPicture.AutoSize = true;
            this.lblPlacementSolidPicture.Location = new System.Drawing.Point(284, 9);
            this.lblPlacementSolidPicture.Name = "lblPlacementSolidPicture";
            this.lblPlacementSolidPicture.Size = new System.Drawing.Size(150, 15);
            this.lblPlacementSolidPicture.TabIndex = 2;
            this.lblPlacementSolidPicture.Text = "lblPlacementSolidPicture";
            // 
            // lstPlacementSolidPicture
            // 
            this.lstPlacementSolidPicture.FormattingEnabled = true;
            this.lstPlacementSolidPicture.ItemHeight = 15;
            this.lstPlacementSolidPicture.Location = new System.Drawing.Point(286, 24);
            this.lstPlacementSolidPicture.Name = "lstPlacementSolidPicture";
            this.lstPlacementSolidPicture.Size = new System.Drawing.Size(253, 229);
            this.lstPlacementSolidPicture.TabIndex = 3;
            this.lstPlacementSolidPicture.Validated += new System.EventHandler(this.lstPlacementSolidPicture_Validated);
            // 
            // rdbWindow
            // 
            this.rdbWindow.AutoSize = true;
            this.rdbWindow.Location = new System.Drawing.Point(13, 250);
            this.rdbWindow.Name = "rdbWindow";
            this.rdbWindow.Size = new System.Drawing.Size(90, 19);
            this.rdbWindow.TabIndex = 0;
            this.rdbWindow.TabStop = true;
            this.rdbWindow.Text = "rdbWindow";
            this.rdbWindow.UseVisualStyleBackColor = true;
            this.rdbWindow.CheckedChanged += new System.EventHandler(this.rdbWindow_CheckedChanged);
            // 
            // rdbDoor
            // 
            this.rdbDoor.AutoSize = true;
            this.rdbDoor.Location = new System.Drawing.Point(96, 250);
            this.rdbDoor.Name = "rdbDoor";
            this.rdbDoor.Size = new System.Drawing.Size(74, 19);
            this.rdbDoor.TabIndex = 1;
            this.rdbDoor.TabStop = true;
            this.rdbDoor.Text = "rdbDoor";
            this.rdbDoor.UseVisualStyleBackColor = true;
            this.rdbDoor.CheckedChanged += new System.EventHandler(this.rdbDoor_CheckedChanged);
            // 
            // rdbBoth
            // 
            this.rdbBoth.AutoSize = true;
            this.rdbBoth.Location = new System.Drawing.Point(188, 250);
            this.rdbBoth.Name = "rdbBoth";
            this.rdbBoth.Size = new System.Drawing.Size(74, 19);
            this.rdbBoth.TabIndex = 2;
            this.rdbBoth.TabStop = true;
            this.rdbBoth.Text = "rdbBoth";
            this.rdbBoth.UseVisualStyleBackColor = true;
            this.rdbBoth.CheckedChanged += new System.EventHandler(this.rdbBoth_CheckedChanged);
            // 
            // btnMove
            // 
            this.btnMove.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMove.Location = new System.Drawing.Point(240, 24);
            this.btnMove.Name = "btnMove";
            this.btnMove.Size = new System.Drawing.Size(40, 30);
            this.btnMove.TabIndex = 5;
            this.btnMove.Text = "btnMove";
            this.btnMove.UseVisualStyleBackColor = true;
            this.btnMove.Click += new System.EventHandler(this.btnMove_Click);
            // 
            // btnDel
            // 
            this.btnDel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDel.Location = new System.Drawing.Point(240, 60);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(40, 30);
            this.btnDel.TabIndex = 6;
            this.btnDel.Text = "btnDel";
            this.btnDel.UseVisualStyleBackColor = true;
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // btnUp
            // 
            this.btnUp.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUp.Location = new System.Drawing.Point(240, 144);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(40, 30);
            this.btnUp.TabIndex = 8;
            this.btnUp.Text = "btnUp";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDn
            // 
            this.btnDn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDn.Location = new System.Drawing.Point(240, 180);
            this.btnDn.Name = "btnDn";
            this.btnDn.Size = new System.Drawing.Size(40, 30);
            this.btnDn.TabIndex = 9;
            this.btnDn.Text = "btnDn";
            this.btnDn.UseVisualStyleBackColor = true;
            this.btnDn.Click += new System.EventHandler(this.btnDn_Click);
            // 
            // lblBlank
            // 
            this.lblBlank.AutoSize = true;
            this.lblBlank.Location = new System.Drawing.Point(12, 280);
            this.lblBlank.Name = "lblBlank";
            this.lblBlank.Size = new System.Drawing.Size(53, 15);
            this.lblBlank.TabIndex = 9;
            this.lblBlank.Text = "lblBlank";
            // 
            // gpbBlank
            // 
            this.gpbBlank.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.gpbBlank.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.gpbBlank.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.gpbBlank.Location = new System.Drawing.Point(47, 286);
            this.gpbBlank.Name = "gpbBlank";
            this.gpbBlank.Size = new System.Drawing.Size(490, 1);
            this.gpbBlank.TabIndex = 10;
            this.gpbBlank.TabStop = false;
            // 
            // lblTop
            // 
            this.lblTop.AutoSize = true;
            this.lblTop.Location = new System.Drawing.Point(27, 307);
            this.lblTop.Name = "lblTop";
            this.lblTop.Size = new System.Drawing.Size(43, 15);
            this.lblTop.TabIndex = 11;
            this.lblTop.Text = "lblTop";
            // 
            // lblBottom
            // 
            this.lblBottom.AutoSize = true;
            this.lblBottom.Location = new System.Drawing.Point(207, 307);
            this.lblBottom.Name = "lblBottom";
            this.lblBottom.Size = new System.Drawing.Size(63, 15);
            this.lblBottom.TabIndex = 12;
            this.lblBottom.Text = "lblBottom";
            // 
            // lblLeft
            // 
            this.lblLeft.AutoSize = true;
            this.lblLeft.Location = new System.Drawing.Point(27, 331);
            this.lblLeft.Name = "lblLeft";
            this.lblLeft.Size = new System.Drawing.Size(42, 15);
            this.lblLeft.TabIndex = 13;
            this.lblLeft.Text = "lblLeft";
            // 
            // lblRight
            // 
            this.lblRight.AutoSize = true;
            this.lblRight.Location = new System.Drawing.Point(207, 331);
            this.lblRight.Name = "lblRight";
            this.lblRight.Size = new System.Drawing.Size(51, 15);
            this.lblRight.TabIndex = 14;
            this.lblRight.Text = "lblRight";
            // 
            // txtTop
            // 
            this.txtTop.Location = new System.Drawing.Point(56, 304);
            this.txtTop.MaxLength = 3;
            this.txtTop.Name = "txtTop";
            this.txtTop.Size = new System.Drawing.Size(100, 23);
            this.txtTop.TabIndex = 15;
            this.txtTop.Validated += new System.EventHandler(this.txtTop_Validated);
            // 
            // txtBottom
            // 
            this.txtBottom.Location = new System.Drawing.Point(232, 304);
            this.txtBottom.MaxLength = 3;
            this.txtBottom.Name = "txtBottom";
            this.txtBottom.Size = new System.Drawing.Size(100, 23);
            this.txtBottom.TabIndex = 16;
            this.txtBottom.Validated += new System.EventHandler(this.txtBottom_Validated);
            // 
            // txtLeft
            // 
            this.txtLeft.Location = new System.Drawing.Point(56, 328);
            this.txtLeft.MaxLength = 3;
            this.txtLeft.Name = "txtLeft";
            this.txtLeft.Size = new System.Drawing.Size(100, 23);
            this.txtLeft.TabIndex = 17;
            this.txtLeft.Validated += new System.EventHandler(this.txtLeft_Validated);
            // 
            // txtRight
            // 
            this.txtRight.Location = new System.Drawing.Point(232, 328);
            this.txtRight.MaxLength = 3;
            this.txtRight.Name = "txtRight";
            this.txtRight.Size = new System.Drawing.Size(100, 23);
            this.txtRight.TabIndex = 18;
            this.txtRight.Validated += new System.EventHandler(this.txtRight_Validated);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(373, 328);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(80, 23);
            this.btnOK.TabIndex = 19;
            this.btnOK.Text = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(459, 328);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // errPvd
            // 
            this.errPvd.ContainerControl = this;
            // 
            // btnNewLine
            // 
            this.btnNewLine.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewLine.Location = new System.Drawing.Point(240, 102);
            this.btnNewLine.Name = "btnNewLine";
            this.btnNewLine.Size = new System.Drawing.Size(40, 30);
            this.btnNewLine.TabIndex = 7;
            this.btnNewLine.Text = "btnNewLine";
            this.btnNewLine.UseVisualStyleBackColor = true;
            this.btnNewLine.Click += new System.EventHandler(this.btnNewLine_Click);
            // 
            // FormLayoutPartsDrawing
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(551, 360);
            this.Controls.Add(this.btnNewLine);
            this.Controls.Add(this.rdbBoth);
            this.Controls.Add(this.rdbDoor);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.rdbWindow);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtRight);
            this.Controls.Add(this.txtLeft);
            this.Controls.Add(this.txtBottom);
            this.Controls.Add(this.txtTop);
            this.Controls.Add(this.lblRight);
            this.Controls.Add(this.lblLeft);
            this.Controls.Add(this.lblBottom);
            this.Controls.Add(this.lblTop);
            this.Controls.Add(this.gpbBlank);
            this.Controls.Add(this.lblBlank);
            this.Controls.Add(this.btnDn);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnDel);
            this.Controls.Add(this.btnMove);
            this.Controls.Add(this.lstPlacementSolidPicture);
            this.Controls.Add(this.lblPlacementSolidPicture);
            this.Controls.Add(this.lstSolidPicture);
            this.Controls.Add(this.lblSolidPicture);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormLayoutPartsDrawing";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormLayoutPartsDrawing";
            this.Load += new System.EventHandler(this.FormLayoutPartsDrawing_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errPvd)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lblSolidPicture;
    private System.Windows.Forms.ListBox lstSolidPicture;
    private System.Windows.Forms.Label lblPlacementSolidPicture;
    private System.Windows.Forms.ListBox lstPlacementSolidPicture;
    private System.Windows.Forms.RadioButton rdbBoth;
    private System.Windows.Forms.RadioButton rdbDoor;
    private System.Windows.Forms.RadioButton rdbWindow;
    private System.Windows.Forms.Button btnMove;
    private System.Windows.Forms.Button btnDel;
    private System.Windows.Forms.Button btnUp;
    private System.Windows.Forms.Button btnDn;
    private System.Windows.Forms.Label lblBlank;
    private System.Windows.Forms.GroupBox gpbBlank;
    private System.Windows.Forms.Label lblTop;
    private System.Windows.Forms.Label lblBottom;
    private System.Windows.Forms.Label lblLeft;
    private System.Windows.Forms.Label lblRight;
    private System.Windows.Forms.TextBox txtTop;
    private System.Windows.Forms.TextBox txtBottom;
    private System.Windows.Forms.TextBox txtLeft;
    private System.Windows.Forms.TextBox txtRight;
    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.ErrorProvider errPvd;
    private System.Windows.Forms.Button btnNewLine;
  }
}

namespace ADSK.JExtRAC.FittingSchedule.CreateAndEdit
{
    partial class FormCreatePartsDrawing
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
            this.lblDoorTag = new System.Windows.Forms.Label();
            this.cboDoorTag = new System.Windows.Forms.ComboBox();
            this.lblWindowTag = new System.Windows.Forms.Label();
            this.cboWindowTag = new System.Windows.Forms.ComboBox();
            this.gpbDuplicateView = new System.Windows.Forms.GroupBox();
            this.rdbViewChangeOld = new System.Windows.Forms.RadioButton();
            this.rdbViewNotUndate = new System.Windows.Forms.RadioButton();
            this.rdbViewDelOld = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.errPvd = new System.Windows.Forms.ErrorProvider(this.components);
            this.cboScale = new System.Windows.Forms.ComboBox();
            this.lblScale = new System.Windows.Forms.Label();
            this.cboDetailLevel = new System.Windows.Forms.ComboBox();
            this.lblDetailLevel = new System.Windows.Forms.Label();
            this.gpbDuplicateView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errPvd)).BeginInit();
            this.SuspendLayout();
            // 
            // lblDoorTag
            // 
            this.lblDoorTag.AutoSize = true;
            this.lblDoorTag.Location = new System.Drawing.Point(12, 9);
            this.lblDoorTag.Name = "lblDoorTag";
            this.lblDoorTag.Size = new System.Drawing.Size(69, 15);
            this.lblDoorTag.TabIndex = 0;
            this.lblDoorTag.Text = "lblDoorTag";
            this.lblDoorTag.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cboDoorTag
            // 
            this.cboDoorTag.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDoorTag.FormattingEnabled = true;
            this.cboDoorTag.Location = new System.Drawing.Point(90, 6);
            this.cboDoorTag.Name = "cboDoorTag";
            this.cboDoorTag.Size = new System.Drawing.Size(445, 23);
            this.cboDoorTag.TabIndex = 0;
            this.cboDoorTag.Validated += new System.EventHandler(this.cboDoorTag_Validated);
            // 
            // lblWindowTag
            // 
            this.lblWindowTag.AutoSize = true;
            this.lblWindowTag.Location = new System.Drawing.Point(12, 35);
            this.lblWindowTag.Name = "lblWindowTag";
            this.lblWindowTag.Size = new System.Drawing.Size(85, 15);
            this.lblWindowTag.TabIndex = 2;
            this.lblWindowTag.Text = "lblWindowTag";
            this.lblWindowTag.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cboWindowTag
            // 
            this.cboWindowTag.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboWindowTag.FormattingEnabled = true;
            this.cboWindowTag.Location = new System.Drawing.Point(90, 32);
            this.cboWindowTag.Name = "cboWindowTag";
            this.cboWindowTag.Size = new System.Drawing.Size(445, 23);
            this.cboWindowTag.TabIndex = 1;
            this.cboWindowTag.Validated += new System.EventHandler(this.cboWindowTag_Validated);
            // 
            // gpbDuplicateView
            // 
            this.gpbDuplicateView.Controls.Add(this.rdbViewChangeOld);
            this.gpbDuplicateView.Controls.Add(this.rdbViewNotUndate);
            this.gpbDuplicateView.Controls.Add(this.rdbViewDelOld);
            this.gpbDuplicateView.Location = new System.Drawing.Point(14, 112);
            this.gpbDuplicateView.Name = "gpbDuplicateView";
            this.gpbDuplicateView.Size = new System.Drawing.Size(521, 87);
            this.gpbDuplicateView.TabIndex = 3;
            this.gpbDuplicateView.TabStop = false;
            this.gpbDuplicateView.Text = "gpbDuplicateView";
            // 
            // rdbViewChangeOld
            // 
            this.rdbViewChangeOld.AutoSize = true;
            this.rdbViewChangeOld.Location = new System.Drawing.Point(6, 62);
            this.rdbViewChangeOld.Name = "rdbViewChangeOld";
            this.rdbViewChangeOld.Size = new System.Drawing.Size(136, 19);
            this.rdbViewChangeOld.TabIndex = 2;
            this.rdbViewChangeOld.TabStop = true;
            this.rdbViewChangeOld.Text = "rdbViewChangeOld";
            this.rdbViewChangeOld.UseVisualStyleBackColor = true;
            // 
            // rdbViewNotUndate
            // 
            this.rdbViewNotUndate.AutoSize = true;
            this.rdbViewNotUndate.Location = new System.Drawing.Point(6, 40);
            this.rdbViewNotUndate.Name = "rdbViewNotUndate";
            this.rdbViewNotUndate.Size = new System.Drawing.Size(136, 19);
            this.rdbViewNotUndate.TabIndex = 1;
            this.rdbViewNotUndate.TabStop = true;
            this.rdbViewNotUndate.Text = "rdbViewNotUndate";
            this.rdbViewNotUndate.UseVisualStyleBackColor = true;
            // 
            // rdbViewDelOld
            // 
            this.rdbViewDelOld.AutoSize = true;
            this.rdbViewDelOld.Location = new System.Drawing.Point(6, 18);
            this.rdbViewDelOld.Name = "rdbViewDelOld";
            this.rdbViewDelOld.Size = new System.Drawing.Size(114, 19);
            this.rdbViewDelOld.TabIndex = 0;
            this.rdbViewDelOld.TabStop = true;
            this.rdbViewDelOld.Text = "rdbViewDelOld";
            this.rdbViewDelOld.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(369, 205);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(80, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(455, 205);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // errPvd
            // 
            this.errPvd.ContainerControl = this;
            // 
            // cboScale
            // 
            this.cboScale.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboScale.FormattingEnabled = true;
            this.cboScale.Location = new System.Drawing.Point(90, 58);
            this.cboScale.Name = "cboScale";
            this.cboScale.Size = new System.Drawing.Size(80, 23);
            this.cboScale.TabIndex = 2;
            this.cboScale.SelectedIndexChanged += new System.EventHandler(this.cboScale_SelectedIndexChanged);
            // 
            // lblScale
            // 
            this.lblScale.AutoSize = true;
            this.lblScale.Location = new System.Drawing.Point(12, 61);
            this.lblScale.Name = "lblScale";
            this.lblScale.Size = new System.Drawing.Size(52, 15);
            this.lblScale.TabIndex = 26;
            this.lblScale.Text = "lblScale";
            this.lblScale.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cboDetailLevel
            // 
            this.cboDetailLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDetailLevel.FormattingEnabled = true;
            this.cboDetailLevel.Location = new System.Drawing.Point(90, 84);
            this.cboDetailLevel.Name = "cboDetailLevel";
            this.cboDetailLevel.Size = new System.Drawing.Size(80, 23);
            this.cboDetailLevel.TabIndex = 2;
            this.cboDetailLevel.SelectedIndexChanged += new System.EventHandler(this.cboScale_SelectedIndexChanged);
            // 
            // lblDetailLevel
            // 
            this.lblDetailLevel.AutoSize = true;
            this.lblDetailLevel.Location = new System.Drawing.Point(12, 87);
            this.lblDetailLevel.Name = "lblDetailLevel";
            this.lblDetailLevel.Size = new System.Drawing.Size(85, 15);
            this.lblDetailLevel.TabIndex = 26;
            this.lblDetailLevel.Text = "lblDetailLevel";
            this.lblDetailLevel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FormCreatePartsDrawing
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(547, 236);
            this.Controls.Add(this.lblDetailLevel);
            this.Controls.Add(this.lblScale);
            this.Controls.Add(this.cboDetailLevel);
            this.Controls.Add(this.cboScale);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.gpbDuplicateView);
            this.Controls.Add(this.cboWindowTag);
            this.Controls.Add(this.lblWindowTag);
            this.Controls.Add(this.cboDoorTag);
            this.Controls.Add(this.lblDoorTag);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormCreatePartsDrawing";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormCreatePartsDrawing";
            this.Load += new System.EventHandler(this.FormCreatePartsDrawing_Load);
            this.gpbDuplicateView.ResumeLayout(false);
            this.gpbDuplicateView.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errPvd)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDoorTag;
        private System.Windows.Forms.ComboBox cboDoorTag;
        private System.Windows.Forms.Label lblWindowTag;
        private System.Windows.Forms.ComboBox cboWindowTag;
        private System.Windows.Forms.GroupBox gpbDuplicateView;
        private System.Windows.Forms.RadioButton rdbViewChangeOld;
        private System.Windows.Forms.RadioButton rdbViewNotUndate;
        private System.Windows.Forms.RadioButton rdbViewDelOld;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ErrorProvider errPvd;
        private System.Windows.Forms.Label lblScale;
        private System.Windows.Forms.ComboBox cboScale;
        private System.Windows.Forms.Label lblDetailLevel;
        private System.Windows.Forms.ComboBox cboDetailLevel;
    }
}

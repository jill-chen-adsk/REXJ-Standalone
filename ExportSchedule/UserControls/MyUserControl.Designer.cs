namespace ADSK.JExtRAC.ExportSchedule.UserControls
{
    partial class MyUserControl
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
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chbItemized = new System.Windows.Forms.CheckBox();
            this.radForImport = new System.Windows.Forms.RadioButton();
            this.radForSchedule = new System.Windows.Forms.RadioButton();
            this.chbAddDateTime = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // chbItemized
            // 
            this.chbItemized.AutoSize = true;
            this.chbItemized.Location = new System.Drawing.Point(185, 3);
            this.chbItemized.Name = "chbItemized";
            this.chbItemized.Size = new System.Drawing.Size(125, 17);
            this.chbItemized.TabIndex = 33;
            this.chbItemized.Text = "";
            this.chbItemized.UseVisualStyleBackColor = true;
            // 
            // radForImport
            // 
            this.radForImport.AutoSize = true;
            this.radForImport.Checked = true;
            this.radForImport.Location = new System.Drawing.Point(22, 25);
            this.radForImport.Name = "radForImport";
            this.radForImport.Size = new System.Drawing.Size(128, 17);
            this.radForImport.TabIndex = 34;
            this.radForImport.TabStop = true;
            this.radForImport.Text = "";
            this.radForImport.UseVisualStyleBackColor = true;
            // 
            // radForSchedule
            // 
            this.radForSchedule.AutoSize = true;
            this.radForSchedule.Location = new System.Drawing.Point(185, 25);
            this.radForSchedule.Name = "radForSchedule";
            this.radForSchedule.Size = new System.Drawing.Size(142, 17);
            this.radForSchedule.TabIndex = 35;
            this.radForSchedule.Text = "";
            this.radForSchedule.UseVisualStyleBackColor = true;
            // 
            // chbAddDateTime
            // 
            this.chbAddDateTime.AutoSize = true;
            this.chbAddDateTime.Checked = true;
            this.chbAddDateTime.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbAddDateTime.Location = new System.Drawing.Point(22, 3);
            this.chbAddDateTime.Name = "chbAddDateTime";
            this.chbAddDateTime.Size = new System.Drawing.Size(159, 17);
            this.chbAddDateTime.TabIndex = 36;
            this.chbAddDateTime.Text = "";
            this.chbAddDateTime.UseVisualStyleBackColor = true;
            this.chbAddDateTime.CheckedChanged += new System.EventHandler(this.chbAddDateTime_CheckedChanged);
            // 
            // MyUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chbAddDateTime);
            this.Controls.Add(this.radForSchedule);
            this.Controls.Add(this.radForImport);
            this.Controls.Add(this.chbItemized);
            this.Name = "MyUserControl";
            this.Size = new System.Drawing.Size(494, 54);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chbItemized;
        private System.Windows.Forms.RadioButton radForImport;
        private System.Windows.Forms.RadioButton radForSchedule;
        private System.Windows.Forms.CheckBox chbAddDateTime;
    }
}

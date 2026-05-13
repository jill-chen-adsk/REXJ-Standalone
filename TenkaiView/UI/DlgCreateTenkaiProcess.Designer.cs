using R = ADSK.ViewExtension.TenkaiView.Resources;

namespace ADSK.ViewExtension.TenkaiView.UI
{
    partial class DlgCreateTenkaiProcess
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components != null)
                    components.Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        private void InitializeComponent()
        {
            lblInformation = new System.Windows.Forms.Label();
            ProgressBar1 = new System.Windows.Forms.ProgressBar();
            btnStop = new System.Windows.Forms.Button();
            lblMax = new System.Windows.Forms.Label();
            btnStart = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            SuspendLayout();
            //
            // lblInformation
            //
            lblInformation.AutoSize = true;
            lblInformation.Location = new System.Drawing.Point(12, 9);
            lblInformation.Name = "lblInformation";
            lblInformation.Size = new System.Drawing.Size(241, 12);
            lblInformation.TabIndex = 2;
            lblInformation.Text = R.Text.PROC_INFO;
            //
            // ProgressBar1
            //
            ProgressBar1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            ProgressBar1.Location = new System.Drawing.Point(13, 33);
            ProgressBar1.Name = "ProgressBar1";
            ProgressBar1.Size = new System.Drawing.Size(409, 23);
            ProgressBar1.TabIndex = 4;
            //
            // btnStop
            //
            btnStop.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            btnStop.Enabled = false;
            btnStop.Location = new System.Drawing.Point(186, 66);
            btnStop.Name = "btnStop";
            btnStop.Size = new System.Drawing.Size(75, 23);
            btnStop.TabIndex = 1;
            btnStop.Text = R.Text.BTN_STOP;
            btnStop.UseVisualStyleBackColor = true;
            //
            // lblMax
            //
            lblMax.AutoSize = true;
            lblMax.Location = new System.Drawing.Point(390, 9);
            lblMax.Name = "lblMax";
            lblMax.Size = new System.Drawing.Size(23, 12);
            lblMax.TabIndex = 3;
            lblMax.Text = "272";
            lblMax.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // btnStart
            //
            btnStart.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            btnStart.Location = new System.Drawing.Point(105, 66);
            btnStart.Name = "btnStart";
            btnStart.Size = new System.Drawing.Size(75, 23);
            btnStart.TabIndex = 0;
            btnStart.Text = R.Text.BTN_START;
            btnStart.UseVisualStyleBackColor = true;
            //
            // btnCancel
            //
            btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(267, 66);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(75, 23);
            btnCancel.TabIndex = 5;
            btnCancel.Text = R.Text.DLG_CANCEL;
            btnCancel.UseVisualStyleBackColor = true;
            //
            // DlgCreateTenkaiProcess
            //
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(439, 105);
            ControlBox = true;
            Controls.Add(btnCancel);
            Controls.Add(btnStart);
            Controls.Add(lblMax);
            Controls.Add(btnStop);
            Controls.Add(ProgressBar1);
            Controls.Add(lblInformation);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(455, 144);
            Name = "DlgCreateTenkaiProcess";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "dlgCreateTenkaiProcess";
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Label lblInformation;
        private System.Windows.Forms.ProgressBar ProgressBar1;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label lblMax;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnCancel;
    }
}

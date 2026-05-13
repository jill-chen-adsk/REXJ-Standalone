using System;
using System.Windows.Forms;

namespace ADSK.Ext.Fukashi.Utils
{
    public class ProgressBarForm : Form
    {
        private ProgressBar _progressBar;
        private Label _label;
        private int _maxSteps;

        public ProgressBarForm(bool showCancel, bool showPercentage)
        {
            this.Text = "Processing...";
            this.Size = new System.Drawing.Size(400, 120);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = false;
            this.TopMost = true;

            _label = new Label();
            _label.Location = new System.Drawing.Point(10, 10);
            _label.Size = new System.Drawing.Size(370, 20);
            _label.AutoSize = false;
            this.Controls.Add(_label);

            _progressBar = new ProgressBar();
            _progressBar.Location = new System.Drawing.Point(10, 35);
            _progressBar.Size = new System.Drawing.Size(365, 25);
            _progressBar.Minimum = 0;
            this.Controls.Add(_progressBar);
        }

        public void SetData(string text, int maxSteps, int currentStep)
        {
            _label.Text = text;
            _maxSteps = maxSteps;
            _progressBar.Maximum = maxSteps;
            _progressBar.Value = Math.Min(currentStep, maxSteps);
        }

        public void Active()
        {
            Application.DoEvents();
        }

        public new void ShowDialog()
        {
            this.Show();
            Application.DoEvents();
        }

        public new void Close()
        {
            if (!this.IsDisposed)
            {
                base.Close();
                base.Dispose();
            }
        }
    }
}

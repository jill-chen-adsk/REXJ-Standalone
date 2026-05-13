using System;
using System.Windows.Forms;

namespace ADSK.JExtRAC.SwitchJoinOrder.Utils
{
    public class ProgressDialog : Form
    {
        private ProgressBar _progressBar;
        private Label _label;

        public ProgressDialog(string title, int maximum)
        {
            this.Text = title;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ControlBox = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ClientSize = new System.Drawing.Size(400, 80);
            this.TopMost = true;

            _label = new Label();
            _label.Location = new System.Drawing.Point(12, 10);
            _label.Size = new System.Drawing.Size(376, 20);
            _label.Text = title;
            this.Controls.Add(_label);

            _progressBar = new ProgressBar();
            _progressBar.Location = new System.Drawing.Point(12, 35);
            _progressBar.Size = new System.Drawing.Size(376, 25);
            _progressBar.Minimum = 0;
            _progressBar.Maximum = maximum > 0 ? maximum : 1;
            _progressBar.Value = 0;
            _progressBar.Style = ProgressBarStyle.Continuous;
            this.Controls.Add(_progressBar);
        }

        public void SetMaximum(int maximum)
        {
            if (_progressBar.InvokeRequired)
                _progressBar.Invoke(new Action(() => _progressBar.Maximum = maximum));
            else
                _progressBar.Maximum = maximum;
        }

        public void SetProgress(int value)
        {
            if (_progressBar.InvokeRequired)
                _progressBar.Invoke(new Action(() => { if (value <= _progressBar.Maximum) _progressBar.Value = value; }));
            else if (value <= _progressBar.Maximum)
                _progressBar.Value = value;
        }

        public void ShowNonModal()
        {
            this.Show();
            Application.DoEvents();
        }

        public void UpdateProgress(int value)
        {
            SetProgress(value);
            Application.DoEvents();
        }
    }
}

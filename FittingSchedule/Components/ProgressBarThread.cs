using System;
using System.Drawing;
using System.Windows.Forms;

namespace ADSK.JExtRAC.FittingSchedule.Components
{
    public class ProgressBarThread
    {
        private Form _form;
        private ProgressBar _progressBar;
        private Label _label;

        public ProgressBarThread(bool showCancel, bool showProgress)
        {
            _form = new Form
            {
                Text = "Processing...",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,
                MaximizeBox = false,
                MinimizeBox = false,
                ControlBox = false,
                TopMost = true,
                AutoScaleMode = AutoScaleMode.Dpi,
                AutoScaleDimensions = new SizeF(96F, 96F),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point),
                ClientSize = new Size(400, 80),
                Padding = new Padding(12)
            };

            _label = new Label
            {
                Dock = DockStyle.Top,
                Height = 24,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false
            };

            _progressBar = new ProgressBar
            {
                Dock = DockStyle.Bottom,
                Height = 28
            };

            _form.Controls.Add(_progressBar);
            _form.Controls.Add(_label);
        }

        public void ShowDialog()
        {
            _form.Show();
            Application.DoEvents();
        }

        public void SetData(string text, int value)
        {
            _label.Text = text;
            _progressBar.Value = 0;
            Application.DoEvents();
        }

        public void SetData(int max, int value)
        {
            _progressBar.Minimum = 0;
            _progressBar.Maximum = Math.Max(max, 1);
            _progressBar.Value = Math.Min(value, _progressBar.Maximum);
            Application.DoEvents();
        }

        public void SetData(int value)
        {
            if (value <= _progressBar.Maximum)
                _progressBar.Value = value;
            Application.DoEvents();
        }

        public void Close()
        {
            if (_form != null && !_form.IsDisposed)
            {
                _form.Close();
                _form.Dispose();
            }
        }
    }
}

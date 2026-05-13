using System;
using System.Windows.Forms;

namespace ADSK.JExtRAC.AreaSchedule.Utils
{
    public sealed class ProgressBarThread : IDisposable
    {
        private readonly Form _form;
        private readonly ProgressBar _bar;

        public ProgressBarThread(bool _, bool unusedTopMostFlag)
        {
            _ = unusedTopMostFlag;

            _form = new Form
            {
                FormBorderStyle = FormBorderStyle.FixedToolWindow,
                StartPosition = FormStartPosition.CenterScreen,
                TopMost = true,
                Width = 360,
                Height = 80,
                Text = "",
                ControlBox = true,
                ShowInTaskbar = false,
                Visible = false
            };
            _bar = new ProgressBar
            {
                Dock = DockStyle.Fill,
                Style = ProgressBarStyle.Continuous,
                Minimum = 0,
                Maximum = 100
            };
            _form.Controls.Add(_bar);
        }

        public void ShowDialog()
        {
            if (_form == null || _form.Visible) return;
            _form.Visible = true;
            _form.Show();
            Application.DoEvents();
        }

        public void Close()
        {
            try
            {
                if (_form != null && !_form.IsDisposed)
                {
                    _form.Visible = false;
                    _form.Hide();
                }
            }
            catch { }
        }

        public void SetData(int maximum, int value)
        {
            try
            {
                _bar.Maximum = Math.Max(maximum, 1);
                _bar.Value = Math.Clamp(value, _bar.Minimum, _bar.Maximum);
                Application.DoEvents();
            }
            catch { }
        }

        public void SetData(int value)
        {
            SetData(_bar.Maximum <= 0 ? 100 : _bar.Maximum, value);
        }

        /// <summary>Caption + progress hint (ported from legacy API).</summary>
        public void SetData(string caption, int value)
        {
            if (_form != null && !_form.IsDisposed)
                _form.Text = caption ?? "";
            SetData(value);
        }

        public void Dispose()
        {
            try
            {
                _form?.Close();
                _form?.Dispose();
            }
            catch { }
        }
    }
}

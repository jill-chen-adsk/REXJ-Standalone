using System;
using System.Windows.Forms;

namespace SectionListSteel.JExtComCompat
{
    public class ProgressBarThread : IDisposable
    {
        private Form? _form;
        private ProgressBar? _progressBar;
        private Label? _label;
        private bool _disposed;

        public ProgressBarThread(bool param1, bool param2)
        {
        }

        public void ShowDialog()
        {
            _form = new Form
            {
                Text = "Processing...",
                Width = 400,
                Height = 120,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,
                MaximizeBox = false,
                MinimizeBox = false,
                ControlBox = false,
                TopMost = true
            };

            _label = new Label
            {
                Left = 10,
                Top = 10,
                Width = 370,
                Height = 20,
                Text = ""
            };

            _progressBar = new ProgressBar
            {
                Left = 10,
                Top = 35,
                Width = 370,
                Height = 25,
                Minimum = 0,
                Maximum = 100,
                Value = 0
            };

            _form.Controls.Add(_label);
            _form.Controls.Add(_progressBar);
            _form.Show();
            Application.DoEvents();
        }

        public void SetData(string title, int max, int current)
        {
            if (_form == null || _form.IsDisposed) return;

            if (_label != null) _label.Text = $"{title} ({current}/{max})";
            if (_progressBar != null)
            {
                _progressBar.Maximum = max > 0 ? max : 1;
                _progressBar.Value = Math.Min(current, _progressBar.Maximum);
            }
        }

        public void Active()
        {
            if (_form == null || _form.IsDisposed) return;
            Application.DoEvents();
        }

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (_form != null && !_form.IsDisposed)
            {
                _form.Close();
                _form.Dispose();
            }
        }
    }
}

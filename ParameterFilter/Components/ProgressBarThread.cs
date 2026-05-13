using System;
using System.Threading;
using System.Windows.Forms;

namespace ADSK.JExtRAC.ParameterFilter.Components
{
    public class ProgressBarThread
    {
        private Form _form;
        private ProgressBar _progressBar;
        private Thread _thread;
        private ManualResetEvent _startEvent;
        private string _txtItem = "";
        private int _numProgress;
        private int _cntProgress;
        private bool _showed;
        private readonly bool _visBtnCancel;
        private bool _canceled;
        private readonly bool _showTaskbar;

        public bool Canceled
        {
            get
            {
                if (_form != null && !_form.IsDisposed)
                    _form.Invoke(new MethodInvoker(() => { }));
                return _canceled;
            }
        }

        public ProgressBarThread(bool visBtnCancel, bool showTaskbar)
        {
            _visBtnCancel = visBtnCancel;
            _showTaskbar = showTaskbar;
        }

        public void ShowDialog()
        {
            if (_showed) return;
            _showed = true;
            _startEvent = new ManualResetEvent(false);
            _thread = new Thread(FrmShowDialog) { IsBackground = true };
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Start();
            _startEvent.WaitOne();
        }

        private void FrmShowDialog()
        {
            _form = new Form
            {
                Text = "Processing...",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,
                MaximizeBox = false,
                MinimizeBox = false,
                ControlBox = false,
                ClientSize = new System.Drawing.Size(420, 75),
                ShowInTaskbar = _showTaskbar,
                Cursor = Cursors.WaitCursor
            };

            _progressBar = new ProgressBar
            {
                Location = new System.Drawing.Point(8, 22),
                Size = new System.Drawing.Size(400, 16),
                Style = ProgressBarStyle.Continuous
            };
            _form.Controls.Add(_progressBar);

            if (_visBtnCancel)
            {
                var btn = new Button
                {
                    Text = "Cancel",
                    Location = new System.Drawing.Point(333, 44),
                    Size = new System.Drawing.Size(75, 23)
                };
                btn.Click += (s, e) => { btn.Enabled = false; _canceled = true; };
                _form.Controls.Add(btn);
            }

            _form.Activated += (s, e) => { _startEvent.Set(); };
            _form.ShowDialog();
        }

        public void Close()
        {
            if (_form != null && !_form.IsDisposed)
                _form.Invoke(new MethodInvoker(() => _form.Close()));
        }

        public void SetData(string txtItem, int numProgress, int cntProgress)
        {
            _txtItem = txtItem; _numProgress = numProgress; _cntProgress = cntProgress;
            UpdateProgress();
        }

        public void SetData(string txtItem, int cntProgress)
        {
            _txtItem = txtItem; _cntProgress = cntProgress;
            UpdateProgress();
        }

        public void SetData(int numProgress, int cntProgress)
        {
            _numProgress = numProgress; _cntProgress = cntProgress;
            UpdateProgress();
        }

        public void SetData(int cntProgress)
        {
            _cntProgress = cntProgress;
            UpdateProgress();
        }

        private void UpdateProgress()
        {
            if (_form == null || _form.IsDisposed) return;
            _form.Invoke(new MethodInvoker(() =>
            {
                int val = _numProgress > 0
                    ? Math.Min(100, _cntProgress * 100 / _numProgress)
                    : 0;
                _progressBar.Value = val;
                _progressBar.Update();
                _form.Text = _txtItem + " " + val + "%";
            }));
        }
    }
}

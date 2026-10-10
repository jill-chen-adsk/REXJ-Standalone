using System;
using System.Windows.Threading;
using ADSK.JExtRAC.ParameterFilter.Utils;

namespace ADSK.JExtRAC.ParameterFilter.Components
{
    public sealed class ProgressBarThread : IDisposable
    {
        private WeaveProgressWindow _window;
        private int _maximum = 100;
        private IntPtr _ownerHandle = IntPtr.Zero;
        private string _caption = string.Empty;

        public ProgressBarThread(bool _, bool unusedTopMostFlag)
        {
            _ = unusedTopMostFlag;
        }

        public void SetOwner(IntPtr ownerHandle)
        {
            _ownerHandle = ownerHandle;
            RunOnWindow(() => _window?.SetOwnerHandle(_ownerHandle));
        }

        public void ShowDialog()
        {
            if (_window != null && _window.IsVisible)
                return;

            _window = new WeaveProgressWindow();
            if (_ownerHandle != IntPtr.Zero)
                _window.SetOwnerHandle(_ownerHandle);
            if (!string.IsNullOrEmpty(_caption))
                _window.SetMessage(_caption);

            _window.Show();
            PumpUi();
        }

        public void Close()
        {
            try
            {
                if (_window == null)
                    return;

                RunOnWindow(() =>
                {
                    _window.Hide();
                    _window.Close();
                    _window = null;
                });
            }
            catch
            {
                // ignored
            }
        }

        public void SetData(int maximum, int value)
        {
            _maximum = Math.Max(maximum, 1);
            RunOnWindow(() => _window?.SetProgress(_maximum, value));
            PumpUi();
        }

        public void SetData(int value)
        {
            SetData(_maximum <= 0 ? 100 : _maximum, value);
        }

        public void SetData(string caption, int value)
        {
            _caption = caption ?? string.Empty;
            RunOnWindow(() => _window?.SetMessage(_caption));
            SetData(value);
        }

        public void SetData(string caption, int numProgress, int cntProgress)
        {
            _caption = caption ?? string.Empty;
            RunOnWindow(() => _window?.SetMessage(_caption));
            if (numProgress > 0)
                SetData(numProgress, cntProgress);
            else
                SetData(cntProgress);
        }

        public void Dispose()
        {
            Close();
        }

        private void RunOnWindow(Action action)
        {
            if (_window == null)
                return;

            if (_window.Dispatcher.CheckAccess())
            {
                action();
                return;
            }

            _window.Dispatcher.Invoke(action);
        }

        private void PumpUi()
        {
            if (_window != null)
                _window.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { }));
        }
    }
}

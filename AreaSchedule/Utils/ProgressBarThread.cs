using System;
using System.Windows.Threading;

namespace ADSK.JExtRAC.AreaSchedule.Utils
{
    /// <summary>
    /// Shared Weave-compliant progress UI for all Area Schedule commands
    /// (Room to Area, Grounds Expression, Legal Area, and related service steps).
    /// </summary>
    public sealed class ProgressBarThread : IDisposable
    {
        private WeaveProgressWindow _window;
        private int _maximum = 100;
        private IntPtr _ownerHandle = IntPtr.Zero;
        private string _caption = string.Empty;
        private string _commandTitle = string.Empty;

        public void SetCommandTitle(string title)
        {
            _commandTitle = title ?? string.Empty;
            RunOnWindow(() => _window?.SetCommandTitle(_commandTitle));
        }

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
            if (!string.IsNullOrEmpty(_commandTitle))
                _window.SetCommandTitle(_commandTitle);
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

        /// <summary>Caption + progress hint (ported from legacy API).</summary>
        public void SetData(string caption, int value)
        {
            _caption = caption ?? string.Empty;
            RunOnWindow(() => _window?.SetMessage(_caption));
            SetData(value);
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
            {
                _window.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { }));
            }

            System.Windows.Forms.Application.DoEvents();
        }
    }
}

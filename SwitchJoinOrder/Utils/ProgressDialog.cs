using System;
using System.Windows.Threading;

namespace ADSK.JExtRAC.SwitchJoinOrder.Utils
{
    public sealed class ProgressDialog : IDisposable
    {
        private WeaveProgressWindow _window;
        private int _maximum = 1;
        private IntPtr _ownerHandle = IntPtr.Zero;

        public ProgressDialog(string title, int maximum)
        {
            _maximum = Math.Max(maximum, 1);
            _window = new WeaveProgressWindow();
            _window.SetCommandTitle(title);
            _window.SetMessage(title);
            _window.SetProgress(_maximum, 0);
        }

        public void SetOwner(IntPtr ownerHandle)
        {
            _ownerHandle = ownerHandle;
            RunOnWindow(() => _window?.SetOwnerHandle(_ownerHandle));
        }

        public void ShowNonModal()
        {
            if (_window == null)
                return;

            if (_ownerHandle != IntPtr.Zero)
                _window.SetOwnerHandle(_ownerHandle);

            _window.Show();
            PumpUi();
        }

        public void UpdateProgress(int value)
        {
            RunOnWindow(() => _window?.SetProgress(_maximum, value));
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

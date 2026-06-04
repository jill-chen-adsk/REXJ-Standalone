using System;
using System.Windows;
using System.Windows.Threading;

namespace ADSK.JExtRAC.FittingSchedule.Components
{
    public class ProgressBarThread
    {
        private ProgressWindow _window;

        public ProgressBarThread(bool showCancel, bool showProgress)
        {
            _window = new ProgressWindow();
        }

        public void ShowDialog()
        {
            _window.Show();
            DoEvents();
        }

        public void SetData(string text, int value)
        {
            if (_window == null) return;
            _window.SetStatus(text);
            DoEvents();
        }

        public void SetData(int max, int value)
        {
            if (_window == null) return;
            _window.SetMaximum(max, value);
            DoEvents();
        }

        public void SetData(int value)
        {
            if (_window == null) return;
            _window.SetValue(value);
            DoEvents();
        }

        public void Close()
        {
            if (_window != null)
            {
                _window.Close();
                _window = null;
            }
        }

        private static void DoEvents()
        {
            var frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(obj =>
                {
                    ((DispatcherFrame)obj).Continue = false;
                    return null;
                }),
                frame);
            Dispatcher.PushFrame(frame);
        }
    }
}

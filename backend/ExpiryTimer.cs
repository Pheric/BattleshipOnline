using System;
using System.Threading;
using System.Threading.Tasks;

namespace server {
    public class ExpiryTimer<T> {
        private Func<Task, T, bool> _callback;
        private T _caller;
        private Task _task;
        private CancellationTokenSource _cancellationTokenSource;
        private TimeSpan _timeSpan;

        public ExpiryTimer(TimeSpan delay, Func<Task, T, bool> callback, T caller) {
            _timeSpan = delay;
            _callback = callback;

            _cancellationTokenSource = new CancellationTokenSource();
            ReRun();
        }

        public void ReRun() {
            if (_cancellationTokenSource == null)
                _cancellationTokenSource = new CancellationTokenSource();
            else
                _cancellationTokenSource.Cancel();
            _task?.Dispose();
            
            _task = Task.Delay(_timeSpan).ContinueWith(t => _callback(t, _caller), _cancellationTokenSource.Token);
            _task.Start();
        }
        public void ReRun(TimeSpan _ts) {
            _timeSpan = _ts;
            ReRun();
        }

        public void Cancel() {
            _cancellationTokenSource?.Cancel();
        }
    }
}
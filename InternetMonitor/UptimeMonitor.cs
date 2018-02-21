using System;
using System.Threading;
using System.Threading.Tasks;

namespace InternetMonitor
{
    public class UptimeMonitor
    {
        private readonly IPinger _pinger;
        private readonly IResultLogger _resultLogger;
        private readonly Action _waitFunction;
        private CancellationTokenSource _tokenSource;
        private Task _runningTask;

        public UptimeMonitor(TimeSpan successWait, IPinger pinger, IResultLogger resultLogger, Action waitFunction = null)
        {
            _pinger = pinger;
            _resultLogger = resultLogger;
            _waitFunction = waitFunction ?? (() => Thread.Sleep(successWait));
        }

        public Task Run()
        {
            _tokenSource?.Dispose();
            _tokenSource = new CancellationTokenSource();
            var cancellationToken = _tokenSource.Token;
            _runningTask = Task.Factory.StartNew(() =>
            {
                while (cancellationToken.IsCancellationRequested == false)
                {
                    var result = false;
                    while (result == false)
                    {
                        result = _pinger.Ping();
                        _resultLogger.Log(DateTime.Now, result);
                    }

                    _waitFunction();
                }
            }, _tokenSource.Token);
            return _runningTask;
        }

        public void Stop()
        {
            _tokenSource.Cancel();
        }
    }
}

using System;

namespace InternetMonitor
{
    public class ResultLogger : IResultLogger
    {
        private readonly IOutputWriter _logWriter;
        private readonly IOutputWriter _failWriter;
        private DateTime _firstFailure = DateTime.MaxValue;
        private bool _isFailing;

        public ResultLogger(IOutputWriter logWriter, IOutputWriter failWriter)
        {
            _logWriter = logWriter;
            _failWriter = failWriter;
        }

        public void Log(DateTime timestamp, bool result)
        {
            if (result == false && _isFailing == false)
            {
                _logWriter.WriteLine($"{timestamp:yyyy-MM-dd HH:mm:ss} Failure begin.");
                _firstFailure = timestamp;
                _isFailing = true;
            }
            else if (result == false && _isFailing)
            {
                _logWriter.WriteLine($"{timestamp:yyyy-MM-dd HH:mm:ss} Failure continuing.");
            }
            else if (result && _isFailing)
            {
                var duration = timestamp - _firstFailure;
                _logWriter.WriteLine($"{timestamp:yyyy-MM-dd HH:mm:ss} Connection restored. Duration {duration}");
                _failWriter.WriteLine($"{timestamp:yyyy-MM-dd HH:mm:ss} Connection Restored. Begin: {_firstFailure:yyyy-MM-dd HH:mm:ss} Outage Duration: {duration}");
                _isFailing = false;
            }
            else
            {
                _logWriter.WriteLine($"{timestamp:yyyy-MM-dd HH:mm:ss} No Failure.");
            }
        }
    }
}
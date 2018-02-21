using System;

namespace InternetMonitor
{
    public interface IResultLogger
    {
        void Log(DateTime timestamp, bool result);
    }
}
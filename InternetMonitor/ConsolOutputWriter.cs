using System;

namespace InternetMonitor
{
    public class ConsolOutputWriter : IOutputWriter
    {
        public void WriteLine(string line)
        {
            Console.Out.WriteLine(line);
        }
    }
}

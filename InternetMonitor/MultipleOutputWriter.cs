namespace InternetMonitor
{
    public class MultipleOutputWriter : IOutputWriter
    {
        private readonly IOutputWriter[] _outputWriters;

        public MultipleOutputWriter(params IOutputWriter[] outputWriters)
        {
            _outputWriters = outputWriters ?? new IOutputWriter[0];
        }

        public void WriteLine(string line)
        {
            foreach (var outputWriter in _outputWriters)
            {
                outputWriter.WriteLine(line);
            }
        }
    }
}

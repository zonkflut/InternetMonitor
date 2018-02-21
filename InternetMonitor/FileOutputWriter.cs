using System.IO;

namespace InternetMonitor
{
    public class FileOutputWriter : IOutputWriter
    {
        private readonly string _filePath;

        public FileOutputWriter(string filePath)
        {
            _filePath = filePath;
        }

        public void WriteLine(string line)
        {
            using (var stream = new FileStream(_filePath, FileMode.Append))
            using (var writer = new StreamWriter(stream))
            {
                writer.WriteLine(line);
            }
        }
    }
}

using System;
using System.Configuration;
using Zonkflut.ArgumentValidation;
using static Zonkflut.ArgumentValidation.ArgumentValidator;

namespace InternetMonitor
{
    public class Program
    {
        public const string FailLogFileConfigKey = "FailLogFile";
        public const string LogFileConfigKey = "LogFile";
        public const string MinutesBetweenSuccessfulPingsConfigKey = "MinutesBetweenSuccessfulPings";
        public const string PingDestinationConfigKey = "PingDestination";

        public static void Main(string[] args)
        {
            var failLogFile = ConfigurationManager.AppSettings[FailLogFileConfigKey];
            var logFile = ConfigurationManager.AppSettings[LogFileConfigKey];
            var successfulPingSeconds = double.TryParse(ConfigurationManager.AppSettings[MinutesBetweenSuccessfulPingsConfigKey], out var d) ? d : 0;
            var pingDestination = ConfigurationManager.AppSettings[PingDestinationConfigKey];

            try
            {
                CheckArgument(() => failLogFile, $"appSettiongs key: {FailLogFileConfigKey}").Is.NotNullOrWhitespace();
                CheckArgument(() => logFile, $"appSettings key: {LogFileConfigKey}").Is.NotNullOrWhitespace();
                CheckArgument(() => successfulPingSeconds, $"appSettings key: {MinutesBetweenSuccessfulPingsConfigKey}").Is.GreaterThan(0);
                CheckArgument(() => pingDestination, $"appSettings key: {PingDestinationConfigKey}").Is.Matching("[0-9]{1,3}[.][0-9]{1,3}[.][0-9]{1,3}[.][0-9]{1,3}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            var pinger = new Pinger(pingDestination);
            var logWriter = new MultipleOutputWriter(
                new ConsolOutputWriter(),
                new FileOutputWriter(logFile)
            );
            var failWriter = new FileOutputWriter(failLogFile);
            var resultLogger = new ResultLogger(logWriter, failWriter);
            var monitor = new UptimeMonitor(TimeSpan.FromMinutes(successfulPingSeconds), pinger, resultLogger);

            var task = monitor.Run();
            task.Wait();
        }
    }
}

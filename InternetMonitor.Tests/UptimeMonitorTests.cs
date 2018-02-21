using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Rhino.Mocks;

namespace InternetMonitor.Tests
{
    [TestFixture]
    public class UptimeMonitorTests
    {
        [Test]
        public void Pingger_Fail()
        {
            var pinger = new Pinger("0.0.0.0");
            Assert.IsFalse(pinger.Ping());
        }

        [Test]
        public void Pinger_Success()
        {
            var pinger = new Pinger("8.8.8.8");
            Assert.IsTrue(pinger.Ping());
        }

        [Test]//, Ignore("integration")]
        public void UptimeMonitor_Runs()
        {
            var successWait = TimeSpan.FromSeconds(1);
            var pinger = new Pinger("8.8.8.8");
            var logWriter = new MultipleOutputWriter(
                new ConsolOutputWriter(),
                new FileOutputWriter("D:\\Projects\\InternetMonitor\\InternetMonitor\\InternetMonitor.Tests\\bin\\Debug\\log.txt")
            );
            var failWriter = new FileOutputWriter("D:\\Projects\\InternetMonitor\\InternetMonitor\\InternetMonitor.Tests\\bin\\Debug\\failures.txt");
            var resultLogger = new ResultLogger(logWriter, failWriter);
            var monitor = new UptimeMonitor(successWait, pinger, resultLogger);

            var task = monitor.Run();

            Thread.Sleep(TimeSpan.FromSeconds(60));

            monitor.Stop();

            task.Wait();
        }

        [Test]
        public void TestLogic()
        {
            var successWaitCount = 0;
            var pinger = MockRepository.GenerateStrictMock<IPinger>();
            var resultLogger = MockRepository.GenerateStub<IResultLogger>();

            pinger.Expect(p => p.Ping()).Return(true).Repeat.Once();
            resultLogger.Expect(l => l.Log(Arg<DateTime>.Is.Anything, Arg<bool>.Is.Equal(true))).Repeat.Once();

            pinger.Expect(p => p.Ping()).Return(false).Repeat.Once();
            resultLogger.Expect(l => l.Log(Arg<DateTime>.Is.Anything, Arg<bool>.Is.Equal(false))).Repeat.Once();
            pinger.Expect(p => p.Ping()).Return(false).Repeat.Once();
            resultLogger.Expect(l => l.Log(Arg<DateTime>.Is.Anything, Arg<bool>.Is.Equal(false))).Repeat.Once();

            pinger.Expect(p => p.Ping()).Return(true).Repeat.Once();
            resultLogger.Expect(l => l.Log(Arg<DateTime>.Is.Anything, Arg<bool>.Is.Equal(true))).Repeat.Once();

            UptimeMonitor monitor = null;

            monitor = new UptimeMonitor(TimeSpan.MinValue, pinger, resultLogger, () =>
            {
                successWaitCount++;
                if (successWaitCount == 2)
                    monitor.Stop();
            });

            monitor.Run().Wait();
        }

        [Test]
        public void ResultLoggerTests()
        {
            var logWriter = MockRepository.GenerateStrictMock<IOutputWriter>();
            var failWriter = MockRepository.GenerateStrictMock<IOutputWriter>();

            logWriter.Expect(l => l.WriteLine(Arg<string>.Matches(p => p.EndsWith("No Failure."))));
            logWriter.Expect(l => l.WriteLine(Arg<string>.Matches(p => p.EndsWith("Failure begin."))));
            logWriter.Expect(l => l.WriteLine(Arg<string>.Matches(p => p.EndsWith("Failure continuing."))));
            logWriter.Expect(l => l.WriteLine(Arg<string>.Matches(p => p.Contains("Connection restored. Outage Duration: "))));
            logWriter.Expect(l => l.WriteLine(Arg<string>.Matches(p => p.EndsWith("No Failure."))));

            failWriter.Expect(l => l.WriteLine(Arg<string>.Matches(p => p.Contains("Connection Restored. Begin: "))));

            var resultLogger = new ResultLogger(logWriter, failWriter);

            resultLogger.Log(DateTime.Now, true);
            resultLogger.Log(DateTime.Now, false);
            resultLogger.Log(DateTime.Now, false);
            resultLogger.Log(DateTime.Now, true);
            resultLogger.Log(DateTime.Now, true);
        }
    }
}

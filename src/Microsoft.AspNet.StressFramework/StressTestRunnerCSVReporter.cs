using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.AspNet.StressFramework.Collectors;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.AspNet.StressFramework
{
    public class StressTestRunnerCSVReporter : DefaultRunnerReporter
    {
        public override string RunnerSwitch => "stresscsv";

        public override string Description => "output stress test results to CSV files. 1 CSV file per test.";

        public override IMessageSink CreateMessageHandler(IRunnerLogger logger)
        {
            return new CSVRunnerReporterMessageHandler(new ConsoleRunnerLogger(useColors: true));
        }

        private class CSVRunnerReporterMessageHandler : DefaultStressTestMessageVisitor
        {
            public CSVRunnerReporterMessageHandler(IRunnerLogger logger)
                : base(logger)
            {
            }

            protected override bool Visit(MetricsRecordedMessage metricsRecordedMessage)
            {
                Debug.Assert(metricsRecordedMessage.Test.TestCase is StressTestCase);

                WriteCSV(metricsRecordedMessage);

                return true;
            }

            private static void WriteCSV(MetricsRecordedMessage metricsRecordedMessage)
            {
                var test = metricsRecordedMessage.Test;
                var metrics = metricsRecordedMessage.Metrics;
                var testOutputDir = Path.Combine(Directory.GetCurrentDirectory(), "TestOutput");
                if (!Directory.Exists(testOutputDir))
                {
                    Directory.CreateDirectory(testOutputDir);
                }

                var outputFile = Path.Combine(testOutputDir, $"{test.TestCase.DisplayName}.{test.TestCase.UniqueID}.metrics.csv");

                using (var writer = new StreamWriter(new FileStream(outputFile, FileMode.Create)))
                {
                    writer.WriteLine("Iteration,Heap,WorkingSet,PrivateBytes,ElapsedTicks");

                    foreach (var iteration in metrics.GroupBy(g => g.Iteration).OrderBy(g => g.Key))
                    {
                        var elapsed = (ElapsedTime)iteration.Single(m => m.Value is ElapsedTime).Value;
                        var mem = (MemoryUsage)iteration.Single(m => m.Value is MemoryUsage).Value;
                        writer.WriteLine($"{iteration.Key},{mem.HeapMemoryBytes},{mem.WorkingSet},{mem.PrivateBytes},{elapsed.Elapsed.Ticks}");
                    }
                }
            }
        }
    }
}

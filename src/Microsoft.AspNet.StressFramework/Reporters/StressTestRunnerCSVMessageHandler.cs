using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.AspNet.StressFramework.Collectors;
using Xunit;

namespace Microsoft.AspNet.StressFramework.Reporters
{
    public class StressTestRunnerCSVMessageHandler : StressTestRunnerMessageHandlerBase
    {
        public StressTestRunnerCSVMessageHandler(IRunnerLogger logger)
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

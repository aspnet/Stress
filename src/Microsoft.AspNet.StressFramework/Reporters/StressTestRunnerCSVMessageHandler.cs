using System.Diagnostics;
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

            var fileLocation = StressTestOutputManager.WriteTestOutput(metricsRecordedMessage.Metrics, metricsRecordedMessage.Test);

            Logger.LogMessage(
                $"Stress test {metricsRecordedMessage.Test.DisplayName} metrics file generated at: {fileLocation}");

            return true;
        }
    }
}

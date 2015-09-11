using Xunit;
using Xunit.Abstractions;

namespace Microsoft.AspNet.StressFramework.Reporters
{
    public class StressTestRunnerCSVReporter : DefaultRunnerReporter
    {
        internal const string Command = "stresscsv";

        public override string RunnerSwitch => Command;

        public override string Description => "output stress test results to CSV files. 1 CSV file per test.";

        public override IMessageSink CreateMessageHandler(IRunnerLogger logger)
        {
            return new StressTestRunnerCSVMessageHandler(logger);
        }
    }
}

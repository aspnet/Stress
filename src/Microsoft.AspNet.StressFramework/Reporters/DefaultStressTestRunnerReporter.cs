using Xunit;
using Xunit.Abstractions;

namespace Microsoft.AspNet.StressFramework.Reporters
{
    public class DefaultStressTestRunnerReporter : DefaultRunnerReporter
    {
        public override string RunnerSwitch => "stress";

        public override string Description => "doesn't output stress test results.";

        public override IMessageSink CreateMessageHandler(IRunnerLogger logger)
        {
            return new DefaultRunnerReporterMessageHandler(new ConsoleRunnerLogger(useColors: true));
        }
    }
}

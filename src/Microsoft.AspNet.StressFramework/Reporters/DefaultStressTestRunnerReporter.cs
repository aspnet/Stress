using Xunit;
using Xunit.Abstractions;

namespace Microsoft.AspNet.StressFramework.Reporters
{
    public class DefaultStressTestRunnerReporter : DefaultRunnerReporter
    {
        internal const string Command = "stress";

        public override string RunnerSwitch => Command;

        public override string Description => "doesn't output stress test results.";

        public override IMessageSink CreateMessageHandler(IRunnerLogger logger)
        {
            return new DefaultRunnerReporterMessageHandler(logger);
        }
    }
}

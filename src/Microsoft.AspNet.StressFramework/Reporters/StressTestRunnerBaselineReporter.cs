using Xunit;
using Xunit.Abstractions;

namespace Microsoft.AspNet.StressFramework.Reporters
{
    public class StressTestRunnerBaselineReporter : DefaultRunnerReporter
    {
        internal const string Command = "stressbaseline";

        private readonly BaselineConfiguration _configuration;

        public StressTestRunnerBaselineReporter()
            : this(new BaselineConfiguration())
        {
        }

        public StressTestRunnerBaselineReporter(BaselineConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override string RunnerSwitch => Command;

        public override string Description => "compares stress run results to existing CSV stress test baselines.";

        public override IMessageSink CreateMessageHandler(IRunnerLogger logger)
        {
            return new StressTestRunnerBaselineMessageHandler(_configuration, logger);
        }
    }
}

using System;
using System.IO;
using Microsoft.Framework.Configuration;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.AspNet.StressFramework.Reporters
{
    public class StressTestRunnerConfigReporter : DefaultRunnerReporter
    {
        private const string StressConfigFileName = "stressConfig.json";
        private const string StressOutputReporterConfigName = "StressOutputReporter";
        private static readonly Lazy<IRunnerReporter> _defaultReporter = new Lazy<IRunnerReporter>(
            () => new DefaultStressTestRunnerReporter());
        private static readonly Lazy<IRunnerReporter> _csvReporter = new Lazy<IRunnerReporter>(
            () => new StressTestRunnerCSVReporter());
        private IRunnerReporter _activeReporter;

        private IRunnerReporter DefaultReporter => _defaultReporter.Value;
        private IRunnerReporter CSVReporter => _csvReporter.Value;

        public override string RunnerSwitch => "stressconfig";

        public override string Description => "determine stress output mechanism based on ";

        public override IMessageSink CreateMessageHandler(IRunnerLogger logger)
        {
            if (_activeReporter == null)
            {
                var stressConfigFile = new FileInfo(StressConfigFileName);
                if (!stressConfigFile.Exists)
                {
                    throw new InvalidOperationException(
                        $"Cannot determine stress configuration. {StressConfigFileName} does not exist.");
                }

                var config = new ConfigurationBuilder(".")
                    .AddJsonFile(StressConfigFileName)
                    .Build();
                var stressTestOutputReporterName = config[StressOutputReporterConfigName];

                if (!string.IsNullOrEmpty(stressTestOutputReporterName) ||
                    string.Equals(
                        stressTestOutputReporterName,
                        DefaultReporter.RunnerSwitch,
                        StringComparison.OrdinalIgnoreCase))
                {
                    _activeReporter = DefaultReporter;
                }
                else if (string.Equals(
                    stressTestOutputReporterName,
                    CSVReporter.RunnerSwitch,
                    StringComparison.OrdinalIgnoreCase))
                {
                    _activeReporter = CSVReporter;
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Unknown {StressOutputReporterConfigName} value in {StressConfigFileName}.");
                }
            }

            return _activeReporter.CreateMessageHandler(logger);
        }
    }
}

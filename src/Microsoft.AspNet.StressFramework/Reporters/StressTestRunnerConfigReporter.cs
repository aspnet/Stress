using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Framework.Configuration;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.AspNet.StressFramework.Reporters
{
    public class StressTestRunnerConfigReporter : DefaultRunnerReporter
    {
        private const string StressConfigFileName = "stressConfig.json";
        private const string CommandConfigName = "Command";
        private IEnumerable<IRunnerReporter> _activeReporters;

        public override string RunnerSwitch => "stressconfig";

        public override string Description => "determine stress output mechanism based on ";

        public override IMessageSink CreateMessageHandler(IRunnerLogger logger)
        {
            if (_activeReporters == null)
            {
                var reporters = new List<IRunnerReporter>();
                var stressConfigFile = new FileInfo(StressConfigFileName);
                if (!stressConfigFile.Exists)
                {
                    throw new InvalidOperationException(
                        $"Cannot determine stress configuration. {StressConfigFileName} does not exist.");
                }

                var config = new ConfigurationBuilder(".").AddJsonFile(StressConfigFileName).Build();
                var reporterBuilders = new Dictionary<string, Func<IRunnerReporter>>(StringComparer.OrdinalIgnoreCase)
                {
                    { DefaultStressTestRunnerReporter.Command, () => new DefaultStressTestRunnerReporter() },
                    { StressTestRunnerCSVReporter.Command, () => new StressTestRunnerCSVReporter() },
                    {
                        StressTestRunnerBaselineReporter.Command,
                        () =>
                        {
                            var baselineConfiguration = new BaselineConfiguration(config);
                            var baselineReporter = new StressTestRunnerBaselineReporter(baselineConfiguration);

                            return baselineReporter;
                        }
                    },
                };

                // Can expand this to support multiple, comma separated commands.
                // Ex: "stressbaseline,stresscsv" to signal that we want to run a baseline and then save the results.
                var command = config[CommandConfigName];

                Func<IRunnerReporter> reporterBuilder;
                if (string.IsNullOrEmpty(command))
                {
                    reporterBuilder = reporterBuilders[DefaultStressTestRunnerReporter.Command];
                }
                else if (!reporterBuilders.TryGetValue(command, out reporterBuilder))
                {
                    throw new InvalidOperationException(
                        $"Unknown {CommandConfigName} value in {StressConfigFileName}.");
                }

                var reporter = reporterBuilder();
                reporters.Add(reporter);

                _activeReporters = reporters;
            }

            var aggregateMessageHandler = new AggregateStressTestRunnerMessageHandler(logger);

            foreach (var reporter in _activeReporters)
            {
                var resolvedMessageHandler = reporter.CreateMessageHandler(logger);
                aggregateMessageHandler.AddMessageHandler(resolvedMessageHandler);
            }

            return aggregateMessageHandler;
        }
    }
}

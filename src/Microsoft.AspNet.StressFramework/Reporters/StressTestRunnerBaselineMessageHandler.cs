using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.StressFramework.Collectors;
using Xunit;

namespace Microsoft.AspNet.StressFramework.Reporters
{
    public class StressTestRunnerBaselineMessageHandler : StressTestRunnerMessageHandlerBase
    {
        private readonly BaselineConfiguration _configuration;

        public StressTestRunnerBaselineMessageHandler(BaselineConfiguration configuration, IRunnerLogger logger)
            : base(logger)
        {
            _configuration = configuration;
        }

        protected override bool Visit(MetricsRecordedMessage metricsRecordedMessage)
        {
            var targetMetrics = metricsRecordedMessage.Metrics;
            var baselineMetrics = StressTestOutputManager.ReadBaseline(metricsRecordedMessage.Test);

            // Could be smarter here and throw away outliers etc.
            var baselineHeapMemoryUsageAverage = AverageHeapMemoryUsage(baselineMetrics);
            var targetHeapMemoryUsageAverage = AverageHeapMemoryUsage(targetMetrics);

            var combinedHeapMemoryUsageAverage = (baselineHeapMemoryUsageAverage + targetHeapMemoryUsageAverage) / 2;
            var percentDifference =
                (Math.Abs(baselineHeapMemoryUsageAverage - targetHeapMemoryUsageAverage) / combinedHeapMemoryUsageAverage) * 100;

            if (percentDifference > _configuration.HeapMemoryFailPercentThreshold)
            {
                Logger.LogError(
$@"Current stress test run resulted in an unexpectedly different heap memory usage.
Current run usage average: {targetHeapMemoryUsageAverage} bytes
Baseline run usage average: {baselineHeapMemoryUsageAverage} bytes
% Difference: {percentDifference}%
Fail threshold: {_configuration.HeapMemoryFailPercentThreshold}%");
            }

            return base.Visit(metricsRecordedMessage);
        }

        private static double AverageHeapMemoryUsage(IEnumerable<Metric> metrics)
        {
            return metrics
                .Where(metric => metric.Value is MemoryUsage)
                .Select(metric => ((MemoryUsage)metric.Value).HeapMemoryBytes)
                .Average(heapMemoryUsage => heapMemoryUsage);
        }
    }
}

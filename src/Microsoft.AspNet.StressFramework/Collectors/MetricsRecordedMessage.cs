using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;

namespace Microsoft.AspNet.StressFramework.Collectors
{
    public class MetricsRecordedMessage : IMessageSinkMessage
    {
        private ITest Test { get; }
        private IReadOnlyList<Metric> Metrics { get; }

        public MetricsRecordedMessage(ITest test, IEnumerable<Metric> metrics)
        {
            Test = test;
            Metrics = metrics.ToList().AsReadOnly();
        }
    }
}
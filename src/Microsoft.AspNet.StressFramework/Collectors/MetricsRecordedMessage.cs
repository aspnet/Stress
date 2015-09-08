using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;

namespace Microsoft.AspNet.StressFramework.Collectors
{
    public class MetricsRecordedMessage : IMessageSinkMessage
    {
        public MetricsRecordedMessage(ITest test, IEnumerable<Metric> metrics)
        {
            Test = test;
            Metrics = metrics.ToList().AsReadOnly();
        }

        public ITest Test { get; }

        public IReadOnlyList<Metric> Metrics { get; }
    }
}
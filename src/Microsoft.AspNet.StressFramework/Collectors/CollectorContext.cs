using System.Diagnostics;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Microsoft.AspNet.StressFramework.Collectors
{
    public class CollectorContext
    {
        private readonly IMessageBus _messageBus;
        private readonly ITest _test;

        public CollectorContext(IMessageBus messageBus, ITest test)
        {
            _messageBus = messageBus;
            _test = test;
        }

        public void EmitMetric(Metric metric)
        {
            _messageBus.QueueMessage(
                new MetricsRecordedMessage(_test, new[] { metric }));
        }
    }
}
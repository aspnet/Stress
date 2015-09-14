using System.Diagnostics;
using Microsoft.AspNet.StressFramework.Tracing;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Microsoft.AspNet.StressFramework.Collectors
{
    public class CollectorContext
    {
        private readonly IMessageBus _messageBus;
        private readonly ITest _test;

        public TraceSession TraceSession { get; }

        public CollectorContext(IMessageBus messageBus, ITest test, TraceSession traceSession)
        {
            _messageBus = messageBus;
            _test = test;

            TraceSession = traceSession;
        }

        public void EmitMetric(Metric metric)
        {
            _messageBus.QueueMessage(
                new MetricsRecordedMessage(_test, new[] { metric }));
        }
    }
}
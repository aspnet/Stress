using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.AspNet.StressFramework.Reporters
{
    public class AggregateStressTestRunnerMessageHandler : StressTestRunnerMessageHandlerBase
    {
        private readonly List<IMessageSink> _messageHandlers;

        public AggregateStressTestRunnerMessageHandler(IRunnerLogger logger)
            : base(logger)
        {
            _messageHandlers = new List<IMessageSink>();
        }

        public void AddMessageHandler(IMessageSink messageHandler)
        {
            _messageHandlers.Add(messageHandler);
        }

        public override bool OnMessage(IMessageSinkMessage message)
        {
            return _messageHandlers.Aggregate(true, (current, next) => current && next.OnMessage(message));
        }
    }
}

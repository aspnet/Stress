using System;
using Microsoft.AspNet.StressFramework.Collectors;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.AspNet.StressFramework
{
    public abstract class DefaultStressTestMessageVisitor : DefaultRunnerReporterMessageHandler
    {
        public DefaultStressTestMessageVisitor(IRunnerLogger logger)
            : base(logger)
        {
        }

        public override bool OnMessage(IMessageSinkMessage message)
        {
            return DoVisit<MetricsRecordedMessage>(message, (t, m) => t.Visit(m)) &&
                base.OnMessage(message);
        }

        protected virtual bool Visit(MetricsRecordedMessage metric)
        {
            return true;
        }

        bool DoVisit<TMessage>(IMessageSinkMessage message, Func<DefaultStressTestMessageVisitor, TMessage, bool> callback)
            where TMessage : class, IMessageSinkMessage
        {
            var castMessage = message as TMessage;
            if (castMessage != null)
            {
                return callback(this, castMessage);
            }

            return true;
        }
    }
}

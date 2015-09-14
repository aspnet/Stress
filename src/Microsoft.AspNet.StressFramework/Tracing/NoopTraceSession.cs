using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNet.StressFramework.Tracing
{
    internal class NoopTraceSession : TraceSession
    {
        public static readonly NoopTraceSession Instance = new NoopTraceSession();

        private NoopTraceSession() { }

        public override void AddCallback(EventSource log, string eventName, Action<TraceEvent> callback)
        {
        }

        public override void Initialize(Process targetProcess)
        {
        }

        public override void EnableSource(EventSource log)
        {
        }

        public override void StartProcessingEvents()
        {
        }

        public override Task WaitForEventPumpAsync()
        {
            return Task.FromResult(0);
        }
    }
}

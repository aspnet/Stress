using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;
using Microsoft.AspNet.StressFramework.Collectors;

namespace Microsoft.AspNet.StressFramework.Tracing
{
    /// <summary>
    /// Abstraction over ETW and potentially something similar on *nix
    /// </summary>
    public abstract class TraceSession
    {
        public abstract void Initialize(Process targetProcess);
        public abstract void EnableSource(EventSource log);
        public abstract void AddCallback(EventSource log, string eventName, Action<TraceEvent> callback);

        public abstract void StartProcessingEvents();
        public abstract Task WaitForEventPumpAsync();
    }
}

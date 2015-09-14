#if DNX451
using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;
using Microsoft.AspNet.StressFramework.Collectors;
using Microsoft.Diagnostics.Tracing.Session;

namespace Microsoft.AspNet.StressFramework.Tracing
{
    internal class EtwTraceSession : TraceSession, IDisposable
    {
        private Process _target;
        private TraceEventSession _session;
        private Task _eventPump;

        public override void AddCallback(EventSource log, string eventName, Action<TraceEvent> callback)
        {
            _session.Source.Dynamic.AddCallbackForProviderEvent(
                log.Name,
                eventName,
                evt =>
                {
                    if (evt.ProcessID != _target.Id)
                    {
                        return;
                    }
                    callback(new EtwTraceEvent(evt));
                });
        }

        public override void Initialize(Process targetProcess)
        {
            // Process ID can be used in the session name because there should only be one session per host process
            _session = new TraceEventSession($"DnxStress_{targetProcess.Id}", TraceEventSessionOptions.Create);
            _target = targetProcess;

            // Wire up the run end event to stop listening to events
            _session.EnableProvider(StressTestEventSource.Log.Guid);
            _session.Source.Dynamic.AddCallbackForProviderEvent(
                StressTestEventSource.Log.Name,
                "Run/Stop",
                evt =>
                {
                    if (evt.ProcessID == _target.Id)
                    {
                        _session.Dispose();
                    }
                });
        }

        public override void EnableSource(EventSource log)
        {
            // We already added the main stress test event source source
            if (log.Guid != StressTestEventSource.Log.Guid)
            {
                _session.EnableProvider(log.Guid);
            }
        }

        public void Dispose()
        {
            // It is safe to call dispose multiple times.
            _session?.Dispose();
        }

        public override void StartProcessingEvents()
        {
            _eventPump = Task.Run(() => _session.Source.Process());
        }

        public override Task WaitForEventPumpAsync()
        {
            return _eventPump;
        }
    }
}
#endif
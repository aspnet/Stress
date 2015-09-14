#if DNX451
namespace Microsoft.AspNet.StressFramework.Tracing
{
    internal class EtwTraceEvent : TraceEvent
    {
        private Diagnostics.Tracing.TraceEvent _event;

        public EtwTraceEvent(Diagnostics.Tracing.TraceEvent evt)
        {
            _event = evt;
        }

        public override double TimeStampRelativeMSec => _event.TimeStampRelativeMSec;
        public override object PayloadByName(string name) => _event.PayloadByName(name);
    }
}
#endif
namespace Microsoft.AspNet.StressFramework.Tracing
{
    public abstract class TraceEvent
    {
        public abstract double TimeStampRelativeMSec { get; }
        public abstract object PayloadByName(string name);
    }
}
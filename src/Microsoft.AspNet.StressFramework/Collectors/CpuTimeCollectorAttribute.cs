using System;

namespace Microsoft.AspNet.StressFramework.Collectors
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CpuTimeCollectorAttribute : Attribute, ICollector
    {
        public void Initialize()
        {

        }

        public void BeginIteration(StressTestIterationContext iteration)
        {
            iteration.Record(CpuTime.Capture());
        }

        public void EndIteration(StressTestIterationContext iteration)
        {
            iteration.Record(CpuTime.Capture());
        }
    }
}
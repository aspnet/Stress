using System;
using System.Diagnostics;

namespace Microsoft.AspNet.StressFramework
{
    public struct CpuTime
    {
        public TimeSpan KernelTime { get; }
        public TimeSpan UserTime { get; }

        public CpuTime(TimeSpan kernelTime, TimeSpan userTime)
        {
            KernelTime = kernelTime;
            UserTime = userTime;
        }

        public static CpuTime Capture(Process process)
        {
            return new CpuTime(
                process.PrivilegedProcessorTime,
                process.UserProcessorTime);
        }

        public override string ToString()
        {
            return $"K: {KernelTime}; U: {UserTime}";
        }
    }
}
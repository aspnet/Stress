using System;
using System.Diagnostics;

namespace Microsoft.AspNet.StressFramework
{
    public class CpuTime
    {
        public TimeSpan KernelTime { get; }
        public TimeSpan UserTime { get; }

        public CpuTime(TimeSpan kernelTime, TimeSpan userTime)
        {
            KernelTime = kernelTime;
            UserTime = userTime;
        }

        public static CpuTime Capture()
        {
            var me = Process.GetCurrentProcess();
            return new CpuTime(
                me.PrivilegedProcessorTime,
                me.UserProcessorTime);
        }

        public override string ToString()
        {
            return $"K: {KernelTime}; U: {UserTime}";
        }
    }
}
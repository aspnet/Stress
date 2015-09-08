using System;
using System.Diagnostics;

namespace Microsoft.AspNet.StressFramework
{
    public class MemorySnapshot
    {
        public long HeapMemoryBytes { get; }
        public long WorkingSet { get; }
        public long PrivateBytes { get; }

        public MemorySnapshot(long heapMemoryBytes, long workingSet, long privateBytes)
        {
            HeapMemoryBytes = heapMemoryBytes;
            WorkingSet = workingSet;
            PrivateBytes = privateBytes;
        }

        public static MemorySnapshot Capture()
        {
            var me = Process.GetCurrentProcess();
            return new MemorySnapshot(
                GC.GetTotalMemory(forceFullCollection: false),
                me.WorkingSet64,
                me.PrivateMemorySize64);
        }

        public override string ToString()
        {
            return $"Heap: {HeapMemoryBytes / 1024.0:0.00}KB, WorkingSet: {WorkingSet / 1024.0:0.00}KB, Private: {PrivateBytes / 1024.0:0.00}KB";
        }
    }
}
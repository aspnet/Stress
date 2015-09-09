using System;
using System.Diagnostics;

namespace Microsoft.AspNet.StressFramework
{
    public struct MemoryUsage
    {
        public long HeapMemoryBytes { get; }
        public long WorkingSet { get; }
        public long PrivateBytes { get; }

        public MemoryUsage(long heapMemoryBytes, long workingSet, long privateBytes)
        {
            HeapMemoryBytes = heapMemoryBytes;
            WorkingSet = workingSet;
            PrivateBytes = privateBytes;
        }

        public static MemoryUsage Capture(Process process)
        {
            // Get the GC to run fully
            for(int i = 0; i < 5; i++)
            {
                GC.GetTotalMemory(forceFullCollection: true);
            }
            var heap = GC.GetTotalMemory(forceFullCollection: true);

            return new MemoryUsage(
                heap,
                process.WorkingSet64,
                process.PrivateMemorySize64);
        }

        public override string ToString()
        {
            return $"Heap: {HeapMemoryBytes / 1024.0:0.00}KB, WorkingSet: {WorkingSet / 1024.0:0.00}KB, Private: {PrivateBytes / 1024.0:0.00}KB";
        }

        public static MemoryUsage Compare(MemoryUsage start, MemoryUsage end)
        {
            return new MemoryUsage(
                end.HeapMemoryBytes - start.HeapMemoryBytes,
                end.WorkingSet - start.WorkingSet,
                end.PrivateBytes - start.PrivateBytes);
        }
    }
}
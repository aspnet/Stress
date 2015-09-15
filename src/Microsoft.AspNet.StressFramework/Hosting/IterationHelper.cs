using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.StressFramework.Collectors;

namespace Microsoft.AspNet.StressFramework.Hosting
{
    internal static class IterationHelper
    {
        public static async Task IterateAsync(StressRunSetup setup, Func<Task> action)
        {
            StressTestTrace.WriteLine("Warming up...");
            for (int i = 0; i < setup.WarmupIterations; i++)
            {
                await action();
            }
            StressTestTrace.WriteLine("Iterating...");
            StressTestEventSource.Log.RunStart();
            for (int i = 0; i < setup.Iterations; i++)
            {
                StressTestEventSource.Log.IterationStart(i);
                await action();
                StressTestEventSource.Log.IterationStop(i);
            }
            StressTestEventSource.Log.RunEnd();
            StressTestTrace.WriteLine("Host Complete");
        }
    }
}

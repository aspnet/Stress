using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.AspNet.StressFramework.Hosting
{
    public class IteratingDriver : IStressTestDriver
    {
        private Func<Task> _execute;

        public IteratingDriver(Func<Task> execute)
        {
            _execute = execute;
        }

        public async Task RunAsync(StressRunSetup setup)
        {
            // Perform the actual iterations.
            StressTestTrace.WriteLine("Warming up...");
            for(int i = 0; i < setup.WarmupIterations; i++)
            {
                await _execute();
            }
            StressTestTrace.WriteLine("Iterating...");
            for(int i = 0; i < setup.Iterations; i++)
            {
                await _execute();
            }

            // Shut down the host
            StressTestTrace.WriteLine("Shutting down host...");
            await setup.Host.ShutdownAsync();
        }
    }
}
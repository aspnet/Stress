using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNet.StressFramework.Collectors;

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
            await IterationHelper.IterateAsync(setup, _execute);

            // Shut down the host
            StressTestTrace.WriteLine("Shutting down host...");
            await setup.Host.ShutdownAsync();
        }
    }
}
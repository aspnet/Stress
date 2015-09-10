using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNet.StressFramework.Hosting
{
    public class WaitForHostDriver : IStressTestDriver
    {
        public static readonly WaitForHostDriver Instance = new WaitForHostDriver();

        private WaitForHostDriver() { }

        public async Task RunAsync(StressRunSetup setup)
        {
            // Wait for the host to shut down
            // TODO(anurse): Protect against a runaway host with a timeout or heartbeat?
            StressTestTrace.WriteLine("Waiting for host to complete...");
            await setup.Host.WaitForExitAsync();
        }
    }
}

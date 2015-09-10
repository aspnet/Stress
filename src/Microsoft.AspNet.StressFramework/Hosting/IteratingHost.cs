using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.StressFramework.Collectors;

namespace Microsoft.AspNet.StressFramework.Hosting
{
    public class IteratingHost : IStressTestHost
    {
        private StressTestHostProcess _process;
        private Func<Task> _execute;

        public IteratingHost(Func<Task> execute)
        {
            _execute = execute;
        }

        public void Start()
        {
            _process.StartExecutingHost();
        }

        public Process LaunchHost(MethodInfo method)
        {
            _process = StressTestHostProcess.Launch(method, StressTestTrace.WriteRawLine);
            return _process.Process;
        }

        public async Task RunInHostProcessAsync(StressRunSetup setup)
        {
            // Perform the actual iterations. Called by the host process ONLY
            StressTestTrace.WriteLine("Warming up...");
            for (int i = 0; i < setup.WarmupIterations; i++)
            {
                await _execute();
            }
            StressTestTrace.WriteLine("Iterating...");
            StressTestEventSource.Log.RunStart();
            for (int i = 0; i < setup.Iterations; i++)
            {
                StressTestEventSource.Log.IterationStart(i);
                await _execute();
                StressTestEventSource.Log.IterationStop(i);
            }
            StressTestEventSource.Log.RunEnd();
            StressTestTrace.WriteLine("Host Complete");
        }

        public Task WaitForExitAsync()
        {
            return _process.WaitForExitAsync();
        }

        public Task ShutdownAsync()
        {
            return _process.ShutdownAsync();
        }
    }
}
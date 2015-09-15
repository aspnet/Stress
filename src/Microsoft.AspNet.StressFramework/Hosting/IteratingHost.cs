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

        public Task StartAsync()
        {
            _process.StartExecutingHost();
            return Task.FromResult(0);
        }

        public Process LaunchHost(MethodInfo method)
        {
            _process = StressTestHostProcess.Launch(method, StressTestTrace.WriteRawLine);
            return _process.Process;
        }

        public async Task RunInHostProcessAsync(StressRunSetup setup)
        {
            // Perform the actual iterations. Called by the host process ONLY
            await IterationHelper.IterateAsync(setup, _execute);
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
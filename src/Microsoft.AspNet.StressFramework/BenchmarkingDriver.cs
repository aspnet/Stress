using System;
using System.Threading.Tasks;

namespace Microsoft.AspNet.StressFramework
{
    public class BenchmarkingDriver : IStressTestDriver, IDisposable
    {
        private StressTestHostProcess _process;

        public void Setup(StressTestDriverContext context)
        {
            StressTestTrace.WriteLine("Initializing Host");
            _process = StressTestHostProcess.Launch(context.TestMethod, StressTestTrace.WriteRawLine);
        }

        public Task RunAsync(StressTestDriverContext context)
        {
            StressTestTrace.WriteLine("Host Start");
            _process.BeginIterations();
            _process.Process.WaitForExit();
            StressTestTrace.WriteLine("Host End");

            return Task.FromResult(0);
        }

        public void Dispose()
        {
            _process?.Dispose();
        }
    }
}
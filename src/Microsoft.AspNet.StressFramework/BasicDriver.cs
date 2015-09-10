using System;
using System.Threading.Tasks;

namespace Microsoft.AspNet.StressFramework
{
    public class BasicDriver : IStressTestDriver
    {
        private readonly IStressTestHost _host;
        private readonly Func<Task> _execute;

        public BasicDriver(Func<Task> execute)
        {
            _execute = execute;
        }

        public void Setup(StressTestDriverContext context)
        {
            context.Setup.Host.Setup();
        }

        public async Task RunAsync(StressTestDriverContext context)
        {
            StressTestTrace.WriteRawLine("Begin warmup");
            for (var i = 0; i < context.Setup.WarmupIterations; i++)
            {
                await _execute();
            }

            StressTestTrace.WriteRawLine("End warmup");

            for (var i = 0; i < context.Setup.Iterations; i++)
            {
                await _execute();
            }
        }
    }
}
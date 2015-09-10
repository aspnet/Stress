using System;
using System.Threading.Tasks;

namespace Microsoft.AspNet.StressFramework
{
    public class BasicHost : IStressTestHost
    {
        private Func<Task> _execute;

        public BasicHost(Func<Task> execute)
        {
            _execute = execute;
        }

        public void Run(StressTestHostContext context)
        {
            Console.WriteLine("Executing host.");

            for (var i = 0; i < context.WarmupIterations; i++)
            {
                _execute();
            }

            for (var i = 0; i < context.Iterations; i++)
            {
                _execute();
            }

            Console.WriteLine("Done executing host.");
        }

        public void Setup()
        {
        }
    }
}
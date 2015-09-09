using System;
using System.Threading.Tasks;

namespace Microsoft.AspNet.StressFramework
{
    internal class BasicHost : IStressTestHost
    {
        private Func<Task> _execute;
        private Action _setup;

        public BasicHost(Action setup, Func<Task> execute)
        {
            _setup = setup;
            _execute = execute;
        }

        public void Run(StressTestHostContext context)
        {
            Console.WriteLine("Executing host.");

            for (var i = 0; i < context.Iterations; i++)
            {
                _execute();
            }

            Console.WriteLine("Done executing host.");
        }

        public void Setup(StressTestHostContext context)
        {
            _setup();

            for (var i = 0; i < context.WarmupIterations; i++)
            {
                _execute();
            }
        }
    }
}
using System;

namespace Microsoft.AspNet.StressFramework
{
    public class BasicDriver : IStressTestDriver
    {
        private readonly Action _execute;
        private readonly Action _setup;

        public BasicDriver(Action setup, Action execute)
        {
            _setup = setup;
            _execute = execute;
        }

        public void Setup() => _setup();

        public void Run(int driverIterations)
        {
            Console.WriteLine("Executing driver.");
            for (var i = 0; i < driverIterations; i++)
            {
                _execute();
            }
            Console.WriteLine("Done executing driver.");
        }
    }
}
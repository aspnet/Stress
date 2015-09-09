using System;
using System.Threading.Tasks;

namespace Microsoft.AspNet.StressFramework
{
    public class StressRunSetup
    {
        public IStressTestHost Host { get; set; }

        public IStressTestDriver Driver { get; set; }

        public int WarmupIterations { get; set; } = 1;

        public int HostIterations { get; set; } = 0;

        public int DriverIterations { get; set; } = 10;

        public static StressRunSetup CreateTest(Action setup, Func<Task> execute)
        {
            var host = new BasicHost(setup, execute);
            var driver = new BasicDriver(
                setup: () => { },
                execute: () => { });

            return new StressRunSetup
            {
                Host = host,
                Driver = driver,
                DriverIterations = 0,
                HostIterations = 10
            };
        }
    }
}

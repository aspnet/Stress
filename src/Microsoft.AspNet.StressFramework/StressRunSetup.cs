using System;
using System.Threading.Tasks;

namespace Microsoft.AspNet.StressFramework
{
    public class StressRunSetup
    {
        public IStressTestHost Host { get; set; }

        public IStressTestDriver Driver { get; set; }

        public int WarmupIterations { get; set; } = 1;

        public int Iterations { get; set; } = 50;

        public static StressRunSetup CreateTest(Func<Task> execute)
        {
            var host = new BasicHost(execute);
            var driver = new BenchmarkingDriver();

            return new StressRunSetup
            {
                Host = host,
                Driver = driver,
            };
        }

        public static StressRunSetup CreateClientServerTest(string applicationPath, Func<Task> execute)
        {
            var host = new KestrelHost(applicationPath);
            var driver = new BasicDriver(execute);

            return new StressRunSetup
            {
                Host = host,
                Driver = driver,
            };
        }
    }
}

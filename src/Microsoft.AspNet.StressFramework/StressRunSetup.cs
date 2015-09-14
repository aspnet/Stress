using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.StressFramework.Collectors;
using Microsoft.AspNet.StressFramework.Hosting;

namespace Microsoft.AspNet.StressFramework
{
    public class StressRunSetup  : IDisposable
    {
        private IEnumerable<ICollector> _collectors;
        private CollectorContext _collectorContext;

        public IStressTestHost Host { get; set; }

        public IStressTestDriver Driver { get; set; }

        public int WarmupIterations { get; set; } = 1;

        public int Iterations { get; set; } = 50;

        public static StressRunSetup CreateTest(Func<Task> execute)
        {
            var host = new IteratingHost(execute);
            var driver = WaitForHostDriver.Instance;

            return new StressRunSetup
            {
                Host = host,
                Driver = driver,
            };
        }

        public static StressRunSetup CreateClientServerTest(string applicationPath, Func<Task> execute)
        {
            var host = new KestrelHost(applicationPath);
            var driver = new IteratingDriver(execute);

            return new StressRunSetup
            {
                Host = host,
                Driver = driver,
            };
        }

        public void Setup(MethodInfo method, IEnumerable<ICollector> collectors, CollectorContext context)
        {
            _collectors = collectors;
            _collectorContext = context;

            StressTestTrace.WriteLine("Launching Host");
            var hostProcess = Host.LaunchHost(method);

            // Initialize Tracing
            _collectorContext.TraceSession.Initialize(hostProcess);

            // Configure collectors
            foreach(var collector in _collectors)
            {
                collector.Initialize(hostProcess, _collectorContext);
            }

            // Start tracing
            _collectorContext.TraceSession.StartProcessingEvents();
        }

        public async Task RunAsync()
        {
            StressTestTrace.WriteLine("Releasing Host");
            Host.Start();

            StressTestTrace.WriteLine("Running Driver");
            await Driver.RunAsync(this);
            StressTestTrace.WriteLine("Driver Complete");

            // Shut down tracing
            // TODO(anurse): Forcibly terminate event pumps that don't shut down in a timely manner?
            await _collectorContext.TraceSession.WaitForEventPumpAsync();

            // Shut down collectors
            // TODO(anurse): Forcibly terminate collectors that don't shut down in a timely manner?
            await Task.WhenAll(_collectors.Select(c => c.StopAsync()));

            return;
        }

        public void Dispose()
        {
            (Host as IDisposable)?.Dispose();
            (Driver as IDisposable)?.Dispose();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.AspNet.StressFramework.Collectors
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class IterationElapsedTimeCollectorAttribute : Attribute, ICollector
    {
        private Dictionary<int, double> _startTimes = new Dictionary<int, double>();
        private Task _eventPump;

        public void Initialize(Process hostProcess, CollectorContext context)
        {
            context.TraceSession.EnableSource(StressTestEventSource.Log);

            // Set up capture
            context.TraceSession.AddCallback(
                StressTestEventSource.Log,
                "Iteration/Start",
                evt =>
                {
                    var iterationId = (int)evt.PayloadByName("iterationNumber");
                    _startTimes[iterationId] = evt.TimeStampRelativeMSec;
                });

            context.TraceSession.AddCallback(
                StressTestEventSource.Log,
                "Iteration/Stop",
                evt =>
                {
                    var iterationId = (int)evt.PayloadByName("iterationNumber");
                    double startTime;
                    if (_startTimes.TryGetValue(iterationId, out startTime))
                    {
                        // Create an elapsed time metric
                        var elapsed = evt.TimeStampRelativeMSec - startTime;
                        var metric = Metric.Create(iterationId, new ElapsedTime(TimeSpan.FromMilliseconds(elapsed)));
                        
                        // For testing
                        //Console.WriteLine($"Iteration {iterationId} completed in {elapsed:0.00}ms");

                        // Emit the metric
                        context.EmitMetric(metric);
                    }
                });
        }

        public Task StopAsync()
        {
            // Trace session will shut itself down
            return Task.FromResult(0);
        }
    }
}

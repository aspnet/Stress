using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Session;

namespace Microsoft.AspNet.StressFramework.Collectors
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class IterationElapsedTimeCollectorAttribute : Attribute, ICollector, IDisposable
    {
        private TraceEventSession _session;

        private Dictionary<int, double> _startTimes = new Dictionary<int, double>();
        private Task _eventPump;

        public void Initialize(Process hostProcess, CollectorContext context)
        {
            // Create a trace session for the process
            _session = new TraceEventSession("DnxStress_IterationTime", TraceEventSessionOptions.Create);

            // Enable the StressTestEventSource
            _session.EnableProvider(StressTestEventSource.Log.Guid);

            // Set up capture
            _session.Source.Dynamic.AddCallbackForProviderEvent(
                StressTestEventSource.Log.Name,
                "Iteration/Start",
                evt =>
                {
                    if (evt.ProcessID != hostProcess.Id)
                    {
                        return;
                    }

                    var iterationId = (int)evt.PayloadByName("iterationNumber");
                    _startTimes[iterationId] = evt.TimeStampRelativeMSec;
                });

            _session.Source.Dynamic.AddCallbackForProviderEvent(
                StressTestEventSource.Log.Name,
                "Iteration/Stop",
                evt =>
                {
                    if (evt.ProcessID != hostProcess.Id)
                    {
                        return;
                    }

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

            _session.Source.Dynamic.AddCallbackForProviderEvent(
                StressTestEventSource.Log.Name,
                "Run/Stop",
                evt =>
                {
                    if (evt.ProcessID != hostProcess.Id)
                    {
                        return;
                    }

                    _session.Dispose();
                });

            // Begin pumping events
            _eventPump = Task.Run(() =>
            {
                _session.Source.Process();
            });
        }

        public Task StopAsync()
        {
            // Wait for the event pump to shut down
            return _eventPump;
        }

        public void Dispose()
        {
            _session?.Dispose();
        }
    }
}
